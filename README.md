# BulkNotificationSystem

Solution ban đầu cho hệ thống gửi thông báo hàng loạt. Thư mục được kiểm tra lúc khởi tạo không có Solution, project hay mã nguồn nào trước đó.

## Cấu trúc

```text
BulkNotificationSystem.slnx
docs/
  client-server-communication.md
  message-exchange-patterns.md
  postman-testing.md
  system-architecture.md
  week-1-checklist.md
  week-1-report.md
postman/
  BulkNotificationSystem.postman_collection.json
  BulkNotificationSystem.postman_environment.json
src/
  Campaign.Api/
    Campaign.Api.csproj
    Program.cs
    DTOs/
    Endpoints/
    Models/
    Services/
  Campaign.Client/
    Campaign.Client.csproj
    Program.cs
    CampaignApiClient.cs
    CampaignApiException.cs
    Models/
```

- `Campaign.Api`: ASP.NET Core Web API, cung cấp `GET /` và `GET /health` để kiểm tra trạng thái; `POST /api/campaigns` để tạo chiến dịch; `GET /api/campaigns` để liệt kê chiến dịch. Dữ liệu được lưu trong bộ nhớ và mất khi API dừng. Mặc định chạy tại `http://localhost:5000`; không bật Swagger/OpenAPI.
- `Campaign.Client`: ứng dụng Console độc lập, dùng `HttpClient` gọi `GET /api/campaigns` và `POST /api/campaigns` qua HTTP. Client không tham chiếu project API; có thể truyền địa chỉ API làm tham số đầu tiên, mặc định là `http://localhost:5000`.
- Cả hai project hiện target `net10.0`.
- Cả hai project chưa khai báo package NuGet bên ngoài.
- Tài liệu kiến trúc tổng quan: [docs/system-architecture.md](docs/system-architecture.md).
- Hướng dẫn giao tiếp độc lập Client–Server: [docs/client-server-communication.md](docs/client-server-communication.md).
- Tài liệu Message Exchange Patterns: [docs/message-exchange-patterns.md](docs/message-exchange-patterns.md).
- Hướng dẫn Postman và báo cáo kiểm thử: [docs/postman-testing.md](docs/postman-testing.md).
- Báo cáo tổng kết và checklist Tuần 1: [docs/week-1-report.md](docs/week-1-report.md), [docs/week-1-checklist.md](docs/week-1-checklist.md).

## Chạy thử

Từ thư mục gốc:

```powershell
dotnet restore
dotnet build 
dotnet run --project src\Campaign.Api
```

Mở terminal thứ hai để chạy Client:

```powershell
dotnet run --project src\Campaign.Client -- http://localhost:5000
```

Client tải danh sách chiến dịch khi khởi động, sau đó hiển thị menu để tải lại danh sách hoặc tạo chiến dịch. Nếu API chưa chạy, Client hiển thị thông báo lỗi kết nối.

Kiểm tra bằng Postman:

- Tạo chiến dịch: `POST http://localhost:5000/api/campaigns`, chọn **Body → raw → JSON**:

  ```json
  {
    "name": "Summer Sale",
    "content": "Discover our summer offers."
  }
  ```

- Thử dữ liệu không hợp lệ bằng cách bỏ `name` hoặc `content`; API trả HTTP `400 Bad Request` cùng thông tin lỗi.
- Lấy danh sách: `GET http://localhost:5000/api/campaigns`. Trả `200 OK` cùng mảng JSON; mảng rỗng nếu chưa tạo chiến dịch.
- Kiểm tra API: `GET http://localhost:5000/` hoặc `GET http://localhost:5000/health`. API trả HTTP `200 OK` và JSON:

  ```json
  {
    "service": "Campaign.Api",
    "status": "running",
    "message": "Campaign API is running"
  }
  ```

Các request mẫu cũng có trong `src\Campaign.Api\Campaign.Api.http`.

## Phạm vi hiện tại

Quản lý chiến dịch hiện dùng In-Memory storage, chưa có Database, RabbitMQ, Worker hoặc chức năng gửi thông báo thật.
