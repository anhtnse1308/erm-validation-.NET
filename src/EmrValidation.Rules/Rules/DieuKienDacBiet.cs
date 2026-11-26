using System;
using System.Collections.Generic;
using System.Linq;
using EmrValidation.Core.GiamDinh;
using EmrValidation.Core.Helpers;
using EmrValidation.Core.Models;

namespace EmrValidation.Rules
{
    /// <summary>
    /// Rule – Điều kiện đặc biệt (nhiều mã lý do), từ Phụ lục 02 PDF 477.
    ///   KT23  / D011.1   – DVKT chuyên khoa Nhi cho BN &gt; 16 tuổi
    ///   KT47  / S006.122 – Vận động trị liệu hô hấp không phải bệnh phổi mạn
    ///   KT57  / S006.117 – Định lượng SCC không phải ung thư cổ tử cung / phổi
    ///   KT79  / S006.1   – PHCN cho BN ung thư (chống chỉ định)
    ///   KT84  / S011.1   – Nội soi tiêu hóa (TQDDTT) cho BN suy tim
    ///   KT144 / S018.4   – Tiêm/truyền TM, thay băng... cho BN nội trú (chỉ áp dụng ngoại trú)
    ///   KT147 / S016.12  – DVKT PHCN vượt số lượng cho phép/ngày
    ///   KT166 / T001.3   – Hút đờm qua NKQ/canuyn không có thở máy
    /// Mỗi rule là 1 method CheckKtXxx riêng → dễ bật/tắt từng rule.
    /// </summary>
    public class DieuKienDacBiet : GiamDinhBase
    {
        // KT23 – DVKT chuyên khoa Nhi cho BN > 16 tuổi (D011.1) – TT 43/2013/TT-BYT
        private static readonly string[] MaDvktNhiPrefixes = { "35.NHI", "nhi." };   // placeholder – cần map theo danh mục Nhi
        private static readonly HashSet<string> MaDvktNhi = new HashSet<string>
        {
            "35.001", "35.002", "35.003",   // DVKT chuyên khoa Nhi (placeholder)
        };
        private const int TuoiNhiMax = 16;   // BN > 16 tuổi không được dùng mã Nhi

        // KT47 – Vận động trị liệu hô hấp (S006.122) – chỉ áp dụng COPD hoặc bệnh phổi mạn giai đoạn ổn định
        private static readonly HashSet<string> MaVdTlHoHap = new HashSet<string>
        {
            "40.1981", "40.198100",   // Vận động trị liệu hô hấp (placeholder)
            "34.001", "34.00100",
        };
        private static readonly string[] IcdCopdPhoiMan = { "J44", "J41", "J42", "J43", "J47", "J84" };

        // KT57 – Định lượng SCC (S006.117) – chỉ ung thư cổ tử cung (C53) hoặc ung thư phổi (C33, C34)
        private static readonly HashSet<string> MaXnScc = new HashSet<string>
        {
            "09.SCC1", "09.SCC100",   // Định lượng SCC (placeholder)
            "09.200", "09.20000",
        };
        private static readonly string[] IcdSccDuocPhep = { "C53", "C33", "C34" };

        // KT79 – PHCN cho BN ung thư (S006.1)
        private static readonly string[] MaPhcnPrefixes = { "34.", "33." };   // nhóm PHCN
        private static readonly string[] IcdUngThu = Enumerable.Range(0, 98).Select(i => "C" + i.ToString("00")).ToArray();   // C00-C97

        // KT84 – Nội soi TQDDTT cho BN suy tim (S011.1)
        private static readonly HashSet<string> MaNsTqddtt = new HashSet<string>
        {
            "36.010", "36.011", "36.01000", "36.01100",   // NS chẩn đoán
            "36.014", "36.015", "36.01400", "36.01500",   // QT 63, 64
        };
        private static readonly string[] IcdSuyTim = { "I50" };

        // KT144 – Tiêm/truyền/thay băng/cắt chỉ... cho BN nội trú (S018.4) – chỉ áp dụng ngoại trú
        private static readonly HashSet<string> MaNgoaitruOnly = new HashSet<string>
        {
            "41.001", "41.00100",   // Tiêm bắp
            "41.002", "41.00200",   // Tiêm tĩnh mạch
            "41.003", "41.00300",   // Truyền TM
            "41.010", "41.01000",   // Tháo bột
            "41.020", "41.02000",   // Cắt chỉ
        };
        private static readonly HashSet<string> MaLoaiKcbNoiTru = new HashSet<string> { "03", "04", "06" };

        // KT147 – PHCN vượt số lượng/ngày (S016.12): ma_dvkt → số lần tối đa / ngày
        private static readonly Dictionary<string, int> MaPhcnGioiHan = new Dictionary<string, int>
        {
            { "34.018", 1 },   // Vận động trị liệu hô hấp (STT 18 Danh mục 2 TT50)
            { "34.01800", 1 },
        };

        // KT166 – Hút đờm qua NKQ/canuyn không có thở máy (T001.3)
        private static readonly HashSet<string> MaHutDomNkq = new HashSet<string>
        {
            "41.HUT1", "41.HUT100",   // Hút đờm qua NKQ/canuyn kín (placeholder)
            "41.050", "41.05000",
        };
        private static readonly HashSet<string> MaThoMay = new HashSet<string>
        {
            "41.TM1", "41.TM100",     // Thở máy các loại (placeholder)
            "41.100", "41.10000", "41.10001", "41.10002",
        };

        // Căn cứ
        private const string CcKt23 = "TT 43/2013/TT-BYT";
        private const string CcKt47 = "QĐ 1981/QĐ-BYT ngày 05/06/2014 (QTKT Nội khoa Hô hấp)";
        private const string CcKt57 = "TT 35/TT-BYT (Danh mục DVKT BHYT)";
        private const string CcKt79 = "QĐ 54/QĐ-BYT ngày 06/01/2014 + QĐ 5737/QĐ-BYT ngày 22/12/2017";
        private const string CcKt84 = "QĐ 3805/QĐ-BYT ngày 25/9/2014 (QT số 14, 15, 63, 64)";
        private const string CcKt144 = "TT 50/2017/TT-BYT";
        private const string CcKt147 = "TT 50/2017/TT-BYT Danh mục 2 STT 18";
        private const string CcKt166 = "Phụ lục 02 CV /GĐĐT-NVGĐ tháng 12/2022 – KT166";

        /// <summary>Tính tuổi BN tại thời điểm khám. Trả null nếu không parse được.</summary>
        private static int? TinhTuoi(string ngaySinh, string ngayKham)
        {
            string ns = Left8(ngaySinh);
            string nk = Left8(ngayKham);
            if (ns.Length < 8 || nk.Length < 8) return null;

            long yearSinh, yearKham, monthSinh, monthKham, daySinh, dayKham;
            if (!ParseHelper.TryPyInt(ns.Substring(0, 4), out yearSinh)) return null;
            if (!ParseHelper.TryPyInt(nk.Substring(0, 4), out yearKham)) return null;
            if (!ParseHelper.TryPyInt(ns.Substring(4, 2), out monthSinh)) return null;
            if (!ParseHelper.TryPyInt(nk.Substring(4, 2), out monthKham)) return null;
            if (!ParseHelper.TryPyInt(ns.Substring(6, 2), out daySinh)) return null;
            if (!ParseHelper.TryPyInt(nk.Substring(6, 2), out dayKham)) return null;

            long tuoi = yearKham - yearSinh;
            if (monthKham < monthSinh || (monthKham == monthSinh && dayKham < daySinh))
                tuoi--;
            return (int)tuoi;
        }

        private static string Left8(string s)
        {
            s = s ?? "";
            return s.Length > 8 ? s.Substring(0, 8) : s;
        }

        private static bool StartsWithAny(string s, string[] prefixes)
        {
            foreach (string p in prefixes)
                if (s.StartsWith(p, StringComparison.Ordinal)) return true;
            return false;
        }

        /// <summary>MA_LK (không trim) → ICDHelper; dòng sau ghi đè dòng trước.</summary>
        private static Dictionary<string, ICDHelper> BuildIcdMap(IDictionary<string, List<BhytBase>> allObjects)
        {
            var map = new Dictionary<string, ICDHelper>();
            foreach (SheetRow r in GetRows(allObjects, "XML1"))
                map[r.Record.GetValue("MA_LK") ?? ""] = ICDHelper.FromRecord(r.Record);
            return map;
        }

        private static ICDHelper IcdOf(Dictionary<string, ICDHelper> map, string maLk)
        {
            ICDHelper icd;
            return map.TryGetValue(maLk, out icd) ? icd : new ICDHelper("", "");
        }

        public override List<GiamDinhError> Check(IDictionary<string, List<BhytBase>> allObjects)
        {
            var errors = new List<GiamDinhError>();
            errors.AddRange(CheckKt23(allObjects));
            errors.AddRange(CheckKt47(allObjects));
            errors.AddRange(CheckKt57(allObjects));
            errors.AddRange(CheckKt79(allObjects));
            errors.AddRange(CheckKt84(allObjects));
            errors.AddRange(CheckKt144(allObjects));
            errors.AddRange(CheckKt147(allObjects));
            errors.AddRange(CheckKt166(allObjects));
            return errors;
        }

        // KT23 – DVKT Nhi cho BN > 16 tuổi
        private List<GiamDinhError> CheckKt23(IDictionary<string, List<BhytBase>> allObjects)
        {
            var errors = new List<GiamDinhError>();
            var xml1Map = new Dictionary<string, BhytBase>();
            foreach (SheetRow r in GetRows(allObjects, "XML1"))
                xml1Map[r.Record.GetValue("MA_LK") ?? ""] = r.Record;

            foreach (SheetRow row in GetRows(allObjects, "XML3"))
            {
                string maLk = (row.Record.GetValue("MA_LK") ?? "").Trim();
                string maDv = (row.Record.GetValue("MA_DICH_VU") ?? "").Trim();
                if (maLk.Length == 0 || maDv.Length == 0) continue;

                bool isNhi = MaDvktNhi.Contains(maDv) || StartsWithAny(maDv, MaDvktNhiPrefixes);
                if (!isNhi) continue;

                BhytBase rec1;
                if (!xml1Map.TryGetValue(maLk, out rec1)) continue;

                int? tuoi = TinhTuoi(rec1.GetValue("NGAY_SINH") ?? "", rec1.GetValue("NGAY_VAO") ?? "");
                if (tuoi != null && tuoi.Value > TuoiNhiMax)
                {
                    errors.Add(new GiamDinhError
                    {
                        Sheet = "XML3", MaLk = maLk, RowExcel = row.RowExcel,
                        MaLyDo = "D011.1",
                        MoTa = $"DVKT chuyên khoa Nhi '{maDv}' áp dụng cho BN " +
                               $"{tuoi.Value} tuổi (> {TuoiNhiMax} tuổi). " +
                               "Không được thanh toán theo mã và giá Nhi.",
                        CanCu = CcKt23, MaDichVu = maDv,
                    });
                }
            }
            return errors;
        }

        // KT47 – Vận động trị liệu hô hấp không phải COPD/bệnh phổi mạn
        private List<GiamDinhError> CheckKt47(IDictionary<string, List<BhytBase>> allObjects)
        {
            var errors = new List<GiamDinhError>();
            var icdMap = BuildIcdMap(allObjects);
            foreach (SheetRow row in GetRows(allObjects, "XML3"))
            {
                string maLk = (row.Record.GetValue("MA_LK") ?? "").Trim();
                string maDv = (row.Record.GetValue("MA_DICH_VU") ?? "").Trim();
                if (maLk.Length == 0 || !MaVdTlHoHap.Contains(maDv)) continue;

                ICDHelper icd = IcdOf(icdMap, maLk);
                if (!icd.HasAny(IcdCopdPhoiMan))
                {
                    errors.Add(new GiamDinhError
                    {
                        Sheet = "XML3", MaLk = maLk, RowExcel = row.RowExcel,
                        MaLyDo = "S006.122",
                        MoTa = $"DVKT Vận động trị liệu hô hấp '{maDv}' chỉ định cho BN " +
                               $"không mắc COPD/bệnh phổi mạn giai đoạn ổn định. {icd}",
                        CanCu = CcKt47, MaDichVu = maDv,
                    });
                }
            }
            return errors;
        }

        // KT57 – Định lượng SCC không phải ung thư CTC / ung thư phổi
        private List<GiamDinhError> CheckKt57(IDictionary<string, List<BhytBase>> allObjects)
        {
            var errors = new List<GiamDinhError>();
            var icdMap = BuildIcdMap(allObjects);
            foreach (SheetRow row in GetRows(allObjects, "XML3"))
            {
                string maLk = (row.Record.GetValue("MA_LK") ?? "").Trim();
                string maDv = (row.Record.GetValue("MA_DICH_VU") ?? "").Trim();
                if (maLk.Length == 0 || !MaXnScc.Contains(maDv)) continue;

                ICDHelper icd = IcdOf(icdMap, maLk);
                if (!icd.HasAny(IcdSccDuocPhep))
                {
                    errors.Add(new GiamDinhError
                    {
                        Sheet = "XML3", MaLk = maLk, RowExcel = row.RowExcel,
                        MaLyDo = "S006.117",
                        MoTa = $"Xét nghiệm Định lượng SCC '{maDv}' chỉ định cho BN " +
                               "không có chẩn đoán ung thư cổ tử cung (C53) " +
                               $"hoặc ung thư phổi (C33/C34). {icd}",
                        CanCu = CcKt57, MaDichVu = maDv,
                    });
                }
            }
            return errors;
        }

        // KT79 – PHCN cho BN ung thư
        private List<GiamDinhError> CheckKt79(IDictionary<string, List<BhytBase>> allObjects)
        {
            var errors = new List<GiamDinhError>();
            var icdMap = BuildIcdMap(allObjects);
            foreach (SheetRow row in GetRows(allObjects, "XML3"))
            {
                string maLk = (row.Record.GetValue("MA_LK") ?? "").Trim();
                string maDv = (row.Record.GetValue("MA_DICH_VU") ?? "").Trim();
                if (maLk.Length == 0 || maDv.Length == 0) continue;
                if (!StartsWithAny(maDv, MaPhcnPrefixes)) continue;

                ICDHelper icd = IcdOf(icdMap, maLk);
                if (icd.HasAny(IcdUngThu))
                {
                    errors.Add(new GiamDinhError
                    {
                        Sheet = "XML3", MaLk = maLk, RowExcel = row.RowExcel,
                        MaLyDo = "S006.1",
                        MoTa = $"DVKT PHCN '{maDv}' bị chống chỉ định: " +
                               $"BN có chẩn đoán khối u ác tính. {icd}",
                        CanCu = CcKt79, MaDichVu = maDv,
                    });
                }
            }
            return errors;
        }

        // KT84 – Nội soi TQDDTT cho BN suy tim
        private List<GiamDinhError> CheckKt84(IDictionary<string, List<BhytBase>> allObjects)
        {
            var errors = new List<GiamDinhError>();
            var icdMap = BuildIcdMap(allObjects);
            foreach (SheetRow row in GetRows(allObjects, "XML3"))
            {
                string maLk = (row.Record.GetValue("MA_LK") ?? "").Trim();
                string maDv = (row.Record.GetValue("MA_DICH_VU") ?? "").Trim();
                if (maLk.Length == 0 || !MaNsTqddtt.Contains(maDv)) continue;

                ICDHelper icd = IcdOf(icdMap, maLk);
                if (icd.HasAny(IcdSuyTim))
                {
                    errors.Add(new GiamDinhError
                    {
                        Sheet = "XML3", MaLk = maLk, RowExcel = row.RowExcel,
                        MaLyDo = "S011.1",
                        MoTa = $"Nội soi TQDDTT '{maDv}' chống chỉ định: " +
                               $"BN có chẩn đoán suy tim (I50). {icd}",
                        CanCu = CcKt84, MaDichVu = maDv,
                    });
                }
            }
            return errors;
        }

        // KT144 – Tiêm/truyền/thay băng... cho BN nội trú
        private List<GiamDinhError> CheckKt144(IDictionary<string, List<BhytBase>> allObjects)
        {
            var errors = new List<GiamDinhError>();
            // MA_LK (không trim) → MA_LOAI_KCB
            var loaiKcbMap = new Dictionary<string, string>();
            foreach (SheetRow r in GetRows(allObjects, "XML1"))
                loaiKcbMap[r.Record.GetValue("MA_LK") ?? ""] = (r.Record.GetValue("MA_LOAI_KCB") ?? "").Trim();

            foreach (SheetRow row in GetRows(allObjects, "XML3"))
            {
                string maLk = (row.Record.GetValue("MA_LK") ?? "").Trim();
                string maDv = (row.Record.GetValue("MA_DICH_VU") ?? "").Trim();
                if (maLk.Length == 0 || !MaNgoaitruOnly.Contains(maDv)) continue;

                string loai;
                if (!loaiKcbMap.TryGetValue(maLk, out loai)) loai = "";
                if (Array.Exists(ParseHelper.SplitList(loai), x => MaLoaiKcbNoiTru.Contains(x)))
                {
                    errors.Add(new GiamDinhError
                    {
                        Sheet = "XML3", MaLk = maLk, RowExcel = row.RowExcel,
                        MaLyDo = "S018.4",
                        MoTa = $"DVKT '{maDv}' (tiêm/truyền/thay băng/cắt chỉ...) " +
                               "chỉ áp dụng cho BN ngoại trú. " +
                               $"Lần khám này là nội trú (MA_LOAI_KCB={loai}). " +
                               "Không thanh toán riêng khi BN nội trú.",
                        CanCu = CcKt144, MaDichVu = maDv,
                    });
                }
            }
            return errors;
        }

        private class Group
        {
            public string MaLk, MaDv, Ngay;
            public List<SheetRow> Rows = new List<SheetRow>();
        }

        // KT147 – DVKT PHCN vượt số lượng/ngày
        private List<GiamDinhError> CheckKt147(IDictionary<string, List<BhytBase>> allObjects)
        {
            var errors = new List<GiamDinhError>();
            var groups = new Dictionary<string, Group>();
            foreach (SheetRow row in GetRows(allObjects, "XML3"))
            {
                string maLk = (row.Record.GetValue("MA_LK") ?? "").Trim();
                string maDv = (row.Record.GetValue("MA_DICH_VU") ?? "").Trim();
                string ngay = Left8(row.Record.GetValue("NGAY_TH_YL") ?? "");
                if (MaPhcnGioiHan.ContainsKey(maDv) && maLk.Length > 0)
                {
                    string key = maLk + "\u0001" + maDv + "\u0001" + ngay;
                    Group g;
                    if (!groups.TryGetValue(key, out g))
                    {
                        g = new Group { MaLk = maLk, MaDv = maDv, Ngay = ngay };
                        groups[key] = g;
                    }
                    g.Rows.Add(row);
                }
            }

            foreach (Group g in groups.Values)
            {
                int maxLan = MaPhcnGioiHan[g.MaDv];
                if (g.Rows.Count > maxLan)
                {
                    for (int i = maxLan; i < g.Rows.Count; i++)
                    {
                        errors.Add(new GiamDinhError
                        {
                            Sheet = "XML3", MaLk = g.MaLk, RowExcel = g.Rows[i].RowExcel,
                            MaLyDo = "S016.12",
                            MoTa = $"DVKT PHCN '{g.MaDv}' thực hiện {g.Rows.Count} lần " +
                                   $"ngày {g.Ngay}, vượt mức tối đa {maxLan} lần/ngày.",
                            CanCu = CcKt147, MaDichVu = g.MaDv,
                        });
                    }
                }
            }
            return errors;
        }

        // KT166 – Hút đờm qua NKQ/canuyn không có thở máy
        private List<GiamDinhError> CheckKt166(IDictionary<string, List<BhytBase>> allObjects)
        {
            var errors = new List<GiamDinhError>();

            // MA_LK có DVKT thở máy
            var lkCoThoMay = new HashSet<string>();
            foreach (SheetRow row in GetRows(allObjects, "XML3"))
            {
                string maLk = (row.Record.GetValue("MA_LK") ?? "").Trim();
                string maDv = (row.Record.GetValue("MA_DICH_VU") ?? "").Trim();
                if (maLk.Length > 0 && MaThoMay.Contains(maDv)) lkCoThoMay.Add(maLk);
            }

            foreach (SheetRow row in GetRows(allObjects, "XML3"))
            {
                string maLk = (row.Record.GetValue("MA_LK") ?? "").Trim();
                string maDv = (row.Record.GetValue("MA_DICH_VU") ?? "").Trim();
                if (maLk.Length > 0 && MaHutDomNkq.Contains(maDv) && !lkCoThoMay.Contains(maLk))
                {
                    errors.Add(new GiamDinhError
                    {
                        Sheet = "XML3", MaLk = maLk, RowExcel = row.RowExcel,
                        MaLyDo = "T001.3",
                        MoTa = $"DVKT Hút đờm qua NKQ/canuyn '{maDv}' thực hiện cho BN " +
                               "không có DVKT thở máy trong cùng lần khám. " +
                               "Không thanh toán khi BN không sử dụng thở máy.",
                        CanCu = CcKt166, MaDichVu = maDv,
                    });
                }
            }
            return errors;
        }
    }
}
