// ============================================================================
// ROOM TYPES - Định nghĩa kiểu dữ liệu phòng
// ============================================================================

// Trạng thái phòng
export type RoomStatus = 'AVAILABLE' | 'OCCUPIED' | 'CLEANING' | 'MAINTENANCE'

// Thông tin phòng
export interface Room {
  id: number
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
  avgRating: number
  reviewCount: number
  createdAt: string
}

// Tiện nghi
export interface Amenity {
  id: number
  name: string
  description?: string
}

// Request tìm kiếm phòng (khớp RoomSearchRequest backend)
export interface RoomSearchParams {
  search?: string
  locationId?: number
  checkIn?: string
  checkOut?: string
  guests?: number
  capacity?: number
  minPrice?: number
  maxPrice?: number
  amenityIds?: number[]
  sortBy?: string // price_asc, price_desc, newest
  page?: number
  pageSize?: number
}

// Kết quả phân trang chuẩn (khớp PagedResult backend)
export interface PagedResult<T> {
  items: T[]
  totalCount: number
  page: number
  pageSize: number
  totalPages: number
}

// Request tạo phòng
export interface CreateRoomRequest {
  locationId: number
  name: string
  description: string
  pricePerHour: number
  pricePerDay: number
  capacity: number
  images?: string[]
  amenityIds?: number[]
}
