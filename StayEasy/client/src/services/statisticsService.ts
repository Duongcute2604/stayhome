import api from './api'
import type { RevenuePoint, Occupancy, TopRoom, Summary } from '../types/statistics'

// ============================================================================
// STATISTICS SERVICE - API thống kê (Admin/Employee)
// ============================================================================

interface ApiResponse<T> {
  data: T
  message: string
  statusCode: number
}

export const statisticsService = {
  async getSummary(): Promise<Summary> {
    const response = await api.get<ApiResponse<Summary>>('/statistics/summary')
    return response.data.data
  },

  async getRevenue(year?: number): Promise<RevenuePoint[]> {
    const response = await api.get<ApiResponse<RevenuePoint[]>>('/statistics/revenue', {
      params: { year },
    })
    return response.data.data
  },

  async getOccupancy(year?: number, month?: number): Promise<Occupancy> {
    const response = await api.get<ApiResponse<Occupancy>>('/statistics/occupancy', {
      params: { year, month },
    })
    return response.data.data
  },

  async getTopRooms(top = 5): Promise<TopRoom[]> {
    const response = await api.get<ApiResponse<TopRoom[]>>('/statistics/top-rooms', {
      params: { top },
    })
    return response.data.data
  },
}
