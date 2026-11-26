using System;
using System.Collections.Generic;
using EmrValidation.Core.GiamDinh;
using EmrValidation.Core.Models;

namespace EmrValidation.Rules
{
    /// <summary>
    /// Rule III – Tách lượt KCB + KCB sau ngày chết
    /// Căn cứ: QĐ 4210/QĐ-BYT ngày 20/09/2017 + CV 3370/BHXH-GĐĐT ngày 26/10/2020
    ///
    /// III.1 – Thận nhân tạo ngoại trú: tối đa 12 lượt / 1 BN / 1 năm (group XML1 theo MA_BN + NAM_QT)
    /// III.2 – KCB sau ngày chết: lần khám khác cùng MA_BN có NGAY_VAO sau NGAY_RA của lượt tử vong (KET_QUA_DTRI = 5)
    /// </summary>
    public class LuotKhamBenhNhan : GiamDinhBase
    {
        // Mã DVKT thận nhân tạo (prefix)
        private static readonly string[] MaThanNhanTaoPrefixes = { "13." };

        // ICD-10 thận nhân tạo chu kỳ
        private static readonly string[] IcdThanNhanTao =
        {
            "N18.5", "N18.6", "N18.9", "Z49", "Z49.0", "Z49.1", "Z49.2",
        };

        private const int MaxLuotThanNam = 12;

        private const string CanCuTachLuot = "Chỉ tiêu số 35 Bảng 1, QĐ 4210/QĐ-BYT ngày 20/09/2017";
        private const string CanCuChet = "CV 3370/BHXH-GĐĐT ngày 26/10/2020 + NĐ 117/2020/NĐ-CP ngày 28/09/2020";

        private static bool IsThanNhanTao(string maBenh, HashSet<string> dvSet)
        {
            if (!string.IsNullOrEmpty(maBenh))
            {
                foreach (string icd in IcdThanNhanTao)
                {
                    if (maBenh.StartsWith(icd, StringComparison.Ordinal) ||
                        icd.StartsWith(maBenh, StringComparison.Ordinal))
                        return true;
                }
            }
            foreach (string dv in dvSet)
            {
                foreach (string p in MaThanNhanTaoPrefixes)
                    if (dv.StartsWith(p, StringComparison.Ordinal)) return true;
            }
            return false;
        }

        private class ThanGroup
        {
            public string MaBn;
            public string NamQt;
            public List<SheetRow> Rows = new List<SheetRow>();
        }

        public override List<GiamDinhError> Check(IDictionary<string, List<BhytBase>> allObjects)
        {
            var errors = new List<GiamDinhError>();

            List<SheetRow> xml1Rows = GetRows(allObjects, "XML1");

            // DVKT của mỗi MA_LK từ XML3
            var lkToDvkt = new Dictionary<string, HashSet<string>>();
            foreach (SheetRow row in GetRows(allObjects, "XML3"))
            {
                string maLk = row.Record.GetValue("MA_LK") ?? "";
                string maDv = row.Record.GetValue("MA_DICH_VU") ?? "";
                if (maLk.Length > 0 && maDv.Length > 0)
                {
                    HashSet<string> set;
                    if (!lkToDvkt.TryGetValue(maLk, out set))
                    {
                        set = new HashSet<string>();
                        lkToDvkt[maLk] = set;
                    }
                    set.Add(maDv.Trim());
                }
            }

            // ===== III.1 – Thận nhân tạo ngoại trú > 12 lượt/BN/năm =====
            var thanGroups = new Dictionary<string, ThanGroup>();
            var emptySet = new HashSet<string>();

            foreach (SheetRow row in xml1Rows)
            {
                BhytBase rec = row.Record;
                string maBn = rec.GetValue("MA_BN") ?? "";
                string namQt = rec.GetValue("NAM_QT") ?? "";
                string maLoaiKcb = rec.GetValue("MA_LOAI_KCB") ?? "";
                string maBenh = rec.GetValue("MA_BENH_CHINH") ?? "";
                string maLk = rec.GetValue("MA_LK") ?? "";

                // Chỉ xét ngoại trú (MA_LOAI_KCB ∈ {02, 05, 08})
                if (maLoaiKcb != "02" && maLoaiKcb != "05" && maLoaiKcb != "08") continue;
                if (maBn.Length == 0 || namQt.Length == 0) continue;

                HashSet<string> dvktSet;
                if (!lkToDvkt.TryGetValue(maLk, out dvktSet)) dvktSet = emptySet;

                if (IsThanNhanTao(maBenh, dvktSet))
                {
                    string key = maBn + "\u0001" + namQt;
                    ThanGroup grp;
                    if (!thanGroups.TryGetValue(key, out grp))
                    {
                        grp = new ThanGroup { MaBn = maBn, NamQt = namQt };
                        thanGroups[key] = grp;
                    }
                    grp.Rows.Add(row);
                }
            }

            foreach (ThanGroup grp in thanGroups.Values)
            {
                if (grp.Rows.Count > MaxLuotThanNam)
                {
                    // Báo lỗi trên toàn bộ lượt thừa (từ lượt thứ 13 trở đi)
                    foreach (SheetRow row in grp.Rows.GetRange(MaxLuotThanNam, grp.Rows.Count - MaxLuotThanNam))
                    {
                        errors.Add(new GiamDinhError
                        {
                            Sheet = "XML1",
                            MaLk = row.Record.GetValue("MA_LK") ?? "",
                            RowExcel = row.RowExcel,
                            MaLyDo = "",  // Không có mã lý do cụ thể trong văn bản
                            MoTa = $"BN '{grp.MaBn}' năm {grp.NamQt}: " +
                                   $"có {grp.Rows.Count} lượt thận nhân tạo ngoại trú, " +
                                   $"vượt mức tối đa {MaxLuotThanNam} lượt/năm. " +
                                   "Các lượt từ thứ 13 trở đi cần điều chỉnh lại theo quy định " +
                                   "(mỗi tháng chỉ gửi 1 lượt từ ngày 01 đến ngày cuối tháng).",
                            CanCu = CanCuTachLuot,
                        });
                    }
                }
            }

            // ===== III.2 – KCB sau ngày tử vong =====
            var tuVongMap = new Dictionary<string, List<string>>();
            foreach (SheetRow row in xml1Rows)
            {
                BhytBase rec = row.Record;
                string maBn = rec.GetValue("MA_BN") ?? "";
                string ketQua = (rec.GetValue("KET_QUA_DTRI") ?? "").Trim();
                string ngayRa = (rec.GetValue("NGAY_RA") ?? "").Trim();

                // KET_QUA_DTRI = 5: tử vong tại cơ sở KBCB
                if (ketQua == "5" && maBn.Length > 0 && ngayRa.Length > 0 && ngayRa.Length >= 8)
                {
                    List<string> list;
                    if (!tuVongMap.TryGetValue(maBn, out list))
                    {
                        list = new List<string>();
                        tuVongMap[maBn] = list;
                    }
                    list.Add(ngayRa);
                }
            }

            foreach (SheetRow row in xml1Rows)
            {
                BhytBase rec = row.Record;
                string maBn = rec.GetValue("MA_BN") ?? "";
                string maLk = rec.GetValue("MA_LK") ?? "";
                string ngayVao = (rec.GetValue("NGAY_VAO") ?? "").Trim();
                string ketQua = (rec.GetValue("KET_QUA_DTRI") ?? "").Trim();

                List<string> chetList;
                if (maBn.Length == 0 || !tuVongMap.TryGetValue(maBn, out chetList)) continue;
                if (ngayVao.Length == 0 || ngayVao.Length < 8) continue;

                foreach (string ngayChet in chetList)
                {
                    // So sánh chuỗi yyyymmdd (thứ tự từ điển = thứ tự thời gian)
                    string ngayVaoD = ngayVao.Substring(0, 8);
                    string ngayChetD = ngayChet.Substring(0, 8);

                    if (string.CompareOrdinal(ngayVaoD, ngayChetD) > 0 && ketQua != "5")
                    {
                        errors.Add(new GiamDinhError
                        {
                            Sheet = "XML1",
                            MaLk = maLk,
                            RowExcel = row.RowExcel,
                            MaLyDo = "",
                            MoTa = $"BN '{maBn}': lần khám NGAY_VAO={ngayVao} " +
                                   $"xảy ra SAU ngày tử vong đã ghi nhận ({ngayChet}). " +
                                   "Đây là trường hợp KCB sau ngày chết. " +
                                   "Đề nghị kiểm tra và thu hồi chi phí, " +
                                   "xử phạt hành chính theo NĐ 117/2020/NĐ-CP.",
                            CanCu = CanCuChet,
                        });
                        break;  // chỉ báo 1 lần cho mỗi dòng
                    }
                }
            }

            return errors;
        }
    }
}
