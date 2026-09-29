# StayEasy — Hệ thống đặt phòng và quản lý homestay

## Tổng quan

StayEasy là ứng dụng web cho phép khách hàng tìm kiếm, đặt phòng homestay trực tuyến và giúp chủ homestay/quản trị viên quản lý phòng, đặt phòng, trạng thái phòng và doanh thu.

## Công nghệ sử dụng

| Tầng | Công nghệ |
|------|-----------|
| Frontend | React + TypeScript + Vite |
| Backend | ASP.NET Core Web API |
| ORM | Entity Framework Core |
| Database | MySQL 8.0 |
| Authentication | JWT (JSON Web Token) |
| Deployment | Docker + Docker Compose |

## Cấu trúc thư mục

```
StayEasy/
├── client/              # Frontend (React + TypeScript + Vite)
│   ├── src/
│   │   ├── components/  # React components
│   │   ├── pages/       # Các trang chính
│   │   ├── services/    # API services
│   │   ├── types/       # TypeScript types
│   │   └── utils/       # Utility functions
│   ├── Dockerfile
│   └── package.json
├── server/              # Backend (ASP.NET Core Web API)
│   ├── Controllers/     # API Controllers
│   ├── Services/        # Business logic
│   ├── Models/          # Data models
│   ├── Data/            # DbContext, Migrations
│   ├── Dockerfile
│   └── StayEasy.csproj
├── docker-compose.yml   # Cấu hình Docker
├── .env.example         # Mẫu biến môi trường
└── README.md            # File này
```

## Yêu cầu hệ thống

| Phần mềm | Phiên bản | Link cài đặt |
|----------|-----------|---------------|
| Node.js | 20+ LTS | https://nodejs.org |
| .NET SDK | 8.0+ | https://dotnet.microsoft.com/download |
| Docker Desktop | Mới nhất | https://www.docker.com/products/docker-desktop |
| MySQL | 8.0+ | https://www.mysql.com/downloads/ |

## Hướng dẫn cài đặt

### Bước 1: Clone repository

```bash
git clone https://github.com/Duongcute2604/stayhome.git
cd StayEasy
```

### Bước 2: Cài đặt biến môi trường

```bash
# Copy file mẫu thành file .env thật
cp .env.example .env

# Chỉnh sửa file .env với thông tin thật
```

### Bước 3: Chạy bằng Docker Compose (Khuyến nghị)

```bash
# Khởi động toàn bộ hệ thống
docker compose up -d

# Kiểm tra trạng thái
docker compose ps

# Xem logs
docker compose logs -f
```

### Bước 4: Chạy thủ công (Development)

#### Frontend

```bash
cd client
npm install
npm run dev
# Chạy tại: http://localhost:5173
```

#### Backend

```bash
cd server
dotnet restore
dotnet ef database update   # Tạo database
dotnet run
# Chạy tại: http://localhost:5000
```

## API Endpoints

| Method | Endpoint | Mô tả |
|--------|----------|-------|
| POST | /api/auth/register | Đăng ký tài khoản |
| POST | /api/auth/login | Đăng nhập |
| GET | /api/rooms | Lấy danh sách phòng |
| GET | /api/rooms/{id} | Chi tiết phòng |
| POST | /api/bookings | Tạo đặt phòng |
| GET | /api/bookings | Lấy danh sách đặt phòng |
| PUT | /api/bookings/{id} | Cập nhật đặt phòng |
| DELETE | /api/bookings/{id} | Hủy đặt phòng |

## Quy tắc nghiệp vụ

- Đặt phòng phải trước ít nhất **2 giờ**
- Đặt theo giờ: tối thiểu **3 giờ**
- Check-in: **14:00**, Check-out: **12:00** ngày hôm sau
- Sau check-out: phòng **CLEANING 2 giờ** trước khi AVAILABLE

## Tác giả

- **Sinh viên**: Nguyễn Hải Nam
- **Lớp**: 12523W.1
- **Giảng viên hướng dẫn**: TS. Hoàng Quốc Việt
