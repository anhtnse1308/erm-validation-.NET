using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using EmrValidation.Core.Helpers;

namespace EmrValidation.Core.Models
{
    /// <summary>Bảng 8. Chỉ tiêu dữ liệu tóm tắt hồ sơ bệnh án.</summary>
    public class Bang8_TomTatHSBA : BhytBase
    {
        public string MA_LK { get; set; }
        public string MA_LOAI_KCB { get; set; }
        public string HO_TEN_CHA { get; set; }
        public string HO_TEN_ME { get; set; }
        public string NGUOI_GIAM_HO { get; set; }
        public string DON_VI { get; set; }
        public string NGAY_VAO { get; set; }
        public string NGAY_RA { get; set; }
        public string CHAN_DOAN_VAO { get; set; }
        public string CHAN_DOAN_RV { get; set; }
        public string QT_BENHLY { get; set; }
        public string TOMTAT_KQ { get; set; }
        public string PP_DIEUTRI { get; set; }
        public string NGAY_SINHCON { get; set; }
        public string NGAY_CONCHET { get; set; }
        public string SO_CONCHET { get; set; }
        public string KET_QUA_DTRI { get; set; }
        public string GHI_CHU { get; set; }
        public string MA_TTDV { get; set; }
        public string NGAY_CT { get; set; }
        public string MA_THE_TAM { get; set; }
        public string DU_PHONG { get; set; }

        // ---------------- Metadata ----------------
        private static readonly string[] _fieldNames =
        {
            "MA_LK", "MA_LOAI_KCB", "HO_TEN_CHA", "HO_TEN_ME", "NGUOI_GIAM_HO", "DON_VI",
            "NGAY_VAO", "NGAY_RA", "CHAN_DOAN_VAO", "CHAN_DOAN_RV", "QT_BENHLY", "TOMTAT_KQ",
            "PP_DIEUTRI", "NGAY_SINHCON", "NGAY_CONCHET", "SO_CONCHET", "KET_QUA_DTRI", "GHI_CHU",
            "MA_TTDV", "NGAY_CT", "MA_THE_TAM", "DU_PHONG",
        };

        private static readonly KeyValuePair<string, int>[] _maxLen =
        {
            new KeyValuePair<string, int>("MA_LK", 100),
            new KeyValuePair<string, int>("MA_LOAI_KCB", 10),
            new KeyValuePair<string, int>("HO_TEN_CHA", 255),
            new KeyValuePair<string, int>("HO_TEN_ME", 255),
            new KeyValuePair<string, int>("NGUOI_GIAM_HO", 255),
            new KeyValuePair<string, int>("DON_VI", 1024),
            new KeyValuePair<string, int>("NGAY_VAO", 12),
            new KeyValuePair<string, int>("NGAY_RA", 12),
            new KeyValuePair<string, int>("NGAY_SINHCON", 8),
            new KeyValuePair<string, int>("NGAY_CONCHET", 8),
            new KeyValuePair<string, int>("SO_CONCHET", 2),
            new KeyValuePair<string, int>("KET_QUA_DTRI", 1),
            new KeyValuePair<string, int>("MA_TTDV", 255),
            new KeyValuePair<string, int>("NGAY_CT", 8),
        };

        private static readonly string[] _numeric =
        {
            "SO_CONCHET", "KET_QUA_DTRI",
        };

        private static readonly string[] _date12 =
        {
            "NGAY_VAO", "NGAY_RA",
        };

        private static readonly string[] _date8 =
        {
            "NGAY_SINHCON", "NGAY_CONCHET", "NGAY_CT",
        };

        private static readonly string[] _required = new string[0];


        protected override string[] FieldNamesCore { get { return _fieldNames; } }
        protected override KeyValuePair<string, int>[] MaxLenCore { get { return _maxLen; } }
        protected override string[] NumericCore { get { return _numeric; } }
        protected override string[] Date12Core { get { return _date12; } }
        protected override string[] Date8Core { get { return _date8; } }
        protected override string[] RequiredCore { get { return _required; } }

        public override string GetValue(string fieldName)
        {
            switch (fieldName)
            {
                case "MA_LK": return MA_LK;
                case "MA_LOAI_KCB": return MA_LOAI_KCB;
                case "HO_TEN_CHA": return HO_TEN_CHA;
                case "HO_TEN_ME": return HO_TEN_ME;
                case "NGUOI_GIAM_HO": return NGUOI_GIAM_HO;
                case "DON_VI": return DON_VI;
                case "NGAY_VAO": return NGAY_VAO;
                case "NGAY_RA": return NGAY_RA;
                case "CHAN_DOAN_VAO": return CHAN_DOAN_VAO;
                case "CHAN_DOAN_RV": return CHAN_DOAN_RV;
                case "QT_BENHLY": return QT_BENHLY;
                case "TOMTAT_KQ": return TOMTAT_KQ;
                case "PP_DIEUTRI": return PP_DIEUTRI;
                case "NGAY_SINHCON": return NGAY_SINHCON;
                case "NGAY_CONCHET": return NGAY_CONCHET;
                case "SO_CONCHET": return SO_CONCHET;
                case "KET_QUA_DTRI": return KET_QUA_DTRI;
                case "GHI_CHU": return GHI_CHU;
                case "MA_TTDV": return MA_TTDV;
                case "NGAY_CT": return NGAY_CT;
                case "MA_THE_TAM": return MA_THE_TAM;
                case "DU_PHONG": return DU_PHONG;
                default: return null;
            }
        }

        public override bool SetValue(string fieldName, string value)
        {
            switch (fieldName)
            {
                case "MA_LK": MA_LK = value; return true;
                case "MA_LOAI_KCB": MA_LOAI_KCB = value; return true;
                case "HO_TEN_CHA": HO_TEN_CHA = value; return true;
                case "HO_TEN_ME": HO_TEN_ME = value; return true;
                case "NGUOI_GIAM_HO": NGUOI_GIAM_HO = value; return true;
                case "DON_VI": DON_VI = value; return true;
                case "NGAY_VAO": NGAY_VAO = value; return true;
                case "NGAY_RA": NGAY_RA = value; return true;
                case "CHAN_DOAN_VAO": CHAN_DOAN_VAO = value; return true;
                case "CHAN_DOAN_RV": CHAN_DOAN_RV = value; return true;
                case "QT_BENHLY": QT_BENHLY = value; return true;
                case "TOMTAT_KQ": TOMTAT_KQ = value; return true;
                case "PP_DIEUTRI": PP_DIEUTRI = value; return true;
                case "NGAY_SINHCON": NGAY_SINHCON = value; return true;
                case "NGAY_CONCHET": NGAY_CONCHET = value; return true;
                case "SO_CONCHET": SO_CONCHET = value; return true;
                case "KET_QUA_DTRI": KET_QUA_DTRI = value; return true;
                case "GHI_CHU": GHI_CHU = value; return true;
                case "MA_TTDV": MA_TTDV = value; return true;
                case "NGAY_CT": NGAY_CT = value; return true;
                case "MA_THE_TAM": MA_THE_TAM = value; return true;
                case "DU_PHONG": DU_PHONG = value; return true;
                default: return false;
            }
        }

        private static string Left8(string s)
        {
            s = s ?? "None";
            return s.Length > 8 ? s.Substring(0, 8) : s;
        }

        // Lưu ý (giữ nguyên bản Python): validate() của Bảng 8 KHÔNG gọi base.Validate().
        public override List<string> Validate()
        {
            var errs = new List<string>();

            // 1. MA_LK – bắt buộc
            if (string.IsNullOrEmpty(MA_LK))
                errs.Add("MA_LK: bắt buộc, không được để trống");

            // 2. MA_LOAI_KCB – nhiều mã phân cách ";" (VD 01;03); mỗi mã thuộc enum 01,02,03,04,05,06,08,09;
            //    Bảng 8 chỉ gửi khi có ít nhất một mã ∈ {03, 04, 06}
            if (!string.IsNullOrEmpty(MA_LOAI_KCB))
            {
                bool loaiSai = false, coNoiTru = false;
                foreach (string item in ParseHelper.SplitList(MA_LOAI_KCB))
                {
                    string loai = item.PadLeft(2, '0');
                    if (!In(loai, "01", "02", "03", "04", "05", "06", "08", "09"))
                        loaiSai = true;
                    else if (In(loai, "03", "04", "06"))
                        coNoiTru = true;
                }
                if (loaiSai)
                    errs.Add($"MA_LOAI_KCB: '{MA_LOAI_KCB}' không hợp lệ. " +
                             "Chỉ chấp nhận ['01', '02', '03', '04', '05', '06', '08', '09'], nhiều mã cách nhau ';'");
                else if (!coNoiTru)
                    errs.Add($"MA_LOAI_KCB: '{MA_LOAI_KCB}' – Bảng 8 chỉ gửi khi " +
                             "MA_LOAI_KCB ∈ {03 (nội trú), 04 (nội trú ban ngày), 06 (lưu TYT/PKĐKKV)}");
            }

            // 7-8. NGAY_VAO, NGAY_RA – parse + NGAY_RA >= NGAY_VAO
            DateTime? dtVao = ParseDate12(NGAY_VAO);
            DateTime? dtRa = ParseDate12(NGAY_RA);

            if (!string.IsNullOrEmpty(NGAY_VAO) && dtVao == null)
                errs.Add($"NGAY_VAO: '{NGAY_VAO}' không phải ngày giờ hợp lệ " +
                         "(định dạng yyyymmddHHMM)");
            if (!string.IsNullOrEmpty(NGAY_RA) && dtRa == null)
                errs.Add($"NGAY_RA: '{NGAY_RA}' không phải ngày giờ hợp lệ " +
                         "(định dạng yyyymmddHHMM)");
            if (dtVao != null && dtRa != null && dtRa.Value < dtVao.Value)
                errs.Add($"NGAY_RA ({NGAY_RA}) không được trước NGAY_VAO ({NGAY_VAO})");

            // 17. KET_QUA_DTRI – enum {1..8}
            long? ketQua = ToInt(KET_QUA_DTRI);
            if (KET_QUA_DTRI != null && ketQua != null)
            {
                if (!(1 <= ketQua && ketQua <= 8))
                    errs.Add($"KET_QUA_DTRI: '{KET_QUA_DTRI}' không hợp lệ. " +
                             "Chỉ chấp nhận 1(Khỏi) 2(Đỡ) 3(Không đổi) 4(Nặng hơn) " +
                             "5(Tử vong tại KBCB) 6(Tiên lượng nặng xin về) " +
                             "7(Chưa xác định) 8(Tử vong ngoại viện)");
            }

            // Nhóm con chết (14–16): NGAY_SINHCON, NGAY_CONCHET, SO_CONCHET
            DateTime? dtSinh = ParseDate8(NGAY_SINHCON);
            DateTime? dtChet = ParseDate8(NGAY_CONCHET);
            long? soCon = ToInt(SO_CONCHET);

            bool hasSinh = !string.IsNullOrEmpty(NGAY_SINHCON);
            bool hasChet = !string.IsNullOrEmpty(NGAY_CONCHET);
            bool hasSo = !string.IsNullOrEmpty(SO_CONCHET);

            if (!string.IsNullOrEmpty(NGAY_SINHCON) && dtSinh == null)
                errs.Add($"NGAY_SINHCON: '{NGAY_SINHCON}' không phải ngày hợp lệ " +
                         "(định dạng yyyymmdd)");
            if (!string.IsNullOrEmpty(NGAY_CONCHET) && dtChet == null)
                errs.Add($"NGAY_CONCHET: '{NGAY_CONCHET}' không phải ngày hợp lệ " +
                         "(định dạng yyyymmdd)");

            // Nếu bất kỳ field nào có → cả nhóm phải đủ
            if (hasSinh || hasChet || hasSo)
            {
                if ((!hasSinh && hasChet) || (!hasSinh && hasSo))
                    errs.Add("NGAY_SINHCON: bắt buộc khi có thông tin con chết " +
                             "(NGAY_CONCHET hoặc SO_CONCHET đã được ghi)");
                if (hasSo && !hasChet)
                    errs.Add("NGAY_CONCHET: bắt buộc khi có thông tin con chết " +
                             "(NGAY_SINHCON và SO_CONCHET đã được ghi)");
                if (hasChet && !hasSo)
                    errs.Add("SO_CONCHET: bắt buộc khi có thông tin con chết " +
                             "(NGAY_SINHCON và NGAY_CONCHET đã được ghi)");
            }

            // NGAY_CONCHET >= NGAY_SINHCON
            if (dtSinh != null && dtChet != null && dtChet.Value < dtSinh.Value)
                errs.Add($"NGAY_CONCHET ({NGAY_CONCHET}) không được trước " +
                         $"NGAY_SINHCON ({NGAY_SINHCON})");

            // SO_CONCHET >= 1
            if (hasSo && soCon != null && soCon < 1)
                errs.Add($"SO_CONCHET: '{SO_CONCHET}' phải >= 1");

            // NGAY_SINHCON phải trong khoảng NGAY_VAO..NGAY_RA
            if (dtSinh != null && dtVao != null && dtSinh.Value.Date < dtVao.Value.Date)
                errs.Add($"NGAY_SINHCON ({NGAY_SINHCON}) không được trước " +
                         $"NGAY_VAO ({Left8(NGAY_VAO)})");
            if (dtSinh != null && dtRa != null && dtSinh.Value.Date > dtRa.Value.Date)
                errs.Add($"NGAY_SINHCON ({NGAY_SINHCON}) không được sau " +
                         $"NGAY_RA ({Left8(NGAY_RA)})");

            // 20. NGAY_CT – DATE8, >= NGAY_RA (ngày)
            DateTime? dtCt = ParseDate8(NGAY_CT);
            if (!string.IsNullOrEmpty(NGAY_CT) && dtCt == null)
                errs.Add($"NGAY_CT: '{NGAY_CT}' không phải ngày hợp lệ " +
                         "(định dạng yyyymmdd)");
            if (dtCt != null && dtRa != null && dtCt.Value.Date < dtRa.Value.Date)
                errs.Add($"NGAY_CT ({NGAY_CT}) không được trước ngày ra viện " +
                         $"NGAY_RA ({Left8(NGAY_RA)})");

            return errs;
        }
    }
}