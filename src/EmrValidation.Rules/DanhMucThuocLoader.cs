using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EmrValidation.Core.Helpers;
using EmrValidation.Excel;

namespace EmrValidation.Rules
{
    /// <summary>1 dòng trong danh mục thuốc trúng thầu.</summary>
    public class DrugEntry
    {
        public string MaThuoc { get; set; }
        public string SoDangKy { get; set; }
        public string TenThuoc { get; set; }
        public string TenHoatChat { get; set; }
        public string DonViTinh { get; set; }
        public string HamLuong { get; set; }
        public string DuongDung { get; set; }
        public string MaDuongDung { get; set; }
        public string DangBaoChe { get; set; }
        public double? DonGia { get; set; }
        public double? DonGiaBh { get; set; }
        public double? SoLuong { get; set; }
        public string TtThau { get; set; }
        /// <summary>DATE8.</summary>
        public string TuNgay { get; set; }
        /// <summary>DATE8, "" = còn hiệu lực.</summary>
        public string DenNgay { get; set; }
        public string MaCskcb { get; set; }
        /// <summary>"1" = hóa dược, "2" = YHCT.</summary>
        public string LoaiThuoc { get; set; }
        /// <summary>"1" = thầu tập trung, "2" = địa phương.</summary>
        public string LoaiThau { get; set; }
        /// <summary>ICD yêu cầu, nhiều mã phân cách ";", "" = không yêu cầu.</summary>
        public string IcdReq { get; set; }
        /// <summary>ICD chống chỉ định, nhiều mã phân cách ";", "" = không có (cột ICD_NEG là tùy chọn).</summary>
        public string IcdNeg { get; set; }
        /// <summary>Dòng trong file gốc (để truy vết).</summary>
        public int RowIndex { get; set; }

        private static string Left8(string s)
        {
            s = s ?? "";
            return s.Length > 8 ? s.Substring(0, 8) : s;
        }

        /// <summary>Kiểm tra entry có hiệu lực tại ngày DATE8 cho trước. Không phân tích được ngày → coi là còn hiệu lực.</summary>
        public bool IsValidOn(string ngay)
        {
            DateTime d;
            if (!PyStrptime.TryParseYmd(Left8(ngay), out d)) return true;   // không parse được → không lọc

            if (!string.IsNullOrEmpty(TuNgay))
            {
                DateTime tu;
                if (!PyStrptime.TryParseYmd(Left8(TuNgay), out tu)) return true;
                if (d < tu) return false;
            }
            if (!string.IsNullOrEmpty(DenNgay))
            {
                DateTime den;
                if (!PyStrptime.TryParseYmd(Left8(DenNgay), out den)) return true;
                if (d > den) return false;
            }
            return true;
        }
    }

    /// <summary>
    /// Đọc và đánh chỉ mục file danh mục thuốc trúng thầu (sheet đầu tiên, dòng 1 là tiêu đề).
    /// Các cột: MA_THUOC, SO_DANG_KY, TEN_THUOC, TEN_HOAT_CHAT, DON_VI_TINH, HAM_LUONG, MA_DUONG_DUNG,
    /// DANG_BAO_CHE, DON_GIA, DON_GIA_BH, SO_LUONG, TT_THAU, TU_NGAY, DEN_NGAY, MA_CSKCB, LOAI_THUOC, LOAI_THAU, ICD_REQ, ICD_NEG (tùy chọn).
    /// Tên cột so khớp chính xác. Load một lần, tra cứu nhiều lần.
    /// </summary>
    public class DanhMucThuocLoader
    {
        private static readonly IList<DrugEntry> NoEntries = new DrugEntry[0];

        private readonly List<DrugEntry> _entries = new List<DrugEntry>();
        private readonly Dictionary<string, List<DrugEntry>> _idxMaSdk = new Dictionary<string, List<DrugEntry>>();
        private readonly Dictionary<string, List<DrugEntry>> _idxMa = new Dictionary<string, List<DrugEntry>>();
        private readonly Dictionary<string, List<DrugEntry>> _idxSdk = new Dictionary<string, List<DrugEntry>>();

        public string FilePath { get; private set; }

        /// <summary>Tập tất cả MA_THUOC có trong danh mục.</summary>
        public HashSet<string> MaThuocSet { get; private set; }

        /// <summary>Tập tất cả SO_DANG_KY (khác rỗng) có trong danh mục.</summary>
        public HashSet<string> SoDangKySet { get; private set; }

        /// <summary>Đọc từ file Excel.</summary>
        public DanhMucThuocLoader(string filepath)
        {
            if (!File.Exists(filepath))
                throw new FileNotFoundException("Không tìm thấy file danh mục thuốc: " + filepath);
            FilePath = filepath;
            MaThuocSet = new HashSet<string>();
            SoDangKySet = new HashSet<string>();
            Load(ExcelTableReader.ReadFirstSheet(filepath));
        }

        /// <summary>Dựng từ bảng đã đọc sẵn (dùng khi kiểm thử hoặc nguồn dữ liệu khác).</summary>
        public DanhMucThuocLoader(ExcelTable table, string label)
        {
            FilePath = label;
            MaThuocSet = new HashSet<string>();
            SoDangKySet = new HashSet<string>();
            Load(table);
        }

        private static string G(IDictionary<string, string> row, string col)
        {
            return ExcelTable.Get(row, col).Trim();
        }

        private static double? Gf(IDictionary<string, string> row, string col)
        {
            string v = G(row, col);
            return v.Length > 0 ? ParseHelper.ToFloat(v) : null;
        }

        private void Load(ExcelTable table)
        {
            for (int i = 0; i < table.Rows.Count; i++)
            {
                IDictionary<string, string> row = table.Rows[i];
                var entry = new DrugEntry
                {
                    MaThuoc = G(row, "MA_THUOC"),
                    SoDangKy = G(row, "SO_DANG_KY"),
                    TenThuoc = G(row, "TEN_THUOC"),
                    TenHoatChat = G(row, "TEN_HOAT_CHAT"),
                    DonViTinh = G(row, "DON_VI_TINH"),
                    HamLuong = G(row, "HAM_LUONG"),
                    DuongDung = G(row, "MA_DUONG_DUNG"),
                    MaDuongDung = G(row, "MA_DUONG_DUNG"),
                    DangBaoChe = G(row, "DANG_BAO_CHE"),
                    DonGia = Gf(row, "DON_GIA"),
                    DonGiaBh = Gf(row, "DON_GIA_BH"),
                    SoLuong = Gf(row, "SO_LUONG"),
                    TtThau = G(row, "TT_THAU"),
                    TuNgay = G(row, "TU_NGAY"),
                    DenNgay = G(row, "DEN_NGAY"),
                    MaCskcb = G(row, "MA_CSKCB"),
                    LoaiThuoc = G(row, "LOAI_THUOC"),
                    LoaiThau = G(row, "LOAI_THAU"),
                    IcdReq = G(row, "ICD_REQ"),
                    IcdNeg = G(row, "ICD_NEG"),
                    RowIndex = i + 2,   // dòng 1 là tiêu đề
                };
                if (entry.MaThuoc.Length == 0) continue;

                _entries.Add(entry);
                Add(_idxMaSdk, entry.MaThuoc + "\u0001" + entry.SoDangKy, entry);
                Add(_idxMa, entry.MaThuoc, entry);
                Add(_idxSdk, entry.SoDangKy, entry);
                MaThuocSet.Add(entry.MaThuoc);
                if (entry.SoDangKy.Length > 0) SoDangKySet.Add(entry.SoDangKy);
            }
        }

        private static void Add(Dictionary<string, List<DrugEntry>> index, string key, DrugEntry entry)
        {
            List<DrugEntry> list;
            if (!index.TryGetValue(key, out list))
            {
                list = new List<DrugEntry>();
                index[key] = list;
            }
            list.Add(entry);
        }

        private static IList<DrugEntry> Get(Dictionary<string, List<DrugEntry>> index, string key)
        {
            List<DrugEntry> list;
            return index.TryGetValue(key, out list) ? (IList<DrugEntry>)list : NoEntries;
        }

        /// <summary>
        /// Tra cứu danh mục thuốc.
        /// - Có cả maThuoc + soDangKy → tìm chính xác cặp đó.
        /// - Chỉ có maThuoc → tất cả dòng cùng MA_THUOC; chỉ có soDangKy → tất cả dòng cùng SO_DANG_KY.
        /// - ngayYl (DATE8/12): lọc thêm theo hiệu lực TU_NGAY/DEN_NGAY.
        /// </summary>
        public IList<DrugEntry> Lookup(string maThuoc = null, string soDangKy = null, string ngayYl = null)
        {
            IList<DrugEntry> results;
            bool hasMa = !string.IsNullOrEmpty(maThuoc);
            bool hasSdk = !string.IsNullOrEmpty(soDangKy);
            if (hasMa && hasSdk)
                results = Get(_idxMaSdk, maThuoc.Trim() + "\u0001" + soDangKy.Trim());
            else if (hasMa)
                results = Get(_idxMa, maThuoc.Trim());
            else if (hasSdk)
                results = Get(_idxSdk, soDangKy.Trim());
            else
                results = _entries;

            if (!string.IsNullOrEmpty(ngayYl))
            {
                string ngay8 = ngayYl.Length > 8 ? ngayYl.Substring(0, 8) : ngayYl;
                results = results.Where(e => e.IsValidOn(ngay8)).ToList();
            }
            return results;
        }

        /// <summary>True nếu thuốc có trong danh mục (và còn hiệu lực tại ngày).</summary>
        public bool InCatalog(string maThuoc = null, string soDangKy = null, string ngayYl = null)
        {
            return Lookup(maThuoc, soDangKy, ngayYl).Count > 0;
        }

        public int Total { get { return _entries.Count; } }

        public override string ToString()
        {
            return "DanhMucThuocLoader(file='" + Path.GetFileName(FilePath) + "', entries=" + Total +
                   ", ma_thuoc=" + MaThuocSet.Count + ")";
        }
    }
}