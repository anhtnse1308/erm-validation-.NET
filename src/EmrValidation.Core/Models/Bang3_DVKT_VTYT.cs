using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using EmrValidation.Core.Helpers;

namespace EmrValidation.Core.Models
{
    /// <summary>Bảng 3. Chỉ tiêu chi tiết dịch vụ kỹ thuật và vật tư y tế.</summary>
    public class Bang3_DVKT_VTYT : BhytBase
    {
        public string MA_LK { get; set; }
        public string STT { get; set; }
        public string MA_DICH_VU { get; set; }
        public string MA_PTTT_QT { get; set; }
        public string MA_VAT_TU { get; set; }
        public string MA_NHOM { get; set; }
        public string GOI_VTYT { get; set; }
        public string TEN_VAT_TU { get; set; }
        public string TEN_DICH_VU { get; set; }
        public string MA_XANG_DAU { get; set; }
        public string DON_VI_TINH { get; set; }
        public string PHAM_VI { get; set; }
        public string SO_LUONG { get; set; }
        public string DON_GIA_BV { get; set; }
        public string DON_GIA_BH { get; set; }
        public string TT_THAU { get; set; }
        public string TYLE_TT_DV { get; set; }
        public string TYLE_TT_BH { get; set; }
        public string THANH_TIEN_BV { get; set; }
        public string THANH_TIEN_BH { get; set; }
        public string T_TRANTT { get; set; }
        public string MUC_HUONG { get; set; }
        public string T_NGUONKHAC_NSNN { get; set; }
        public string T_NGUONKHAC_VTNN { get; set; }
        public string T_NGUONKHAC_VTTN { get; set; }
        public string T_NGUONKHAC_CL { get; set; }
        public string T_NGUONKHAC { get; set; }
        public string T_BNTT { get; set; }
        public string T_BNCCT { get; set; }
        public string T_BHTT { get; set; }
        public string MA_KHOA { get; set; }
        public string MA_GIUONG { get; set; }
        public string MA_BAC_SI { get; set; }
        public string NGUOI_THUC_HIEN { get; set; }
        public string MA_BENH { get; set; }
        public string MA_BENH_YHCT { get; set; }
        public string NGAY_YL { get; set; }
        public string NGAY_TH_YL { get; set; }
        public string NGAY_KQ { get; set; }
        public string MA_PTTT { get; set; }
        public string VET_THUONG_TP { get; set; }
        public string PP_VO_CAM { get; set; }
        public string VI_TRI_TH_DVKT { get; set; }
        public string MA_MAY { get; set; }
        public string MA_HIEU_SP { get; set; }
        public string TAI_SU_DUNG { get; set; }
        public string DU_PHONG { get; set; }

        // ---------------- Metadata ----------------
        private static readonly string[] _fieldNames =
        {
            "MA_LK", "STT", "MA_DICH_VU", "MA_PTTT_QT", "MA_VAT_TU", "MA_NHOM",
            "GOI_VTYT", "TEN_VAT_TU", "TEN_DICH_VU", "MA_XANG_DAU", "DON_VI_TINH", "PHAM_VI",
            "SO_LUONG", "DON_GIA_BV", "DON_GIA_BH", "TT_THAU", "TYLE_TT_DV", "TYLE_TT_BH",
            "THANH_TIEN_BV", "THANH_TIEN_BH", "T_TRANTT", "MUC_HUONG", "T_NGUONKHAC_NSNN", "T_NGUONKHAC_VTNN",
            "T_NGUONKHAC_VTTN", "T_NGUONKHAC_CL", "T_NGUONKHAC", "T_BNTT", "T_BNCCT", "T_BHTT",
            "MA_KHOA", "MA_GIUONG", "MA_BAC_SI", "NGUOI_THUC_HIEN", "MA_BENH", "MA_BENH_YHCT",
            "NGAY_YL", "NGAY_TH_YL", "NGAY_KQ", "MA_PTTT", "VET_THUONG_TP", "PP_VO_CAM",
            "VI_TRI_TH_DVKT", "MA_MAY", "MA_HIEU_SP", "TAI_SU_DUNG", "DU_PHONG",
        };

        private static readonly KeyValuePair<string, int>[] _maxLen =
        {
            new KeyValuePair<string, int>("MA_LK", 100),
            new KeyValuePair<string, int>("STT", 10),
            new KeyValuePair<string, int>("MA_DICH_VU", 50),
            new KeyValuePair<string, int>("MA_PTTT_QT", 255),
            new KeyValuePair<string, int>("MA_VAT_TU", 255),
            new KeyValuePair<string, int>("MA_NHOM", 2),
            new KeyValuePair<string, int>("GOI_VTYT", 3),
            new KeyValuePair<string, int>("TEN_VAT_TU", 1024),
            new KeyValuePair<string, int>("TEN_DICH_VU", 1024),
            new KeyValuePair<string, int>("MA_XANG_DAU", 20),
            new KeyValuePair<string, int>("DON_VI_TINH", 50),
            new KeyValuePair<string, int>("PHAM_VI", 1),
            new KeyValuePair<string, int>("SO_LUONG", 10),
            new KeyValuePair<string, int>("DON_GIA_BV", 15),
            new KeyValuePair<string, int>("DON_GIA_BH", 15),
            new KeyValuePair<string, int>("TT_THAU", 50),
            new KeyValuePair<string, int>("TYLE_TT_DV", 3),
            new KeyValuePair<string, int>("TYLE_TT_BH", 3),
            new KeyValuePair<string, int>("THANH_TIEN_BV", 15),
            new KeyValuePair<string, int>("THANH_TIEN_BH", 15),
            new KeyValuePair<string, int>("T_TRANTT", 15),
            new KeyValuePair<string, int>("MUC_HUONG", 4),
            new KeyValuePair<string, int>("T_NGUONKHAC_NSNN", 15),
            new KeyValuePair<string, int>("T_NGUONKHAC_VTNN", 15),
            new KeyValuePair<string, int>("T_NGUONKHAC_VTTN", 15),
            new KeyValuePair<string, int>("T_NGUONKHAC_CL", 15),
            new KeyValuePair<string, int>("T_NGUONKHAC", 15),
            new KeyValuePair<string, int>("T_BNTT", 15),
            new KeyValuePair<string, int>("T_BNCCT", 15),
            new KeyValuePair<string, int>("T_BHTT", 15),
            new KeyValuePair<string, int>("MA_KHOA", 50),
            new KeyValuePair<string, int>("MA_GIUONG", 50),
            new KeyValuePair<string, int>("MA_BAC_SI", 255),
            new KeyValuePair<string, int>("NGUOI_THUC_HIEN", 255),
            new KeyValuePair<string, int>("MA_BENH", 100),
            new KeyValuePair<string, int>("MA_BENH_YHCT", 150),
            new KeyValuePair<string, int>("NGAY_YL", 12),
            new KeyValuePair<string, int>("NGAY_TH_YL", 12),
            new KeyValuePair<string, int>("NGAY_KQ", 12),
            new KeyValuePair<string, int>("MA_PTTT", 1),
            new KeyValuePair<string, int>("VET_THUONG_TP", 1),
            new KeyValuePair<string, int>("PP_VO_CAM", 1),
            new KeyValuePair<string, int>("VI_TRI_TH_DVKT", 3),
            new KeyValuePair<string, int>("MA_MAY", 1024),
            new KeyValuePair<string, int>("MA_HIEU_SP", 255),
            new KeyValuePair<string, int>("TAI_SU_DUNG", 1),
        };

        private static readonly string[] _numeric =
        {
            "STT", "MA_NHOM", "PHAM_VI", "SO_LUONG", "DON_GIA_BV", "DON_GIA_BH",
            "TYLE_TT_DV", "TYLE_TT_BH", "THANH_TIEN_BV", "THANH_TIEN_BH", "T_TRANTT", "MUC_HUONG",
            "T_NGUONKHAC_NSNN", "T_NGUONKHAC_VTNN", "T_NGUONKHAC_VTTN", "T_NGUONKHAC_CL", "T_NGUONKHAC", "T_BNTT",
            "T_BNCCT", "T_BHTT", "MA_PTTT", "VET_THUONG_TP", "PP_VO_CAM", "VI_TRI_TH_DVKT",
            "TAI_SU_DUNG",
        };

        private static readonly string[] _date12 =
        {
            "NGAY_YL", "NGAY_TH_YL", "NGAY_KQ",
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
                case "MA_PTTT_QT": return MA_PTTT_QT;
                case "MA_VAT_TU": return MA_VAT_TU;
                case "MA_NHOM": return MA_NHOM;
                case "GOI_VTYT": return GOI_VTYT;
                case "TEN_VAT_TU": return TEN_VAT_TU;
                case "TEN_DICH_VU": return TEN_DICH_VU;
                case "MA_XANG_DAU": return MA_XANG_DAU;
                case "DON_VI_TINH": return DON_VI_TINH;
                case "PHAM_VI": return PHAM_VI;
                case "SO_LUONG": return SO_LUONG;
                case "DON_GIA_BV": return DON_GIA_BV;
                case "DON_GIA_BH": return DON_GIA_BH;
                case "TT_THAU": return TT_THAU;
                case "TYLE_TT_DV": return TYLE_TT_DV;
                case "TYLE_TT_BH": return TYLE_TT_BH;
                case "THANH_TIEN_BV": return THANH_TIEN_BV;
                case "THANH_TIEN_BH": return THANH_TIEN_BH;
                case "T_TRANTT": return T_TRANTT;
                case "MUC_HUONG": return MUC_HUONG;
                case "T_NGUONKHAC_NSNN": return T_NGUONKHAC_NSNN;
                case "T_NGUONKHAC_VTNN": return T_NGUONKHAC_VTNN;
                case "T_NGUONKHAC_VTTN": return T_NGUONKHAC_VTTN;
                case "T_NGUONKHAC_CL": return T_NGUONKHAC_CL;
                case "T_NGUONKHAC": return T_NGUONKHAC;
                case "T_BNTT": return T_BNTT;
                case "T_BNCCT": return T_BNCCT;
                case "T_BHTT": return T_BHTT;
                case "MA_KHOA": return MA_KHOA;
                case "MA_GIUONG": return MA_GIUONG;
                case "MA_BAC_SI": return MA_BAC_SI;
                case "NGUOI_THUC_HIEN": return NGUOI_THUC_HIEN;
                case "MA_BENH": return MA_BENH;
                case "MA_BENH_YHCT": return MA_BENH_YHCT;
                case "NGAY_YL": return NGAY_YL;
                case "NGAY_TH_YL": return NGAY_TH_YL;
                case "NGAY_KQ": return NGAY_KQ;
                case "MA_PTTT": return MA_PTTT;
                case "VET_THUONG_TP": return VET_THUONG_TP;
                case "PP_VO_CAM": return PP_VO_CAM;
                case "VI_TRI_TH_DVKT": return VI_TRI_TH_DVKT;
                case "MA_MAY": return MA_MAY;
                case "MA_HIEU_SP": return MA_HIEU_SP;
                case "TAI_SU_DUNG": return TAI_SU_DUNG;
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
                case "MA_PTTT_QT": MA_PTTT_QT = value; return true;
                case "MA_VAT_TU": MA_VAT_TU = value; return true;
                case "MA_NHOM": MA_NHOM = value; return true;
                case "GOI_VTYT": GOI_VTYT = value; return true;
                case "TEN_VAT_TU": TEN_VAT_TU = value; return true;
                case "TEN_DICH_VU": TEN_DICH_VU = value; return true;
                case "MA_XANG_DAU": MA_XANG_DAU = value; return true;
                case "DON_VI_TINH": DON_VI_TINH = value; return true;
                case "PHAM_VI": PHAM_VI = value; return true;
                case "SO_LUONG": SO_LUONG = value; return true;
                case "DON_GIA_BV": DON_GIA_BV = value; return true;
                case "DON_GIA_BH": DON_GIA_BH = value; return true;
                case "TT_THAU": TT_THAU = value; return true;
                case "TYLE_TT_DV": TYLE_TT_DV = value; return true;
                case "TYLE_TT_BH": TYLE_TT_BH = value; return true;
                case "THANH_TIEN_BV": THANH_TIEN_BV = value; return true;
                case "THANH_TIEN_BH": THANH_TIEN_BH = value; return true;
                case "T_TRANTT": T_TRANTT = value; return true;
                case "MUC_HUONG": MUC_HUONG = value; return true;
                case "T_NGUONKHAC_NSNN": T_NGUONKHAC_NSNN = value; return true;
                case "T_NGUONKHAC_VTNN": T_NGUONKHAC_VTNN = value; return true;
                case "T_NGUONKHAC_VTTN": T_NGUONKHAC_VTTN = value; return true;
                case "T_NGUONKHAC_CL": T_NGUONKHAC_CL = value; return true;
                case "T_NGUONKHAC": T_NGUONKHAC = value; return true;
                case "T_BNTT": T_BNTT = value; return true;
                case "T_BNCCT": T_BNCCT = value; return true;
                case "T_BHTT": T_BHTT = value; return true;
                case "MA_KHOA": MA_KHOA = value; return true;
                case "MA_GIUONG": MA_GIUONG = value; return true;
                case "MA_BAC_SI": MA_BAC_SI = value; return true;
                case "NGUOI_THUC_HIEN": NGUOI_THUC_HIEN = value; return true;
                case "MA_BENH": MA_BENH = value; return true;
                case "MA_BENH_YHCT": MA_BENH_YHCT = value; return true;
                case "NGAY_YL": NGAY_YL = value; return true;
                case "NGAY_TH_YL": NGAY_TH_YL = value; return true;
                case "NGAY_KQ": NGAY_KQ = value; return true;
                case "MA_PTTT": MA_PTTT = value; return true;
                case "VET_THUONG_TP": VET_THUONG_TP = value; return true;
                case "PP_VO_CAM": PP_VO_CAM = value; return true;
                case "VI_TRI_TH_DVKT": VI_TRI_TH_DVKT = value; return true;
                case "MA_MAY": MA_MAY = value; return true;
                case "MA_HIEU_SP": MA_HIEU_SP = value; return true;
                case "TAI_SU_DUNG": TAI_SU_DUNG = value; return true;
                case "DU_PHONG": DU_PHONG = value; return true;
                default: return false;
            }
        }

        // ---------------------------------------------------------------
        // Lưu ý (giữ nguyên hành vi bản Python): validate() của Bảng 3 KHÔNG gọi
        // base.Validate(), nên không kiểm tra _MAX_LEN, _NUMERIC, _DATE12 ở mức base.
        // ---------------------------------------------------------------
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

            // 3. MA_DICH_VU – chỉ kiểm suffix hợp lệ (_GT, _TB, _TT)
            if (!string.IsNullOrEmpty(MA_DICH_VU))
            {
                if (Rx.Search("_[A-Z]{2}$", MA_DICH_VU))
                {
                    string[] parts = MA_DICH_VU.Split('_');
                    string suffix = parts[parts.Length - 1];
                    if (suffix != "GT" && suffix != "TB" && suffix != "TT")
                        errs.Add($"MA_DICH_VU: suffix '_{suffix}' không hợp lệ, " +
                                 "chỉ chấp nhận _GT (gây tê), _TB (dừng kỹ thuật), _TT (gây tê chưa có giá)");
                }
            }

            // 6. MA_NHOM – enum 1-19
            long? maNhom = ToInt(MA_NHOM);
            if (MA_NHOM != null && maNhom != null)
            {
                if (!(1 <= maNhom && maNhom <= 19))
                    errs.Add($"MA_NHOM: '{MA_NHOM}' không hợp lệ, phải trong 1–19");
            }

            // 7. GOI_VTYT – format G<số> (G1 ... G99)
            if (!string.IsNullOrEmpty(GOI_VTYT))
            {
                if (!Rx.Full(@"G[1-9]\d?", GOI_VTYT.Trim()))
                    errs.Add($"GOI_VTYT: '{GOI_VTYT}' sai định dạng, " +
                             "phải là G1, G2, … G99 (tối đa 3 ký tự)");
            }

            // 12. PHAM_VI – enum {1, 2, 3}
            long? phamVi = ToInt(PHAM_VI);
            if (PHAM_VI != null && phamVi != null)
            {
                if (phamVi != 1 && phamVi != 2 && phamVi != 3)
                    errs.Add($"PHAM_VI: '{PHAM_VI}' không hợp lệ, " +
                             "chỉ chấp nhận 1 (BHYT) / 2 (tự trả) / 3 (quân đội/công an/cơ yếu)");
            }

            // 13. SO_LUONG – tối đa 3 chữ số thập phân, dùng dấu "."
            double? sl = ToFloat(SO_LUONG);
            if (SO_LUONG != null && sl != null)
            {
                if (SO_LUONG.Contains(","))
                    errs.Add("SO_LUONG: dùng dấu chấm '.' thay vì dấu phẩy ',' cho số thập phân");
                else if (DecimalPlaces(SO_LUONG) > 3)
                    errs.Add($"SO_LUONG: '{SO_LUONG}' vượt quá 3 chữ số thập phân");
            }

            // 14. DON_GIA_BV – tối đa 3 chữ số thập phân, >= 0
            double? dgbv = ToFloat(DON_GIA_BV);
            if (DON_GIA_BV != null && dgbv != null)
            {
                if (DON_GIA_BV.Contains(","))
                    errs.Add("DON_GIA_BV: dùng dấu chấm '.' thay vì dấu phẩy ','");
                else if (DecimalPlaces(DON_GIA_BV) > 3)
                    errs.Add("DON_GIA_BV: vượt quá 3 chữ số thập phân");
                if (dgbv.Value < 0)
                    errs.Add("DON_GIA_BV: không được âm");
            }

            // 15. DON_GIA_BH – tối đa 3 chữ số thập phân, >= 0
            double? dgbh = ToFloat(DON_GIA_BH);
            if (DON_GIA_BH != null && dgbh != null)
            {
                if (DON_GIA_BH.Contains(","))
                    errs.Add("DON_GIA_BH: dùng dấu chấm '.' thay vì dấu phẩy ','");
                else if (DecimalPlaces(DON_GIA_BH) > 3)
                    errs.Add("DON_GIA_BH: vượt quá 3 chữ số thập phân");
                if (dgbh.Value < 0)
                    errs.Add("DON_GIA_BH: không được âm");
            }

            // Khi T_TRANTT có giá trị, DON_GIA_BV > T_TRANTT → DON_GIA_BH phải = T_TRANTT
            double? tTran = ToFloat(T_TRANTT);
            if (dgbv != null && dgbh != null && tTran != null && tTran.Value > 0)
            {
                if (dgbv.Value > tTran.Value && Math.Abs(dgbh.Value - tTran.Value) > 0.001)
                    errs.Add($"DON_GIA_BH: khi DON_GIA_BV ({F(dgbv.Value)}) > T_TRANTT ({F(tTran.Value)}), " +
                             $"DON_GIA_BH phải = T_TRANTT ({F(tTran.Value)}), thực tế {F(dgbh.Value)}");
            }

            // 17. TYLE_TT_DV – 0-100; tập thông thường {0,10,30,33,50,80,100}
            long? tyleDv = ToInt(TYLE_TT_DV);
            if (TYLE_TT_DV != null && tyleDv != null)
            {
                if (!(0 <= tyleDv && tyleDv <= 100))
                    errs.Add($"TYLE_TT_DV: '{TYLE_TT_DV}' phải trong 0-100");
                else if (tyleDv != 0 && tyleDv != 10 && tyleDv != 30 && tyleDv != 33 &&
                         tyleDv != 50 && tyleDv != 80 && tyleDv != 100)
                    errs.Add($"TYLE_TT_DV: '{TYLE_TT_DV}' không thuộc tập thông thường " +
                             "[0, 10, 30, 33, 50, 80, 100]. Kiểm tra lại nếu không phải trường hợp giảm đơn giá BV");
            }

            // 18. TYLE_TT_BH – 0-100
            long? tyleBh = ToInt(TYLE_TT_BH);
            if (TYLE_TT_BH != null && tyleBh != null)
            {
                if (!(0 <= tyleBh && tyleBh <= 100))
                    errs.Add($"TYLE_TT_BH: '{TYLE_TT_BH}' không hợp lệ, phải trong 0–100");
            }

            // 19. THANH_TIEN_BV – tối đa 2 chữ số thập phân
            double? ttbv = ToFloat(THANH_TIEN_BV);
            if (THANH_TIEN_BV != null && ttbv != null)
            {
                if (THANH_TIEN_BV.Contains(","))
                    errs.Add("THANH_TIEN_BV: dùng dấu chấm '.' thay vì dấu phẩy ','");
                else if (DecimalPlaces(THANH_TIEN_BV) > 2)
                    errs.Add("THANH_TIEN_BV: vượt quá 2 chữ số thập phân");
                if (ttbv.Value < 0)
                    errs.Add("THANH_TIEN_BV: không được âm");
            }

            if (sl != null && dgbv != null && tyleDv != null && ttbv != null)
            {
                double tyleDvD = tyleDv.Value;
                // Công thức chuẩn
                double expectedStd = R2(sl.Value * dgbv.Value * tyleDvD / 100);
                // Công thức đặc biệt (BV không thu thêm)
                double? expectedSpecial = null;
                if (dgbh != null)
                    expectedSpecial = R2(sl.Value * dgbv.Value - sl.Value * dgbh.Value * (100 - tyleDvD) / 100);

                bool okStd = Math.Abs(ttbv.Value - expectedStd) <= 0.02;
                bool okSpecial = expectedSpecial != null && Math.Abs(ttbv.Value - expectedSpecial.Value) <= 0.02;

                if (!okStd && !okSpecial)
                {
                    string specialStr = expectedSpecial != null ? F(expectedSpecial.Value) : "None";
                    errs.Add($"THANH_TIEN_BV: thực tế {F(ttbv.Value)}, " +
                             $"kỳ vọng {F(expectedStd)} (SO_LUONG×DON_GIA_BV×TYLE_TT_DV/100) " +
                             $"hoặc {specialStr} (trường hợp BV không thu thêm). " +
                             "Công thức đã sửa đổi theo QĐ mới (không còn = SO_LUONG×DON_GIA_BV)");
                }
            }

            // 20. THANH_TIEN_BH – tối đa 2 chữ số thập phân
            double? ttbh = ToFloat(THANH_TIEN_BH);
            if (THANH_TIEN_BH != null && ttbh != null)
            {
                if (THANH_TIEN_BH.Contains(","))
                    errs.Add("THANH_TIEN_BH: dùng dấu chấm '.' thay vì dấu phẩy ','");
                else if (DecimalPlaces(THANH_TIEN_BH) > 2)
                    errs.Add("THANH_TIEN_BH: vượt quá 2 chữ số thập phân");
                if (ttbh.Value < 0)
                    errs.Add("THANH_TIEN_BH: không được âm");
            }

            if (sl != null && dgbh != null && tyleDv != null && tyleBh != null && ttbh != null)
            {
                double expectedTtbh = R2(sl.Value * dgbh.Value * (double)tyleDv.Value / 100 * (double)tyleBh.Value / 100);
                if (Math.Abs(ttbh.Value - expectedTtbh) > 0.02)
                    errs.Add($"THANH_TIEN_BH: kỳ vọng {F(expectedTtbh)} " +
                             "(SO_LUONG×DON_GIA_BH×TYLE_TT_DV/100×TYLE_TT_BH/100), " +
                             $"thực tế {F(ttbh.Value)}");
            }

            // 22. MUC_HUONG – số thực 0-100
            double? mucHuong = ToFloat(MUC_HUONG);
            if (MUC_HUONG != null && mucHuong != null)
            {
                if (!(0 <= mucHuong.Value && mucHuong.Value <= 100))
                    errs.Add($"MUC_HUONG: '{MUC_HUONG}' không hợp lệ, phải trong 0–100");
            }

            // 23-26. T_NGUONKHAC_* – >= 0, tối đa 2 chữ số thập phân
            double nguonNsnn = ToFloat(T_NGUONKHAC_NSNN) ?? 0.0;
            double nguonVtnn = ToFloat(T_NGUONKHAC_VTNN) ?? 0.0;
            double nguonVttn = ToFloat(T_NGUONKHAC_VTTN) ?? 0.0;
            double nguonCl   = ToFloat(T_NGUONKHAC_CL)   ?? 0.0;

            string[] nguonNames = { "T_NGUONKHAC_NSNN", "T_NGUONKHAC_VTNN", "T_NGUONKHAC_VTTN", "T_NGUONKHAC_CL" };
            double[] nguonVals = { nguonNsnn, nguonVtnn, nguonVttn, nguonCl };
            for (int i = 0; i < nguonNames.Length; i++)
            {
                string raw = GetValue(nguonNames[i]);
                if (raw != null)
                {
                    if (nguonVals[i] < 0)
                        errs.Add($"{nguonNames[i]}: không được âm");
                    if (DecimalPlaces(raw) > 2)
                        errs.Add($"{nguonNames[i]}: vượt quá 2 chữ số thập phân");
                }
            }

            // 27. T_NGUONKHAC = tổng 4 nguồn
            double? tNguon = ToFloat(T_NGUONKHAC);
            if (T_NGUONKHAC != null && tNguon != null)
            {
                if (tNguon.Value < 0)
                    errs.Add("T_NGUONKHAC: không được âm");
                double expectedNguon = R2(nguonNsnn + nguonVtnn + nguonVttn + nguonCl);
                if (Math.Abs(tNguon.Value - expectedNguon) > 0.02)
                    errs.Add($"T_NGUONKHAC: kỳ vọng {F(expectedNguon)} " +
                             $"(T_NGUONKHAC_NSNN + VTNN + VTTN + CL), thực tế {F(tNguon.Value)}");
                // Ràng buộc: T_NGUONKHAC < THANH_TIEN_BV
                if (ttbv != null && tNguon.Value >= ttbv.Value)
                    errs.Add($"T_NGUONKHAC ({F(tNguon.Value)}) phải < THANH_TIEN_BV ({F(ttbv.Value)})");
            }

            // Giá trị tạm thời để kiểm tra T_BNTT, T_BNCCT, T_BHTT
            double? tBhttTt = null, tBncctTt = null, tBnttTt = null;
            if (ttbh != null && mucHuong != null)
            {
                tBhttTt = R2(ttbh.Value * mucHuong.Value / 100);
                tBncctTt = R2(ttbh.Value - tBhttTt.Value);
            }
            if (ttbv != null && ttbh != null)
                tBnttTt = R2(ttbv.Value - ttbh.Value);

            // Giá trị kỳ vọng sau khi điều chỉnh T_NGUONKHAC
            double? expBntt = null, expBncct = null, expBhtt = null;
            string expBhttStr = null;   // Python: round(max(x, 0), 2) trả int 0 (in "0") khi x < 0
            if (tBnttTt != null && tBncctTt != null && tBhttTt != null)
            {
                double nguon = (tNguon != null && tNguon.Value != 0.0) ? tNguon.Value : 0.0;
                if (nguon <= tBnttTt.Value)
                {
                    expBntt = R2(tBnttTt.Value - nguon);
                    expBncct = tBncctTt;
                    expBhtt = tBhttTt;
                }
                else
                {
                    expBntt = 0.0;
                    double du = nguon - tBnttTt.Value;
                    if (du <= tBncctTt.Value)
                    {
                        expBncct = R2(tBncctTt.Value - du);
                        expBhtt = tBhttTt;
                    }
                    else
                    {
                        expBncct = 0.0;
                        double du2 = du - tBncctTt.Value;
                        double rest = tBhttTt.Value - du2;
                        if (rest < 0)
                        {
                            expBhtt = 0.0;
                            expBhttStr = "0";
                        }
                        else
                        {
                            expBhtt = R2(rest);
                        }
                    }
                }
            }
            bool coNguon = tNguon != null && tNguon.Value != 0.0;
            string dieuChinh = coNguon ? ", đã điều chỉnh T_NGUONKHAC)" : ")";

            // 30. T_BHTT
            double? tBhtt = ToFloat(T_BHTT);
            if (T_BHTT != null && tBhtt != null)
            {
                if (tBhtt.Value < 0)
                    errs.Add("T_BHTT: không được âm");
                if (DecimalPlaces(T_BHTT) > 2)
                    errs.Add("T_BHTT: vượt quá 2 chữ số thập phân");
                if (expBhtt != null && Math.Abs(tBhtt.Value - expBhtt.Value) > 0.02)
                    errs.Add($"T_BHTT: kỳ vọng {(expBhttStr ?? F(expBhtt.Value))} " +
                             "(THANH_TIEN_BH×MUC_HUONG/100" + dieuChinh +
                             $", thực tế {F(tBhtt.Value)}");
            }

            // 29. T_BNCCT
            double? tBncct = ToFloat(T_BNCCT);
            if (T_BNCCT != null && tBncct != null)
            {
                if (tBncct.Value < 0)
                    errs.Add("T_BNCCT: không được âm");
                if (DecimalPlaces(T_BNCCT) > 2)
                    errs.Add("T_BNCCT: vượt quá 2 chữ số thập phân");
                if (expBncct != null && Math.Abs(tBncct.Value - expBncct.Value) > 0.02)
                    errs.Add($"T_BNCCT: kỳ vọng {F(expBncct.Value)} " +
                             "(THANH_TIEN_BH - T_BHTT" + dieuChinh +
                             $", thực tế {F(tBncct.Value)}");
            }

            // 28. T_BNTT
            double? tBntt = ToFloat(T_BNTT);
            if (T_BNTT != null && tBntt != null)
            {
                if (tBntt.Value < 0)
                    errs.Add("T_BNTT: không được âm");
                if (DecimalPlaces(T_BNTT) > 2)
                    errs.Add("T_BNTT: vượt quá 2 chữ số thập phân");
                if (expBntt != null && Math.Abs(tBntt.Value - expBntt.Value) > 0.02)
                    errs.Add($"T_BNTT: kỳ vọng {F(expBntt.Value)} " +
                             "(THANH_TIEN_BV - THANH_TIEN_BH" + dieuChinh +
                             $", thực tế {F(tBntt.Value)}");
            }

            // Cân đối tổng: THANH_TIEN_BV = T_BNTT + T_BNCCT + T_BHTT + T_NGUONKHAC
            if (ttbv != null && tBntt != null && tBncct != null && tBhtt != null)
            {
                double nguonTong = (tNguon != null && tNguon.Value != 0.0) ? tNguon.Value : 0.0;
                double total = R2(tBntt.Value + tBncct.Value + tBhtt.Value + nguonTong);
                if (Math.Abs(total - ttbv.Value) > 0.05)
                    errs.Add($"Cân đối chi phí: THANH_TIEN_BV ({F(ttbv.Value)}) ≠ " +
                             $"T_BNTT+T_BNCCT+T_BHTT+T_NGUONKHAC ({F(total)})");
            }

            // 32. MA_GIUONG – định dạng X.Y.ZZZ (X: H|T, Y: 1-4, ZZZ: 3 chữ số), có thể kèm mã khoa (H.4.020.K03); nhiều giường cách nhau ";"
            if (!string.IsNullOrEmpty(MA_GIUONG))
            {
                foreach (string item in MA_GIUONG.Split(';'))
                {
                    string mg = item.Trim();
                    if (mg.Length > 0 && !Rx.Full(@"[HT]\.[1-4]\.\d{3}(\.[A-Za-z0-9]+)?", mg))
                        errs.Add($"MA_GIUONG: '{mg}' sai định dạng, " +
                                 "phải là X.Y.ZZZ (X: H|T, Y: 1-4, ZZZ: 3 chữ số, VD H.4.020), " +
                                 "có thể kèm mã khoa phía sau (VD H.4.020.K03)");
                }
            }

            // 37-39. Trật tự thời gian: NGAY_YL ≤ NGAY_TH_YL ≤ NGAY_KQ (so sánh chuỗi)
            string yl = (NGAY_YL ?? "").Trim();
            string th = (NGAY_TH_YL ?? "").Trim();
            string kq = (NGAY_KQ ?? "").Trim();

            if (yl.Length > 0 && th.Length > 0 && yl.Length == 12 && th.Length == 12)
            {
                if (string.CompareOrdinal(th, yl) < 0)
                    errs.Add($"NGAY_TH_YL ({th}) không được trước NGAY_YL ({yl})");
            }
            if (th.Length > 0 && kq.Length > 0 && th.Length == 12 && kq.Length == 12)
            {
                if (string.CompareOrdinal(kq, th) < 0)
                    errs.Add($"NGAY_KQ ({kq}) không được trước NGAY_TH_YL ({th})");
            }

            // 40. MA_PTTT – enum {1, 2, 3}
            long? maPttt = ToInt(MA_PTTT);
            if (MA_PTTT != null && maPttt != null)
            {
                if (maPttt != 1 && maPttt != 2 && maPttt != 3)
                    errs.Add($"MA_PTTT: '{MA_PTTT}' không hợp lệ, " +
                             "chỉ chấp nhận 1 (phí dịch vụ) / 2 (định suất) / 3 (DRG)");
            }

            // 41. VET_THUONG_TP – chỉ ghi "1" hoặc để trống
            if (VET_THUONG_TP != null)
            {
                if (ToInt(VET_THUONG_TP) != 1)
                    errs.Add($"VET_THUONG_TP: '{VET_THUONG_TP}' không hợp lệ, " +
                             "chỉ chấp nhận '1' (vết thương tái phát) hoặc để trống");
            }

            // 42. PP_VO_CAM – enum {1, 2, 3, 4}
            long? ppVc = ToInt(PP_VO_CAM);
            if (PP_VO_CAM != null && ppVc != null)
            {
                if (ppVc != 1 && ppVc != 2 && ppVc != 3 && ppVc != 4)
                    errs.Add($"PP_VO_CAM: '{PP_VO_CAM}' không hợp lệ, " +
                             "chỉ chấp nhận 1 (gây mê) / 2 (gây tê) / 3 (châm tê) / 4 (khác)");
            }

            // 44. MA_MAY – định dạng XX.n.Z1;Z2;Z3
            if (!string.IsNullOrEmpty(MA_MAY))
            {
                string rawMay = MA_MAY.Trim();
                // Split tối đa 2 lần theo "." → [XX, n, "serial1;serial2;..."]
                string[] parts = rawMay.Split(new[] { '.' }, 3);

                if (parts.Length < 3)
                {
                    errs.Add($"MA_MAY: '{rawMay}' sai định dạng, " +
                             "phải là XX.n.Z (VD: MD.1.0175420;0228420;0091720)");
                }
                else
                {
                    string xxRaw = parts[0], nRaw = parts[1], serialsRaw = parts[2];

                    // Mã nhóm máy: 2-3 ký tự chữ cái (tiếng Việt hoặc Latin)
                    if (!Rx.Full("[A-ZĐÁẮẶÂẤẬĂÊẾỆÔỐỘƠỚỢƯỨỰa-zđ]{2,3}", xxRaw,
                                 RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
                        errs.Add($"MA_MAY: mã nhóm máy '{xxRaw}' phải là 2-3 ký tự chữ cái " +
                                 "(VD: HH, VS, MD, MRI, PET, DSA, ĐT…)");

                    // Nguồn kinh phí: 1 / 2 / 3[...]
                    if (!Rx.Full(@"[123](\[.+?])?", nRaw))
                        errs.Add($"MA_MAY: nguồn kinh phí '{nRaw}' không hợp lệ. " +
                                 "Phải là: 1 (NSNN), 2 (tài trợ/viện trợ/hỗ trợ), " +
                                 "3[tên nguồn] (nguồn khác – VD: 3[vaynganhang])");

                    // Phần serial: không được rỗng
                    if (serialsRaw.Trim().Length == 0)
                    {
                        errs.Add($"MA_MAY: thiếu số serial sau '{xxRaw}.{nRaw}.'");
                    }
                    else
                    {
                        // Chỉ kiểm tra không có mảnh nào rỗng giữa hai dấu ";"
                        string[] serials = serialsRaw.Split(';');
                        var emptyIdx = new List<string>();
                        for (int i = 0; i < serials.Length; i++)
                            if (serials[i].Trim().Length == 0)
                                emptyIdx.Add((i + 1).ToString());
                        if (emptyIdx.Count > 0)
                            errs.Add($"MA_MAY: serial rỗng tại vị trí [{string.Join(", ", emptyIdx)}] " +
                                     $"(kiểm tra dấu ';' thừa trong '{serialsRaw}')");
                    }
                }
            }

            // 46. TAI_SU_DUNG – chỉ ghi "1" hoặc để trống
            if (TAI_SU_DUNG != null)
            {
                if (ToInt(TAI_SU_DUNG) != 1)
                    errs.Add($"TAI_SU_DUNG: '{TAI_SU_DUNG}' không hợp lệ, " +
                             "chỉ chấp nhận '1' (VTYT tái sử dụng) hoặc để trống");
            }

            // Cross-field: MA_DICH_VU / MA_VAT_TU – ít nhất 1 phải có giá trị
            if (string.IsNullOrEmpty(MA_DICH_VU) && string.IsNullOrEmpty(MA_VAT_TU))
                errs.Add("MA_DICH_VU và MA_VAT_TU: ít nhất 1 trường phải có giá trị");

            // Cross-field: PHAM_VI = 1 → DON_GIA_BH > 0
            if (phamVi == 1 && dgbh != null && dgbh.Value == 0)
            {
                string maDvStr = MA_DICH_VU ?? "";
                if (!(maDvStr.EndsWith(".0000") || maDvStr.EndsWith("_TB") || maDvStr.EndsWith("_TT")))
                    errs.Add("DON_GIA_BH = 0 khi PHAM_VI = 1: " +
                             "chỉ hợp lệ khi DVKT chưa có mức giá (mã kết thúc .0000/_TB/_TT)");
            }

            // Cross-field: TAI_SU_DUNG = 1 → DON_GIA_BV phải = DON_GIA_BH
            if (TAI_SU_DUNG != null && ToInt(TAI_SU_DUNG) == 1 && dgbv != null && dgbh != null)
            {
                if (Math.Abs(dgbv.Value - dgbh.Value) > 0.001)
                    errs.Add($"TAI_SU_DUNG=1: DON_GIA_BV ({F(dgbv.Value)}) phải = DON_GIA_BH ({F(dgbh.Value)}) " +
                             "theo quy định VTYT tái sử dụng");
            }

            return errs;
        }
    }
}
