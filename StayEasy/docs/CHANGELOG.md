# StayEasy — Nhật ký thay đổi

## [Unreleased]

### Đã thêm
- Cấu trúc project hoàn chỉnh (Frontend + Backend + Docker)
- Docker Compose với 3 services: frontend, backend, mysql
- Dockerfile cho frontend (multi-stage: Node → Nginx)
- Dockerfile cho backend (multi-stage: SDK → Runtime)
- Nginx config cho SPA routing + API proxy
- Environment variables template (.env.example)
- Gitignore đầy đủ
- README.md với hướng dẫn cài đặt

### Frontend (React + TypeScript + Vite)
- Types: auth.ts, room.ts, booking.ts
- Services: api.ts, authService.ts, roomService.ts, bookingService.ts
- Hooks: useAuth.ts
- Context: AuthContext.tsx
- Pages: Home, Login, Register, Rooms, RoomDetail, Booking, MyBookings, Admin, Profile

### Backend (ASP.NET Core Web API)
- Entities: User, Location, Room, RoomImage, Amenity, RoomAmenity, Booking, BookingStatusHistory, Review, ReviewImage, Notification
- DTOs: AuthDtos, RoomDtos, BookingDtos
- Services: IAuthService, AuthService, IRoomService, RoomService, IBookingService, BookingService
- Controllers: AuthController, RoomController, BookingController
- Middleware: ExceptionMiddleware
- Database: StayEasyDbContext với EF Core + MySQL

### Tính năng
- Đăng ký / Đăng nhập với JWT
- Quản lý phòng (CRUD)
- Đặt phòng theo giờ / theo ngày
- Kiểm tra phòng trống
- Quản lý trạng thái đặt phòng
- Phân quyền: Customer, Employee, Admin

### Quy tắc nghiệp vụ
- Đặt phòng phải trước ít nhất 2 giờ
- Đặt theo giờ: tối thiểu 3 giờ
- Check-in: 14:00, Check-out: 12:00
- Sau check-out: phòng CLEANING 2 giờ

## [0.1.0] - 2026-09-28

### Đã thêm
- Khởi tạo project
- Cài đặt .NET SDK 8.0
- Tạo cấu trúc thư mục
- Viết tài liệu setup
