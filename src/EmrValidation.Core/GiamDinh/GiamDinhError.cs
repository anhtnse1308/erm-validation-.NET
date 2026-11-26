namespace EmrValidation.Core.GiamDinh
{
    /// <summary>Một lỗi giám định thanh toán BHYT.</summary>
    public class GiamDinhError
    {
        /// <summary>Sheet XML chứa dịch vụ bị từ chối.</summary>
        public string Sheet { get; set; }
        /// <summary>Mã lần khám.</summary>
        public string MaLk { get; set; }
        /// <summary>Dòng Excel (tính từ 2).</summary>
        public int RowExcel { get; set; }
        /// <summary>Mã lý do từ chối: S011.1, S054.1, WARN.DM01...</summary>
        public string MaLyDo { get; set; }
        /// <summary>Mô tả chi tiết lý do.</summary>
        public string MoTa { get; set; }
        /// <summary>Căn cứ pháp lý (tên QĐ/TT).</summary>
        public string CanCu { get; set; }
        /// <summary>Mã DVKT hoặc mã thuốc bị từ chối (nếu có).</summary>
        public string MaDichVu { get; set; }

        public GiamDinhError()
        {
            Sheet = ""; MaLk = ""; MaLyDo = ""; MoTa = ""; CanCu = ""; MaDichVu = "";
        }
    }
}
