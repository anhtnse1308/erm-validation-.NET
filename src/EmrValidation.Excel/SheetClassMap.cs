using System;
using System.Collections.Generic;
using EmrValidation.Core.Models;

namespace EmrValidation.Excel
{
    /// <summary>
    /// Ánh xạ tên sheet trong file Excel BHYT → class model tương ứng.
    /// XML6, XML12 không nằm trong file hiện tại của bệnh viện nên chưa có model.
    /// </summary>
    public static class SheetClassMap
    {
        public static readonly IReadOnlyDictionary<string, Func<BhytBase>> Factories =
            new Dictionary<string, Func<BhytBase>>
            {
                { "XML1",  () => new Bang1_TongHop() },
                { "XML2",  () => new Bang2_Thuoc() },
                { "XML3",  () => new Bang3_DVKT_VTYT() },
                { "XML4",  () => new Bang4_DichVuCLS() },
                { "XML5",  () => new Bang5_DienBienLamSang() },
                { "XML7",  () => new Bang7_GiayRaVien() },
                { "XML8",  () => new Bang8_TomTatHSBA() },
                { "XML9",  () => new Bang9_GiayChungSinh() },
                { "XML10", () => new Bang10_GiayNghiDuongThai() },
                { "XML11", () => new Bang11_GiayNghiHuongBHXH() },
                { "XML13", () => new Bang13_GiayChuyenTuyen() },
                { "XML14", () => new Bang14_GiayHenKhamLai() },
            };
    }
}
