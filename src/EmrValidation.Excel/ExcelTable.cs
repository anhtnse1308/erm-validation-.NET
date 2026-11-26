using System;
using System.Collections.Generic;
using System.IO;
using ClosedXML.Excel;

namespace EmrValidation.Excel
{
    /// <summary>
    /// Bảng dữ liệu đọc từ sheet đầu tiên của một file Excel (dùng cho file danh mục).
    /// Dòng 1 là tiêu đề; mọi giá trị là chuỗi, ô trống là "".
    /// </summary>
    public sealed class ExcelTable
    {
        /// <summary>Tên các cột (theo thứ tự cột, bỏ cột tiêu đề trống, trùng tên lấy cột đầu).</summary>
        public IList<string> Headers { get; private set; }

        /// <summary>
        /// Các dòng dữ liệu (tên cột → giá trị). Phần tử thứ i ứng với dòng Excel i + 2,
        /// kể cả dòng trống ở giữa.
        /// </summary>
        public IList<IDictionary<string, string>> Rows { get; private set; }

        public ExcelTable(IList<string> headers, IList<IDictionary<string, string>> rows)
        {
            Headers = headers;
            Rows = rows;
        }

        public bool HasColumn(string name)
        {
            return Headers.Contains(name);
        }

        /// <summary>Giá trị ô theo tên cột; cột không tồn tại → "".</summary>
        public static string Get(IDictionary<string, string> row, string column)
        {
            string v;
            return row.TryGetValue(column, out v) ? (v ?? "") : "";
        }
    }

    /// <summary>Đọc file Excel danh mục thành <see cref="ExcelTable"/>.</summary>
    public static class ExcelTableReader
    {
        /// <summary>
        /// Đọc sheet ĐẦU TIÊN của file (theo vị trí, không phụ thuộc tên sheet),
        /// dòng 1 làm tiêu đề cột.
        /// </summary>
        public static ExcelTable ReadFirstSheet(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var wb = new XLWorkbook(fs))
            {
                if (wb.Worksheets.Count == 0)
                    return new ExcelTable(new List<string>(), new List<IDictionary<string, string>>());

                IXLWorksheet ws = wb.Worksheet(1);
                SheetData data = ExcelService.ReadSheet(ws, name => true);

                var rows = new List<IDictionary<string, string>>();
                foreach (var kv in data.Rows) rows.Add(kv.Value);
                return new ExcelTable(data.Headers, rows);
            }
        }
    }
}
