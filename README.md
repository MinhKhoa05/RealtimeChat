# RealtimeChat

Nền tảng giao tiếp thời gian thực hỗ trợ nhắn tin, gọi thoại/video và chia sẻ hành trình trực tiếp, được xây dựng với **ASP.NET Core, React, SignalR và WebRTC**.

RealtimeChat lấy cảm hứng từ các ứng dụng như Zalo, Discord và Slack, tập trung vào trải nghiệm giao tiếp cùng cách tổ chức backend theo **Clean Architecture**, **Domain-centric Entity** và **UseCase-oriented Application Layer**.

## Features

Tính năng của dự án được phát triển và mở rộng theo từng cấp độ:

### 1. Nhắn tin cơ bản & Tài khoản (Core Chat)

- Đăng ký, đăng nhập và quản lý phiên với JWT & Refresh Token.
- Tìm kiếm người dùng, gửi/chấp nhận lời mời kết bạn, quản lý danh sách bạn bè và chặn người dùng.
- Nhắn tin văn bản 1-1 theo thời gian thực qua SignalR.
- Lưu trữ lịch sử trò chuyện, phân trang tin nhắn và cập nhật trạng thái đã gửi / đã xem (Read Status).

### 2. Nhóm trò chuyện & Hoàn thiện trải nghiệm (Group & Rich Messaging)

- Tạo nhóm, thêm/xóa thành viên, tham gia nhóm qua liên kết mời, chuyển quyền trưởng nhóm và giải tán nhóm.
- Mở rộng tương tác tin nhắn: Gửi hình ảnh/file (Media), trả lời tin nhắn (Reply), nhắc tên (`@Mention`), thu hồi tin nhắn (Recall) và ghim hội thoại.
- Theo dõi trạng thái Online / Offline (hỗ trợ đa kết nối trên cùng một tài khoản) và hiển thị trạng thái đang nhập (Typing Indicator).

### 3. Gọi thoại / Video & Mở rộng quy mô (Call & Horizontal Scaling)

- Gọi thoại và video 1-1 thời gian thực với WebRTC và SignalR signaling, quản lý vòng đời cuộc gọi và lưu lịch sử cuộc gọi vào hội thoại.
- Tối ưu hiệu năng và định hướng mở rộng ngang (Horizontal Scaling) với Redis (quản lý Presence, Caching và SignalR Backplane khi chạy nhiều instance).

### 4. Tính năng mở rộng (Extended & Planned Features)

- Gợi ý kết bạn dựa trên bạn chung và nhóm chung.
- Bộ sưu tập Sticker cá nhân và chia sẻ qua mã mời.
- Chia sẻ vị trí và hành trình trực tiếp (Live Trip Sharing & ETA).
- Bảng tin và tương tác bài viết cơ bản.

## Tech Stack

| Thành phần     | Công nghệ                             |
| -------------- | ------------------------------------- |
| Backend        | ASP.NET Core 8, Entity Framework Core |
| Frontend       | React, TypeScript, Vite, Tailwind CSS |
| Database       | MariaDB                               |
| Authentication | JWT                                   |
| Realtime       | SignalR                               |
| Audio / Video  | WebRTC                                |
| Infrastructure | Redis, Docker, Docker Compose, Nginx  |

## Architecture

### System Architecture

Hệ thống sử dụng **Nginx** làm Reverse Proxy đứng trước **Frontend** và **Backend API**, kết nối với **MariaDB**, **Redis** và **File Storage**:

```text
                        +-------------------+
                        |   Client (Web)    |
                        +---------+---------+
                                  |
                     HTTP / REST  |  WebSocket (SignalR)
                                  v
                        +-------------------+
                        |       Nginx       |
                        |  (Reverse Proxy)  |
                        +----+---------+----+
                             |         |
            Static UI (/...) |         | (/api, /hubs/chat, /media)
                             v         v
               +----------------+   +-----------------------+
               | React Frontend |   |  ASP.NET Core Backend |
               +----------------+   +---+-------+-------+---+
                                        |       |       |
                         +--------------+       |       +--------------+
                         |                      |                      |
                         v                      v                      v
                 +---------------+      +---------------+      +---------------+
                 |    MariaDB    |      |     Redis     |      | Media Storage |
                 |  (Database)   |      | (Cache/Scale) |      | (Local Files) |
                 +---------------+      +---------------+      +---------------+
```

### Backend Layer Architecture

Backend áp dụng **Clean Architecture** kết hợp **Rich Domain Model** với bốn layer chính:

- **Api:** HTTP endpoints, SignalR Hub, middleware và các thành phần giao tiếp với client.
- **Application:** UseCase, workflow orchestration và các abstraction phục vụ nghiệp vụ.
- **Domain:** Entity, Value Object, enum, exception và business rules cốt lõi.
- **Infrastructure:** Implementation cho persistence (EF Core), caching, storage và bảo mật.

```text
Api  --->  Application  --->  Domain
                ^
                |
         Infrastructure
       implements abstractions
```

### Luồng hoạt động (HTTP & SignalR)

Các thao tác gửi tin nhắn và nghiệp vụ cần đảm bảo tính hợp lệ, lưu trữ bền vững trong Database đều đi qua **HTTP REST API** (xác thực, kiểm tra quyền, thực thi business rules và lưu dữ liệu).

Sau khi dữ liệu được lưu thành công, Backend mới dùng **SignalR Hub** để broadcast sự kiện đến các thành viên liên quan, bên cạnh việc xử lý các tín hiệu tức thời không lưu Database như Online/Offline, Typing Indicator và WebRTC Signaling. Cách tiếp cận này giúp thống nhất đầu vào xử lý nghiệp vụ ở Web API, dễ dàng kiểm thử và cho phép các client (Web, Mobile) gọi chung API mà không phụ thuộc vào kết nối SignalR khi gửi tin nhắn.

Ví dụ luồng gửi tin nhắn:

```text
Client A  --(1) HTTP POST /messages/text-->  Controller
                                                 |
                                                 v
                                        SendTextMessageUseCase
                                           |            |
                             (2) Lưu DB    |            | (3) Phát sự kiện
                                           v            v
                                        MariaDB     SignalR Hub
                                                        |
                                                        v
                                           (4) Push realtime event
                                                        |
                                                        v
                                               Client B, Client C
```

## Project Structure

Cấu trúc source code được chia theo trách nhiệm của từng layer để quản lý logic nghiệp vụ, giao tiếp với client và các implementation hạ tầng riêng biệt.

```text
RealtimeChat/
├── .github/                              # Cấu hình GitHub Actions CI/CD
│
├── backend/
│   ├── RealtimeChat.Api/
│   │   ├── Controllers/                  # Các HTTP API endpoints
│   │   ├── Filters/                      # Bộ lọc chuẩn hóa response
│   │   ├── Hubs/                         # SignalR Hub và kết nối realtime
│   │   ├── Middleware/                   # Xử lý request pipeline và bắt lỗi
│   │   ├── Security/                     # Cấu hình JWT và thông tin user hiện tại
│   │   ├── Responses/                    # Khuôn mẫu API response
│   │   ├── Swagger/                      # Cấu hình tài liệu Swagger
│   │   ├── DependencyInjection.cs        # Đăng ký DI cho tầng API
│   │   └── Program.cs                    # Điểm khởi chạy ứng dụng
│   │
│   ├── RealtimeChat.Application/
│   │   ├── Common/                       # Các model và tiện ích dùng chung (phân trang)
│   │   ├── Features/                     # Các UseCase chia theo tính năng
│   │   ├── Interfaces/                   # Các interface trừu tượng
│   │   ├── QueryExtensions/              # Các hàm mở rộng truy vấn tái sử dụng
│   │   ├── Exceptions/                   # Các exception ở tầng Application
│   │   └── DependencyInjection.cs        # Đăng ký DI cho tầng Application
│   │
│   ├── RealtimeChat.Domain/
│   │   ├── Entities/                     # Các thực thể và logic trạng thái
│   │   ├── Factories/                    # Khởi tạo tin nhắn hệ thống
│   │   ├── ValueObjects/                 # Các đối tượng giá trị (Value Objects)
│   │   ├── Enums/                        # Các kiểu liệt kê trong Domain
│   │   └── Exceptions/                   # Các exception vi phạm quy tắc nghiệp vụ
│   │
│   └── RealtimeChat.Infrastructure/
│       ├── Caching/                      # Triển khai bộ nhớ đệm (Cache)
│       ├── Persistence/                  # Cấu hình EF Core và truy cập Database
│       ├── Presence/                     # Theo dõi trạng thái Online/Offline
│       ├── Routings/                     # Dịch vụ định tuyến và tính ETA
│       ├── Security/                     # Tạo JWT và băm mật khẩu
│       ├── Storage/                      # Lưu trữ file và media
│       └── DependencyInjection.cs        # Đăng ký DI cho tầng Infrastructure
│
├── tests/
│   ├── RealtimeChat.UnitTests/           # Unit test cho tầng Domain
│   └── RealtimeChat.IntegrationTests/    # Integration test cho hệ thống
│
├── frontend/
│   ├── src/                              # Mã nguồn giao diện React
│   └── public/                           # Tài nguyên tĩnh
│
├── .env.example                          # File mẫu biến môi trường
├── .gitignore                            # Danh sách file bỏ qua của Git
├── docker-compose.yml                    # Cấu hình chạy đa dịch vụ với Docker
└── README.md                             # Tài liệu tổng quan dự án
```

## Getting Started

### Yêu cầu

- Đã cài đặt [Docker Desktop](https://www.docker.com/products/docker-desktop/) và Docker Compose.

### Khởi chạy

**Bước 1: Cấu hình biến môi trường**

Sao chép file `.env.example` thành `.env`:

```bash
cp .env.example .env
```

Sau đó cập nhật các biến môi trường cần thiết trong file `.env`.

**Bước 2: Khởi chạy toàn bộ ứng dụng**

Tại thư mục gốc của dự án, chạy lệnh:

```bash
docker compose up -d --build
```

Docker Compose sẽ build và khởi chạy toàn bộ dịch vụ được cấu hình, bao gồm frontend, backend API, Nginx và các dịch vụ phụ trợ.

### Truy cập ứng dụng

- **Frontend:** `http://localhost`
- **API Swagger:** `http://localhost/swagger`

Nginx đóng vai trò reverse proxy, định tuyến các request đến frontend hoặc backend API tương ứng.

## Added Documentation

| Tài liệu | Nội dung |
| -------- | -------- |
| [Thiết kế cơ sở dữ liệu](./docs/database_design.md) | Sơ đồ ERD và mô tả cấu trúc cơ sở dữ liệu. |

## Project Status

Đang trong quá trình phát triển.
