# StayEasy — Technical Overview

## 🎯 Mục đích
Hệ thống Đặt phòng và Quản lý Homestay toàn diện gồm: Trang Quản trị Web Admin (React + Vite), REST API Backend (ASP.NET Core) và Cơ sở dữ liệu MySQL.

---

## 🛠 Tech Stack

| Layer | Technology | Version/Note |
|-------|------------|--------------|
| **Web Admin / Client** | React 18 + Vite + TypeScript | React Router v6 |
| | UI & Styling | TailwindCSS + Headless UI / shadcn/ui |
| | State & Data Fetching | TanStack Query + Zustand |
| | Forms & Validation | React Hook Form + Zod |
| | API Client | Axios / Fetch (Typed from API contracts) |
| **Backend API** | **ASP.NET Core Web API** (.NET 8+) | C# End-to-end |
| | Architecture | Clean Architecture / Layered Architecture |
| | Auth | JWT (Access + Refresh Token) + ASP.NET Core Identity / Custom Roles (Admin, Host, Guest) |
| | Database & ORM | **MySQL** + **Entity Framework Core (EF Core)** |
| | **Image Storage** | **Local filesystem** (lưu đường dẫn relative path trong DB) |
| **API Contract** | OpenAPI 3.1 (Swagger) | `docs/api/openapi.yaml` (Single source of truth) |

---

## 📐 Kiến trúc Tổng Quát

```
┌────────────────────────────────────────┐     REST API (JSON)      ┌─────────────────────────┐
│         Web Admin / Client             │ ◄──────────────────────► │   ASP.NET Core Web API  │
│    (React + TypeScript + Vite)         │                          │     (Clean Architecture)│
└────────────────────────────────────────┘                          └────────────┬────────────┘
                                                                                 │
                                                                        ┌────────▼────────────┐
                                                                        │  MySQL + EF Core    │
                                                                        └─────────────────────┘
```

- **Monorepo / Workspace**: Quản lý gọn gàng với thư mục chung cho Frontend (`client/`) và Backend (`server/`).
- **Shared types**: Đồng bộ kiểu dữ liệu TypeScript từ C# DTOs hoặc OpenAPI spec.

---

## 🔒 Quy Tắc Bắt Buộc (Non-negotiable)

### 1. Ẩn hoàn toàn ID/UI
- **Backend**: Không trả `id` (GUID/UUID nội bộ) trong **list/public endpoints** (hoặc chỉ trả mã định danh công khai thân thiện như Booking Code).
- **Frontend**: Tự tính STT hiển thị bảng = `index + 1 + page * size` (1, 2, 3...).
- **Detail endpoint**: Có `id` nội bộ nhưng **FE tuyệt đối không hiển thị thô ra giao diện người dùng**.

### 2. Căn lề (Alignment)
| Loại dữ liệu | Giao diện Web (TailwindCSS) |
|--------------|-----------------------------|
| **Chữ/Text** | `text-left` |
| **Số/Number (Giá tiền, diện tích, số phòng, số lượng)** | `text-right` |

### 3. Format Số & Tiền tệ (Vietnamese Locale - VND)
| Input | Output | Quy tắc |
|-------|--------|---------|
| `500000` | `"500.000 ₫"` hoặc `"500.000"` | Dấu chấm phân cách nghìn |
| `1500000` | `"1.500.000"` | |
| `0` | `"0"` | |

**Implementation (TypeScript):**
```typescript
export const formatVnd = (value: number) => {
  return new Intl.NumberFormat('vi-VN').format(value);
};
```

### 4. UI Components Chung
- Sử dụng class `.number-vn` (cho các cột số tiền, giá phòng, đơn giá căn lề phải).
- Sử dụng class `.text-left` cho tên phòng, địa chỉ location, tiện nghi.

---

## 📁 Cấu Trúc Thư Mục (Skeleton)

```
StayEasy/
├── PROJECT_OVERVIEW.md           # File tổng quan kỹ thuật này
├── docker-compose.yml            # Docker cấu hình MySQL + Backend
├── .env.example
├── .gitignore
├── README.md
├── client/                       # React + TypeScript + Vite (Web Admin / Client)
├── server/                       # ASP.NET Core Web API (.NET 8)
└── docs/
    ├── requirements/             # Tài liệu nghiệp vụ, SRS
    ├── api/                      # OpenAPI specs & Swagger docs
    ├── database/                 # Sơ đồ ERD, migration scripts MySQL
    ├── diagrams/                 # Sơ đồ kiến trúc, sequence diagrams
    └── decisions/                # ADRs (Architecture Decision Records)
```

---

## 📋 Trạng Thái Dự Án

| Item | Status |
|------|--------|
| Tech Stack | ✅ Đã quyết định (React + ASP.NET Core + MySQL) |
| Architecture | ✅ Đã quyết định (Layered/Clean Architecture) |
| UI Rules & Formatting | ✅ Đã quyết định |
| Image Storage | ✅ **Local filesystem** (lưu path trong DB) |
| Database Schema | ⏳ **Chưa định nghĩa** (cần thiết kế Entities: Location, Room, Amenity, Booking, User) |
| Features/Modules | ⏳ **Chưa định nghĩa** (Quản lý Location, Phòng, Tiện nghi, Đặt phòng) |
| API Endpoints | ⏳ **Chưa định nghĩa** |

---

## 🚀 Bước Tiếp Theo

1. **Thiết kế Database Schema (MySQL + EF Core)** — Xác định các bảng chính: `Locations`, `Rooms`, `Amenities`, `RoomAmenities`, `Bookings`, `Users`.
2. **Xây dựng Backend Core (ASP.NET Core)** — Khởi tạo cấu trúc Web API, cấu hình DbContext kết nối MySQL và Authentication (JWT).
3. **Xây dựng Frontend (React + Vite)** — Khởi tạo app, cấu hình Axios, Router, và TailwindCSS.
4. **Hiện thực các tính năng cốt lõi (Module theo yêu cầu)**: Quản lý Location, Quản lý Tiện nghi (Amenity), xử lý validation/edge cases (trùng lặp, ràng buộc xóa phòng/tiện nghi).