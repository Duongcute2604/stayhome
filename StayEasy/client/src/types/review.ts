// ============================================================================
// REVIEW TYPES - Định nghĩa kiểu dữ liệu đánh giá (khớp ReviewDto backend)
// ============================================================================

export interface Review {
  id: number
  userId: number
  userName?: string
  roomId: number
  rating: number
  comment?: string
  createdAt: string
}

export interface CreateReviewRequest {
  roomId: number
  rating: number
  comment?: string
}
