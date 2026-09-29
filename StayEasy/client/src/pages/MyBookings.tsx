import { useState, useEffect } from 'react'
import { bookingService } from '../services/bookingService'
import type { Booking } from '../types/booking'

// ============================================================================
// MY BOOKINGS PAGE - Trang đặt phòng của tôi
// ============================================================================

export default function MyBookings() {
  const [bookings, setBookings] = useState<Booking[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    loadBookings()
  }, [])

  const loadBookings = async () => {
    try {
      const data = await bookingService.getMyBookings()
      setBookings(data)
    } catch (err) {
      setError('Không thể tải danh sách đặt phòng')
    } finally {
      setIsLoading(false)
    }
  }

  const handleCancel = async (id: number) => {
    if (!confirm('Bạn có chắc muốn hủy đặt phòng này?')) return
    try {
      await bookingService.cancelBooking(id)
      loadBookings()
    } catch (err) {
      setError('Không thể hủy đặt phòng')
    }
  }

  if (isLoading) {
    return <div className="container py-8">Đang tải...</div>
  }

  if (error) {
    return <div className="container py-8 text-red-600">{error}</div>
  }

  return (
    <div className="container py-8">
      <h1 className="text-2xl font-bold mb-6">Đặt phòng của tôi</h1>

      {bookings.length === 0 ? (
        <p className="text-gray-600">Bạn chưa có đặt phòng nào.</p>
      ) : (
        <div className="space-y-4">
          {bookings.map((booking) => (
            <div key={booking.id} className="card">
              <div className="flex justify-between items-start">
                <div>
                  <h3 className="font-semibold">{booking.roomName}</h3>
                  <p className="text-sm text-gray-600">
                    {new Date(booking.checkIn).toLocaleString('vi-VN')} -{' '}
                    {new Date(booking.checkOut).toLocaleString('vi-VN')}
                  </p>
                  <p className="text-sm text-gray-600">
                    Tổng tiền: {booking.totalPrice.toLocaleString('vi-VN')}đ
                  </p>
                </div>
                <div className="text-right">
                  <span className="inline-block bg-blue-50 text-blue-600 px-3 py-1 rounded-full text-sm">
                    {booking.status}
                  </span>
                  {booking.status === 'PENDING' && (
                    <button
                      onClick={() => handleCancel(booking.id)}
                      className="block mt-2 text-red-600 text-sm hover:underline"
                    >
                      Hủy đặt phòng
                    </button>
                  )}
                </div>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  )
}
