using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using EmrValidation.Core.Helpers;

namespace EmrValidation.Core.Models
{
    /// <summary>Bảng 7. Chỉ tiêu dữ liệu giấy ra viện.</summary>
    public class Bang7_GiayRaVien : BhytBase
    {
        public string MA_LK { get; set; }
        public string SO_LUU_TRU { get; set; }
        public string MA_YTE { get; set; }
        public string MA_KHOA_RV { get; set; }
        public string NGAY_VAO { get; set; }
        public string NGAY_VAO_NOI_TRU { get; set; }
        public string NGAY_RA { get; set; }
        public string MA_DINH_CHI_THAI { get; set; }
        public string NGUYENNHAN_DINHCHI { get; set; }
        public string THOIGIAN_DINHCHI { get; set; }
        public string TUOI_THAI { get; set; }
        public string CHAN_DOAN_RV { get; set; }
        public string PP_DIEUTRI { get; set; }
        public string GHI_CHU { get; set; }
        public string MA_TTDV { get; set; }
        public string MA_BS { get; set; }
        public string TEN_BS { get; set; }
        public string NGAY_CT { get; set; }
        public string MA_CHA { get; set; }
        public string MA_ME { get; set; }
        public string MA_THE_TAM { get; set; }
        public string HO_TEN_CHA { get; set; }
        public string HO_TEN_ME { get; set; }
        public string SO_NGAY_NGHI { get; set; }
        public string NGOAITRU_TUNGAY { get; set; }
        public string NGOAITRU_DENNGAY { get; set; }
        public string DU_PHONG { get; set; }

        // ---------------- Metadata ----------------
        private static readonly string[] _fieldNames =
        {
            "MA_LK", "SO_LUU_TRU", "MA_YTE", "MA_KHOA_RV", "NGAY_VAO", "NGAY_VAO_NOI_TRU", "NGAY_RA",
            "MA_DINH_CHI_THAI", "NGUYENNHAN_DINHCHI", "THOIGIAN_DINHCHI", "TUOI_THAI", "CHAN_DOAN_RV", "PP_DIEUTRI",
            "GHI_CHU", "MA_TTDV", "MA_BS", "TEN_BS", "NGAY_CT", "MA_CHA",
            "MA_ME", "MA_THE_TAM", "HO_TEN_CHA", "HO_TEN_ME", "SO_NGAY_NGHI", "NGOAITRU_TUNGAY",
            "NGOAITRU_DENNGAY", "DU_PHONG",
        };

        private static readonly KeyValuePair<string, int>[] _maxLen =
        {
            new KeyValuePair<string, int>("MA_LK", 100),
            new KeyValuePair<string, int>("SO_LUU_TRU", 200),
            new KeyValuePair<string, int>("MA_YTE", 200),
            new KeyValuePair<string, int>("MA_KHOA_RV", 200),
            new KeyValuePair<string, int>("NGAY_VAO", 12),
            new KeyValuePair<string, int>("NGAY_VAO_NOI_TRU", 12),
            new KeyValuePair<string, int>("NGAY_RA", 12),
            new KeyValuePair<string, int>("MA_DINH_CHI_THAI", 1),
            new KeyValuePair<string, int>("THOIGIAN_DINHCHI", 12),
            new KeyValuePair<string, int>("TUOI_THAI", 2),
            new KeyValuePair<string, int>("CHAN_DOAN_RV", 1500),
            new KeyValuePair<string, int>("GHI_CHU", 1500),
            new KeyValuePair<string, int>("MA_TTDV", 255),
            new KeyValuePair<string, int>("MA_BS", 255),
            new KeyValuePair<string, int>("TEN_BS", 255),
            new KeyValuePair<string, int>("NGAY_CT", 8),
            new KeyValuePair<string, int>("MA_CHA", 10),
            new KeyValuePair<string, int>("MA_ME", 10),
            new KeyValuePair<string, int>("HO_TEN_CHA", 255),
            new KeyValuePair<string, int>("HO_TEN_ME", 255),
            new KeyValuePair<string, int>("SO_NGAY_NGHI", 3),
            new KeyValuePair<string, int>("NGOAITRU_TUNGAY", 8),
            new KeyValuePair<string, int>("NGOAITRU_DENNGAY", 8),
        };

        private static readonly string[] _numeric =
        {
            "MA_DINH_CHI_THAI", "TUOI_THAI", "SO_NGAY_NGHI",
        };

        private static readonly string[] _date12 =
        {
            "NGAY_VAO", "NGAY_VAO_NOI_TRU", "NGAY_RA", "THOIGIAN_DINHCHI",
        };

        private static readonly string[] _date8 =
        {
            "NGAY_CT", "NGOAITRU_TUNGAY", "NGOAITRU_DENNGAY",
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
                case "SO_LUU_TRU": return SO_LUU_TRU;
                case "MA_YTE": return MA_YTE;
                case "MA_KHOA_RV": return MA_KHOA_RV;
                case "NGAY_VAO": return NGAY_VAO;
                case "NGAY_VAO_NOI_TRU": return NGAY_VAO_NOI_TRU;
                case "NGAY_RA": return NGAY_RA;
                case "MA_DINH_CHI_THAI": return MA_DINH_CHI_THAI;
                case "NGUYENNHAN_DINHCHI": return NGUYENNHAN_DINHCHI;
                case "THOIGIAN_DINHCHI": return THOIGIAN_DINHCHI;
                case "TUOI_THAI": return TUOI_THAI;
                case "CHAN_DOAN_RV": return CHAN_DOAN_RV;
                case "PP_DIEUTRI": return PP_DIEUTRI;
                case "GHI_CHU": return GHI_CHU;
                case "MA_TTDV": return MA_TTDV;
                case "MA_BS": return MA_BS;
                case "TEN_BS": return TEN_BS;
                case "NGAY_CT": return NGAY_CT;
                case "MA_CHA": return MA_CHA;
                case "MA_ME": return MA_ME;
                case "MA_THE_TAM": return MA_THE_TAM;
                case "HO_TEN_CHA": return HO_TEN_CHA;
                case "HO_TEN_ME": return HO_TEN_ME;
                case "SO_NGAY_NGHI": return SO_NGAY_NGHI;
                case "NGOAITRU_TUNGAY": return NGOAITRU_TUNGAY;
                case "NGOAITRU_DENNGAY": return NGOAITRU_DENNGAY;
                case "DU_PHONG": return DU_PHONG;
                default: return null;
            }
        }

        public override bool SetValue(string fieldName, string value)
        {
            switch (fieldName)
            {
                case "MA_LK": MA_LK = value; return true;
                case "SO_LUU_TRU": SO_LUU_TRU = value; return true;
                case "MA_YTE": MA_YTE = value; return true;
                case "MA_KHOA_RV": MA_KHOA_RV = value; return true;
                case "NGAY_VAO": NGAY_VAO = value; return true;
                case "NGAY_VAO_NOI_TRU": NGAY_VAO_NOI_TRU = value; return true;
                case "NGAY_RA": NGAY_RA = value; return true;
                case "MA_DINH_CHI_THAI": MA_DINH_CHI_THAI = value; return true;
                case "NGUYENNHAN_DINHCHI": NGUYENNHAN_DINHCHI = value; return true;
                case "THOIGIAN_DINHCHI": THOIGIAN_DINHCHI = value; return true;
                case "TUOI_THAI": TUOI_THAI = value; return true;
                case "CHAN_DOAN_RV": CHAN_DOAN_RV = value; return true;
                case "PP_DIEUTRI": PP_DIEUTRI = value; return true;
                case "GHI_CHU": GHI_CHU = value; return true;
                case "MA_TTDV": MA_TTDV = value; return true;
                case "MA_BS": MA_BS = value; return true;
                case "TEN_BS": TEN_BS = value; return true;
                case "NGAY_CT": NGAY_CT = value; return true;
                case "MA_CHA": MA_CHA = value; return true;
                case "MA_ME": MA_ME = value; return true;
                case "MA_THE_TAM": MA_THE_TAM = value; return true;
                case "HO_TEN_CHA": HO_TEN_CHA = value; return true;
                case "HO_TEN_ME": HO_TEN_ME = value; return true;
                case "SO_NGAY_NGHI": SO_NGAY_NGHI = value; return true;
                case "NGOAITRU_TUNGAY": NGOAITRU_TUNGAY = value; return true;
                case "NGOAITRU_DENNGAY": NGOAITRU_DENNGAY = value; return true;
                case "DU_PHONG": DU_PHONG = value; return true;
                default: return false;
            }
        }

        private static string Left8(string s)
        {
            s = s ?? "None";
            return s.Length > 8 ? s.Substring(0, 8) : s;
        }

        // Lưu ý (giữ nguyên bản Python): validate() của Bảng 7 KHÔNG gọi base.Validate().
        public override List<string> Validate()
        {
            var errs = new List<string>();

            // 1. MA_LK – bắt buộc
            if (string.IsNullOrEmpty(MA_LK))
                errs.Add("MA_LK: bắt buộc, không được để trống");

            // 5-6. NGAY_VAO, NGAY_RA – DATE12, NGAY_RA >= NGAY_VAO
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

            // 7. MA_DINH_CHI_THAI – enum {0, 1}
            long? maDct = ToInt(MA_DINH_CHI_THAI);
            if (MA_DINH_CHI_THAI != null && !(maDct == 0 || maDct == 1))
                errs.Add($"MA_DINH_CHI_THAI: '{MA_DINH_CHI_THAI}' không hợp lệ, " +
                         "chỉ chấp nhận 0 (không đình chỉ) hoặc 1 (đình chỉ thai nghén)");

            // Nhóm đình chỉ thai: khi MA_DINH_CHI_THAI = 1 → 3 trường bắt buộc
            if (maDct == 1)
            {
                if (string.IsNullOrEmpty(NGUYENNHAN_DINHCHI))
                    errs.Add("NGUYENNHAN_DINHCHI: bắt buộc khi MA_DINH_CHI_THAI = 1");
                if (string.IsNullOrEmpty(THOIGIAN_DINHCHI))
                    errs.Add("THOIGIAN_DINHCHI: bắt buộc khi MA_DINH_CHI_THAI = 1");
                if (string.IsNullOrEmpty(TUOI_THAI))
                    errs.Add("TUOI_THAI: bắt buộc khi MA_DINH_CHI_THAI = 1");
            }

            // 9. THOIGIAN_DINHCHI – hợp lệ và nằm trong [NGAY_VAO, NGAY_RA]
            if (!string.IsNullOrEmpty(THOIGIAN_DINHCHI))
            {
                DateTime? dtDct = ParseDate12(THOIGIAN_DINHCHI);
                if (dtDct == null)
                    errs.Add($"THOIGIAN_DINHCHI: '{THOIGIAN_DINHCHI}' " +
                             "không phải ngày giờ hợp lệ (định dạng yyyymmddHHMM)");
                if (dtDct != null && dtVao != null && dtDct.Value < dtVao.Value)
                    errs.Add($"THOIGIAN_DINHCHI ({THOIGIAN_DINHCHI}) " +
                             $"không được trước NGAY_VAO ({NGAY_VAO})");
                if (dtDct != null && dtRa != null && dtDct.Value > dtRa.Value)
                    errs.Add($"THOIGIAN_DINHCHI ({THOIGIAN_DINHCHI}) " +
                             $"không được sau NGAY_RA ({NGAY_RA})");
            }

            // 10. TUOI_THAI – 1–42 tuần
            long? tuoiThai = ToInt(TUOI_THAI);
            if (TUOI_THAI != null && tuoiThai != null)
            {
                if (!(1 <= tuoiThai && tuoiThai <= 42))
                    errs.Add($"TUOI_THAI: '{TUOI_THAI}' không hợp lệ, " +
                             "tuổi thai phải trong 1–42 tuần");
            }

            // 17. NGAY_CT – DATE8, phải trùng ngày với NGAY_RA
            DateTime? dtCt = ParseDate8(NGAY_CT);
            if (!string.IsNullOrEmpty(NGAY_CT) && dtCt == null)
                errs.Add($"NGAY_CT: '{NGAY_CT}' không phải ngày hợp lệ " +
                         "(định dạng yyyymmdd)");
            if (dtCt != null && dtRa != null)
            {
                if (dtCt.Value.Date != dtRa.Value.Date)
                    errs.Add($"NGAY_CT ({NGAY_CT}) phải trùng với ngày ra viện " +
                             $"NGAY_RA ({Left8(NGAY_RA)})");
            }

            // 23. SO_NGAY_NGHI – >= 0
            long? soNgayNghi = ToInt(SO_NGAY_NGHI);
            if (SO_NGAY_NGHI != null && soNgayNghi != null)
            {
                if (soNgayNghi < 0)
                    errs.Add($"SO_NGAY_NGHI: '{SO_NGAY_NGHI}' không được âm");
            }

            // 24-25. NGOAITRU_TUNGAY / NGOAITRU_DENNGAY – DATE8
            DateTime? dtTu = ParseDate8(NGOAITRU_TUNGAY);
            DateTime? dtDen = ParseDate8(NGOAITRU_DENNGAY);

            if (!string.IsNullOrEmpty(NGOAITRU_TUNGAY) && dtTu == null)
                errs.Add($"NGOAITRU_TUNGAY: '{NGOAITRU_TUNGAY}' không phải ngày hợp lệ " +
                         "(định dạng yyyymmdd)");
            if (!string.IsNullOrEmpty(NGOAITRU_DENNGAY) && dtDen == null)
                errs.Add($"NGOAITRU_DENNGAY: '{NGOAITRU_DENNGAY}' không phải ngày hợp lệ " +
                         "(định dạng yyyymmdd)");

            // NGOAITRU_TUNGAY không được trước ngày ra viện
            if (dtTu != null && dtRa != null && dtTu.Value.Date < dtRa.Value.Date)
                errs.Add($"NGOAITRU_TUNGAY ({NGOAITRU_TUNGAY}) không được trước " +
                         $"ngày ra viện ({Left8(NGAY_RA)})");

            // NGOAITRU_DENNGAY >= NGOAITRU_TUNGAY
            if (dtTu != null && dtDen != null && dtDen.Value < dtTu.Value)
                errs.Add($"NGOAITRU_DENNGAY ({NGOAITRU_DENNGAY}) không được trước " +
                         $"NGOAITRU_TUNGAY ({NGOAITRU_TUNGAY})");

            // SO_NGAY_NGHI = DENNGAY - TUNGAY + 1
            if (dtTu != null && dtDen != null && soNgayNghi != null && soNgayNghi > 0)
            {
                long expectedNgay = (long)(dtDen.Value.Date - dtTu.Value.Date).TotalDays + 1;
                if (expectedNgay != soNgayNghi)
                    errs.Add($"SO_NGAY_NGHI: kỳ vọng {expectedNgay} ngày " +
                             "(NGOAITRU_DENNGAY - NGOAITRU_TUNGAY + 1), " +
                             $"thực tế {soNgayNghi}");
            }

            // Tính nhất quán: có ngày ngoại trú ⇔ có SO_NGAY_NGHI
            bool hasNgoaiTru = !string.IsNullOrEmpty(NGOAITRU_TUNGAY) || !string.IsNullOrEmpty(NGOAITRU_DENNGAY);
            bool hasSoNgay = soNgayNghi != null && soNgayNghi > 0;
            if (hasNgoaiTru && !hasSoNgay)
                errs.Add("SO_NGAY_NGHI: bắt buộc khi có NGOAITRU_TUNGAY/NGOAITRU_DENNGAY");
            if (hasSoNgay && !hasNgoaiTru)
                errs.Add("NGOAITRU_TUNGAY / NGOAITRU_DENNGAY: bắt buộc khi SO_NGAY_NGHI > 0");
            // Phải có đủ cả TUNGAY lẫn DENNGAY
            if (!string.IsNullOrEmpty(NGOAITRU_TUNGAY) != !string.IsNullOrEmpty(NGOAITRU_DENNGAY))
                errs.Add("NGOAITRU_TUNGAY và NGOAITRU_DENNGAY: phải cùng có hoặc cùng để trống");

            return errs;
        }
    }
}