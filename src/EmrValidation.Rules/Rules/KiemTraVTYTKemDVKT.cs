using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EmrValidation.Core.GiamDinh;
using EmrValidation.Core.Models;
using EmrValidation.Excel;

namespace EmrValidation.Rules
{
    /// <summary>Lỗi của luồng VTYT kèm DVKT (có thêm thông tin để xuất Excel).</summary>
    public class VtytDvktError : GiamDinhError
    {
        public string MaVatTu { get; set; }
        public string MaDichVuXml3 { get; set; }
        public string TenVtytTt { get; set; }
        public string TenThuongMai { get; set; }

        public VtytDvktError()
        {
            MaVatTu = ""; MaDichVuXml3 = ""; TenVtytTt = ""; TenThuongMai = "";
        }
    }

    /// <summary>Thông tin một VTYT trong danh mục VTYT kèm DVKT.</summary>
    public sealed class VtytInfo
    {
        /// <summary>Tập mã DVKT được phép đi kèm (gộp mọi dòng cùng MA_VTYT).</summary>
        public HashSet<string> Dvkt { get; private set; }
        public string TenVtytTt { get; set; }
        public string TenThuongMai { get; set; }

        public VtytInfo()
        {
            Dvkt = new HashSet<string>();
            TenVtytTt = "";
            TenThuongMai = "";
        }
    }

    /// <summary>
    /// Load file Excel danh mục VTYT kèm DVKT (sheet đầu tiên).
    /// Cột bắt buộc: MA_VTYT, MA_DVKT_KT (mảng mã ngăn cách ";"). Cột tùy chọn: TEN_VTYT_TT, TEN_THUONG_MAI.
    /// Tên cột so khớp chính xác.
    /// </summary>
    public class DanhMucVTYTLoader
    {
        private static readonly string[] CotBatBuoc = { "MA_VTYT", "MA_DVKT_KT" };

        private readonly Dictionary<string, VtytInfo> _catalog = new Dictionary<string, VtytInfo>();

        public string FilePath { get; private set; }

        public DanhMucVTYTLoader(string filepath)
        {
            if (!File.Exists(filepath))
                throw new FileNotFoundException("Không tìm thấy file danh mục: " + filepath);
            FilePath = filepath;
            Load(ExcelTableReader.ReadFirstSheet(filepath));
        }

        /// <summary>Dựng từ bảng đã đọc sẵn (dùng khi kiểm thử).</summary>
        public DanhMucVTYTLoader(ExcelTable table, string label)
        {
            FilePath = label;
            Load(table);
        }

        private void Load(ExcelTable table)
        {
            List<string> thieu = CotBatBuoc.Where(c => !table.HasColumn(c)).ToList();
            if (thieu.Count > 0)
                throw new CatalogFormatException(
                    "File danh mục VTYT kèm DVKT thiếu cột " + string.Join(", ", thieu) + ": " + FilePath);

            bool hasTenTt = table.HasColumn("TEN_VTYT_TT");
            bool hasTenTm = table.HasColumn("TEN_THUONG_MAI");

            foreach (IDictionary<string, string> row in table.Rows)
            {
                string ma = ExcelTable.Get(row, "MA_VTYT").Trim();
                if (ma.Length == 0) continue;

                VtytInfo e;
                if (!_catalog.TryGetValue(ma, out e))
                {
                    e = new VtytInfo();
                    _catalog[ma] = e;
                }
                foreach (string x in ExcelTable.Get(row, "MA_DVKT_KT").Split(';'))
                {
                    string t = x.Trim();
                    if (t.Length > 0) e.Dvkt.Add(t);
                }
                // Tên: lấy giá trị đầu tiên không rỗng
                if (e.TenVtytTt.Length == 0 && hasTenTt) e.TenVtytTt = ExcelTable.Get(row, "TEN_VTYT_TT").Trim();
                if (e.TenThuongMai.Length == 0 && hasTenTm) e.TenThuongMai = ExcelTable.Get(row, "TEN_THUONG_MAI").Trim();
            }
        }

        /// <summary>Thông tin của MA_VTYT; null nếu không có trong danh mục.</summary>
        public VtytInfo GetInfo(string maVtyt)
        {
            VtytInfo info;
            return _catalog.TryGetValue(maVtyt.Trim(), out info) ? info : null;
        }

        public int Total { get { return _catalog.Count; } }

        public override string ToString()
        {
            return "DanhMucVTYTLoader(file='" + Path.GetFileName(FilePath) + "', vtyt=" + Total + ")";
        }
    }

    /// <summary>
    /// Kiểm tra VTYT kèm DVKT trong XML3.
    /// WARN.VT01 – MA_VAT_TU thuộc danh mục nhưng MA_DICH_VU (cùng dòng) trống / không thuộc MA_DVKT_KT.
    /// So sánh tuyệt đối (chỉ trim). Một MA_VTYT nhiều dòng danh mục → gộp mã MA_DVKT_KT.
    /// </summary>
    public class KiemTraVTYTKemDVKT : GiamDinhBase
    {
        private const string CanCu = "Danh mục VTYT kèm DVKT";
        private const string MaCanhBao = "WARN.VT01";

        private readonly DanhMucVTYTLoader _loader;

        public KiemTraVTYTKemDVKT(DanhMucVTYTLoader loader)
        {
            _loader = loader;
        }

        /// <summary>Liệt kê mã (sắp xếp), rút gọn nếu quá dài.</summary>
        private static string RutGon(ICollection<string> ds, int toiDa = 10)
        {
            List<string> ls = ds.OrderBy(x => x, StringComparer.Ordinal).ToList();
            if (ls.Count <= toiDa) return string.Join(", ", ls);
            return string.Join(", ", ls.Take(toiDa)) + $", ... (+{ls.Count - toiDa} mã)";
        }

        public override List<GiamDinhError> Check(IDictionary<string, List<BhytBase>> allObjects)
        {
            var errors = new List<GiamDinhError>();

            foreach (SheetRow row in GetRows(allObjects, "XML3"))
            {
                BhytBase rec = row.Record;
                string maLk = (rec.GetValue("MA_LK") ?? "").Trim();
                string maVt = (rec.GetValue("MA_VAT_TU") ?? "").Trim();
                string maDv = (rec.GetValue("MA_DICH_VU") ?? "").Trim();

                if (maLk.Length == 0 || maVt.Length == 0) continue;
                VtytInfo info = _loader.GetInfo(maVt);
                if (info == null) continue;   // chỉ check VTYT có trong danh mục

                HashSet<string> dvkt = info.Dvkt;
                string ten = info.TenVtytTt.Length > 0 ? info.TenVtytTt
                           : info.TenThuongMai.Length > 0 ? info.TenThuongMai : maVt;
                string goc = $"VTYT '{ten}' (mã: {maVt}) thuộc danh mục VTYT kèm DVKT";

                string lyDo;
                if (maDv.Length == 0)
                    lyDo = $"{goc} nhưng MA_DICH_VU để trống.";
                else if (dvkt.Count == 0)
                    lyDo = $"{goc} nhưng danh mục không khai báo MA_DVKT_KT " +
                           $"(MA_DICH_VU='{maDv}').";
                else if (!dvkt.Contains(EmrValidation.Core.Helpers.ParseHelper.MaDvktGoc(maDv)))
                    lyDo = $"{goc} nhưng MA_DICH_VU='{maDv}' không thuộc " +
                           $"MA_DVKT_KT {{{RutGon(dvkt)}}}. So sánh tuyệt đối.";
                else
                    continue;

                errors.Add(new VtytDvktError
                {
                    Sheet = "XML3",
                    MaLk = maLk,
                    RowExcel = row.RowExcel,
                    MaLyDo = MaCanhBao,
                    MoTa = lyDo,
                    CanCu = CanCu,
                    MaDichVu = maVt,
                    MaVatTu = maVt,
                    MaDichVuXml3 = maDv,
                    TenVtytTt = info.TenVtytTt,
                    TenThuongMai = info.TenThuongMai,
                });
            }

            return errors;
        }

        /// <summary>
        /// Xuất kết quả: &lt;ten_goc&gt;[&lt;timestamp&gt;]_vtyt_dvkt.xlsx
        /// (CHI_TIET: STT, ROW_EXCEL, MA_LK, MA_VAT_TU, TEN_VTYT_TT, TEN_THUONG_MAI, MA_DICH_VU, MÔ TẢ).
        /// </summary>
        public static string WriteExcel(string excelFile, IList<GiamDinhError> errors)
        {
            return CanhBaoExcelWriter.Write(
                excelFile, errors,
                "KIỂM TRA VTYT KÈM DVKT",
                "vtyt_dvkt",
                MaCanhBao,
                "VTYT thuộc danh mục nhưng MA_DICH_VU trống / không thuộc MA_DVKT_KT",
                "✓ Tất cả VTYT thuộc danh mục đều có DVKT kèm theo hợp lệ.",
                new[] { "STT", "ROW_EXCEL", "MA_LK", "MA_VAT_TU", "TEN_VTYT_TT", "TEN_THUONG_MAI", "MA_DICH_VU", "MÔ TẢ" },
                (idx, e) =>
                {
                    var v = (VtytDvktError)e;
                    return new object[] { idx, v.RowExcel, v.MaLk, v.MaVatTu, v.TenVtytTt, v.TenThuongMai, v.MaDichVuXml3, v.MoTa };
                });
        }
    }
}
