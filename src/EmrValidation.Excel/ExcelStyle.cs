namespace EmrValidation.Excel
{
    /// <summary>Hằng số style cho các file Excel báo cáo (tương đương EXCEL_STYLE trong helper.py).</summary>
    public static class ExcelStyle
    {
        // --- Màu nền ---
        public const string FillHdrSummary = "#2F4F8F";  // header summary  – xanh đậm
        public const string FillHdrErrors = "#8B0000";   // header errors   – đỏ đậm
        public const string FillRowErr = "#FFCCCC";      // dòng lỗi        – đỏ nhạt
        public const string FillRowOk = "#CCFFCC";       // dòng OK         – xanh nhạt
        public const string FillSheetClean = "#E8F5E9";  // sheet không lỗi – xanh rất nhạt

        // --- Màu chữ ---
        public const string FontWhite = "#FFFFFF";
        public const string FontErr = "#CC0000";         // text lỗi – đỏ
        public const string FontClean = "#2E7D32";       // sheet sạch – xanh lá
        public const string BorderColor = "#AAAAAA";

        public const string FontName = "Arial";
        public const double FontSize = 10;

        // --- Kích thước ---
        public const int ColWidthMax = 60;   // độ rộng cột tối đa (ký tự)
        public const int ColWidthPad = 3;    // padding thêm vào sau auto-width
        public const double RowHdrHeight = 30;  // chiều cao dòng header (pt)
    }
}
