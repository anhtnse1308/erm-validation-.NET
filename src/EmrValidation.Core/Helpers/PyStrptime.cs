using System;
using System.Text.RegularExpressions;

namespace EmrValidation.Core.Helpers
{
    /// <summary>
    /// Phân tích ngày giống datetime.strptime(s, "%Y%m%d") của Python:
    /// cho phép tháng/ngày 1 chữ số ("2024011" → 2024-01-01), phải khớp hết chuỗi.
    /// </summary>
    public static class PyStrptime
    {
        // Mẫu regex mà Python tạo ra cho "%Y%m%d"
        private static readonly Regex Ymd = new Regex(
            @"\A(\d\d\d\d)(1[0-2]|0[1-9]|[1-9])(3[0-1]|[1-2]\d|0[1-9]|[1-9]| [1-9])");

        public static bool TryParseYmd(string s, out DateTime result)
        {
            result = default(DateTime);
            if (s == null) return false;

            Match m = Ymd.Match(s);
            if (!m.Success || m.Length != s.Length) return false;   // "unconverted data remains"

            int y, mo, d;
            if (!int.TryParse(m.Groups[1].Value, out y)) return false;
            if (!int.TryParse(m.Groups[2].Value, out mo)) return false;
            if (!int.TryParse(m.Groups[3].Value.Trim(), out d)) return false;
            try
            {
                result = new DateTime(y, mo, d);
                return true;
            }
            catch (ArgumentOutOfRangeException)
            {
                return false;
            }
        }
    }
}
