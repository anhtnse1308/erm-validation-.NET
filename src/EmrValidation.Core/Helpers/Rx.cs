using System.Text.RegularExpressions;

namespace EmrValidation.Core.Helpers
{
    /// <summary>Tương đương re.fullmatch / re.search của Python.</summary>
    public static class Rx
    {
        /// <summary>Toàn bộ chuỗi phải khớp mẫu (re.fullmatch).</summary>
        public static bool Full(string pattern, string input, RegexOptions options = RegexOptions.None)
        {
            return input != null && Regex.IsMatch(input, "^(?:" + pattern + ")\\z", options);
        }

        /// <summary>Tìm mẫu ở bất kỳ vị trí nào trong chuỗi (re.search).</summary>
        public static bool Search(string pattern, string input, RegexOptions options = RegexOptions.None)
        {
            return input != null && Regex.IsMatch(input, pattern, options);
        }
    }
}
