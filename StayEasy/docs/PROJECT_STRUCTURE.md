# StayEasy — Cấu trúc project

## Sơ đồ tổng quat

```
┌─────────────────────────────────────────────────────────────┐
│                        STAYEASY                              │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│  ┌──────────────┐         REST API         ┌──────────────┐ │
│  │   FRONTEND   │  ─────────────────────▶  │   BACKEND    │ │
│  │              │    JSON over HTTP        │              │ │
│  │  React + TS  │                          │  ASP.NET Core│ │
│  │  Vite        │                          │  Web API     │ │
│  └──────────────┘                          └──────────────┘ │
│        │                                          │          │
│        │                                          │          │
│        │         ┌──────────────┐                 │          │
│        │         │    MySQL     │ ◀───────────────┘          │
│        │         │   Database   │                            │
│        │         └──────────────┘                            │
│        │                                                     │
│        └─────────────────────────────────────────────────────┘
│                         Docker Network                        │
└─────────────────────────────────────────────────────────────┘
```

## Frontend Structure (React + TypeScript + Vite)

```
client/
├── src/
│   ├── components/           # React components tái sử dụng
│   │   ├── common/           # Button, Input, Modal, etc.
│   │   ├── layout/           # Header, Footer, Sidebar
│   │   └── ui/               # Card, Badge, Spinner
│   ├── pages/                # Các trang chính
│   │   ├── Home/             # Trang chủ
│   │   ├── Login/            # Đăng nhập
│   │   ├── Register/         # Đăng ký
│   │   ├── Rooms/            # Danh sách phòng
│   │   ├── RoomDetail/       # Chi tiết phòng
│   │   ├── Booking/          # Đặt phòng
│   │   ├── MyBookings/       # Đặt phòng của tôi
│   │   ├── Admin/            # Trang quản trị
│   │   └── Profile/          # Hồ sơ
│   ├── services/             # API services
│   │   ├── api.ts            # Axios instance
│   │   ├── authService.ts    # API xác thực
│   │   ├── roomService.ts    # API phòng
│   │   └── bookingService.ts # API đặt phòng
│   ├── types/                # TypeScript types
│   │   ├── auth.ts
│   │   ├── room.ts
│   │   └── booking.ts
│   ├── utils/                # Utility functions
│   │   ├── constants.ts
│   │   ├── helpers.ts
│   │   └── validation.ts
│   ├── hooks/                # Custom React hooks
│   │   ├── useAuth.ts
│   │   └── useApi.ts
│   ├── context/              # React Context
│   │   └── AuthContext.tsx
│   ├── App.tsx               # Root component
│   ├── main.tsx              # Entry point
│   └── index.css             # Global styles
├── public/                   # Static files
├── Dockerfile
├── nginx.conf
├── package.json
├── tsconfig.json
├── vite.config.ts
└── tailwind.config.js
```

## Backend Structure (ASP.NET Core Web API)

```
server/
├── Controllers/              # API Controllers
│   ├── AuthController.ts     # Đăng ký, đăng nhập
│   ├── RoomController.ts     # CRUD phòng
│   ├── BookingController.ts  # CRUD đặt phòng
│   ├── LocationController.ts # CRUD địa điểm
│   ├── AmenityController.ts  # CRUD tiện nghi
│   ├── ReviewController.ts   # CRUD đánh giá
│   └── StatisticsController.ts # Thống kê
├── Services/                 # Business logic
│   ├── IAuthService.ts       # Interface
│   ├── AuthService.ts        # Logic xác thực
│   ├── IRoomService.ts
│   ├── RoomService.ts
│   ├── IBookingService.ts
│   ├── BookingService.ts
│   └── ...
├── Models/                   # Data models
│   ├── Entities/             # Database entities
│   │   ├── User.cs
│   │   ├── Room.cs
│   │   ├── Booking.cs
│   │   ├── Location.cs
│   │   ├── Amenity.cs
│   │   ├── Review.cs
│   │   └── ...
│   ├── DTOs/                 # Data Transfer Objects
│   │   ├── AuthDtos/
│   │   ├── RoomDtos/
│   │   └── BookingDtos/
│   └── Common/               # Common models
│       ├── ApiResponse.cs
│       └── PagedResult.cs
├── Data/                     # Database context
│   ├── StayEasyDbContext.ts  # EF Core DbContext
│   ├── Configurations/       # Entity configurations
│   └── Migrations/           # EF Core migrations
├── Middleware/               # Custom middleware
│   ├── ExceptionMiddleware.ts
│   └── JwtMiddleware.ts
├── Helpers/                  # Helper classes
│   ├── JwtHelper.ts
│   ├── PasswordHelper.ts
│   └── MappingProfile.ts
├── Dockerfile
├── Program.cs                # Entry point
├── appsettings.json          # Configuration
└── StayEasy.csproj          # Project file
```

## Database Schema (MySQL)

```
┌──────────────┐     ┌──────────────┐     ┌──────────────┐
│    users     │     │   locations  │     │    rooms     │
├──────────────┤     ├──────────────┤     ├──────────────┤
│ id (PK)      │     │ id (PK)      │     │ id (PK)      │
│ email        │     │ name         │     │ location_id  │──┐
│ password     │     │ address      │     │ name         │  │
│ full_name    │     │ description  │     │ description  │  │
│ phone        │     │ created_at   │     │ price_hour   │  │
│ role         │     └──────────────┘     │ price_day    │  │
│ created_at   │                          │ capacity     │  │
└──────────────┘                          │ status       │  │
                                          │ created_at   │  │
                                          └──────────────┘  │
                                                   ▲        │
                                                   │        │
┌──────────────┐     ┌──────────────┐              │        │
│   bookings   │     │   amenities  │              │        │
├──────────────┤     ├──────────────┤              │        │
│ id (PK)      │     │ id (PK)      │              │        │
│ user_id (FK) │────▶│ name         │              │        │
│ room_id (FK) │─────┘ description  │              │        │
│ check_in     │     └──────────────┘              │        │
│ check_out    │                                   │        │
│ total_price  │     ┌──────────────┐              │        │
│ status       │     │ room_amenity │              │        │
│ created_at   │     ├──────────────┤              │        │
└──────────────┘     │ room_id (FK) │──────────────┘        │
                     │ amenity_id   │───────────────────────┘
                     └──────────────┘
```

## API Endpoints

### Auth
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | /api/auth/register | Đăng ký |
| POST | /api/auth/login | Đăng nhập |
| GET | /api/auth/profile | Thông tin user |

### Rooms
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /api/rooms | Danh sách phòng |
| GET | /api/rooms/{id} | Chi tiết phòng |
| POST | /api/rooms | Tạo phòng (Admin) |
| PUT | /api/rooms/{id} | Cập nhật phòng (Admin) |
| DELETE | /api/rooms/{id} | Xóa phòng (Admin) |

### Bookings
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /api/bookings | Danh sách đặt phòng |
| GET | /api/bookings/{id} | Chi tiết đặt phòng |
| POST | /api/bookings | Tạo đặt phòng |
| PUT | /api/bookings/{id} | Cập nhật đặt phòng |
| DELETE | /api/bookings/{id} | Hủy đặt phòng |

### Locations
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /api/locations | Danh sách địa điểm |
| POST | /api/locations | Tạo địa điểm (Admin) |

### Reviews
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /api/reviews | Danh sách đánh giá |
| POST | /api/reviews | Tạo đánh giá |

### Statistics
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /api/statistics/revenue | Doanh thu |
| GET | /api/statistics/occupancy | Tỷ lệ lấp đầy |
