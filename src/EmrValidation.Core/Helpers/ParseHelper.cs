using System;
using System.Globalization;

namespace EmrValidation.Core.Helpers
{
    /// <summary>
    /// Các hàm parse dùng chung cho toàn bộ model XML
    /// (tương đương services/helper.py: to_float, to_int, decimal_places, parse_date12, parse_date8).
    /// </summary>
    public static class ParseHelper
    {
        /// <summary>Chuyển chuỗi → double. Trả null nếu rỗng hoặc không parse được.</summary>
        public static double? ToFloat(string value)
        {
            if (value == null || value.Trim().Length == 0) return null;
            double d;
            if (double.TryParse(value.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out d))
                return d;
            return null;
        }

        /// <summary>
        /// Chuyển chuỗi → số nguyên (qua double, cắt phần thập phân như int(float(x)) của Python).
        /// Trả null nếu rỗng hoặc không parse được.
        /// </summary>
        public static long? ToInt(string value)
        {
            double? d = ToFloat(value);
            if (d == null || double.IsNaN(d.Value) || double.IsInfinity(d.Value)) return null;
            double t = Math.Truncate(d.Value);
            if (t >= 9.2e18) return long.MaxValue;
            if (t <= -9.2e18) return long.MinValue;
            return (long)t;
        }

        /// <summary>int(x) của Python: chỉ nhận chuỗi số nguyên ("5", " 5 ", "+5"), không nhận "5.0".</summary>
        public static bool TryPyInt(string value, out long result)
        {
            result = 0;
            if (value == null) return false;
            return long.TryParse(value.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out result);
        }

        /// <summary>Đếm số chữ số thập phân của chuỗi số (dấu '.'). VD: '3.500' → 3, '100' → 0.</summary>
        public static int DecimalPlaces(string value)
        {
            string s = (value ?? "").Trim();
            if (s.IndexOf('.') < 0) return 0;
            return s.Split('.')[1].Length;
        }

        /// <summary>Parse DATE12 (yyyyMMddHHmm, 12 ký tự). Trả null nếu rỗng/sai độ dài/ngày giờ không hợp lệ.</summary>
        public static DateTime? ParseDate12(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            string s = value.Trim();
            if (s.Length != 12) return null;
            DateTime dt;
            if (DateTime.TryParseExact(s, "yyyyMMddHHmm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                return dt;
            return null;
        }

        /// <summary>Parse DATE8 (yyyyMMdd, 8 ký tự). Trả null nếu rỗng/sai độ dài/ngày không hợp lệ.</summary>
        public static DateTime? ParseDate8(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            string s = value.Trim();
            if (s.Length != 8) return null;
            DateTime dt;
            if (DateTime.TryParseExact(s, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                return dt;
            return null;
        }

        /// <summary>Chuỗi toàn chữ số (tương đương str.isdigit() với dữ liệu ASCII). Chuỗi rỗng → false.</summary>
        public static bool IsDigits(string s)
        {
            if (string.IsNullOrEmpty(s)) return false;
            for (int i = 0; i < s.Length; i++)
                if (!char.IsDigit(s[i])) return false;
            return true;
        }

        /// <summary>float(x) của Python có thành công không (dùng cho kiểm tra _NUMERIC).</summary>
        public static bool IsFloat(string value)
        {
            if (value == null) return false;
            double d;
            return double.TryParse(value.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out d);
        }
        /// <summary>Tách chuỗi nhiều giá trị phân cách ";", giữ nguyên vị trí, mỗi phần tử được Trim.</summary>
        public static string[] SplitList(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return new string[0];
            string[] parts = value.Split(';');
            for (int i = 0; i < parts.Length; i++) parts[i] = parts[i].Trim();
            return parts;
        }

        private static readonly System.Text.RegularExpressions.Regex MaDvktChuyenGiaoRx =
            new System.Text.RegularExpressions.Regex(@"^(\d{2}\.\d{4}\.\d{4})\.[A-Za-z]\.[A-Za-z0-9]{5}$");

        /// <summary>Mã DVKT để tra danh mục: mã chuyển giao XX.YYYY.ZZZZ.C.WWWWW được cắt về XX.YYYY.ZZZZ, mã khác giữ nguyên.</summary>
        public static string MaDvktGoc(string maDv)
        {
            string s = (maDv ?? "").Trim();
            System.Text.RegularExpressions.Match m = MaDvktChuyenGiaoRx.Match(s);
            return m.Success ? m.Groups[1].Value : s;
        }
    }
}
