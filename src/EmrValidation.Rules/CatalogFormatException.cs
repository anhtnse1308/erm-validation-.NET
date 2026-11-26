using System;

namespace EmrValidation.Rules
{
    /// <summary>File danh mục sai định dạng (thiếu cột bắt buộc...). Tương đương ValueError trong bản Python.</summary>
    public class CatalogFormatException : Exception
    {
        public CatalogFormatException(string message) : base(message) { }
    }
}
