using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using EmrValidation.Core.GiamDinh;
using EmrValidation.Core.Models;
using EmrValidation.Excel;
using EmrValidation.Rules;

namespace EmrValidation.App
{
    /// <summary>
    /// Công cụ validate và giám định XML BHYT (Console). Chạy 8 bước như main.py:
    ///   1. Validate dữ liệu XML            2. Giám định thanh toán
    ///   3. Thuốc theo danh mục trúng thầu  4. Ngày giường nội trú
    ///   5. Sàng lọc mã đối tượng KCB       6. MA_MAY DVKT
    ///   7. VTYT kèm DVKT                   8. DVKT kèm ICD_REQ
    /// Nguồn file: [1] PATH_CHECK_XML.txt, [2] PATH_CHECK_XML_BQP.txt (5 dòng), [3] nhập tay.
    /// </summary>
    internal static class Program
    {
        /// <summary>Người dùng hủy chương trình (tương đương exit() trong bản Python).</summary>
        private sealed class UserAbort : Exception { }

        private static readonly string BaseDir = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly string PathTxtBhyt = Path.Combine(BaseDir, "PATH_CHECK_XML.txt");
        private static readonly string PathTxtBqp = Path.Combine(BaseDir, "PATH_CHECK_XML_BQP.txt");

        // Đường dẫn các file theo nguồn đã chọn
        private static bool _tuFileTxt;                 // true khi đọc đường dẫn từ file txt (option 1/2)
        private static string _danhMucFile = "";        // DM thuốc
        private static string _danhMucMayTxt = "";      // DM DVKT cần MA_MAY
        private static string _danhMucVtytTxt = "";     // DM VTYT kèm DVKT
        private static string _danhMucIcdTxt = "";      // DM DVKT kèm ICD_REQ
        private static string _excelFile = "";          // file Excel BHYT

        private static int Main(string[] args)
        {
            Console.OutputEncoding = new UTF8Encoding(false);   // UTF-8 không BOM
            try
            {
                return Run(args);
            }
            catch (UserAbort)
            {
                return 1;
            }
        }

        // ===========================================================================
        // Nhập liệu
        // ===========================================================================
        private static string ReadLine(string prompt)
        {
            Console.Write(prompt);
            string s = Console.ReadLine();
            if (s == null)
            {
                Console.WriteLine("\nĐã hủy chương trình.");
                throw new UserAbort();
            }
            return s;
        }

        /// <summary>
        /// Hỏi đường dẫn file cho tới khi tồn tại.
        /// required=false: Enter để bỏ qua; nhập sai thì hỏi có bỏ qua không.
        /// </summary>
        private static string PromptPath(string label, bool required = true)
        {
            while (true)
            {
                string p = ReadLine(label + ": ").Trim().Trim('"');
                if (p.Length == 0 && !required) return "";
                if (File.Exists(p) || Directory.Exists(p)) return p;

                Console.WriteLine("❌ File không tồn tại!");
                if (!required)
                {
                    string choice = ReadLine("Bỏ qua và tiếp tục không dùng danh mục? (Y/N): ").Trim().ToLowerInvariant();
                    if (choice == "y") return "";
                }
                else
                {
                    string choice = ReadLine("Nhập lại? (Y/N): ").Trim().ToLowerInvariant();
                    if (choice != "y")
                    {
                        Console.WriteLine("Đã hủy chương trình.");
                        throw new UserAbort();
                    }
                }
            }
        }

        /// <summary>
        /// Đường dẫn danh mục cho các luồng phụ (MA_MAY, VTYT, ICD):
        /// - Có trong file txt (option 1/2) → dùng luôn
        /// - Option 1/2 nhưng dòng txt để trống → bỏ qua, không hỏi
        /// - Option 3 (nhập tay) → hỏi tay, Enter để bỏ qua
        /// </summary>
        private static string ChonDanhMuc(string pathTxt, string label)
        {
            if (pathTxt.Length > 0) return pathTxt;
            if (_tuFileTxt) return "";
            return PromptPath(label, false);
        }

        /// <summary>
        /// Đọc file txt, theo VỊ TRÍ dòng (không bỏ dòng trống):
        /// dòng 1 DM thuốc, dòng 2 DM DVKT MA_MAY, dòng 3 DM VTYT kèm DVKT,
        /// dòng 4 DM DVKT kèm ICD_REQ, dòng 5 file Excel BHYT (bắt buộc).
        /// Dòng 1-4 để trống nếu không dùng (giữ nguyên vị trí dòng, không xóa dòng).
        /// </summary>
        private static void DocFileTxt(string pathTxt)
        {
            string[] raw;
            try
            {
                raw = File.ReadAllLines(pathTxt, new UTF8Encoding(true, true));
            }
            catch (DecoderFallbackException)
            {
                raw = File.ReadAllLines(pathTxt, Encoding.GetEncoding(1252));
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("❌ Không tìm thấy file: " + pathTxt);
                throw new UserAbort();
            }

            var lines = raw.Select(l => l.Trim().Trim('"').Trim()).ToList();
            while (lines.Count < 5) lines.Add("");

            _danhMucFile = lines[0];
            _danhMucMayTxt = lines[1];
            _danhMucVtytTxt = lines[2];
            _danhMucIcdTxt = lines[3];
            _excelFile = lines[4];

            if (_excelFile.Length == 0)
            {
                Console.WriteLine("❌ File txt thiếu đường dẫn file Excel BHYT ở dòng 5: " + pathTxt);
                Console.WriteLine("   Định dạng: dòng 1 DM thuốc, dòng 2 DM MA_MAY, dòng 3 DM VTYT kèm DVKT,");
                Console.WriteLine("              dòng 4 DM DVKT kèm ICD_REQ, dòng 5 file Excel BHYT.");
                throw new UserAbort();
            }

            var thieu = new List<string>();
            Func<string, bool> missing = p => p.Length > 0 && !File.Exists(p) && !Directory.Exists(p);
            if (missing(_danhMucFile)) thieu.Add("   Danh mục thuốc      : " + _danhMucFile);
            if (missing(_danhMucMayTxt)) thieu.Add("   Danh mục MA_MAY     : " + _danhMucMayTxt);
            if (missing(_danhMucVtytTxt)) thieu.Add("   Danh mục VTYT-DVKT  : " + _danhMucVtytTxt);
            if (missing(_danhMucIcdTxt)) thieu.Add("   Danh mục DVKT-ICD   : " + _danhMucIcdTxt);
            if (!File.Exists(_excelFile) && !Directory.Exists(_excelFile)) thieu.Add("   Excel BHYT          : " + _excelFile);
            if (thieu.Count > 0)
            {
                Console.WriteLine("❌ File không tồn tại:\n" + string.Join("\n", thieu));
                throw new UserAbort();
            }

            _tuFileTxt = true;
            Console.WriteLine("  Danh mục : " + _danhMucFile);
            Console.WriteLine("  DM VTYT  : " + (_danhMucVtytTxt.Length > 0 ? _danhMucVtytTxt : "(bỏ qua)"));
            Console.WriteLine("  DM ICD   : " + (_danhMucIcdTxt.Length > 0 ? _danhMucIcdTxt : "(bỏ qua)"));
            Console.WriteLine("  Excel    : " + _excelFile);
        }

        // ===========================================================================
        // Chương trình chính
        // ===========================================================================
        private static int Run(string[] args)
        {
            if (args.Length > 0)
            {
                // Chạy nhanh: EMRV.exe "file_bhyt.xlsx" → bỏ qua menu, không dùng file danh mục
                _excelFile = args[0].Trim().Trim('"');
                if (!File.Exists(_excelFile))
                {
                    Console.WriteLine("❌ File không tồn tại: " + _excelFile);
                    return 1;
                }
            }
            else
            {
                ChonNguonFile();
            }

            string excelFile = _excelFile;

            // ============================================================
            // 1. Đọc + validate dữ liệu
            // ============================================================
            Header("1. VALIDATE DỮ LIỆU XML");
            ExcelReadResult data = ExcelService.ReadExcel(excelFile);
            IDictionary<string, List<BhytBase>> allObjects = data.AllObjects;
            Console.WriteLine("\nĐọc dữ liệu hoàn tất");

            ValidationReport report = ValidationReport.Build(allObjects);
            //ReportWriter.PrintErrorsConsole(report);

            string summaryLog = ReportWriter.WriteLogSummary(excelFile, report);
            string jsonLog = ReportWriter.WriteLogJson(excelFile, report, "errors");
            string errorsOnlyLog = ReportWriter.WriteLogErrorsOnly(excelFile, report);
            string excelSummary = ReportWriter.WriteExcelSummary(excelFile, report);
            string excelErrors = ReportWriter.WriteExcelErrorsOnly(excelFile, report);

            //Console.WriteLine("\n✅ Log tổng hợp      : " + summaryLog);
            //Console.WriteLine("✅ Log JSON          : " + jsonLog);
            //Console.WriteLine("✅ Log lỗi           : " + errorsOnlyLog);
            //Console.WriteLine("✅ Excel tổng hợp    : " + excelSummary);
            //Console.WriteLine("✅ Excel lỗi         : " + excelErrors);

            // ============================================================
            // 2. Giám định thanh toán
            // ============================================================
            Header("2. GIÁM ĐỊNH THANH TOÁN BHYT");
            List<GiamDinhError> giamdinhErrors = GiamDinhService.Run(allObjects, Console.WriteLine);

            if (giamdinhErrors.Count > 0)
            {
                Console.WriteLine("\n⚠️  Phát hiện " + giamdinhErrors.Count + " trường hợp cần xem xét:");
                foreach (KeyValuePair<string, int> kv in CountByCode(giamdinhErrors))
                    Console.WriteLine("   " + (kv.Key.Length > 0 ? kv.Key : "(không có mã)").PadRight(30) + ": " + kv.Value);
            }
            else
            {
                Console.WriteLine("\n✓ Không phát hiện trường hợp vi phạm quy định thanh toán.");
            }
            Console.WriteLine("\n✅ Excel giám định   : " + GiamDinhService.WriteExcelGiamDinh(excelFile, giamdinhErrors));

            // ============================================================
            // 3. Kiểm tra thuốc theo danh mục (nếu có file danh mục)
            // ============================================================
            Header("3. KIỂM TRA THUỐC THEO DANH MỤC TRÚNG THẦU");
            if (_danhMucFile.Length == 0)
            {
                Console.WriteLine("⏭  Bỏ qua (không có file danh mục).");
            }
            else
            {
                try
                {
                    Console.WriteLine("Đang nạp danh mục: " + _danhMucFile + " ...");
                    var loader = new DanhMucThuocLoader(_danhMucFile);
                    Console.WriteLine("  → " + loader);

                    List<BhytBase> xml2;
                    int nXml2 = allObjects.TryGetValue("XML2", out xml2) ? xml2.Count : 0;
                    Console.WriteLine("  → XML2: " + nXml2 + " dòng");

                    if (nXml2 == 0)
                    {
                        Console.WriteLine("⏭  Không có dữ liệu XML2.");
                    }
                    else
                    {
                        List<GiamDinhError> dmErrors = new KiemTraThuocDanhMuc(loader).Check(allObjects);

                        var maMoTa = new Dictionary<string, string>
                        {
                            { "WARN.DM01", "MA_THUOC không có trong danh mục" },
                            { "WARN.DM02", "SO_DANG_KY không khớp" },
                            { "WARN.DM03", "DON_GIA vượt DON_GIA_BH" },
                            { "WARN.DM04", "DUONG_DUNG không khớp" },
                            { "WARN.DM05", "HAM_LUONG không khớp" },
                            { "WARN.DM06", "Thuốc hết hiệu lực tại ngày y lệnh" },
                            { "WARN.DM07", "Thuốc được kê không có mã ICD tương ứng" },
                            { "WARN.DM08", "Thuốc chống chỉ định với mã ICD của hồ sơ" },
                        };

                        if (dmErrors.Count > 0)
                        {
                            Console.WriteLine("\n⚠️  Phát hiện " + dmErrors.Count + " dòng cần xem xét:");
                            foreach (KeyValuePair<string, int> kv in CountByCode(dmErrors))
                            {
                                string mt;
                                if (!maMoTa.TryGetValue(kv.Key, out mt)) mt = "";
                                Console.WriteLine("   " + kv.Key.PadRight(15) + " (" + mt + "): " + kv.Value + " dòng");
                            }
                        }
                        else
                        {
                            Console.WriteLine("\n✓ Thuốc khớp hoàn toàn với danh mục trúng thầu.");
                        }

                        string dmExcel = KiemTraThuocDanhMuc.WriteExcel(excelFile, dmErrors, _danhMucFile, loader);
                        Console.WriteLine("\n✅ Excel danh mục    : " + dmExcel);
                    }
                }
                catch (Exception e) when (e is FileNotFoundException || e is CatalogFormatException || e is IOException)
                {
                    Console.WriteLine("❌ Bỏ qua bước 3: " + e.Message);
                }
            }

            // ============================================================
            // 4. Kiểm tra ngày giường nội trú (MA_LOAI_KCB=03)
            // ============================================================
            Header("4. KIỂM TRA NGÀY GIƯỜNG NỘI TRÚ");
            List<GiamDinhError> ggErrors = new KiemTraNgayGiuong().Check(allObjects);
            if (ggErrors.Count > 0)
            {
                Dictionary<string, int> cnt = CountByCode(ggErrors).ToDictionary(k => k.Key, k => k.Value);
                Console.WriteLine("\n⚠️  Phát hiện " + ggErrors.Count + " trường hợp:");
                Console.WriteLine("   WARN.GG01 (thiếu/thừa dịch vụ giường) : " + Get(cnt, "WARN.GG01"));
                Console.WriteLine("   WARN.GG02 (trùng giường cùng ngày)    : " + Get(cnt, "WARN.GG02"));
            }
            else
            {
                Console.WriteLine("\n✓ Không phát hiện lỗi ngày giường.");
            }
            Console.WriteLine("\n✅ Excel ngày giường : " + KiemTraNgayGiuong.WriteExcel(excelFile, ggErrors));

            // ============================================================
            // 5. Sàng lọc mã đối tượng KCB
            // ============================================================
            Header("5. SÀNG LỌC MÃ ĐỐI TƯỢNG KCB");
            List<GiamDinhError> dtErrors = new SangLocDoiTuong().Check(allObjects);
            if (dtErrors.Count > 0)
            {
                Console.WriteLine("\n⚠️  Phát hiện " + dtErrors.Count + " trường hợp:");
                var maTen = new Dictionary<string, string>
                {
                    { "WARN.DT01", "1.14 ≠ ngoại trú" },
                    { "WARN.DT02", "1.15 ≠ nội trú" },
                    { "WARN.DT03", "DKBD=74021 nhưng đối tượng sai" },
                    { "WARN.DT04", "DKBD≠74021 nhưng đối tượng/giấy CT sai" },
                    { "WARN.DT05", "1.7 sơ sinh vượt 5 ngày" },
                };
                foreach (KeyValuePair<string, int> kv in CountByCode(dtErrors))
                {
                    string ten;
                    if (!maTen.TryGetValue(kv.Key, out ten)) ten = "";
                    Console.WriteLine("   " + kv.Key.PadRight(12) + " (" + ten + "): " + kv.Value);
                }
            }
            else
            {
                Console.WriteLine("\n✓ Không phát hiện vi phạm mã đối tượng KCB.");
            }
            Console.WriteLine("\n✅ Excel đối tượng  : " + SangLocDoiTuong.WriteExcel(excelFile, dtErrors));

            // ============================================================
            // 6. MA_MAY theo danh mục DVKT yêu cầu khai báo máy
            // ============================================================
            Header("6. KIỂM TRA MA_MAY DVKT");
            RunCatalogStep(6, ChonDanhMuc(_danhMucMayTxt, "Nhập file danh mục DVKT cần MA_MAY (Enter để bỏ qua)"),
                "Bỏ qua.", path =>
                {
                    var loader = new DanhMucMaMayLoader(path);
                    Console.WriteLine("  → " + loader);
                    List<GiamDinhError> errs = new KiemTraMaMay(loader).Check(allObjects);
                    Console.WriteLine(errs.Count > 0
                        ? "\n⚠️  Phát hiện " + errs.Count + " dòng DVKT thiếu MA_MAY."
                        : "\n✓ Tất cả DVKT yêu cầu MA_MAY đều đã khai báo.");
                    Console.WriteLine("\n✅ Excel MA_MAY       : " + KiemTraMaMay.WriteExcel(excelFile, errs));
                });

            // ============================================================
            // 7. VTYT kèm DVKT (XML3: MA_VAT_TU ↔ MA_DICH_VU)
            // ============================================================
            Header("7. KIỂM TRA VTYT KÈM DVKT");
            RunCatalogStep(7, ChonDanhMuc(_danhMucVtytTxt, "Nhập file danh mục VTYT kèm DVKT (Enter để bỏ qua)"),
                "Bỏ qua (không có file danh mục VTYT kèm DVKT).", path =>
                {
                    var loader = new DanhMucVTYTLoader(path);
                    Console.WriteLine("  → " + loader);
                    List<GiamDinhError> errs = new KiemTraVTYTKemDVKT(loader).Check(allObjects);
                    Console.WriteLine(errs.Count > 0
                        ? "\n⚠️  Phát hiện " + errs.Count + " dòng VTYT thiếu/sai DVKT kèm theo (WARN.VT01)."
                        : "\n✓ Tất cả VTYT thuộc danh mục đều có DVKT kèm theo hợp lệ.");
                    Console.WriteLine("\n✅ Excel VTYT kèm DVKT : " + KiemTraVTYTKemDVKT.WriteExcel(excelFile, errs));
                });

            // ============================================================
            // 8. DVKT kèm ICD_REQ (XML3 ↔ XML1: MA_BENH_CHINH, MA_BENH_KT)
            // ============================================================
            Header("8. KIỂM TRA DVKT KÈM ICD_REQ");
            RunCatalogStep(8, ChonDanhMuc(_danhMucIcdTxt, "Nhập file danh mục DVKT kèm ICD_REQ (Enter để bỏ qua)"),
                "Bỏ qua (không có file danh mục DVKT kèm ICD_REQ).", path =>
                {
                    var loader = new DanhMucDVKTICDLoader(path);
                    Console.WriteLine("  → " + loader);
                    List<GiamDinhError> errs = new KiemTraDVKTKemICD(loader).Check(allObjects);
                    if (errs.Count > 0)
                    {
                        Console.WriteLine("\n⚠️  Phát hiện " + errs.Count + " dòng cần xem xét:");
                        Console.WriteLine("   WARN.DV01 (không khớp ICD_REQ)            : " + errs.Count(e => e.MaLyDo == "WARN.DV01"));
                        Console.WriteLine("   WARN.DV02 (chống chỉ định theo ICD_NEG)   : " + errs.Count(e => e.MaLyDo == "WARN.DV02"));
                    }
                    else
                    {
                        Console.WriteLine("\n✓ Tất cả DVKT thuộc danh mục đều phù hợp với ICD_REQ và ICD_NEG.");
                    }
                    Console.WriteLine("\n✅ Excel DVKT kèm ICD  : " + KiemTraDVKTKemICD.WriteExcel(excelFile, errs));
                });

            // ============================================================
            Header("HOÀN TẤT");
            for (int i = 5; i > 0; i--)
            {
                Console.WriteLine("Đóng sau " + i + " giây...");
                Thread.Sleep(1000);
            }
            Console.WriteLine("Tạm biệt!");
            return 0;
        }

        /// <summary>Menu chọn nguồn file: [1] BHYT thường, [2] Quân đội/QP, [3] nhập tay.</summary>
        private static void ChonNguonFile()
        {
            Console.WriteLine(new string('=', 60));
            Console.WriteLine("  CÔNG CỤ VALIDATE & GIÁM ĐỊNH XML BHYT");
            Console.WriteLine(new string('=', 60));
            Console.WriteLine();
            Console.WriteLine("  Chọn nguồn file:");
            Console.WriteLine("  [1] BHYT thường        (đọc từ PATH_CHECK_XML.txt)");
            Console.WriteLine("  [2] BHYT Quân đội/QP  (đọc từ PATH_CHECK_XML_BQP.txt)");
            Console.WriteLine("  [3] Nhập tay");
            Console.WriteLine();

            while (true)
            {
                string chon = ReadLine("  Lựa chọn (1/2/3): ").Trim();
                if (chon == "1")
                {
                    Console.WriteLine("\n  Đọc từ: " + PathTxtBhyt);
                    DocFileTxt(PathTxtBhyt);
                    return;
                }
                if (chon == "2")
                {
                    Console.WriteLine("\n  Đọc từ: " + PathTxtBqp);
                    DocFileTxt(PathTxtBqp);
                    return;
                }
                if (chon == "3")
                {
                    _excelFile = PromptPath("Nhập file Excel BHYT");
                    _danhMucFile = PromptPath("Nhập file danh mục thuốc (Enter để bỏ qua)", false);
                    return;
                }
                Console.WriteLine("  ❌ Vui lòng nhập 1, 2 hoặc 3.");
            }
        }

        /// <summary>
        /// Chạy một bước dùng file danh mục: bỏ qua nếu không có file; lỗi nạp danh mục
        /// (thiếu file, thiếu cột) chỉ làm bỏ qua bước đó, không làm dừng các bước khác.
        /// </summary>
        private static void RunCatalogStep(int step, string path, string skipMessage, Action<string> run)
        {
            if (path.Length == 0)
            {
                Console.WriteLine("⏭  " + skipMessage);
                return;
            }
            try
            {
                Console.WriteLine("Đang nạp danh mục: " + path + " ...");
                run(path);
            }
            catch (Exception e) when (e is FileNotFoundException || e is CatalogFormatException || e is IOException)
            {
                Console.WriteLine("❌ Bỏ qua bước " + step + ": " + e.Message);
            }
        }

        private static void Header(string title)
        {
            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("  " + title);
            Console.WriteLine(new string('=', 60));
        }

        /// <summary>Đếm lỗi theo mã lý do, sắp xếp theo mã (giống sorted(Counter.items()) của Python).</summary>
        private static List<KeyValuePair<string, int>> CountByCode(IEnumerable<GiamDinhError> errors)
        {
            return errors.GroupBy(e => e.MaLyDo ?? "")
                         .Select(g => new KeyValuePair<string, int>(g.Key, g.Count()))
                         .OrderBy(kv => kv.Key, StringComparer.Ordinal)
                         .ToList();
        }

        private static int Get(Dictionary<string, int> d, string key)
        {
            int v;
            return d.TryGetValue(key, out v) ? v : 0;
        }
    }
}