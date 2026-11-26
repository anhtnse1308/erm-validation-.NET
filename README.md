# EmrValidation (.NET Framework 4.8)

**EmrValidation** là ứng dụng Console phát triển bằng C# trên nền tảng .NET Framework 4.8, dùng để kiểm tra tính hợp lệ của dữ liệu XML bệnh án điện tử và giám định thanh toán bảo hiểm y tế (BHYT). Ứng dụng thực hiện 8 bước kiểm tra, hỗ trợ BHYT thông thường và BHYT Quân đội/Quốc phòng, xuất nhật ký và báo cáo kết quả kiểm tra.

## Cấu trúc solution

| Project | Loại | Nội dung | Phụ thuộc |
|---|---|---|---|
| `EmrValidation.Core` | DLL | 12 model XML (XML1-5, 7-11, 13-14), `BhytBase`, hàm parse, `GiamDinhError`, `GiamDinhBase` | không |
| `EmrValidation.Excel` | DLL | Đọc Excel BHYT và file danh mục (`ExcelService`, `ExcelTableReader`), ghi log và báo cáo Excel, ClosedXML 0.95.4 | Core |
| `EmrValidation.Rules` | DLL | 12 rule giám định, 4 loader danh mục, `ICDHelper`, `GiamDinhService` | Core, Excel |
| `EmrValidation.App` | EXE (`EMRV.exe`) | Ứng dụng Console 8 bước | cả ba |

## Build và chạy

Cần Visual Studio 2019 trở lên (hoặc .NET SDK) và .NET Framework 4.8 Targeting Pack.
Mở `EmrValidation.sln` → Restore NuGet (ClosedXML) → Build.

Chạy `EMRV.exe`: menu chọn nguồn file
- **[1]** BHYT thường: đọc đường dẫn từ `PATH_CHECK_XML.txt`
- **[2]** BHYT Quân đội/QP: đọc từ `PATH_CHECK_XML_BQP.txt`
- **[3]** Nhập tay

Chạy nhanh không menu: `EMRV.exe "D:\duong_dan\file_bhyt.xlsx"` (không dùng file danh mục).

### Định dạng `PATH_CHECK_XML.txt` và `PATH_CHECK_XML_BQP.txt` (5 dòng, đọc theo vị trí)

| Dòng | Nội dung |
|---|---|
| 1 | File danh mục thuốc trúng thầu |
| 2 | File danh mục DVKT cần khai báo MA_MAY |
| 3 | File danh mục VTYT kèm DVKT |
| 4 | File danh mục DVKT kèm ICD_REQ |
| 5 | File Excel BHYT (**bắt buộc**) |

Dòng 1-4 có thể để trống nếu không dùng, nhưng **không được xóa dòng** vì chương trình đọc theo vị trí. Mẫu: `PATH_CHECK_XML.sample.txt`,
`PATH_CHECK_XML_BQP.sample.txt` (được chép cạnh `EMRV.exe`, đổi tên bỏ chữ `.sample` rồi sửa đường dẫn). File danh mục đọc **sheet đầu tiên**, dòng 1 là tiêu đề.

## 8 bước của ứng dụng

1. Validate cấu trúc dữ liệu XML (log text/JSON, báo cáo Excel)
2. Giám định thanh toán BHYT (8 rule do `GiamDinhService` chạy)
3. Thuốc theo danh mục trúng thầu (`WARN.DM01-07`)
4. Ngày giường nội trú (`WARN.GG01-02`)
5. Sàng lọc mã đối tượng KCB (`WARN.DT01-05`)
6. MA_MAY của DVKT (`WARN.MM01`)
7. VTYT kèm DVKT (`WARN.VT01`)
8. DVKT kèm ICD_REQ (`WARN.DV01`)

Mỗi bước xuất một file Excel cạnh file BHYT gốc. Bước 3, 6, 7, 8 bỏ qua nếu không có file danh mục; lỗi nạp danh mục (thiếu file, thiếu cột) chỉ làm bỏ qua bước đó.

## Đặc điểm xử lý dữ liệu

1. **Dữ liệu số:** sử dụng `double`, không dùng `decimal`; `PyFormat` hỗ trợ định dạng số thực và làm tròn đến 2 chữ số thập phân.
2. **Thứ tự lỗi:** các kiểm tra `_NUMERIC`, `_DATE12`, `_DATE8` thực hiện theo thứ tự khai báo trường dữ liệu.
3. **Đọc Excel:** chỉ đọc các cột có tiêu đề trùng tên trường của model; bỏ qua cột không khớp. Các giá trị `NA`, `N/A`, `NULL`, `null`, `nan`, `None`, `#N/A`, `n/a`, `<NA>`... được chuẩn hóa thành chuỗi rỗng khi đọc ô văn bản (`PandasNa.cs`). Ô lỗi (`#N/A`, `#REF!`) không làm dừng chương trình.
4. **Cấu hình danh mục:** dòng 1–4 để trống thì bỏ qua bước tương ứng và không hỏi lại; chỉ hỏi khi chọn chế độ nhập tay.
5. **Ghi báo cáo:** dữ liệu được kiểm tra một lần thông qua `ValidationReport`, kết quả được dùng chung cho các hàm ghi log. Các DLL không trực tiếp gọi `Console.WriteLine`.

## Phạm vi và giới hạn hiện tại

- Hỗ trợ 12 model: XML1–XML5, XML7–XML11, XML13–XML14; chưa có model XML6 và XML12.
- Hàm `validate()` của Bảng 3, 4, 5, 7, 8 không gọi hàm kiểm tra của lớp cơ sở.
- Kiểm tra giấy chuyển tuyến trong `SangLocDoiTuong` đang bị tắt; nhánh phát sinh mã `S079.1-WARN` chưa được thực thi.

## Kiểm thử và xác nhận trước khi triển khai

Theo kết quả kiểm thử được ghi nhận trong tài liệu dự án:

- Model XML: 80.000 bản ghi ngẫu nhiên đã được kiểm thử nội dung lỗi.
- Nhật ký: đã kiểm thử log văn bản, log chỉ chứa lỗi và JSON (khoảng 400 KB).
- Rule giám định: đã kiểm thử 800 bộ dữ liệu nhiều bảng, bao gồm 10 rule và `GiamDinhService`.
- Rule danh mục: đã kiểm thử 530 bộ dữ liệu với 4 loại danh mục, gồm trường hợp thiếu cột, khác biệt hoa/thường, trùng mã và ngày không hợp lệ.

**Lưu ý:** các thành phần sử dụng ClosedXML (`ExcelService`, `ExcelTableReader`, `ReportWriter` và các `*ExcelWriter`) mới được biên dịch thử với thư viện giả lập, chưa được xác nhận chạy với thư viện thật. Trước khi triển khai cần:

- Build solution với ClosedXML 0.95.4 thực tế.
- Kiểm tra mã có số 0 đứng đầu (ví dụ `01234`) được giữ dưới dạng văn bản trong Excel đầu ra (`SetText`).
- Chạy thử với file Excel BHYT và các file danh mục thực tế; đối chiếu số lượng và nội dung lỗi từng sheet với kết quả kỳ vọng.
- Kiểm tra định dạng báo cáo Excel: màu sắc, độ rộng cột và ô gộp.
