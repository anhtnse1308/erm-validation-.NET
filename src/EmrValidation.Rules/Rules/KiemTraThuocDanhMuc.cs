using System;
using System.Collections.Generic;
using System.Linq;
using EmrValidation.Core.GiamDinh;
using EmrValidation.Core.Helpers;
using EmrValidation.Core.Models;
using EmrValidation.Excel;

namespace EmrValidation.Rules
{
    /// <summary>
    /// Rule – Kiểm tra thuốc XML2 theo danh mục trúng thầu (luồng độc lập, không nằm trong GiamDinhService).
    ///   T1 – MA_THUOC không có trong danh mục                  → WARN.DM01
    ///   T2 – SO_DANG_KY không khớp với MA_THUOC trong danh mục → WARN.DM02
    ///   T3 – DON_GIA vượt DON_GIA_BH trong danh mục            → WARN.DM03
    ///   T4 – DUONG_DUNG không khớp với danh mục                → WARN.DM04
    ///   T5 – HAM_LUONG không khớp với danh mục                 → WARN.DM05
    ///   T6 – Thuốc hết hiệu lực tại thời điểm y lệnh           → WARN.DM06
    ///   T7 – MA_THUOC có ICD_REQ nhưng cả MA_BENH_CHINH và MA_BENH_KT
    ///        đều không có mã nào khớp                          → WARN.DM07
    ///   T8 – MA_THUOC có ICD_NEG (chống chỉ định) và MA_BENH_CHINH hoặc
    ///        MA_BENH_KT có ít nhất 1 mã trùng ICD_NEG             → WARN.DM08
    /// </summary>
    public class KiemTraThuocDanhMuc : GiamDinhBase
    {
        private const string CanCu = "Danh mục thuốc trúng thầu của cơ sở KCB";

        /// <summary>Sai số khi so sánh đơn giá (1% cho sai số làm tròn).</summary>
        private const double DonGiaTolerancePct = 0.01;

        private readonly DanhMucThuocLoader _loader;

        public KiemTraThuocDanhMuc(DanhMucThuocLoader loader)
        {
            _loader = loader;
        }

        /// <summary>Chuẩn hóa chuỗi để so sánh: chữ thường, bỏ khoảng trắng thừa.</summary>
        private static string Norm(string s)
        {
            return string.Join(" ", (s ?? "").ToLowerInvariant().Split((char[])null, StringSplitOptions.RemoveEmptyEntries));
        }

        private static double? ToFloatNoComma(string v)
        {
            return v == null ? null : ParseHelper.ToFloat(v.Replace(",", ""));
        }

        private static string Joined(IEnumerable<string> items)
        {
            return string.Join(", ", items);
        }

        public override List<GiamDinhError> Check(IDictionary<string, List<BhytBase>> allObjects)
        {
            var errors = new List<GiamDinhError>();

            // MA_LK → MA_BENH_CHINH, MA_LK → tập MA_BENH_KT (từ XML1)
            var xml1Map = new Dictionary<string, string>();
            var xml1KtMap = new Dictionary<string, HashSet<string>>();
            foreach (SheetRow r in GetRows(allObjects, "XML1"))
            {
                string maLk1 = (r.Record.GetValue("MA_LK") ?? "").Trim();
                if (maLk1.Length > 0)
                {
                    xml1Map[maLk1] = (r.Record.GetValue("MA_BENH_CHINH") ?? "").Trim().ToUpperInvariant();
                    // MA_BENH_KT: nhiều mã phân cách ";" → split, trim, upper, bỏ rỗng
                    var set = new HashSet<string>();
                    foreach (string x in (r.Record.GetValue("MA_BENH_KT") ?? "").Split(';'))
                    {
                        string t = x.Trim();
                        if (t.Length > 0) set.Add(t.ToUpperInvariant());
                    }
                    xml1KtMap[maLk1] = set;
                }
            }

            foreach (SheetRow row in GetRows(allObjects, "XML2"))
            {
                BhytBase rec = row.Record;
                string maLk = (rec.GetValue("MA_LK") ?? "").Trim();
                string maThuoc = (rec.GetValue("MA_THUOC") ?? "").Trim();
                string soDangKy = (rec.GetValue("SO_DANG_KY") ?? "").Trim();
                double? donGia = ToFloatNoComma(rec.GetValue("DON_GIA"));
                string duongDung = Norm(rec.GetValue("DUONG_DUNG"));
                string hamLuong = Norm(rec.GetValue("HAM_LUONG"));
                string ngayYl = (rec.GetValue("NGAY_YL") ?? "").Trim();

                if (maLk.Length == 0 || maThuoc.Length == 0) continue;

                // T1 – MA_THUOC không có trong danh mục
                if (!_loader.InCatalog(maThuoc))
                {
                    errors.Add(New(row, maLk, "WARN.DM01", maThuoc,
                        $"MA_THUOC '{maThuoc}' không có trong danh mục thuốc trúng thầu. " +
                        "Kiểm tra lại mã thuốc hoặc bổ sung vào danh mục."));
                    continue;   // không check tiếp nếu mã không tồn tại
                }

                // Các entry khớp MA_THUOC (có lọc hiệu lực nếu có ngày)
                IList<DrugEntry> entriesByMa = _loader.Lookup(maThuoc, null, ngayYl.Length > 0 ? ngayYl : null);

                // T6 – Thuốc hết hiệu lực tại thời điểm y lệnh
                if (ngayYl.Length > 0 && soDangKy.Length > 0)
                {
                    IList<DrugEntry> entriesSdkNgay = _loader.Lookup(maThuoc, soDangKy, ngayYl);
                    IList<DrugEntry> entriesSdkAll = _loader.Lookup(maThuoc, soDangKy);
                    if (entriesSdkNgay.Count == 0 && entriesSdkAll.Count > 0)
                    {
                        // Cặp (MA_THUOC, SDK) từng tồn tại nhưng hết hiệu lực
                        List<string> denList = entriesSdkAll
                            .Where(e => e.DenNgay.Length > 0).Select(e => e.DenNgay).Distinct()
                            .OrderByDescending(x => x, StringComparer.Ordinal).ToList();
                        string hetHan = denList.Count > 0 ? denList[0] : "?";
                        errors.Add(New(row, maLk, "WARN.DM06", maThuoc,
                            $"Thuốc '{maThuoc}' / SDK '{soDangKy}' " +
                            $"hết hiệu lực tại ngày y lệnh {ngayYl}. " +
                            $"Hiệu lực kết thúc: {hetHan}. " +
                            "Kiểm tra lại hợp đồng/quyết định thầu."));
                    }
                }

                // T2 – SO_DANG_KY không khớp với MA_THUOC trong danh mục
                IList<DrugEntry> workingEntries;
                if (soDangKy.Length > 0)
                {
                    IList<DrugEntry> entriesExact = _loader.Lookup(maThuoc, soDangKy, ngayYl.Length > 0 ? ngayYl : null);
                    if (entriesExact.Count == 0)
                    {
                        List<string> sdkyInCat = entriesByMa.Select(e => e.SoDangKy).Distinct()
                            .OrderBy(x => x, StringComparer.Ordinal).ToList();
                        errors.Add(New(row, maLk, "WARN.DM02", maThuoc,
                            $"SO_DANG_KY '{soDangKy}' không khớp với MA_THUOC '{maThuoc}' " +
                            "trong danh mục. " +
                            $"Số đăng ký trong danh mục: {Joined(sdkyInCat.Take(5))}."));
                        // Vẫn dùng entries_by_ma để check tiếp các trường khác
                        workingEntries = entriesByMa;
                    }
                    else
                    {
                        workingEntries = entriesExact;
                    }
                }
                else
                {
                    workingEntries = entriesByMa;
                }

                if (workingEntries.Count == 0) continue;

                // T3 – DON_GIA vượt DON_GIA_BH trong danh mục
                if (donGia != null)
                {
                    double? maxGiaBh = null;
                    foreach (DrugEntry e in workingEntries)
                        if (e.DonGiaBh != null && (maxGiaBh == null || e.DonGiaBh.Value > maxGiaBh.Value))
                            maxGiaBh = e.DonGiaBh;

                    if (maxGiaBh != null)
                    {
                        double tolerance = maxGiaBh.Value * DonGiaTolerancePct;
                        if (donGia.Value > maxGiaBh.Value + tolerance)
                        {
                            errors.Add(New(row, maLk, "WARN.DM03", maThuoc,
                                $"DON_GIA {PyFormat.Thousands0(donGia.Value)} đ vượt DON_GIA_BH tối đa " +
                                $"{PyFormat.Thousands0(maxGiaBh.Value)} đ trong danh mục " +
                                $"(chênh {PyFormat.Thousands0(donGia.Value - maxGiaBh.Value)} đ). " +
                                $"MA_THUOC='{maThuoc}', SO_DANG_KY='{soDangKy}'."));
                        }
                    }
                }

                // T4 – DUONG_DUNG không khớp với danh mục
                if (duongDung.Length > 0)
                {
                    var dmDuongDung = new HashSet<string>(
                        workingEntries.Where(e => e.DuongDung.Length > 0).Select(e => Norm(e.DuongDung)));
                    if (dmDuongDung.Count > 0 && !dmDuongDung.Contains(duongDung))
                    {
                        errors.Add(New(row, maLk, "WARN.DM04", maThuoc,
                            $"DUONG_DUNG '{duongDung}' không khớp với danh mục. " +
                            $"Danh mục: {Joined(dmDuongDung.OrderBy(x => x, StringComparer.Ordinal).Take(3))}."));
                    }
                }

                // T5 – HAM_LUONG không khớp với danh mục
                if (hamLuong.Length > 0)
                {
                    var dmHamLuong = new HashSet<string>(
                        workingEntries.Where(e => e.HamLuong.Length > 0).Select(e => Norm(e.HamLuong)));
                    if (dmHamLuong.Count > 0 && !dmHamLuong.Contains(hamLuong))
                    {
                        errors.Add(New(row, maLk, "WARN.DM05", maThuoc,
                            $"HAM_LUONG '{hamLuong}' không khớp với danh mục. " +
                            $"Danh mục: {Joined(dmHamLuong.OrderBy(x => x, StringComparer.Ordinal).Take(3))}."));
                    }
                }

                // T7 – ICD_REQ: MA_BENH_CHINH hoặc 1 mã bất kỳ trong MA_BENH_KT phải khớp tuyệt đối 1 trong các ICD yêu cầu
                string icdReqRaw = string.Join(";",
                    workingEntries.Where(e => e.IcdReq.Length > 0 && e.IcdReq.Trim().Length > 0).Select(e => e.IcdReq));
                if (icdReqRaw.Length > 0)
                {
                    // Tập ICD yêu cầu: split, trim, upper, bỏ rỗng
                    var icdReqSet = new HashSet<string>();
                    foreach (string x in icdReqRaw.Split(';'))
                    {
                        string t = x.Trim();
                        if (t.Length > 0) icdReqSet.Add(t.ToUpperInvariant());
                    }

                    string maBenhChinh;
                    if (!xml1Map.TryGetValue(maLk, out maBenhChinh)) maBenhChinh = "";
                    HashSet<string> maBenhKtSet;
                    if (!xml1KtMap.TryGetValue(maLk, out maBenhKtSet)) maBenhKtSet = new HashSet<string>();

                    // Tập mã bệnh của hồ sơ = MA_BENH_CHINH + các mã MA_BENH_KT
                    var icdHoSo = new HashSet<string>(maBenhKtSet);
                    if (maBenhChinh.Length > 0) icdHoSo.Add(maBenhChinh);

                    // Hợp lệ nếu có ít nhất 1 mã của hồ sơ khớp 1 mã trong ICD_REQ
                    if (icdReqSet.Count > 0 && !icdReqSet.Overlaps(icdHoSo))
                    {
                        string kt = Joined(maBenhKtSet.OrderBy(x => x, StringComparer.Ordinal));
                        errors.Add(New(row, maLk, "WARN.DM07", maThuoc,
                            $"Thuốc '{maThuoc}' yêu cầu MA_BENH_CHINH hoặc MA_BENH_KT " +
                            $"có mã thuộc {{{Joined(icdReqSet.OrderBy(x => x, StringComparer.Ordinal))}}} " +
                            $"nhưng MA_BENH_CHINH='{(maBenhChinh.Length > 0 ? maBenhChinh : "(trống)")}', " +
                            $"MA_BENH_KT='{(kt.Length > 0 ? kt : "(trống)")}'. " +
                            "So sánh tuyệt đối."));
                    }
                }

                // T8 – ICD_NEG: chống chỉ định. Nếu MA_BENH_CHINH hoặc 1 mã bất kỳ trong MA_BENH_KT
                //      trùng tuyệt đối 1 mã trong ICD_NEG thì báo lỗi (ngược với T7)
                string icdNegRaw = string.Join(";",
                    workingEntries.Where(e => e.IcdNeg.Length > 0 && e.IcdNeg.Trim().Length > 0).Select(e => e.IcdNeg));
                if (icdNegRaw.Length > 0)
                {
                    // Tập ICD chống chỉ định: split, trim, upper, bỏ rỗng
                    var icdNegSet = new HashSet<string>();
                    foreach (string x in icdNegRaw.Split(';'))
                    {
                        string t = x.Trim();
                        if (t.Length > 0) icdNegSet.Add(t.ToUpperInvariant());
                    }

                    string maBenhChinh;
                    if (!xml1Map.TryGetValue(maLk, out maBenhChinh)) maBenhChinh = "";
                    HashSet<string> maBenhKtSet;
                    if (!xml1KtMap.TryGetValue(maLk, out maBenhKtSet)) maBenhKtSet = new HashSet<string>();

                    // Tập mã bệnh của hồ sơ = MA_BENH_CHINH + các mã MA_BENH_KT
                    var icdHoSo = new HashSet<string>(maBenhKtSet);
                    if (maBenhChinh.Length > 0) icdHoSo.Add(maBenhChinh);

                    // Các mã bệnh của hồ sơ trùng với ICD_NEG
                    var trung = new HashSet<string>(icdNegSet);
                    trung.IntersectWith(icdHoSo);
                    if (trung.Count > 0)
                    {
                        string kt = Joined(maBenhKtSet.OrderBy(x => x, StringComparer.Ordinal));
                        errors.Add(New(row, maLk, "WARN.DM08", maThuoc,
                            $"Thuốc '{maThuoc}' chống chỉ định với mã bệnh " +
                            $"{{{Joined(trung.OrderBy(x => x, StringComparer.Ordinal))}}} " +
                            $"(ICD_NEG: {{{Joined(icdNegSet.OrderBy(x => x, StringComparer.Ordinal))}}}). " +
                            $"Hồ sơ có MA_BENH_CHINH='{(maBenhChinh.Length > 0 ? maBenhChinh : "(trống)")}', " +
                            $"MA_BENH_KT='{(kt.Length > 0 ? kt : "(trống)")}'. " +
                            "So sánh tuyệt đối."));
                    }
                }
            }

            return errors;
        }

        private static GiamDinhError New(SheetRow row, string maLk, string maLyDo, string maThuoc, string moTa)
        {
            return new GiamDinhError
            {
                Sheet = "XML2",
                MaLk = maLk,
                RowExcel = row.RowExcel,
                MaLyDo = maLyDo,
                MoTa = moTa,
                CanCu = CanCu,
                MaDichVu = maThuoc,
            };
        }

        /// <summary>
        /// Xuất kết quả kiểm tra danh mục thuốc: &lt;ten_goc&gt;[&lt;timestamp&gt;]_danhmuc.xlsx
        /// <paramref name="loader"/> dùng để điền TEN_HOAT_CHAT, TEN_THUOC ở sheet WARN_DM07 và WARN_DM08.
        /// </summary>
        public static string WriteExcel(string excelFile, IList<GiamDinhError> errors, string danhMucPath, DanhMucThuocLoader loader = null)
        {
            Func<string, KeyValuePair<string, string>> info = null;
            if (loader != null)
            {
                info = maThuoc =>
                {
                    if (string.IsNullOrEmpty(maThuoc)) return new KeyValuePair<string, string>("", "");
                    IList<DrugEntry> entries = loader.Lookup(maThuoc);
                    string tenHc = entries.Where(e => e.TenHoatChat.Length > 0).Select(e => e.TenHoatChat).FirstOrDefault() ?? "";
                    string tenT = entries.Where(e => e.TenThuoc.Length > 0).Select(e => e.TenThuoc).FirstOrDefault() ?? "";
                    return new KeyValuePair<string, string>(tenHc, tenT);
                };
            }
            return DanhMucExcelWriter.Write(excelFile, errors, danhMucPath, info);
        }
    }
}