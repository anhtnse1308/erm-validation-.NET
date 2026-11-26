using System;
using System.Collections.Generic;
using System.Linq;
using ClosedXML.Excel;
using EmrValidation.Core.GiamDinh;

namespace EmrValidation.Excel
{
    /// <summary>
    /// Xuất kết quả giám định ra Excel (tương đương GiamDinhService.write_excel_giamdinh).
    /// Tên file: &lt;ten_goc&gt;[&lt;timestamp&gt;]_giamdinh.xlsx
    ///   TONG_HOP – thống kê số lỗi theo mã lý do + sheet XML
    ///   CHI_TIET – toàn bộ lỗi
    ///   Sheet theo từng mã lý do (S011.1, S054.1, D004.1, ...) để dễ tra cứu
    /// </summary>
    public static class GiamDinhExcelWriter
    {
        private const string FillWarn = "#FCE4D6";     // cam nhạt (WARN)
        private const string FillOdd = "#FFF2CC";      // vàng nhạt
        private const string FillHdr2 = "#C00000";     // đỏ đậm (sheet theo mã)

        private static readonly Dictionary<string, string> NhomMoTa = new Dictionary<string, string>
        {
            { "S011.1",      "Dịch vụ kỹ thuật có chống chỉ định" },
            { "S054.1",      "DVKT nằm trong quy trình của DVKT khác" },
            { "D004.1",      "VTYT vượt mức chi quy định" },
            { "S059.2",      "Thuốc YHCT ngoài giới hạn chỉ định" },
            { "S079.1",      "Kháng sinh đặc biệt sai hạng BV" },
            { "S079.1-WARN", "Kháng sinh đặc biệt – CẦN KIỂM TRA THỦ CÔNG" },
            { "",            "Lỗi nghiệp vụ khác (không có mã từ chối riêng)" },
        };

        public static string Write(string excelFile, IList<GiamDinhError> errors)
        {
            var wb = new XLWorkbook();
            string nowStr = XlKit.Now();

            // ===== Sheet 1 – TONG_HOP =====
            var thHeaders = new[] { "MÃ LÝ DO", "TÊN NHÓM LỖI", "SHEET XML", "SỐ LƯỢNG LỖI", "GHI CHÚ" };
            var th = new XlSheet(wb.Worksheets.Add("TONG_HOP"), thHeaders);
            th.Title("TỔNG HỢP KẾT QUẢ GIÁM ĐỊNH BHYT  –  " + nowStr, 6, 30);
            for (int ci = 0; ci < thHeaders.Length; ci++)
                th.Header(2, ci + 1, thHeaders[ci]);

            // Tổng hợp theo (mã lý do, sheet), sắp xếp như sorted() của Python
            var counter = new Dictionary<Tuple<string, string>, int>();
            foreach (GiamDinhError err in errors)
            {
                var key = Tuple.Create(err.MaLyDo ?? "", err.Sheet ?? "");
                int n;
                counter.TryGetValue(key, out n);
                counter[key] = n + 1;
            }

            int ri = 3;
            foreach (var kv in counter.OrderBy(k => k.Key.Item1, StringComparer.Ordinal)
                                      .ThenBy(k => k.Key.Item2, StringComparer.Ordinal))
            {
                string maLyDo = kv.Key.Item1;
                string fill = maLyDo.Contains("WARN") ? FillWarn : FillOdd;
                string mota;
                if (!NhomMoTa.TryGetValue(maLyDo, out mota)) mota = "";
                th.Cell(ri, 1, maLyDo, fill, XlFont.Bold);
                th.Cell(ri, 2, mota, fill);
                th.Cell(ri, 3, kv.Key.Item2, fill, XlFont.Data, XlAlign.Center);
                th.Cell(ri, 4, kv.Value, fill, XlFont.Red, XlAlign.Center);
                th.Cell(ri, 5, "", fill);
                ri++;
            }

            // Tổng cộng
            th.Cell(ri, 1, "TỔNG", XlSheet.FillHdr, XlFont.WhiteBold, XlAlign.Center);
            th.Cell(ri, 2, "", XlSheet.FillHdr);
            th.Cell(ri, 3, "", XlSheet.FillHdr);
            th.Cell(ri, 4, errors.Count, XlSheet.FillHdr, XlFont.WhiteBold, XlAlign.Center);
            th.Cell(ri, 5, "", XlSheet.FillHdr);
            th.Finish(2);

            // ===== Sheet 2 – CHI_TIET (tất cả lỗi) =====
            var ctHeaders = new[] { "STT", "SHEET", "ROW_EXCEL", "MA_LK", "MA_DICH_VU", "MA_LY_DO", "MO_TA", "CAN_CU" };
            var ct = new XlSheet(wb.Worksheets.Add("CHI_TIET"), ctHeaders);
            for (int ci = 0; ci < ctHeaders.Length; ci++)
                ct.Header(1, ci + 1, ctHeaders[ci]);

            for (int idx = 1; idx <= errors.Count; idx++)
            {
                GiamDinhError err = errors[idx - 1];
                int r = idx + 1;
                string fill = (err.MaLyDo ?? "").Contains("WARN") ? FillWarn
                            : (idx % 2 == 1 ? FillOdd : XlSheet.FillEven);
                ct.Cell(r, 1, idx, fill, XlFont.Data, XlAlign.Center);
                ct.Cell(r, 2, err.Sheet, fill, XlFont.Data, XlAlign.Center);
                ct.Cell(r, 3, err.RowExcel, fill, XlFont.Data, XlAlign.Center);
                ct.Cell(r, 4, err.MaLk, fill);
                ct.Cell(r, 5, err.MaDichVu, fill);
                ct.Cell(r, 6, err.MaLyDo, fill,
                    string.IsNullOrEmpty(err.MaLyDo) ? XlFont.Data : XlFont.Red, XlAlign.Center);
                ct.Cell(r, 7, err.MoTa, fill, XlFont.Data, XlAlign.Top);
                ct.Cell(r, 8, err.CanCu, fill, XlFont.Data, XlAlign.Top);
                ct.RowHeight(r, XlKit.RowHeightFor(err.MoTa, 30, 15));
            }
            ct.RowHeight(1, 30);
            ct.Finish(1);

            // ===== Sheet theo từng nhóm mã lý do =====
            var groups = new Dictionary<string, List<GiamDinhError>>();
            foreach (GiamDinhError err in errors)
            {
                string key = string.IsNullOrEmpty(err.MaLyDo) ? "KHAC" : err.MaLyDo;
                List<GiamDinhError> list;
                if (!groups.TryGetValue(key, out list))
                {
                    list = new List<GiamDinhError>();
                    groups[key] = list;
                }
                list.Add(err);
            }

            var gHeaders = new[] { "STT", "SHEET", "ROW_EXCEL", "MA_LK", "MA_DICH_VU", "MO_TA", "CAN_CU" };
            foreach (var g in groups.OrderBy(x => x.Key, StringComparer.Ordinal))
            {
                // Tên sheet tối đa 31 ký tự (giới hạn Excel)
                string sheetName = g.Key.Replace(".", "_").Replace("-", "_");
                if (sheetName.Length > 31) sheetName = sheetName.Substring(0, 31);
                if (wb.Worksheets.Any(w => w.Name == sheetName))
                    sheetName = sheetName.Substring(0, Math.Min(28, sheetName.Length)) + "_2";

                var gs = new XlSheet(wb.Worksheets.Add(sheetName), gHeaders);
                for (int ci = 0; ci < gHeaders.Length; ci++)
                    gs.Header(1, ci + 1, gHeaders[ci], FillHdr2);

                for (int idx = 1; idx <= g.Value.Count; idx++)
                {
                    GiamDinhError err = g.Value[idx - 1];
                    int r = idx + 1;
                    string fill = idx % 2 == 1 ? FillOdd : XlSheet.FillEven;
                    gs.Cell(r, 1, idx, fill, XlFont.Data, XlAlign.Center);
                    gs.Cell(r, 2, err.Sheet, fill, XlFont.Data, XlAlign.Center);
                    gs.Cell(r, 3, err.RowExcel, fill, XlFont.Data, XlAlign.Center);
                    gs.Cell(r, 4, err.MaLk, fill);
                    gs.Cell(r, 5, err.MaDichVu, fill);
                    gs.Cell(r, 6, err.MoTa, fill, XlFont.Data, XlAlign.Top);
                    gs.Cell(r, 7, err.CanCu, fill, XlFont.Data, XlAlign.Top);
                    gs.RowHeight(r, XlKit.RowHeightFor(err.MoTa, 30, 15));
                }
                gs.RowHeight(1, 30);
                gs.Finish(1);
            }

            string outFile = XlKit.OutPath(excelFile, "giamdinh");
            wb.SaveAs(outFile);
            return outFile;
        }
    }
}
