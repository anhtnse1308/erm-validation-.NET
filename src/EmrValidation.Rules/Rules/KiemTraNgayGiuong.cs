using System;
using System.Collections.Generic;
using EmrValidation.Core.GiamDinh;
using EmrValidation.Core.Helpers;
using EmrValidation.Core.Models;
using EmrValidation.Excel;

namespace EmrValidation.Rules
{
    /// <summary>
    /// Rule – Kiểm tra ngày giường bệnh nhân nội trú (MA_LOAI_KCB = 03)
    /// Căn cứ: TT 39/2018/TT-BYT + TT 13/2019/TT-BYT
    ///
    /// Detect từ XML1 + XML3:
    ///   G1 – Số dịch vụ giường ≠ số ngày điều trị          → WARN.GG01
    ///   G2 – Cùng MA_LK, cùng ngày có &gt; 1 dịch vụ giường → WARN.GG02
    ///
    /// "Dịch vụ giường" = dòng XML3 có TEN_DICH_VU bắt đầu bằng "giường" (không phân biệt hoa/thường).
    /// Số ngày điều trị = (ngày NGAY_RA - ngày NGAY_VAO).Days. Chỉ áp dụng MA_LOAI_KCB = "03".
    /// </summary>
    public class KiemTraNgayGiuong : GiamDinhBase
    {
        private const string MaLoaiKcbNoiTru = "03";
        private const string CanCu = "TT 39/2018/TT-BYT + TT 13/2019/TT-BYT (giá dịch vụ ngày giường)";

        /// <summary>Lấy phần ngày từ DATE12 hoặc DATE8.</summary>
        private static DateTime? ParseDate(string val)
        {
            string s = (val ?? "").Trim();
            if (s.Length > 8) s = s.Substring(0, 8);
            if (s.Length == 8 && ParseHelper.IsDigits(s))
                return ParseHelper.ParseDate8(s);
            return null;
        }

        /// <summary>Số ngày điều trị = NGAY_RA - NGAY_VAO.</summary>
        private static int? SoNgay(string ngayVao, string ngayRa)
        {
            DateTime? dVao = ParseDate(ngayVao);
            DateTime? dRa = ParseDate(ngayRa);
            if (dVao != null && dRa != null && dRa.Value >= dVao.Value)
                return (int)(dRa.Value - dVao.Value).TotalDays;
            return null;
        }

        private static bool IsGiuong(string ten)
        {
            return (ten ?? "").Trim().ToLowerInvariant().StartsWith("giường", StringComparison.Ordinal);
        }

        private class Xml1Info
        {
            public int RowExcel;
            public string NgayVao, NgayRa;
        }

        private class GiuongRow
        {
            public int RowExcel;
            public string Ngay, Ten;
        }

        public override List<GiamDinhError> Check(IDictionary<string, List<BhytBase>> allObjects)
        {
            var errors = new List<GiamDinhError>();

            // XML1: chỉ lấy BN nội trú MA_LOAI_KCB=03
            var xml1Map = new Dictionary<string, Xml1Info>();
            foreach (SheetRow row in GetRows(allObjects, "XML1"))
            {
                string maLk = (row.Record.GetValue("MA_LK") ?? "").Trim();
                string loai = (row.Record.GetValue("MA_LOAI_KCB") ?? "").Trim();
                if (maLk.Length == 0 || Array.IndexOf(ParseHelper.SplitList(loai), MaLoaiKcbNoiTru) < 0) continue;
                xml1Map[maLk] = new Xml1Info
                {
                    RowExcel = row.RowExcel,
                    NgayVao = (row.Record.GetValue("NGAY_VAO") ?? "").Trim(),
                    NgayRa = (row.Record.GetValue("NGAY_RA") ?? "").Trim(),
                };
            }

            if (xml1Map.Count == 0) return errors;

            // XML3: thu thập dịch vụ giường theo MA_LK
            var giuongRows = new Dictionary<string, List<GiuongRow>>();
            foreach (SheetRow row in GetRows(allObjects, "XML3"))
            {
                string maLk = (row.Record.GetValue("MA_LK") ?? "").Trim();
                string tenDichVu = (row.Record.GetValue("TEN_DICH_VU") ?? "").Trim();
                string ngayYl = (row.Record.GetValue("NGAY_YL") ?? "").Trim();
                if (maLk.Length == 0 || !xml1Map.ContainsKey(maLk)) continue;
                if (!IsGiuong(tenDichVu)) continue;

                string ngayStr = ngayYl.Length > 0 ? (ngayYl.Length > 8 ? ngayYl.Substring(0, 8) : ngayYl) : "";
                List<GiuongRow> list;
                if (!giuongRows.TryGetValue(maLk, out list))
                {
                    list = new List<GiuongRow>();
                    giuongRows[maLk] = list;
                }
                list.Add(new GiuongRow { RowExcel = row.RowExcel, Ngay = ngayStr, Ten = tenDichVu });
            }

            // Kiểm tra
            foreach (KeyValuePair<string, Xml1Info> kv in xml1Map)
            {
                string maLk = kv.Key;
                Xml1Info info = kv.Value;
                string ngayVao = info.NgayVao;
                string ngayRa = info.NgayRa;
                int? soNgay = SoNgay(ngayVao, ngayRa);
                List<GiuongRow> rowsG;
                if (!giuongRows.TryGetValue(maLk, out rowsG)) rowsG = new List<GiuongRow>();
                int dem = rowsG.Count;

                // G1
                if (soNgay != null && dem != soNgay.Value)
                {
                    errors.Add(new GiamDinhError
                    {
                        Sheet = "XML1",
                        MaLk = maLk,
                        RowExcel = info.RowExcel,
                        MaLyDo = "WARN.GG01",
                        MoTa = $"Số dịch vụ giường ({dem}) " +
                               $"{(dem < soNgay.Value ? "ít hơn" : "nhiều hơn")} " +
                               $"số ngày điều trị ({soNgay.Value}). " +
                               $"NGAY_VAO={ngayVao} → NGAY_RA={ngayRa} " +
                               $"(= {soNgay.Value} ngày).",
                        CanCu = CanCu,
                    });
                }
                else if (soNgay == null && dem == 0)
                {
                    errors.Add(new GiamDinhError
                    {
                        Sheet = "XML1",
                        MaLk = maLk,
                        RowExcel = info.RowExcel,
                        MaLyDo = "WARN.GG01",
                        MoTa = "Không tìm thấy dịch vụ giường nào trong XML3. " +
                               $"NGAY_VAO={ngayVao} → NGAY_RA={ngayRa}.",
                        CanCu = CanCu,
                    });
                }

                // G2
                var ngayGroups = new Dictionary<string, List<GiuongRow>>();
                foreach (GiuongRow g in rowsG)
                {
                    List<GiuongRow> bucket;
                    if (!ngayGroups.TryGetValue(g.Ngay, out bucket))
                    {
                        bucket = new List<GiuongRow>();
                        ngayGroups[g.Ngay] = bucket;
                    }
                    bucket.Add(g);
                }

                foreach (KeyValuePair<string, List<GiuongRow>> ng in ngayGroups)
                {
                    List<GiuongRow> rowsNgay = ng.Value;
                    if (rowsNgay.Count > 1)
                    {
                        var tenList = new List<string>();
                        foreach (GiuongRow g in rowsNgay) tenList.Add(g.Ten);
                        foreach (GiuongRow g in rowsNgay)
                        {
                            errors.Add(new GiamDinhError
                            {
                                Sheet = "XML3",
                                MaLk = maLk,
                                RowExcel = g.RowExcel,
                                MaLyDo = "WARN.GG02",
                                MoTa = $"Ngày {ng.Key}: {rowsNgay.Count} dịch vụ giường " +
                                       "cho cùng BN — mỗi ngày chỉ được 1 dịch vụ giường. " +
                                       $"Các dịch vụ: {string.Join("; ", tenList)}.",
                                CanCu = CanCu,
                                MaDichVu = g.Ten,
                            });
                        }
                    }
                }
            }

            return errors;
        }

        /// <summary>
        /// Xuất file Excel kết quả kiểm tra ngày giường: &lt;ten_goc&gt;[&lt;timestamp&gt;]_ngaygiuong.xlsx
        /// Sheet TONG_HOP, CHI_TIET, WARN_GG01_SoNgay, WARN_GG02_TrungNgay.
        /// </summary>
        public static string WriteExcel(string excelFile, IList<GiamDinhError> errors)
        {
            return NgayGiuongExcelWriter.Write(excelFile, errors);
        }
    }
}
