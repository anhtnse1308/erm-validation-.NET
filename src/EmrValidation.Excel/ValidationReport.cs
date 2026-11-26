using System.Collections.Generic;
using EmrValidation.Core.Models;

namespace EmrValidation.Excel
{
    /// <summary>Kết quả validate của một sheet: lỗi của từng dòng (cùng thứ tự với Records).</summary>
    public class SheetValidation
    {
        public string Name { get; private set; }
        public IList<BhytBase> Records { get; private set; }
        /// <summary>Errors[i] là danh sách lỗi của Records[i] (rỗng nếu dòng hợp lệ).</summary>
        public IList<List<string>> Errors { get; private set; }
        public int ErrorRowCount { get; private set; }

        public SheetValidation(string name, IList<BhytBase> records)
        {
            Name = name;
            Records = records;
            Errors = new List<List<string>>(records.Count);
            foreach (var rec in records)
            {
                List<string> errs = rec.Validate();
                if (errs.Count > 0) ErrorRowCount++;
                Errors.Add(errs);
            }
        }
    }

    /// <summary>
    /// Validate toàn bộ dữ liệu MỘT lần, dùng chung cho nhiều hàm ghi log/Excel
    /// (bản Python gọi validate() lại ở mỗi lần ghi).
    /// </summary>
    public class ValidationReport
    {
        public IList<SheetValidation> Sheets { get; private set; }
        public int TotalRows { get; private set; }
        public int TotalErrorRows { get; private set; }

        private ValidationReport() { Sheets = new List<SheetValidation>(); }

        public static ValidationReport Build(IDictionary<string, List<BhytBase>> allObjects)
        {
            var report = new ValidationReport();
            foreach (var kv in allObjects)
            {
                var sheet = new SheetValidation(kv.Key, kv.Value);
                report.Sheets.Add(sheet);
                report.TotalRows += kv.Value.Count;
                report.TotalErrorRows += sheet.ErrorRowCount;
            }
            return report;
        }
    }
}
