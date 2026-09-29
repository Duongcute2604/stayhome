import api from './api'
import type { Amenity, CreateAmenityRequest, UpdateAmenityRequest } from '../types/amenity'

// ============================================================================
// AMENITY SERVICE - API tiện nghi (backend trả ApiResponse { data, ... })
// ============================================================================

interface ApiResponse<T> {
  data: T
  message: string
  statusCode: number
}

export const amenityService = {
  async getAll(): Promise<Amenity[]> {
    const response = await api.get<ApiResponse<Amenity[]>>('/amenities')
    return response.data.data
  },

  async getById(id: number): Promise<Amenity> {
    const response = await api.get<ApiResponse<Amenity>>(`/amenities/${id}`)
    return response.data.data
  },

  async create(data: CreateAmenityRequest): Promise<Amenity> {
    const response = await api.post<ApiResponse<Amenity>>('/amenities', data)
    return response.data.data
  },

  async update(id: number, data: UpdateAmenityRequest): Promise<Amenity> {
    const response = await api.put<ApiResponse<Amenity>>(`/amenities/${id}`, data)
    return response.data.data
  },

  async delete(id: number): Promise<void> {
    await api.delete(`/amenities/${id}`)
  },
}
