# RealtimeChat

Nền tảng giao tiếp thời gian thực hỗ trợ nhắn tin, gọi thoại/video và chia sẻ hành trình trực tiếp, được xây dựng với **ASP.NET Core, React, SignalR và WebRTC**.

RealtimeChat lấy cảm hứng từ các ứng dụng như Zalo, Discord và Slack, tập trung vào trải nghiệm giao tiếp cùng cách tổ chức backend theo **Clean Architecture**, **Domain-centric Entity** và **UseCase-oriented Application Layer**.

## Features

### Users & Authentication

- Đăng ký, đăng nhập với JWT Authentication.
- Quản lý hồ sơ người dùng và avatar.
- Hỗ trợ nhiều kết nối trên cùng một tài khoản.
- Theo dõi trạng thái Online / Offline.

### Messaging

- Nhắn tin 1-1 và nhóm theo thời gian thực.
- Lưu trữ, phân trang và truy xuất lịch sử tin nhắn.
- Read Status, Reply, Reaction, Mention và Recall.
- Hỗ trợ Text, Image, Video, Audio, File và Sticker.
- Lưu lịch sử cuộc gọi trong conversation.

### Friends & Groups

- Gửi, chấp nhận và từ chối lời mời kết bạn.
- Quản lý danh sách bạn bè và chặn người dùng.
- Gợi ý bạn bè dựa trên conversation chung và các mối quan hệ gián tiếp.
- Tạo nhóm, quản lý thành viên và vai trò, chuyển quyền quản trị và giải tán nhóm.

### Sticker & Media

- Tạo và quản lý bộ sưu tập Sticker Public / Private.
- Chia sẻ bộ sưu tập qua mã chia sẻ và gửi Sticker trong conversation.
- Tải lên, quản lý metadata và chia sẻ hình ảnh, video, audio và file.
- Phân phối Media thông qua static file storage.

### Realtime Communication

- Đồng bộ tin nhắn và sự kiện conversation qua SignalR.
- Presence và Typing Indicator.
- Hỗ trợ nhiều SignalR connection cho một người dùng.

### Audio / Video Call

- Gọi thoại và video 1-1 với WebRTC.
- SignalR signaling để thiết lập kết nối giữa các client.
- Quản lý vòng đời cuộc gọi và lưu lịch sử sau khi kết thúc.

### Live Trip Sharing

- Bắt đầu và kết thúc phiên chia sẻ hành trình.
- Cập nhật và đồng bộ vị trí trực tiếp qua SignalR.
- Quản lý trạng thái phiên và hiển thị hành trình trên bản đồ.
- Hỗ trợ ETA khi tích hợp dịch vụ định tuyến.

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

Project áp dụng **Clean Architecture** với bốn layer chính:

- **Api:** HTTP endpoints, SignalR Hub, middleware và các thành phần giao tiếp với client.
- **Application:** UseCase, workflow orchestration và các abstraction phục vụ nghiệp vụ.
- **Domain:** Entity, Value Object, enum, exception và business rules.
- **Infrastructure:** Implementation cho persistence và các dịch vụ hạ tầng.

Application phụ thuộc vào abstraction; Infrastructure cung cấp các implementation tương ứng.

```text
Api  --->  Application  --->  Domain
                ^
                |
         Infrastructure
       implements abstractions
```

## Key Design Decisions

### Friend Recommendation

Hệ thống gợi ý bạn bè dựa trên **lịch sử conversation chung**, được xác định thông qua bảng `ConversationMembers`, bao gồm cả direct conversation và group chat. Những người có nhiều conversation chung được ưu tiên, đồng thời loại trừ các tài khoản đã kết bạn, có lời mời kết bạn đang chờ xử lý hoặc bị chặn.

- **Candidate Generation:** Xếp hạng ứng viên theo mức độ liên quan và sử dụng BFS để mở rộng tìm kiếm qua các mối liên hệ gián tiếp dựa trên conversation. Nếu chưa đủ ứng viên, hệ thống chọn ngẫu nhiên những người dùng khác đủ điều kiện để bổ sung.
- **Recommendation Pool:** Tạo pool ứng viên lớn hơn số lượng cần trả về và cache bằng Redis để tái sử dụng qua nhiều lần gợi ý, hạn chế phải chạy lại thuật toán cho mỗi request.
- **Recommendation Selection:** Sử dụng weighted random để cân bằng giữa mức độ liên quan và sự đa dạng. Trước khi trả kết quả, hệ thống kiểm tra lại trạng thái quan hệ và loại bỏ các ứng viên không còn hợp lệ.
- **Candidate Suppression:** Hệ thống cho phép user tạm ẩn một ứng viên trong 2 ngày bằng Redis TTL, để tránh đề xuất lại người dùng đó trong thời gian này.

**Trade-off:** Thuật toán dựa trên lịch sử conversation thay vì trực tiếp mở rộng qua quan hệ bạn bè. Ví dụ, nếu A và B đã kết bạn nhưng chưa từng trò chuyện qua direct chat hay group chat, hệ thống có thể không mở rộng tìm kiếm qua B để đề xuất bạn bè của B cho A. Vì vậy, hệ thống có thể bỏ lỡ một số ứng viên tiềm năng.

## Live Trip Sharing

Live Trip Sharing cho phép người dùng chia sẻ vị trí trực tiếp hoặc hành trình có điểm đến ngay trong conversation. Tính năng hỗ trợ theo dõi tiến trình di chuyển và ETA dự kiến, phù hợp với các tình huống như hẹn gặp hoặc đón nhau mà không cần liên tục nhắn tin hỏi thăm.

- **Live Location Updates:** Backend tiếp nhận vị trí định kỳ (ví dụ mỗi 10 giây) và đồng bộ đến các client liên quan qua SignalR, giúp cập nhật vị trí và theo dõi quá trình di chuyển.
- **ETA Calculation:** Tích hợp routing service để tính thời gian dự kiến đến đích. Backend chỉ tính lại ETA khi người dùng di chuyển đủ xa (tối thiểu 100 m) và đã qua ít nhất 1 phút kể từ lần tính trước, với giới hạn tối đa 3 phút giữa hai lần tính. Cơ chế này giúp hạn chế các lần gọi dịch vụ định tuyến không cần thiết, đồng thời tránh để thông tin ETA quá cũ.

### HTTP & SignalR

Project tách thao tác nghiệp vụ cần persistence khỏi việc phát sự kiện realtime:

- **HTTP:** Xử lý command/query và workflow nghiệp vụ.
- **SignalR:** Phát realtime event, notification, Presence và Typing Indicator.

Ví dụ, khi gửi tin nhắn, backend xử lý và lưu message trước khi thông báo cho các client liên quan.

```text
Client -> HTTP -> Controller -> SendMessageUseCase
                                  |          |
                               Persist     Notify
                                             |
                                          SignalR
                                             |
                                       Other Clients
```

### Shared Data Cache ở Frontend

Frontend sử dụng **TanStack Query** để xây dựng shared cache cho những dữ liệu được dùng chung giữa nhiều feature, chẳng hạn như User Profile trong danh sách bạn bè, conversation, tin nhắn và thành viên nhóm.

Các API nghiệp vụ được thiết kế để trả về dữ liệu tối thiểu cần cho từng chức năng, thay vì lặp lại những thông tin hiển thị như `userName` hoặc `avatarUrl` trong nhiều response. Frontend dựa vào `userId` để tra cứu User Profile từ shared cache, sau đó kết hợp dữ liệu cần thiết để hiển thị theo từng feature.

Nếu profile chưa có trong cache, frontend gọi **batch API** để lấy thông tin của nhiều user trong một request và cập nhật cache. Nhờ vậy, dữ liệu dùng chung được quản lý tập trung ở frontend, hạn chế payload trùng lặp và các request riêng lẻ. Backend cũng không cần gắn chặt response của từng endpoint với nhu cầu hiển thị cụ thể của frontend; mỗi endpoint tập trung trả về dữ liệu cần thiết cho nghiệp vụ, còn cách sử dụng và hiển thị profile do frontend quản lý.

## Project Structure

Cấu trúc source code được chia theo trách nhiệm của từng layer để quản lý logic nghiệp vụ, giao tiếp với client và các implementation hạ tầng riêng biệt.

```text
RealtimeChat/
├── .github/                              # GitHub workflows and project configuration
│
├── backend/
│   ├── RealtimeChat.Api/
│   │   ├── Controllers/                  # HTTP endpoints
│   │   ├── Filters/                      # API response filters
│   │   ├── Hubs/                         # SignalR hubs and realtime connections
│   │   ├── Middleware/                   # Request pipeline and exception handling
│   │   ├── Security/                     # JWT and current-user context
│   │   ├── Responses/                    # API response models
│   │   ├── Swagger/                      # API documentation configuration
│   │   ├── DependencyInjection.cs         # API-layer service registration
│   │   └── Program.cs                    # Application entry point
│   │
│   ├── RealtimeChat.Application/
│   │   ├── Common/                       # Shared application models and utilities
│   │   ├── Features/                     # UseCases organized by feature
│   │   ├── Interfaces/                   # Application abstractions
│   │   ├── QueryExtensions/              # Reusable query extensions
│   │   ├── Exceptions/                   # Application-level exceptions
│   │   └── DependencyInjection.cs         # Application service registration
│   │
│   ├── RealtimeChat.Domain/
│   │   ├── Entities/                     # Domain entities and state transitions
│   │   ├── ValueObject/                  # Value objects and value-based concepts
│   │   ├── Enums/                        # Domain enumerations
│   │   └── Exceptions/                   # Domain-specific exceptions
│   │
│   └── RealtimeChat.Infrastructure/
│       ├── Caching/                      # Caching implementations
│       ├── Persistence/                  # EF Core and database access
│       ├── Presence/                     # Presence tracking implementations
│       ├── Routings/                     # Routing and ETA service implementations
│       ├── Security/                     # JWT and password-hashing implementations
│       ├── Storage/                      # File and media storage implementations
│       └── DependencyInjection.cs        # Infrastructure service registration
│
├── frontend/
│   ├── src/                              # Frontend application source
│   └── public/                           # Static public assets
│
├── .env.example                          # Example environment configuration
├── .gitignore                            # Files excluded from version control
├── docker-compose.yml                    # Multi-service Docker configuration
└── README.md                             # Project documentation
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

## Project Status

Đang trong quá trình phát triển.
