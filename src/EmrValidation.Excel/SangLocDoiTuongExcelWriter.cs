using System;
using System.Collections.Generic;
using System.Linq;
using ClosedXML.Excel;
using EmrValidation.Core.GiamDinh;

namespace EmrValidation.Excel
{
    /// <summary>
    /// Xuất kết quả sàng lọc đối tượng KCB: &lt;ten_goc&gt;[&lt;timestamp&gt;]_sanglocdoituong.xlsx
    ///   TONG_HOP – thống kê theo mã cảnh báo
    ///   CHI_TIET – toàn bộ lỗi
    ///   Mỗi mã WARN_DT01..DT05 có lỗi → một sheet riêng
    /// </summary>
    public static class SangLocDoiTuongExcelWriter
    {
        private const string FillOdd = "#EBF3FB";

        private static readonly Dictionary<string, string> Fills = new Dictionary<string, string>
        {
            { "DT01", "#FCE4D6" },
            { "DT02", "#FFF2CC" },
            { "DT03", "#DDEBF7" },
            { "DT04", "#EAF1DD" },
            { "DT05", "#F4CCFF" },
        };

        private static readonly Dictionary<string, string> HdrColors = new Dictionary<string, string>
        {
            { "WARN.DT01", "#C55A11" },
            { "WARN.DT02", "#7F6000" },
            { "WARN.DT03", "#1F497D" },
            { "WARN.DT04", "#375623" },
            { "WARN.DT05", "#5B2C6F" },
        };

        private static readonly KeyValuePair<string, string>[] MaMoTa =
        {
            new KeyValuePair<string, string>("WARN.DT01", "MA_DOITUONG_KCB=1.14 nhưng MA_LOAI_KCB không phải ngoại trú (01,02,05,08)"),
            new KeyValuePair<string, string>("WARN.DT02", "MA_DOITUONG_KCB=1.15 nhưng MA_LOAI_KCB không phải nội trú (03,09)"),
            new KeyValuePair<string, string>("WARN.DT03", "MA_DKBD=74021 (đúng tuyến) nhưng MA_DOITUONG_KCB không phải 1.1 hoặc 2"),
            new KeyValuePair<string, string>("WARN.DT04", "MA_DKBD≠74021 (trái tuyến) nhưng MA_DOITUONG_KCB sai hoặc thiếu giấy chuyển"),
            new KeyValuePair<string, string>("WARN.DT05", "MA_DOITUONG_KCB=1.7 (sơ sinh) nhưng NGAY_VAO cách NGAY_SINH > 5 ngày"),
        };

        private static string KeyOf(string maLyDo)
        {
            return (maLyDo ?? "").Replace("WARN.", "");
        }

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

            // ===== TONG_HOP =====
            var thHdrs = new[] { "MÃ CẢNH BÁO", "QUY TẮC VI PHẠM", "SỐ TRƯỜNG HỢP" };
            var th = new XlSheet(wb.Worksheets.Add("TONG_HOP"), thHdrs);
            th.Title("SÀNG LỌC MÃ ĐỐI TƯỢNG KCB (MA_DOITUONG_KCB)  –  " + nowStr, 4, 28);
            for (int ci = 0; ci < thHdrs.Length; ci++) th.Header(2, ci + 1, thHdrs[ci]);

            int ri = 3;
            foreach (var m in MaMoTa)
            {
                int so;
                cnt.TryGetValue(m.Key, out so);
                string fill;
                if (!Fills.TryGetValue(KeyOf(m.Key), out fill)) fill = XlSheet.FillEven;
                th.Cell(ri, 1, m.Key, fill, XlFont.Data, XlAlign.Center);
                th.Cell(ri, 2, m.Value, fill);
                th.Cell(ri, 3, so, fill, so > 0 ? XlFont.RedBold : XlFont.Data, XlAlign.Center);
                ri++;
            }
            th.Cell(ri, 1, "TỔNG", XlSheet.FillSum, XlFont.WhiteBold, XlAlign.Center);
            th.Cell(ri, 2, "", XlSheet.FillSum);
            th.Cell(ri, 3, errors.Count, XlSheet.FillSum, XlFont.WhiteBold, XlAlign.Center);

            if (errors.Count == 0)
                th.Message(ri + 2, 4, "✓ Không phát hiện vi phạm mã đối tượng KCB.");
            th.Finish(2);

            // ===== CHI_TIET =====
            var ctHdrs = new[] { "STT", "MÃ CẢNH BÁO", "ROW_EXCEL", "MA_LK", "MÔ TẢ" };
            var ct = new XlSheet(wb.Worksheets.Add("CHI_TIET"), ctHdrs);
            for (int ci = 0; ci < ctHdrs.Length; ci++) ct.Header(1, ci + 1, ctHdrs[ci]);

            for (int idx = 1; idx <= errors.Count; idx++)
            {
                GiamDinhError err = errors[idx - 1];
                int r = idx + 1;
                string fill;
                if (!Fills.TryGetValue(KeyOf(err.MaLyDo), out fill))
                    fill = idx % 2 == 0 ? XlSheet.FillEven : FillOdd;
                ct.Cell(r, 1, idx, fill, XlFont.Data, XlAlign.Center);
                ct.Cell(r, 2, err.MaLyDo, fill, XlFont.RedBold, XlAlign.Center);
                ct.Cell(r, 3, err.RowExcel, fill, XlFont.Data, XlAlign.Center);
                ct.Cell(r, 4, err.MaLk, fill);
                ct.Cell(r, 5, err.MoTa, fill, XlFont.Data, XlAlign.Top);
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

            var gHdrs = new[] { "STT", "ROW_EXCEL", "MA_LK", "MÔ TẢ" };
            foreach (var g in groups.OrderBy(x => x.Key, StringComparer.Ordinal))
            {
                string sname = g.Key.Replace("WARN.", "WARN_").Replace(".", "_");
                if (sname.Length > 31) sname = sname.Substring(0, 31);
                var gs = new XlSheet(wb.Worksheets.Add(sname), gHdrs);

                string hdrColor;
                if (!HdrColors.TryGetValue(g.Key, out hdrColor)) hdrColor = XlSheet.FillHdr;
                for (int ci = 0; ci < gHdrs.Length; ci++) gs.Header(1, ci + 1, gHdrs[ci], hdrColor);

                string fillOdd;
                if (!Fills.TryGetValue(KeyOf(g.Key), out fillOdd)) fillOdd = FillOdd;

                for (int idx = 1; idx <= g.Value.Count; idx++)
                {
                    GiamDinhError err = g.Value[idx - 1];
                    int r = idx + 1;
                    string fill = idx % 2 == 1 ? fillOdd : XlSheet.FillEven;
                    gs.Cell(r, 1, idx, fill, XlFont.Data, XlAlign.Center);
                    gs.Cell(r, 2, err.RowExcel, fill, XlFont.Data, XlAlign.Center);
                    gs.Cell(r, 3, err.MaLk, fill);
                    gs.Cell(r, 4, err.MoTa, fill, XlFont.Data, XlAlign.Top);
                    gs.RowHeight(r, XlKit.RowHeightFor(err.MoTa, 25, 14));
                }
                gs.Finish(1);
                gs.RowHeight(1, 28);
            }

            string outFile = XlKit.OutPath(excelFile, "sanglocdoituong");
            wb.SaveAs(outFile);
            return outFile;
        }
    }
}
