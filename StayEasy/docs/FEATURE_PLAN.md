# StayEasy - Kế hoạch tổng quan 6 tính năng

## Tổng quan

| # | Tính năng | Độ ưu tiên | Phạm vi | Files liên quan |
|---|-----------|-----------|---------|-----------------|
| 1 | Reviews - Đánh giá phòng | Cao | Frontend + Backend | `Review.cs`, `ReviewImage.cs`, `reviewService.ts`, `ReviewController.cs` |
| 2 | Notifications - Thông báo | Cao | Frontend + Backend | `Notification.cs`, `notificationService.ts`, `NotificationController.cs` |
| 3 | Tìm kiếm & lọc phòng | Trung bình | Frontend + Backend | `roomService.ts`, `RoomController.cs`, `RoomService.cs` |
| 4 | Quản lý Locations & Amenities | Trung bình | Frontend + Backend | `Location.cs`, `Amenity.cs`, `LocationController.cs`, `AmenityController.cs` |
| 5 | Thống kê & báo cáo | Trung bình | Frontend + Backend | `StatisticsController.cs`, `statisticsService.ts` |
| 6 | Thanh toán | Thấp | Frontend + Backend | `Payment.cs`, `PaymentController.cs`, `paymentService.ts` |

---

## 1. Reviews - Đánh giá phòng

### Mục đích
- Khách hàng đánh giá phòng sau khi trả phòng
- Hiển thị rating trung bình và số lượng đánh giá
- Admin có thể xóa review vi phạm

### Phạm vi
- **Backend**: `ReviewController`, `ReviewService`, `IReviewService`
- **Frontend**: `reviewService.ts`, `ReviewList.tsx`, `ReviewForm.tsx`
- **Database**: Đã có sẵn bảng `reviews`, `review_images`

### API Endpoints
| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | `/api/reviews/room/{roomId}` | Lấy reviews của phòng |
| POST | `/api/reviews` | Tạo review mới |
| DELETE | `/api/reviews/{id}` | Xóa review (Admin) |
| GET | `/api/reviews/user/{userId}` | Lấy reviews của user |

### Quy tắc nghiệp vụ
- Chỉ đánh giá sau khi `CHECKED_OUT`
- Rating từ 1-5 sao
- Có thể thêm hình ảnh (tối đa 5 ảnh)
- Mỗi user chỉ được đánh giá 1 lần mỗi booking

---

## 2. Notifications - Thông báo

### Mục đích
- Thông báo khi đặt phòng được xác nhận/hủy
- Thông báo nhắc nhở check-in
- Hiển thị số thông báo chưa đọc

### Phạm vi
- **Backend**: `NotificationController`, `NotificationService`, `INotificationService`
- **Frontend**: `notificationService.ts`, `NotificationBell.tsx`, `NotificationList.tsx`
- **Database**: Đã có sẵn bảng `notifications`

### API Endpoints
| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | `/api/notifications` | Lấy thông báo của user |
| GET | `/api/notifications/unread-count` | Số thông báo chưa đọc |
| PUT | `/api/notifications/{id}/read` | Đánh dấu đã đọc |
| PUT | `/api/notifications/read-all` | Đánh dấu tất cả đã đọc |
| DELETE | `/api/notifications/{id}` | Xóa thông báo |

### Quy tắc nghiệp vụ
- Tự động tạo thông báo khi:
  - Đặt phòng thành công
  - Admin xác nhận/hủy đặt phòng
  - Nhắc nhở check-in (trước 24h)
- Polling mỗi 30s hoặc WebSocket (nếu có thời gian)

---

## 3. Tìm kiếm & lọc phòng

### Mục đích
- Tìm kiếm phòng theo tên, vị trí
- Lọc theo giá, sức chứa, tiện nghi
- Sắp xếp theo giá, rating

### Phạm vi
- **Backend**: Cập nhật `RoomController`, `RoomService`
- **Frontend**: Cập nhật `roomService.ts`, `Rooms.tsx`, `SearchFilters.tsx`
- **Database**: Không thay đổi

### API Endpoints
| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | `/api/rooms/search` | Tìm kiếm & lọc phòng |

### Query Parameters
- `search` - Từ khóa tìm kiếm
- `locationId` - Lọc theo địa điểm
- `minPrice`, `maxPrice` - Lọc theo giá
- `capacity` - Sức chứa tối thiểu
- `amenityIds` - Lọc theo tiện nghi
- `sortBy` - Sắp xếp (price_asc, price_desc, rating)
- `page`, `pageSize` - Phân trang

---

## 4. Quản lý Locations & Amenities

### Mục đích
- CRUD địa điểm homestay
- CRUD tiện nghi
- Gán tiện nghi cho phòng

### Phạm vi
- **Backend**: `LocationController`, `AmenityController`, `LocationService`, `AmenityService`
- **Frontend**: `locationService.ts`, `amenityService.ts`, `LocationManager.tsx`, `AmenityManager.tsx`
- **Database**: Đã có sẵn bảng `locations`, `amenities`, `room_amenities`

### API Endpoints

**Locations:**
| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | `/api/locations` | Lấy tất cả địa điểm |
| POST | `/api/locations` | Tạo địa điểm mới |
| PUT | `/api/locations/{id}` | Cập nhật địa điểm |
| DELETE | `/api/locations/{id}` | Xóa địa điểm |

**Amenities:**
| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | `/api/amenities` | Lấy tất cả tiện nghi |
| POST | `/api/amenities` | Tạo tiện nghi mới |
| PUT | `/api/amenities/{id}` | Cập nhật tiện nghi |
| DELETE | `/api/amenities/{id}` | Xóa tiện nghi |

---

## 5. Thống kê & báo cáo

### Mục đích
- Dashboard thống kê cho Admin
- Doanh thu theo tháng/năm
- Tỷ lệ lấp đầy phòng
- Biểu đồ trực quan

### Phạm vi
- **Backend**: `StatisticsController`, `StatisticsService`
- **Frontend**: `statisticsService.ts`, `Dashboard.tsx`, `RevenueChart.tsx`
- **Database**: Không thay đổi (dùng query từ các bảng có sẵn)

### API Endpoints
| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | `/api/statistics/revenue` | Doanh thu theo tháng |
| GET | `/api/statistics/occupancy` | Tỷ lệ lấp đầy |
| GET | `/api/statistics/top-rooms` | Top phòng được đặt nhiều |
| GET | `/api/statistics/summary` | Tổng quan dashboard |

### Quy tắc nghiệp vụ
- Doanh thu tính từ bookings có status `CHECKED_OUT`
- Tỷ lệ lấp đầy = (số ngày đã đặt / tổng số ngày) * 100
- Chỉ Admin/Employee mới xem được

---

## 6. Thanh toán

### Mục đích
- Tích hợp cổng thanh toán (VPay, MoMo)
- Lưu lịch sử thanh toán
- Hoàn tiền khi hủy đặt phòng

### Phạm vi
- **Backend**: `PaymentController`, `PaymentService`, `Payment.cs` (entity mới)
- **Frontend**: `paymentService.ts`, `PaymentPage.tsx`, `PaymentHistory.tsx`
- **Database**: Thêm bảng `payments`

### API Endpoints
| Method | Endpoint | Mô tả |
|--------|----------|-------|
| POST | `/api/payments/create` | Tạo yêu cầu thanh toán |
| GET | `/api/payments/{id}` | Lấy thông tin thanh toán |
| GET | `/api/payments/booking/{bookingId}` | Lấy thanh toán theo booking |
| POST | `/api/payments/{id}/refund` | Hoàn tiền |
| GET | `/api/payments/history` | Lịch sử thanh toán |

### Quy tắc nghiệp vụ
- Thanh toán trước khi check-in
- Hoàn tiền 100% nếu hủy trước 24h
- Hoàn tiền 50% nếu hủc trong 24h
- Lưu transaction ID từ cổng thanh toán

---

## Thứ tự đề xuất

| Thứ tự | Tính năng | Lý do |
|--------|-----------|-------|
| 1 | Reviews | Đơn giản, không phụ thuộc tính năng khác |
| 2 | Notifications | Độc lập, dễ test |
| 3 | Tìm kiếm & lọc | Cải thiện UX, không ảnh hưởng khác |
| 4 | Locations & Amenities | CRUD cơ bản |
| 5 | Thống kê | Cần dữ liệu từ nhiều bảng |
| 6 | Thanh toán | Phức tạp nhất, cần tích hợp bên ngoài |

---

## Files cần tạo

### Backend
```
server/
├── Controllers/
│   ├── ReviewController.cs
│   ├── NotificationController.cs
│   ├── LocationController.cs
│   ├── AmenityController.cs
│   ├── StatisticsController.cs
│   └── PaymentController.cs
├── Services/
│   ├── IReviewService.cs
│   ├── ReviewService.cs
│   ├── INotificationService.cs
│   ├── NotificationService.cs
│   ├── ILocationService.cs
│   ├── LocationService.cs
│   ├── IAmenityService.cs
│   ├── AmenityService.cs
│   ├── IStatisticsService.cs
│   ├── StatisticsService.cs
│   ├── IPaymentService.cs
│   └── PaymentService.cs
├── Models/
│   ├── DTOs/
│   │   ├── ReviewDtos.cs
│   │   ├── NotificationDtos.cs
│   │   ├── LocationDtos.cs
│   │   ├── AmenityDtos.cs
│   │   ├── StatisticsDtos.cs
│   │   └── PaymentDtos.cs
│   └── Entities/
│       └── Payment.cs
└── Data/
    └── StayEasyDbContext.cs (cập nhật)
```

### Frontend
```
client/src/
├── services/
│   ├── reviewService.ts
│   ├── notificationService.ts
│   ├── locationService.ts
│   ├── amenityService.ts
│   ├── statisticsService.ts
│   └── paymentService.ts
├── pages/
│   ├── Reviews.tsx
│   ├── Notifications.tsx
│   ├── LocationManager.tsx
│   ├── AmenityManager.tsx
│   ├── Dashboard.tsx
│   └── PaymentPage.tsx
└── components/
    ├── ReviewList.tsx
    ├── ReviewForm.tsx
    ├── NotificationBell.tsx
    ├── NotificationList.tsx
    ├── SearchFilters.tsx
    ├── RevenueChart.tsx
    └── PaymentHistory.tsx
```

---

## Bước tiếp theo

1. **Duyệt kế hoạch này** - Bạn có muốn điều chỉnh gì không?
2. **Chọn tính năng đầu tiên** - Theo đề xuất, bắt đầu với **Reviews**
3. **Plan chi tiết** - Tôi sẽ đi vào chi tiết từng file, từng API endpoint

---

**Bạn muốn bắt đầu với tính năng nào?** Hoặc muốn điều chỉnh kế hoạch trước?