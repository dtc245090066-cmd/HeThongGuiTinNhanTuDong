# Hướng dẫn RESTful API – BulkNotificationSystem

## Mục tiêu thực hành

Tài liệu hướng dẫn cách dùng HTTP để kết nối ứng dụng console `Campaign.Client` với ASP.NET Core Web API `Campaign.Api`. Các nội dung thực hành gồm REST, GET/POST, URI tài nguyên, DTO, JSON, `HttpClient`, dependency injection, validation và HTTP status code.

Phạm vi hiện tại chỉ quản lý chiến dịch và nội dung tin nhắn. API không gửi SMS/email và không tuyên bố tin nhắn đã được gửi.

## Kiến trúc Client – Server

- **Campaign.Api (Server):** nhận HTTP request, gọi `ICampaignService`, kiểm tra dữ liệu, trả JSON cùng status code. Endpoint giữ vai trò định tuyến và chuyển kết quả dịch vụ thành HTTP response; lưu trữ dữ liệu tạm thời bằng `InMemoryCampaignService`.
- **Campaign.Client (Client):** ứng dụng console độc lập, gọi API bằng `HttpClient`, chuyển object thành JSON khi POST, đọc JSON response, báo lỗi HTTP/kết nối và hiển thị kết quả cho người dùng.
- Hai project không gọi trực tiếp lẫn nhau bằng tham chiếu mã nguồn; giao tiếp qua HTTP tại `http://localhost:5000`.

DI đăng ký `ICampaignService` với `InMemoryCampaignService` singleton trong suốt vòng đời tiến trình API. Chiến dịch và tin nhắn chỉ nằm trong RAM và bị mất khi API khởi động lại.

## Danh sách API

| Method | URI | Thành công | Lỗi chính |
|---|---|---|---|
| GET | `/api/campaigns` | `200 OK`, danh sách chiến dịch | `500` |
| GET | `/api/campaigns/{id}` | `200 OK`, chi tiết chiến dịch | `404`, `500` |
| POST | `/api/campaigns` | `201 Created`, có header `Location` tới chiến dịch | `400`, `500` |
| GET | `/api/campaigns/{id}/messages` | `200 OK`, danh sách tin nhắn của chiến dịch | `404` nếu chiến dịch không tồn tại, `500` |
| POST | `/api/campaigns/{id}/messages` | `201 Created`, có header `Location` tới tin nhắn | `400`, `404` nếu chiến dịch không tồn tại, `500` |
| GET | `/api/messages/{id}` | `200 OK`, chi tiết tin nhắn | `404`, `500` |

`{id}` là GUID. Response tạo mới trả tài nguyên vừa tạo và `Location` trỏ tới GET resource tương ứng.

### Ví dụ JSON

Tạo chiến dịch (`POST /api/campaigns`):

```json
{
  "name": "Summer Sale",
  "content": "Discover our summer offers."
}
```

Tạo tin nhắn trong chiến dịch (`POST /api/campaigns/{id}/messages`):

```json
{
  "content": "Your order is ready."
}
```

Tên chiến dịch và nội dung tin nhắn không được rỗng hoặc chỉ chứa khoảng trắng. `content` của chiến dịch cũng là trường bắt buộc.

## GET, POST, DTO và HttpClient

- **GET** đọc tài nguyên, không tạo hoặc thay đổi tài nguyên.
- **POST** gửi JSON để tạo tài nguyên mới. Server trả `201 Created` và URI đọc lại tài nguyên qua `Location`.
- **DTO** (`CreateCampaignRequest`, `CreateMessageRequest`, `CampaignResponse`, `MessageResponse`) định nghĩa hình dạng dữ liệu trao đổi, tách request/response HTTP khỏi model lưu trữ trong service.
- **HttpClient** gửi các request HTTP từ client. `CampaignApiClient` gom các lời gọi API, serialize body JSON, deserialize response, kiểm tra status code và giữ nội dung lỗi trong `CampaignApiException`.

## HTTP status code

- **200 OK:** GET thành công.
- **201 Created:** tạo tài nguyên thành công; response bao gồm tài nguyên và `Location`.
- **400 Bad Request:** body thiếu trường bắt buộc hoặc chứa giá trị không hợp lệ; response chứa thông tin validation.
- **404 Not Found:** không tìm thấy chiến dịch/tin nhắn hoặc chiến dịch cha của request tạo tin nhắn.
- **500 Internal Server Error:** lỗi không dự kiến phía server; exception middleware trả Problem Details. Client hiển thị lỗi server thay vì xem request là thành công.

## Chạy dự án

Yêu cầu .NET SDK 10. Từ thư mục gốc của repository, mở hai terminal:

1. Build solution:

   ```powershell
   dotnet build BulkNotificationSystem.sln
   ```

2. Chạy API ở terminal thứ nhất:

   ```powershell
   dotnet run --project src\Campaign.Api\Campaign.Api.csproj
   ```

   API mặc định lắng nghe `http://localhost:5000`. Giữ terminal này mở.

3. Chạy client ở terminal thứ hai:

   ```powershell
   dotnet run --project src\Campaign.Client\Campaign.Client.csproj -- http://localhost:5000
   ```

   Client liệt kê chiến dịch lúc khởi động và hiển thị menu: liệt kê/tạo chiến dịch, xem chi tiết chiến dịch, liệt kê/tạo tin nhắn, xem chi tiết tin nhắn. Tạo tin nhắn chỉ lưu nội dung trong API, không gửi SMS.

## Kiểm thử API

Có thể mở [Campaign.Api.http](../src/Campaign.Api/Campaign.Api.http) trong VS Code và chạy từng request. Sau khi tạo chiến dịch/tin nhắn, thay `campaignId`/`messageId` bằng ID từ response để chạy các request đọc tương ứng. `missingId` là GUID hợp lệ nhưng không được tạo, dùng để kiểm tra 404. Hoặc gửi các request dưới đây bằng Postman, đặt `Content-Type: application/json` cho POST.

| # | Request | Body | Kết quả mong đợi |
|---|---|---|---|
| 1 | `GET /api/campaigns` | – | 200 |
| 2 | `GET /api/campaigns/{campaignId}` với ID tồn tại | – | 200 |
| 3 | `GET /api/campaigns/{missingId}` | – | 404 |
| 4 | `POST /api/campaigns` | `{"name":"Summer Sale","content":"Discover our summer offers."}` | 201 và `Location` |
| 5 | `POST /api/campaigns` | `{"name":" ","content":"Some content"}` | 400 |
| 6 | `POST /api/campaigns/{campaignId}/messages` | `{"content":"Your order is ready."}` | 201 và `Location` |
| 7 | `POST /api/campaigns/{campaignId}/messages` | `{"content":" "}` | 400 |
| 8 | `POST /api/campaigns/{missingId}/messages` | `{"content":"This campaign does not exist."}` | 404 |
| 9 | `GET /api/messages/{messageId}` với ID tồn tại | – | 200 |
| 10 | `GET /api/messages/{missingId}` | – | 404 |

### Kết quả kiểm thử thực tế

Đã thực hiện trong môi trường phát triển:

- `dotnet build BulkNotificationSystem.sln --no-restore`: **thành công**, 0 cảnh báo, 0 lỗi; gồm Campaign.Api, Campaign.Client và các project Notification.
- Chạy Campaign.Api tại `http://localhost:5000` và gọi trực tiếp đủ 11 kiểm tra HTTP: GET danh sách; tạo chiến dịch hợp lệ; đọc chiến dịch; GET chiến dịch không tồn tại; tạo chiến dịch tên trắng; tạo tin nhắn hợp lệ; lấy danh sách tin nhắn; tạo tin nhắn nội dung trắng; tạo tin nhắn cho chiến dịch không tồn tại; đọc tin nhắn; GET tin nhắn không tồn tại. Tất cả status code lần lượt phù hợp (200/201/400/404). Đã kiểm tra thêm `Location` của cả hai response 201.
- Chạy Campaign.Client với API đang hoạt động: thành công khi liệt kê, đọc chi tiết chiến dịch, lấy danh sách tin nhắn, tạo tin nhắn và đọc chi tiết tin nhắn. Đã xác nhận client hiển thị lỗi rõ ràng cho HTTP 400 và 404.
- Không chủ động kích hoạt lỗi server; phản hồi 500 chưa được kiểm thử runtime.

## Hạn chế và hướng phát triển

- Lưu trữ hiện tại chỉ trong RAM; restart API sẽ xóa toàn bộ chiến dịch và tin nhắn.
- Tin nhắn chỉ là dữ liệu được quản lý; không có SMS/email provider, Worker hay xác nhận đã gửi.
- Chưa có cơ sở dữ liệu, xác thực/phân quyền, phân trang hoặc giới hạn tốc độ.
- Có thể phát triển tiếp bằng persistent storage, logging/monitoring, kiểm thử tự động và tích hợp provider/queue trong phạm vi riêng.
