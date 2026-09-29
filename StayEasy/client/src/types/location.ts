// ============================================================================
// LOCATION TYPES - Định nghĩa kiểu dữ liệu địa điểm (khớp LocationDto backend)
// ============================================================================
// Lưu ý: id chỉ dùng làm key nội bộ, KHÔNG hiển thị ra UI
// ============================================================================

export interface Location {
  id: number
  name: string
  address?: string
  description?: string
  imageUrl?: string
  roomCount: number
}

export interface CreateLocationRequest {
  name: string
  address?: string
  description?: string
  imageUrl?: string
}

export interface UpdateLocationRequest {
  name?: string
  address?: string
  description?: string
  imageUrl?: string
}
