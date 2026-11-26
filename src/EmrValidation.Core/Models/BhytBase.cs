using System;
using System.Collections.Generic;
using EmrValidation.Core.Helpers;

namespace EmrValidation.Core.Models
{
    /// <summary>
    /// Base class cho tất cả bảng XML BHYT (tương đương model/BHYTBase.py).
    /// Mỗi field là một property string; giá trị trống được lưu là null.
    /// Class con khai báo metadata (độ dài tối đa, kiểu số, ngày giờ...) và override Validate().
    /// </summary>
    public abstract class BhytBase
    {
        // ------------------------------------------------------------------
        // Metadata do class con khai báo (tương đương _MAX_LEN, _NUMERIC, _DATE12, _DATE8, _REQUIRED)
        // ------------------------------------------------------------------
        private static readonly string[] EmptyStrings = new string[0];
        private static readonly KeyValuePair<string, int>[] EmptyMaxLen = new KeyValuePair<string, int>[0];

        /// <summary>Danh sách tên field theo đúng thứ tự khai báo.</summary>
        protected abstract string[] FieldNamesCore { get; }

        protected virtual KeyValuePair<string, int>[] MaxLenCore { get { return EmptyMaxLen; } }
        protected virtual string[] NumericCore { get { return EmptyStrings; } }
        protected virtual string[] Date12Core { get { return EmptyStrings; } }
        protected virtual string[] Date8Core { get { return EmptyStrings; } }
        protected virtual string[] RequiredCore { get { return EmptyStrings; } }

        /// <summary>Tên các field (theo thứ tự khai báo).</summary>
        public IReadOnlyList<string> FieldNames { get { return FieldNamesCore; } }

        /// <summary>Lấy giá trị field theo tên. Trả null nếu field không tồn tại hoặc giá trị trống.</summary>
        public abstract string GetValue(string fieldName);

        /// <summary>Gán giá trị field theo tên. Trả false nếu class không có field đó.</summary>
        public abstract bool SetValue(string fieldName, string value);

        // ------------------------------------------------------------------
        // Khởi tạo từ dòng Excel (tương đương from_dict + __post_init__)
        // ------------------------------------------------------------------

        /// <summary>
        /// Tạo instance từ một dòng dữ liệu (tên cột → giá trị).
        /// - Chỉ lấy các cột trùng tên field, bỏ qua cột thừa
        /// - Giá trị rỗng/nan/None → null
        /// - Sau đó clean toàn bộ field (bỏ khoảng trắng, bỏ dấu ' đầu chuỗi)
        /// </summary>
        public static T FromDict<T>(IDictionary<string, string> row) where T : BhytBase, new()
        {
            var obj = new T();
            obj.LoadFrom(row);
            return obj;
        }

        /// <summary>Nạp dữ liệu từ dòng Excel vào instance hiện tại.</summary>
        public void LoadFrom(IDictionary<string, string> row)
        {
            foreach (var kv in row)
            {
                string val = kv.Value;
                if (val == null || IsNoneToken(val)) val = null;
                SetValue(kv.Key, val);
            }
            foreach (string name in FieldNamesCore)
                SetValue(name, CleanExcelStr(GetValue(name)));
        }

        private static bool IsNoneToken(string v)
        {
            string t = v.Trim();
            return t == "" || t == "nan" || t == "NaN" || t == "None";
        }

        /// <summary>
        /// Xử lý giá trị đọc từ Excel: bỏ khoảng trắng đầu/cuối, bỏ ký tự ' ở đầu
        /// (text-prefix do cell format Text), trả null nếu rỗng.
        /// </summary>
        public static string CleanExcelStr(string value)
        {
            if (value == null) return null;
            string v = value.Trim();
            if (v.StartsWith("'")) v = v.Substring(1).Trim();
            return v.Length > 0 ? v : null;
        }

        // ------------------------------------------------------------------
        // Helper parse dùng cho class con (tương đương _parse_date12, _to_float...)
        // ------------------------------------------------------------------
        protected static DateTime? ParseDate12(string value) { return ParseHelper.ParseDate12(value); }
        protected static DateTime? ParseDate8(string value) { return ParseHelper.ParseDate8(value); }
        protected static double? ToFloat(string value) { return ParseHelper.ToFloat(value); }
        protected static long? ToInt(string value) { return ParseHelper.ToInt(value); }
        protected static int DecimalPlaces(string value) { return ParseHelper.DecimalPlaces(value); }

        // ------------------------------------------------------------------
        // Validate: base chỉ kiểm tra độ dài, kiểu số, DATE12, DATE8.
        // Class con override để thêm logic nghiệp vụ.
        // ------------------------------------------------------------------
        public virtual List<string> Validate()
        {
            var errs = new List<string>();

            foreach (var kv in MaxLenCore)
            {
                string val = GetValue(kv.Key);
                if (!string.IsNullOrEmpty(val) && kv.Value > 0 && val.Length > kv.Value)
                    errs.Add(string.Format("{0}: độ dài {1} vượt quá {2} ký tự", kv.Key, val.Length, kv.Value));
            }

            foreach (string fname in NumericCore)
            {
                string val = GetValue(fname);
                if (val != null && !ParseHelper.IsFloat(val))
                    errs.Add(string.Format("{0}: '{1}' không phải số hợp lệ", fname, val));
            }

            foreach (string fname in Date12Core)
            {
                string val = GetValue(fname);
                if (!string.IsNullOrEmpty(val) && (val.Length != 12 || !ParseHelper.IsDigits(val)))
                    errs.Add(string.Format(
                        "{0}: '{1}' sai định dạng DATE12, phải là yyyymmddHHMM (12 chữ số)", fname, val));
            }

            foreach (string fname in Date8Core)
            {
                string val = GetValue(fname);
                if (!string.IsNullOrEmpty(val) && (val.Length != 8 || !ParseHelper.IsDigits(val)))
                    errs.Add(string.Format(
                        "{0}: '{1}' sai định dạng DATE8, phải là yyyymmdd (8 chữ số)", fname, val));
            }

            return errs;
        }

        /// <summary>Giá trị có nằm trong tập cho trước không (tương đương "x in (...)" của Python).</summary>
        protected static bool In(string value, params string[] set)
        {
            return Array.IndexOf(set, value) >= 0;
        }

        /// <summary>str(float) của Python (VD: 150000.0).</summary>
        protected static string F(double value) { return PyFormat.Str(value); }

        /// <summary>round(x, 2) của Python.</summary>
        protected static double R2(double value) { return PyFormat.Round(value, 2); }

        /// <summary>Field có nằm trong tập _REQUIRED của class không.</summary>
        protected bool IsRequired(string fieldName)
        {
            return Array.IndexOf(RequiredCore, fieldName) >= 0;
        }
    }
}
