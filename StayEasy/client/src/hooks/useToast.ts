import { useContext } from 'react'
import { ToastContext } from '../context/ToastContext'

// ============================================================================
// USE TOAST HOOK - Hook hiển thị thông báo (toast thay alert)
// ============================================================================

export function useToast() {
  const context = useContext(ToastContext)
  if (!context) {
    throw new Error('useToast must be used within a ToastProvider')
  }
  return context
}
