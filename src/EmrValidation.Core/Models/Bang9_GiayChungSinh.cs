using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using EmrValidation.Core.Helpers;

namespace EmrValidation.Core.Models
{
    /// <summary>Bảng 9. Chỉ tiêu dữ liệu giấy chứng sinh.</summary>
    public class Bang9_GiayChungSinh : BhytBase
    {
        public string MA_LK { get; set; }
        public string MA_BHXH_NND { get; set; }
        public string MA_THE_NND { get; set; }
        public string HO_TEN_NND { get; set; }
        public string NGAYSINH_NND { get; set; }
        public string MA_DANTOC_NND { get; set; }
        public string SO_CCCD_NND { get; set; }
        public string NGAYCAP_CCCD_NND { get; set; }
        public string NOICAP_CCCD_NND { get; set; }
        public string NOI_CU_TRU_NND { get; set; }
        public string MA_QUOCTICH { get; set; }
        public string MATINH_CU_TRU { get; set; }
        public string MAHUYEN_CU_TRU { get; set; }
        public string MAXA_CU_TRU { get; set; }
        public string HO_TEN_CHA { get; set; }
        public string MA_THE_TAM { get; set; }
        public string HO_TEN_CON { get; set; }
        public string GIOI_TINH_CON { get; set; }
        public string SO_CON { get; set; }
        public string LAN_SINH { get; set; }
        public string SO_CON_SONG { get; set; }
        public string CAN_NANG_CON { get; set; }
        public string NGAY_SINH_CON { get; set; }
        public string NOI_SINH_CON { get; set; }
        public string TINH_TRANG_CON { get; set; }
        public string SINHCON_PHAUTHUAT { get; set; }
        public string SINHCON_DUOI32TUAN { get; set; }
        public string GHI_CHU { get; set; }
        public string NGUOI_DO_DE { get; set; }
        public string NGUOI_GHI_PHIEU { get; set; }
        public string NGAY_CT { get; set; }
        public string SO { get; set; }
        public string QUYEN_SO { get; set; }
        public string MA_TTDV { get; set; }
        public string DU_PHONG { get; set; }

        // ---------------- Metadata ----------------
        private static readonly string[] _fieldNames =
        {
            "MA_LK", "MA_BHXH_NND", "MA_THE_NND", "HO_TEN_NND", "NGAYSINH_NND", "MA_DANTOC_NND",
            "SO_CCCD_NND", "NGAYCAP_CCCD_NND", "NOICAP_CCCD_NND", "NOI_CU_TRU_NND", "MA_QUOCTICH", "MATINH_CU_TRU",
            "MAHUYEN_CU_TRU", "MAXA_CU_TRU", "HO_TEN_CHA", "MA_THE_TAM", "HO_TEN_CON", "GIOI_TINH_CON",
            "SO_CON", "LAN_SINH", "SO_CON_SONG", "CAN_NANG_CON", "NGAY_SINH_CON", "NOI_SINH_CON",
            "TINH_TRANG_CON", "SINHCON_PHAUTHUAT", "SINHCON_DUOI32TUAN", "GHI_CHU", "NGUOI_DO_DE", "NGUOI_GHI_PHIEU",
            "NGAY_CT", "SO", "QUYEN_SO", "MA_TTDV", "DU_PHONG",
        };

        private static readonly KeyValuePair<string, int>[] _maxLen =
        {
            new KeyValuePair<string, int>("MA_LK", 100),
            new KeyValuePair<string, int>("HO_TEN_NND", 255),
            new KeyValuePair<string, int>("NGAYSINH_NND", 8),
            new KeyValuePair<string, int>("MA_DANTOC_NND", 2),
            new KeyValuePair<string, int>("NGAYCAP_CCCD_NND", 8),
            new KeyValuePair<string, int>("NOICAP_CCCD_NND", 1024),
            new KeyValuePair<string, int>("NOI_CU_TRU_NND", 1024),
            new KeyValuePair<string, int>("MA_QUOCTICH", 3),
            new KeyValuePair<string, int>("MATINH_CU_TRU", 3),
            new KeyValuePair<string, int>("MAHUYEN_CU_TRU", 3),
            new KeyValuePair<string, int>("MAXA_CU_TRU", 5),
            new KeyValuePair<string, int>("HO_TEN_CHA", 255),
            new KeyValuePair<string, int>("HO_TEN_CON", 255),
            new KeyValuePair<string, int>("GIOI_TINH_CON", 1),
            new KeyValuePair<string, int>("SO_CON", 2),
            new KeyValuePair<string, int>("LAN_SINH", 2),
            new KeyValuePair<string, int>("SO_CON_SONG", 2),
            new KeyValuePair<string, int>("CAN_NANG_CON", 10),
            new KeyValuePair<string, int>("NGAY_SINH_CON", 12),
            new KeyValuePair<string, int>("NOI_SINH_CON", 1024),
            new KeyValuePair<string, int>("SINHCON_PHAUTHUAT", 1),
            new KeyValuePair<string, int>("SINHCON_DUOI32TUAN", 1),
            new KeyValuePair<string, int>("NGUOI_DO_DE", 255),
            new KeyValuePair<string, int>("NGUOI_GHI_PHIEU", 255),
            new KeyValuePair<string, int>("NGAY_CT", 8),
            new KeyValuePair<string, int>("SO", 200),
            new KeyValuePair<string, int>("QUYEN_SO", 200),
            new KeyValuePair<string, int>("MA_TTDV", 255),
        };

        private static readonly string[] _numeric =
        {
            "GIOI_TINH_CON", "SO_CON", "LAN_SINH", "SO_CON_SONG", "CAN_NANG_CON", "SINHCON_PHAUTHUAT",
            "SINHCON_DUOI32TUAN",
        };

        private static readonly string[] _date12 =
        {
            "NGAY_SINH_CON",
        };

        private static readonly string[] _date8 =
        {
            "NGAYSINH_NND", "NGAYCAP_CCCD_NND", "NGAY_CT",
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
                case "MA_BHXH_NND": return MA_BHXH_NND;
                case "MA_THE_NND": return MA_THE_NND;
                case "HO_TEN_NND": return HO_TEN_NND;
                case "NGAYSINH_NND": return NGAYSINH_NND;
                case "MA_DANTOC_NND": return MA_DANTOC_NND;
                case "SO_CCCD_NND": return SO_CCCD_NND;
                case "NGAYCAP_CCCD_NND": return NGAYCAP_CCCD_NND;
                case "NOICAP_CCCD_NND": return NOICAP_CCCD_NND;
                case "NOI_CU_TRU_NND": return NOI_CU_TRU_NND;
                case "MA_QUOCTICH": return MA_QUOCTICH;
                case "MATINH_CU_TRU": return MATINH_CU_TRU;
                case "MAHUYEN_CU_TRU": return MAHUYEN_CU_TRU;
                case "MAXA_CU_TRU": return MAXA_CU_TRU;
                case "HO_TEN_CHA": return HO_TEN_CHA;
                case "MA_THE_TAM": return MA_THE_TAM;
                case "HO_TEN_CON": return HO_TEN_CON;
                case "GIOI_TINH_CON": return GIOI_TINH_CON;
                case "SO_CON": return SO_CON;
                case "LAN_SINH": return LAN_SINH;
                case "SO_CON_SONG": return SO_CON_SONG;
                case "CAN_NANG_CON": return CAN_NANG_CON;
                case "NGAY_SINH_CON": return NGAY_SINH_CON;
                case "NOI_SINH_CON": return NOI_SINH_CON;
                case "TINH_TRANG_CON": return TINH_TRANG_CON;
                case "SINHCON_PHAUTHUAT": return SINHCON_PHAUTHUAT;
                case "SINHCON_DUOI32TUAN": return SINHCON_DUOI32TUAN;
                case "GHI_CHU": return GHI_CHU;
                case "NGUOI_DO_DE": return NGUOI_DO_DE;
                case "NGUOI_GHI_PHIEU": return NGUOI_GHI_PHIEU;
                case "NGAY_CT": return NGAY_CT;
                case "SO": return SO;
                case "QUYEN_SO": return QUYEN_SO;
                case "MA_TTDV": return MA_TTDV;
                case "DU_PHONG": return DU_PHONG;
                default: return null;
            }
        }

        public override bool SetValue(string fieldName, string value)
        {
            switch (fieldName)
            {
                case "MA_LK": MA_LK = value; return true;
                case "MA_BHXH_NND": MA_BHXH_NND = value; return true;
                case "MA_THE_NND": MA_THE_NND = value; return true;
                case "HO_TEN_NND": HO_TEN_NND = value; return true;
                case "NGAYSINH_NND": NGAYSINH_NND = value; return true;
                case "MA_DANTOC_NND": MA_DANTOC_NND = value; return true;
                case "SO_CCCD_NND": SO_CCCD_NND = value; return true;
                case "NGAYCAP_CCCD_NND": NGAYCAP_CCCD_NND = value; return true;
                case "NOICAP_CCCD_NND": NOICAP_CCCD_NND = value; return true;
                case "NOI_CU_TRU_NND": NOI_CU_TRU_NND = value; return true;
                case "MA_QUOCTICH": MA_QUOCTICH = value; return true;
                case "MATINH_CU_TRU": MATINH_CU_TRU = value; return true;
                case "MAHUYEN_CU_TRU": MAHUYEN_CU_TRU = value; return true;
                case "MAXA_CU_TRU": MAXA_CU_TRU = value; return true;
                case "HO_TEN_CHA": HO_TEN_CHA = value; return true;
                case "MA_THE_TAM": MA_THE_TAM = value; return true;
                case "HO_TEN_CON": HO_TEN_CON = value; return true;
                case "GIOI_TINH_CON": GIOI_TINH_CON = value; return true;
                case "SO_CON": SO_CON = value; return true;
                case "LAN_SINH": LAN_SINH = value; return true;
                case "SO_CON_SONG": SO_CON_SONG = value; return true;
                case "CAN_NANG_CON": CAN_NANG_CON = value; return true;
                case "NGAY_SINH_CON": NGAY_SINH_CON = value; return true;
                case "NOI_SINH_CON": NOI_SINH_CON = value; return true;
                case "TINH_TRANG_CON": TINH_TRANG_CON = value; return true;
                case "SINHCON_PHAUTHUAT": SINHCON_PHAUTHUAT = value; return true;
                case "SINHCON_DUOI32TUAN": SINHCON_DUOI32TUAN = value; return true;
                case "GHI_CHU": GHI_CHU = value; return true;
                case "NGUOI_DO_DE": NGUOI_DO_DE = value; return true;
                case "NGUOI_GHI_PHIEU": NGUOI_GHI_PHIEU = value; return true;
                case "NGAY_CT": NGAY_CT = value; return true;
                case "SO": SO = value; return true;
                case "QUYEN_SO": QUYEN_SO = value; return true;
                case "MA_TTDV": MA_TTDV = value; return true;
                case "DU_PHONG": DU_PHONG = value; return true;
                default: return false;
            }
        }

        public override List<string> Validate()
        {
            var errs = base.Validate();

            // 1. MA_LK – bắt buộc
            if (string.IsNullOrEmpty(MA_LK))
                errs.Add("MA_LK: bắt buộc, không được để trống");

            // 18. GIOI_TINH_CON – enum {1, 2, 3}
            long? gtCon = ToInt(GIOI_TINH_CON);
            if (GIOI_TINH_CON != null && !(gtCon == 1 || gtCon == 2 || gtCon == 3))
                errs.Add($"GIOI_TINH_CON: '{GIOI_TINH_CON}' không hợp lệ " +
                         "(cần 1=Nam / 2=Nữ / 3=Chưa xác định)");

            // 19. SO_CON – >= 1
            long? soCon = ToInt(SO_CON);
            if (SO_CON != null && soCon != null && soCon < 1)
                errs.Add($"SO_CON: '{SO_CON}' phải >= 1");

            // 20. LAN_SINH – >= 1
            long? lanSinh = ToInt(LAN_SINH);
            if (LAN_SINH != null && lanSinh != null && lanSinh < 1)
                errs.Add($"LAN_SINH: '{LAN_SINH}' phải >= 1 (tính cả lần sinh này)");

            // 21. SO_CON_SONG – >= 0
            long? soConSong = ToInt(SO_CON_SONG);
            if (SO_CON_SONG != null && soConSong != null && soConSong < 0)
                errs.Add($"SO_CON_SONG: '{SO_CON_SONG}' không được âm");

            // 22. CAN_NANG_CON – > 0 (gram)
            double? canNang = ToFloat(CAN_NANG_CON);
            if (CAN_NANG_CON != null && canNang != null && canNang.Value <= 0)
                errs.Add($"CAN_NANG_CON: '{CAN_NANG_CON}' phải > 0 (đơn vị gram)");

            // 26. SINHCON_PHAUTHUAT – enum {0, 1}
            long? phauThuat = ToInt(SINHCON_PHAUTHUAT);
            if (SINHCON_PHAUTHUAT != null && !(phauThuat == 0 || phauThuat == 1))
                errs.Add($"SINHCON_PHAUTHUAT: '{SINHCON_PHAUTHUAT}' không hợp lệ " +
                         "(cần 0=không phẫu thuật / 1=có phẫu thuật)");

            // 27. SINHCON_DUOI32TUAN – enum {0, 1}
            long? duoi32Tuan = ToInt(SINHCON_DUOI32TUAN);
            if (SINHCON_DUOI32TUAN != null && !(duoi32Tuan == 0 || duoi32Tuan == 1))
                errs.Add($"SINHCON_DUOI32TUAN: '{SINHCON_DUOI32TUAN}' không hợp lệ " +
                         "(cần 0=không / 1=sinh con dưới 32 tuần tuổi)");

            // 28. GHI_CHU – bắt buộc khi có phẫu thuật / dưới 32 tuần
            bool needGhiChu = (phauThuat == 1) || (duoi32Tuan == 1);
            if (needGhiChu)
            {
                if (string.IsNullOrEmpty(GHI_CHU))
                {
                    errs.Add("GHI_CHU: bắt buộc khi SINHCON_PHAUTHUAT=1 hoặc SINHCON_DUOI32TUAN=1. " +
                             "Phải ghi rõ: \"Sinh con phải phẫu thuật\" hoặc " +
                             "\"Sinh con dưới 32 tuần tuổi\" hoặc " +
                             "\"Phẫu thuật, sinh con dưới 32 tuần tuổi\"");
                }
                else
                {
                    string ghiChuNorm = GHI_CHU.Trim().ToLowerInvariant();
                    if (!In(ghiChuNorm,
                            "sinh con phải phẫu thuật",
                            "sinh con dưới 32 tuần tuổi",
                            "phẫu thuật, sinh con dưới 32 tuần tuổi"))
                        errs.Add($"GHI_CHU: '{GHI_CHU}' không đúng nội dung quy định. " +
                                 "Phải là (không phân biệt hoa thường): " +
                                 "\"Sinh con phải phẫu thuật\" / " +
                                 "\"Sinh con dưới 32 tuần tuổi\" / " +
                                 "\"Phẫu thuật, sinh con dưới 32 tuần tuổi\"");
                }
            }

            // Cross-field: cả hai = 1 → GHI_CHU phải ghi đủ cả hai nội dung
            if (phauThuat == 1 && duoi32Tuan == 1 && !string.IsNullOrEmpty(GHI_CHU))
            {
                string ghiChuNorm = GHI_CHU.Trim().ToLowerInvariant();
                if (ghiChuNorm != "phẫu thuật, sinh con dưới 32 tuần tuổi")
                    errs.Add("GHI_CHU: khi cả SINHCON_PHAUTHUAT=1 và SINHCON_DUOI32TUAN=1, " +
                             "phải ghi: \"Phẫu thuật, sinh con dưới 32 tuần tuổi\"");
            }

            return errs;
        }
    }
}