import api from './api'
import type { Location, CreateLocationRequest, UpdateLocationRequest } from '../types/location'

// ============================================================================
// LOCATION SERVICE - API địa điểm (backend trả ApiResponse { data, ... })
// ============================================================================

interface ApiResponse<T> {
  data: T
  message: string
  statusCode: number
}

export const locationService = {
  async getAll(): Promise<Location[]> {
    const response = await api.get<ApiResponse<Location[]>>('/locations')
    return response.data.data
  },

  async getById(id: number): Promise<Location> {
    const response = await api.get<ApiResponse<Location>>(`/locations/${id}`)
    return response.data.data
  },

  async create(data: CreateLocationRequest): Promise<Location> {
    const response = await api.post<ApiResponse<Location>>('/locations', data)
    return response.data.data
  },

  async update(id: number, data: UpdateLocationRequest): Promise<Location> {
    const response = await api.put<ApiResponse<Location>>(`/locations/${id}`, data)
    return response.data.data
  },

  async delete(id: number): Promise<void> {
    await api.delete(`/locations/${id}`)
  },
}
