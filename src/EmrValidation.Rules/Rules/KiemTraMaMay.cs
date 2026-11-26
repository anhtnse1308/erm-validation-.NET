using System;
using System.Collections.Generic;
using System.IO;
using EmrValidation.Core.GiamDinh;
using EmrValidation.Core.Models;
using EmrValidation.Excel;

namespace EmrValidation.Rules
{
    /// <summary>Thông tin hiển thị của một DVKT trong danh mục yêu cầu MA_MAY.</summary>
    public sealed class MaMayInfo
    {
        public string TenTt23 { get; private set; }
        public string TenPd { get; private set; }

        public MaMayInfo(string tenTt23, string tenPd)
        {
            TenTt23 = tenTt23;
            TenPd = tenPd;
        }
    }

    /// <summary>
    /// Load file Excel danh mục DVKT yêu cầu MA_MAY (sheet đầu tiên).
    /// Cột bắt buộc: MA_DVKT. Cột tùy chọn: TEN_DVKT_TT23, TEN_DVKT_PHE_DUYET.
    /// Tên cột không phân biệt hoa thường và bỏ khoảng trắng thừa.
    /// </summary>
    public class DanhMucMaMayLoader
    {
        private readonly Dictionary<string, MaMayInfo> _catalog = new Dictionary<string, MaMayInfo>();

        public string FilePath { get; private set; }

        public DanhMucMaMayLoader(string filepath)
        {
            if (!File.Exists(filepath))
                throw new FileNotFoundException("Không tìm thấy file danh mục: " + filepath);
            FilePath = filepath;
            Load(ExcelTableReader.ReadFirstSheet(filepath));
        }

        /// <summary>Dựng từ bảng đã đọc sẵn (dùng khi kiểm thử).</summary>
        public DanhMucMaMayLoader(ExcelTable table, string label)
        {
            FilePath = label;
            Load(table);
        }

        private void Load(ExcelTable table)
        {
            // Tìm cột không phân biệt hoa thường (cột sau ghi đè cột trước nếu trùng tên chuẩn hóa)
            var colMap = new Dictionary<string, string>();
            foreach (string h in table.Headers)
                colMap[h.Trim().ToUpperInvariant()] = h;

            string colMa;
            if (!colMap.TryGetValue("MA_DVKT", out colMa))
                throw new CatalogFormatException("File danh mục thiếu cột MA_DVKT: " + FilePath);

            string colTenTt23, colTenPd;
            if (!colMap.TryGetValue("TEN_DVKT_TT23", out colTenTt23)) colTenTt23 = "";
            if (!colMap.TryGetValue("TEN_DVKT_PHE_DUYET", out colTenPd)) colTenPd = "";

            foreach (IDictionary<string, string> row in table.Rows)
            {
                string ma = ExcelTable.Get(row, colMa).Trim();
                string lower = ma.ToLowerInvariant();
                if (ma.Length == 0 || lower == "nan" || lower == "none") continue;
                _catalog[ma] = new MaMayInfo(
                    colTenTt23.Length > 0 ? ExcelTable.Get(row, colTenTt23).Trim() : "",
                    colTenPd.Length > 0 ? ExcelTable.Get(row, colTenPd).Trim() : "");
            }
        }

        public bool RequiresMaMay(string maDvkt)
        {
            return _catalog.ContainsKey(EmrValidation.Core.Helpers.ParseHelper.MaDvktGoc(maDvkt));
        }

        /// <summary>Thông tin hiển thị; mã không có trong danh mục → các tên rỗng.</summary>
        public MaMayInfo GetInfo(string maDvkt)
        {
            MaMayInfo info;
            return _catalog.TryGetValue(EmrValidation.Core.Helpers.ParseHelper.MaDvktGoc(maDvkt), out info) ? info : new MaMayInfo("", "");
        }

        public int Total { get { return _catalog.Count; } }

        public override string ToString()
        {
            return "DanhMucMaMayLoader(file='" + Path.GetFileName(FilePath) + "', dvkt=" + Total + ")";
        }
    }

    /// <summary>
    /// Rule – Kiểm tra MA_MAY trong XML3 đối với các DVKT yêu cầu khai báo máy.
    /// Với mỗi dòng XML3: MA_DICH_VU thuộc danh mục VÀ MA_PTTT_QT có giá trị (sau trim) → kiểm tra MA_MAY.
    /// MA_PTTT_QT trống → bỏ qua. MA_MAY trống hoặc chỉ khoảng trắng → cảnh báo WARN.MM01.
    /// </summary>
    public class KiemTraMaMay : GiamDinhBase
    {
        private const string CanCu = "Danh mục DVKT yêu cầu khai báo mã máy thực hiện";

        private readonly DanhMucMaMayLoader _loader;

        public KiemTraMaMay(DanhMucMaMayLoader loader)
        {
            _loader = loader;
        }

        public override List<GiamDinhError> Check(IDictionary<string, List<BhytBase>> allObjects)
        {
            var errors = new List<GiamDinhError>();

            foreach (SheetRow row in GetRows(allObjects, "XML3"))
            {
                BhytBase rec = row.Record;
                string maLk = (rec.GetValue("MA_LK") ?? "").Trim();
                string maDv = (rec.GetValue("MA_DICH_VU") ?? "").Trim();
                string maMay = rec.GetValue("MA_MAY") ?? "";
                string tenDv = (rec.GetValue("TEN_DICH_VU") ?? "").Trim();
                string maPttt = (rec.GetValue("MA_PTTT_QT") ?? "").Trim();

                if (maLk.Length == 0 || maDv.Length == 0) continue;
                if (!_loader.RequiresMaMay(maDv)) continue;
                // Chỉ kiểm tra khi có MA_PTTT_QT; để trống thì bỏ qua
                if (maPttt.Length == 0) continue;

                // Trim khoảng trắng, kiểm tra có giá trị không
                if (maMay.Trim().Length == 0)
                {
                    MaMayInfo info = _loader.GetInfo(maDv);
                    string ten = info.TenTt23.Length > 0 ? info.TenTt23
                               : info.TenPd.Length > 0 ? info.TenPd
                               : tenDv.Length > 0 ? tenDv : maDv;
                    errors.Add(new GiamDinhError
                    {
                        Sheet = "XML3",
                        MaLk = maLk,
                        RowExcel = row.RowExcel,
                        MaLyDo = "WARN.MM01",
                        MoTa = $"DVKT '{ten}' (mã: {maDv}) yêu cầu khai báo MA_MAY " +
                               $"nhưng MA_MAY để trống (MA_PTTT_QT: {maPttt}).",
                        CanCu = CanCu,
                        MaDichVu = maDv,
                    });
                }
            }

            return errors;
        }

        /// <summary>
        /// Xuất kết quả kiểm tra MA_MAY: &lt;ten_goc&gt;[&lt;timestamp&gt;]_mamay.xlsx
        /// (TONG_HOP và CHI_TIET: STT, ROW_EXCEL, MA_LK, MA_DVKT, MÔ TẢ).
        /// </summary>
        public static string WriteExcel(string excelFile, IList<GiamDinhError> errors)
        {
            return CanhBaoExcelWriter.Write(
                excelFile, errors,
                "KIỂM TRA MA_MAY DVKT",
                "mamay",
                "WARN.MM01",
                "DVKT yêu cầu MA_MAY nhưng để trống",
                "✓ Tất cả DVKT yêu cầu MA_MAY đều đã khai báo.",
                new[] { "STT", "ROW_EXCEL", "MA_LK", "MA_DVKT", "MÔ TẢ" },
                (idx, e) => new object[] { idx, e.RowExcel, e.MaLk, e.MaDichVu, e.MoTa });
        }
    }
}
