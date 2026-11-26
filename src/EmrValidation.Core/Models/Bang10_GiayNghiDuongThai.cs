using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using EmrValidation.Core.Helpers;

namespace EmrValidation.Core.Models
{
    /// <summary>Bảng 10. Chỉ tiêu dữ liệu giấy chứng nhận nghỉ dưỡng thai.</summary>
    public class Bang10_GiayNghiDuongThai : BhytBase
    {
        public string MA_LK { get; set; }
        public string SO_SERI { get; set; }
        public string SO_CT { get; set; }
        public string SO_NGAY { get; set; }
        public string DON_VI { get; set; }
        public string CHAN_DOAN_RV { get; set; }
        public string TU_NGAY { get; set; }
        public string DEN_NGAY { get; set; }
        public string MA_TTDV { get; set; }
        public string TEN_BS { get; set; }
        public string MA_BS { get; set; }
        public string NGAY_CT { get; set; }
        public string DU_PHONG { get; set; }

        // ---------------- Metadata ----------------
        private static readonly string[] _fieldNames =
        {
            "MA_LK", "SO_SERI", "SO_CT", "SO_NGAY", "DON_VI", "CHAN_DOAN_RV",
            "TU_NGAY", "DEN_NGAY", "MA_TTDV", "TEN_BS", "MA_BS", "NGAY_CT",
            "DU_PHONG",
        };

        private static readonly KeyValuePair<string, int>[] _maxLen =
        {
            new KeyValuePair<string, int>("MA_LK", 100),
            new KeyValuePair<string, int>("SO_SERI", 200),
            new KeyValuePair<string, int>("SO_CT", 200),
            new KeyValuePair<string, int>("SO_NGAY", 3),
            new KeyValuePair<string, int>("DON_VI", 1024),
            new KeyValuePair<string, int>("TU_NGAY", 8),
            new KeyValuePair<string, int>("DEN_NGAY", 8),
            new KeyValuePair<string, int>("MA_TTDV", 255),
            new KeyValuePair<string, int>("TEN_BS", 255),
            new KeyValuePair<string, int>("MA_BS", 255),
            new KeyValuePair<string, int>("NGAY_CT", 8),
        };

        private static readonly string[] _numeric =
        {
            "SO_NGAY",
        };

        private static readonly string[] _date12 = new string[0];

        private static readonly string[] _date8 =
        {
            "TU_NGAY", "DEN_NGAY", "NGAY_CT",
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
                case "SO_SERI": return SO_SERI;
                case "SO_CT": return SO_CT;
                case "SO_NGAY": return SO_NGAY;
                case "DON_VI": return DON_VI;
                case "CHAN_DOAN_RV": return CHAN_DOAN_RV;
                case "TU_NGAY": return TU_NGAY;
                case "DEN_NGAY": return DEN_NGAY;
                case "MA_TTDV": return MA_TTDV;
                case "TEN_BS": return TEN_BS;
                case "MA_BS": return MA_BS;
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
                case "SO_SERI": SO_SERI = value; return true;
                case "SO_CT": SO_CT = value; return true;
                case "SO_NGAY": SO_NGAY = value; return true;
                case "DON_VI": DON_VI = value; return true;
                case "CHAN_DOAN_RV": CHAN_DOAN_RV = value; return true;
                case "TU_NGAY": TU_NGAY = value; return true;
                case "DEN_NGAY": DEN_NGAY = value; return true;
                case "MA_TTDV": MA_TTDV = value; return true;
                case "TEN_BS": TEN_BS = value; return true;
                case "MA_BS": MA_BS = value; return true;
                case "NGAY_CT": NGAY_CT = value; return true;
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

            // 4. SO_NGAY – số nguyên dương
            long? soNgay = ToInt(SO_NGAY);
            if (SO_NGAY != null && soNgay != null && soNgay < 1)
                errs.Add($"SO_NGAY: '{SO_NGAY}' phải là số nguyên dương >= 1");

            // 6. CHAN_DOAN_RV – QĐ 4750: bắt buộc chứa cụm từ "dưỡng thai"
            if (!string.IsNullOrEmpty(CHAN_DOAN_RV))
            {
                if (!CHAN_DOAN_RV.ToLowerInvariant().Contains("dưỡng thai"))
                    errs.Add($"CHAN_DOAN_RV: '{CHAN_DOAN_RV}' phải chứa cụm từ " +
                             "\"dưỡng thai\" theo QĐ 4750/QĐ-BYT");
            }
            else
            {
                errs.Add("CHAN_DOAN_RV: bắt buộc, không được để trống");
            }

            // 7-8. TU_NGAY, DEN_NGAY – parse + DEN_NGAY >= TU_NGAY
            DateTime? dtTu = ParseDate8(TU_NGAY);
            DateTime? dtDen = ParseDate8(DEN_NGAY);

            if (!string.IsNullOrEmpty(TU_NGAY) && dtTu == null)
                errs.Add($"TU_NGAY: '{TU_NGAY}' không phải ngày hợp lệ " +
                         "(định dạng yyyymmdd)");
            if (!string.IsNullOrEmpty(DEN_NGAY) && dtDen == null)
                errs.Add($"DEN_NGAY: '{DEN_NGAY}' không phải ngày hợp lệ " +
                         "(định dạng yyyymmdd)");
            if (dtTu != null && dtDen != null)
            {
                if (dtDen.Value < dtTu.Value)
                    errs.Add($"DEN_NGAY ({DEN_NGAY}) không được trước " +
                             $"TU_NGAY ({TU_NGAY})");

                // Cross-field: SO_NGAY = (DEN_NGAY - TU_NGAY).days + 1
                if (soNgay != null && soNgay >= 1)
                {
                    long expected = (long)(dtDen.Value.Date - dtTu.Value.Date).TotalDays + 1;
                    if (expected != soNgay)
                        errs.Add($"SO_NGAY: kỳ vọng {expected} ngày " +
                                 "(DEN_NGAY - TU_NGAY + 1, tính cả 2 đầu), " +
                                 $"thực tế {soNgay}");
                }
            }

            // 12. NGAY_CT – DATE8, >= TU_NGAY
            DateTime? dtCt = ParseDate8(NGAY_CT);
            if (!string.IsNullOrEmpty(NGAY_CT) && dtCt == null)
                errs.Add($"NGAY_CT: '{NGAY_CT}' không phải ngày hợp lệ " +
                         "(định dạng yyyymmdd)");
            if (dtCt != null && dtTu != null)
            {
                if (dtCt.Value.Date < dtTu.Value.Date)
                    errs.Add($"NGAY_CT ({NGAY_CT}) không được trước " +
                             $"ngày bắt đầu nghỉ TU_NGAY ({TU_NGAY})");
            }

            return errs;
        }
    }
}
