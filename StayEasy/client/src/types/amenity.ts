// ============================================================================
// AMENITY TYPES - Định nghĩa kiểu dữ liệu tiện nghi (khớp AmenityDto backend)
// ============================================================================

export interface Amenity {
  id: number
  name: string
  description?: string
  icon?: string
  category?: string
  roomCount: number
}

export interface CreateAmenityRequest {
  name: string
  description?: string
  icon?: string
  category?: string
}

export interface UpdateAmenityRequest {
  name?: string
  description?: string
  icon?: string
  category?: string
}
