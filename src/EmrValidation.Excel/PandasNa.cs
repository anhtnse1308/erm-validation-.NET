using System.Collections.Generic;

namespace EmrValidation.Excel
{
    /// <summary>
    /// Các chuỗi mà pandas.read_excel tự coi là "thiếu dữ liệu" (NaN → ô trống) khi đọc file.
    /// Bản Python đọc bằng pandas nên các ô chứa những chuỗi này luôn thành rỗng;
    /// để kết quả giống hệt, bản C# đổi chúng về rỗng khi đọc ô văn bản.
    /// </summary>
    public static class PandasNa
    {
        private static readonly HashSet<string> Values = new HashSet<string>
        {
            "-1.#IND", "1.#QNAN", "1.#IND", "-1.#QNAN", "#N/A N/A", "#N/A", "N/A", "n/a", "NA", "<NA>",
            "#NA", "NULL", "null", "NaN", "-NaN", "nan", "-nan", "None", "",
        };

        /// <summary>Chuỗi (so khớp chính xác, không trim) có bị pandas coi là NaN không.</summary>
        public static bool IsNa(string value)
        {
            return value != null && Values.Contains(value);
        }
    }
}
