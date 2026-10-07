# RealtimeChat

Nền tảng giao tiếp thời gian thực hỗ trợ nhắn tin và gọi thoại/video, được xây dựng với ASP.NET Core, SignalR và WebRTC, hướng đến khả năng mở rộng theo chiều ngang.

## Tổng quan

RealtimeChat là nền tảng giao tiếp thời gian thực lấy cảm hứng từ các ứng dụng như Zalo, Discord và Slack.

Project tập trung vào nhắn tin thời gian thực, hội thoại 1-1 và nhóm, quản lý quan hệ giữa người dùng, trạng thái online/offline, chia sẻ đa phương tiện và cuộc gọi thoại/video 1-1.

Project được xây dựng với **Clean Architecture**, kết hợp mô hình **Domain-centric Entity**, UseCase-oriented Application Layer và cơ chế realtime communication thông qua SignalR.

## Phạm vi project

RealtimeChat là nền tảng giao tiếp trực tuyến, tập trung vào nhắn tin thời gian thực, quản lý quan hệ người dùng, chia sẻ đa phương tiện và Audio/Video Call.

1. Người dùng & Xác thực

- Authentication: Đăng ký, đăng nhập và xác thực bằng JWT.
- User Profile: Quản lý thông tin cá nhân và avatar.
- Multi-Connection: Hỗ trợ nhiều kết nối trên cùng một tài khoản.
- Presence: Theo dõi trạng thái Online / Offline theo thời gian thực.

2. Nhắn tin

- Private Chat: Nhắn tin 1-1 theo thời gian thực.
- Group Chat: Nhắn tin trong các cuộc trò chuyện nhóm.
- Message History: Lưu trữ và truy xuất lịch sử tin nhắn.
- Message Pagination: Phân trang lịch sử tin nhắn.
- Read Status: Theo dõi trạng thái đã đọc, chưa đọc
- Reply: Trả lời tin nhắn.
- Reaction: Bày tỏ cảm xúc với tin nhắn.
- Mention: Mention người dùng trong cuộc trò chuyện.
- Recall: Thu hồi tin nhắn.
- Message Types: Hỗ trợ Text, Image, Video, Audio, File và Sticker.
- Call History: Lưu lịch sử cuộc gọi dưới dạng tin nhắn trong conversation.

3. Bạn bè & Quan hệ

- Friend Request: Gửi, chấp nhận và từ chối lời mời kết bạn.
- Friend List: Quản lý danh sách bạn bè.
- Block: Chặn và bỏ chặn người dùng.
- Friend Recommendation: Gợi ý bạn bè dựa trên các conversation chung và mối quan hệ gián tiếp.

4. Quản lý nhóm

- Group Management: Tạo và quản lý thông tin nhóm.
- Member Management: Thêm, xóa thành viên và quản lý thành viên.
- Role Management: Chuyển quyền quản trị nhóm.
- Group Disband: Giải tán nhóm.

5. Sticker System

- Sticker Collection: Tạo và quản lý bộ sưu tập Sticker.
- Visibility: Hỗ trợ bộ sưu tập Public / Private.
- Sharing: Chia sẻ bộ sưu tập thông qua mã chia sẻ.
- Collection Management: Thêm / lưu bộ sưu tập và quản lý Sticker.
- Sticker Message: Gửi Sticker trực tiếp trong cuộc trò chuyện.

6. Media & File Sharing

- Media Upload: Tải lên hình ảnh, video, audio và file.
- Media Metadata: Quản lý metadata của các Media.
- File Sharing: Chia sẻ Media trực tiếp trong cuộc trò chuyện.
- Static Media Delivery: Phân phối Media thông qua static file storage.

7. Realtime Communication

- Realtime Messaging: Đồng bộ tin nhắn theo thời gian thực.
- Presence: Cập nhật Online / Offline.
- Typing Indicator: Hiển thị trạng thái đang soạn tin.
- Realtime Events: Đồng bộ các sự kiện trong conversation theo thời gian thực.

8. Audio / Video Call

- Audio Call: Gọi thoại 1-1.
- Video Call: Gọi video 1-1.
- WebRTC: Truyền tải Audio / Video giữa các client.
- Signaling: Trao đổi signaling thông qua hệ thống realtime.
- Call Lifecycle: Quản lý trạng thái và vòng đời cuộc gọi.
- Call History: Lưu lại thông tin cuộc gọi sau khi kết thúc.

9. Scalability & Infrastructure

- Redis: Caching và hỗ trợ đồng bộ realtime giữa các instance.
- Horizontal Scaling: Hỗ trợ chạy nhiều Backend instance.
- Nginx: Reverse Proxy và Load Balancing.
- Docker: Container hóa các thành phần của hệ thống.
- Docker Compose: Quản lý và triển khai toàn bộ hệ thống.

## Công nghệ sử dụng

### Backend

- ASP.NET Core 8
- Entity Framework Core
- SignalR
- JWT Authentication

### Frontend

- React
- TypeScript
- Vite
- Tailwind CSS

### Database & Infrastructure

- MariaDB
- Redis
- Docker
- Docker Compose
- Nginx

### Real-time Communication

- SignalR
- WebRTC

## Kiến trúc

Project được xây dựng theo **Clean Architecture**, với các layer chính:

- **Api**: HTTP API, SignalR Hub và các thành phần giao tiếp với client.
- **Application**: UseCase và logic điều phối workflow.
- **Domain**: Entity, business rule và state transition cốt lõi.
- **Infrastructure**: Persistence và các infrastructure service.

Application phụ thuộc vào abstraction thay vì implementation cụ thể.
Infrastructure cung cấp implementation cho các abstraction này.

## Một số quyết định thiết kế

### Domain-centric Entity

Entity không chỉ là nơi chứa dữ liệu mà còn chịu trách nhiệm bảo vệ các
invariant và thay đổi trạng thái của chính nó.

Ví dụ:

```csharp
call.Accept();
call.Reject();
call.Miss();
call.End();
```

Thay vì cho phép Application Layer thay đổi trực tiếp các property trạng
thái.

Điều này giúp các state transition hợp lệ được kiểm soát ngay tại
Domain.

### UseCase-oriented Application Layer

Mỗi UseCase đại diện cho một workflow nghiệp vụ cụ thể.

UseCase chịu trách nhiệm điều phối:

- Authorization
- Domain operations
- Persistence
- Các service bên ngoài
- Realtime notification

Entity tập trung vào business rule của chính nó, trong khi UseCase chịu
trách nhiệm kết nối các thành phần lại với nhau.

### HTTP và SignalR

Project phân biệt giữa **persistent operations** và **realtime
communication**.

HTTP được sử dụng cho các command/query cần xử lý nghiệp vụ và
persistence.

SignalR được sử dụng chủ yếu để:

- Phát realtime event
- Gửi notification
- Presence
- Typing Indicator
- Các tương tác realtime không cần persistence trực tiếp

Ví dụ, khi gửi một message:

```text
Client
  │
  │ HTTP
  ▼
Controller
  │
  ▼
SendMessageUseCase
  │
  ├── Persist Message
  │
  └── Notify
        │
        ▼
      SignalR
        │
        ▼
   Other Clients
```

SignalR không thay thế HTTP trong việc xử lý các nghiệp vụ cần
persistence.

### Explicit State Transition

Các entity quản lý state transition thông qua các method thể hiện rõ ý
nghĩa nghiệp vụ.

Ví dụ:

```csharp
call.Accept();
call.Reject();
call.Miss();
call.End();
```

Điều này giúp tránh việc thay đổi trạng thái entity một cách tùy ý từ
bên ngoài.

## Real-time Communication

SignalR được sử dụng làm lớp realtime communication giữa server và
client.

Project sử dụng abstraction `IClientNotifier` để tách Application Layer
khỏi implementation cụ thể của SignalR.

```text
Application
     │
     │ IClientNotifier
     ▼
SignalRNotifier
     │
     ▼
SignalR Hub
     │
     ▼
Connected Clients
```

Application Layer có thể phát realtime event thông qua abstraction mà
không cần phụ thuộc trực tiếp vào implementation cụ thể của SignalR.

## Presence

Presence được quản lý thông qua một presence service trong backend.

Một user có thể có nhiều SignalR connection cùng lúc.

```text
User
 ├── Connection A
 ├── Connection B
 └── Connection C
```

User chỉ được xem là **Online** khi còn ít nhất một connection đang hoạt
động.

Khi connection cuối cùng bị ngắt, hệ thống có thể phát sự kiện Offline
tới những user liên quan.

## WebRTC

WebRTC được sử dụng cho cuộc gọi audio/video 1-1.

SignalR chỉ đảm nhiệm phần **signaling** để hai client trao đổi thông
tin cần thiết nhằm thiết lập WebRTC connection.

Sau khi kết nối được thiết lập, luồng audio/video được xử lý bởi WebRTC.

```text
Client A
   │
   │ Signaling
   ▼
SignalR
   │
   │ Signaling
   ▼
Client B

Client A ◄──────── WebRTC ────────► Client B
                 Audio / Video
```

## Horizontal Scalability

Backend được thiết kế để có thể chạy nhiều instance phía sau Nginx.

```text
                    Nginx
                      │
          ┌───────────┼───────────┐
          ↓           ↓           ↓
       Backend     Backend     Backend
       Instance    Instance    Instance
          │           │           │
          └───────────┼───────────┘
                      │
             Shared Infrastructure
                 ├── MariaDB
                 └── Redis
```

Nhiều backend instance có thể chạy song song phía sau Nginx.

Việc này cho phép hệ thống mở rộng bằng cách bổ sung thêm backend
instance thay vì phụ thuộc vào một backend duy nhất.

Redis được sử dụng cho caching và các nhu cầu infrastructure dùng chung
giữa các backend instance.

## Infrastructure

Project được container hóa bằng Docker và có thể được triển khai bằng
Docker Compose.

Các thành phần chính:

```text
                    Nginx
                      │
          ┌───────────┴───────────┐
          │                       │
      Frontend                  Backend
                                  │
                       ┌──────────┴──────────┐
                       │                     │
                    MariaDB                Redis
```

Nginx đóng vai trò reverse proxy và load balancer.

## Project Structure

```text
RealtimeChat/
├── .github/
│   └── ...
│
├── backend/
│   ├── RealtimeChat.Api/
│   │   ├── Controllers/     # HTTP endpoints
│   │   ├── Filters/         # API response filter
│   │   ├── Hubs/            # SignalR realtime
│   │   ├── Middleware/      # Exception handling
│   │   ├── Security/        # JWT, current user
│   │   ├── Responses/       # API response models
│   │   ├── Swagger/
│   │   ├── DependencyInjection.cs
│   │   └── Program.cs
│   │
│   ├── RealtimeChat.Application/
│   │   ├── Common/          # Shared models and utilities
│   │   ├── Features/        # Use cases by feature
│   │   ├── Interfaces/      # Application abstractions
│   │   ├── QueryExtensions/ # EF Core queries
│   │   ├── Exceptions/
│   │   └── DependencyInjection.cs
│   │
│   ├── RealtimeChat.Domain/
│   │   ├── Entities/        # Domain entities
│   │   ├── Enums/           # Domain enums
│   │   ├── Exceptions/      # Domain exceptions
│   │   └── RealtimeChat.Domain.csproj
│   │
│   └── RealtimeChat.Infrastructure/
│       ├── Persistence/     # EF Core & database
│       ├── Presence/        # Presence implementation
│       ├── Security/        # JWT & password hashing
│       └── DependencyInjection.cs
│
├── frontend/
│   ├── src/
│   ├── public/
│   └── ...
│
├── .env.example
├── .gitignore
├── docker-compose.yml
└── README.md
```

## Trạng thái

Đang trong quá trình phát triển.
