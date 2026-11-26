using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using ClosedXML.Excel;

namespace EmrValidation.Excel
{
    /// <summary>Kiểu chữ dùng trong các báo cáo Excel (Arial 10).</summary>
    internal enum XlFont
    {
        Data,       // chữ thường
        Bold,       // chữ đậm
        Red,        // chữ đỏ C00000
        RedBold,    // chữ đỏ đậm
        WhiteBold,  // chữ trắng đậm (dòng tổng, tiêu đề cột)
        WhiteBold11,// chữ trắng đậm cỡ 11 (dòng TỔNG của báo cáo danh mục thuốc)
        Warn,       // chữ nâu cam 833C00 (mã cảnh báo ở báo cáo danh mục thuốc)
    }

    /// <summary>Căn lề ô.</summary>
    internal enum XlAlign
    {
        Left,       // dọc giữa, xuống dòng tự động
        Center,     // ngang giữa, dọc giữa, xuống dòng tự động
        Top,        // dọc trên, xuống dòng tự động
    }

    /// <summary>
    /// Một sheet đang dựng: ghi ô có style và theo dõi độ rộng cột
    /// (tương đương hàm _auto_width / auto_w của bản Python: tối đa 80 ký tự dòng đầu, cộng 3).
    /// </summary>
    internal sealed class XlSheet
    {
        public IXLWorksheet Ws { get; private set; }
        private readonly int[] _maxW;

        public XlSheet(IXLWorksheet ws, IList<string> headers)
        {
            Ws = ws;
            _maxW = new int[headers.Count];
            for (int i = 0; i < _maxW.Length; i++) _maxW[i] = headers[i].Length;
        }

        public const string FillHdr = "#1F3864";
        public const string FillEven = "#FFFFFF";
        public const string FillOk = "#E2EFDA";
        public const string FillSum = "#C00000";

        /// <summary>Ô tiêu đề cột: nền màu, chữ trắng đậm, căn giữa, có viền.</summary>
        public void Header(int row, int col, string text, string fillHex = FillHdr)
        {
            IXLCell c = Ws.Cell(row, col);
            XlKit.SetText(c, text);
            Style(c, fillHex, XlFont.WhiteBold, XlAlign.Center);
        }

        /// <summary>Ô dữ liệu: giá trị (chuỗi, số nguyên hoặc null), nền, chữ, căn lề, viền.</summary>
        public void Cell(int row, int col, object value, string fillHex, XlFont font = XlFont.Data, XlAlign align = XlAlign.Left)
        {
            IXLCell c = Ws.Cell(row, col);
            string textForWidth = "";
            if (value is string)
            {
                XlKit.SetText(c, (string)value);
                textForWidth = (string)value;
            }
            else if (value is int || value is long)
            {
                long n = Convert.ToInt64(value);
                c.SetValue(n);
                // Python: "value or ''" → số 0 coi như rỗng khi tính độ rộng
                textForWidth = n == 0 ? "" : n.ToString(CultureInfo.InvariantCulture);
            }
            Track(col, textForWidth);
            Style(c, fillHex, font, align);
        }

        /// <summary>Ghi nhận độ rộng cho ô đã ghi bằng cách khác (VD: dòng thông báo).</summary>
        public void Track(int col, string text)
        {
            if (col < 1 || col > _maxW.Length) return;
            int nl = text.IndexOf('\n');
            int len = Math.Min(nl >= 0 ? nl : text.Length, 80);
            if (len > _maxW[col - 1]) _maxW[col - 1] = len;
        }

        /// <summary>Tiêu đề lớn ở dòng 1 (gộp ô từ cột 1 đến lastCol).</summary>
        public void Title(string text, int lastCol, double height)
        {
            IXLCell c = Ws.Cell(1, 1);
            XlKit.SetText(c, text);
            c.Style.Fill.BackgroundColor = XLColor.FromHtml(FillHdr);
            c.Style.Font.Bold = true;
            c.Style.Font.FontColor = XLColor.FromHtml("#FFFFFF");
            c.Style.Font.FontName = "Arial";
            c.Style.Font.FontSize = 12;
            c.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            c.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            c.Style.Alignment.WrapText = true;
            Ws.Range(1, 1, 1, lastCol).Merge();
            Ws.Row(1).Height = height;
        }

        /// <summary>Dòng thông báo "không có lỗi" (gộp ô từ cột 1 đến lastCol).</summary>
        public void Message(int row, int lastCol, string text)
        {
            IXLCell c = Ws.Cell(row, 1);
            XlKit.SetText(c, text);
            Track(1, text);
            c.Style.Fill.BackgroundColor = XLColor.FromHtml(FillOk);
            c.Style.Font.FontName = "Arial";
            c.Style.Font.FontSize = 11;
            c.Style.Font.Bold = true;
            c.Style.Font.FontColor = XLColor.FromHtml("#375623");
            c.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            c.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            c.Style.Alignment.WrapText = true;
            Ws.Range(row, 1, row, lastCol).Merge();
        }

        /// <summary>Ô chỉ có nền xanh nhạt và chữ (VD: "✓ Không có lỗi" ở ô A2).</summary>
        public void OkCell(int row, int col, string text)
        {
            IXLCell c = Ws.Cell(row, col);
            XlKit.SetText(c, text);
            Track(col, text);
            c.Style.Fill.BackgroundColor = XLColor.FromHtml(FillOk);
        }

        /// <summary>Dòng ghi chú gộp ô (chỉ có chữ Arial 10, không nền, không viền).</summary>
        public void Note(int row, int lastCol, string text)
        {
            IXLCell c = Ws.Cell(row, 1);
            XlKit.SetText(c, text);
            c.Style.Font.FontName = "Arial";
            c.Style.Font.FontSize = 10;
            Ws.Range(row, 1, row, lastCol).Merge();
        }

        public void RowHeight(int row, double height)
        {
            Ws.Row(row).Height = height;
        }

        /// <summary>Áp độ rộng cột đã tính, và cố định số dòng đầu.</summary>
        public void Finish(int freezeRows)
        {
            for (int i = 0; i < _maxW.Length; i++)
                Ws.Column(i + 1).Width = _maxW[i] + 3;
            Ws.SheetView.FreezeRows(freezeRows);
        }

        private static void Style(IXLCell c, string fillHex, XlFont font, XlAlign align)
        {
            c.Style.Fill.BackgroundColor = XLColor.FromHtml(fillHex);

            c.Style.Font.FontName = "Arial";
            c.Style.Font.FontSize = font == XlFont.WhiteBold11 ? 11 : 10;
            c.Style.Font.Bold = font == XlFont.Bold || font == XlFont.RedBold
                             || font == XlFont.WhiteBold || font == XlFont.WhiteBold11;
            if (font == XlFont.Red || font == XlFont.RedBold)
                c.Style.Font.FontColor = XLColor.FromHtml("#C00000");
            else if (font == XlFont.WhiteBold || font == XlFont.WhiteBold11)
                c.Style.Font.FontColor = XLColor.FromHtml("#FFFFFF");
            else if (font == XlFont.Warn)
                c.Style.Font.FontColor = XLColor.FromHtml("#833C00");

            switch (align)
            {
                case XlAlign.Center:
                    c.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    c.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    break;
                case XlAlign.Top:
                    c.Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                    break;
                default:
                    c.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    break;
            }
            c.Style.Alignment.WrapText = true;

            c.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            c.Style.Border.OutsideBorderColor = XLColor.FromHtml("#AAAAAA");
        }
    }

    /// <summary>Tiện ích chung cho các bộ ghi Excel.</summary>
    internal static class XlKit
    {
        /// <summary>Lưu giá trị dạng VĂN BẢN (không để ClosedXML tự đổi "0012" thành số 12).</summary>
        public static void SetText(IXLCell cell, string text)
        {
            cell.SetValue(text);
            if (cell.DataType != XLDataType.Text)
                cell.DataType = XLDataType.Text;
        }

        public static string Now()
        {
            return DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
        }

        /// <summary>Chiều cao dòng theo độ dài mô tả: max(min, len // 60 * step + step).</summary>
        public static double RowHeightFor(string text, double min, double step)
        {
            return Math.Max(min, (text ?? "").Length / 60 * step + step);
        }

        /// <summary>Đường dẫn file xuất: &lt;thư mục file gốc&gt;\&lt;tên gốc&gt;[&lt;timestamp&gt;]_&lt;suffix&gt;.xlsx</summary>
        public static string OutPath(string excelFile, string suffix)
        {
            string folder = Path.GetDirectoryName(Path.GetFullPath(excelFile));
            string baseName = Path.GetFileNameWithoutExtension(excelFile);
            string ts = DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);
            return Path.Combine(folder, string.Format("{0}[{1}]_{2}.xlsx", baseName, ts, suffix));
        }
    }
}
