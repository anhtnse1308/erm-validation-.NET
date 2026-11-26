using System;
using System.Collections.Generic;
using EmrValidation.Core.GiamDinh;

namespace EmrValidation.Excel
{
    /// <summary>
    /// Bộ ghi Excel dùng chung cho các luồng cảnh báo đơn mã: MA_MAY, VTYT kèm DVKT, DVKT kèm ICD_REQ.
    /// Tên file: &lt;ten_goc&gt;[&lt;timestamp&gt;]_&lt;hauTo&gt;.xlsx
    ///   TONG_HOP – số lỗi của mã cảnh báo
    ///   CHI_TIET – toàn bộ lỗi theo các cột do rule truyền vào (cột cuối nên là "MÔ TẢ")
    /// </summary>
    public static class CanhBaoExcelWriter
    {
        private const string FillErr = "#FCE4D6";

        /// <summary>Một mã cảnh báo duy nhất (MA_MAY, VTYT kèm DVKT).</summary>
        /// <param name="rowFn">(stt, lỗi) → giá trị từng cột (chuỗi hoặc số nguyên), đúng thứ tự <paramref name="headers"/></param>
        public static string Write(
            string excelFile,
            IList<GiamDinhError> errors,
            string tieuDe,
            string hauTo,
            string maCanhBao,
            string yNghia,
            string thongDiepOk,
            IList<string> headers,
            Func<int, GiamDinhError, object[]> rowFn)
        {
            return WriteNhieuMa(excelFile, errors, tieuDe, hauTo,
                new[] { new KeyValuePair<string, string>(maCanhBao, yNghia) },
                thongDiepOk, headers, rowFn);
        }

        /// <summary>
        /// Nhiều mã cảnh báo trong cùng một file (VD WARN.DV01 và WARN.DV02): TONG_HOP có một dòng cho mỗi mã
        /// (theo thứ tự <paramref name="cacMa"/>: mã, ý nghĩa), CHI_TIET chứa lỗi của tất cả các mã.
        /// </summary>
        public static string WriteNhieuMa(
            string excelFile,
            IList<GiamDinhError> errors,
            string tieuDe,
            string hauTo,
            IList<KeyValuePair<string, string>> cacMa,
            string thongDiepOk,
            IList<string> headers,
            Func<int, GiamDinhError, object[]> rowFn)
        {
            var wb = new ClosedXML.Excel.XLWorkbook();
            string nowStr = XlKit.Now();

            // ===== TONG_HOP =====
            var thHdrs = new[] { "MÃ CẢNH BÁO", "Ý NGHĨA", "SỐ DÒNG" };
            var th = new XlSheet(wb.Worksheets.Add("TONG_HOP"), thHdrs);
            th.Title(tieuDe + "  –  " + nowStr, 4, 28);
            for (int ci = 0; ci < thHdrs.Length; ci++) th.Header(2, ci + 1, thHdrs[ci]);

            int ri = 3;
            foreach (KeyValuePair<string, string> ma in cacMa)
            {
                int soMa = 0;
                foreach (GiamDinhError e in errors)
                    if (e.MaLyDo == ma.Key) soMa++;
                string fillMa = soMa > 0 ? FillErr : XlSheet.FillOk;
                th.Cell(ri, 1, ma.Key, fillMa, XlFont.Data, XlAlign.Center);
                th.Cell(ri, 2, ma.Value, fillMa);
                th.Cell(ri, 3, soMa, fillMa, soMa > 0 ? XlFont.RedBold : XlFont.Data, XlAlign.Center);
                ri++;
            }

            int so = errors.Count;
            th.Cell(ri, 1, "TỔNG", XlSheet.FillSum, XlFont.WhiteBold, XlAlign.Center);
            th.Cell(ri, 2, "", XlSheet.FillSum);
            th.Cell(ri, 3, so, XlSheet.FillSum, XlFont.WhiteBold, XlAlign.Center);

            if (so == 0) th.Message(ri + 2, 4, thongDiepOk);
            th.Finish(2);

            // ===== CHI_TIET =====
            var ct = new XlSheet(wb.Worksheets.Add("CHI_TIET"), headers);
            for (int ci = 0; ci < headers.Count; ci++) ct.Header(1, ci + 1, headers[ci]);

            for (int idx = 1; idx <= errors.Count; idx++)
            {
                int r = idx + 1;
                string rowFill = idx % 2 == 1 ? FillErr : XlSheet.FillEven;
                object[] values = rowFn(idx, errors[idx - 1]);
                for (int ci = 0; ci < values.Length; ci++)
                {
                    string h = headers[ci];
                    XlAlign align = (h == "STT" || h == "ROW_EXCEL" || h == "MÃ CẢNH BÁO") ? XlAlign.Center
                                  : h == "MÔ TẢ" ? XlAlign.Top : XlAlign.Left;
                    ct.Cell(r, ci + 1, values[ci], rowFill, XlFont.Data, align);
                }
                string last = values[values.Length - 1] as string ?? Convert.ToString(values[values.Length - 1]);
                ct.RowHeight(r, XlKit.RowHeightFor(last, 25, 14));
            }
            ct.Finish(1);
            ct.RowHeight(1, 28);

            string outFile = XlKit.OutPath(excelFile, hauTo);
            wb.SaveAs(outFile);
            return outFile;
        }
    }
}