using System.Collections.Generic;
using System.Linq;
using EmrValidation.Core.GiamDinh;
using EmrValidation.Core.Models;

namespace EmrValidation.Rules
{
    /// <summary>
    /// Rule I.2 – Dịch vụ kỹ thuật nằm trong quy trình của DVKT khác (mã từ chối S054.1)
    /// Căn cứ: QĐ 3983/QĐ-BYT (Tim mạch), QĐ 3906/QĐ-BYT (Nhãn khoa), QĐ 25/QĐ-BYT (Chẩn đoán hình ảnh),
    /// QĐ 654/QĐ-BYT (Cơ xương khớp) và Thông báo chuyên đề tháng 12/2022 (PDF 477).
    ///
    /// Nguyên tắc: nếu trong cùng 1 MA_LK có cả DVKT "cha" và DVKT "con" thì dịch vụ "con" không được thanh toán riêng.
    /// Detect: group XML3 theo MA_LK, kiểm tra tập MA_DICH_VU trong từng nhóm.
    ///
    /// Bảng quy tắc <see cref="Rules"/> được sinh trực tiếp từ bản Python (giữ nguyên từng mã).
    /// Thêm rule mới: thêm một dòng vào bảng, không cần sửa logic Check().
    /// </summary>
    public class DichVuTrongQuyTrinh : GiamDinhBase
    {
        private sealed class ConcurrentRule
        {
            public readonly HashSet<string> Cha;
            public readonly HashSet<string> Con;
            public readonly string MoTa;
            public readonly string CanCu;

            public ConcurrentRule(string[] cha, string[] con, string moTa, string canCu)
            {
                Cha = new HashSet<string>(cha);
                Con = new HashSet<string>(con);
                MoTa = moTa;
                CanCu = canCu;
            }
        }

        // (tập mã DVKT cha, tập mã DVKT con, mô tả, căn cứ)
        private static readonly ConcurrentRule[] Rules =
        {
            new ConcurrentRule(
                new[] { "17.106", "17.10600", "17.10601", "17.10602", "17.107", "17.108" },
                new[] { "17.060", "17.06000", "17.06001", "17.061", "17.062" },
                "Chụp động mạch vành dưới DSA là bước trong quy trình Nong+đặt stent ĐMV (QT số 22). Không thanh toán Chụp ĐMV khi thực hiện đồng thời Nong+đặt stent ĐMV.",
                "QĐ 3983/QĐ-BYT ngày 03/10/2014 (QT số 22) + QĐ 7435/QĐ-BYT ngày 14/12/2018"),
            new ConcurrentRule(
                new[] { "17.106", "17.10600", "17.10601", "17.10602", "17.107", "17.108" },
                new[] { "17.085", "17.08500", "17.086", "17.08600" },
                "Hút huyết khối ĐMV đồng thời Chụp/nong/đặt stent ĐMV: vật tư lấy huyết khối đã tính trong gói dịch vụ. Không thanh toán riêng Hút huyết khối.",
                "QĐ 3983/QĐ-BYT ngày 03/10/2014 (QT số 22) + QĐ 7435/QĐ-BYT ngày 14/12/2018"),
            new ConcurrentRule(
                new[] { "27.008", "27.00800", "27.00801" },
                new[] { "27.015", "27.01500", "27.01501" },
                "Cắt mống mắt chu biên là bước trong quy trình Cắt bè củng giác mạc (QT số 8). Không thanh toán Cắt mống mắt chu biên khi thực hiện đồng thời.",
                "QĐ 3906/QĐ-BYT ngày 12/10/2012 (QT số 8)"),
            new ConcurrentRule(
                new[] { "27.069", "27.06900", "27.06901" },
                new[] { "27.015", "27.01500", "27.01501" },
                "Cắt mống mắt chu biên là bước trong quy trình Lấy TTT trong bao (QT số 69). Không thanh toán Cắt mống mắt chu biên khi thực hiện đồng thời.",
                "QĐ 3906/QĐ-BYT ngày 12/10/2012 (QT số 69)"),
            new ConcurrentRule(
                new[] { "18.145", "18.14500", "18.146", "18.14600", "18.163", "18.16300" },
                new[] { "18.009", "18.00900", "18.010", "18.01000", "18.SAU" },
                "Siêu âm hạch/u là bước trong quy trình Chọc sinh thiết u/hạch dưới SA (QT 145,146,163). Không thanh toán SA hạch/u khi thực hiện đồng thời.",
                "QĐ 25/QĐ-BYT ngày 03/01/2014 (QT 145, 146, 163)"),
            new ConcurrentRule(
                new[] { "18.150", "18.15000" },
                new[] { "18.011", "18.01100", "18.012", "18.01200" },
                "Siêu âm tuyến giáp là bước trong quy trình Chọc nang tuyến giáp dưới SA (QT 150). Không thanh toán SA tuyến giáp khi thực hiện đồng thời.",
                "QĐ 25/QĐ-BYT ngày 03/01/2014 (QT 150)"),
            new ConcurrentRule(
                new[] { "18.073", "18.074", "18.075", "18.076", "18.077", "18.078", "18.079", "18.080", "18.081", "18.082", "18.083", "18.084", "18.085", "18.164", "18.165" },
                new[] { "18.020", "18.02000", "18.021", "18.02100", "18.022", "18.02200" },
                "Siêu âm khớp/phần mềm là bước trong quy trình Tiêm gân/khớp dưới SA hoặc Chọc hút dịch khớp. Không thanh toán SA khớp/phần mềm khi thực hiện đồng thời.",
                "QĐ 25/QĐ-BYT ngày 03/01/2014 (QT 164, 165) + QĐ 654/QĐ-BYT ngày 24/02/2014 (QT 73-85)"),
            new ConcurrentRule(
                new[] { "20.023", "20.02300", "20.02301" },
                new[] { "20.001", "20.00100", "20.00101", "20.002", "20.00200" },
                "Mở bụng thăm dò/lau rửa ổ bụng/đặt dẫn lưu là bước trong quy trình Khâu lỗ thủng dạ dày hành tá tràng (QT số 23). Không thanh toán riêng.",
                "QĐ 201/QĐ-BYT ngày 16/01/2014 (QT ngoại khoa tiêu hóa, QT số 23)"),
            new ConcurrentRule(
                new[] { "22.050", "22.05000", "22.05001", "22.05002" },
                new[] { "22.010", "22.01000", "22.01001" },
                "Mở sào bào là bước trong quy trình PT tạo hình tai giữa. Không thanh toán Mở sào bào khi thực hiện đồng thời.",
                "Phụ lục 02 CV /GĐĐT-NVGĐ tháng 12/2022 – KT01_1 (S054.1)"),
            new ConcurrentRule(
                new[] { "22.070", "22.07000", "22.07001" },
                new[] { "22.005", "22.00500", "22.00501" },
                "PT mở khí quản là bước trong quy trình PT mở sụn giáp dây thanh. Không thanh toán PT mở khí quản khi thực hiện đồng thời.",
                "Phụ lục 02 CV /GĐĐT-NVGĐ tháng 12/2022 – KT01_2 (S054.1)"),
            new ConcurrentRule(
                new[] { "20.100", "20.10000", "20.10001", "20.10002" },
                new[] { "20.110", "20.11000", "20.11001" },
                "Cắt nang thừng tinh là bước trong quy trình PT thoát vị bẹn. Không thanh toán Cắt nang thừng tinh khi thực hiện đồng thời.",
                "Phụ lục 02 CV /GĐĐT-NVGĐ tháng 12/2022 – KT01_3 (S054.1)"),
            new ConcurrentRule(
                new[] { "09.050", "09.05000", "09.05001", "09.05002" },
                new[] { "09.010", "09.01000", "09.01001" },
                "Vi khuẩn nhuộm soi là bước trong quy trình Vi khuẩn nuôi cấy định danh. Không thanh toán Nhuộm soi khi thực hiện đồng thời.",
                "Phụ lục 02 CV /GĐĐT-NVGĐ tháng 12/2022 – KT01_4 (S054.1)"),
            new ConcurrentRule(
                new[] { "23.050", "23.05000", "23.051", "23.05100", "23.052", "23.05200" },
                new[] { "23.010", "23.01000", "23.011", "23.01100" },
                "PT vá/tạo hình màng cứng là bước trong quy trình PT u não. Không thanh toán riêng.",
                "Phụ lục 02 CV /GĐĐT-NVGĐ tháng 12/2022 – KT01_5 (S054.1)"),
            new ConcurrentRule(
                new[] { "20.030", "20.03000", "20.031", "20.03100", "20.032", "20.03200", "20.033", "20.03300" },
                new[] { "20.200", "20.20000", "20.20001" },
                "Nạo vét hạch là bước trong quy trình PT dạ dày. Không thanh toán Nạo vét hạch khi thực hiện đồng thời.",
                "Phụ lục 02 CV /GĐĐT-NVGĐ tháng 12/2022 – KT01_6 (S054.1)"),
            new ConcurrentRule(
                new[] { "20.060", "20.06000", "20.061", "20.06100", "20.062", "20.06200" },
                new[] { "20.070", "20.07000", "20.07001" },
                "Cắt túi mật là bước trong quy trình PT gan. Không thanh toán Cắt túi mật khi thực hiện đồng thời.",
                "Phụ lục 02 CV /GĐĐT-NVGĐ tháng 12/2022 – KT01_10 (S054.1)"),
            new ConcurrentRule(
                new[] { "23.100", "23.10000", "23.101", "23.10100", "23.102", "23.10200", "23.103" },
                new[] { "23.200", "23.20000", "23.20001" },
                "PT ghép xương là bước trong quy trình PT cột sống. Không thanh toán PT ghép xương khi thực hiện đồng thời.",
                "Phụ lục 02 CV /GĐĐT-NVGĐ tháng 12/2022 – KT01_12 (S054.1)"),
            new ConcurrentRule(
                new[] { "20.140", "20.14000", "24.010", "24.01000", "24.050", "24.05000" },
                new[] { "24.200", "24.20000", "24.20001" },
                "Dẫn lưu cùng đồ Douglas là bước trong quy trình PT cắt ruột thừa / chửa ngoài TC vỡ / lấy thai. Không thanh toán riêng.",
                "Phụ lục 02 CV /GĐĐT-NVGĐ tháng 12/2022 – KT01_16 (S054.1)"),
            new ConcurrentRule(
                new[] { "36.050", "36.05000", "36.051", "36.05100", "36.052", "36.05200", "36.053" },
                new[] { "36.010", "36.01000", "36.011", "36.01100" },
                "Nội soi chẩn đoán TQDDTT là bước trong quy trình Nội soi can thiệp DDTT. Không thanh toán Nội soi chẩn đoán khi thực hiện đồng thời can thiệp.",
                "Phụ lục 02 CV /GĐĐT-NVGĐ tháng 12/2022 – KT01_18 (S054.1)"),
            new ConcurrentRule(
                new[] { "36.070", "36.07000", "36.071", "36.07100", "36.072" },
                new[] { "36.060", "36.06000", "36.06001" },
                "Nội soi đại trực tràng toàn bộ là bước trong quy trình Can thiệp ống tiêu hóa dưới. Không thanh toán Nội soi chẩn đoán khi thực hiện đồng thời can thiệp.",
                "Phụ lục 02 CV /GĐĐT-NVGĐ tháng 12/2022 – KT01_19 (S054.1)"),
            new ConcurrentRule(
                new[] { "28.050", "28.05000", "28.051", "28.05100", "28.052", "28.05200" },
                new[] { "28.020", "28.02000", "28.02001" },
                "Đặt sond JJ là bước trong quy trình Nội soi tán sỏi niệu quản. Không thanh toán Đặt sond JJ khi thực hiện đồng thời.",
                "Phụ lục 02 CV /GĐĐT-NVGĐ tháng 12/2022 – KT01_21 (S054.1)"),
            new ConcurrentRule(
                new[] { "17.200", "17.20000", "17.20001" },
                new[] { "17.001", "17.00100" },
                "Đo điện tâm đồ là bước trong quy trình Lập trình máy tạo nhịp. Không thanh toán Đo ĐTĐ khi thực hiện đồng thời.",
                "Phụ lục 02 CV /GĐĐT-NVGĐ tháng 12/2022 – KT189_2 (S054.1)"),
            new ConcurrentRule(
                new[] { "31.106", "31.10600" },
                new[] { "31.001", "31.00100", "31.002", "31.00200" },
                "Thay băng bỏng là bước trong quy trình Rạch hoại tử bỏng giải thoát chèn ép. Không thanh toán Thay băng bỏng khi thực hiện đồng thời.",
                "Phụ lục 02 CV /GĐĐT-NVGĐ tháng 12/2022 – KT189_3 (S054.1)"),
            new ConcurrentRule(
                new[] { "31.107", "31.10700" },
                new[] { "31.001", "31.00100", "31.002", "31.00200" },
                "Thay băng bỏng là bước trong quy trình Khâu cầm máu bỏng sâu. Không thanh toán Thay băng bỏng khi thực hiện đồng thời.",
                "Phụ lục 02 CV /GĐĐT-NVGĐ tháng 12/2022 – KT189_4 (S054.1)"),
            new ConcurrentRule(
                new[] { "24.001", "24.00100", "24.002", "24.00200" },
                new[] { "24.108", "24.10800" },
                "Làm lại thành âm đạo/tầng sinh môn là bước trong quy trình Đẻ thường/Đẻ khó. Không thanh toán riêng.",
                "Phụ lục 02 CV /GĐĐT-NVGĐ tháng 12/2022 – KT189_5 (S054.1)"),
            new ConcurrentRule(
                new[] { "27.088", "27.08800", "27.08801" },
                new[] { "27.005", "27.00500" },
                "Đo khúc xạ giác mạc Javal là bước trong quy trình Tính công suất TTT (QT số 88). Không thanh toán Đo khúc xạ Javal khi thực hiện đồng thời.",
                "QĐ 3906/QĐ-BYT ngày 12/10/2012 (QT số 88) – KT189_6 (S054.1)"),
            new ConcurrentRule(
                new[] { "13.026", "13.02600", "13.02601" },
                new[] { "13.001", "13.00100", "13.00101" },
                "Đặt catheter lọc máu cấp cứu là bước trong quy trình Thận nhân tạo cấp cứu (QT 26). Không thanh toán riêng Đặt catheter.",
                "QĐ 2482/QĐ-BYT ngày 13/4/2018 (QT 26) – KT189_7 (S054.1)"),
            new ConcurrentRule(
                new[] { "15.009", "15.00900", "15.010", "15.01000", "15.011", "15.01100", "15.012", "15.013" },
                new[] { "15.020", "15.02000", "15.021", "15.02100", "15.022", "15.02200", "15.023", "15.024" },
                "Tiêm khớp là bước trong quy trình Hút dịch khớp (QT 9-23 QĐ 654/QĐ-BYT). Không thanh toán Tiêm khớp khi thực hiện đồng thời Hút dịch cùng vị trí.",
                "QĐ 654/QĐ-BYT ngày 24/02/2014 (QT 9-23) – KT189_8 (S054.1)"),
            new ConcurrentRule(
                new[] { "24.030", "24.03000", "24.03001" },
                new[] { "24.040", "24.04000", "24.041", "24.04100" },
                "PT cắt u buồng trứng/phần phụ là bước trong quy trình PT cắt tử cung toàn bộ + 2 phần phụ (QT số 30 – tử cung bao gồm vòi TC và buồng trứng). Không thanh toán riêng.",
                "QĐ 1377/QĐ-BYT ngày 24/4/2013 (QT số 30) – KT189_9 (S054.1)"),
            new ConcurrentRule(
                new[] { "36.010", "36.01000", "36.011", "36.01100", "36.050", "36.05000", "36.051", "36.05100", "36.052" },
                new[] { "09.1673", "09.167300" },
                "HP Ag test nhanh không được thanh toán khi thực hiện đồng thời Nội soi dạ dày/tá tràng theo điều kiện quy định tại TT 39/2018/TT-BYT (STT 1673).",
                "TT 39/2018/TT-BYT ngày 30/11/2018 (STT 1673) – KT32 (S023.1)"),
        };

        public override List<GiamDinhError> Check(IDictionary<string, List<BhytBase>> allObjects)
        {
            var errors = new List<GiamDinhError>();

            Dictionary<string, List<SheetRow>> groups = GroupByMaLk(GetRows(allObjects, "XML3"));

            foreach (KeyValuePair<string, List<SheetRow>> g in groups)
            {
                string maLk = g.Key;
                List<SheetRow> rows = g.Value;
                if (maLk.Length == 0) continue;

                // Tập mã DVKT trong lần khám này
                var dvSet = new HashSet<string>();
                foreach (SheetRow r in rows)
                    dvSet.Add((r.Record.GetValue("MA_DICH_VU") ?? "").Trim());

                foreach (ConcurrentRule rule in Rules)
                {
                    // Có bất kỳ dịch vụ cha nào?
                    if (!dvSet.Any(d => rule.Cha.Contains(d))) continue;

                    // Có bất kỳ dịch vụ con nào bị từ chối?
                    var matchedCon = new HashSet<string>(dvSet.Where(d => rule.Con.Contains(d)));
                    if (matchedCon.Count == 0) continue;

                    // Tìm các dòng chứa dịch vụ con → tạo lỗi
                    foreach (SheetRow r in rows)
                    {
                        string maDv = (r.Record.GetValue("MA_DICH_VU") ?? "").Trim();
                        if (matchedCon.Contains(maDv))
                        {
                            errors.Add(new GiamDinhError
                            {
                                Sheet = "XML3",
                                MaLk = maLk,
                                RowExcel = r.RowExcel,
                                MaLyDo = "S054.1",
                                MoTa = rule.MoTa,
                                CanCu = rule.CanCu,
                                MaDichVu = maDv,
                            });
                        }
                    }
                }
            }

            return errors;
        }
    }
}
