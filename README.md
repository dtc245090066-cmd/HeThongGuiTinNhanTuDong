# BulkNotificationSystem

Hệ thống gửi thông báo hàng loạt – dự án thực hành môn Phát triển ứng dụng phân tán.

Dự án được xây dựng nhằm tìm hiểu và thực hành mô hình giao tiếp Client–Server, xây dựng RESTful API bằng ASP.NET Core Web API và sử dụng `HttpClient` để gửi HTTP Request từ ứng dụng máy khách đến máy chủ.

## 1. Giới thiệu dự án

**BulkNotificationSystem** là hệ thống quản lý chiến dịch gửi thông báo hàng loạt.

Trong phạm vi thực hành hiện tại, hệ thống tập trung vào chức năng quản lý chiến dịch thông báo thông qua RESTful API.

Các chức năng chính:

- Kiểm tra trạng thái hoạt động của API.
- Tạo chiến dịch thông báo.
- Lấy danh sách chiến dịch.
- Kiểm tra dữ liệu đầu vào.
- Trả về HTTP Status Code phù hợp.
- Sử dụng ứng dụng Console làm Client để giao tiếp với API.
- Kiểm thử các API bằng Postman.

## 2. Cấu trúc dự án

```text
BulkNotificationSystem/
│
├── BulkNotificationSystem.slnx
│
├── docs/
│   ├── client-server-communication.md
│   ├── message-exchange-patterns.md
│   ├── postman-testing.md
│   ├── system-architecture.md
│   ├── week-1-checklist.md
│   └── week-1-report.md
│
├── postman/
│   ├── BulkNotificationSystem.postman_collection.json
│   └── BulkNotificationSystem.postman_environment.json
│
└── src/
    ├── Campaign.Api/
    │   ├── Campaign.Api.csproj
    │   ├── Program.cs
    │   ├── DTOs/
    │   ├── Endpoints/
    │   ├── Models/
    │   └── Services/
    │
    └── Campaign.Client/
        ├── Campaign.Client.csproj
        ├── Program.cs
        ├── CampaignApiClient.cs
        ├── CampaignApiException.cs
        └── Models/
```

## 3. Công nghệ sử dụng

| Công nghệ | Mục đích |
|---|---|
| C# | Ngôn ngữ lập trình |
| .NET 10 | Nền tảng phát triển ứng dụng |
| ASP.NET Core Web API | Xây dựng RESTful API |
| HttpClient | Gửi HTTP Request từ Client |
| JSON | Định dạng trao đổi dữ liệu |
| Postman | Kiểm thử RESTful API |
| Git và GitHub | Quản lý mã nguồn |

Hiện tại, cả hai project đều sử dụng `net10.0` và chưa khai báo package NuGet bên ngoài.

## 4. Kiến trúc Client–Server

Hệ thống gồm hai thành phần chính:

### 4.1. Campaign.Api

Đây là thành phần máy chủ được xây dựng bằng ASP.NET Core Web API.

Nhiệm vụ:

- Tiếp nhận HTTP Request từ Client.
- Xử lý yêu cầu quản lý chiến dịch.
- Kiểm tra dữ liệu đầu vào.
- Trả về dữ liệu JSON.
- Trả về HTTP Status Code tương ứng với kết quả xử lý.

API hiện cung cấp các chức năng:

- `GET /`: Kiểm tra trạng thái API.
- `GET /health`: Kiểm tra tình trạng hoạt động của dịch vụ.
- `GET /api/campaigns`: Lấy danh sách chiến dịch.
- `POST /api/campaigns`: Tạo chiến dịch mới.

### 4.2. Campaign.Client

Đây là ứng dụng Console độc lập đóng vai trò máy khách.

Nhiệm vụ:

- Sử dụng `HttpClient` để giao tiếp với Campaign.Api.
- Gửi HTTP GET để lấy danh sách chiến dịch.
- Gửi HTTP POST để tạo chiến dịch.
- Nhận và xử lý dữ liệu JSON.
- Hiển thị kết quả cho người sử dụng.
- Xử lý lỗi khi API không hoạt động hoặc trả về lỗi HTTP.

Client không tham chiếu trực tiếp đến project API mà giao tiếp thông qua HTTP.

## 5. RESTful API

### 5.1. Danh sách các API Endpoint

API mặc định chạy tại:

`http://localhost:5000`

| HTTP Method | Endpoint | Chức năng |
|---|---|---|
| GET | `/` | Kiểm tra trạng thái API |
| GET | `/health` | Kiểm tra tình trạng dịch vụ |
| GET | `/api/campaigns` | Lấy danh sách chiến dịch |
| POST | `/api/campaigns` | Tạo chiến dịch mới |

### 5.2. Tạo chiến dịch mới

**Request:**

```http
POST http://localhost:5000/api/campaigns
Content-Type: application/json
```

**Request Body:**

```json
{
  "name": "Summer Sale",
  "content": "Discover our summer offers."
}
```

API tiếp nhận dữ liệu JSON, kiểm tra thông tin chiến dịch và xử lý yêu cầu tạo mới.

Nếu dữ liệu không hợp lệ, API trả về:

`400 Bad Request`

### 5.3. Lấy danh sách chiến dịch

**Request:**

```http
GET http://localhost:5000/api/campaigns
```

Nếu yêu cầu thành công, API trả về:

`200 OK`

Dữ liệu trả về có định dạng JSON. Nếu chưa có chiến dịch nào, API trả về một mảng rỗng.

### 5.4. Kiểm tra trạng thái API

**Request:**

```http
GET http://localhost:5000/
```

Hoặc:

```http
GET http://localhost:5000/health
```

**Response:**

```json
{
  "service": "Campaign.Api",
  "status": "running",
  "message": "Campaign API is running"
}
```

## 6. HTTP Status Code sử dụng

| Status Code | Ý nghĩa | Trường hợp sử dụng |
|---|---|---|
| 200 OK | Yêu cầu thành công | Lấy danh sách chiến dịch, kiểm tra API |
| 400 Bad Request | Yêu cầu không hợp lệ | Thiếu hoặc sai dữ liệu đầu vào |
| 500 Internal Server Error | Lỗi máy chủ | Lỗi phát sinh trong quá trình xử lý |

## 7. Hướng dẫn chạy chương trình

### Bước 1. Mở thư mục dự án

Mở PowerShell tại thư mục gốc của dự án:

```powershell
cd "Users\HOME PC\Desktop\Hệ Thống Gửi Thông Báo Hàng Loạt"
```

### Bước 2. Khôi phục các dependency

```powershell
dotnet restore
```

### Bước 3. Build Solution

```powershell
dotnet build
```

### Bước 4. Chạy Campaign.Api

```powershell
dotnet run --project src/Campaign.Api
```

Sau khi chạy thành công, API mặc định sử dụng địa chỉ:

`http://localhost:5000`

Nếu terminal hiển thị địa chỉ khác tại dòng `Now listening on`, cần sử dụng địa chỉ thực tế đó khi kiểm thử.

### Bước 5. Chạy Campaign.Client

Mở một cửa sổ PowerShell thứ hai:

```powershell
cd "ĐƯỜNG_DẪN_ĐẾN_BULKNOTIFICATIONSYSTEM"
```

Sau đó chạy:

```powershell
dotnet run --project src/Campaign.Client -- http://localhost:5000
```

Client sẽ tự động tải danh sách chiến dịch khi khởi động và hiển thị menu để người dùng lựa chọn chức năng.

## 8. Kiểm thử bằng Postman

Dự án có thư mục `postman/` chứa Collection và Environment phục vụ kiểm thử.

Các trường hợp kiểm thử:

| STT | Phương thức | Nội dung kiểm thử | Kết quả mong đợi |
|---|---|---|---|
| 1 | GET | Kiểm tra `/` | 200 OK |
| 2 | GET | Kiểm tra `/health` | 200 OK |
| 3 | GET | Lấy danh sách chiến dịch | 200 OK |
| 4 | POST | Tạo chiến dịch với dữ liệu hợp lệ | Thành công |
| 5 | POST | Tạo chiến dịch thiếu `name` | 400 Bad Request |
| 6 | POST | Tạo chiến dịch thiếu `content` | 400 Bad Request |

Để kiểm thử chức năng tạo chiến dịch:

1. Mở Postman.
2. Chọn phương thức `POST`.
3. Nhập URL `http://localhost:5000/api/campaigns`.
4. Chọn **Body → raw → JSON**.
5. Nhập dữ liệu JSON hợp lệ.
6. Nhấn **Send**.
7. Kiểm tra HTTP Status Code và nội dung Response.

## 9. Tài liệu thực hành

Các tài liệu đi kèm dự án:

| Tài liệu | Nội dung |
|---|---|
| `system-architecture.md` | Kiến trúc tổng quan hệ thống |
| `client-server-communication.md` | Cơ chế giao tiếp Client–Server |
| `message-exchange-patterns.md` | Các mô hình trao đổi thông điệp |
| `postman-testing.md` | Hướng dẫn kiểm thử bằng Postman |
| `week-1-report.md` | Báo cáo tổng kết Tuần 1 |
| `week-1-checklist.md` | Danh sách công việc Tuần 1 |

## 10. Phạm vi và giới hạn hiện tại

Các chức năng đã được mô tả và triển khai trong phạm vi hiện tại:

- Xây dựng ASP.NET Core Web API.
- Xây dựng ứng dụng Console Client.
- Giao tiếp Client–Server qua HTTP.
- Sử dụng RESTful API với các phương thức GET và POST.
- Trao đổi dữ liệu JSON.
- Kiểm tra dữ liệu đầu vào.
- Xử lý lỗi kết nối và lỗi HTTP ở phía Client.
- Kiểm thử API bằng Postman.

**Các giới hạn hiện tại:**

- Dữ liệu chiến dịch được lưu bằng In-Memory Storage.
- Dữ liệu sẽ mất khi API dừng hoặc khởi động lại.
- Chưa tích hợp cơ sở dữ liệu.
- Chưa tích hợp RabbitMQ hoặc hệ thống Message Queue.
- Chưa triển khai Worker xử lý gửi thông báo nền.
- Chưa tích hợp nhà cung cấp SMS, Email hoặc dịch vụ gửi tin nhắn thực tế.
- Chưa bật Swagger/OpenAPI.

## 11. Hướng phát triển tiếp theo

Trong các giai đoạn tiếp theo, dự án có thể được mở rộng theo các hướng:

1. Hoàn thiện các RESTful API quản lý chiến dịch và thông báo.
2. Bổ sung API lấy thông tin chi tiết và quản lý thông báo thuộc chiến dịch.
3. Tích hợp cơ sở dữ liệu để lưu trữ dữ liệu lâu dài.
4. Tích hợp RabbitMQ để xử lý hàng đợi thông báo.
5. Xây dựng Worker xử lý gửi thông báo nền.
6. Tích hợp dịch vụ gửi Email hoặc SMS.
7. Bổ sung xác thực và phân quyền người dùng.
8. Bổ sung Swagger/OpenAPI để mô tả và kiểm thử API.

## 12. Kết luận

Dự án BulkNotificationSystem là bước thực hành xây dựng hệ thống theo mô hình Client–Server, sử dụng ASP.NET Core Web API để cung cấp dịch vụ RESTful API và sử dụng HttpClient để giao tiếp giữa máy khách và máy chủ.

Thông qua dự án, người thực hiện có thể hiểu cách gửi HTTP Request, nhận HTTP Response, trao đổi dữ liệu JSON, xử lý lỗi và kiểm thử API bằng Postman.

Đây là nền tảng ban đầu để tiếp tục phát triển hệ thống gửi thông báo hàng loạt hoàn chỉnh trong các giai đoạn tiếp theo.
