using System;
using System.Collections.Generic;
using EmrValidation.Core.GiamDinh;
using EmrValidation.Core.Helpers;
using EmrValidation.Core.Models;
using EmrValidation.Excel;

namespace EmrValidation.Rules
{
    /// <summary>
    /// Rule – Sàng lọc mã đối tượng KCB (MA_DOITUONG_KCB) tại XML1. Mọi trường dữ liệu đều từ XML1.
    ///   DT1 – MA_DOITUONG_KCB=1.14 → MA_LOAI_KCB phải thuộc {01,02,05,08}   → WARN.DT01
    ///   DT2 – MA_DOITUONG_KCB=1.15 → MA_LOAI_KCB phải thuộc {03,09}         → WARN.DT02
    ///   DT3 – MA_DKBD=74021        → MA_DOITUONG_KCB phải thuộc {1.1, 2}    → WARN.DT03
    ///   DT4 – MA_DKBD≠74021        → MA_DOITUONG_KCB phải thuộc {1.14,1.15,1.3}
    ///         (1.3 yêu cầu có MA_NOI_DI)                                     → WARN.DT04
    ///   DT5 – MA_DOITUONG_KCB=1.7  → |NGAY_VAO.date - NGAY_SINH.date| ≤ 5  → WARN.DT05
    /// </summary>
    public class SangLocDoiTuong : GiamDinhBase
    {
        private const string CanCu = "Thông tư 30/2020/TT-BYT + QĐ 4210/QĐ-BYT (Mã đối tượng KCB BHYT)";

        private static readonly HashSet<string> LoaiKcbNgoaiTru = new HashSet<string> { "01", "02", "05", "08" };
        private static readonly HashSet<string> LoaiKcbNoiTru = new HashSet<string> { "03", "09" };

        private const string DkbdDungTuyen = "74021";
        private static readonly HashSet<string> DoiTuongDungTuyen = new HashSet<string> { "1.1", "2" };
        private static readonly HashSet<string> DoiTuongTraiTuyen = new HashSet<string> { "1.14", "1.15", "1.3", "2" };

        private const string DoiTuongSoSinh = "1.7";
        private const int SoSinhMaxNgay = 5;

        private static DateTime? ParseDate(string val)
        {
            string s = (val ?? "").Trim();
            if (s.Length > 8) s = s.Substring(0, 8);
            if (s.Length == 8 && ParseHelper.IsDigits(s))
                return ParseHelper.ParseDate8(s);
            return null;
        }

        private static string Norm(BhytBase rec, string field)
        {
            return (rec.GetValue(field) ?? "").Trim();
        }

        private static string Left8(string s)
        {
            return s.Length > 8 ? s.Substring(0, 8) : s;
        }

        public override List<GiamDinhError> Check(IDictionary<string, List<BhytBase>> allObjects)
        {
            var errors = new List<GiamDinhError>();

            foreach (SheetRow row in GetRows(allObjects, "XML1"))
            {
                BhytBase rec = row.Record;
                string maLk = Norm(rec, "MA_LK");
                string maDkbd = Norm(rec, "MA_DKBD");
                string maNoiDi = Norm(rec, "MA_NOI_DI");
                string ngaySinh = Norm(rec, "NGAY_SINH");
                string ngayVao = Norm(rec, "NGAY_VAO");

                // MA_DOITUONG_KCB và MA_LOAI_KCB có thể có nhiều mã phân cách ";" (VD loại "01;03" ↔ đối tượng "1.14;1.15").
                // Ghép theo đúng vị trí: mã đối tượng thứ i ứng với mã loại KCB thứ i.
                string[] doiTuongs = ParseHelper.SplitList(Norm(rec, "MA_DOITUONG_KCB"));
                string[] loaiKcbs = ParseHelper.SplitList(Norm(rec, "MA_LOAI_KCB"));

                if (maLk.Length == 0 || doiTuongs.Length == 0) continue;

                for (int i = 0; i < doiTuongs.Length; i++)
                {
                    string doiTuong = doiTuongs[i];
                    string loaiKcb = i < loaiKcbs.Length ? loaiKcbs[i] : "";
                    string viTri = doiTuongs.Length > 1 ? $" [vị trí {i + 1}/{doiTuongs.Length}]" : "";

                    if (doiTuong.Length == 0) continue;

                    // DT1 – 1.14 → ngoại trú (01,02,05,08)
                    if (doiTuong == "1.14" && !LoaiKcbNgoaiTru.Contains(loaiKcb))
                    {
                        errors.Add(new GiamDinhError
                        {
                            Sheet = "XML1",
                            MaLk = maLk,
                            RowExcel = row.RowExcel,
                            MaLyDo = "WARN.DT01",
                            MoTa = "MA_DOITUONG_KCB=1.14 (trái tuyến ngoại trú)" + viTri + " " +
                                   $"nhưng MA_LOAI_KCB='{loaiKcb}' không hợp lệ. " +
                                   "Phải thuộc {01,02,05,08}.",
                            CanCu = CanCu,
                        });
                    }

                    // DT2 – 1.15 → nội trú (03,09)
                    if (doiTuong == "1.15" && !LoaiKcbNoiTru.Contains(loaiKcb))
                    {
                        errors.Add(new GiamDinhError
                        {
                            Sheet = "XML1",
                            MaLk = maLk,
                            RowExcel = row.RowExcel,
                            MaLyDo = "WARN.DT02",
                            MoTa = "MA_DOITUONG_KCB=1.15 (chuyển từ 1.14 trái tuyến sang nội trú)" + viTri + " " +
                                   $"nhưng MA_LOAI_KCB='{loaiKcb}' không hợp lệ. " +
                                   "Phải thuộc {03,09}.",
                            CanCu = CanCu,
                        });
                    }

                    // DT3 – MA_DKBD=74021 → đối tượng phải là 1.1 hoặc 2
                    if (maDkbd == DkbdDungTuyen && !DoiTuongDungTuyen.Contains(doiTuong))
                    {
                        errors.Add(new GiamDinhError
                        {
                            Sheet = "XML1",
                            MaLk = maLk,
                            RowExcel = row.RowExcel,
                            MaLyDo = "WARN.DT03",
                            MoTa = $"MA_DKBD='{maDkbd}' (đúng tuyến CSKCB đăng ký) " +
                                   $"nhưng MA_DOITUONG_KCB='{doiTuong}'{viTri} không hợp lệ. " +
                                   "Phải là 1.1 hoặc 2.",
                            CanCu = CanCu,
                        });
                    }

                    // DT4 – MA_DKBD≠74021 → đối tượng phải là 1.14, 1.15, hoặc 1.3 (1.3 thì phải có MA_NOI_DI)
                    if (maDkbd.Length > 0 && maDkbd != DkbdDungTuyen)
                    {
                        if (!DoiTuongTraiTuyen.Contains(doiTuong))
                        {
                            errors.Add(new GiamDinhError
                            {
                                Sheet = "XML1",
                                MaLk = maLk,
                                RowExcel = row.RowExcel,
                                MaLyDo = "WARN.DT04",
                                MoTa = $"MA_DKBD='{maDkbd}' (khác nơi đăng ký) " +
                                       $"nhưng MA_DOITUONG_KCB='{doiTuong}'{viTri} không hợp lệ. " +
                                       "Phải là 1.14 (ngoại trú), 1.15 (nội trú) " +
                                       "hoặc 1.3 (có giấy chuyển tuyến).",
                                CanCu = CanCu,
                            });
                        }
                        else if (doiTuong == "1.3" && maNoiDi.Length == 0)
                        {
                            errors.Add(new GiamDinhError
                            {
                                Sheet = "XML1",
                                MaLk = maLk,
                                RowExcel = row.RowExcel,
                                MaLyDo = "WARN.DT04",
                                MoTa = "MA_DOITUONG_KCB=1.3 (có giấy chuyển tuyến)" + viTri + " " +
                                       "nhưng MA_NOI_DI để trống. " +
                                       "Phải ghi MA_NOI_DI tức nơi chuyển tuyến.",
                                CanCu = CanCu,
                            });
                        }
                    }

                    // DT5 – 1.7 (sơ sinh) → NGAY_VAO - NGAY_SINH ≤ 5 ngày
                    if (doiTuong == DoiTuongSoSinh)
                    {
                        DateTime? dSinh = ParseDate(ngaySinh);
                        DateTime? dVao = ParseDate(ngayVao);
                        if (dSinh != null && dVao != null)
                        {
                            int delta = (int)Math.Abs((dVao.Value - dSinh.Value).TotalDays);
                            if (delta > SoSinhMaxNgay)
                            {
                                errors.Add(new GiamDinhError
                                {
                                    Sheet = "XML1",
                                    MaLk = maLk,
                                    RowExcel = row.RowExcel,
                                    MaLyDo = "WARN.DT05",
                                    MoTa = "MA_DOITUONG_KCB=1.7 (trẻ sơ sinh)" + viTri + " " +
                                           $"nhưng NGAY_VAO ({Left8(ngayVao)}) cách " +
                                           $"NGAY_SINH ({Left8(ngaySinh)}) {delta} ngày " +
                                           $"(> {SoSinhMaxNgay} ngày cho phép).",
                                    CanCu = CanCu,
                                });
                            }
                        }
                        else
                        {
                            errors.Add(new GiamDinhError
                            {
                                Sheet = "XML1",
                                MaLk = maLk,
                                RowExcel = row.RowExcel,
                                MaLyDo = "WARN.DT05",
                                MoTa = "MA_DOITUONG_KCB=1.7 (trẻ sơ sinh)" + viTri + " " +
                                       "nhưng không thể xác định ngày: " +
                                       $"NGAY_SINH='{ngaySinh}', NGAY_VAO='{ngayVao}'.",
                                CanCu = CanCu,
                            });
                        }
                    }
                }
            }

            return errors;
        }

        /// <summary>
        /// Xuất kết quả sàng lọc đối tượng KCB: &lt;ten_goc&gt;[&lt;timestamp&gt;]_sanglocdoituong.xlsx
        /// Sheet TONG_HOP, CHI_TIET và mỗi mã WARN_DT01..DT05 một sheet.
        /// </summary>
        public static string WriteExcel(string excelFile, IList<GiamDinhError> errors)
        {
            return SangLocDoiTuongExcelWriter.Write(excelFile, errors);
        }
    }
}