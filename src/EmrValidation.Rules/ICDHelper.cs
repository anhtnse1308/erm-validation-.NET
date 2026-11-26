using System;
using System.Collections.Generic;
using System.Linq;
using EmrValidation.Core.Models;

namespace EmrValidation.Rules
{
    /// <summary>
    /// Tiện ích xử lý và so khớp mã ICD-10 dùng chung cho các rule giám định.
    ///
    /// Cấu trúc dữ liệu ICD trong XML1:
    ///   MA_BENH_CHINH : 1 mã duy nhất, VD "E11.9"
    ///   MA_BENH_KT    : nhiều mã phân cách ";", VD "E78.2;I10;K58.9;Z76.0"
    ///
    /// So khớp hỗ trợ prefix:
    ///   - "N18.5" khớp ref "N18" (mã cụ thể thuộc nhóm cha)
    ///   - "N18" khớp ref "N18.5" CHỈ KHI code đủ dài >= 3 (tránh "I1" khớp "I10")
    /// Mọi so sánh đều không phân biệt hoa/thường sau khi chuẩn hóa (trim, upper).
    /// </summary>
    public sealed class ICDHelper
    {
        private readonly HashSet<string> _main;
        private readonly HashSet<string> _kt;
        private readonly HashSet<string> _all;

        public ICDHelper(string maBenhChinh, string maBenhKt)
        {
            _main = Parse(maBenhChinh);
            _kt = Parse(maBenhKt);
            _all = new HashSet<string>(_main);
            _all.UnionWith(_kt);
        }

        /// <summary>Tạo từ record XML1 (hoặc bất kỳ model có 2 field MA_BENH_CHINH, MA_BENH_KT).</summary>
        public static ICDHelper FromRecord(BhytBase record)
        {
            return new ICDHelper(record.GetValue("MA_BENH_CHINH") ?? "", record.GetValue("MA_BENH_KT") ?? "");
        }

        /// <summary>Tập mã chính (MA_BENH_CHINH).</summary>
        public IReadOnlyCollection<string> Main { get { return _main; } }

        /// <summary>Tập mã kèm theo (MA_BENH_KT).</summary>
        public IReadOnlyCollection<string> Kt { get { return _kt; } }

        /// <summary>Tất cả mã (chính + kèm).</summary>
        public IReadOnlyCollection<string> Codes { get { return _all; } }

        /// <summary>True nếu bất kỳ mã nào (chính hoặc kèm) khớp ít nhất 1 ref.</summary>
        public bool HasAny(IEnumerable<string> refSet)
        {
            var refs = refSet as ICollection<string> ?? refSet.ToList();
            return _all.Any(code => MatchOne(code, refs));
        }

        /// <summary>True nếu MỖI ref đều có ít nhất 1 mã bệnh khớp (BN phải có đồng thời nhiều bệnh).</summary>
        public bool HasAll(IEnumerable<string> refSet)
        {
            return refSet.All(r => _all.Any(code => MatchOne(code, new[] { r })));
        }

        /// <summary>True nếu MA_BENH_CHINH khớp ít nhất 1 ref.</summary>
        public bool MainMatches(IEnumerable<string> refSet)
        {
            var refs = refSet as ICollection<string> ?? refSet.ToList();
            return _main.Any(code => MatchOne(code, refs));
        }

        /// <summary>True nếu MA_BENH_KT có ít nhất 1 mã khớp ref.</summary>
        public bool KtMatches(IEnumerable<string> refSet)
        {
            var refs = refSet as ICollection<string> ?? refSet.ToList();
            return _kt.Any(code => MatchOne(code, refs));
        }

        /// <summary>True nếu KHÔNG có mã nào khớp.</summary>
        public bool NoneOf(IEnumerable<string> refSet)
        {
            return !HasAny(refSet);
        }

        private static HashSet<string> Parse(string value)
        {
            var set = new HashSet<string>();
            if (string.IsNullOrEmpty(value)) return set;
            foreach (string c in value.Split(';'))
            {
                string t = c.Trim();
                if (t.Length > 0) set.Add(t.ToUpperInvariant());
            }
            return set;
        }

        /// <summary>
        /// So khớp 1 mã với tập ref:
        ///   1. Bằng nhau chính xác
        ///   2. code cụ thể hơn ref (prefix): "N18.5" khớp "N18"
        ///   3. ref cụ thể hơn code: chỉ cho phép khi len(code) >= 3
        /// </summary>
        private static bool MatchOne(string code, IEnumerable<string> refSet)
        {
            string codeU = code.ToUpperInvariant();
            foreach (string r in refSet)
            {
                string refU = r.ToUpperInvariant();
                if (codeU == refU) return true;
                if (codeU.StartsWith(refU, StringComparison.Ordinal)) return true;
                if (codeU.Length >= 3 && refU.StartsWith(codeU, StringComparison.Ordinal)) return true;
            }
            return false;
        }

        /// <summary>VD: "ICDHelper(main=E11.9 | kt=I10, K58.9)" (dùng trong mô tả lỗi).</summary>
        public override string ToString()
        {
            string mainStr = string.Join(", ", _main.OrderBy(x => x, StringComparer.Ordinal));
            string ktStr = string.Join(", ", _kt.OrderBy(x => x, StringComparer.Ordinal));
            if (mainStr.Length == 0) mainStr = "(trống)";
            if (ktStr.Length == 0) ktStr = "(trống)";
            return "ICDHelper(main=" + mainStr + " | kt=" + ktStr + ")";
        }
    }
}
