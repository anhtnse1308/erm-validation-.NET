using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using EmrValidation.Core.Helpers;

namespace EmrValidation.Core.Models
{
    /// <summary>Bảng 4. Chỉ tiêu chi tiết dịch vụ cận lâm sàng.</summary>
    public class Bang4_DichVuCLS : BhytBase
    {
        public string MA_LK { get; set; }
        public string STT { get; set; }
        public string MA_DICH_VU { get; set; }
        public string MA_CHI_SO { get; set; }
        public string TEN_CHI_SO { get; set; }
        public string GIA_TRI { get; set; }
        public string DON_VI_DO { get; set; }
        public string MO_TA { get; set; }
        public string KET_LUAN { get; set; }
        public string NGAY_KQ { get; set; }
        public string MA_BS_DOC_KQ { get; set; }
        public string DU_PHONG { get; set; }

        // ---------------- Metadata ----------------
        private static readonly string[] _fieldNames =
        {
            "MA_LK", "STT", "MA_DICH_VU", "MA_CHI_SO", "TEN_CHI_SO", "GIA_TRI",
            "DON_VI_DO", "MO_TA", "KET_LUAN", "NGAY_KQ", "MA_BS_DOC_KQ", "DU_PHONG",
        };

        private static readonly KeyValuePair<string, int>[] _maxLen =
        {
            new KeyValuePair<string, int>("MA_LK", 100),
            new KeyValuePair<string, int>("STT", 10),
            new KeyValuePair<string, int>("MA_DICH_VU", 50),
            new KeyValuePair<string, int>("MA_CHI_SO", 255),
            new KeyValuePair<string, int>("TEN_CHI_SO", 255),
            new KeyValuePair<string, int>("GIA_TRI", 255),
            new KeyValuePair<string, int>("DON_VI_DO", 50),
            new KeyValuePair<string, int>("NGAY_KQ", 12),
            new KeyValuePair<string, int>("MA_BS_DOC_KQ", 255),
        };

        private static readonly string[] _numeric =
        {
            "STT",
        };

        private static readonly string[] _date12 =
        {
            "NGAY_KQ",
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
                case "MA_DICH_VU": return MA_DICH_VU;
                case "MA_CHI_SO": return MA_CHI_SO;
                case "TEN_CHI_SO": return TEN_CHI_SO;
                case "GIA_TRI": return GIA_TRI;
                case "DON_VI_DO": return DON_VI_DO;
                case "MO_TA": return MO_TA;
                case "KET_LUAN": return KET_LUAN;
                case "NGAY_KQ": return NGAY_KQ;
                case "MA_BS_DOC_KQ": return MA_BS_DOC_KQ;
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
                case "MA_DICH_VU": MA_DICH_VU = value; return true;
                case "MA_CHI_SO": MA_CHI_SO = value; return true;
                case "TEN_CHI_SO": TEN_CHI_SO = value; return true;
                case "GIA_TRI": GIA_TRI = value; return true;
                case "DON_VI_DO": DON_VI_DO = value; return true;
                case "MO_TA": MO_TA = value; return true;
                case "KET_LUAN": KET_LUAN = value; return true;
                case "NGAY_KQ": NGAY_KQ = value; return true;
                case "MA_BS_DOC_KQ": MA_BS_DOC_KQ = value; return true;
                case "DU_PHONG": DU_PHONG = value; return true;
                default: return false;
            }
        }

        // Lưu ý (giữ nguyên bản Python): validate() của Bảng 4 KHÔNG gọi base.Validate().
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

            // 3. MA_DICH_VU – bắt buộc (mã DVKT CLS theo QĐ 7603/QĐ-BYT)
            if (string.IsNullOrEmpty(MA_DICH_VU))
                errs.Add("MA_DICH_VU: bắt buộc, không được để trống");

            // 4. MA_CHI_SO – chưa có mã chuẩn thì mã hóa bằng tên viết liền, không khoảng trắng
            if (!string.IsNullOrEmpty(MA_CHI_SO))
            {
                if (MA_CHI_SO.Contains(" "))
                    errs.Add($"MA_CHI_SO: '{MA_CHI_SO}' chứa khoảng trắng. " +
                             "Trường hợp chưa có mã chuẩn, mã hoá bằng tên viết liền không dấu " +
                             "(VD: 'glucosetoan' thay vì 'glucose toàn phần')");
            }

            // Cross-field: DON_VI_DO có giá trị → MA_CHI_SO và TEN_CHI_SO phải có giá trị
            if (!string.IsNullOrEmpty(DON_VI_DO))
            {
                if (string.IsNullOrEmpty(MA_CHI_SO))
                    errs.Add("MA_CHI_SO: bắt buộc khi DON_VI_DO có giá trị " +
                             $"(DON_VI_DO='{DON_VI_DO}')");
                if (string.IsNullOrEmpty(TEN_CHI_SO))
                    errs.Add("TEN_CHI_SO: bắt buộc khi DON_VI_DO có giá trị " +
                             $"(DON_VI_DO='{DON_VI_DO}')");
            }

            // Cross-field: GIA_TRI có giá trị → KET_LUAN phải để trống
            if (!string.IsNullOrEmpty(GIA_TRI) && !string.IsNullOrEmpty(KET_LUAN))
                errs.Add($"KET_LUAN: phải để trống khi GIA_TRI đã có giá trị ('{GIA_TRI}'). " +
                         "Theo QĐ mới: GIA_TRI dùng cho kết quả số, KET_LUAN dùng cho kết luận văn bản");

            // Cross-field: MO_TA / KET_LUAN (chỉ cảnh báo nếu có MA_CHI_SO)
            if (string.IsNullOrEmpty(GIA_TRI) && string.IsNullOrEmpty(MO_TA) && string.IsNullOrEmpty(KET_LUAN))
            {
                if (!string.IsNullOrEmpty(MA_CHI_SO))
                    errs.Add("GIA_TRI, MO_TA, KET_LUAN: ít nhất 1 trường phải có giá trị " +
                             "khi MA_CHI_SO đã được khai báo");
            }

            // 11. MA_BS_DOC_KQ – <GPHN> | <GPHN>.TX.<XXXXX> | <GPHN>.CG.<XXXXX>
            if (!string.IsNullOrEmpty(MA_BS_DOC_KQ))
            {
                string val = MA_BS_DOC_KQ.Trim();
                if (val.Contains("."))
                {
                    string[] parts = val.Split('.');
                    if (parts.Length != 3)
                    {
                        errs.Add($"MA_BS_DOC_KQ: '{val}' sai định dạng. " +
                                 "Phải là <GPHN>.TX.<MaCsKbcb> hoặc <GPHN>.CG.<MaCsKbcb>");
                    }
                    else
                    {
                        string gphn = parts[0], loai = parts[1], maCs = parts[2];
                        if (gphn.Trim().Length == 0)
                            errs.Add($"MA_BS_DOC_KQ: thiếu mã GPHN bác sĩ trong '{val}'");
                        string loaiUp = loai.ToUpperInvariant();
                        if (loaiUp != "TX" && loaiUp != "CG")
                            errs.Add($"MA_BS_DOC_KQ: loại '{loai}' không hợp lệ trong '{val}', " +
                                     "chỉ chấp nhận TX (từ xa) hoặc CG (chuyển giao kỹ thuật)");
                        if (maCs.Trim().Length == 0)
                            errs.Add($"MA_BS_DOC_KQ: thiếu mã cơ sở KBCB trong '{val}'");
                    }
                }
            }

            return errs;
        }
    }
}
