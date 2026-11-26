using System;
using System.Globalization;
using System.Numerics;
using System.Text;

namespace EmrValidation.Core.Helpers
{
    /// <summary>
    /// Định dạng và làm tròn số giống Python để thông báo lỗi giữ nguyên chữ
    /// (VD: Python in 150000.0, round(x, 2) làm tròn chính xác theo giá trị nhị phân).
    /// </summary>
    public static class PyFormat
    {
        /// <summary>str(float) của Python. VD: 150000.0 → "150000.0", 0.5 → "0.5", 1e16 → "1e+16".</summary>
        public static string Str(double d)
        {
            if (double.IsNaN(d)) return "nan";
            if (double.IsPositiveInfinity(d)) return "inf";
            if (double.IsNegativeInfinity(d)) return "-inf";
            if (d == 0) return (1.0 / d < 0) ? "-0.0" : "0.0";

            bool neg = d < 0;
            double a = Math.Abs(d);

            // Tìm số chữ số ngắn nhất mà vẫn parse ngược lại đúng (shortest round-trip)
            string sci = null;
            for (int p = 1; p <= 17; p++)
            {
                string s = a.ToString("E" + (p - 1), CultureInfo.InvariantCulture);
                if (double.Parse(s, CultureInfo.InvariantCulture) == a) { sci = s; break; }
            }
            if (sci == null) sci = a.ToString("E16", CultureInfo.InvariantCulture);

            int ePos = sci.IndexOf('E');
            string mant = sci.Substring(0, ePos).Replace(".", "");
            int exp10 = int.Parse(sci.Substring(ePos + 1), CultureInfo.InvariantCulture);
            string digits = mant.TrimEnd('0');
            if (digits.Length == 0) digits = "0";
            int decpt = exp10 + 1;

            var sb = new StringBuilder();
            if (neg) sb.Append('-');

            if (decpt <= -4 || decpt > 16)
            {
                sb.Append(digits[0]);
                if (digits.Length > 1) sb.Append('.').Append(digits, 1, digits.Length - 1);
                int e = decpt - 1;
                sb.Append('e').Append(e < 0 ? '-' : '+');
                sb.Append(Math.Abs(e).ToString("00", CultureInfo.InvariantCulture));
            }
            else if (decpt <= 0)
            {
                sb.Append("0.").Append('0', -decpt).Append(digits);
            }
            else if (decpt >= digits.Length)
            {
                sb.Append(digits).Append('0', decpt - digits.Length).Append(".0");
            }
            else
            {
                sb.Append(digits, 0, decpt).Append('.').Append(digits, decpt, digits.Length - decpt);
            }
            return sb.ToString();
        }

        /// <summary>format(x, ",.0f") của Python: làm tròn về số nguyên (hòa → chẵn) và ngăn cách hàng nghìn bằng dấu phẩy.</summary>
        public static string Thousands0(double x)
        {
            double r = Round(x, 0);
            bool neg = r < 0 || (r == 0 && 1.0 / r < 0);
            string digits = new BigInteger(Math.Abs(r)).ToString(CultureInfo.InvariantCulture);

            var sb = new StringBuilder();
            if (neg) sb.Append('-');
            int first = digits.Length % 3;
            if (first == 0) first = 3;
            sb.Append(digits, 0, Math.Min(first, digits.Length));
            for (int i = first; i < digits.Length; i += 3)
                sb.Append(',').Append(digits, i, 3);
            return sb.ToString();
        }

        /// <summary>round(x, nd) của Python: làm tròn chính xác theo giá trị nhị phân, hòa → chẵn.</summary>
        public static double Round(double x, int nd)
        {
            if (double.IsNaN(x) || double.IsInfinity(x) || x == 0) return x;

            long bits = BitConverter.DoubleToInt64Bits(x);
            bool neg = bits < 0;
            int exp = (int)((bits >> 52) & 0x7FF);
            long mant = bits & 0xFFFFFFFFFFFFFL;
            if (exp == 0) exp++; else mant |= (1L << 52);
            exp -= 1075; // |x| = mant * 2^exp

            BigInteger num = new BigInteger(mant);
            BigInteger den = BigInteger.One;
            if (exp > 0) num <<= exp; else den <<= -exp;
            if (nd >= 0) num *= BigInteger.Pow(10, nd); else den *= BigInteger.Pow(10, -nd);

            BigInteger rem;
            BigInteger q = BigInteger.DivRem(num, den, out rem);
            int cmp = (rem * 2).CompareTo(den);
            if (cmp > 0 || (cmp == 0 && !q.IsEven)) q += 1;

            if (q.IsZero) return neg ? -0.0 : 0.0;

            string s = q.ToString(CultureInfo.InvariantCulture);
            if (nd > 0)
            {
                s = s.PadLeft(nd + 1, '0');
                s = s.Substring(0, s.Length - nd) + "." + s.Substring(s.Length - nd);
            }
            else if (nd < 0)
            {
                s = s + new string('0', -nd);
            }
            double res = double.Parse(s, CultureInfo.InvariantCulture);
            return neg ? -res : res;
        }
    }
}
