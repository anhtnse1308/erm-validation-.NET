using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using ClosedXML.Excel;
using EmrValidation.Core.Helpers;
using EmrValidation.Core.Models;

namespace EmrValidation.Excel
{
    /// <summary>Kết quả đọc file Excel BHYT.</summary>
    public class ExcelReadResult
    {
        /// <summary>Tên sheet (XML1, XML2...) → danh sách model, theo thứ tự sheet trong file.</summary>
        public Dictionary<string, List<BhytBase>> AllObjects { get; private set; }

        /// <summary>Lỗi validate dạng "XML3 - Row 5 - nội dung lỗi" (không trùng lặp, giữ thứ tự).</summary>
        public List<string> Errors { get; private set; }

        public ExcelReadResult()
        {
            AllObjects = new Dictionary<string, List<BhytBase>>();
            Errors = new List<string>();
        }
    }

    /// <summary>Dữ liệu một sheet sau khi đọc: tên cột + các dòng (số dòng Excel, giá trị).</summary>
    internal sealed class SheetData
    {
        public List<string> Headers = new List<string>();
        public List<KeyValuePair<int, Dictionary<string, string>>> Rows = new List<KeyValuePair<int, Dictionary<string, string>>>();
    }

    /// <summary>Đọc file Excel BHYT (nhiều sheet XML) thành các model (tương đương services/excel_service.py).</summary>
    public static class ExcelService
    {
        /// <summary>
        /// Đọc toàn bộ các sheet có tên nằm trong <see cref="SheetClassMap"/>.
        /// Mọi giá trị được đọc dưới dạng chuỗi; dòng 1 là tiêu đề; dòng dữ liệu đánh số từ 2.
        /// </summary>
        public static ExcelReadResult ReadExcel(string excelFile)
        {
            var result = new ExcelReadResult();
            var seen = new HashSet<string>();

            // Mở chế độ chia sẻ để vẫn đọc được khi file đang mở trong Excel
            using (var fs = new FileStream(excelFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var wb = new XLWorkbook(fs))
            {
                foreach (IXLWorksheet ws in wb.Worksheets)
                {
                    Func<BhytBase> factory;
                    if (!SheetClassMap.Factories.TryGetValue(ws.Name, out factory))
                        continue;

                    // Chỉ đọc các cột có header trùng tên field của model
                    var validFields = new HashSet<string>(factory().FieldNames);

                    var objects = new List<BhytBase>();
                    foreach (var rowData in ReadRows(ws, validFields))
                    {
                        BhytBase obj = factory();
                        obj.LoadFrom(rowData.Value);
                        objects.Add(obj);

                        foreach (string err in obj.Validate())
                        {
                            string line = string.Format("{0} - Row {1} - {2}", ws.Name, rowData.Key, err);
                            if (seen.Add(line)) result.Errors.Add(line);
                        }
                    }
                    result.AllObjects[ws.Name] = objects;
                }
            }
            return result;
        }

        /// <summary>
        /// Đọc các dòng dữ liệu của sheet: (số dòng Excel, tên cột → giá trị).
        /// Chỉ đọc những cột có header (dòng 1) trùng tên field trong <paramref name="validFields"/>;
        /// các cột khác (sai tên, thừa, nằm lệch vị trí) bị bỏ qua hoàn toàn.
        /// Giữ cả dòng trống ở giữa (để số dòng khớp file gốc).
        /// </summary>
        private static IEnumerable<KeyValuePair<int, Dictionary<string, string>>> ReadRows(
            IXLWorksheet ws, ICollection<string> validFields)
        {
            return ReadSheet(ws, name => validFields.Contains(name)).Rows;
        }

        /// <summary>
        /// Đọc một sheet: tiêu đề ở dòng 1 (chỉ giữ cột có tên thỏa <paramref name="accept"/>, trùng tên lấy cột đầu),
        /// dữ liệu từ dòng 2 đến dòng cuối, giữ cả dòng trống ở giữa.
        /// </summary>
        internal static SheetData ReadSheet(IXLWorksheet ws, Func<string, bool> accept)
        {
            var data = new SheetData();

            IXLCell lastCell = ws.LastCellUsed();
            if (lastCell == null) return data;

            int lastRow = lastCell.Address.RowNumber;

            // Bước 1: header (dòng 1)
            var headerByCol = new Dictionary<int, string>();
            var usedNames = new HashSet<string>();
            foreach (IXLCell cell in ws.Row(1).CellsUsed())
            {
                string name = CellText(cell);
                if (name.Length > 0 && accept(name) && usedNames.Add(name))
                {
                    headerByCol[cell.Address.ColumnNumber] = name;
                    data.Headers.Add(name);
                }
            }
            if (headerByCol.Count == 0) return data;

            // Bước 2: chỉ đọc các ô thuộc cột hợp lệ, gom theo dòng
            var rows = new Dictionary<int, Dictionary<int, string>>();
            foreach (IXLCell cell in ws.CellsUsed())
            {
                int r = cell.Address.RowNumber;
                int c = cell.Address.ColumnNumber;
                if (r == 1 || !headerByCol.ContainsKey(c)) continue;

                Dictionary<int, string> row;
                if (!rows.TryGetValue(r, out row))
                {
                    row = new Dictionary<int, string>();
                    rows[r] = row;
                }
                row[c] = CellText(cell, true);
            }

            for (int r = 2; r <= lastRow; r++)
            {
                var dict = new Dictionary<string, string>();
                Dictionary<int, string> cells;
                rows.TryGetValue(r, out cells);
                foreach (var h in headerByCol)
                {
                    string v = "";
                    if (cells != null) cells.TryGetValue(h.Key, out v);
                    dict[h.Value] = v ?? "";
                }
                data.Rows.Add(new KeyValuePair<int, Dictionary<string, string>>(r, dict));
            }
            return data;
        }

        /// <summary>
        /// Giá trị ô → chuỗi, giống cách pandas (dtype=str) đọc:
        /// số nguyên lưu dạng "20240101", số thực dạng "5.5", ngày dạng "2024-01-01 00:00:00".
        /// </summary>
        private static string CellText(IXLCell cell, bool applyNa = false)
        {
            try
            {
                if (cell.IsEmpty()) return "";
                switch (cell.DataType)
                {
                    case XLDataType.Number:
                        return NumberText(cell.GetDouble());
                    case XLDataType.DateTime:
                        return cell.GetDateTime().ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    case XLDataType.Boolean:
                        return cell.GetBoolean() ? "True" : "False";
                    default:
                    {
                        string text = cell.GetString() ?? "";
                        // pandas coi "NA", "N/A", "NULL", "nan", "None"... là ô trống
                        return applyNa && PandasNa.IsNa(text) ? "" : text;
                    }
                }
            }
            catch (Exception)
            {
                // Ô lỗi (#N/A, #REF!...) hoặc giá trị không đổi được sang chuỗi:
                // thử lấy giá trị thô, không được thì coi như ô trống
                try
                {
                    object v = cell.Value;
                    return v == null ? "" : Convert.ToString(v, CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                    return "";
                }
            }
        }

        private static string NumberText(double d)
        {
            if (d == Math.Floor(d) && Math.Abs(d) < 1e15)
                return ((long)d).ToString(CultureInfo.InvariantCulture);
            return PyFormat.Str(d);
        }
    }
}
