using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using ClosedXML.Excel;
using EmrValidation.Core.Models;

namespace EmrValidation.Excel
{
    /// <summary>
    /// Ghi log và báo cáo kết quả validate (tương đương class FileHelper trong services/helper.py).
    /// Mọi hàm Write* trả về đường dẫn file đã ghi, đặt cạnh file Excel gốc.
    /// </summary>
    public static class ReportWriter
    {
        private static readonly Encoding Utf8NoBom = new UTF8Encoding(false);

        // ------------------------------------------------------------------
        // Tiện ích chung
        // ------------------------------------------------------------------
        private static string Folder(string excelFile)
        {
            return Path.GetDirectoryName(Path.GetFullPath(excelFile));
        }

        private static string BaseName(string excelFile)
        {
            return Path.GetFileNameWithoutExtension(excelFile);
        }

        private static string Stamp()
        {
            return DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);
        }

        private static string Now()
        {
            return DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
        }

        private static string OutPath(string excelFile, string suffix, string ext)
        {
            return Path.Combine(Folder(excelFile),
                string.Format("{0}[{1}]_{2}.{3}", BaseName(excelFile), Stamp(), suffix, ext));
        }

        /// <summary>Ghi các dòng text (UTF-8 không BOM); ký tự '\n' trong dòng được đổi theo hệ điều hành như open(..., "w") của Python.</summary>
        private static void WriteTextLines(string path, IEnumerable<string> lines)
        {
            using (var w = new StreamWriter(path, false, Utf8NoBom))
            {
                foreach (string line in lines)
                {
                    w.Write(line.Replace("\n", Environment.NewLine));
                    w.Write(Environment.NewLine);
                }
            }
        }

        // ------------------------------------------------------------------
        // Log text
        // ------------------------------------------------------------------

        /// <summary>Ghi log dạng text thuần: &lt;ten_excel&gt;[&lt;timestamp&gt;].txt</summary>
        public static string WriteLog(string excelFile, IEnumerable<string> logs)
        {
            string path = Path.Combine(Folder(excelFile),
                string.Format("{0}[{1}].txt", BaseName(excelFile), Stamp()));
            WriteTextLines(path, logs);
            return path;
        }

        /// <summary>Nội dung tổng hợp dùng chung cho console lẫn file.</summary>
        public static List<string> RenderSummaryLines(ValidationReport report)
        {
            var lines = new List<string>();
            lines.Add("Thời gian: " + Now());
            lines.Add(string.Format("Tổng: {0} dòng | Lỗi: {1} | OK: {2}",
                report.TotalRows, report.TotalErrorRows, report.TotalRows - report.TotalErrorRows));

            foreach (SheetValidation sheet in report.Sheets)
            {
                lines.Add("\n" + new string('=', 60));
                lines.Add(string.Format("  SHEET: {0}  ({1} dòng)", sheet.Name, sheet.Records.Count));
                lines.Add(new string('=', 60));
                bool hasError = false;
                for (int i = 0; i < sheet.Records.Count; i++)
                {
                    int excelRow = i + 2;
                    List<string> errors = sheet.Errors[i];
                    if (errors.Count > 0)
                    {
                        hasError = true;
                        lines.Add(string.Format("\n  [LOI] Row Excel {0}:", excelRow));
                        foreach (string e in errors)
                            lines.Add("       - " + e);
                    }
                    else
                    {
                        lines.Add(string.Format("  [OK]  Row Excel {0}", excelRow));
                    }
                }
                if (!hasError)
                    lines.Add("  -> Khong co loi.");
            }
            return lines;
        }

        /// <summary>Ghi log tổng hợp, nội dung y chang in ra console: &lt;ten_excel&gt;[&lt;timestamp&gt;]_summary.txt</summary>
        public static string WriteLogSummary(string excelFile, ValidationReport report)
        {
            string path = OutPath(excelFile, "summary", "txt");
            WriteTextLines(path, RenderSummaryLines(report));
            return path;
        }

        /// <summary>Ghi file txt CHỈ chứa các dòng có lỗi: &lt;ten_excel&gt;[&lt;timestamp&gt;]_errors_only.txt</summary>
        public static string WriteLogErrorsOnly(string excelFile, ValidationReport report)
        {
            string path = OutPath(excelFile, "errors_only", "txt");

            var lines = new List<string>();
            lines.Add("Thời gian  : " + Now());
            lines.Add("File       : " + Path.GetFileName(excelFile));
            lines.Add(string.Format("Tổng       : {0} dòng | Lỗi: {1} | OK: {2}",
                report.TotalRows, report.TotalErrorRows, report.TotalRows - report.TotalErrorRows));

            foreach (SheetValidation sheet in report.Sheets)
            {
                var sheetErrorLines = new List<string>();
                for (int i = 0; i < sheet.Records.Count; i++)
                {
                    List<string> errs = sheet.Errors[i];
                    if (errs.Count > 0)
                    {
                        sheetErrorLines.Add(string.Format("\n  [LOI] Row Excel {0}:", i + 2));
                        foreach (string e in errs)
                            sheetErrorLines.Add("       - " + e);
                    }
                }

                int errCount = 0;
                foreach (string l in sheetErrorLines)
                    if (l.Contains("[LOI]")) errCount++;

                lines.Add("\n" + new string('=', 60));
                lines.Add(string.Format("  SHEET: {0}  ({1} dòng | {2} lỗi)",
                    sheet.Name, sheet.Records.Count, errCount));
                lines.Add(new string('=', 60));

                if (sheetErrorLines.Count > 0) lines.AddRange(sheetErrorLines);
                else lines.Add("  -> Không có lỗi.");
            }

            WriteTextLines(path, lines);
            return path;
        }

        /// <summary>In lỗi ra console theo định dạng dễ đọc.</summary>
        public static void PrintErrorsConsole(ValidationReport report)
        {
            foreach (string line in RenderSummaryLines(report))
                Console.WriteLine(line);
        }

        // ------------------------------------------------------------------
        // Log JSON
        // ------------------------------------------------------------------

        /// <summary>
        /// Ghi log lỗi dạng JSON kèm toàn bộ dữ liệu của dòng lỗi:
        /// &lt;ten_excel&gt;[&lt;timestamp&gt;]_&lt;suffix&gt;.json.
        /// Chỉ ghi các dòng CÓ LỖI vào mảng "errors"; dòng hợp lệ chỉ được đếm vào "total".
        /// </summary>
        public static string WriteLogJson(string excelFile, ValidationReport report, string suffix = "errors")
        {
            string path = OutPath(excelFile, suffix, "json");

            var sheetsData = new MiniJson.JsonObject();
            foreach (SheetValidation sheet in report.Sheets)
            {
                var sheetErrors = new List<object>();
                for (int i = 0; i < sheet.Records.Count; i++)
                {
                    List<string> errs = sheet.Errors[i];
                    if (errs.Count == 0) continue;

                    BhytBase record = sheet.Records[i];
                    var rowData = new MiniJson.JsonObject();
                    foreach (string name in record.FieldNames)
                        rowData.Add(name, record.GetValue(name));

                    var errList = new List<object>();
                    foreach (string e in errs) errList.Add(e);

                    sheetErrors.Add(new MiniJson.JsonObject()
                        .Add("row_excel", i + 2)
                        .Add("errors", errList)
                        .Add("data", rowData));
                }

                sheetsData.Add(sheet.Name, new MiniJson.JsonObject()
                    .Add("total", sheet.Records.Count)
                    .Add("error_count", sheetErrors.Count)
                    .Add("errors", sheetErrors));
            }

            var output = new MiniJson.JsonObject()
                .Add("file", Path.GetFileName(excelFile))
                .Add("generated_at", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture))
                .Add("summary", new MiniJson.JsonObject()
                    .Add("total_rows", report.TotalRows)
                    .Add("error_rows", report.TotalErrorRows)
                    .Add("ok_rows", report.TotalRows - report.TotalErrorRows))
                .Add("sheets", sheetsData);

            File.WriteAllText(path, MiniJson.Serialize(output), Utf8NoBom);
            return path;
        }

        // ------------------------------------------------------------------
        // Báo cáo Excel
        // ------------------------------------------------------------------

        /// <summary>
        /// Xuất toàn bộ dữ liệu ra Excel, mỗi sheet = 1 XML. Thêm 2 cột cuối: STATUS (OK/LOI) và ERRORS.
        /// Đỏ nhạt = lỗi, xanh nhạt = OK. Tên: &lt;ten_excel&gt;[&lt;timestamp&gt;]_summary.xlsx
        /// </summary>
        public static string WriteExcelSummary(string excelFile, ValidationReport report)
        {
            var wb = new XLWorkbook();
            foreach (SheetValidation sheet in report.Sheets)
            {
                if (sheet.Records.Count == 0) continue;
                IXLWorksheet ws = wb.Worksheets.Add(sheet.Name);

                IReadOnlyList<string> colNames = sheet.Records[0].FieldNames;
                int nCols = colNames.Count;
                var headers = new List<string>(colNames) { "STATUS", "ERRORS" };
                int[] maxW = NewWidths(headers);

                WriteHeaderRow(ws, headers, ExcelStyle.FillHdrSummary);

                for (int i = 0; i < sheet.Records.Count; i++)
                {
                    int ri = i + 2;
                    BhytBase record = sheet.Records[i];
                    List<string> errs = sheet.Errors[i];
                    bool isOk = errs.Count == 0;
                    string fill = isOk ? ExcelStyle.FillRowOk : ExcelStyle.FillRowErr;

                    for (int c = 0; c < nCols; c++)
                    {
                        string val = record.GetValue(colNames[c]);
                        if (val != null)
                        {
                            SetText(ws.Cell(ri, c + 1), val);
                            Track(maxW, c, val);
                        }
                    }
                    StyleRange(ws.Range(ri, 1, ri, nCols), fill, null, false);

                    // Cột STATUS
                    string status = isOk ? "OK" : "LOI";
                    SetText(ws.Cell(ri, nCols + 1), status);
                    Track(maxW, nCols, status);
                    StyleRange(ws.Range(ri, nCols + 1, ri, nCols + 1), fill,
                        isOk ? null : ExcelStyle.FontErr, true);

                    // Cột ERRORS
                    string errText = isOk ? "" : string.Join("\n", Bullets(errs));
                    if (errText.Length > 0)
                    {
                        SetText(ws.Cell(ri, nCols + 2), errText);
                        Track(maxW, nCols + 1, errText);
                    }
                    StyleRange(ws.Range(ri, nCols + 2, ri, nCols + 2), fill,
                        isOk ? null : ExcelStyle.FontErr, false);
                }

                ApplyWidths(ws, maxW);
                ws.SheetView.FreezeRows(1);
                ws.Row(1).Height = ExcelStyle.RowHdrHeight;
            }

            string outFile = OutPath(excelFile, "summary", "xlsx");
            SaveWorkbook(wb, outFile);
            return outFile;
        }

        /// <summary>
        /// Xuất chỉ các dòng LỖI ra Excel, mỗi sheet = 1 XML, có thêm cột ROW_EXCEL để tra ngược.
        /// Sheet không có lỗi vẫn xuất header + dòng thông báo sạch.
        /// Tên: &lt;ten_excel&gt;[&lt;timestamp&gt;]_errors_only.xlsx
        /// </summary>
        public static string WriteExcelErrorsOnly(string excelFile, ValidationReport report)
        {
            var wb = new XLWorkbook();
            foreach (SheetValidation sheet in report.Sheets)
            {
                if (sheet.Records.Count == 0) continue;
                IXLWorksheet ws = wb.Worksheets.Add(sheet.Name);

                IReadOnlyList<string> colNames = sheet.Records[0].FieldNames;
                int nCols = colNames.Count;
                var headers = new List<string>();
                headers.Add("ROW_EXCEL");
                headers.AddRange(colNames);
                headers.Add("ERRORS");
                int[] maxW = NewWidths(headers);

                WriteHeaderRow(ws, headers, ExcelStyle.FillHdrErrors);

                int writeRow = 1;   // dòng cuối đã ghi (ws.max_row của bản Python)
                int errorRows = 0;
                for (int i = 0; i < sheet.Records.Count; i++)
                {
                    List<string> errs = sheet.Errors[i];
                    if (errs.Count == 0) continue;
                    errorRows++;
                    writeRow++;
                    BhytBase record = sheet.Records[i];

                    // Cột ROW_EXCEL
                    ws.Cell(writeRow, 1).SetValue(i + 2);
                    Track(maxW, 0, (i + 2).ToString(CultureInfo.InvariantCulture));
                    StyleRange(ws.Range(writeRow, 1, writeRow, 1), ExcelStyle.FillRowErr, null, true);

                    // Cột dữ liệu
                    for (int c = 0; c < nCols; c++)
                    {
                        string val = record.GetValue(colNames[c]);
                        if (val != null)
                        {
                            SetText(ws.Cell(writeRow, c + 2), val);
                            Track(maxW, c + 1, val);
                        }
                    }
                    StyleRange(ws.Range(writeRow, 2, writeRow, nCols + 1), ExcelStyle.FillRowErr, null, false);

                    // Cột ERRORS
                    string errText = string.Join("\n", Bullets(errs));
                    SetText(ws.Cell(writeRow, nCols + 2), errText);
                    Track(maxW, nCols + 1, errText);
                    StyleRange(ws.Range(writeRow, nCols + 2, writeRow, nCols + 2),
                        ExcelStyle.FillRowErr, ExcelStyle.FontErr, false);
                }

                // Nếu sheet sạch → ghi 1 dòng thông báo
                if (errorRows == 0)
                {
                    string msg = string.Format("✓ {0}: không có lỗi ({1} dòng)", sheet.Name, sheet.Records.Count);
                    IXLCell msgCell = ws.Cell(2, 1);
                    SetText(msgCell, msg);
                    Track(maxW, 0, msg);
                    msgCell.Style.Fill.BackgroundColor = XLColor.FromHtml(ExcelStyle.FillSheetClean);
                    msgCell.Style.Font.FontName = ExcelStyle.FontName;
                    msgCell.Style.Font.FontSize = ExcelStyle.FontSize;
                    msgCell.Style.Font.Italic = true;
                    msgCell.Style.Font.FontColor = XLColor.FromHtml(ExcelStyle.FontClean);
                    ws.Range(2, 1, 2, headers.Count).Merge();
                }

                ApplyWidths(ws, maxW);
                ws.SheetView.FreezeRows(1);      // freeze cả ROW_EXCEL + header (B2)
                ws.SheetView.FreezeColumns(1);
                ws.Row(1).Height = ExcelStyle.RowHdrHeight;
            }

            string outFile = OutPath(excelFile, "errors_only", "xlsx");
            SaveWorkbook(wb, outFile);
            return outFile;
        }

        // ------------------------------------------------------------------
        // Helper Excel
        // ------------------------------------------------------------------
        private static IEnumerable<string> Bullets(IEnumerable<string> errs)
        {
            foreach (string e in errs) yield return "• " + e;
        }

        /// <summary>Lưu giá trị dạng VĂN BẢN (không để ClosedXML tự đổi "0012" thành số 12).</summary>
        private static void SetText(IXLCell cell, string text)
        {
            cell.SetValue(text);
            if (cell.DataType != XLDataType.Text)
                cell.DataType = XLDataType.Text;
        }

        private static int[] NewWidths(IList<string> headers)
        {
            var w = new int[headers.Count];
            for (int i = 0; i < w.Length; i++) w[i] = headers[i].Length;
            return w;
        }

        /// <summary>Cập nhật độ rộng cột theo dòng đầu của giá trị (tối đa 60 ký tự).</summary>
        private static void Track(int[] maxW, int colIndex, string value)
        {
            int nl = value.IndexOf('\n');
            int len = nl >= 0 ? nl : value.Length;
            len = Math.Min(len, ExcelStyle.ColWidthMax);
            if (len > maxW[colIndex]) maxW[colIndex] = len;
        }

        private static void ApplyWidths(IXLWorksheet ws, int[] maxW)
        {
            for (int i = 0; i < maxW.Length; i++)
                ws.Column(i + 1).Width = maxW[i] + ExcelStyle.ColWidthPad;
        }

        private static void WriteHeaderRow(IXLWorksheet ws, IList<string> headers, string fill)
        {
            for (int ci = 0; ci < headers.Count; ci++)
                SetText(ws.Cell(1, ci + 1), headers[ci]);

            IXLRange range = ws.Range(1, 1, 1, headers.Count);
            range.Style.Fill.BackgroundColor = XLColor.FromHtml(fill);
            range.Style.Font.Bold = true;
            range.Style.Font.FontColor = XLColor.FromHtml(ExcelStyle.FontWhite);
            range.Style.Font.FontName = ExcelStyle.FontName;
            range.Style.Font.FontSize = ExcelStyle.FontSize;
            range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            range.Style.Alignment.WrapText = true;
            ApplyBorder(range);
        }

        /// <summary>Style cho một dải ô cùng dòng: nền, font, căn lề, viền.</summary>
        private static void StyleRange(IXLRange range, string fill, string fontColor, bool center)
        {
            range.Style.Fill.BackgroundColor = XLColor.FromHtml(fill);
            range.Style.Font.FontName = ExcelStyle.FontName;
            range.Style.Font.FontSize = ExcelStyle.FontSize;
            if (fontColor != null)
                range.Style.Font.FontColor = XLColor.FromHtml(fontColor);
            range.Style.Alignment.Horizontal = center
                ? XLAlignmentHorizontalValues.Center : XLAlignmentHorizontalValues.General;
            range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            range.Style.Alignment.WrapText = true;
            ApplyBorder(range);
        }

        private static void ApplyBorder(IXLRange range)
        {
            range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            range.Style.Border.OutsideBorderColor = XLColor.FromHtml(ExcelStyle.BorderColor);
            range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            range.Style.Border.InsideBorderColor = XLColor.FromHtml(ExcelStyle.BorderColor);
        }

        private static void SaveWorkbook(XLWorkbook wb, string path)
        {
            // Workbook không có sheet nào (dữ liệu rỗng) vẫn cần 1 sheet để Excel mở được
            if (wb.Worksheets.Count == 0)
                wb.Worksheets.Add("EMPTY");
            wb.SaveAs(path);
        }

        // ------------------------------------------------------------------
        // Overload nhận trực tiếp all_objects (tự validate)
        // ------------------------------------------------------------------
        public static string WriteLogSummary(string excelFile, IDictionary<string, List<BhytBase>> allObjects)
        { return WriteLogSummary(excelFile, ValidationReport.Build(allObjects)); }

        public static string WriteLogErrorsOnly(string excelFile, IDictionary<string, List<BhytBase>> allObjects)
        { return WriteLogErrorsOnly(excelFile, ValidationReport.Build(allObjects)); }

        public static string WriteLogJson(string excelFile, IDictionary<string, List<BhytBase>> allObjects, string suffix = "errors")
        { return WriteLogJson(excelFile, ValidationReport.Build(allObjects), suffix); }

        public static string WriteExcelSummary(string excelFile, IDictionary<string, List<BhytBase>> allObjects)
        { return WriteExcelSummary(excelFile, ValidationReport.Build(allObjects)); }

        public static string WriteExcelErrorsOnly(string excelFile, IDictionary<string, List<BhytBase>> allObjects)
        { return WriteExcelErrorsOnly(excelFile, ValidationReport.Build(allObjects)); }
    }
}
