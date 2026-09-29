import api from './api'
import type { Room, RoomSearchParams, CreateRoomRequest } from '../types/room'

// ============================================================================
// ROOM SERVICE - Xử lý các API liên quan đến phòng
// ============================================================================

export const roomService = {
  // Lấy danh sách phòng (có thể lọc)
  async getRooms(params?: RoomSearchParams): Promise<Room[]> {
    const response = await api.get('/rooms', { params })
    return response.data
  },

  // Lấy chi tiết phòng
  async getRoom(id: number): Promise<Room> {
    const response = await api.get(`/rooms/${id}`)
    return response.data
  },

  // Tạo phòng mới (Admin)
  async createRoom(data: CreateRoomRequest): Promise<Room> {
    const response = await api.post('/rooms', data)
    return response.data
  },

  // Cập nhật phòng (Admin)
  async updateRoom(id: number, data: Partial<CreateRoomRequest>): Promise<Room> {
    const response = await api.put(`/rooms/${id}`, data)
    return response.data
  },

  // Xóa phòng (Admin)
  async deleteRoom(id: number): Promise<void> {
    await api.delete(`/rooms/${id}`)
  },

  // Kiểm tra phòng trống
  async checkAvailability(roomId: number, checkIn: string, checkOut: string): Promise<boolean> {
    const response = await api.get(`/rooms/${roomId}/availability`, {
      params: { checkIn, checkOut },
    })
    return response.data
  },
}
