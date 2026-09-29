import api from './api'
import type { Review, CreateReviewRequest } from '../types/review'

// ============================================================================
// REVIEW SERVICE - API đánh giá (backend trả ApiResponse { data, ... })
// ============================================================================

interface ApiResponse<T> {
  data: T
  message: string
  statusCode: number
}

export const reviewService = {
  // Lấy reviews của phòng (public)
  async getByRoom(roomId: number): Promise<Review[]> {
    const response = await api.get<ApiResponse<Review[]>>(`/reviews/room/${roomId}`)
    return response.data.data
  },

  // Tạo review (cần đăng nhập, backend check đã CHECKED_OUT)
  async create(data: CreateReviewRequest): Promise<Review> {
    const response = await api.post<ApiResponse<Review>>('/reviews', data)
    return response.data.data
  },

  // Xóa review (chủ hoặc Admin)
  async remove(id: number): Promise<void> {
    await api.delete(`/reviews/${id}`)
  },
}
