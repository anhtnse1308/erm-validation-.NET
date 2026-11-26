using System.Collections.Generic;
using EmrValidation.Core.Models;

namespace EmrValidation.Core.GiamDinh
{
    /// <summary>Một dòng dữ liệu của sheet kèm số dòng Excel (tính từ 2).</summary>
    public class SheetRow
    {
        public int RowExcel { get; private set; }
        public BhytBase Record { get; private set; }

        public SheetRow(int rowExcel, BhytBase record)
        {
            RowExcel = rowExcel;
            Record = record;
        }
    }

    /// <summary>
    /// Base class cho tất cả rule giám định.
    /// Mỗi văn bản/nhóm quy định → 1 class con.
    /// </summary>
    public abstract class GiamDinhBase
    {
        /// <summary>
        /// Nhận toàn bộ dữ liệu (sheet_name → danh sách model),
        /// trả về danh sách GiamDinhError.
        /// </summary>
        public abstract List<GiamDinhError> Check(IDictionary<string, List<BhytBase>> allObjects);

        /// <summary>Trả về danh sách (row_excel, record) của một sheet.</summary>
        protected static List<SheetRow> GetRows(IDictionary<string, List<BhytBase>> allObjects, string sheet)
        {
            var result = new List<SheetRow>();
            List<BhytBase> list;
            if (allObjects.TryGetValue(sheet, out list) && list != null)
            {
                for (int i = 0; i < list.Count; i++)
                    result.Add(new SheetRow(i + 2, list[i]));
            }
            return result;
        }

        /// <summary>Group danh sách dòng theo MA_LK (giữ thứ tự xuất hiện).</summary>
        protected static Dictionary<string, List<SheetRow>> GroupByMaLk(IEnumerable<SheetRow> rows)
        {
            var result = new Dictionary<string, List<SheetRow>>();
            foreach (var row in rows)
            {
                string key = row.Record.GetValue("MA_LK") ?? "";
                List<SheetRow> bucket;
                if (!result.TryGetValue(key, out bucket))
                {
                    bucket = new List<SheetRow>();
                    result[key] = bucket;
                }
                bucket.Add(row);
            }
            return result;
        }
    }
}
