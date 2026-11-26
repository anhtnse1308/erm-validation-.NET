using System;
using System.Collections.Generic;
using System.Linq;
using EmrValidation.Core.GiamDinh;

namespace EmrValidation.Excel
{
    /// <summary>
    /// Xuất kết quả kiểm tra thuốc theo danh mục trúng thầu: &lt;ten_goc&gt;[&lt;timestamp&gt;]_danhmuc.xlsx
    ///   TONG_HOP – thống kê theo mã WARN.DMxx
    ///   CHI_TIET – toàn bộ lỗi
    ///   Mỗi mã WARN.DMxx một sheet (WARN.DM07, WARN.DM08 có thêm cột TEN_HOAT_CHAT, TEN_THUOC lấy từ danh mục)
    /// </summary>
    public static class DanhMucExcelWriter
    {
        private const string FillOdd = "#FFF2CC";
        private const string FillWarn = "#FCE4D6";

        private static readonly Dictionary<string, string> MaMoTa = new Dictionary<string, string>
        {
            { "WARN.DM01", "MA_THUOC không có trong danh mục" },
            { "WARN.DM02", "SO_DANG_KY không khớp với MA_THUOC trong danh mục" },
            { "WARN.DM03", "DON_GIA vượt DON_GIA_BH trong danh mục" },
            { "WARN.DM04", "DUONG_DUNG không khớp với danh mục" },
            { "WARN.DM05", "HAM_LUONG không khớp với danh mục" },
            { "WARN.DM06", "Thuốc hết hiệu lực tại thời điểm y lệnh" },
            { "WARN.DM07", "Thuốc được kê không có mã ICD tương ứng" },
            { "WARN.DM08", "Thuốc chống chỉ định với mã ICD của hồ sơ" },
        };

        /// <param name="thongTinThuoc">
        /// MA_THUOC → (TEN_HOAT_CHAT, TEN_THUOC) lấy từ file danh mục, dùng cho sheet WARN.DM07 và WARN.DM08.
        /// Truyền null thì hai cột này để trống.
        /// </param>
        public static string Write(
            string excelFile,
            IList<GiamDinhError> errors,
            string danhMucPath,
            Func<string, KeyValuePair<string, string>> thongTinThuoc = null)
        {
            var wb = new ClosedXML.Excel.XLWorkbook();
            string nowStr = XlKit.Now();

            // ===== TONG_HOP =====
            var thHdrs = new[] { "MÃ CẢNH BÁO", "Ý NGHĨA", "SỐ DÒNG" };
            var th = new XlSheet(wb.Worksheets.Add("TONG_HOP"), thHdrs);
            th.Title("KIỂM TRA THUỐC THEO DANH MỤC TRÚNG THẦU  –  " + nowStr, 5, 28);
            th.Note(2, 5, "Danh mục: " + danhMucPath);
            for (int ci = 0; ci < thHdrs.Length; ci++) th.Header(3, ci + 1, thHdrs[ci]);

            var cnt = new Dictionary<string, int>();
            foreach (GiamDinhError e in errors)
            {
                int n;
                cnt.TryGetValue(e.MaLyDo ?? "", out n);
                cnt[e.MaLyDo ?? ""] = n + 1;
            }

            int ri = 4;
            foreach (var kv in cnt.OrderBy(k => k.Key, StringComparer.Ordinal))
            {
                string fill = kv.Value > 0 ? FillWarn : XlSheet.FillOk;
                string moTa;
                if (!MaMoTa.TryGetValue(kv.Key, out moTa)) moTa = "";
                th.Cell(ri, 1, kv.Key, fill, XlFont.Warn, XlAlign.Center);
                th.Cell(ri, 2, moTa, fill);
                th.Cell(ri, 3, kv.Value, fill, XlFont.RedBold, XlAlign.Center);
                ri++;
            }

            th.Cell(ri, 1, "TỔNG", XlSheet.FillSum, XlFont.WhiteBold11, XlAlign.Center);
            th.Cell(ri, 2, "", XlSheet.FillSum);
            th.Cell(ri, 3, errors.Count, XlSheet.FillSum, XlFont.WhiteBold11, XlAlign.Center);

            if (errors.Count == 0)
                th.Message(ri + 2, 5, "✓ Không phát hiện lỗi so với danh mục thuốc trúng thầu.");
            th.Finish(3);

            // ===== CHI_TIET =====
            var ctHdrs = new[] { "STT", "SHEET", "ROW_EXCEL", "MA_LK", "MA_THUOC", "MA_LY_DO", "MO_TA", "CAN_CU" };
            var ct = new XlSheet(wb.Worksheets.Add("CHI_TIET"), ctHdrs);
            for (int ci = 0; ci < ctHdrs.Length; ci++) ct.Header(1, ci + 1, ctHdrs[ci]);

            for (int idx = 1; idx <= errors.Count; idx++)
            {
                GiamDinhError err = errors[idx - 1];
                int r = idx + 1;
                string fill = idx % 2 == 1 ? FillOdd : XlSheet.FillEven;
                ct.Cell(r, 1, idx, fill, XlFont.Data, XlAlign.Center);
                ct.Cell(r, 2, err.Sheet, fill, XlFont.Data, XlAlign.Center);
                ct.Cell(r, 3, err.RowExcel, fill, XlFont.Data, XlAlign.Center);
                ct.Cell(r, 4, err.MaLk, fill);
                ct.Cell(r, 5, err.MaDichVu, fill);
                ct.Cell(r, 6, err.MaLyDo, fill, XlFont.RedBold, XlAlign.Center);
                ct.Cell(r, 7, err.MoTa, fill, XlFont.Data, XlAlign.Top);
                ct.Cell(r, 8, err.CanCu, fill, XlFont.Data, XlAlign.Top);
                ct.RowHeight(r, XlKit.RowHeightFor(err.MoTa, 25, 14));
            }
            ct.Finish(1);
            ct.RowHeight(1, 28);

            // ===== Mỗi mã cảnh báo một sheet =====
            var groups = new Dictionary<string, List<GiamDinhError>>();
            foreach (GiamDinhError e in errors)
            {
                string key = e.MaLyDo ?? "";
                List<GiamDinhError> list;
                if (!groups.TryGetValue(key, out list))
                {
                    list = new List<GiamDinhError>();
                    groups[key] = list;
                }
                list.Add(e);
            }

            foreach (var g in groups.OrderBy(x => x.Key, StringComparer.Ordinal))
            {
                string sname = g.Key.Replace(".", "_");
                if (sname.Length > 31) sname = sname.Substring(0, 31);

                // Riêng sheet WARN.DM07, WARN.DM08: thêm TEN_HOAT_CHAT, TEN_THUOC (từ file danh mục) sau MA_THUOC
                bool coTenThuoc = g.Key == "WARN.DM07" || g.Key == "WARN.DM08";
                string[] gHdrs = coTenThuoc
                    ? new[] { "STT", "ROW_EXCEL", "MA_LK", "MA_THUOC", "TEN_HOAT_CHAT", "TEN_THUOC", "MO_TA" }
                    : new[] { "STT", "ROW_EXCEL", "MA_LK", "MA_THUOC", "MO_TA" };

                var gs = new XlSheet(wb.Worksheets.Add(sname), gHdrs);
                for (int ci = 0; ci < gHdrs.Length; ci++) gs.Header(1, ci + 1, gHdrs[ci]);

                for (int idx = 1; idx <= g.Value.Count; idx++)
                {
                    GiamDinhError err = g.Value[idx - 1];
                    int r = idx + 1;
                    string fill = idx % 2 == 1 ? FillOdd : XlSheet.FillEven;
                    gs.Cell(r, 1, idx, fill, XlFont.Data, XlAlign.Center);
                    gs.Cell(r, 2, err.RowExcel, fill, XlFont.Data, XlAlign.Center);
                    gs.Cell(r, 3, err.MaLk, fill);
                    gs.Cell(r, 4, err.MaDichVu, fill);
                    if (coTenThuoc)
                    {
                        KeyValuePair<string, string> info = thongTinThuoc != null
                            ? thongTinThuoc(err.MaDichVu)
                            : new KeyValuePair<string, string>("", "");
                        gs.Cell(r, 5, info.Key, fill);
                        gs.Cell(r, 6, info.Value, fill);
                        gs.Cell(r, 7, err.MoTa, fill, XlFont.Data, XlAlign.Top);
                    }
                    else
                    {
                        gs.Cell(r, 5, err.MoTa, fill, XlFont.Data, XlAlign.Top);
                    }
                    gs.RowHeight(r, XlKit.RowHeightFor(err.MoTa, 25, 14));
                }
                gs.Finish(1);
                gs.RowHeight(1, 28);
            }

            string outFile = XlKit.OutPath(excelFile, "danhmuc");
            wb.SaveAs(outFile);
            return outFile;
        }
    }
}