using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using EmrValidation.Core.Helpers;

namespace EmrValidation.Core.Models
{
    /// <summary>Bảng 1. Chỉ tiêu tổng hợp khám bệnh, chữa bệnh.</summary>
    public class Bang1_TongHop : BhytBase
    {
        public string MA_LK { get; set; }
        public string STT { get; set; }
        public string MA_BN { get; set; }
        public string HO_TEN { get; set; }
        public string SO_CCCD { get; set; }
        public string NGAY_SINH { get; set; }
        public string GIOI_TINH { get; set; }
        public string NHOM_MAU { get; set; }
        public string MA_QUOCTICH { get; set; }
        public string MA_DANTOC { get; set; }
        public string MA_NGHE_NGHIEP { get; set; }
        public string DIA_CHI { get; set; }
        public string MATINH_CU_TRU { get; set; }
        public string MAHUYEN_CU_TRU { get; set; }
        public string MAXA_CU_TRU { get; set; }
        public string DIEN_THOAI { get; set; }
        public string MA_THE_BHYT { get; set; }
        public string MA_DKBD { get; set; }
        public string GT_THE_TU { get; set; }
        public string GT_THE_DEN { get; set; }
        public string NGAY_MIEN_CCT { get; set; }
        public string LY_DO_VV { get; set; }
        public string LY_DO_VNT { get; set; }
        public string MA_LY_DO_VNT { get; set; }
        public string CHAN_DOAN_VAO { get; set; }
        public string CHAN_DOAN_RV { get; set; }
        public string MA_BENH_CHINH { get; set; }
        public string MA_BENH_KT { get; set; }
        public string MA_BENH_KHAC { get; set; }
        public string MA_BENH_YHCT { get; set; }
        public string MA_PTTT_QT { get; set; }
        public string MA_DOITUONG_KCB { get; set; }
        public string MA_NOI_DI { get; set; }
        public string MA_NOI_DEN { get; set; }
        public string MA_TAI_NAN { get; set; }
        public string NGAY_VAO { get; set; }
        public string NGAY_VAO_NOI_TRU { get; set; }
        public string NGAY_RA { get; set; }
        public string GIAY_CHUYEN_TUYEN { get; set; }
        public string SO_NGAY_DTRI { get; set; }
        public string PP_DIEU_TRI { get; set; }
        public string KET_QUA_DTRI { get; set; }
        public string MA_LOAI_RV { get; set; }
        public string GHI_CHU { get; set; }
        public string NGAY_TTOAN { get; set; }
        public string T_THUOC { get; set; }
        public string T_VTYT { get; set; }
        public string T_TONGCHI_BV { get; set; }
        public string T_TONGCHI_BH { get; set; }
        public string T_BNTT { get; set; }
        public string T_BNCCT_LK { get; set; }
        public string T_BNCCT { get; set; }
        public string T_BHTT { get; set; }
        public string T_NGUONKHAC { get; set; }
        public string T_BHTT_GDV { get; set; }
        public string NAM_QT { get; set; }
        public string THANG_QT { get; set; }
        public string MA_LOAI_KCB { get; set; }
        public string MA_KHOA { get; set; }
        public string MA_CSKCB { get; set; }
        public string MA_KHUVUC { get; set; }
        public string CAN_NANG { get; set; }
        public string CAN_NANG_CON { get; set; }
        public string NAM_NAM_LIEN_TUC { get; set; }
        public string NGAY_TAI_KHAM { get; set; }
        public string MA_HSBA { get; set; }
        public string MA_TTDV { get; set; }
        public string DU_PHONG { get; set; }

        // ---------------- Metadata ----------------
        private static readonly string[] _fieldNames =
        {
            "MA_LK", "STT", "MA_BN", "HO_TEN", "SO_CCCD", "NGAY_SINH",
            "GIOI_TINH", "NHOM_MAU", "MA_QUOCTICH", "MA_DANTOC", "MA_NGHE_NGHIEP", "DIA_CHI",
            "MATINH_CU_TRU", "MAHUYEN_CU_TRU", "MAXA_CU_TRU", "DIEN_THOAI", "MA_THE_BHYT", "MA_DKBD",
            "GT_THE_TU", "GT_THE_DEN", "NGAY_MIEN_CCT", "LY_DO_VV", "LY_DO_VNT", "MA_LY_DO_VNT",
            "CHAN_DOAN_VAO", "CHAN_DOAN_RV", "MA_BENH_CHINH", "MA_BENH_KT", "MA_BENH_KHAC", "MA_BENH_YHCT", "MA_PTTT_QT",
            "MA_DOITUONG_KCB", "MA_NOI_DI", "MA_NOI_DEN", "MA_TAI_NAN", "NGAY_VAO", "NGAY_VAO_NOI_TRU",
            "NGAY_RA", "GIAY_CHUYEN_TUYEN", "SO_NGAY_DTRI", "PP_DIEU_TRI", "KET_QUA_DTRI", "MA_LOAI_RV",
            "GHI_CHU", "NGAY_TTOAN", "T_THUOC", "T_VTYT", "T_TONGCHI_BV", "T_TONGCHI_BH",
            "T_BNTT", "T_BNCCT_LK", "T_BNCCT", "T_BHTT", "T_NGUONKHAC", "T_BHTT_GDV", "NAM_QT",
            "THANG_QT", "MA_LOAI_KCB", "MA_KHOA", "MA_CSKCB", "MA_KHUVUC", "CAN_NANG",
            "CAN_NANG_CON", "NAM_NAM_LIEN_TUC", "NGAY_TAI_KHAM", "MA_HSBA", "MA_TTDV", "DU_PHONG",
        };

        private static readonly KeyValuePair<string, int>[] _maxLen =
        {
            new KeyValuePair<string, int>("MA_LK", 100),
            new KeyValuePair<string, int>("STT", 10),
            new KeyValuePair<string, int>("MA_BN", 100),
            new KeyValuePair<string, int>("HO_TEN", 255),
            new KeyValuePair<string, int>("NGAY_SINH", 12),
            new KeyValuePair<string, int>("GIOI_TINH", 1),
            new KeyValuePair<string, int>("NHOM_MAU", 5),
            new KeyValuePair<string, int>("MA_QUOCTICH", 3),
            new KeyValuePair<string, int>("MA_DANTOC", 2),
            new KeyValuePair<string, int>("MA_NGHE_NGHIEP", 2),
            new KeyValuePair<string, int>("DIA_CHI", 1024),
            new KeyValuePair<string, int>("MATINH_CU_TRU", 3),
            new KeyValuePair<string, int>("MAHUYEN_CU_TRU", 3),
            new KeyValuePair<string, int>("MAXA_CU_TRU", 5),
            new KeyValuePair<string, int>("DIEN_THOAI", 15),
            new KeyValuePair<string, int>("MA_LY_DO_VNT", 5),
            new KeyValuePair<string, int>("MA_BENH_CHINH", 7),
            new KeyValuePair<string, int>("MA_BENH_KT", 100),
            new KeyValuePair<string, int>("MA_BENH_YHCT", 150),
            new KeyValuePair<string, int>("MA_PTTT_QT", 125),
            new KeyValuePair<string, int>("MA_DOITUONG_KCB", 50),
            new KeyValuePair<string, int>("MA_NOI_DI", 5),
            new KeyValuePair<string, int>("MA_NOI_DEN", 5),
            new KeyValuePair<string, int>("MA_TAI_NAN", 1),
            new KeyValuePair<string, int>("NGAY_VAO", 12),
            new KeyValuePair<string, int>("NGAY_VAO_NOI_TRU", 12),
            new KeyValuePair<string, int>("NGAY_RA", 12),
            new KeyValuePair<string, int>("GIAY_CHUYEN_TUYEN", 50),
            new KeyValuePair<string, int>("SO_NGAY_DTRI", 3),
            new KeyValuePair<string, int>("KET_QUA_DTRI", 1),
            new KeyValuePair<string, int>("MA_LOAI_RV", 1),
            new KeyValuePair<string, int>("NGAY_TTOAN", 12),
            new KeyValuePair<string, int>("T_THUOC", 15),
            new KeyValuePair<string, int>("T_VTYT", 15),
            new KeyValuePair<string, int>("T_TONGCHI_BV", 15),
            new KeyValuePair<string, int>("T_TONGCHI_BH", 15),
            new KeyValuePair<string, int>("T_BNTT", 15),
            new KeyValuePair<string, int>("T_BNCCT_LK", 15),
            new KeyValuePair<string, int>("T_BNCCT", 15),
            new KeyValuePair<string, int>("T_BHTT", 15),
            new KeyValuePair<string, int>("T_NGUONKHAC", 15),
            new KeyValuePair<string, int>("T_BHTT_GDV", 15),
            new KeyValuePair<string, int>("NAM_QT", 4),
            new KeyValuePair<string, int>("THANG_QT", 2),
            new KeyValuePair<string, int>("MA_LOAI_KCB", 10),
            new KeyValuePair<string, int>("MA_KHOA", 50),
            new KeyValuePair<string, int>("MA_CSKCB", 5),
            new KeyValuePair<string, int>("MA_KHUVUC", 2),
            new KeyValuePair<string, int>("CAN_NANG", 6),
            new KeyValuePair<string, int>("CAN_NANG_CON", 100),
            new KeyValuePair<string, int>("NAM_NAM_LIEN_TUC", 8),
            new KeyValuePair<string, int>("NGAY_TAI_KHAM", 50),
            new KeyValuePair<string, int>("MA_HSBA", 100),
            new KeyValuePair<string, int>("MA_TTDV", 255),
        };

        private static readonly string[] _numeric =
        {
            "STT", "MA_TAI_NAN", "SO_NGAY_DTRI", "KET_QUA_DTRI", "MA_LOAI_RV", "T_THUOC",
            "T_VTYT", "T_TONGCHI_BV", "T_TONGCHI_BH", "T_BNTT", "T_BNCCT_LK", "T_BNCCT", "T_BHTT",
            "T_NGUONKHAC", "T_BHTT_GDV", "NAM_QT", "THANG_QT",
        };

        private static readonly string[] _date12 =
        {
            "NGAY_SINH", "NGAY_VAO", "NGAY_VAO_NOI_TRU", "NGAY_RA", "NGAY_TTOAN",
        };

        private static readonly string[] _date8 =
        {
            "NAM_NAM_LIEN_TUC", "NGAY_TAI_KHAM",
        };

        private static readonly string[] _required =
        {
            "MA_LK",
        };


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
                case "MA_BN": return MA_BN;
                case "HO_TEN": return HO_TEN;
                case "SO_CCCD": return SO_CCCD;
                case "NGAY_SINH": return NGAY_SINH;
                case "GIOI_TINH": return GIOI_TINH;
                case "NHOM_MAU": return NHOM_MAU;
                case "MA_QUOCTICH": return MA_QUOCTICH;
                case "MA_DANTOC": return MA_DANTOC;
                case "MA_NGHE_NGHIEP": return MA_NGHE_NGHIEP;
                case "DIA_CHI": return DIA_CHI;
                case "MATINH_CU_TRU": return MATINH_CU_TRU;
                case "MAHUYEN_CU_TRU": return MAHUYEN_CU_TRU;
                case "MAXA_CU_TRU": return MAXA_CU_TRU;
                case "DIEN_THOAI": return DIEN_THOAI;
                case "MA_THE_BHYT": return MA_THE_BHYT;
                case "MA_DKBD": return MA_DKBD;
                case "GT_THE_TU": return GT_THE_TU;
                case "GT_THE_DEN": return GT_THE_DEN;
                case "NGAY_MIEN_CCT": return NGAY_MIEN_CCT;
                case "LY_DO_VV": return LY_DO_VV;
                case "LY_DO_VNT": return LY_DO_VNT;
                case "MA_LY_DO_VNT": return MA_LY_DO_VNT;
                case "CHAN_DOAN_VAO": return CHAN_DOAN_VAO;
                case "CHAN_DOAN_RV": return CHAN_DOAN_RV;
                case "MA_BENH_CHINH": return MA_BENH_CHINH;
                case "MA_BENH_KT": return MA_BENH_KT;
                case "MA_BENH_KHAC": return MA_BENH_KHAC;
                case "MA_BENH_YHCT": return MA_BENH_YHCT;
                case "MA_PTTT_QT": return MA_PTTT_QT;
                case "MA_DOITUONG_KCB": return MA_DOITUONG_KCB;
                case "MA_NOI_DI": return MA_NOI_DI;
                case "MA_NOI_DEN": return MA_NOI_DEN;
                case "MA_TAI_NAN": return MA_TAI_NAN;
                case "NGAY_VAO": return NGAY_VAO;
                case "NGAY_VAO_NOI_TRU": return NGAY_VAO_NOI_TRU;
                case "NGAY_RA": return NGAY_RA;
                case "GIAY_CHUYEN_TUYEN": return GIAY_CHUYEN_TUYEN;
                case "SO_NGAY_DTRI": return SO_NGAY_DTRI;
                case "PP_DIEU_TRI": return PP_DIEU_TRI;
                case "KET_QUA_DTRI": return KET_QUA_DTRI;
                case "MA_LOAI_RV": return MA_LOAI_RV;
                case "GHI_CHU": return GHI_CHU;
                case "NGAY_TTOAN": return NGAY_TTOAN;
                case "T_THUOC": return T_THUOC;
                case "T_VTYT": return T_VTYT;
                case "T_TONGCHI_BV": return T_TONGCHI_BV;
                case "T_TONGCHI_BH": return T_TONGCHI_BH;
                case "T_BNTT": return T_BNTT;
                case "T_BNCCT_LK": return T_BNCCT_LK;
                case "T_BNCCT": return T_BNCCT;
                case "T_BHTT": return T_BHTT;
                case "T_NGUONKHAC": return T_NGUONKHAC;
                case "T_BHTT_GDV": return T_BHTT_GDV;
                case "NAM_QT": return NAM_QT;
                case "THANG_QT": return THANG_QT;
                case "MA_LOAI_KCB": return MA_LOAI_KCB;
                case "MA_KHOA": return MA_KHOA;
                case "MA_CSKCB": return MA_CSKCB;
                case "MA_KHUVUC": return MA_KHUVUC;
                case "CAN_NANG": return CAN_NANG;
                case "CAN_NANG_CON": return CAN_NANG_CON;
                case "NAM_NAM_LIEN_TUC": return NAM_NAM_LIEN_TUC;
                case "NGAY_TAI_KHAM": return NGAY_TAI_KHAM;
                case "MA_HSBA": return MA_HSBA;
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
                case "STT": STT = value; return true;
                case "MA_BN": MA_BN = value; return true;
                case "HO_TEN": HO_TEN = value; return true;
                case "SO_CCCD": SO_CCCD = value; return true;
                case "NGAY_SINH": NGAY_SINH = value; return true;
                case "GIOI_TINH": GIOI_TINH = value; return true;
                case "NHOM_MAU": NHOM_MAU = value; return true;
                case "MA_QUOCTICH": MA_QUOCTICH = value; return true;
                case "MA_DANTOC": MA_DANTOC = value; return true;
                case "MA_NGHE_NGHIEP": MA_NGHE_NGHIEP = value; return true;
                case "DIA_CHI": DIA_CHI = value; return true;
                case "MATINH_CU_TRU": MATINH_CU_TRU = value; return true;
                case "MAHUYEN_CU_TRU": MAHUYEN_CU_TRU = value; return true;
                case "MAXA_CU_TRU": MAXA_CU_TRU = value; return true;
                case "DIEN_THOAI": DIEN_THOAI = value; return true;
                case "MA_THE_BHYT": MA_THE_BHYT = value; return true;
                case "MA_DKBD": MA_DKBD = value; return true;
                case "GT_THE_TU": GT_THE_TU = value; return true;
                case "GT_THE_DEN": GT_THE_DEN = value; return true;
                case "NGAY_MIEN_CCT": NGAY_MIEN_CCT = value; return true;
                case "LY_DO_VV": LY_DO_VV = value; return true;
                case "LY_DO_VNT": LY_DO_VNT = value; return true;
                case "MA_LY_DO_VNT": MA_LY_DO_VNT = value; return true;
                case "CHAN_DOAN_VAO": CHAN_DOAN_VAO = value; return true;
                case "CHAN_DOAN_RV": CHAN_DOAN_RV = value; return true;
                case "MA_BENH_CHINH": MA_BENH_CHINH = value; return true;
                case "MA_BENH_KT": MA_BENH_KT = value; return true;
                case "MA_BENH_KHAC": MA_BENH_KHAC = value; return true;
                case "MA_BENH_YHCT": MA_BENH_YHCT = value; return true;
                case "MA_PTTT_QT": MA_PTTT_QT = value; return true;
                case "MA_DOITUONG_KCB": MA_DOITUONG_KCB = value; return true;
                case "MA_NOI_DI": MA_NOI_DI = value; return true;
                case "MA_NOI_DEN": MA_NOI_DEN = value; return true;
                case "MA_TAI_NAN": MA_TAI_NAN = value; return true;
                case "NGAY_VAO": NGAY_VAO = value; return true;
                case "NGAY_VAO_NOI_TRU": NGAY_VAO_NOI_TRU = value; return true;
                case "NGAY_RA": NGAY_RA = value; return true;
                case "GIAY_CHUYEN_TUYEN": GIAY_CHUYEN_TUYEN = value; return true;
                case "SO_NGAY_DTRI": SO_NGAY_DTRI = value; return true;
                case "PP_DIEU_TRI": PP_DIEU_TRI = value; return true;
                case "KET_QUA_DTRI": KET_QUA_DTRI = value; return true;
                case "MA_LOAI_RV": MA_LOAI_RV = value; return true;
                case "GHI_CHU": GHI_CHU = value; return true;
                case "NGAY_TTOAN": NGAY_TTOAN = value; return true;
                case "T_THUOC": T_THUOC = value; return true;
                case "T_VTYT": T_VTYT = value; return true;
                case "T_TONGCHI_BV": T_TONGCHI_BV = value; return true;
                case "T_TONGCHI_BH": T_TONGCHI_BH = value; return true;
                case "T_BNTT": T_BNTT = value; return true;
                case "T_BNCCT_LK": T_BNCCT_LK = value; return true;
                case "T_BNCCT": T_BNCCT = value; return true;
                case "T_BHTT": T_BHTT = value; return true;
                case "T_NGUONKHAC": T_NGUONKHAC = value; return true;
                case "T_BHTT_GDV": T_BHTT_GDV = value; return true;
                case "NAM_QT": NAM_QT = value; return true;
                case "THANG_QT": THANG_QT = value; return true;
                case "MA_LOAI_KCB": MA_LOAI_KCB = value; return true;
                case "MA_KHOA": MA_KHOA = value; return true;
                case "MA_CSKCB": MA_CSKCB = value; return true;
                case "MA_KHUVUC": MA_KHUVUC = value; return true;
                case "CAN_NANG": CAN_NANG = value; return true;
                case "CAN_NANG_CON": CAN_NANG_CON = value; return true;
                case "NAM_NAM_LIEN_TUC": NAM_NAM_LIEN_TUC = value; return true;
                case "NGAY_TAI_KHAM": NGAY_TAI_KHAM = value; return true;
                case "MA_HSBA": MA_HSBA = value; return true;
                case "MA_TTDV": MA_TTDV = value; return true;
                case "DU_PHONG": DU_PHONG = value; return true;
                default: return false;
            }
        }

        public override List<string> Validate()
        {
            var errors = base.Validate();

            // GIOI_TINH: 1/2/3
            if (!string.IsNullOrEmpty(GIOI_TINH) && !In(GIOI_TINH, "1", "2", "3"))
                errors.Add($"[GIOI_TINH] Giá trị không hợp lệ: '{GIOI_TINH}' (cần 1/2/3)");

            // KET_QUA_DTRI: 1-8 (QĐ 4750 bổ sung mã 8: Tử vong ngoại viện)
            if (!string.IsNullOrEmpty(KET_QUA_DTRI) && !In(KET_QUA_DTRI, "1", "2", "3", "4", "5", "6", "7", "8"))
                errors.Add($"[KET_QUA_DTRI] Giá trị không hợp lệ: '{KET_QUA_DTRI}' " +
                           "(cần 1-8; mã 8 = Tử vong ngoại viện theo QĐ 4750)");

            // MA_LOAI_RV: 1-5
            if (!string.IsNullOrEmpty(MA_LOAI_RV) && !In(MA_LOAI_RV, "1", "2", "3", "4", "5"))
                errors.Add($"[MA_LOAI_RV] Giá trị không hợp lệ: '{MA_LOAI_RV}' (cần 1-5)");

            // NGAY_MIEN_CCT: chấp nhận cả yyyymmdd (8 số) lẫn yyyymmddHHMM (12 số)
            if (!string.IsNullOrEmpty(NGAY_MIEN_CCT))
            {
                string val = NGAY_MIEN_CCT.Trim();
                if (!(Rx.Full(@"\d{8}", val) || Rx.Full(@"\d{12}", val)))
                    errors.Add("[NGAY_MIEN_CCT] Sai định dạng – cần yyyymmdd (8 số) hoặc " +
                               $"yyyymmddHHMM (12 số), nhận: '{val}'");
            }

            // MA_BENH_CHINH: chỉ 1 mã ICD-10
            if (!string.IsNullOrEmpty(MA_BENH_CHINH) && MA_BENH_CHINH.Contains(";"))
                errors.Add($"[MA_BENH_CHINH] Chỉ được 1 mã ICD-10, nhận: '{MA_BENH_CHINH}'");

            // MA_BENH_KT: tối đa 12 mã bệnh kèm theo (phân cách ";")
            if (!string.IsNullOrEmpty(MA_BENH_KT))
            {
                int soMaKt = 0;
                foreach (string ma in ParseHelper.SplitList(MA_BENH_KT))
                    if (ma.Length > 0) soMaKt++;
                if (soMaKt > 12)
                    errors.Add($"[MA_BENH_KT] Tối đa 12 mã bệnh kèm theo, nhận: {soMaKt} mã");
            }

            // MA_LOAI_KCB: mỗi mã 2 ký tự số (01, 02, ... 09); nhiều mã phân cách ";" (VD: 01;03)
            if (!string.IsNullOrEmpty(MA_LOAI_KCB))
            {
                foreach (string loai in ParseHelper.SplitList(MA_LOAI_KCB))
                {
                    if (!Rx.Full(@"\d{2}", loai))
                    {
                        errors.Add($"[MA_LOAI_KCB] Mỗi mã cần 2 ký tự số (VD: 01, 05; nhiều mã: 01;03), nhận: '{MA_LOAI_KCB}'");
                        break;
                    }
                }
            }

            // GT_THE_TU, GT_THE_DEN: mỗi giá trị DATE8 (yyyymmdd); nhiều thẻ phân cách ";"
            foreach (string fname in new[] { "GT_THE_TU", "GT_THE_DEN" })
            {
                string gt = GetValue(fname);
                if (string.IsNullOrEmpty(gt)) continue;
                foreach (string item in ParseHelper.SplitList(gt))
                {
                    if (item.Length > 0 && (item.Length != 8 || !ParseHelper.IsDigits(item)))
                        errors.Add($"[{fname}] '{item}' sai định dạng DATE8, phải là yyyymmdd (8 chữ số; nhiều thẻ cách nhau ';')");
                }
            }

            // MA_CSKCB: đúng 5 ký tự
            if (!string.IsNullOrEmpty(MA_CSKCB) && MA_CSKCB.Length != 5)
                errors.Add($"[MA_CSKCB] Cần 5 ký tự, nhận: '{MA_CSKCB}'");

            // MA_KHUVUC: chỉ chấp nhận K1 / K2 / K3
            if (!string.IsNullOrEmpty(MA_KHUVUC) && !In(MA_KHUVUC, "K1", "K2", "K3"))
                errors.Add($"[MA_KHUVUC] Giá trị không hợp lệ: '{MA_KHUVUC}' (cần K1/K2/K3)");

            // _REQUIRED
            foreach (string name in FieldNamesCore)
            {
                string valStr = (GetValue(name) ?? "").Trim();
                if (IsRequired(name) && valStr.Length == 0)
                    errors.Add($"[{name}] Bắt buộc nhập");
            }

            return errors;
        }
    }
}