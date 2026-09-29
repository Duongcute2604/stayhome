import api from './api'
import type { LoginRequest, LoginResponse, RegisterRequest, User } from '../types/auth'

// ============================================================================
// AUTH SERVICE - Xử lý các API liên quan đến xác thực
// ============================================================================

export const authService = {
  // Đăng ký tài khoản mới
  async register(data: RegisterRequest): Promise<void> {
    const response = await api.post('/auth/register', data)
    const { token, user } = response.data
    localStorage.setItem('token', token)
    localStorage.setItem('user', JSON.stringify(user))
  },

  // Đăng nhập
  async login(data: LoginRequest): Promise<LoginResponse> {
    const response = await api.post('/auth/login', data)
    const { token, user } = response.data
    localStorage.setItem('token', token)
    localStorage.setItem('user', JSON.stringify(user))
    return response.data
  },

  // Đăng xuất
  logout(): void {
    localStorage.removeItem('token')
    localStorage.removeItem('user')
  },

  // Lấy thông tin user hiện tại
  getCurrentUser(): User | null {
    const userStr = localStorage.getItem('user')
    if (!userStr) return null
    try {
      return JSON.parse(userStr)
    } catch {
      return null
    }
  },

  // Kiểm tra đã đăng nhập chưa
  isAuthenticated(): boolean {
    return !!localStorage.getItem('token')
  },
}
