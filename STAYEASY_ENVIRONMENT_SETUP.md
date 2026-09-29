# StayEasy — Danh sách chuẩn bị môi trường phát triển

## 0. Mục tiêu

Dự án: **StayEasy — Hệ thống đặt phòng và quản lý homestay**

Hai nhóm người dùng chính:
- **Khách hàng**
- **Admin**

Không tạo actor Employee/Nhân viên riêng nếu chưa có yêu cầu mới.

---

## 1. Kiến trúc chính

```text
React + TypeScript + Vite
          |
          | REST API / JSON
          v
ASP.NET Core Web API
          |
          | Entity Framework Core
          v
        MySQL
```

Docker dùng để chạy môi trường ổn định:

```text
Docker Compose
├── frontend
├── backend
└── mysql
```

OpenChamber/OpenCode dùng để hỗ trợ viết, kiểm tra và sửa code; không giao cho agent giữ các process chạy vô hạn trong cùng một turn.

---

## 2. Phần mềm cần cài trên Windows

### Bắt buộc

- [ ] Docker Desktop
- [ ] Git
- [ ] Node.js LTS
- [ ] npm
- [ ] .NET SDK phù hợp với project ASP.NET Core
- [ ] IDE/editor đang dùng cho OpenChamber
- [ ] MySQL client (khuyến nghị DBeaver hoặc MySQL Workbench)

### Kiểm tra sau khi cài

```powershell
git --version
node -v
npm -v
dotnet --version
docker --version
docker compose version
```

Tất cả lệnh phải trả về phiên bản hợp lệ.

---

## 3. Cấu trúc project chuẩn

```text
StayEasy/
├── client/                    # React + TypeScript + Vite
├── server/                    # ASP.NET Core Web API
├── docker-compose.yml
├── .env.example
├── .gitignore
├── README.md
└── docs/
    ├── requirements/
    ├── api/
    ├── database/
    ├── diagrams/
    └── decisions/
```

Không tự ý đổi tên hoặc di chuyển thư mục đang tồn tại nếu chưa kiểm tra project.

---

## 4. Frontend

Công nghệ:

- React
- TypeScript
- Vite

Cần kiểm tra:

```powershell
cd client
npm install
npm run build
```

`npm run build` phải chạy thành công trước khi coi frontend đã ổn.

Trong quá trình phát triển, frontend có thể chạy riêng bằng:

```powershell
npm run dev
```

Không yêu cầu OpenChamber giữ `npm run dev` chạy mãi trong một turn.

---

## 5. Backend

Công nghệ:

- ASP.NET Core Web API
- Entity Framework Core
- REST API
- Swagger/OpenAPI nếu project sử dụng

Kiểm tra:

```powershell
cd server
dotnet restore
dotnet build
```

Chạy phát triển:

```powershell
dotnet watch run
```

Không dùng PM2 cho ASP.NET Core.

---

## 6. Database

Database:

```text
MySQL
Database name: StayEasy
```

Các nhóm dữ liệu dự kiến:

```text
User
Location
Room
RoomImage
Amenity
RoomAmenity
Booking
BookingStatusHistory
Review
ReviewImage
Notification
```

Schema cuối cùng phải được chốt trước khi code hàng loạt API.

---

## 7. Docker

Docker Compose dự kiến quản lý:

```text
stayeasy-frontend
stayeasy-backend
stayeasy-mysql
```

Các lệnh cơ bản:

```powershell
docker compose up -d
docker compose ps
docker compose logs
docker compose down
docker compose up -d --build
```

Khi sửa Dockerfile hoặc dependency:

```powershell
docker compose up -d --build
```

### Database phải có volume

Không được cấu hình MySQL theo cách làm mất database khi container bị xóa.

Mục tiêu:

```text
Container MySQL bị xóa
        ↓
Volume vẫn còn
        ↓
Database StayEasy vẫn còn
```

---

## 8. Biến môi trường

Không commit mật khẩu thật vào Git.

Tạo:

```text
.env.example
```

và nếu cần:

```text
.env
```

`.env` thật phải nằm trong `.gitignore`.

Các thông tin có thể cần:

```text
MYSQL_HOST
MYSQL_PORT
MYSQL_DATABASE
MYSQL_USER
MYSQL_PASSWORD

API_URL
```

Tên biến cuối cùng phải thống nhất với Docker Compose và backend.

---

## 9. API

Backend cung cấp API cho frontend.

Nhóm API dự kiến:

```text
/auth
/users
/locations
/rooms
/amenities
/bookings
/reviews
/statistics
```

Không code hàng loạt endpoint trước khi nghiệp vụ và database được chốt.

---

## 10. Quy tắc nghiệp vụ phải chuẩn bị

### Đặt theo giờ

- Tối thiểu 3 giờ.
- Kiểm tra phòng trống.
- Không được đặt trùng thời gian.
- Phải đặt trước thời gian nhận phòng ít nhất 2 giờ.

### Đặt theo ngày

- Check-in tiêu chuẩn: 14:00.
- Check-out tiêu chuẩn: 12:00 ngày hôm sau.
- Phải đặt trước ít nhất 2 giờ.
- Không được đặt trùng.

### Sau check-out

```text
OCCUPIED
   ↓
CLEANING
   ↓ 2 giờ
AVAILABLE
```

Trong 2 giờ vệ sinh, phòng không được nhận booking mới.

---

## 11. Quy tắc giá

Đặt theo giờ:

```text
Số giờ tính tiền = max(3, số giờ hợp lệ theo quy tắc hệ thống)
Tổng tiền = số giờ tính tiền × PricePerHour
```

Đặt theo ngày:

```text
Tổng tiền = số ngày × PricePerDay
```

Booking phải lưu giá tại thời điểm đặt để giá phòng thay đổi sau này không làm thay đổi lịch sử booking.

---

## 12. Thứ tự setup

Làm theo thứ tự:

1. [ ] Kiểm tra Git
2. [ ] Kiểm tra Node/npm
3. [ ] Kiểm tra .NET SDK
4. [ ] Kiểm tra Docker Desktop
5. [ ] Kiểm tra Docker Compose
6. [ ] Xác định cấu trúc `client/server`
7. [ ] Kiểm tra frontend build
8. [ ] Kiểm tra backend build
9. [ ] Tạo/chốt MySQL
10. [ ] Tạo Docker Compose
11. [ ] Tạo volume MySQL
12. [ ] Kết nối backend → MySQL
13. [ ] Chạy migration
14. [ ] Kiểm tra API
15. [ ] Kiểm tra frontend gọi API
16. [ ] Sau khi môi trường ổn định mới bắt đầu code feature.

---

## 13. Tiêu chí môi trường sẵn sàng

Chỉ coi môi trường READY khi:

- `npm run build` thành công.
- `dotnet build` thành công.
- MySQL chạy ổn định.
- Backend kết nối được MySQL.
- Migration chạy được.
- API khởi động được.
- Frontend gọi được API.
- Docker Compose khởi động được toàn bộ stack.
- Xóa/recreate container MySQL không làm mất dữ liệu trong volume.
- OpenChamber có thể đọc project và các file quy tắc.

---

## 14. Lưu ý về tài liệu PROJECT_OVERVIEW cũ

Nếu project có một `PROJECT_OVERVIEW.md` khác thuộc hệ thống khác, **không được copy nguyên tech stack hoặc kiến trúc của nó sang StayEasy**.

StayEasy hiện tại phải giữ:

```text
React + TypeScript + Vite
ASP.NET Core Web API
Entity Framework Core
MySQL
Docker
```

Hai actor:

```text
Khách hàng
Admin
```
