// ============================================================================
// BOOKING TYPES - Định nghĩa kiểu dữ liệu đặt phòng
// ============================================================================

// Trạng thái đặt phòng
export type BookingStatus = 'PENDING' | 'CONFIRMED' | 'CHECKED_IN' | 'CHECKED_OUT' | 'CANCELLED'

// Loại đặt phòng
export type BookingType = 'HOURLY' | 'DAILY'

// Thông tin đặt phòng
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

// Request tạo đặt phòng
export interface CreateBookingRequest {
  roomId: number
  bookingType: BookingType
  checkIn: string
  checkOut: string
}

// Request cập nhật trạng thái đặt phòng
export interface UpdateBookingStatusRequest {
  status: BookingStatus
}
