import api from './api'
import type { Notification } from '../types/notification'

// ============================================================================
// NOTIFICATION SERVICE - API thông báo (backend trả ApiResponse { data, ... })
// ============================================================================

interface ApiResponse<T> {
  data: T
  message: string
  statusCode: number
}

export const notificationService = {
  async getAll(): Promise<Notification[]> {
    const response = await api.get<ApiResponse<Notification[]>>('/notifications')
    return response.data.data
  },

  async getUnreadCount(): Promise<number> {
    const response = await api.get<ApiResponse<number>>('/notifications/unread-count')
    return response.data.data
  },

  async markRead(id: number): Promise<void> {
    await api.put(`/notifications/${id}/read`)
  },

  async markAllRead(): Promise<void> {
    await api.put('/notifications/read-all')
  },

  async remove(id: number): Promise<void> {
    await api.delete(`/notifications/${id}`)
  },
}
