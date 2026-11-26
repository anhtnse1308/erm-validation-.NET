using System.Collections.Generic;
using EmrValidation.Core.GiamDinh;
using EmrValidation.Core.Models;

namespace EmrValidation.Rules
{
    /// <summary>
    /// Rule I.1 – Chống chỉ định kỹ thuật (mã từ chối S011.1)
    /// Căn cứ: QĐ 25/QĐ-BYT ngày 03/01/2014
    ///
    /// a) Chụp cộng hưởng từ / can thiệp dưới CHT: chống chỉ định BN đã đặt máy tạo nhịp tim
    /// b) Chụp CT động mạch vành: chống chỉ định suy thận, rung nhĩ (trừ đã hết), hen phế quản
    /// </summary>
    public class ChongChiDinh : GiamDinhBase
    {
        // Mã DVKT
        private static readonly string[] MaChtPrefixes =
        {
            "18.50", "18.51", "18.52", "18.53", "18.54", "18.55",
            "18.56", "18.57", "18.58", "18.59", "18.60", "18.61",
            "18.62", "18.63", "18.64", "18.65", "18.66", "18.67",
            "18.68", "18.69", "18.70",
        };

        private static readonly HashSet<string> MaCtDongMachVanh =
            new HashSet<string> { "18.32", "18.32.01", "18.32.02" };

        // ICD-10
        private static readonly string[] IcdMayTaoNhip = { "Z95.0", "Z95", "T82.1" };
        private static readonly string[] IcdSuyThan = { "N17", "N18", "N19" };
        private static readonly string[] IcdRungNhi = { "I48" };
        private static readonly string[] IcdHenPheQuan = { "J45", "J46" };

        private const string CanCu = "QĐ 25/QĐ-BYT ngày 03/01/2014 (QTKT Chẩn đoán hình ảnh & Điện quang can thiệp)";

        private static bool IsCht(string maDv)
        {
            string t = maDv.Trim();
            foreach (string p in MaChtPrefixes)
                if (t.StartsWith(p, System.StringComparison.Ordinal)) return true;
            return false;
        }

        private static bool IsCtDmv(string maDv)
        {
            return MaCtDongMachVanh.Contains(maDv.Trim());
        }

        public override List<GiamDinhError> Check(IDictionary<string, List<BhytBase>> allObjects)
        {
            var errors = new List<GiamDinhError>();

            // MA_LK → ICDHelper
            var icdMap = new Dictionary<string, ICDHelper>();
            foreach (SheetRow row in GetRows(allObjects, "XML1"))
            {
                string maLk = row.Record.GetValue("MA_LK") ?? "";
                if (maLk.Length > 0)
                    icdMap[maLk] = ICDHelper.FromRecord(row.Record);
            }

            foreach (SheetRow row in GetRows(allObjects, "XML3"))
            {
                string maLk = row.Record.GetValue("MA_LK") ?? "";
                string maDv = (row.Record.GetValue("MA_DICH_VU") ?? "").Trim();
                if (maLk.Length == 0 || maDv.Length == 0) continue;

                ICDHelper icd;
                if (!icdMap.TryGetValue(maLk, out icd)) continue;

                // a) CHT → máy tạo nhịp
                if (IsCht(maDv) && icd.HasAny(IcdMayTaoNhip))
                {
                    errors.Add(new GiamDinhError
                    {
                        Sheet = "XML3", MaLk = maLk, RowExcel = row.RowExcel,
                        MaLyDo = "S011.1",
                        MoTa = $"Chụp CHT '{maDv}' chống chỉ định tuyệt đối: " +
                               $"BN đã đặt máy tạo nhịp tim. {icd}",
                        CanCu = CanCu, MaDichVu = maDv,
                    });
                }

                // b) CT ĐMV → suy thận / rung nhĩ / hen
                if (IsCtDmv(maDv))
                {
                    var reasons = new List<string>();
                    if (icd.HasAny(IcdSuyThan)) reasons.Add("suy thận");
                    if (icd.HasAny(IcdRungNhi)) reasons.Add("rung nhĩ");
                    if (icd.HasAny(IcdHenPheQuan)) reasons.Add("hen phế quản");
                    if (reasons.Count > 0)
                    {
                        errors.Add(new GiamDinhError
                        {
                            Sheet = "XML3", MaLk = maLk, RowExcel = row.RowExcel,
                            MaLyDo = "S011.1",
                            MoTa = $"Chụp CT ĐMV '{maDv}' chống chỉ định: " +
                                   $"{string.Join(", ", reasons)}. {icd}. " +
                                   "Lưu ý ngoại lệ: suy thận chạy thận ngay sau / " +
                                   "rung nhĩ đã điều trị hết → cần kiểm tra HSBA.",
                            CanCu = CanCu, MaDichVu = maDv,
                        });
                    }
                }
            }

            return errors;
        }
    }
}
