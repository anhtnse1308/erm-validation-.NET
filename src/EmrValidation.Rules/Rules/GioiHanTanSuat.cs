using System;
using System.Collections.Generic;
using EmrValidation.Core.GiamDinh;
using EmrValidation.Core.Models;

namespace EmrValidation.Rules
{
    /// <summary>
    /// Rule – Giới hạn tần suất thực hiện DVKT trong ngày (mã từ chối S011.1, S016.47, D002.2)
    ///   KT184 / S011.1  – AFB nhuộm trực tiếp: không quá 2 lần/ngày (QĐ 1314/QĐ-BYT ngày 24/3/2022)
    ///   KT193 / S016.47 – Monitoring sản khoa: 1 lần/ngày (QĐ 7435/QĐ-BYT ngày 14/12/2018 + TT 13/2019/TT-BYT)
    ///   KT183 / D002.2  – Chụp CT có thuốc cản quang nhưng không kê thuốc cản quang (TT 13/2019/TT-BYT)
    ///   CV 1008/2022    – Khám bệnh (MA_LOAI_KCB = 01): DVKT lặp > 2 lần cùng mã, cùng ngày y lệnh
    /// </summary>
    public class GioiHanTanSuat : GiamDinhBase
    {
        // KT184 – Mã DVKT xét nghiệm AFB nhuộm trực tiếp
        private static readonly HashSet<string> MaAfb = new HashSet<string>
        {
            "09.AFB1", "09.AFB2",           // placeholder – thay bằng mã thực tế
            "09.001", "09.00100",           // AFB trực tiếp nhuộm Ziehl-Neelsen
            "09.002", "09.00200",           // AFB trực tiếp nhuộm huỳnh quang
        };
        private const int MaxAfbNgay = 2;

        // KT193 – Mã DVKT Monitoring sản khoa
        private static readonly HashSet<string> MaMonitoringSan = new HashSet<string>
        {
            "35.5356", "35.535600",         // STT 5356 theo QĐ 7435/QĐ-BYT
            "35.001", "35.00100",           // Monitoring sản khoa (placeholder)
        };
        private const int MaxMonitoringNgay = 1;

        // KT183 – CT có thuốc cản quang
        private static readonly string[] MaCtCoCanQuangPrefixes = { "18.CT.CQ" };   // placeholder prefix
        private static readonly HashSet<string> MaCtCoCanQuang = new HashSet<string>
        {
            "18.100", "18.101", "18.102", "18.103", "18.104",
            "18.10000", "18.10100", "18.10200", "18.10300",
        };

        // Mã thuốc cản quang trong XML2 (MA_THUOC hoặc MA_NHOM)
        private static readonly HashSet<string> MaThuocCanQuang = new HashSet<string>
        {
            "40.CQ1", "40.CQ2",             // placeholder
            "40.500", "40.501", "40.502",   // thuốc cản quang iod
            "40.503", "40.504",
        };
        private static readonly HashSet<string> MaNhomCanQuang = new HashSet<string> { "6" };

        private const string CanCuAfb = "QĐ 1314/QĐ-BYT ngày 24/3/2022 (Hướng dẫn CĐ-ĐT-DP bệnh lao)";
        private const string CanCuMonitoring = "QĐ 7435/QĐ-BYT ngày 14/12/2018 (STT 5356) + TT 13/2019/TT-BYT";
        private const string CanCuCtCq = "TT 13/2019/TT-BYT Phụ lục III – Điều chỉnh giá CT có/không cản quang";

        // Khám bệnh (MA_LOAI_KCB=01) – DVKT lặp > 2 lần cùng mã, cùng ngày y lệnh
        private const string MaLoaiKcbKhamBenh = "01";
        private const int MaxDvktLapNgay = 2;
        private const string CanCuDvktLapKham = "CV 1008/BHXH-GĐBHYT ngày tháng 07/2022 (BHXH tỉnh Bình Dương)";

        /// <summary>Lấy 8 ký tự ngày từ chuỗi DATE12 hoặc DATE8.</summary>
        private static string Ngay(string s)
        {
            s = s ?? "";
            return s.Length > 8 ? s.Substring(0, 8) : s;
        }

        private static bool IsCtCoCq(string maDv)
        {
            maDv = maDv.Trim();
            if (MaCtCoCanQuang.Contains(maDv)) return true;
            foreach (string p in MaCtCoCanQuangPrefixes)
                if (maDv.StartsWith(p, StringComparison.Ordinal)) return true;
            return false;
        }

        private class Group
        {
            public string MaLk;
            public string Key2;     // ngày (KT184/KT193) hoặc MA_DICH_VU (CV 1008)
            public string Key3;     // ngày y lệnh (CV 1008)
            public List<SheetRow> Rows = new List<SheetRow>();
        }

        public override List<GiamDinhError> Check(IDictionary<string, List<BhytBase>> allObjects)
        {
            var errors = new List<GiamDinhError>();
            errors.AddRange(CheckTanSuat(allObjects));
            errors.AddRange(CheckCtCanQuang(allObjects));
            errors.AddRange(CheckDvktLapLaiKhamBenh(allObjects));
            return errors;
        }

        // KT184 + KT193 – Đếm số lần DVKT trong ngày
        private List<GiamDinhError> CheckTanSuat(IDictionary<string, List<BhytBase>> allObjects)
        {
            var errors = new List<GiamDinhError>();

            // (tập mã, số lần tối đa, mã lý do, căn cứ, tên DV)
            var rules = new[]
            {
                new { MaSet = MaAfb, MaxLan = MaxAfbNgay, MaLyDo = "S011.1", CanCu = CanCuAfb,
                      TenDv = "Xét nghiệm AFB trực tiếp" },
                new { MaSet = MaMonitoringSan, MaxLan = MaxMonitoringNgay, MaLyDo = "S016.47", CanCu = CanCuMonitoring,
                      TenDv = "Monitoring sản khoa (theo dõi tim thai + cơn co TC)" },
            };

            List<SheetRow> rowsXml3 = GetRows(allObjects, "XML3");

            foreach (var rule in rules)
            {
                // Group: (MA_LK, ngày) → danh sách dòng
                var groups = new Dictionary<string, Group>();
                foreach (SheetRow row in rowsXml3)
                {
                    string maDv = (row.Record.GetValue("MA_DICH_VU") ?? "").Trim();
                    string maLk = (row.Record.GetValue("MA_LK") ?? "").Trim();
                    string ngay = Ngay(row.Record.GetValue("NGAY_TH_YL") ?? "");
                    if (rule.MaSet.Contains(maDv) && maLk.Length > 0)
                    {
                        string key = maLk + "\u0001" + ngay;
                        Group g;
                        if (!groups.TryGetValue(key, out g))
                        {
                            g = new Group { MaLk = maLk, Key2 = ngay };
                            groups[key] = g;
                        }
                        g.Rows.Add(row);
                    }
                }

                foreach (Group g in groups.Values)
                {
                    if (g.Rows.Count > rule.MaxLan)
                    {
                        // Báo lỗi trên các dòng thừa (từ lần max_lan+1 trở đi)
                        for (int i = rule.MaxLan; i < g.Rows.Count; i++)
                        {
                            SheetRow row = g.Rows[i];
                            string maDv = (row.Record.GetValue("MA_DICH_VU") ?? "").Trim();
                            errors.Add(new GiamDinhError
                            {
                                Sheet = "XML3",
                                MaLk = g.MaLk,
                                RowExcel = row.RowExcel,
                                MaLyDo = rule.MaLyDo,
                                MoTa = $"'{rule.TenDv}' thực hiện {g.Rows.Count} lần " +
                                       $"trong ngày {g.Key2}, vượt mức tối đa " +
                                       $"{rule.MaxLan} lần/ngày. " +
                                       $"Lần thứ {i + 1} bị từ chối.",
                                CanCu = rule.CanCu,
                                MaDichVu = maDv,
                            });
                        }
                    }
                }
            }

            return errors;
        }

        // KT183 – CT có thuốc cản quang nhưng không kê thuốc cản quang
        private List<GiamDinhError> CheckCtCanQuang(IDictionary<string, List<BhytBase>> allObjects)
        {
            var errors = new List<GiamDinhError>();

            // Tập MA_LK có thuốc cản quang trong XML2
            var lkCoThuocCq = new HashSet<string>();
            foreach (SheetRow row in GetRows(allObjects, "XML2"))
            {
                string maLk = (row.Record.GetValue("MA_LK") ?? "").Trim();
                string maThuoc = (row.Record.GetValue("MA_THUOC") ?? "").Trim();
                string maNhom = (row.Record.GetValue("MA_NHOM") ?? "").Trim();
                if (maLk.Length > 0 && (MaThuocCanQuang.Contains(maThuoc) || MaNhomCanQuang.Contains(maNhom)))
                    lkCoThuocCq.Add(maLk);
            }

            // XML3: CT có CQ nhưng MA_LK không có thuốc CQ
            foreach (SheetRow row in GetRows(allObjects, "XML3"))
            {
                string maLk = (row.Record.GetValue("MA_LK") ?? "").Trim();
                string maDv = (row.Record.GetValue("MA_DICH_VU") ?? "").Trim();
                if (maLk.Length == 0 || maDv.Length == 0) continue;
                if (IsCtCoCq(maDv) && !lkCoThuocCq.Contains(maLk))
                {
                    errors.Add(new GiamDinhError
                    {
                        Sheet = "XML3",
                        MaLk = maLk,
                        RowExcel = row.RowExcel,
                        MaLyDo = "D002.2",
                        MoTa = $"DVKT '{maDv}' là Chụp CT có thuốc cản quang nhưng " +
                               "không có thuốc cản quang trong XML2 của lần khám này. " +
                               "Điều chỉnh về giá CT không thuốc cản quang theo TT 13/2019.",
                        CanCu = CanCuCtCq,
                        MaDichVu = maDv,
                    });
                }
            }

            return errors;
        }

        // CV 1008/2022 – Khám bệnh (MA_LOAI_KCB=1): DVKT lặp > 2 lần cùng mã DVKT, cùng ngày y lệnh
        private List<GiamDinhError> CheckDvktLapLaiKhamBenh(IDictionary<string, List<BhytBase>> allObjects)
        {
            var errors = new List<GiamDinhError>();

            // MA_LK → MA_LOAI_KCB (từ XML1); dòng sau ghi đè dòng trước
            var loaiKcbMap = new Dictionary<string, string>();
            foreach (SheetRow row in GetRows(allObjects, "XML1"))
            {
                string k = (row.Record.GetValue("MA_LK") ?? "").Trim();
                loaiKcbMap[k] = (row.Record.GetValue("MA_LOAI_KCB") ?? "").Trim();
            }

            // Group XML3 theo (MA_LK, MA_DICH_VU, NGAY_YL) – chỉ với lần khám MA_LOAI_KCB=1
            var groups = new Dictionary<string, Group>();
            foreach (SheetRow row in GetRows(allObjects, "XML3"))
            {
                string maLk = (row.Record.GetValue("MA_LK") ?? "").Trim();
                string maDv = (row.Record.GetValue("MA_DICH_VU") ?? "").Trim();
                string ngayYl = Ngay(row.Record.GetValue("NGAY_YL") ?? "");
                if (maLk.Length == 0 || maDv.Length == 0) continue;

                string loai;
                if (!loaiKcbMap.TryGetValue(maLk, out loai)) loai = "";
                if (loai != MaLoaiKcbKhamBenh) continue;

                string key = maLk + "\u0001" + maDv + "\u0001" + ngayYl;
                Group g;
                if (!groups.TryGetValue(key, out g))
                {
                    g = new Group { MaLk = maLk, Key2 = maDv, Key3 = ngayYl };
                    groups[key] = g;
                }
                g.Rows.Add(row);
            }

            foreach (Group g in groups.Values)
            {
                if (g.Rows.Count > MaxDvktLapNgay)
                {
                    for (int i = MaxDvktLapNgay; i < g.Rows.Count; i++)
                    {
                        SheetRow row = g.Rows[i];
                        errors.Add(new GiamDinhError
                        {
                            Sheet = "XML3",
                            MaLk = g.MaLk,
                            RowExcel = row.RowExcel,
                            MaLyDo = "WARN.DVKT_LAP",
                            MoTa = $"Khám bệnh (MA_LOAI_KCB=01): DVKT '{g.Key2}' đề nghị " +
                                   $"thanh toán {g.Rows.Count} lần cùng ngày y lệnh {g.Key3}, " +
                                   $"vượt mức {MaxDvktLapNgay} lần. " +
                                   "Kiểm tra lỗi nhập liệu trùng dòng hoặc khai tăng DVKT.",
                            CanCu = CanCuDvktLapKham,
                            MaDichVu = g.Key2,
                        });
                    }
                }
            }

            return errors;
        }
    }
}
