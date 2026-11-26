using System.Collections.Generic;
using System.Linq;
using ClosedXML.Excel;
using EmrValidation.Core.GiamDinh;

namespace EmrValidation.Excel
{
    /// <summary>
    /// Xuất kết quả kiểm tra ngày giường: &lt;ten_goc&gt;[&lt;timestamp&gt;]_ngaygiuong.xlsx
    ///   TONG_HOP           – thống kê số lỗi theo mã cảnh báo
    ///   CHI_TIET           – toàn bộ lỗi
    ///   WARN_GG01_SoNgay   – thiếu/thừa dịch vụ giường (lỗi XML1)
    ///   WARN_GG02_TrungNgay – trùng dịch vụ giường cùng ngày (lỗi XML3)
    /// </summary>
    public static class NgayGiuongExcelWriter
    {
        private const string FillG1 = "#FCE4D6";    // cam – G1
        private const string FillG2 = "#FFF2CC";    // vàng – G2
        private const string FillOdd = "#EBF3FB";

        public static string Write(string excelFile, IList<GiamDinhError> errors)
        {
            var wb = new XLWorkbook();
            string nowStr = XlKit.Now();
            var cnt = new Dictionary<string, int>();
            foreach (GiamDinhError e in errors)
            {
                int n;
                cnt.TryGetValue(e.MaLyDo ?? "", out n);
                cnt[e.MaLyDo ?? ""] = n + 1;
            }

            var maMoTa = new[]
            {
                new KeyValuePair<string, string>("WARN.GG01", "Số dịch vụ giường ≠ số ngày điều trị (NGAY_RA − NGAY_VAO + 1)"),
                new KeyValuePair<string, string>("WARN.GG02", "Cùng BN, cùng ngày có > 1 dịch vụ giường"),
            };
            Dictionary<string, string> fillMa = new Dictionary<string, string>
            {
                { "WARN.GG01", FillG1 }, { "WARN.GG02", FillG2 },
            };

            // ===== TONG_HOP =====
            var thHdrs = new[] { "MÃ CẢNH BÁO", "Ý NGHĨA", "SỐ TRƯỜNG HỢP" };
            var th = new XlSheet(wb.Worksheets.Add("TONG_HOP"), thHdrs);
            th.Title("KIỂM TRA NGÀY GIƯỜNG NỘI TRÚ (MA_LOAI_KCB=03)  –  " + nowStr, 4, 28);
            for (int ci = 0; ci < thHdrs.Length; ci++) th.Header(2, ci + 1, thHdrs[ci]);

            int ri = 3;
            foreach (var m in maMoTa)
            {
                int so;
                cnt.TryGetValue(m.Key, out so);
                string fill = fillMa[m.Key];
                th.Cell(ri, 1, m.Key, fill, XlFont.Data, XlAlign.Center);
                th.Cell(ri, 2, m.Value, fill);
                th.Cell(ri, 3, so, fill, so > 0 ? XlFont.RedBold : XlFont.Data, XlAlign.Center);
                ri++;
            }
            th.Cell(ri, 1, "TỔNG", XlSheet.FillSum, XlFont.WhiteBold, XlAlign.Center);
            th.Cell(ri, 2, "", XlSheet.FillSum);
            th.Cell(ri, 3, errors.Count, XlSheet.FillSum, XlFont.WhiteBold, XlAlign.Center);

            if (errors.Count == 0)
                th.Message(ri + 2, 4, "✓ Không phát hiện lỗi ngày giường.");
            th.Finish(2);

            // ===== CHI_TIET =====
            var ctHdrs = new[] { "STT", "MÃ CẢNH BÁO", "SHEET", "ROW_EXCEL", "MA_LK", "MA_DICH_VU / TEN_DV", "MÔ TẢ" };
            var ct = new XlSheet(wb.Worksheets.Add("CHI_TIET"), ctHdrs);
            for (int ci = 0; ci < ctHdrs.Length; ci++) ct.Header(1, ci + 1, ctHdrs[ci]);

            for (int idx = 1; idx <= errors.Count; idx++)
            {
                GiamDinhError err = errors[idx - 1];
                int r = idx + 1;
                string fill;
                if (!fillMa.TryGetValue(err.MaLyDo ?? "", out fill))
                    fill = idx % 2 == 1 ? FillOdd : XlSheet.FillEven;
                ct.Cell(r, 1, idx, fill, XlFont.Data, XlAlign.Center);
                ct.Cell(r, 2, err.MaLyDo, fill, XlFont.RedBold, XlAlign.Center);
                ct.Cell(r, 3, err.Sheet, fill, XlFont.Data, XlAlign.Center);
                ct.Cell(r, 4, err.RowExcel, fill, XlFont.Data, XlAlign.Center);
                ct.Cell(r, 5, err.MaLk, fill);
                ct.Cell(r, 6, err.MaDichVu, fill, XlFont.Data, XlAlign.Top);
                ct.Cell(r, 7, err.MoTa, fill, XlFont.Data, XlAlign.Top);
                ct.RowHeight(r, XlKit.RowHeightFor(err.MoTa, 25, 14));
            }
            ct.Finish(1);
            ct.RowHeight(1, 28);

            // ===== WARN_GG01 – thiếu/thừa giường (từ XML1) =====
            List<GiamDinhError> g1 = errors.Where(e => e.MaLyDo == "WARN.GG01").ToList();
            var g1Hdrs = new[] { "STT", "ROW_EXCEL", "MA_LK", "MÔ TẢ" };
            var s1 = new XlSheet(wb.Worksheets.Add("WARN_GG01_SoNgay"), g1Hdrs);
            for (int ci = 0; ci < g1Hdrs.Length; ci++) s1.Header(1, ci + 1, g1Hdrs[ci], "#C55A11");
            for (int idx = 1; idx <= g1.Count; idx++)
            {
                GiamDinhError err = g1[idx - 1];
                int r = idx + 1;
                string fill = idx % 2 == 1 ? FillG1 : XlSheet.FillEven;
                s1.Cell(r, 1, idx, fill, XlFont.Data, XlAlign.Center);
                s1.Cell(r, 2, err.RowExcel, fill, XlFont.Data, XlAlign.Center);
                s1.Cell(r, 3, err.MaLk, fill);
                s1.Cell(r, 4, err.MoTa, fill, XlFont.Data, XlAlign.Top);
                s1.RowHeight(r, XlKit.RowHeightFor(err.MoTa, 25, 14));
            }
            if (g1.Count == 0) s1.OkCell(2, 1, "✓ Không có lỗi");
            s1.Finish(1);
            s1.RowHeight(1, 28);

            // ===== WARN_GG02 – trùng giường cùng ngày (từ XML3) =====
            List<GiamDinhError> g2 = errors.Where(e => e.MaLyDo == "WARN.GG02").ToList();
            var g2Hdrs = new[] { "STT", "ROW_EXCEL", "MA_LK", "TEN_DICH_VU", "MÔ TẢ" };
            var s2 = new XlSheet(wb.Worksheets.Add("WARN_GG02_TrungNgay"), g2Hdrs);
            for (int ci = 0; ci < g2Hdrs.Length; ci++) s2.Header(1, ci + 1, g2Hdrs[ci], "#7F6000");
            for (int idx = 1; idx <= g2.Count; idx++)
            {
                GiamDinhError err = g2[idx - 1];
                int r = idx + 1;
                string fill = idx % 2 == 1 ? FillG2 : XlSheet.FillEven;
                s2.Cell(r, 1, idx, fill, XlFont.Data, XlAlign.Center);
                s2.Cell(r, 2, err.RowExcel, fill, XlFont.Data, XlAlign.Center);
                s2.Cell(r, 3, err.MaLk, fill);
                s2.Cell(r, 4, err.MaDichVu, fill, XlFont.Data, XlAlign.Top);
                s2.Cell(r, 5, err.MoTa, fill, XlFont.Data, XlAlign.Top);
                s2.RowHeight(r, XlKit.RowHeightFor(err.MoTa, 25, 14));
            }
            if (g2.Count == 0) s2.OkCell(2, 1, "✓ Không có lỗi");
            s2.Finish(1);
            s2.RowHeight(1, 28);

            string outFile = XlKit.OutPath(excelFile, "ngaygiuong");
            wb.SaveAs(outFile);
            return outFile;
        }
    }
}
