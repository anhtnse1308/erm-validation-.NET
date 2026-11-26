using System.Collections.Generic;
using EmrValidation.Core.GiamDinh;
using EmrValidation.Core.Models;

namespace EmrValidation.Rules
{
    /// <summary>
    /// Rule II – Chỉ định thuốc không đúng quy định (S059.2, S079.1, S011.02)
    /// Căn cứ: TT 05/2015/TT-BYT (Danh mục thuốc YHCT); TT 30/2018/TT-BYT + TT 01/2020/TT-BYT
    /// (Kháng sinh đặc biệt, thuốc giới hạn chỉ định).
    ///   ThuocCciRules     – thuốc có chống chỉ định theo ICD (S011.02)
    ///   ThuocGioiHan      – thuốc chỉ thanh toán khi có ICD cụ thể (S059.2 / S006.119)
    ///   KhangSinhDacBiet  – kháng sinh chỉ dùng tại BV hạng cao (S079.1)
    /// Các bảng dữ liệu bên dưới được sinh trực tiếp từ bản Python để giữ nguyên từng ký tự.
    /// </summary>
    public class ThuocChiDinh : GiamDinhBase
    {
        private sealed class CciRule
        {
            public readonly string[] Icd; public readonly string Ten, MoTa, CanCu;
            public CciRule(string[] icd, string ten, string moTa, string canCu)
            { Icd = icd; Ten = ten; MoTa = moTa; CanCu = canCu; }
        }

        private sealed class GioiHanRule
        {
            public readonly string[] Allowed; public readonly string Ten, GioiHan, MaLyDo, CanCu;
            public GioiHanRule(string[] allowed, string ten, string gioiHan, string maLyDo, string canCu)
            { Allowed = allowed; Ten = ten; GioiHan = gioiHan; MaLyDo = maLyDo; CanCu = canCu; }
        }

        // S011.02 – Thuốc chống chỉ định theo ICD: ma_thuoc → (ICD bị CCI, tên thuốc, mô tả, căn cứ)
        private static readonly Dictionary<string, CciRule> ThuocCciRules = new Dictionary<string, CciRule>
        {
            { "VN-15960-12", new CciRule(new[] { "I20.0", "I21", "R57.0" },
                "Procoralan 5mg",
                "CCI: sốc tim (R57.0), NMCT cấp (I21), đau thắt ngực không ổn định (I20.0)",
                "TT 30/2018/TT-BYT khoản 2 Điều 3") },
            { "VN-21893-19", new CciRule(new[] { "I20.0", "I21", "R57.0" },
                "Procoralan 5mg",
                "CCI: sốc tim, NMCT cấp, đau thắt ngực không ổn định",
                "TT 30/2018/TT-BYT khoản 2 Điều 3") },
            { "VN-15961-12", new CciRule(new[] { "I20.0", "I21", "R57.0" },
                "Procoralan 7.5mg",
                "CCI: sốc tim, NMCT cấp, đau thắt ngực không ổn định",
                "TT 30/2018/TT-BYT khoản 2 Điều 3") },
            { "VN-21894-19", new CciRule(new[] { "I20.0", "I21", "R57.0" },
                "Procoralan 7.5mg",
                "CCI: sốc tim, NMCT cấp, đau thắt ngực không ổn định",
                "TT 30/2018/TT-BYT khoản 2 Điều 3") },
            { "VN-20575-17", new CciRule(new[] { "E10", "I50", "J44", "K72", "N18", "N19" },
                "Melanov-M",
                "CCI: ĐTĐ phụ thuộc insulin (E10), suy gan (K72), suy thận (N18/N19), COPD (J44), suy tim (I50)",
                "TT 30/2018/TT-BYT khoản 2 Điều 3") },
            { "VN-18273-14", new CciRule(new[] { "I50", "J45", "J46", "K72" },
                "Coryol 12.5mg",
                "CCI: suy tim (I50), suy gan (K72), hen phế quản (J45/J46)",
                "TT 30/2018/TT-BYT khoản 2 Điều 3") },
            { "VD-20671-14", new CciRule(new[] { "C00", "C01", "C02", "C03", "C04", "C05", "C06", "C07", "C08", "C09", "C10", "C11", "C12", "C13", "C14", "C15", "C16", "C17", "C18", "C19", "C20", "C21", "C22", "C23", "C24", "C25", "C26", "C30", "C31", "C32", "C33", "C34", "C37", "C38", "C39", "C40", "C41", "C43", "C44", "C45", "C46", "C47", "C48", "C49", "C50", "C51", "C52", "C53", "C54", "C55", "C56", "C57", "C58", "C60", "C61", "C62", "C63", "C64", "C65", "C66", "C67", "C68", "C69", "C70", "C71", "C72", "C73", "C74", "C75", "C76", "C77", "C78", "C79", "C80", "C81", "C82", "C83", "C84", "C85", "C88", "C90", "C91", "C92", "C93", "C94", "C95", "C96", "C97" },
                "Neutrivit (VD-20671-14)",
                "CCI: u ác tính (nhóm C00-C97)",
                "TT 30/2018/TT-BYT khoản 2 Điều 3") },
            { "VD-30494-18", new CciRule(new[] { "C00", "C16", "C18", "C34", "C50", "C53", "C61", "C64", "C71", "C80", "C81", "C91", "C92" },
                "Savi 3B (VD-30494-18)",
                "CCI: u ác tính",
                "TT 30/2018/TT-BYT khoản 2 Điều 3") },
            { "VD-20376-13", new CciRule(new[] { "K31.1", "K37", "K51", "K56", "N18.5", "N18.6", "O14" },
                "Gelactive Fort (VD-20376-13)",
                "CCI: suy thận nặng/TNT (N18.5/N18.6), tiền sản giật (O14), loét ruột kết (K51), viêm RT (K37), tắc ruột (K56), hẹp môn vị (K31.1). Lưu ý: cần kiểm tra thêm tuổi BN < 6 tuổi.",
                "TT 30/2018/TT-BYT khoản 2 Điều 3") },
            { "VD-32408-19", new CciRule(new[] { "K31.1", "K37", "K51", "K56", "N18.5", "N18.6", "O14" },
                "Gelactive Fort (VD-32408-19)",
                "CCI: suy thận nặng/TNT, tiền sản giật, loét ruột kết, viêm RT, tắc ruột, hẹp môn vị.",
                "TT 30/2018/TT-BYT khoản 2 Điều 3") },
            { "VD-24094-16", new CciRule(new[] { "E10", "E11", "E12", "E13", "E14" },
                "Phalintop (VD-24094-16)",
                "CCI: đái tháo đường (E10-E14)",
                "TT 30/2018/TT-BYT khoản 2 Điều 3") },
        };

        // S059.2 / S006.119 – Thuốc chỉ thanh toán khi có ICD cụ thể
        private static readonly Dictionary<string, GioiHanRule> ThuocGioiHan = new Dictionary<string, GioiHanRule>
        {
            { "05c.53", new GioiHanRule(new[] { "M16", "M17" },
                "Cao avocado/soya không xà phòng hóa (05c.53)",
                "Chỉ thanh toán điều trị thoái hóa khớp hông (M16) và khớp gối (M17)",
                "S059.2",
                "TT 05/2015/TT-BYT ngày 17/3/2015") },
            { "05c.103", new GioiHanRule(new[] { "K52" },
                "Ngưu nhĩ phong, La liễu (05c.103)",
                "Chỉ thanh toán điều trị viêm đại tràng mạn tính (K52)",
                "S059.2",
                "TT 05/2015/TT-BYT ngày 17/3/2015") },
            { "40.1042", new GioiHanRule(new[] { "E40", "E41", "E42", "E43", "E44", "E45", "E46" },
                "Lysin+Vitamin+Khoáng chất (40.1042)",
                "Chỉ thanh toán trẻ em dưới 6 tuổi suy dinh dưỡng (E40-E46). Cần kiểm tra thêm tuổi BN.",
                "S006.119",
                "TT 30/2018/TT-BYT cột 8 Phụ lục 01") },
        };

        // S079.1 – Kháng sinh đặc biệt (chỉ BV hạng đặc biệt / I / II / lao-phổi)
        private static readonly Dictionary<string, string> KhangSinhDacBiet = new Dictionary<string, string>
        {
            { "40.168", "Cefepim" },
            { "40.176", "Cefpirom" },
            { "40.183", "Ceftriaxon" },
            { "40.188", "Imipenem + cilastatin" },
            { "40.189", "Meropenem" },
            { "40.192", "Piperacillin + tazobactam" },
            { "40.196", "Ticarcillin + acid clavulanic" },
            { "40.258", "Vancomycin" },
        };

        // Whitelist mã CSKCB được phép dùng kháng sinh đặc biệt (cần bổ sung theo danh mục phân hạng BV của BYT)
        private static readonly HashSet<string> CskcbHangDuocPhep = new HashSet<string> { "01000", "01001", "01002", "01003", "75404", "75405" };

        // True = chỉ cảnh báo khi whitelist chưa đầy đủ
        private const bool KhangSinhWarnOnly = true;

        private const string CanCuKs = "TT 30/2018/TT-BYT + TT 01/2020/TT-BYT (khoản 3 Điều 4 + khoản 1 Điều 1)";

        private sealed class Xml1Info
        {
            public ICDHelper Icd;
            public string Cskcb;
        }

        public override List<GiamDinhError> Check(IDictionary<string, List<BhytBase>> allObjects)
        {
            var errors = new List<GiamDinhError>();

            // MA_LK → {icd, ma_cskcb}
            var xml1Map = new Dictionary<string, Xml1Info>();
            foreach (SheetRow row in GetRows(allObjects, "XML1"))
            {
                string maLk = row.Record.GetValue("MA_LK") ?? "";
                if (maLk.Length > 0)
                {
                    xml1Map[maLk] = new Xml1Info
                    {
                        Icd = ICDHelper.FromRecord(row.Record),
                        Cskcb = row.Record.GetValue("MA_CSKCB") ?? "",
                    };
                }
            }

            foreach (SheetRow row in GetRows(allObjects, "XML2"))
            {
                BhytBase rec = row.Record;
                string maLk = rec.GetValue("MA_LK") ?? "";
                string maThuoc = (rec.GetValue("MA_THUOC") ?? "").Trim();
                string soDangKy = (rec.GetValue("SO_DANG_KY") ?? "").Trim();
                if (maLk.Length == 0 || maThuoc.Length == 0) continue;

                Xml1Info info;
                ICDHelper icd;
                string cskcb;
                if (xml1Map.TryGetValue(maLk, out info)) { icd = info.Icd; cskcb = info.Cskcb; }
                else { icd = new ICDHelper("", ""); cskcb = ""; }

                // S011.02 – Thuốc CCI theo mã đăng ký (SO_DANG_KY)
                string lookupKey = ThuocCciRules.ContainsKey(soDangKy) ? soDangKy : maThuoc;
                CciRule cci;
                if (ThuocCciRules.TryGetValue(lookupKey, out cci))
                {
                    if (icd.HasAny(cci.Icd))
                    {
                        errors.Add(new GiamDinhError
                        {
                            Sheet = "XML2", MaLk = maLk, RowExcel = row.RowExcel,
                            MaLyDo = "S011.02",
                            MoTa = $"Thuốc '{cci.Ten}' ({lookupKey}) có chống chỉ định. " +
                                   $"{cci.MoTa}. {icd}",
                            CanCu = cci.CanCu,
                            MaDichVu = maThuoc,
                        });
                    }
                }

                // S059.2 / S006.119 – Thuốc ngoài giới hạn chỉ định
                GioiHanRule gh;
                if (ThuocGioiHan.TryGetValue(maThuoc, out gh))
                {
                    if (!icd.HasAny(gh.Allowed))
                    {
                        errors.Add(new GiamDinhError
                        {
                            Sheet = "XML2", MaLk = maLk, RowExcel = row.RowExcel,
                            MaLyDo = gh.MaLyDo,
                            MoTa = $"Thuốc '{gh.Ten}' chỉ định ngoài giới hạn. " +
                                   $"{gh.GioiHan}. {icd}",
                            CanCu = gh.CanCu,
                            MaDichVu = maThuoc,
                        });
                    }
                }

                // S079.1 – Kháng sinh đặc biệt sai hạng BV
                string tenKs;
                if (KhangSinhDacBiet.TryGetValue(maThuoc, out tenKs))
                {
                    bool isAllowed = CskcbHangDuocPhep.Count > 0 ? CskcbHangDuocPhep.Contains(cskcb) : true;

                    if (!isAllowed)
                    {
                        errors.Add(new GiamDinhError
                        {
                            Sheet = "XML2", MaLk = maLk, RowExcel = row.RowExcel,
                            MaLyDo = "S079.1",
                            MoTa = $"Kháng sinh đặc biệt '{tenKs}' (mã {maThuoc}) " +
                                   $"tại CSKCB '{cskcb}' không đúng hạng BV quy định. " +
                                   "Cần kiểm tra có dùng kèm DVKT được phê duyệt không.",
                            CanCu = CanCuKs,
                            MaDichVu = maThuoc,
                        });
                    }
                    else if (KhangSinhWarnOnly && CskcbHangDuocPhep.Count == 0)
                    {
                        errors.Add(new GiamDinhError
                        {
                            Sheet = "XML2", MaLk = maLk, RowExcel = row.RowExcel,
                            MaLyDo = "S079.1-WARN",
                            MoTa = $"[CẦN KIỂM TRA] Kháng sinh đặc biệt '{tenKs}' " +
                                   $"(mã {maThuoc}) tại CSKCB '{cskcb}'. " +
                                   "Xác minh hạng BV và điều kiện sử dụng.",
                            CanCu = CanCuKs,
                            MaDichVu = maThuoc,
                        });
                    }
                }
            }

            return errors;
        }
    }
}
