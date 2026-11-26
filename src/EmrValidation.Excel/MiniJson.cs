using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace EmrValidation.Excel
{
    /// <summary>
    /// Bộ ghi JSON tối giản, cho ra định dạng giống json.dump(ensure_ascii=False, indent=2) của Python.
    /// Hỗ trợ: null, string, bool, số nguyên, IList, danh sách cặp khóa-giá trị có thứ tự.
    /// </summary>
    public static class MiniJson
    {
        /// <summary>Đối tượng JSON giữ thứ tự khóa.</summary>
        public class JsonObject : List<KeyValuePair<string, object>>
        {
            public JsonObject Add(string key, object value)
            {
                Add(new KeyValuePair<string, object>(key, value));
                return this;
            }
        }

        public static string Serialize(object value)
        {
            var sb = new StringBuilder();
            Write(sb, value, 0);
            return sb.ToString();
        }

        private static void Write(StringBuilder sb, object value, int level)
        {
            if (value == null) { sb.Append("null"); return; }

            string s = value as string;
            if (s != null) { WriteString(sb, s); return; }

            if (value is bool) { sb.Append((bool)value ? "true" : "false"); return; }
            if (value is int || value is long)
            {
                sb.Append(Convert.ToString(value, CultureInfo.InvariantCulture));
                return;
            }

            var obj = value as JsonObject;
            if (obj != null)
            {
                if (obj.Count == 0) { sb.Append("{}"); return; }
                sb.Append("{\n");
                for (int i = 0; i < obj.Count; i++)
                {
                    sb.Append(' ', (level + 1) * 2);
                    WriteString(sb, obj[i].Key);
                    sb.Append(": ");
                    Write(sb, obj[i].Value, level + 1);
                    if (i < obj.Count - 1) sb.Append(',');
                    sb.Append('\n');
                }
                sb.Append(' ', level * 2).Append('}');
                return;
            }

            var list = value as System.Collections.IList;
            if (list != null)
            {
                if (list.Count == 0) { sb.Append("[]"); return; }
                sb.Append("[\n");
                for (int i = 0; i < list.Count; i++)
                {
                    sb.Append(' ', (level + 1) * 2);
                    Write(sb, list[i], level + 1);
                    if (i < list.Count - 1) sb.Append(',');
                    sb.Append('\n');
                }
                sb.Append(' ', level * 2).Append(']');
                return;
            }

            throw new NotSupportedException("Kiểu không hỗ trợ: " + value.GetType().FullName);
        }

        private static void WriteString(StringBuilder sb, string s)
        {
            sb.Append('"');
            foreach (char c in s)
            {
                switch (c)
                {
                    case '"': sb.Append("\\\""); break;
                    case '\\': sb.Append("\\\\"); break;
                    case '\n': sb.Append("\\n"); break;
                    case '\r': sb.Append("\\r"); break;
                    case '\t': sb.Append("\\t"); break;
                    case '\b': sb.Append("\\b"); break;
                    case '\f': sb.Append("\\f"); break;
                    default:
                        if (c < 0x20) sb.Append("\\u").Append(((int)c).ToString("x4", CultureInfo.InvariantCulture));
                        else sb.Append(c);
                        break;
                }
            }
            sb.Append('"');
        }
    }
}
