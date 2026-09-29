import { useContext } from 'react'
import { AuthContext } from '../context/AuthContext'
import type { AuthContextType } from '../types/auth'

// ============================================================================
// USE AUTH HOOK - Hook để truy cập AuthContext
// ============================================================================
// Tại sao cần: Tránh phải import useContext + AuthContext ở mỗi component
// ============================================================================

export function useAuth(): AuthContextType {
  const context = useContext(AuthContext)
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider')
  }
  return context
}
