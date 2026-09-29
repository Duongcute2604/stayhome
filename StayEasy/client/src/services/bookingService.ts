import api from './api'
import type { Booking, CreateBookingRequest, UpdateBookingStatusRequest } from '../types/booking'

// ============================================================================
// BOOKING SERVICE - Xử lý các API liên quan đến đặt phòng
// ============================================================================

export const bookingService = {
  // Lấy danh sách đặt phòng của user hiện tại
  async getMyBookings(): Promise<Booking[]> {
    const response = await api.get('/bookings/my')
    return response.data
  },

  // Lấy tất cả đặt phòng (Admin/Employee)
  async getAllBookings(): Promise<Booking[]> {
    const response = await api.get('/bookings')
    return response.data
  },

  // Lấy chi tiết đặt phòng
  async getBooking(id: number): Promise<Booking> {
    const response = await api.get(`/bookings/${id}`)
    return response.data
  },

  // Tạo đặt phòng mới
  async createBooking(data: CreateBookingRequest): Promise<Booking> {
    const response = await api.post('/bookings', data)
    return response.data
  },

  // Cập nhật trạng thái đặt phòng
  async updateStatus(id: number, data: UpdateBookingStatusRequest): Promise<Booking> {
    const response = await api.put(`/bookings/${id}/status`, data)
    return response.data
  },

  // Hủy đặt phòng
  async cancelBooking(id: number): Promise<void> {
    await api.delete(`/bookings/${id}`)
  },
}
