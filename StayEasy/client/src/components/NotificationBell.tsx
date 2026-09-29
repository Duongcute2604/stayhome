import { useState, useEffect } from 'react'
import { Link } from 'react-router-dom'
import { notificationService } from '../services/notificationService'
import { useAuth } from '../hooks/useAuth'

// ============================================================================
// NOTIFICATION BELL - Chuông thông báo với số chưa đọc (polling 30s)
// ============================================================================

const POLL_INTERVAL = 30000

export default function NotificationBell() {
  const { isAuthenticated } = useAuth()
  const [unread, setUnread] = useState(0)

  useEffect(() => {
    if (!isAuthenticated) {
      setUnread(0)
      return
    }

    const load = async () => {
      try {
        setUnread(await notificationService.getUnreadCount())
      } catch {
        // Im lặng khi lỗi để không làm phiền user
      }
    }

    load()
    const timer = setInterval(load, POLL_INTERVAL)
    return () => clearInterval(timer)
  }, [isAuthenticated])

  if (!isAuthenticated) return null

  return (
    <Link to="/notifications" className="relative inline-block" title="Thông báo">
      <span className="text-2xl">🔔</span>
      {unread > 0 && (
        <span className="absolute -top-1 -right-1 bg-red-600 text-white text-xs rounded-full px-1.5 py-0.5">
          {unread > 99 ? '99+' : unread}
        </span>
      )}
    </Link>
  )
}
