import { createContext, useState, useEffect, type ReactNode } from 'react'
import { authService } from '../services/authService'
import type { AuthContextType, User } from '../types/auth'

// ============================================================================
// AUTH CONTEXT - Quản lý trạng thái xác thực toàn cục
// ============================================================================
// Tại sao cần: Tránh truyền props nhiều tầng (prop drilling)
// ============================================================================

export const AuthContext = createContext<AuthContextType | undefined>(undefined)

interface AuthProviderProps {
  children: ReactNode
}

export function AuthProvider({ children }: AuthProviderProps) {
  const [user, setUser] = useState<User | null>(null)
  const [token, setToken] = useState<string | null>(null)
  const [isLoading, setIsLoading] = useState(true)

  // Khôi phục trạng thái đăng nhập khi load trang
  useEffect(() => {
    const savedToken = localStorage.getItem('token')
    const savedUser = authService.getCurrentUser()
    if (savedToken && savedUser) {
      setToken(savedToken)
      setUser(savedUser)
    }
    setIsLoading(false)
  }, [])

  // Đăng nhập
  const login = async (email: string, password: string) => {
    const response = await authService.login({ email, password })
    setToken(response.token)
    setUser(response.user)
  }

  // Đăng ký
  const register = async (data: { email: string; password: string; fullName: string; phone?: string }) => {
    await authService.register(data)
    // Tự động đăng nhập sau khi đăng ký
    await login(data.email, data.password)
  }

  // Đăng xuất
  const logout = () => {
    authService.logout()
    setToken(null)
    setUser(null)
  }

  const value: AuthContextType = {
    user,
    token,
    isAuthenticated: !!token,
    isLoading,
    login,
    register,
    logout,
  }

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}
