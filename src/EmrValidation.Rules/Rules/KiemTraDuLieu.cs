using System;
using System.Collections.Generic;
using System.Linq;
using EmrValidation.Core.GiamDinh;
using EmrValidation.Core.Helpers;
using EmrValidation.Core.Models;

namespace EmrValidation.Rules
{
    /// <summary>
    /// Rule – Kiểm tra chất lượng dữ liệu (CV 199/GĐĐT-PTDB ngày 13/05/2022)
    /// Nguồn: Trung tâm Giám định BHYT và Thanh toán đa tuyến.
    ///
    ///   R1 – Ngoại trú chỉ phát sinh tiền khám, không có DVKT/thuốc  → WARN.DL01
    ///   R2 – Ngoại trú SO_NGAY_DTRI = 0                              → WARN.DL02
    ///   R3 – Nội trú MA_LOAI_KCB=3 nhưng thời gian nằm &lt; 4 giờ     → WARN.DL03
    ///   R4 – Nội trú &gt; 4 giờ nhưng SO_NGAY_DTRI = 0                → WARN.DL04
    ///   R5 – COVID-19 (U07.1) không có bệnh kèm hợp lệ               → WARN.DL05
    ///
    /// Mã lý do WARN.DLxx là mã nội bộ, không phải mã từ chối BHXH chính thức.
    /// </summary>
    public class KiemTraDuLieu : GiamDinhBase
    {
        // MA_LOAI_KCB
        private static readonly HashSet<string> LoaiNgoaiTru = new HashSet<string> { "02", "05", "08" };
        private static readonly HashSet<string> LoaiNoiTru = new HashSet<string> { "03", "04", "06" };

        // Ngưỡng thời gian nằm viện nội trú (phút)
        private const int NoiTruMinPhut = 240;   // 4 giờ

        // ICD COVID-19
        private static readonly HashSet<string> IcdCovid = new HashSet<string> { "U07.1", "U07.2" };

        // Mã DVKT khám bệnh (chỉ tính phí khám, không phải DVKT thực sự)
        private static readonly string[] MaKhamBenhPrefixes = { "00." };   // nhóm khám bệnh
        private static readonly HashSet<string> MaKhamBenh = new HashSet<string>
        {
            "00.001", "00.00100",   // Khám bệnh thông thường
            "00.002", "00.00200",   // Khám bệnh theo yêu cầu
            "00.003", "00.00300",   // Khám chuyên khoa
        };

        private const string CanCu = "CV 199/GĐĐT-PTDB ngày 13/05/2022 + CV 3100/BYT-BH ngày 20/4/2021";

        /// <summary>Parse chuỗi DATE12 (yyyymmddHHMM) → DateTime, null nếu lỗi.</summary>
        private static DateTime? ParseDate12(string val)
        {
            string s = (val ?? "").Trim();
            if (s.Length == 12 && ParseHelper.IsDigits(s))
                return ParseHelper.ParseDate12(s);
            return null;
        }

        /// <summary>Parse SO_NGAY_DTRI → số nguyên, null nếu không parse được.</summary>
        private static long? SoNgay(string val)
        {
            return val == null ? null : ParseHelper.ToInt(val);
        }

        /// <summary>True nếu mã DVKT chỉ là mã khám bệnh (không phải DVKT thực sự).</summary>
        private static bool IsKhamBenhOnly(string maDv)
        {
            maDv = maDv.Trim();
            if (MaKhamBenh.Contains(maDv)) return true;
            foreach (string p in MaKhamBenhPrefixes)
                if (maDv.StartsWith(p, StringComparison.Ordinal)) return true;
            return false;
        }

        /// <summary>Tính số phút nằm viện. Null nếu không parse được hoặc ra &lt; vào.</summary>
        private static double? ThoiGianNamPhut(string ngayVao, string ngayRa)
        {
            DateTime? vao = ParseDate12(ngayVao);
            DateTime? ra = ParseDate12(ngayRa);
            if (vao != null && ra != null && ra.Value >= vao.Value)
                return (ra.Value - vao.Value).TotalSeconds / 60;
            return null;
        }

        private static string Trimmed(BhytBase rec, string field)
        {
            return (rec.GetValue(field) ?? "").Trim();
        }

        public override List<GiamDinhError> Check(IDictionary<string, List<BhytBase>> allObjects)
        {
            var errors = new List<GiamDinhError>();
            errors.AddRange(R1NgoaiTruChiKham(allObjects));
            errors.AddRange(R2NgoaiTru0Ngay(allObjects));
            errors.AddRange(R3NoiTruDuoi4Gio(allObjects));
            errors.AddRange(R4NoiTru0Ngay(allObjects));
            errors.AddRange(R5CovidKhongBenhKem(allObjects));
            return errors;
        }

        // R1 – Ngoại trú chỉ phát sinh tiền khám, không có DVKT/thuốc
        private List<GiamDinhError> R1NgoaiTruChiKham(IDictionary<string, List<BhytBase>> allObjects)
        {
            var errors = new List<GiamDinhError>();

            // MA_LK có thuốc (XML2)
            var lkCoThuoc = new HashSet<string>();
            foreach (SheetRow r in GetRows(allObjects, "XML2"))
            {
                string k = Trimmed(r.Record, "MA_LK");
                if (k.Length > 0) lkCoThuoc.Add(k);
            }

            // MA_LK có DVKT thực sự (XML3, loại trừ mã khám bệnh)
            var lkCoDvkt = new HashSet<string>();
            foreach (SheetRow r in GetRows(allObjects, "XML3"))
            {
                string maLk = Trimmed(r.Record, "MA_LK");
                string maDv = Trimmed(r.Record, "MA_DICH_VU");
                if (maLk.Length > 0 && maDv.Length > 0 && !IsKhamBenhOnly(maDv))
                    lkCoDvkt.Add(maLk);
            }

            // Kiểm tra XML1
            foreach (SheetRow row in GetRows(allObjects, "XML1"))
            {
                string maLk = Trimmed(row.Record, "MA_LK");
                string loai = Trimmed(row.Record, "MA_LOAI_KCB");
                if (maLk.Length == 0 || !LoaiNgoaiTru.Contains(loai)) continue;
                if (!lkCoThuoc.Contains(maLk) && !lkCoDvkt.Contains(maLk))
                {
                    errors.Add(new GiamDinhError
                    {
                        Sheet = "XML1", MaLk = maLk, RowExcel = row.RowExcel,
                        MaLyDo = "WARN.DL01",
                        MoTa = $"Ngoại trú (MA_LOAI_KCB={loai}) chỉ phát sinh tiền khám bệnh, " +
                               "không có DVKT hoặc thuốc nào trong XML2/XML3. " +
                               "Kiểm tra dữ liệu có gửi đủ sheet không.",
                        CanCu = CanCu,
                    });
                }
            }
            return errors;
        }

        // R2 – Ngoại trú SO_NGAY_DTRI = 0
        private List<GiamDinhError> R2NgoaiTru0Ngay(IDictionary<string, List<BhytBase>> allObjects)
        {
            var errors = new List<GiamDinhError>();
            foreach (SheetRow row in GetRows(allObjects, "XML1"))
            {
                string maLk = Trimmed(row.Record, "MA_LK");
                string loai = Trimmed(row.Record, "MA_LOAI_KCB");
                long? soNgay = SoNgay(row.Record.GetValue("SO_NGAY_DTRI"));
                if (maLk.Length == 0 || !LoaiNgoaiTru.Contains(loai)) continue;
                if (soNgay != null && soNgay.Value == 0)
                {
                    errors.Add(new GiamDinhError
                    {
                        Sheet = "XML1", MaLk = maLk, RowExcel = row.RowExcel,
                        MaLyDo = "WARN.DL02",
                        MoTa = $"Ngoại trú (MA_LOAI_KCB={loai}) có SO_NGAY_DTRI=0. " +
                               "Ngoại trú tối thiểu phải = 1 ngày. " +
                               "Kiểm tra lại dữ liệu thống kê.",
                        CanCu = CanCu,
                    });
                }
            }
            return errors;
        }

        // R3 – Nội trú MA_LOAI_KCB=3 nhưng thời gian nằm < 4 giờ
        private List<GiamDinhError> R3NoiTruDuoi4Gio(IDictionary<string, List<BhytBase>> allObjects)
        {
            var errors = new List<GiamDinhError>();
            foreach (SheetRow row in GetRows(allObjects, "XML1"))
            {
                string maLk = Trimmed(row.Record, "MA_LK");
                string loai = Trimmed(row.Record, "MA_LOAI_KCB");
                if (maLk.Length == 0 || loai != "03") continue;

                string ngayVao = row.Record.GetValue("NGAY_VAO") ?? "";
                string ngayRa = row.Record.GetValue("NGAY_RA") ?? "";
                double? phut = ThoiGianNamPhut(ngayVao, ngayRa);
                if (phut != null && phut.Value < NoiTruMinPhut)
                {
                    int gio = (int)Math.Floor(phut.Value / 60);
                    int phutLe = (int)(phut.Value % 60);
                    errors.Add(new GiamDinhError
                    {
                        Sheet = "XML1", MaLk = maLk, RowExcel = row.RowExcel,
                        MaLyDo = "WARN.DL03",
                        MoTa = "Nội trú (MA_LOAI_KCB=03) thời gian nằm viện " +
                               $"{gio} giờ {phutLe} phút (< 4 giờ). " +
                               $"NGAY_VAO={ngayVao} → NGAY_RA={ngayRa}. " +
                               "Nếu đúng < 4 giờ nên dùng MA_LOAI_KCB=09 " +
                               "(nội trú dưới 4 giờ theo QĐ 4750).",
                        CanCu = CanCu,
                    });
                }
            }
            return errors;
        }

        // R4 – Nội trú > 4 giờ nhưng SO_NGAY_DTRI = 0
        private List<GiamDinhError> R4NoiTru0Ngay(IDictionary<string, List<BhytBase>> allObjects)
        {
            var errors = new List<GiamDinhError>();
            foreach (SheetRow row in GetRows(allObjects, "XML1"))
            {
                string maLk = Trimmed(row.Record, "MA_LK");
                string loai = Trimmed(row.Record, "MA_LOAI_KCB");
                long? soNgay = SoNgay(row.Record.GetValue("SO_NGAY_DTRI"));
                if (maLk.Length == 0 || !LoaiNoiTru.Contains(loai)) continue;
                if (soNgay == null || soNgay.Value != 0) continue;

                // Chỉ cảnh báo nếu thực sự nằm > 4 giờ
                string ngayVao = row.Record.GetValue("NGAY_VAO") ?? "";
                string ngayRa = row.Record.GetValue("NGAY_RA") ?? "";
                double? phut = ThoiGianNamPhut(ngayVao, ngayRa);
                if (phut == null || phut.Value > NoiTruMinPhut)
                {
                    string phutStr = (phut != null && phut.Value != 0)
                        ? PyFormat.Round(phut.Value, 0).ToString("0", System.Globalization.CultureInfo.InvariantCulture) + " phút"
                        : "không xác định";
                    errors.Add(new GiamDinhError
                    {
                        Sheet = "XML1", MaLk = maLk, RowExcel = row.RowExcel,
                        MaLyDo = "WARN.DL04",
                        MoTa = $"Nội trú (MA_LOAI_KCB={loai}) thời gian nằm {phutStr} " +
                               "nhưng SO_NGAY_DTRI=0. " +
                               $"NGAY_VAO={ngayVao} → NGAY_RA={ngayRa}. " +
                               "Mâu thuẫn dữ liệu, cần điều chỉnh SO_NGAY_DTRI.",
                        CanCu = CanCu,
                    });
                }
            }
            return errors;
        }

        // R5 – COVID-19 không có bệnh kèm hợp lệ
        private List<GiamDinhError> R5CovidKhongBenhKem(IDictionary<string, List<BhytBase>> allObjects)
        {
            var errors = new List<GiamDinhError>();
            foreach (SheetRow row in GetRows(allObjects, "XML1"))
            {
                string maLk = Trimmed(row.Record, "MA_LK");
                string maChinh = Trimmed(row.Record, "MA_BENH_CHINH").ToUpperInvariant();
                string maKt = Trimmed(row.Record, "MA_BENH_KT");
                if (maLk.Length == 0 || maChinh != "U07.1") continue;

                // Parse MA_BENH_KT → tập mã kèm theo
                var benhKem = new HashSet<string>();
                foreach (string c in maKt.Split(';'))
                {
                    string t = c.Trim();
                    if (t.Length > 0) benhKem.Add(t.ToUpperInvariant());
                }

                // Hợp lệ: có ít nhất 1 mã kèm theo KHÁC U07.1 và U07.2
                bool hopLe = benhKem.Any(c => !IcdCovid.Contains(c));
                if (!hopLe)
                {
                    string moTaKem;
                    if (benhKem.Count == 0)
                        moTaKem = "không có mã bệnh kèm theo";
                    else
                        moTaKem = "mã bệnh kèm theo chỉ có: " +
                                  $"{string.Join("; ", benhKem.OrderBy(x => x, StringComparer.Ordinal))} (đều là COVID)";

                    errors.Add(new GiamDinhError
                    {
                        Sheet = "XML1", MaLk = maLk, RowExcel = row.RowExcel,
                        MaLyDo = "WARN.DL05",
                        MoTa = $"Chẩn đoán chính COVID-19 (U07.1), {moTaKem}. " +
                               "Theo CV 3100/BYT-BH ngày 20/4/2021: BN COVID-19 phải có " +
                               "ít nhất 1 bệnh kèm theo khác U07.1/U07.2. " +
                               "BHXH sẽ thực hiện giảm trừ trực tiếp trên hệ thống.",
                        CanCu = "CV 3100/BYT-BH ngày 20/4/2021 + CV 199/GĐĐT-PTDB ngày 13/05/2022",
                    });
                }
            }
            return errors;
        }
    }
}
