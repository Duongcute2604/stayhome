# stayeasy-shared-types

## Skill quản lý Type definitions dùng chung

### Khi nào sử dụng
- Định nghĩa TypeScript interfaces khớp với DTOs của ASP.NET Core API
- Tạo hàm tiện ích định dạng tiền tệ VND
- Tạo hàm tiện ích định dạng ngày tháng
- Chia sẻ types giữa các components

---

## Quy tắc bắt buộc

### 1. Ẩn hoàn toàn ID/UI
```typescript
// ❌ KHÔNG hiển thị ID thô
<div>ID: {room.id}</div>

// ✅ Chỉ hiển thị ID trong detail, không hiển thị trên UI
// Frontend tự tính STT = index + 1 + page * size
const stt = index + 1 + (page - 1) * pageSize
```

### 2. Căn lề (Alignment)
```typescript
// Text → text-left
<div className="text-left">{room.name}</div>
<div className="text-left">{location.address}</div>

// Number → text-right
<div className="text-right">{formatVnd(room.pricePerDay)}</div>
<div className="text-right">{room.capacity}</div>
```

### 3. Format VND
```typescript
export const formatVnd = (value: number) => {
  return new Intl.NumberFormat('vi-VN').format(value) + ' ₫'
}

// Sử dụng
<span className="number-vn text-right">{formatVnd(room.pricePerDay)}</span>
```

### 4. UI Components
```typescript
// Class cho số tiền (căn phải)
<span className="number-vn text-right">{formatVnd(price)}</span>

// Class cho text (căn trái)
<span className="text-left">{name}</span>
```

---

## TypeScript Interface Pattern

### Entity Types
```typescript
// types/room.ts
export interface Room {
  id: number // Chỉ dùng trong detail
  locationId: number
  locationName?: string
  name: string
  description: string
  pricePerHour: number
  pricePerDay: number
  capacity: number
  status: RoomStatus
  images: string[]
  amenities: Amenity[]
  createdAt: string
}

export type RoomStatus = 'AVAILABLE' | 'OCCUPIED' | 'CLEANING' | 'MAINTENANCE'

export interface Amenity {
  id: number
  name: string
  description?: string
}
```

### List Response Types (không có id)
```typescript
// types/room.ts
export interface RoomListItem {
  name: string
  description: string
  pricePerDay: number
  capacity: number
  status: RoomStatus
  locationName?: string
}
```

### Request/Response Types
```typescript
// types/booking.ts
export interface CreateBookingRequest {
  roomId: number
  bookingType: BookingType
  checkIn: string
  checkOut: string
}

export type BookingType = 'HOURLY' | 'DAILY'

export interface Booking {
  id: number
  userId: number
  userName?: string
  roomId: number
  roomName?: string
  bookingType: BookingType
  checkIn: string
  checkOut: string
  totalPrice: number
  status: BookingStatus
  createdAt: string
}

export type BookingStatus = 'PENDING' | 'CONFIRMED' | 'CHECKED_IN' | 'CHECKED_OUT' | 'CANCELLED'
```

### API Response Types
```typescript
// types/api.ts
export interface ApiResponse<T> {
  data: T
  message: string
  statusCode: number
}

export interface PagedResult<T> {
  items: T[]
  totalCount: number
  page: number
  pageSize: number
  totalPages: number
}
```

---

## VND Currency Formatting

```typescript
// utils/format.ts

/**
 * Định dạng số tiền sang VND
 * @param value - Số tiền cần định dạng
 * @returns Chuỗi định dạng VND (vd: 1.500.000 ₫)
 */
export const formatVnd = (value: number): string => {
  return new Intl.NumberFormat('vi-VN').format(value) + ' ₫'
}

/**
 * Định dạng số tiền sang VND (ngắn gọn)
 * @param value - Số tiền cần định dạng
 * @returns Chuỗi định dạng ngắn (vd: 1.5M, 2.3K)
 */
export function formatVndShort(value: number): string {
  if (value >= 1_000_000_000) {
    return `${(value / 1_000_000_000).toFixed(1)}B`
  }
  if (value >= 1_000_000) {
    return `${(value / 1_000_000).toFixed(1)}M`
  }
  if (value >= 1_000) {
    return `${(value / 1_000).toFixed(1)}K`
  }
  return value.toString()
}

// Sử dụng
import { formatVnd } from '../utils/format'

const price = 1500000
console.log(formatVnd(price)) // "1.500.000 ₫"
console.log(formatVndShort(price)) // "1.5M"
```

---

## Date Formatting

```typescript
// utils/date.ts

/**
 * Định dạng ngày tháng sang định dạng Việt Nam
 * @param date - Ngày cần định dạng
 * @returns Chuỗi ngày định dạng (vd: 25/12/2026)
 */
export function formatDate(date: string | Date): string {
  const d = new Date(date)
  return d.toLocaleDateString('vi-VN')
}

/**
 * Định dạng ngày tháng có giờ
 * @param date - Ngày cần định dạng
 * @returns Chuỗi ngày giờ (vd: 25/12/2026 14:30)
 */
export function formatDateTime(date: string | Date): string {
  const d = new Date(date)
  return d.toLocaleString('vi-VN')
}

/**
 * Định dạng khoảng thời gian
 * @param startDate - Ngày bắt đầu
 * @param endDate - Ngày kết thúc
 * @returns Chuỗi khoảng thời gian (vd: 25/12 - 27/12/2026)
 */
export function formatDateRange(startDate: string | Date, endDate: string | Date): string {
  const start = new Date(startDate)
  const end = new Date(endDate)
  
  const startStr = start.toLocaleDateString('vi-VN')
  const endStr = end.toLocaleDateString('vi-VN')
  
  // Nếu cùng năm, bỏ năm ở ngày bắt đầu
  if (start.getFullYear() === end.getFullYear()) {
    const startDay = start.getDate()
    const startMonth = start.getMonth() + 1
    return `${startDay}/${startMonth} - ${endStr}`
  }
  
  return `${startStr} - ${endStr}`
}

/**
 * Tính số ngày giữa 2 ngày
 * @param startDate - Ngày bắt đầu
 * @param endDate - Ngày kết thúc
 * @returns Số ngày
 */
export function daysBetween(startDate: string | Date, endDate: string | Date): number {
  const start = new Date(startDate)
  const end = new Date(endDate)
  const diffTime = Math.abs(end.getTime() - start.getTime())
  return Math.ceil(diffTime / (1000 * 60 * 60 * 24))
}

/**
 * Tính số giờ giữa 2 ngày
 * @param startDate - Ngày bắt đầu
 * @param endDate - Ngày kết thúc
 * @returns Số giờ
 */
export function hoursBetween(startDate: string | Date, endDate: string | Date): number {
  const start = new Date(startDate)
  const end = new Date(endDate)
  const diffTime = Math.abs(end.getTime() - start.getTime())
  return Math.ceil(diffTime / (1000 * 60 * 60))
}

// Sử dụng
import { formatDate, formatDateTime, formatDateRange, daysBetween } from '../utils/date'

const checkIn = '2026-12-25'
const checkOut = '2026-12-27'

console.log(formatDate(checkIn)) // "25/12/2026"
console.log(formatDateTime(checkIn)) // "25/12/2026 00:00:00"
console.log(formatDateRange(checkIn, checkOut)) // "25/12 - 27/12/2026"
console.log(daysBetween(checkIn, checkOut)) // 2
```

---

## Validation Helpers

```typescript
// utils/validation.ts

/**
 * Kiểm tra email hợp lệ
 */
export function isValidEmail(email: string): boolean {
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
  return emailRegex.test(email)
}

/**
 * Kiểm tra số điện thoại hợp lệ (Việt Nam)
 */
export function isValidPhone(phone: string): boolean {
  const phoneRegex = /^(0|\+84)(3|5|7|8|9)\d{8}$/
  return phoneRegex.test(phone)
}

/**
 * Kiểm tra mật khẩu mạnh (ít nhất 6 ký tự)
 */
export function isValidPassword(password: string): boolean {
  return password.length >= 6
}

/**
 * Kiểm tra ngày hợp lệ
 */
export function isValidDate(date: string): boolean {
  const d = new Date(date)
  return !isNaN(d.getTime())
}

/**
 * Kiểm tra ngày bắt đầu trước ngày kết thúc
 */
export function isValidDateRange(startDate: string, endDate: string): boolean {
  const start = new Date(startDate)
  const end = new Date(endDate)
  return start < end
}
```

---

## Status Helpers

```typescript
// utils/status.ts

/**
 * Lấy màu badge theo trạng thái
 */
export function getStatusColor(status: string): string {
  switch (status) {
    case 'AVAILABLE':
      return 'bg-green-100 text-green-800'
    case 'OCCUPIED':
      return 'bg-red-100 text-red-800'
    case 'CLEANING':
      return 'bg-yellow-100 text-yellow-800'
    case 'MAINTENANCE':
      return 'bg-gray-100 text-gray-800'
    case 'PENDING':
      return 'bg-yellow-100 text-yellow-800'
    case 'CONFIRMED':
      return 'bg-green-100 text-green-800'
    case 'CHECKED_IN':
      return 'bg-blue-100 text-blue-800'
    case 'CHECKED_OUT':
      return 'bg-gray-100 text-gray-800'
    case 'CANCELLED':
      return 'bg-red-100 text-red-800'
    default:
      return 'bg-gray-100 text-gray-800'
  }
}

/**
 * Lấy text hiển thị theo trạng thái
 */
export function getStatusText(status: string): string {
  switch (status) {
    case 'AVAILABLE':
      return 'Còn trống'
    case 'OCCUPIED':
      return 'Đã có khách'
    case 'CLEANING':
      return 'Đang dọn dẹp'
    case 'MAINTENANCE':
      return 'Đang bảo trì'
    case 'PENDING':
      return 'Chờ xác nhận'
    case 'CONFIRMED':
      return 'Đã xác nhận'
    case 'CHECKED_IN':
      return 'Đã nhận phòng'
    case 'CHECKED_OUT':
      return 'Đã trả phòng'
    case 'CANCELLED':
      return 'Đã hủy'
    default:
      return status
  }
}
```

---

## Common Types

```typescript
// types/common.ts

/**
 * Pagination params
 */
export interface PaginationParams {
  page: number
  pageSize: number
}

/**
 * Sort params
 */
export interface SortParams {
  sortBy?: string
  sortOrder?: 'asc' | 'desc'
}

/**
 * Date range
 */
export interface DateRange {
  startDate: string
  endDate: string
}

/**
 * Select option
 */
export interface SelectOption {
  value: string | number
  label: string
}

/**
 * Tree node (cho dropdown phân cấp)
 */
export interface TreeNode {
  id: number
  name: string
  children?: TreeNode[]
}
```

---

## Type Guards

```typescript
// utils/typeGuards.ts

export function isRoomStatus(status: string): status is RoomStatus {
  return ['AVAILABLE', 'OCCUPIED', 'CLEANING', 'MAINTENANCE'].includes(status)
}

export function isBookingStatus(status: string): status is BookingStatus {
  return ['PENDING', 'CONFIRMED', 'CHECKED_IN', 'CHECKED_OUT', 'CANCELLED'].includes(status)
}

export function isUserRole(role: string): role is UserRole {
  return ['Customer', 'Employee', 'Admin'].includes(role)
}
```
