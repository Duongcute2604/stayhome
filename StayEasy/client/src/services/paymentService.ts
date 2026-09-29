import api from './api'
import type { Payment, PayRequest } from '../types/payment'

// ============================================================================
// PAYMENT SERVICE - API thanh toán MOCK
// ============================================================================

interface ApiResponse<T> {
  data: T
  message: string
  statusCode: number
}

export const paymentService = {
  // Thanh toán booking (mock: thành công ngay)
  async pay(data: PayRequest): Promise<Payment> {
    const response = await api.post<ApiResponse<Payment>>('/payments/pay', data)
    return response.data.data
  },

  // Lịch sử thanh toán của tôi
  async getMy(): Promise<Payment[]> {
    const response = await api.get<ApiResponse<Payment[]>>('/payments/my')
    return response.data.data
  },

  // Payment của 1 booking (404 nếu chưa thanh toán)
  async getByBooking(bookingId: number): Promise<Payment | null> {
    try {
      const response = await api.get<ApiResponse<Payment>>(`/payments/booking/${bookingId}`)
      return response.data.data
    } catch {
      return null
    }
  },
}
