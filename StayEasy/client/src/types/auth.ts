// ============================================================================
// AUTH TYPES - Định nghĩa kiểu dữ liệu xác thực
// ============================================================================

// Vai trò người dùng trong hệ thống
export type UserRole = 'Customer' | 'Employee' | 'Admin'

// Thông tin người dùng
export interface User {
  id: number
  email: string
  fullName: string
  phone?: string
  role: UserRole
  createdAt: string
}

// Request đăng ký
export interface RegisterRequest {
  email: string
  password: string
  fullName: string
  phone?: string
}

// Request đăng nhập
export interface LoginRequest {
  email: string
  password: string
}

// Response đăng nhập (chứa JWT token)
export interface LoginResponse {
  token: string
  user: User
}

// Context xác thực
export interface AuthContextType {
  user: User | null
  token: string | null
  isAuthenticated: boolean
  isLoading: boolean
  login: (email: string, password: string) => Promise<void>
  register: (data: RegisterRequest) => Promise<void>
  logout: () => void
}
