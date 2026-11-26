using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EmrValidation.Core.GiamDinh;
using EmrValidation.Core.Models;
using EmrValidation.Excel;

namespace EmrValidation.Rules
{
    /// <summary>Lỗi của luồng DVKT kèm ICD (ICD_REQ, ICD_NEG), có thêm thông tin để xuất Excel.</summary>
    public class DvktIcdError : GiamDinhError
    {
        public string MaKtTt23 { get; set; }
        public string TenDvktTt23 { get; set; }
        public string TenDvktPdg { get; set; }
        public string MaBenhChinh { get; set; }
        public string MaBenhKt { get; set; }
        public string IcdReq { get; set; }
        /// <summary>ICD chống chỉ định của DVKT (chỉ có ở lỗi WARN.DV02).</summary>
        public string IcdNeg { get; set; }
        /// <summary>Các mã bệnh của hồ sơ trùng ICD_NEG (chỉ có ở lỗi WARN.DV02).</summary>
        public string IcdTrung { get; set; }

        public DvktIcdError()
        {
            MaKtTt23 = ""; TenDvktTt23 = ""; TenDvktPdg = ""; MaBenhChinh = ""; MaBenhKt = ""; IcdReq = "";
            IcdNeg = ""; IcdTrung = "";
        }
    }

    /// <summary>Thông tin một DVKT trong danh mục DVKT kèm ICD_REQ.</summary>
    public sealed class DvktIcdInfo
    {
        /// <summary>Tập ICD yêu cầu (đã trim, viết hoa; gộp mọi dòng cùng mã).</summary>
        public HashSet<string> Icd { get; private set; }
        /// <summary>Tập ICD chống chỉ định (cột ICD_NEG, tùy chọn; gộp mọi dòng cùng mã).</summary>
        public HashSet<string> IcdNeg { get; private set; }
        public string MaKtTt23 { get; set; }
        public string TenTt23 { get; set; }
        public string TenPdg { get; set; }

        public DvktIcdInfo()
        {
            Icd = new HashSet<string>();
            IcdNeg = new HashSet<string>();
            MaKtTt23 = ""; TenTt23 = ""; TenPdg = "";
        }
    }

    /// <summary>
    /// Load file Excel danh mục DVKT kèm ICD_REQ (sheet đầu tiên).
    /// Cột bắt buộc: MA_TUONG_DUONG_7603 (có thể nhiều mã ngăn cách ";"), ICD_REQ (mảng ngăn cách ";").
    /// Cột tùy chọn: ICD_NEG (ICD chống chỉ định, mảng ngăn cách ";"), MA_KT_TT23, TEN_DVKT_TT23, TEN_DVKT_PDG.
    /// Tên cột so khớp chính xác.
    /// </summary>
    public class DanhMucDVKTICDLoader
    {
        private static readonly string[] CotBatBuoc = { "MA_TUONG_DUONG_7603", "ICD_REQ" };

        private readonly Dictionary<string, DvktIcdInfo> _catalog = new Dictionary<string, DvktIcdInfo>();

        public string FilePath { get; private set; }

        public DanhMucDVKTICDLoader(string filepath)
        {
            if (!File.Exists(filepath))
                throw new FileNotFoundException("Không tìm thấy file danh mục: " + filepath);
            FilePath = filepath;
            Load(ExcelTableReader.ReadFirstSheet(filepath));
        }

        /// <summary>Dựng từ bảng đã đọc sẵn (dùng khi kiểm thử).</summary>
        public DanhMucDVKTICDLoader(ExcelTable table, string label)
        {
            FilePath = label;
            Load(table);
        }

        /// <summary>Tách chuỗi mã ngăn cách ';' → tập, trim từng mã, bỏ mã rỗng.</summary>
        internal static HashSet<string> TachMa(string chuoi, bool upper)
        {
            var kq = new HashSet<string>();
            foreach (string x in (chuoi ?? "").Split(';'))
            {
                string t = x.Trim();
                if (t.Length > 0) kq.Add(upper ? t.ToUpperInvariant() : t);
            }
            return kq;
        }

        private void Load(ExcelTable table)
        {
            List<string> thieu = CotBatBuoc.Where(c => !table.HasColumn(c)).ToList();
            if (thieu.Count > 0)
                throw new CatalogFormatException(
                    "File danh mục DVKT kèm ICD thiếu cột " + string.Join(", ", thieu) + ": " + FilePath);

            bool hasMaKt = table.HasColumn("MA_KT_TT23");
            bool hasTen23 = table.HasColumn("TEN_DVKT_TT23");
            bool hasTenPdg = table.HasColumn("TEN_DVKT_PDG");
            bool hasIcdNeg = table.HasColumn("ICD_NEG");

            foreach (IDictionary<string, string> row in table.Rows)
            {
                HashSet<string> icd = TachMa(ExcelTable.Get(row, "ICD_REQ"), true);
                HashSet<string> neg = hasIcdNeg ? TachMa(ExcelTable.Get(row, "ICD_NEG"), true) : new HashSet<string>();
                // MA_TUONG_DUONG_7603 có thể chứa nhiều mã ngăn cách ";"
                foreach (string ma in TachMa(ExcelTable.Get(row, "MA_TUONG_DUONG_7603"), false))
                {
                    DvktIcdInfo e;
                    if (!_catalog.TryGetValue(ma, out e))
                    {
                        e = new DvktIcdInfo();
                        _catalog[ma] = e;
                    }
                    e.Icd.UnionWith(icd);
                    e.IcdNeg.UnionWith(neg);
                    // Tên/mã hiển thị: lấy giá trị đầu tiên không rỗng
                    if (e.MaKtTt23.Length == 0 && hasMaKt) e.MaKtTt23 = ExcelTable.Get(row, "MA_KT_TT23").Trim();
                    if (e.TenTt23.Length == 0 && hasTen23) e.TenTt23 = ExcelTable.Get(row, "TEN_DVKT_TT23").Trim();
                    if (e.TenPdg.Length == 0 && hasTenPdg) e.TenPdg = ExcelTable.Get(row, "TEN_DVKT_PDG").Trim();
                }
            }
        }

        /// <summary>Thông tin của mã DVKT; null nếu không có trong danh mục.</summary>
        public DvktIcdInfo GetInfo(string maDvkt)
        {
            DvktIcdInfo info;
            return _catalog.TryGetValue(EmrValidation.Core.Helpers.ParseHelper.MaDvktGoc(maDvkt), out info) ? info : null;
        }

        public int Total { get { return _catalog.Count; } }

        public override string ToString()
        {
            return "DanhMucDVKTICDLoader(file='" + Path.GetFileName(FilePath) + "', dvkt=" + Total + ")";
        }
    }

    /// <summary>
    /// Kiểm tra DVKT kèm ICD (XML3 đối chiếu MA_BENH_CHINH, MA_BENH_KT của XML1).
    /// WARN.DV01 – DVKT thuộc danh mục có ICD_REQ nhưng hồ sơ không có mã bệnh nào khớp.
    /// WARN.DV02 – DVKT thuộc danh mục có ICD_NEG (chống chỉ định) và hồ sơ có ít nhất 1 mã bệnh trùng ICD_NEG.
    /// ICD_REQ trống → bỏ qua DV01; ICD_NEG trống hoặc không có cột → bỏ qua DV02 (hai quy tắc độc lập).
    /// Mã DVKT so sánh tuyệt đối (trim); mã ICD sau khi trim và viết hoa, so sánh tuyệt đối.
    /// </summary>
    public class KiemTraDVKTKemICD : GiamDinhBase
    {
        private const string CanCu = "Danh mục DVKT kèm ICD_REQ";
        private const string MaCanhBao = "WARN.DV01";
        private const string MaChongChiDinh = "WARN.DV02";
        private const string CanCuNeg = "Danh mục DVKT kèm ICD_NEG (chống chỉ định)";

        private readonly DanhMucDVKTICDLoader _loader;

        public KiemTraDVKTKemICD(DanhMucDVKTICDLoader loader)
        {
            _loader = loader;
        }

        /// <summary>Liệt kê mã (sắp xếp), rút gọn nếu quá dài.</summary>
        private static string RutGon(ICollection<string> ds, int toiDa = 15)
        {
            List<string> ls = ds.OrderBy(x => x, StringComparer.Ordinal).ToList();
            if (ls.Count <= toiDa) return string.Join(", ", ls);
            return string.Join(", ", ls.Take(toiDa)) + $", ... (+{ls.Count - toiDa} mã)";
        }

        public override List<GiamDinhError> Check(IDictionary<string, List<BhytBase>> allObjects)
        {
            var errors = new List<GiamDinhError>();

            // MA_LK → (MA_BENH_CHINH, tập MA_BENH_KT) từ XML1
            var xml1Chinh = new Dictionary<string, string>();
            var xml1Kt = new Dictionary<string, HashSet<string>>();
            foreach (SheetRow r in GetRows(allObjects, "XML1"))
            {
                string maLk1 = (r.Record.GetValue("MA_LK") ?? "").Trim();
                if (maLk1.Length > 0)
                {
                    xml1Chinh[maLk1] = (r.Record.GetValue("MA_BENH_CHINH") ?? "").Trim().ToUpperInvariant();
                    xml1Kt[maLk1] = DanhMucDVKTICDLoader.TachMa(r.Record.GetValue("MA_BENH_KT") ?? "", true);
                }
            }

            foreach (SheetRow row in GetRows(allObjects, "XML3"))
            {
                BhytBase rec = row.Record;
                string maLk = (rec.GetValue("MA_LK") ?? "").Trim();
                string maDv = (rec.GetValue("MA_DICH_VU") ?? "").Trim();
                string tenDv = (rec.GetValue("TEN_DICH_VU") ?? "").Trim();

                if (maLk.Length == 0 || maDv.Length == 0) continue;
                DvktIcdInfo info = _loader.GetInfo(maDv);
                if (info == null) continue;   // chỉ check DVKT có trong danh mục

                HashSet<string> icdReq = info.Icd;
                HashSet<string> icdNeg = info.IcdNeg;
                if (icdReq.Count == 0 && icdNeg.Count == 0) continue;   // không yêu cầu / không chống chỉ định ICD

                string maBenhChinh;
                if (!xml1Chinh.TryGetValue(maLk, out maBenhChinh)) maBenhChinh = "";
                HashSet<string> maBenhKt;
                if (!xml1Kt.TryGetValue(maLk, out maBenhKt)) maBenhKt = new HashSet<string>();
                var icdHoSo = new HashSet<string>(maBenhKt);
                if (maBenhChinh.Length > 0) icdHoSo.Add(maBenhChinh);

                // DV01: vi phạm nếu có ICD_REQ nhưng không có mã bệnh nào của hồ sơ khớp
                bool viPhamReq = icdReq.Count > 0 && !icdReq.Overlaps(icdHoSo);

                // DV02: vi phạm nếu có ít nhất 1 mã bệnh của hồ sơ trùng ICD_NEG
                HashSet<string> trung = new HashSet<string>(icdNeg);
                trung.IntersectWith(icdHoSo);
                bool viPhamNeg = trung.Count > 0;

                if (!viPhamReq && !viPhamNeg) continue;

                string ten = info.TenTt23.Length > 0 ? info.TenTt23
                           : info.TenPdg.Length > 0 ? info.TenPdg
                           : tenDv.Length > 0 ? tenDv : maDv;
                string kt = string.Join("; ", maBenhKt.OrderBy(x => x, StringComparer.Ordinal));
                if (viPhamReq)
                {
                    errors.Add(new DvktIcdError
                    {
                        Sheet = "XML3",
                        MaLk = maLk,
                        RowExcel = row.RowExcel,
                        MaLyDo = MaCanhBao,
                        MoTa = $"DVKT '{ten}' (mã: {maDv}) yêu cầu MA_BENH_CHINH hoặc MA_BENH_KT " +
                               $"có mã thuộc ICD_REQ {{{RutGon(icdReq)}}} " +
                               $"nhưng MA_BENH_CHINH='{(maBenhChinh.Length > 0 ? maBenhChinh : "(trống)")}', " +
                               $"MA_BENH_KT='{(kt.Length > 0 ? kt : "(trống)")}'. " +
                               "So sánh tuyệt đối.",
                        CanCu = CanCu,
                        MaDichVu = maDv,
                        MaKtTt23 = info.MaKtTt23,
                        TenDvktTt23 = info.TenTt23,
                        TenDvktPdg = info.TenPdg,
                        MaBenhChinh = maBenhChinh,
                        MaBenhKt = kt,
                        IcdReq = string.Join("; ", icdReq.OrderBy(x => x, StringComparer.Ordinal)),
                    });
                }

                if (viPhamNeg)
                    errors.Add(new DvktIcdError
                    {
                        Sheet = "XML3",
                        MaLk = maLk,
                        RowExcel = row.RowExcel,
                        MaLyDo = MaChongChiDinh,
                        MoTa = $"DVKT '{ten}' (mã: {maDv}) chống chỉ định với mã bệnh {{{RutGon(trung)}}} " +
                               $"(ICD_NEG: {{{RutGon(icdNeg)}}}). " +
                               $"Hồ sơ có MA_BENH_CHINH='{(maBenhChinh.Length > 0 ? maBenhChinh : "(trống)")}', " +
                               $"MA_BENH_KT='{(kt.Length > 0 ? kt : "(trống)")}'. " +
                               "So sánh tuyệt đối.",
                        CanCu = CanCuNeg,
                        MaDichVu = maDv,
                        MaKtTt23 = info.MaKtTt23,
                        TenDvktTt23 = info.TenTt23,
                        TenDvktPdg = info.TenPdg,
                        MaBenhChinh = maBenhChinh,
                        MaBenhKt = kt,
                        IcdNeg = string.Join("; ", icdNeg.OrderBy(x => x, StringComparer.Ordinal)),
                        IcdTrung = string.Join("; ", trung.OrderBy(x => x, StringComparer.Ordinal)),
                    });
            }

            return errors;
        }

        /// <summary>
        /// Xuất kết quả: &lt;ten_goc&gt;[&lt;timestamp&gt;]_dvkt_icd.xlsx
        /// TONG_HOP có 2 dòng (WARN.DV01, WARN.DV02); CHI_TIET: STT, ROW_EXCEL, MA_LK, MÃ CẢNH BÁO, MA_DICH_VU,
        /// MA_KT_TT23, TEN_DVKT_TT23, TEN_DVKT_PDG, MA_BENH_CHINH, MA_BENH_KT,
        /// ICD_REQ / ICD_NEG (DV01: ICD_REQ, DV02: ICD_NEG), MÃ BỆNH TRÙNG (chỉ DV02), MÔ TẢ.
        /// </summary>
        public static string WriteExcel(string excelFile, IList<GiamDinhError> errors)
        {
            return CanhBaoExcelWriter.WriteNhieuMa(
                excelFile, errors,
                "KIỂM TRA DVKT KÈM ICD (ICD_REQ, ICD_NEG)",
                "dvkt_icd",
                new[]
                {
                    new KeyValuePair<string, string>(MaCanhBao,
                        "DVKT thuộc danh mục nhưng MA_BENH_CHINH/MA_BENH_KT không khớp ICD_REQ"),
                    new KeyValuePair<string, string>(MaChongChiDinh,
                        "DVKT chống chỉ định với mã ICD của hồ sơ (ICD_NEG)"),
                },
                "✓ Tất cả DVKT thuộc danh mục đều phù hợp với ICD_REQ và ICD_NEG.",
                new[] { "STT", "ROW_EXCEL", "MA_LK", "MÃ CẢNH BÁO", "MA_DICH_VU", "MA_KT_TT23", "TEN_DVKT_TT23",
                        "TEN_DVKT_PDG", "MA_BENH_CHINH", "MA_BENH_KT", "ICD_REQ / ICD_NEG", "MÃ BỆNH TRÙNG", "MÔ TẢ" },
                (idx, e) =>
                {
                    var v = (DvktIcdError)e;
                    return new object[] { idx, v.RowExcel, v.MaLk, v.MaLyDo, v.MaDichVu, v.MaKtTt23, v.TenDvktTt23,
                                          v.TenDvktPdg, v.MaBenhChinh, v.MaBenhKt,
                                          v.MaLyDo == MaChongChiDinh ? v.IcdNeg : v.IcdReq, v.IcdTrung, v.MoTa };
                });
        }
    }
}