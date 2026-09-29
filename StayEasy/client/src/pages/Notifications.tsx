import { useState, useEffect } from 'react'
import { notificationService } from '../services/notificationService'
import { formatDateTime } from '../utils/date'
import type { Notification } from '../types/notification'

// ============================================================================
// NOTIFICATIONS PAGE - Danh sách thông báo của tôi
// ============================================================================

export default function Notifications() {
  const [items, setItems] = useState<Notification[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState('')

  const loadItems = async () => {
    setIsLoading(true)
    setError('')
    try {
      setItems(await notificationService.getAll())
    } catch {
      setError('Không thể tải thông báo')
    } finally {
      setIsLoading(false)
    }
  }

  useEffect(() => {
    loadItems()
  }, [])

  const handleRead = async (id: number) => {
    await notificationService.markRead(id)
    setItems((prev) => prev.map((n) => (n.id === id ? { ...n, isRead: true } : n)))
  }

  const handleReadAll = async () => {
    await notificationService.markAllRead()
    setItems((prev) => prev.map((n) => ({ ...n, isRead: true })))
  }

  const handleDelete = async (id: number) => {
    await notificationService.remove(id)
    setItems((prev) => prev.filter((n) => n.id !== id))
  }

  return (
    <div className="container py-8 max-w-2xl">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold text-left">Thông báo</h1>
        <button onClick={handleReadAll} className="btn btn-secondary text-sm">
          Đánh dấu tất cả đã đọc
        </button>
      </div>

      {isLoading && <div className="text-left">Đang tải...</div>}
      {error && <div className="text-red-600 text-left">{error}</div>}

      {!isLoading && !error && items.length === 0 && (
        <p className="text-gray-600 text-left">Bạn chưa có thông báo nào.</p>
      )}

      <div className="space-y-3">
        {items.map((item) => (
          <div key={item.id} className={`card ${item.isRead ? '' : 'border-l-4 border-blue-600'}`}>
            <div className="flex justify-between items-start">
              <div>
                <p className="font-semibold text-left">{item.title}</p>
                {item.message && <p className="text-sm text-gray-600 text-left">{item.message}</p>}
                <p className="text-xs text-gray-400 text-left mt-1">
                  {formatDateTime(item.createdAt)}
                </p>
              </div>
              <div className="flex gap-2 text-sm">
                {!item.isRead && (
                  <button
                    onClick={() => handleRead(item.id)}
                    className="text-blue-600 hover:underline"
                  >
                    Đã đọc
                  </button>
                )}
                <button
                  onClick={() => handleDelete(item.id)}
                  className="text-red-600 hover:underline"
                >
                  Xóa
                </button>
              </div>
            </div>
          </div>
        ))}
      </div>
    </div>
  )
}
