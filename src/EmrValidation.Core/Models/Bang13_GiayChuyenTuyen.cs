using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using EmrValidation.Core.Helpers;

namespace EmrValidation.Core.Models
{
    /// <summary>Bảng 13. Chỉ tiêu dữ liệu giấy chuyển tuyến/chuyển cơ sở KBCB BHYT.</summary>
    public class Bang13_GiayChuyenTuyen : BhytBase
    {
        public string MA_LK { get; set; }
        public string SO_HOSO { get; set; }
        public string SO_CHUYENTUYEN { get; set; }
        public string GIAY_CHUYEN_TUYEN { get; set; }
        public string MA_CSKCB { get; set; }
        public string MA_CSKCB_DI { get; set; }
        public string MA_CSKCB_DEN { get; set; }
        public string HO_TEN { get; set; }
        public string NGAY_SINH { get; set; }
        public string GIOI_TINH { get; set; }
        public string MA_QUOCTICH { get; set; }
        public string MA_DANTOC { get; set; }
        public string MA_NGHE_NGHIEP { get; set; }
        public string DIA_CHI { get; set; }
        public string MA_THE_BHYT { get; set; }
        public string GT_THE_DEN { get; set; }
        public string NGAY_VAO { get; set; }
        public string NGAY_VAO_NOI_TRU { get; set; }
        public string NGAY_RA { get; set; }
        public string DAU_HIEU_LS { get; set; }
        public string CHAN_DOAN_RV { get; set; }
        public string QT_BENHLY { get; set; }
        public string TOMTAT_KQ { get; set; }
        public string PP_DIEUTRI { get; set; }
        public string MA_BENH_CHINH { get; set; }
        public string MA_BENH_KT { get; set; }
        public string MA_BENH_YHCT { get; set; }
        public string TINH_TRANG_CT { get; set; }
        public string MA_LOAI_RV { get; set; }
        public string MA_LYDO_CT { get; set; }
        public string HUONG_DIEU_TRI { get; set; }
        public string PHUONGTIEN_VC { get; set; }
        public string HOTEN_NGUOI_HT { get; set; }
        public string CHUCDANH_NGUOI_HT { get; set; }
        public string MA_BAC_SI { get; set; }
        public string MA_TTDV { get; set; }
        public string DU_PHONG { get; set; }

        // ---------------- Metadata ----------------
        private static readonly string[] _fieldNames =
        {
            "MA_LK", "SO_HOSO", "SO_CHUYENTUYEN", "GIAY_CHUYEN_TUYEN", "MA_CSKCB", "MA_CSKCB_DI",
            "MA_CSKCB_DEN", "HO_TEN", "NGAY_SINH", "GIOI_TINH", "MA_QUOCTICH", "MA_DANTOC",
            "MA_NGHE_NGHIEP", "DIA_CHI", "MA_THE_BHYT", "GT_THE_DEN", "NGAY_VAO", "NGAY_VAO_NOI_TRU",
            "NGAY_RA", "DAU_HIEU_LS", "CHAN_DOAN_RV", "QT_BENHLY", "TOMTAT_KQ", "PP_DIEUTRI",
            "MA_BENH_CHINH", "MA_BENH_KT", "MA_BENH_YHCT", "TINH_TRANG_CT", "MA_LOAI_RV", "MA_LYDO_CT",
            "HUONG_DIEU_TRI", "PHUONGTIEN_VC", "HOTEN_NGUOI_HT", "CHUCDANH_NGUOI_HT", "MA_BAC_SI", "MA_TTDV",
            "DU_PHONG",
        };

        private static readonly KeyValuePair<string, int>[] _maxLen =
        {
            new KeyValuePair<string, int>("MA_LK", 100),
            new KeyValuePair<string, int>("SO_HOSO", 50),
            new KeyValuePair<string, int>("SO_CHUYENTUYEN", 50),
            new KeyValuePair<string, int>("GIAY_CHUYEN_TUYEN", 50),
            new KeyValuePair<string, int>("MA_CSKCB", 5),
            new KeyValuePair<string, int>("MA_CSKCB_DI", 100),
            new KeyValuePair<string, int>("MA_CSKCB_DEN", 5),
            new KeyValuePair<string, int>("HO_TEN", 255),
            new KeyValuePair<string, int>("NGAY_SINH", 12),
            new KeyValuePair<string, int>("GIOI_TINH", 1),
            new KeyValuePair<string, int>("MA_QUOCTICH", 3),
            new KeyValuePair<string, int>("MA_DANTOC", 2),
            new KeyValuePair<string, int>("MA_NGHE_NGHIEP", 5),
            new KeyValuePair<string, int>("DIA_CHI", 1024),
            new KeyValuePair<string, int>("NGAY_VAO", 100),
            new KeyValuePair<string, int>("NGAY_VAO_NOI_TRU", 12),
            new KeyValuePair<string, int>("NGAY_RA", 100),
            new KeyValuePair<string, int>("MA_BENH_CHINH", 7),
            new KeyValuePair<string, int>("MA_BENH_KT", 100),
            new KeyValuePair<string, int>("MA_BENH_YHCT", 255),
            new KeyValuePair<string, int>("MA_LOAI_RV", 1),
            new KeyValuePair<string, int>("MA_LYDO_CT", 1),
            new KeyValuePair<string, int>("PHUONGTIEN_VC", 255),
            new KeyValuePair<string, int>("HOTEN_NGUOI_HT", 255),
            new KeyValuePair<string, int>("CHUCDANH_NGUOI_HT", 255),
            new KeyValuePair<string, int>("MA_BAC_SI", 255),
            new KeyValuePair<string, int>("MA_TTDV", 255),
        };

        private static readonly string[] _numeric =
        {
            "GIOI_TINH", "MA_LOAI_RV", "MA_LYDO_CT",
        };

        private static readonly string[] _date12 =
        {
            "NGAY_SINH", "NGAY_VAO_NOI_TRU",
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
                case "SO_HOSO": return SO_HOSO;
                case "SO_CHUYENTUYEN": return SO_CHUYENTUYEN;
                case "GIAY_CHUYEN_TUYEN": return GIAY_CHUYEN_TUYEN;
                case "MA_CSKCB": return MA_CSKCB;
                case "MA_CSKCB_DI": return MA_CSKCB_DI;
                case "MA_CSKCB_DEN": return MA_CSKCB_DEN;
                case "HO_TEN": return HO_TEN;
                case "NGAY_SINH": return NGAY_SINH;
                case "GIOI_TINH": return GIOI_TINH;
                case "MA_QUOCTICH": return MA_QUOCTICH;
                case "MA_DANTOC": return MA_DANTOC;
                case "MA_NGHE_NGHIEP": return MA_NGHE_NGHIEP;
                case "DIA_CHI": return DIA_CHI;
                case "MA_THE_BHYT": return MA_THE_BHYT;
                case "GT_THE_DEN": return GT_THE_DEN;
                case "NGAY_VAO": return NGAY_VAO;
                case "NGAY_VAO_NOI_TRU": return NGAY_VAO_NOI_TRU;
                case "NGAY_RA": return NGAY_RA;
                case "DAU_HIEU_LS": return DAU_HIEU_LS;
                case "CHAN_DOAN_RV": return CHAN_DOAN_RV;
                case "QT_BENHLY": return QT_BENHLY;
                case "TOMTAT_KQ": return TOMTAT_KQ;
                case "PP_DIEUTRI": return PP_DIEUTRI;
                case "MA_BENH_CHINH": return MA_BENH_CHINH;
                case "MA_BENH_KT": return MA_BENH_KT;
                case "MA_BENH_YHCT": return MA_BENH_YHCT;
                case "TINH_TRANG_CT": return TINH_TRANG_CT;
                case "MA_LOAI_RV": return MA_LOAI_RV;
                case "MA_LYDO_CT": return MA_LYDO_CT;
                case "HUONG_DIEU_TRI": return HUONG_DIEU_TRI;
                case "PHUONGTIEN_VC": return PHUONGTIEN_VC;
                case "HOTEN_NGUOI_HT": return HOTEN_NGUOI_HT;
                case "CHUCDANH_NGUOI_HT": return CHUCDANH_NGUOI_HT;
                case "MA_BAC_SI": return MA_BAC_SI;
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
                case "SO_HOSO": SO_HOSO = value; return true;
                case "SO_CHUYENTUYEN": SO_CHUYENTUYEN = value; return true;
                case "GIAY_CHUYEN_TUYEN": GIAY_CHUYEN_TUYEN = value; return true;
                case "MA_CSKCB": MA_CSKCB = value; return true;
                case "MA_CSKCB_DI": MA_CSKCB_DI = value; return true;
                case "MA_CSKCB_DEN": MA_CSKCB_DEN = value; return true;
                case "HO_TEN": HO_TEN = value; return true;
                case "NGAY_SINH": NGAY_SINH = value; return true;
                case "GIOI_TINH": GIOI_TINH = value; return true;
                case "MA_QUOCTICH": MA_QUOCTICH = value; return true;
                case "MA_DANTOC": MA_DANTOC = value; return true;
                case "MA_NGHE_NGHIEP": MA_NGHE_NGHIEP = value; return true;
                case "DIA_CHI": DIA_CHI = value; return true;
                case "MA_THE_BHYT": MA_THE_BHYT = value; return true;
                case "GT_THE_DEN": GT_THE_DEN = value; return true;
                case "NGAY_VAO": NGAY_VAO = value; return true;
                case "NGAY_VAO_NOI_TRU": NGAY_VAO_NOI_TRU = value; return true;
                case "NGAY_RA": NGAY_RA = value; return true;
                case "DAU_HIEU_LS": DAU_HIEU_LS = value; return true;
                case "CHAN_DOAN_RV": CHAN_DOAN_RV = value; return true;
                case "QT_BENHLY": QT_BENHLY = value; return true;
                case "TOMTAT_KQ": TOMTAT_KQ = value; return true;
                case "PP_DIEUTRI": PP_DIEUTRI = value; return true;
                case "MA_BENH_CHINH": MA_BENH_CHINH = value; return true;
                case "MA_BENH_KT": MA_BENH_KT = value; return true;
                case "MA_BENH_YHCT": MA_BENH_YHCT = value; return true;
                case "TINH_TRANG_CT": TINH_TRANG_CT = value; return true;
                case "MA_LOAI_RV": MA_LOAI_RV = value; return true;
                case "MA_LYDO_CT": MA_LYDO_CT = value; return true;
                case "HUONG_DIEU_TRI": HUONG_DIEU_TRI = value; return true;
                case "PHUONGTIEN_VC": PHUONGTIEN_VC = value; return true;
                case "HOTEN_NGUOI_HT": HOTEN_NGUOI_HT = value; return true;
                case "CHUCDANH_NGUOI_HT": CHUCDANH_NGUOI_HT = value; return true;
                case "MA_BAC_SI": MA_BAC_SI = value; return true;
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

            // 10. GIOI_TINH – enum {1=Nam / 2=Nữ / 3=Không XĐ}
            long? gt = ToInt(GIOI_TINH);
            if (GIOI_TINH != null && !(gt == 1 || gt == 2 || gt == 3))
                errs.Add($"GIOI_TINH: '{GIOI_TINH}' không hợp lệ " +
                         "(cần 1=Nam / 2=Nữ / 3=Chưa xác định)");

            // 6. MA_CSKCB_DI – nhiều cơ sở phân cách ";", mỗi mã đúng 5 ký tự
            if (!string.IsNullOrEmpty(MA_CSKCB_DI))
            {
                string[] list = MA_CSKCB_DI.Split(';');
                for (int idx = 0; idx < list.Length; idx++)
                {
                    string ma = list[idx].Trim();
                    if (ma.Length == 0)
                        errs.Add($"MA_CSKCB_DI: mục {idx + 1} rỗng " +
                                 "(kiểm tra dấu ';' thừa)");
                    else if (ma.Length != 5)
                        errs.Add($"MA_CSKCB_DI: mã '{ma}' (mục {idx + 1}) " +
                                 $"phải đúng 5 ký tự, nhận {ma.Length} ký tự");
                }
            }

            // 31. MA_LOAI_RV – enum {1-5}; Bảng 13 chỉ gửi khi MA_LOAI_RV ∈ {2, 5}
            long? loaiRv = ToInt(MA_LOAI_RV);
            if (MA_LOAI_RV != null)
            {
                if (!(loaiRv == 1 || loaiRv == 2 || loaiRv == 3 || loaiRv == 4 || loaiRv == 5))
                    errs.Add($"MA_LOAI_RV: '{MA_LOAI_RV}' không hợp lệ " +
                             "(cần 1–5)");
                else if (!(loaiRv == 2 || loaiRv == 5))
                    errs.Add($"MA_LOAI_RV: '{MA_LOAI_RV}' – Bảng 13 chỉ trích chuyển " +
                             "khi MA_LOAI_RV ∈ {2 (chuyển viện cùng tuyến), " +
                             "5 (chuyển tuyến trên)}");
            }

            // 32. MA_LYDO_CT – enum {1, 2, 3}
            long? lydo = ToInt(MA_LYDO_CT);
            if (MA_LYDO_CT != null && !(lydo == 1 || lydo == 2 || lydo == 3))
                errs.Add($"MA_LYDO_CT: '{MA_LYDO_CT}' không hợp lệ " +
                         "(cần 1=đủ điều kiện / 2=không đáp ứng khả năng / " +
                         "3=theo yêu cầu người bệnh)");

            // 37. MA_BAC_SI – GPHN hoặc GPHN.HD.XXXXX (huy động/điều động)
            if (!string.IsNullOrEmpty(MA_BAC_SI))
            {
                string val = MA_BAC_SI.Trim();
                if (val.Contains("."))
                {
                    string[] parts = val.Split('.');
                    if (parts.Length != 3)
                    {
                        errs.Add($"MA_BAC_SI: '{val}' sai định dạng. " +
                                 "Huy động/điều động phải là <GPHN>.HD.<MaCsKbcb>");
                    }
                    else
                    {
                        string gphn = parts[0], loai = parts[1], maCs = parts[2];
                        if (gphn.Trim().Length == 0)
                            errs.Add($"MA_BAC_SI: thiếu mã GPHN trong '{val}'");
                        if (loai.ToUpperInvariant() != "HD")
                            errs.Add($"MA_BAC_SI: loại '{loai}' không hợp lệ trong '{val}'. " +
                                     "Bảng 13 chỉ dùng HD (huy động/điều động)");
                        if (maCs.Trim().Length == 0)
                            errs.Add($"MA_BAC_SI: thiếu mã cơ sở KBCB trong '{val}'");
                    }
                }
            }

            return errs;
        }
    }
}
