using System.Collections.Generic;
using EmrValidation.Core.GiamDinh;
using EmrValidation.Core.Helpers;
using EmrValidation.Core.Models;

namespace EmrValidation.Rules
{
    /// <summary>
    /// Rule I.2 (nhánh D004.1) – VTYT vượt mức chi quy định
    /// Căn cứ: khoản 2 Điều 3 Thông tư 04/2017/TT-BYT ngày 14/4/2017
    ///
    /// Áp dụng: tổng chi phí VTYT cho một lần Chụp, nong và đặt stent ĐMV
    /// (bao gồm cả bộ dụng cụ lấy huyết khối) vượt mức quy định.
    ///   - Tìm các MA_LK có dịch vụ Chụp/nong/đặt stent ĐMV trong XML3
    ///   - Tổng hợp THANH_TIEN_BH của tất cả VTYT (MA_NHOM = 4 hoặc 5) trong cùng MA_LK
    ///   - So sánh với ngưỡng quy định
    /// </summary>
    public class VTYTVuotMuc : GiamDinhBase
    {
        private static readonly HashSet<string> MaNongStent = new HashSet<string>
        {
            "17.106", "17.107", "17.108",
            "17.10600", "17.10601", "17.10602",
        };

        // Ngưỡng mức thanh toán VTYT theo TT 04/2017/TT-BYT (VNĐ). Cần rà soát theo phụ lục hiện hành.
        private const double MucChiVtytNongStent = 40000000;   // 40 triệu VNĐ / lần

        // Nhóm VTYT (MA_NHOM trong XML3): 4 = Vật tư thay thế, 5 = Vật tư tiêu hao đặc biệt
        private const string CanCu = "khoản 2 Điều 3 Thông tư 04/2017/TT-BYT ngày 14/4/2017";

        public override List<GiamDinhError> Check(IDictionary<string, List<BhytBase>> allObjects)
        {
            var errors = new List<GiamDinhError>();

            List<SheetRow> rowsXml3 = GetRows(allObjects, "XML3");
            Dictionary<string, List<SheetRow>> groups = GroupByMaLk(rowsXml3);

            foreach (KeyValuePair<string, List<SheetRow>> g in groups)
            {
                string maLk = g.Key;
                List<SheetRow> rows = g.Value;
                if (maLk.Length == 0) continue;

                // Lần khám có dịch vụ Nong + stent không
                bool hasNongStent = false;
                foreach (SheetRow r in rows)
                {
                    if (MaNongStent.Contains((r.Record.GetValue("MA_DICH_VU") ?? "").Trim()))
                    {
                        hasNongStent = true;
                        break;
                    }
                }
                if (!hasNongStent) continue;

                // Tổng VTYT (MA_NHOM thuộc nhóm VTYT)
                double tongVtyt = 0.0;
                var vtytRows = new List<int>();
                foreach (SheetRow r in rows)
                {
                    string maNhomRaw = r.Record.GetValue("MA_NHOM");
                    long? maNhom = string.IsNullOrEmpty(maNhomRaw) ? null : ParseHelper.ToInt(maNhomRaw);

                    if (maNhom == 4 || maNhom == 5)
                    {
                        string ttRaw = r.Record.GetValue("THANH_TIEN_BH");
                        double tt = 0.0;
                        if (!string.IsNullOrEmpty(ttRaw))
                            tt = ParseHelper.ToFloat(ttRaw.Replace(",", "")) ?? 0.0;
                        tongVtyt += tt;
                        vtytRows.Add(r.RowExcel);
                    }
                }

                if (tongVtyt > MucChiVtytNongStent)
                {
                    // Báo lỗi trên dòng cuối cùng của nhóm VTYT (dòng tổng)
                    int lastRow = vtytRows.Count > 0 ? vtytRows[vtytRows.Count - 1] : rows[rows.Count - 1].RowExcel;
                    errors.Add(new GiamDinhError
                    {
                        Sheet = "XML3",
                        MaLk = maLk,
                        RowExcel = lastRow,
                        MaLyDo = "D004.1",
                        MoTa = "Tổng VTYT cho kỹ thuật Chụp/nong/đặt stent ĐMV: " +
                               $"{PyFormat.Thousands0(tongVtyt)} đ – vượt mức quy định " +
                               $"{PyFormat.Thousands0(MucChiVtytNongStent)} đ. " +
                               $"Phần vượt: {PyFormat.Thousands0(tongVtyt - MucChiVtytNongStent)} đ.",
                        CanCu = CanCu,
                        MaDichVu = "NHOM_VTYT_4_5",
                    });
                }
            }

            return errors;
        }
    }
}
