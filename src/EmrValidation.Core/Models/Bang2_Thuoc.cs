using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using EmrValidation.Core.Helpers;

namespace EmrValidation.Core.Models
{
    /// <summary>Bảng 2. Chỉ tiêu chi tiết thuốc.</summary>
    public class Bang2_Thuoc : BhytBase
    {
        public string MA_LK { get; set; }
        public string STT { get; set; }
        public string MA_THUOC { get; set; }
        public string MA_PP_CHEBIEN { get; set; }
        public string MA_CSKCB_THUOC { get; set; }
        public string MA_NHOM { get; set; }
        public string TEN_THUOC { get; set; }
        public string DON_VI_TINH { get; set; }
        public string HAM_LUONG { get; set; }
        public string DUONG_DUNG { get; set; }
        public string DANG_BAO_CHE { get; set; }
        public string LIEU_DUNG { get; set; }
        public string CACH_DUNG { get; set; }
        public string SO_DANG_KY { get; set; }
        public string TT_THAU { get; set; }
        public string PHAM_VI { get; set; }
        public string TYLE_TT_BH { get; set; }
        public string SO_LUONG { get; set; }
        public string DON_GIA { get; set; }
        public string THANH_TIEN_BV { get; set; }
        public string THANH_TIEN_BH { get; set; }
        public string T_NGUONKHAC_NSNN { get; set; }
        public string T_NGUONKHAC_VTNN { get; set; }
        public string T_NGUONKHAC_VTTN { get; set; }
        public string T_NGUONKHAC_CL { get; set; }
        public string T_NGUONKHAC { get; set; }
        public string MUC_HUONG { get; set; }
        public string T_BNTT { get; set; }
        public string T_BNCCT { get; set; }
        public string T_BHTT { get; set; }
        public string MA_KHOA { get; set; }
        public string MA_BAC_SI { get; set; }
        public string MA_DICH_VU { get; set; }
        public string NGAY_YL { get; set; }
        public string NGAY_TH_YL { get; set; }
        public string MA_PTTT { get; set; }
        public string NGUON_CTRA { get; set; }
        public string VET_THUONG_TP { get; set; }
        public string DU_PHONG { get; set; }

        // ---------------- Metadata ----------------
        private static readonly string[] _fieldNames =
        {
            "MA_LK", "STT", "MA_THUOC", "MA_PP_CHEBIEN", "MA_CSKCB_THUOC", "MA_NHOM",
            "TEN_THUOC", "DON_VI_TINH", "HAM_LUONG", "DUONG_DUNG", "DANG_BAO_CHE", "LIEU_DUNG",
            "CACH_DUNG", "SO_DANG_KY", "TT_THAU", "PHAM_VI", "TYLE_TT_BH", "SO_LUONG",
            "DON_GIA", "THANH_TIEN_BV", "THANH_TIEN_BH", "T_NGUONKHAC_NSNN", "T_NGUONKHAC_VTNN", "T_NGUONKHAC_VTTN",
            "T_NGUONKHAC_CL", "T_NGUONKHAC", "MUC_HUONG", "T_BNTT", "T_BNCCT", "T_BHTT",
            "MA_KHOA", "MA_BAC_SI", "MA_DICH_VU", "NGAY_YL", "NGAY_TH_YL", "MA_PTTT",
            "NGUON_CTRA", "VET_THUONG_TP", "DU_PHONG",
        };

        private static readonly KeyValuePair<string, int>[] _maxLen =
        {
            new KeyValuePair<string, int>("MA_LK", 100),
            new KeyValuePair<string, int>("STT", 10),
            new KeyValuePair<string, int>("MA_THUOC", 255),
            new KeyValuePair<string, int>("MA_PP_CHEBIEN", 255),
            new KeyValuePair<string, int>("MA_CSKCB_THUOC", 10),
            new KeyValuePair<string, int>("MA_NHOM", 2),
            new KeyValuePair<string, int>("TEN_THUOC", 1024),
            new KeyValuePair<string, int>("DON_VI_TINH", 50),
            new KeyValuePair<string, int>("HAM_LUONG", 1024),
            new KeyValuePair<string, int>("DUONG_DUNG", 4),
            new KeyValuePair<string, int>("DANG_BAO_CHE", 1024),
            new KeyValuePair<string, int>("LIEU_DUNG", 1024),
            new KeyValuePair<string, int>("CACH_DUNG", 1024),
            new KeyValuePair<string, int>("SO_DANG_KY", 255),
            new KeyValuePair<string, int>("TT_THAU", 50),
            new KeyValuePair<string, int>("PHAM_VI", 1),
            new KeyValuePair<string, int>("TYLE_TT_BH", 3),
            new KeyValuePair<string, int>("SO_LUONG", 10),
            new KeyValuePair<string, int>("DON_GIA", 15),
            new KeyValuePair<string, int>("THANH_TIEN_BV", 15),
            new KeyValuePair<string, int>("THANH_TIEN_BH", 15),
            new KeyValuePair<string, int>("T_NGUONKHAC_NSNN", 15),
            new KeyValuePair<string, int>("T_NGUONKHAC_VTNN", 15),
            new KeyValuePair<string, int>("T_NGUONKHAC_VTTN", 15),
            new KeyValuePair<string, int>("T_NGUONKHAC_CL", 15),
            new KeyValuePair<string, int>("T_NGUONKHAC", 15),
            new KeyValuePair<string, int>("MUC_HUONG", 4),
            new KeyValuePair<string, int>("T_BNTT", 15),
            new KeyValuePair<string, int>("T_BNCCT", 15),
            new KeyValuePair<string, int>("T_BHTT", 15),
            new KeyValuePair<string, int>("MA_KHOA", 50),
            new KeyValuePair<string, int>("MA_BAC_SI", 255),
            new KeyValuePair<string, int>("MA_DICH_VU", 255),
            new KeyValuePair<string, int>("NGAY_YL", 12),
            new KeyValuePair<string, int>("NGAY_TH_YL", 12),
            new KeyValuePair<string, int>("MA_PTTT", 1),
            new KeyValuePair<string, int>("NGUON_CTRA", 1),
            new KeyValuePair<string, int>("VET_THUONG_TP", 1),
        };

        private static readonly string[] _numeric =
        {
            "STT", "MA_NHOM", "PHAM_VI", "TYLE_TT_BH", "SO_LUONG", "DON_GIA",
            "THANH_TIEN_BV", "THANH_TIEN_BH", "T_NGUONKHAC_NSNN", "T_NGUONKHAC_VTNN", "T_NGUONKHAC_VTTN", "T_NGUONKHAC_CL",
            "T_NGUONKHAC", "MUC_HUONG", "T_BNTT", "T_BNCCT", "T_BHTT", "MA_PTTT",
            "NGUON_CTRA", "VET_THUONG_TP",
        };

        private static readonly string[] _date12 =
        {
            "NGAY_YL", "NGAY_TH_YL",
        };

        private static readonly string[] _date8 = new string[0];

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
                case "STT": return STT;
                case "MA_THUOC": return MA_THUOC;
                case "MA_PP_CHEBIEN": return MA_PP_CHEBIEN;
                case "MA_CSKCB_THUOC": return MA_CSKCB_THUOC;
                case "MA_NHOM": return MA_NHOM;
                case "TEN_THUOC": return TEN_THUOC;
                case "DON_VI_TINH": return DON_VI_TINH;
                case "HAM_LUONG": return HAM_LUONG;
                case "DUONG_DUNG": return DUONG_DUNG;
                case "DANG_BAO_CHE": return DANG_BAO_CHE;
                case "LIEU_DUNG": return LIEU_DUNG;
                case "CACH_DUNG": return CACH_DUNG;
                case "SO_DANG_KY": return SO_DANG_KY;
                case "TT_THAU": return TT_THAU;
                case "PHAM_VI": return PHAM_VI;
                case "TYLE_TT_BH": return TYLE_TT_BH;
                case "SO_LUONG": return SO_LUONG;
                case "DON_GIA": return DON_GIA;
                case "THANH_TIEN_BV": return THANH_TIEN_BV;
                case "THANH_TIEN_BH": return THANH_TIEN_BH;
                case "T_NGUONKHAC_NSNN": return T_NGUONKHAC_NSNN;
                case "T_NGUONKHAC_VTNN": return T_NGUONKHAC_VTNN;
                case "T_NGUONKHAC_VTTN": return T_NGUONKHAC_VTTN;
                case "T_NGUONKHAC_CL": return T_NGUONKHAC_CL;
                case "T_NGUONKHAC": return T_NGUONKHAC;
                case "MUC_HUONG": return MUC_HUONG;
                case "T_BNTT": return T_BNTT;
                case "T_BNCCT": return T_BNCCT;
                case "T_BHTT": return T_BHTT;
                case "MA_KHOA": return MA_KHOA;
                case "MA_BAC_SI": return MA_BAC_SI;
                case "MA_DICH_VU": return MA_DICH_VU;
                case "NGAY_YL": return NGAY_YL;
                case "NGAY_TH_YL": return NGAY_TH_YL;
                case "MA_PTTT": return MA_PTTT;
                case "NGUON_CTRA": return NGUON_CTRA;
                case "VET_THUONG_TP": return VET_THUONG_TP;
                case "DU_PHONG": return DU_PHONG;
                default: return null;
            }
        }

        public override bool SetValue(string fieldName, string value)
        {
            switch (fieldName)
            {
                case "MA_LK": MA_LK = value; return true;
                case "STT": STT = value; return true;
                case "MA_THUOC": MA_THUOC = value; return true;
                case "MA_PP_CHEBIEN": MA_PP_CHEBIEN = value; return true;
                case "MA_CSKCB_THUOC": MA_CSKCB_THUOC = value; return true;
                case "MA_NHOM": MA_NHOM = value; return true;
                case "TEN_THUOC": TEN_THUOC = value; return true;
                case "DON_VI_TINH": DON_VI_TINH = value; return true;
                case "HAM_LUONG": HAM_LUONG = value; return true;
                case "DUONG_DUNG": DUONG_DUNG = value; return true;
                case "DANG_BAO_CHE": DANG_BAO_CHE = value; return true;
                case "LIEU_DUNG": LIEU_DUNG = value; return true;
                case "CACH_DUNG": CACH_DUNG = value; return true;
                case "SO_DANG_KY": SO_DANG_KY = value; return true;
                case "TT_THAU": TT_THAU = value; return true;
                case "PHAM_VI": PHAM_VI = value; return true;
                case "TYLE_TT_BH": TYLE_TT_BH = value; return true;
                case "SO_LUONG": SO_LUONG = value; return true;
                case "DON_GIA": DON_GIA = value; return true;
                case "THANH_TIEN_BV": THANH_TIEN_BV = value; return true;
                case "THANH_TIEN_BH": THANH_TIEN_BH = value; return true;
                case "T_NGUONKHAC_NSNN": T_NGUONKHAC_NSNN = value; return true;
                case "T_NGUONKHAC_VTNN": T_NGUONKHAC_VTNN = value; return true;
                case "T_NGUONKHAC_VTTN": T_NGUONKHAC_VTTN = value; return true;
                case "T_NGUONKHAC_CL": T_NGUONKHAC_CL = value; return true;
                case "T_NGUONKHAC": T_NGUONKHAC = value; return true;
                case "MUC_HUONG": MUC_HUONG = value; return true;
                case "T_BNTT": T_BNTT = value; return true;
                case "T_BNCCT": T_BNCCT = value; return true;
                case "T_BHTT": T_BHTT = value; return true;
                case "MA_KHOA": MA_KHOA = value; return true;
                case "MA_BAC_SI": MA_BAC_SI = value; return true;
                case "MA_DICH_VU": MA_DICH_VU = value; return true;
                case "NGAY_YL": NGAY_YL = value; return true;
                case "NGAY_TH_YL": NGAY_TH_YL = value; return true;
                case "MA_PTTT": MA_PTTT = value; return true;
                case "NGUON_CTRA": NGUON_CTRA = value; return true;
                case "VET_THUONG_TP": VET_THUONG_TP = value; return true;
                case "DU_PHONG": DU_PHONG = value; return true;
                default: return false;
            }
        }

        public override List<string> Validate()
        {
            var errors = base.Validate();

            // PHAM_VI: 1 = BHYT thanh toán / 2 = BN tự trả / 3 = QĐ, CA, CY
            if (!string.IsNullOrEmpty(PHAM_VI) && !In(PHAM_VI, "1", "2", "3"))
                errors.Add($"[PHAM_VI] Giá trị không hợp lệ: '{PHAM_VI}' " +
                           "(cần 1=BHYT thanh toán / 2=BN tự trả / 3=QĐ,CA,CY)");

            // TYLE_TT_BH: số nguyên 0-100 (không phải số nguyên đã được bắt ở _NUMERIC)
            if (!string.IsNullOrEmpty(TYLE_TT_BH))
            {
                long v;
                if (ParseHelper.TryPyInt(TYLE_TT_BH, out v) && !(0 <= v && v <= 100))
                    errors.Add($"[TYLE_TT_BH] Ngoài khoảng hợp lệ: '{TYLE_TT_BH}' (cần 0-100)");
            }

            // MUC_HUONG: số thực 0-100, chấp nhận thập phân (VD: 47.5 theo TT mới)
            if (!string.IsNullOrEmpty(MUC_HUONG))
            {
                double? v = ToFloat(MUC_HUONG);
                if (v != null && !(0 <= v.Value && v.Value <= 100))
                    errors.Add($"[MUC_HUONG] Ngoài khoảng hợp lệ: '{MUC_HUONG}' (cần 0-100)");
            }

            // MA_PTTT: 1=Phí dịch vụ / 2=Định suất / 3=DRG
            if (!string.IsNullOrEmpty(MA_PTTT) && !In(MA_PTTT, "1", "2", "3"))
                errors.Add($"[MA_PTTT] Giá trị không hợp lệ: '{MA_PTTT}' " +
                           "(cần 1=Phí DV / 2=Định suất / 3=DRG)");

            // NGUON_CTRA: 1=BHYT / 2=Dự án,viện trợ / 3=CTMTQG / 4=Khác
            if (!string.IsNullOrEmpty(NGUON_CTRA) && !In(NGUON_CTRA, "1", "2", "3", "4"))
                errors.Add($"[NGUON_CTRA] Giá trị không hợp lệ: '{NGUON_CTRA}' " +
                           "(cần 1=BHYT / 2=Dự án,viện trợ / 3=CTMTQG / 4=Khác)");

            // VET_THUONG_TP: chỉ hợp lệ khi = "1" hoặc để trống
            if (!string.IsNullOrEmpty(VET_THUONG_TP) && VET_THUONG_TP != "1")
                errors.Add($"[VET_THUONG_TP] Giá trị không hợp lệ: '{VET_THUONG_TP}' " +
                           "(chỉ ghi '1' nếu là vết thương/bệnh tái phát của thương binh, để trống nếu không)");

            return errors;
        }
    }
}
