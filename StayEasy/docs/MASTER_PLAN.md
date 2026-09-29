# StayEasy — Kế hoạch tổng quát toàn bộ dự án

## Tổng quan dự án

**Mục tiêu**: Xây dựng hệ thống đặt phòng và quản lý homestay hoàn chỉnh với đầy đủ tính năng cho 3 actors: Customer, Employee, Admin.

**Thời gian**: 09/2026 – 11/2026 (10 tuần)

**Trạng thái hiện tại**:
- ✅ Setup môi trường (Docker, MySQL, .NET SDK, Node.js)
- ✅ Database schema + seed data
- ✅ Backend: Auth, Rooms, Bookings (cơ bản)
- ✅ Frontend: Pages cơ bản (Home, Login, Register, Rooms, Booking, MyBookings, Admin, Profile)
- ❌ Còn thiếu: Nhiều tính năng quan trọng

---

## Quy tắc bắt buộc (Non-negotiable)

### 1. Ẩn hoàn toàn ID/UI
- **Backend**: Không trả `id` trong list/public endpoints
- **Frontend**: Tự tính STT = `index + 1 + page * size`
- **Detail endpoint**: Có `id` nhưng FE tuyệt đối không hiển thị

### 2. Căn lề (Alignment)
| Loại dữ liệu | Giao diện Web |
|--------------|---------------|
| **Chữ/Text** | `text-left` |
| **Số/Number (Giá tiền, diện tích, số phòng)** | `text-right` |

### 3. Format Số & Tiền tệ (VND)
| Input | Output |
|-------|--------|
| `500000` | `"500.000 ₫"` |
| `1500000` | `"1.500.000 ₫"` |

```typescript
export const formatVnd = (value: number) => {
  return new Intl.NumberFormat('vi-VN').format(value) + ' ₫'
}
```

### 4. UI Components
- `.number-vn` cho số tiền (căn phải)
- `.text-left` cho tên, địa chỉ

### 5. Tech Stack đầy đủ
- **Frontend**: React 18 + Vite + TypeScript + TailwindCSS + TanStack Query + Zustand + React Hook Form + Zod
- **Backend**: ASP.NET Core Web API + EF Core + JWT (Access + Refresh Token)
- **Database**: MySQL 8.0
- **Image Storage**: Local filesystem (lưu path trong DB)

---

## Modules chính

### Module 1: Authentication & Authorization ✅ (Hoàn thành)
- [x] Đăng ký tài khoản
- [x] Đăng nhập với JWT
- [x] Phân quyền (Customer, Employee, Admin)
- [x] Bảo vệ routes

### Module 2: Room Management ✅ (Hoàn thành)
- [x] CRUD phòng
- [x] Hiển thị danh sách phòng
- [x] Chi tiết phòng
- [x] Quản lý trạng thái phòng

### Module 3: Booking Management ✅ (Hoàn thành)
- [x] Đặt phòng theo giờ/ngày
- [x] Kiểm tra phòng trống
- [x] Quản lý trạng thái đặt phòng
- [x] Hủy đặt phòng

### Module 4: Location & Amenity Management ❌ (Chưa làm)
- [ ] CRUD địa điểm homestay
- [ ] CRUD tiện nghi
- [ ] Gán tiện nghi cho phòng
- [ ] Lọc phòng theo địa điểm

### Module 5: Review & Rating ❌ (Chưa làm)
- [ ] Đánh giá phòng sau khi trả phòng
- [ ] Hiển thị rating trung bình
- [ ] Xem danh sách đánh giá
- [ ] Admin xóa review vi phạm

### Module 6: Notification System ❌ (Chưa làm)
- [ ] Thông báo khi đặt phòng được xác nhận
- [ ] Thông báo nhắc nhở check-in
- [ ] Đánh dấu đã đọc
- [ ] Số thông báo chưa đọc

### Module 7: Search & Filter ❌ (Chưa làm)
- [ ] Tìm kiếm theo tên phòng
- [ ] Lọc theo giá, sức chứa, tiện nghi
- [ ] Sắp xếp theo giá, rating
- [ ] Phân trang

### Module 8: Statistics & Reporting ❌ (Chưa làm)
- [ ] Dashboard thống kê
- [ ] Doanh thu theo tháng/năm
- [ ] Tỷ lệ lấp đầy phòng
- [ ] Top phòng được đặt nhiều nhất

### Module 9: Payment Integration ❌ (Chưa làm)
- [ ] Tích hợp VPay/MoMo
- [ ] Lưu lịch sử thanh toán
- [ ] Hoàn tiền khi hủy
- [ ] Trạng thái thanh toán

### Module 10: UI/UX Polish ❌ (Chưa làm)
- [ ] Responsive design
- [ ] Loading states
- [ ] Error handling
- [ ] Toast notifications
- [ ] Dark mode (optional)

---

## Thứ tự phát triển đề xuất

### Phase 1: Core Features (Tuần 1-3)
**Mục tiêu**: Hoàn thiện các tính năng cốt lõi

| Thứ tự | Module | Ưu tiên | Lý do |
|--------|--------|---------|-------|
| 1 | Location & Amenity | Cao | Cần thiết cho Room Management |
| 2 | Review & Rating | Cao | Tăng độ tin cậy của hệ thống |
| 3 | Search & Filter | Trung bình | Cải thiện UX |

### Phase 2: User Experience (Tuần 4-6)
**Mục tiêu**: Cải thiện trải nghiệm người dùng

| Thứ tự | Module | Ưu tiên | Lý do |
|--------|--------|---------|-------|
| 4 | Notification System | Cao | Giữ người dùng thông tin |
| 5 | UI/UX Polish | Trung bình | Hoàn thiện giao diện |

### Phase 3: Business Intelligence (Tuần 7-8)
**Mục tiêu**: Hỗ trợ quản trị doanh nghiệp

| Thứ tự | Module | Ưu tiên | Lý do |
|--------|--------|---------|-------|
| 6 | Statistics & Reporting | Trung bình | Hỗ trợ ra quyết định |

### Phase 4: Monetization (Tuần 9-10)
**Mục tiêu**: Tích hợp thanh toán

| Thứ tự | Module | Ưu tiên | Lý do |
|--------|--------|---------|-------|
| 7 | Payment Integration | Thấp | Phức tạp, cần tích hợp bên ngoài |

---

## Chi tiết từng Module

### Module 4: Location & Amenity Management

#### Backend
```
server/
├── Controllers/
│   ├── LocationController.cs
│   └── AmenityController.cs
├── Services/
│   ├── ILocationService.cs
│   ├── LocationService.cs
│   ├── IAmenityService.cs
│   └── AmenityService.cs
├── Models/
│   ├── DTOs/
│   │   ├── LocationDtos.cs
│   │   └── AmenityDtos.cs
│   └── Entities/ (đã có)
│       ├── Location.cs
│       ├── Amenity.cs
│       └── RoomAmenity.cs
```

#### Frontend
```
client/src/
├── services/
│   ├── locationService.ts
│   └── amenityService.ts
├── types/
│   ├── location.ts
│   └── amenity.ts
├── utils/
│   └── format.ts
├── pages/
│   ├── LocationManager.tsx
│   └── AmenityManager.tsx
└── components/
    ├── LocationForm.tsx
    └── AmenityForm.tsx
```

#### API Endpoints
| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | /api/locations | Danh sách địa điểm (không id) |
| GET | /api/locations/{id} | Chi tiết địa điểm (có id) |
| POST | /api/locations | Tạo địa điểm (Admin) |
| PUT | /api/locations/{id} | Cập nhật địa điểm (Admin) |
| DELETE | /api/locations/{id} | Xóa địa điểm (Admin) |
| GET | /api/amenities | Danh sách tiện nghi (không id) |
| GET | /api/amenities/{id} | Chi tiết tiện nghi (có id) |
| POST | /api/amenities | Tạo tiện nghi (Admin) |
| PUT | /api/amenities/{id} | Cập nhật tiện nghi (Admin) |
| DELETE | /api/amenities/{id} | Xóa tiện nghi (Admin) |

---

### Module 5: Review & Rating

#### Backend
```
server/
├── Controllers/
│   └── ReviewController.cs
├── Services/
│   ├── IReviewService.cs
│   └── ReviewService.cs
├── Models/
│   ├── DTOs/
│   │   └── ReviewDtos.cs
│   └── Entities/ (đã có)
│       ├── Review.cs
│       └── ReviewImage.cs
```

#### Frontend
```
client/src/
├── services/
│   └── reviewService.ts
├── pages/
│   └── Reviews.tsx
└── components/
    ├── ReviewList.tsx
    ├── ReviewForm.tsx
    └── StarRating.tsx
```

#### API Endpoints
| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | /api/reviews/room/{roomId} | Reviews của phòng |
| POST | /api/reviews | Tạo review |
| DELETE | /api/reviews/{id} | Xóa review (Admin) |
| GET | /api/reviews/user/{userId} | Reviews của user |

#### Quy tắc nghiệp vụ
- Chỉ đánh giá sau khi `CHECKED_OUT`
- Rating 1-5 sao
- Tối đa 5 ảnh mỗi review
- 1 user chỉ đánh giá 1 lần mỗi booking

---

### Module 6: Notification System

#### Backend
```
server/
├── Controllers/
│   └── NotificationController.cs
├── Services/
│   ├── INotificationService.cs
│   └── NotificationService.cs
├── Models/
│   ├── DTOs/
│   │   └── NotificationDtos.cs
│   └── Entities/ (đã có)
│       └── Notification.cs
```

#### Frontend
```
client/src/
├── services/
│   └── notificationService.ts
├── pages/
│   └── Notifications.tsx
└── components/
    ├── NotificationBell.tsx
    └── NotificationList.tsx
```

#### API Endpoints
| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | /api/notifications | Thông báo của user |
| GET | /api/notifications/unread-count | Số chưa đọc |
| PUT | /api/notifications/{id}/read | Đánh dấu đã đọc |
| PUT | /api/notifications/read-all | Đánh dấu tất cả |
| DELETE | /api/notifications/{id} | Xóa thông báo |

#### Quy tắc nghiệp vụ
- Tự động tạo thông báo khi:
  - Đặt phòng thành công
  - Admin xác nhận/hủy
  - Nhắc nhở check-in (trước 24h)
- Polling mỗi 30s

---

### Module 7: Search & Filter

#### Backend
```
server/
├── Controllers/
│   └── RoomController.cs (cập nhật)
├── Services/
│   └── RoomService.cs (cập nhật)
└── Models/
    └── DTOs/
        └── RoomSearchRequest.cs
```

#### Frontend
```
client/src/
├── services/
│   └── roomService.ts (cập nhật)
├── pages/
│   └── Rooms.tsx (cập nhật)
└── components/
    └── SearchFilters.tsx
```

#### API Endpoints
| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | /api/rooms/search | Tìm kiếm & lọc |

#### Query Parameters
- `search` - Từ khóa
- `locationId` - Địa điểm
- `minPrice`, `maxPrice` - Giá
- `capacity` - Sức chứa
- `amenityIds` - Tiện nghi
- `sortBy` - Sắp xếp
- `page`, `pageSize` - Phân trang

---

### Module 8: Statistics & Reporting

#### Backend
```
server/
├── Controllers/
│   └── StatisticsController.cs
├── Services/
│   ├── IStatisticsService.cs
│   └── StatisticsService.cs
└── Models/
    └── DTOs/
        └── StatisticsDtos.cs
```

#### Frontend
```
client/src/
├── services/
│   └── statisticsService.ts
├── pages/
│   └── Dashboard.tsx
└── components/
    ├── RevenueChart.tsx
    ├── OccupancyChart.tsx
    └── TopRooms.tsx
```

#### API Endpoints
| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | /api/statistics/revenue | Doanh thu theo tháng |
| GET | /api/statistics/occupancy | Tỷ lệ lấp đầy |
| GET | /api/statistics/top-rooms | Top phòng |
| GET | /api/statistics/summary | Tổng quan |

---

### Module 9: Payment Integration

#### Backend
```
server/
├── Controllers/
│   └── PaymentController.cs
├── Services/
│   ├── IPaymentService.cs
│   └── PaymentService.cs
├── Models/
│   ├── DTOs/
│   │   └── PaymentDtos.cs
│   └── Entities/
│       └── Payment.cs (mới)
└── Data/
    └── StayEasyDbContext.cs (cập nhật)
```

#### Frontend
```
client/src/
├── services/
│   └── paymentService.ts
├── pages/
│   └── PaymentPage.tsx
└── components/
    └── PaymentHistory.tsx
```

#### API Endpoints
| Method | Endpoint | Mô tả |
|--------|----------|-------|
| POST | /api/payments/create | Tạo thanh toán |
| GET | /api/payments/{id} | Chi tiết |
| POST | /api/payments/{id}/refund` | Hoàn tiền |
| GET | /api/payments/history` | Lịch sử |

---

### Module 10: UI/UX Polish

#### Frontend
```
client/src/
├── components/
│   ├── common/
│   │   ├── Button.tsx
│   │   ├── Input.tsx
│   │   ├── Modal.tsx
│   │   └── Spinner.tsx
│   ├── layout/
│   │   ├── Header.tsx
│   │   ├── Footer.tsx
│   │   └── Sidebar.tsx
│   └── ui/
│       ├── Card.tsx
│       ├── Badge.tsx
│       └── Toast.tsx
├── hooks/
│   └── useToast.ts
└── context/
    └── ToastContext.tsx
```

---

## Timeline

| Tuần | Phase | Tasks |
|------|-------|-------|
| 1 | Phase 1 | Location & Amenity Management |
| 2 | Phase 1 | Review & Rating |
| 3 | Phase 1 | Search & Filter |
| 4 | Phase 2 | Notification System |
| 5 | Phase 2 | UI/UX Polish |
| 6 | Phase 2 | Testing & Bug fixes |
| 7 | Phase 3 | Statistics & Reporting |
| 8 | Phase 3 | Testing & Bug fixes |
| 9 | Phase 4 | Payment Integration |
| 10 | Phase 4 | Final testing & Deployment |

---

## Files cần tạo (Tổng)

### Backend
- **Controllers**: 7 files
- **Services**: 14 files (7 interfaces + 7 implementations)
- **DTOs**: 7 files
- **Entities**: 1 file (Payment)
- **Database**: 1 migration

### Frontend
- **Services**: 7 files
- **Pages**: 10 files
- **Components**: 15+ files
- **Context**: 1 file (Toast)

**Tổng cộng**: ~60+ files

---

## Bước tiếp theo

1. **Duyệt kế hoạch này** - Bạn có muốn điều chỉnh gì không?
2. **Chọn Module đầu tiên** - Theo đề xuất, bắt đầu với **Location & Amenity Management**
3. **Plan chi tiết** - Tôi sẽ đi vào chi tiết từng file, từng API endpoint

---

**Bạn muốn bắt đầu với Module nào?** Hoặc muốn điều chỉng kế hoạch trước?