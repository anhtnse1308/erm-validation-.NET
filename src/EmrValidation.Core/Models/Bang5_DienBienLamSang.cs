using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using EmrValidation.Core.Helpers;

namespace EmrValidation.Core.Models
{
    /// <summary>Bảng 5. Chỉ tiêu chi tiết diễn biến lâm sàng.</summary>
    public class Bang5_DienBienLamSang : BhytBase
    {
        public string MA_LK { get; set; }
        public string STT { get; set; }
        public string DIEN_BIEN_LS { get; set; }
        public string GIAI_DOAN_BENH { get; set; }
        public string HOI_CHAN { get; set; }
        public string PHAU_THUAT { get; set; }
        public string THOI_DIEM_DBLS { get; set; }
        public string NGUOI_THUC_HIEN { get; set; }
        public string DU_PHONG { get; set; }

        // ---------------- Metadata ----------------
        private static readonly string[] _fieldNames =
        {
            "MA_LK", "STT", "DIEN_BIEN_LS", "GIAI_DOAN_BENH", "HOI_CHAN", "PHAU_THUAT",
            "THOI_DIEM_DBLS", "NGUOI_THUC_HIEN", "DU_PHONG",
        };

        private static readonly KeyValuePair<string, int>[] _maxLen =
        {
            new KeyValuePair<string, int>("MA_LK", 100),
            new KeyValuePair<string, int>("STT", 10),
            new KeyValuePair<string, int>("THOI_DIEM_DBLS", 12),
            new KeyValuePair<string, int>("NGUOI_THUC_HIEN", 255),
        };

        private static readonly string[] _numeric =
        {
            "STT",
        };

        private static readonly string[] _date12 =
        {
            "THOI_DIEM_DBLS",
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
                case "DIEN_BIEN_LS": return DIEN_BIEN_LS;
                case "GIAI_DOAN_BENH": return GIAI_DOAN_BENH;
                case "HOI_CHAN": return HOI_CHAN;
                case "PHAU_THUAT": return PHAU_THUAT;
                case "THOI_DIEM_DBLS": return THOI_DIEM_DBLS;
                case "NGUOI_THUC_HIEN": return NGUOI_THUC_HIEN;
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
                case "DIEN_BIEN_LS": DIEN_BIEN_LS = value; return true;
                case "GIAI_DOAN_BENH": GIAI_DOAN_BENH = value; return true;
                case "HOI_CHAN": HOI_CHAN = value; return true;
                case "PHAU_THUAT": PHAU_THUAT = value; return true;
                case "THOI_DIEM_DBLS": THOI_DIEM_DBLS = value; return true;
                case "NGUOI_THUC_HIEN": NGUOI_THUC_HIEN = value; return true;
                case "DU_PHONG": DU_PHONG = value; return true;
                default: return false;
            }
        }

        // Lưu ý (giữ nguyên bản Python): validate() của Bảng 5 KHÔNG gọi base.Validate().
        public override List<string> Validate()
        {
            var errs = new List<string>();

            // 1. MA_LK – bắt buộc
            if (string.IsNullOrEmpty(MA_LK))
                errs.Add("MA_LK: bắt buộc, không được để trống");

            // 2. STT – số nguyên dương, tăng từ 1
            long? stt = ToInt(STT);
            if (STT != null && stt != null && stt < 1)
                errs.Add($"STT: '{STT}' phải là số nguyên dương >= 1");

            // 3. DIEN_BIEN_LS – chỉ số sinh tồn phải dùng format CSST<...>
            if (!string.IsNullOrEmpty(DIEN_BIEN_LS))
            {
                string dbls = DIEN_BIEN_LS;
                string[] sinhTonKeywords = { "mạch", "nhiệt độ", "huyết áp", "nhịp thở", "spo2", "hộ lý" };
                string dblsLower = dbls.ToLowerInvariant();
                bool hasSinhTonText = false;
                foreach (string k in sinhTonKeywords)
                {
                    if (dblsLower.Contains(k)) { hasSinhTonText = true; break; }
                }
                bool hasCsstTag = dbls.Contains("CSST<") && dbls.Contains(">");

                if (hasSinhTonText && !hasCsstTag)
                    errs.Add("DIEN_BIEN_LS: phát hiện chỉ số sinh tồn nhưng chưa dùng format chuẩn. " +
                             "Theo QĐ mới: ghi theo cấu trúc CSST<M{m}_T{t}_HA{ha}_NT{nt}_SP{sp}_HL{hl}>. " +
                             "VD: \"CSST<M75_T37_HA120/70_NT18_HL2>\"");

                // Nếu có tag CSST<> → validate cấu trúc bên trong
                if (hasCsstTag)
                {
                    Match csstMatch = Regex.Match(dbls, "CSST<([^>]*)>");
                    if (csstMatch.Success)
                    {
                        string inner = csstMatch.Groups[1].Value;
                        foreach (string p in inner.Split('_'))
                        {
                            if (p.Length > 0 && !Regex.IsMatch(p, @"^[A-Z]{1,3}[\d/\.]+$"))
                            {
                                errs.Add($"DIEN_BIEN_LS: chỉ số sinh tồn '{p}' trong CSST<> " +
                                         "sai định dạng. Phải là ký hiệu viết hoa + giá trị số " +
                                         "(VD: M75, T37, HA120/70, NT18, SP98, HL2)");
                                break; // chỉ báo lỗi đầu tiên
                            }
                        }
                    }
                }
            }

            // 7. THOI_DIEM_DBLS – thời điểm hợp lý (không phải tương lai, năm >= 2000)
            if (!string.IsNullOrEmpty(THOI_DIEM_DBLS) && THOI_DIEM_DBLS.Length == 12)
            {
                DateTime? dt = ParseDate12(THOI_DIEM_DBLS);
                if (dt != null)
                {
                    if (dt.Value > DateTime.Now)
                        errs.Add($"THOI_DIEM_DBLS: '{THOI_DIEM_DBLS}' là thời điểm tương lai, " +
                                 "kiểm tra lại");
                    if (dt.Value.Year < 2000)
                        errs.Add($"THOI_DIEM_DBLS: '{THOI_DIEM_DBLS}' năm {dt.Value.Year} không hợp lý");
                }
                else
                {
                    errs.Add($"THOI_DIEM_DBLS: '{THOI_DIEM_DBLS}' không phải ngày giờ hợp lệ " +
                             "(yyyymmddHHMM). Kiểm tra tháng/ngày/giờ/phút");
                }
            }

            // 8. NGUOI_THUC_HIEN – nhiều người cách nhau ";", mỗi mã không có khoảng trắng
            if (!string.IsNullOrEmpty(NGUOI_THUC_HIEN))
            {
                string[] nguoiList = NGUOI_THUC_HIEN.Split(';');
                for (int idx = 0; idx < nguoiList.Length; idx++)
                {
                    string nguoi = nguoiList[idx].Trim();
                    if (nguoi.Length == 0)
                    {
                        errs.Add($"NGUOI_THUC_HIEN: mục {idx + 1} rỗng " +
                                 "(kiểm tra dấu ';' thừa)");
                        continue;
                    }
                    if (Regex.IsMatch(nguoi, @"[\s;]"))
                        errs.Add($"NGUOI_THUC_HIEN: '{nguoi}' chứa khoảng trắng hoặc dấu ';'. " +
                                 "Nhiều người thực hiện phân cách bằng ';', " +
                                 "không được có khoảng trắng trong từng mã");
                }
            }

            // Cross-field: ít nhất 1 trong 4 field nội dung phải có giá trị
            if (string.IsNullOrEmpty(DIEN_BIEN_LS) && string.IsNullOrEmpty(GIAI_DOAN_BENH) &&
                string.IsNullOrEmpty(HOI_CHAN) && string.IsNullOrEmpty(PHAU_THUAT))
                errs.Add("DIEN_BIEN_LS / GIAI_DOAN_BENH / HOI_CHAN / PHAU_THUAT: " +
                         "ít nhất 1 trường phải có giá trị trong một record Bảng 5");

            return errs;
        }
    }
}
