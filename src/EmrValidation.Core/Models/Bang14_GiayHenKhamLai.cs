using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using EmrValidation.Core.Helpers;

namespace EmrValidation.Core.Models
{
    /// <summary>Bảng 14. Chỉ tiêu dữ liệu giấy hẹn khám lại.</summary>
    public class Bang14_GiayHenKhamLai : BhytBase
    {
        public string MA_LK { get; set; }
        public string SO_GIAYHEN_KL { get; set; }
        public string MA_CSKCB { get; set; }
        public string HO_TEN { get; set; }
        public string NGAY_SINH { get; set; }
        public string GIOI_TINH { get; set; }
        public string DIA_CHI { get; set; }
        public string MA_THE_BHYT { get; set; }
        public string GT_THE_DEN { get; set; }
        public string NGAY_VAO { get; set; }
        public string NGAY_VAO_NOI_TRU { get; set; }
        public string NGAY_RA { get; set; }
        public string NGAY_HEN_KL { get; set; }
        public string CHAN_DOAN_RV { get; set; }
        public string MA_BENH_CHINH { get; set; }
        public string MA_BENH_KT { get; set; }
        public string MA_BENH_YHCT { get; set; }
        public string MA_DOITUONG_KCB { get; set; }
        public string MA_BAC_SI { get; set; }
        public string MA_TTDV { get; set; }
        public string NGAY_CT { get; set; }
        public string DU_PHONG { get; set; }

        // ---------------- Metadata ----------------
        private static readonly string[] _fieldNames =
        {
            "MA_LK", "SO_GIAYHEN_KL", "MA_CSKCB", "HO_TEN", "NGAY_SINH", "GIOI_TINH",
            "DIA_CHI", "MA_THE_BHYT", "GT_THE_DEN", "NGAY_VAO", "NGAY_VAO_NOI_TRU", "NGAY_RA",
            "NGAY_HEN_KL", "CHAN_DOAN_RV", "MA_BENH_CHINH", "MA_BENH_KT", "MA_BENH_YHCT", "MA_DOITUONG_KCB",
            "MA_BAC_SI", "MA_TTDV", "NGAY_CT", "DU_PHONG",
        };

        private static readonly KeyValuePair<string, int>[] _maxLen =
        {
            new KeyValuePair<string, int>("MA_LK", 100),
            new KeyValuePair<string, int>("SO_GIAYHEN_KL", 50),
            new KeyValuePair<string, int>("MA_CSKCB", 5),
            new KeyValuePair<string, int>("HO_TEN", 255),
            new KeyValuePair<string, int>("NGAY_SINH", 12),
            new KeyValuePair<string, int>("GIOI_TINH", 1),
            new KeyValuePair<string, int>("DIA_CHI", 1024),
            new KeyValuePair<string, int>("NGAY_VAO", 12),
            new KeyValuePair<string, int>("NGAY_VAO_NOI_TRU", 12),
            new KeyValuePair<string, int>("NGAY_RA", 12),
            new KeyValuePair<string, int>("NGAY_HEN_KL", 200),
            new KeyValuePair<string, int>("MA_BENH_CHINH", 7),
            new KeyValuePair<string, int>("MA_BENH_KT", 100),
            new KeyValuePair<string, int>("MA_BENH_YHCT", 255),
            new KeyValuePair<string, int>("MA_DOITUONG_KCB", 50),
            new KeyValuePair<string, int>("MA_BAC_SI", 255),
            new KeyValuePair<string, int>("MA_TTDV", 255),
            new KeyValuePair<string, int>("NGAY_CT", 8),
        };

        private static readonly string[] _numeric =
        {
            "GIOI_TINH",
        };

        private static readonly string[] _date12 =
        {
            "NGAY_SINH", "NGAY_VAO", "NGAY_VAO_NOI_TRU", "NGAY_RA",
        };

        private static readonly string[] _date8 =
        {
            "NGAY_CT",
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
                case "SO_GIAYHEN_KL": return SO_GIAYHEN_KL;
                case "MA_CSKCB": return MA_CSKCB;
                case "HO_TEN": return HO_TEN;
                case "NGAY_SINH": return NGAY_SINH;
                case "GIOI_TINH": return GIOI_TINH;
                case "DIA_CHI": return DIA_CHI;
                case "MA_THE_BHYT": return MA_THE_BHYT;
                case "GT_THE_DEN": return GT_THE_DEN;
                case "NGAY_VAO": return NGAY_VAO;
                case "NGAY_VAO_NOI_TRU": return NGAY_VAO_NOI_TRU;
                case "NGAY_RA": return NGAY_RA;
                case "NGAY_HEN_KL": return NGAY_HEN_KL;
                case "CHAN_DOAN_RV": return CHAN_DOAN_RV;
                case "MA_BENH_CHINH": return MA_BENH_CHINH;
                case "MA_BENH_KT": return MA_BENH_KT;
                case "MA_BENH_YHCT": return MA_BENH_YHCT;
                case "MA_DOITUONG_KCB": return MA_DOITUONG_KCB;
                case "MA_BAC_SI": return MA_BAC_SI;
                case "MA_TTDV": return MA_TTDV;
                case "NGAY_CT": return NGAY_CT;
                case "DU_PHONG": return DU_PHONG;
                default: return null;
            }
        }

        public override bool SetValue(string fieldName, string value)
        {
            switch (fieldName)
            {
                case "MA_LK": MA_LK = value; return true;
                case "SO_GIAYHEN_KL": SO_GIAYHEN_KL = value; return true;
                case "MA_CSKCB": MA_CSKCB = value; return true;
                case "HO_TEN": HO_TEN = value; return true;
                case "NGAY_SINH": NGAY_SINH = value; return true;
                case "GIOI_TINH": GIOI_TINH = value; return true;
                case "DIA_CHI": DIA_CHI = value; return true;
                case "MA_THE_BHYT": MA_THE_BHYT = value; return true;
                case "GT_THE_DEN": GT_THE_DEN = value; return true;
                case "NGAY_VAO": NGAY_VAO = value; return true;
                case "NGAY_VAO_NOI_TRU": NGAY_VAO_NOI_TRU = value; return true;
                case "NGAY_RA": NGAY_RA = value; return true;
                case "NGAY_HEN_KL": NGAY_HEN_KL = value; return true;
                case "CHAN_DOAN_RV": CHAN_DOAN_RV = value; return true;
                case "MA_BENH_CHINH": MA_BENH_CHINH = value; return true;
                case "MA_BENH_KT": MA_BENH_KT = value; return true;
                case "MA_BENH_YHCT": MA_BENH_YHCT = value; return true;
                case "MA_DOITUONG_KCB": MA_DOITUONG_KCB = value; return true;
                case "MA_BAC_SI": MA_BAC_SI = value; return true;
                case "MA_TTDV": MA_TTDV = value; return true;
                case "NGAY_CT": NGAY_CT = value; return true;
                case "DU_PHONG": DU_PHONG = value; return true;
                default: return false;
            }
        }

        private static string Left8(string s)
        {
            s = s ?? "None";
            return s.Length > 8 ? s.Substring(0, 8) : s;
        }

        public override List<string> Validate()
        {
            var errs = base.Validate();

            // 1. MA_LK – bắt buộc
            if (string.IsNullOrEmpty(MA_LK))
                errs.Add("MA_LK: bắt buộc, không được để trống");

            // 3. MA_CSKCB – đúng 5 ký tự
            if (!string.IsNullOrEmpty(MA_CSKCB) && MA_CSKCB.Trim().Length != 5)
                errs.Add($"MA_CSKCB: '{MA_CSKCB}' phải đúng 5 ký tự, " +
                         $"nhận {MA_CSKCB.Trim().Length} ký tự");

            // 6. GIOI_TINH – enum {1=Nam / 2=Nữ / 3=Chưa XĐ}
            long? gt = ToInt(GIOI_TINH);
            if (GIOI_TINH != null && !(gt == 1 || gt == 2 || gt == 3))
                errs.Add($"GIOI_TINH: '{GIOI_TINH}' không hợp lệ " +
                         "(cần 1=Nam / 2=Nữ / 3=Chưa xác định)");

            // Cross-field: NGAY_RA >= NGAY_VAO
            DateTime? dtVao = ParseDate12(NGAY_VAO);
            DateTime? dtRa = ParseDate12(NGAY_RA);

            if (dtVao != null && dtRa != null)
            {
                if (dtRa.Value < dtVao.Value)
                    errs.Add($"NGAY_RA ({NGAY_RA}) không được trước " +
                             $"NGAY_VAO ({NGAY_VAO})");
            }

            // 13. NGAY_HEN_KL – DATE12 (yyyymmddHHMM, không xác định giờ phút thì ghi 0000);
            //     nhiều ngày hẹn phân cách ";", mỗi ngày phải >= NGAY_RA (ngày)
            if (!string.IsNullOrEmpty(NGAY_HEN_KL))
            {
                foreach (string item in ParseHelper.SplitList(NGAY_HEN_KL))
                {
                    DateTime? dtHen = ParseDate12(item);
                    if (dtHen == null)
                    {
                        errs.Add($"NGAY_HEN_KL: '{item}' không phải ngày giờ hợp lệ " +
                                 "(định dạng yyyymmddHHMM, 12 ký tự; không xác định giờ phút thì ghi 0000)");
                        continue;
                    }
                    if (dtRa != null && dtHen.Value.Date < dtRa.Value.Date)
                        errs.Add($"NGAY_HEN_KL ({item}) không được trước " +
                                 $"ngày ra viện NGAY_RA ({Left8(NGAY_RA)})");
                }
            }

            // 15. MA_BENH_CHINH – chỉ 1 mã ICD-10, không chứa ";"
            if (!string.IsNullOrEmpty(MA_BENH_CHINH) && MA_BENH_CHINH.Contains(";"))
                errs.Add($"MA_BENH_CHINH: '{MA_BENH_CHINH}' chỉ được 1 mã ICD-10 " +
                         "(không dùng dấu ';')");

            // 20. MA_BAC_SI – GPHN hoặc GPHN.HD.XXXXX (huy động/điều động)
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
                                     "Bảng 14 chỉ dùng HD (huy động/điều động)");
                        if (maCs.Trim().Length == 0)
                            errs.Add($"MA_BAC_SI: thiếu mã cơ sở KBCB trong '{val}'");
                    }
                }
            }

            // 22. NGAY_CT – DATE8, >= NGAY_RA (ngày)
            DateTime? dtCt = ParseDate8(NGAY_CT);
            if (!string.IsNullOrEmpty(NGAY_CT) && dtCt == null)
                errs.Add($"NGAY_CT: '{NGAY_CT}' không phải ngày hợp lệ " +
                         "(định dạng yyyymmdd)");
            if (dtCt != null && dtRa != null)
            {
                if (dtCt.Value.Date < dtRa.Value.Date)
                    errs.Add($"NGAY_CT ({NGAY_CT}) không được trước " +
                             $"ngày ra viện NGAY_RA ({Left8(NGAY_RA)})");
            }

            return errs;
        }
    }
}