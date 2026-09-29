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
  createdAt: string
}

// Tiện nghi
export interface Amenity {
  id: number
  name: string
  description?: string
}

// Request tìm kiếm phòng
export interface RoomSearchParams {
  locationId?: number
  checkIn?: string
  checkOut?: string
  guests?: number
  minPrice?: number
  maxPrice?: number
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
