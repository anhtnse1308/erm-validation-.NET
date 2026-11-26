using System;
using System.Collections.Generic;
using EmrValidation.Core.GiamDinh;
using EmrValidation.Core.Models;
using EmrValidation.Excel;

namespace EmrValidation.Rules
{
    /// <summary>Chạy toàn bộ rule giám định trên dữ liệu đã đọc (tương đương GiamDinhService trong Python).</summary>
    public static class GiamDinhService
    {
        // Đăng ký rule tại đây – thứ tự = thứ tự xuất hiện trong báo cáo
        private static readonly GiamDinhBase[] Rules =
        {
            new ChongChiDinh(),
            new DichVuTrongQuyTrinh(),
            new VTYTVuotMuc(),
            new ThuocChiDinh(),
            new LuotKhamBenhNhan(),
            new GioiHanTanSuat(),
            new DieuKienDacBiet(),
            new KiemTraDuLieu(),
        };

        /// <summary>
        /// Chạy tất cả rule, trả về danh sách lỗi giám định.
        /// Một rule bị lỗi không làm hỏng cả luồng: lỗi được báo qua <paramref name="onRuleError"/>
        /// (mặc định ghi vào System.Diagnostics.Trace) rồi chạy tiếp rule kế tiếp.
        /// </summary>
        public static List<GiamDinhError> Run(IDictionary<string, List<BhytBase>> allObjects, Action<string> onRuleError = null)
        {
            var allErrors = new List<GiamDinhError>();
            foreach (GiamDinhBase rule in Rules)
            {
                try
                {
                    allErrors.AddRange(rule.Check(allObjects));
                }
                catch (Exception e)
                {
                    string msg = "[GIAMDINH] Lỗi rule " + rule.GetType().Name + ": " + e.Message;
                    if (onRuleError != null) onRuleError(msg);
                    else System.Diagnostics.Trace.WriteLine(msg);
                }
            }
            return allErrors;
        }

        /// <summary>
        /// Xuất kết quả giám định ra Excel: &lt;ten_goc&gt;[&lt;timestamp&gt;]_giamdinh.xlsx
        /// (sheet TONG_HOP, CHI_TIET và mỗi mã lý do một sheet).
        /// </summary>
        public static string WriteExcelGiamDinh(string excelFile, IList<GiamDinhError> errors)
        {
            return GiamDinhExcelWriter.Write(excelFile, errors);
        }
    }
}
