using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using EmrValidation.Core.Helpers;

namespace EmrValidation.Core.Models
{
    /// <summary>Bảng 11. Chỉ tiêu dữ liệu giấy chứng nhận nghỉ việc hưởng BHXH.</summary>
    public class Bang11_GiayNghiHuongBHXH : BhytBase
    {
        public string MA_LK { get; set; }
        public string SO_CT { get; set; }
        public string SO_SERI { get; set; }
        public string SO_KCB { get; set; }
        public string DON_VI { get; set; }
        public string MA_BHXH { get; set; }
        public string MA_THE_BHYT { get; set; }
        public string CHAN_DOAN_RV { get; set; }
        public string PP_DIEUTRI { get; set; }
        public string MA_DINH_CHI_THAI { get; set; }
        public string NGUYENNHAN_DINHCHI { get; set; }
        public string TUOI_THAI { get; set; }
        public string SO_NGAY_NGHI { get; set; }
        public string TU_NGAY { get; set; }
        public string DEN_NGAY { get; set; }
        public string HO_TEN_CHA { get; set; }
        public string HO_TEN_ME { get; set; }
        public string MA_TTDV { get; set; }
        public string MA_BS { get; set; }
        public string NGAY_CT { get; set; }
        public string MA_THE_TAM { get; set; }
        public string MAU_SO { get; set; }
        public string DU_PHONG { get; set; }

        // ---------------- Metadata ----------------
        private static readonly string[] _fieldNames =
        {
            "MA_LK", "SO_CT", "SO_SERI", "SO_KCB", "DON_VI", "MA_BHXH",
            "MA_THE_BHYT", "CHAN_DOAN_RV", "PP_DIEUTRI", "MA_DINH_CHI_THAI", "NGUYENNHAN_DINHCHI", "TUOI_THAI",
            "SO_NGAY_NGHI", "TU_NGAY", "DEN_NGAY", "HO_TEN_CHA", "HO_TEN_ME", "MA_TTDV",
            "MA_BS", "NGAY_CT", "MA_THE_TAM", "MAU_SO", "DU_PHONG",
        };

        private static readonly KeyValuePair<string, int>[] _maxLen =
        {
            new KeyValuePair<string, int>("MA_LK", 100),
            new KeyValuePair<string, int>("SO_CT", 200),
            new KeyValuePair<string, int>("SO_SERI", 200),
            new KeyValuePair<string, int>("SO_KCB", 200),
            new KeyValuePair<string, int>("DON_VI", 1024),
            new KeyValuePair<string, int>("MA_DINH_CHI_THAI", 1),
            new KeyValuePair<string, int>("TUOI_THAI", 2),
            new KeyValuePair<string, int>("SO_NGAY_NGHI", 3),
            new KeyValuePair<string, int>("TU_NGAY", 8),
            new KeyValuePair<string, int>("DEN_NGAY", 8),
            new KeyValuePair<string, int>("HO_TEN_CHA", 255),
            new KeyValuePair<string, int>("HO_TEN_ME", 255),
            new KeyValuePair<string, int>("MA_TTDV", 255),
            new KeyValuePair<string, int>("MA_BS", 255),
            new KeyValuePair<string, int>("NGAY_CT", 8),
            new KeyValuePair<string, int>("MAU_SO", 5),
        };

        private static readonly string[] _numeric =
        {
            "MA_DINH_CHI_THAI", "TUOI_THAI", "SO_NGAY_NGHI",
        };

        private static readonly string[] _date12 = new string[0];

        private static readonly string[] _date8 =
        {
            "TU_NGAY", "DEN_NGAY", "NGAY_CT",
        };

        private static readonly string[] _required = new string[0];


        private static readonly HashSet<string> _PP_DIEUTRI_VALID = new HashSet<string>
        {
            "hoá trị",
            "hóa trị",
            "ngoại khoa",
            "nội khoa",
            "xạ trị",
            "xạ trị + nội khoa",
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
                case "SO_CT": return SO_CT;
                case "SO_SERI": return SO_SERI;
                case "SO_KCB": return SO_KCB;
                case "DON_VI": return DON_VI;
                case "MA_BHXH": return MA_BHXH;
                case "MA_THE_BHYT": return MA_THE_BHYT;
                case "CHAN_DOAN_RV": return CHAN_DOAN_RV;
                case "PP_DIEUTRI": return PP_DIEUTRI;
                case "MA_DINH_CHI_THAI": return MA_DINH_CHI_THAI;
                case "NGUYENNHAN_DINHCHI": return NGUYENNHAN_DINHCHI;
                case "TUOI_THAI": return TUOI_THAI;
                case "SO_NGAY_NGHI": return SO_NGAY_NGHI;
                case "TU_NGAY": return TU_NGAY;
                case "DEN_NGAY": return DEN_NGAY;
                case "HO_TEN_CHA": return HO_TEN_CHA;
                case "HO_TEN_ME": return HO_TEN_ME;
                case "MA_TTDV": return MA_TTDV;
                case "MA_BS": return MA_BS;
                case "NGAY_CT": return NGAY_CT;
                case "MA_THE_TAM": return MA_THE_TAM;
                case "MAU_SO": return MAU_SO;
                case "DU_PHONG": return DU_PHONG;
                default: return null;
            }
        }

        public override bool SetValue(string fieldName, string value)
        {
            switch (fieldName)
            {
                case "MA_LK": MA_LK = value; return true;
                case "SO_CT": SO_CT = value; return true;
                case "SO_SERI": SO_SERI = value; return true;
                case "SO_KCB": SO_KCB = value; return true;
                case "DON_VI": DON_VI = value; return true;
                case "MA_BHXH": MA_BHXH = value; return true;
                case "MA_THE_BHYT": MA_THE_BHYT = value; return true;
                case "CHAN_DOAN_RV": CHAN_DOAN_RV = value; return true;
                case "PP_DIEUTRI": PP_DIEUTRI = value; return true;
                case "MA_DINH_CHI_THAI": MA_DINH_CHI_THAI = value; return true;
                case "NGUYENNHAN_DINHCHI": NGUYENNHAN_DINHCHI = value; return true;
                case "TUOI_THAI": TUOI_THAI = value; return true;
                case "SO_NGAY_NGHI": SO_NGAY_NGHI = value; return true;
                case "TU_NGAY": TU_NGAY = value; return true;
                case "DEN_NGAY": DEN_NGAY = value; return true;
                case "HO_TEN_CHA": HO_TEN_CHA = value; return true;
                case "HO_TEN_ME": HO_TEN_ME = value; return true;
                case "MA_TTDV": MA_TTDV = value; return true;
                case "MA_BS": MA_BS = value; return true;
                case "NGAY_CT": NGAY_CT = value; return true;
                case "MA_THE_TAM": MA_THE_TAM = value; return true;
                case "MAU_SO": MAU_SO = value; return true;
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

            // 8. CHAN_DOAN_RV – bắt buộc (chỉ kiểm tra không trống)
            if (string.IsNullOrEmpty(CHAN_DOAN_RV))
                errs.Add("CHAN_DOAN_RV: bắt buộc, không được để trống");

            // 9. PP_DIEUTRI – QĐ 4750: phải là 1 trong 5 giá trị chuẩn
            if (!string.IsNullOrEmpty(PP_DIEUTRI))
            {
                string ppNorm = PP_DIEUTRI.Trim().ToLowerInvariant();
                if (!_PP_DIEUTRI_VALID.Contains(ppNorm))
                    errs.Add($"PP_DIEUTRI: '{PP_DIEUTRI}' không hợp lệ theo QĐ 4750. " +
                             "Chỉ chấp nhận: nội khoa / ngoại khoa / xạ trị / " +
                             "hoá trị / xạ trị + nội khoa");
            }

            // 10. MA_DINH_CHI_THAI – enum {0, 1}
            long? maDct = ToInt(MA_DINH_CHI_THAI);
            if (MA_DINH_CHI_THAI != null && !(maDct == 0 || maDct == 1))
                errs.Add($"MA_DINH_CHI_THAI: '{MA_DINH_CHI_THAI}' không hợp lệ " +
                         "(cần 0=không đình chỉ / 1=đình chỉ thai nghén)");

            // Nhóm đình chỉ thai: MA_DINH_CHI_THAI = 1 → NGUYENNHAN_DINHCHI và TUOI_THAI bắt buộc
            if (maDct == 1)
            {
                if (string.IsNullOrEmpty(NGUYENNHAN_DINHCHI))
                    errs.Add("NGUYENNHAN_DINHCHI: bắt buộc khi MA_DINH_CHI_THAI = 1");
                if (string.IsNullOrEmpty(TUOI_THAI))
                    errs.Add("TUOI_THAI: bắt buộc khi MA_DINH_CHI_THAI = 1");
            }

            // 12. TUOI_THAI – 1–42 tuần
            long? tuoiThai = ToInt(TUOI_THAI);
            if (TUOI_THAI != null && tuoiThai != null)
            {
                if (!(1 <= tuoiThai && tuoiThai <= 42))
                    errs.Add($"TUOI_THAI: '{TUOI_THAI}' không hợp lệ, " +
                             "tuổi thai phải trong khoảng 1–42 tuần");
            }

            // 13. SO_NGAY_NGHI – số nguyên dương, cảnh báo theo ngưỡng 30/50/180 ngày
            long? soNgay = ToInt(SO_NGAY_NGHI);
            if (SO_NGAY_NGHI != null && soNgay != null)
            {
                if (soNgay < 1)
                    errs.Add($"SO_NGAY_NGHI: '{SO_NGAY_NGHI}' phải >= 1");
                else if (soNgay > 180)
                    errs.Add($"SO_NGAY_NGHI: '{SO_NGAY_NGHI}' vượt quá 180 ngày " +
                             "(tối đa tuyệt đối theo TT 18/2022/TT-BYT)");
                else if (soNgay > 50)
                    errs.Add($"SO_NGAY_NGHI: '{SO_NGAY_NGHI}' vượt 50 ngày – " +
                             "chỉ hợp lệ với bệnh lao (tối đa 180 ngày), " +
                             "kiểm tra lại chẩn đoán");
                else if (soNgay > 30)
                    errs.Add($"SO_NGAY_NGHI: '{SO_NGAY_NGHI}' vượt 30 ngày – " +
                             "chỉ hợp lệ với sẩy/phá/nạo/hút thai ≥13 tuần (tối đa 50) " +
                             "hoặc bệnh lao (tối đa 180), kiểm tra lại chẩn đoán");
            }

            // 14–15. TU_NGAY, DEN_NGAY – DATE8, DEN_NGAY >= TU_NGAY
            DateTime? dtTu = ParseDate8(TU_NGAY);
            DateTime? dtDen = ParseDate8(DEN_NGAY);

            if (!string.IsNullOrEmpty(TU_NGAY) && dtTu == null)
                errs.Add($"TU_NGAY: '{TU_NGAY}' không phải ngày hợp lệ (định dạng yyyymmdd)");
            if (!string.IsNullOrEmpty(DEN_NGAY) && dtDen == null)
                errs.Add($"DEN_NGAY: '{DEN_NGAY}' không phải ngày hợp lệ (định dạng yyyymmdd)");
            if (dtTu != null && dtDen != null)
            {
                if (dtDen.Value < dtTu.Value)
                    errs.Add($"DEN_NGAY ({DEN_NGAY}) không được trước " +
                             $"TU_NGAY ({TU_NGAY})");
                // Cross-field: SO_NGAY_NGHI phải = DEN_NGAY - TU_NGAY + 1
                if (soNgay != null && soNgay >= 1)
                {
                    long expected = (long)(dtDen.Value.Date - dtTu.Value.Date).TotalDays + 1;
                    if (expected != soNgay)
                        errs.Add($"SO_NGAY_NGHI: kỳ vọng {expected} ngày " +
                                 "(DEN_NGAY − TU_NGAY + 1, tính cả 2 đầu), " +
                                 $"thực tế {soNgay}");
                }
            }

            // 20. NGAY_CT – DATE8, nằm trong [TU_NGAY, DEN_NGAY]
            DateTime? dtCt = ParseDate8(NGAY_CT);
            if (!string.IsNullOrEmpty(NGAY_CT) && dtCt == null)
                errs.Add($"NGAY_CT: '{NGAY_CT}' không phải ngày hợp lệ (định dạng yyyymmdd)");
            if (dtCt != null)
            {
                if (dtTu != null && dtCt.Value.Date < dtTu.Value.Date)
                    errs.Add($"NGAY_CT ({NGAY_CT}) không được trước " +
                             $"ngày bắt đầu hưởng chế độ TU_NGAY ({TU_NGAY})");
                if (dtDen != null && dtCt.Value.Date > dtDen.Value.Date)
                    errs.Add($"NGAY_CT ({NGAY_CT}) không được sau " +
                             $"ngày kết thúc hưởng chế độ DEN_NGAY ({DEN_NGAY})");
            }

            // 22. MAU_SO – nếu điền phải là "CT07"
            if (!string.IsNullOrEmpty(MAU_SO) && MAU_SO.Trim().ToUpperInvariant() != "CT07")
                errs.Add($"MAU_SO: '{MAU_SO}' không hợp lệ. " +
                         "Giấy nghỉ hưởng BHXH phải dùng mẫu CT07 " +
                         "(để trống thì hệ thống tự điền CT07)");

            return errs;
        }
    }
}
