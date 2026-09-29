import { useState, useEffect } from 'react'
import { bookingService } from '../services/bookingService'
import { paymentService } from '../services/paymentService'
import { formatVnd } from '../utils/format'
import { formatDateTime } from '../utils/date'
import type { Booking } from '../types/booking'
import type { Payment } from '../types/payment'

// ============================================================================
// MY BOOKINGS PAGE - Đặt phòng của tôi (kèm trạng thái + nút thanh toán mock)
// ============================================================================

export default function MyBookings() {
  const [bookings, setBookings] = useState<Booking[]>([])
  const [payments, setPayments] = useState<Record<number, Payment>>({})
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState('')

  const loadData = async () => {
    setIsLoading(true)
    setError('')
    try {
      const [bookingData, paymentData] = await Promise.all([
        bookingService.getMyBookings(),
        paymentService.getMy(),
      ])
      setBookings(bookingData)
      // Chỉ giữ payment mới nhất mỗi booking (PAID/REFUNDED)
      const map: Record<number, Payment> = {}
      for (const p of paymentData) {
        if (!map[p.bookingId]) map[p.bookingId] = p
      }
      setPayments(map)
    } catch {
      setError('Không thể tải danh sách đặt phòng')
    } finally {
      setIsLoading(false)
    }
  }

  useEffect(() => {
    loadData()
  }, [])

  const handleCancel = async (id: number) => {
    if (!confirm('Bạn có chắc muốn hủy đặt phòng này?')) return
    try {
      await bookingService.cancelBooking(id)
      loadData()
    } catch {
      setError('Không thể hủy đặt phòng')
    }
  }

  const handlePay = async (bookingId: number) => {
    if (!confirm('Thanh toán mock (luôn thành công). Tiếp tục?')) return
    try {
      await paymentService.pay({ bookingId, method: 'MOMO' })
      loadData()
    } catch (err: unknown) {
      const msg =
        (err as { response?: { data?: { message?: string } } })?.response?.data?.message ??
        'Thanh toán thất bại'
      alert(msg)
    }
  }

  if (isLoading) {
    return <div className="container py-8 text-left">Đang tải...</div>
  }

  if (error) {
    return <div className="container py-8 text-red-600 text-left">{error}</div>
  }

  return (
    <div className="container py-8">
      <h1 className="text-2xl font-bold mb-6 text-left">Đặt phòng của tôi</h1>

      {bookings.length === 0 ? (
        <p className="text-gray-600 text-left">Bạn chưa có đặt phòng nào.</p>
      ) : (
        <div className="space-y-4">
          {bookings.map((booking) => {
            const payment = payments[booking.id]
            const canPay =
              !payment &&
              (booking.status === 'PENDING' || booking.status === 'CONFIRMED')

            return (
              <div key={booking.id} className="card">
                <div className="flex justify-between items-start">
                  <div>
                    <h3 className="font-semibold text-left">{booking.roomName}</h3>
                    <p className="text-sm text-gray-600 text-left">
                      {formatDateTime(booking.checkIn)} - {formatDateTime(booking.checkOut)}
                    </p>
                    <p className="text-sm text-gray-600 text-left">
                      Tổng tiền:{' '}
                      <span className="number-vn font-semibold">
                        {formatVnd(booking.totalPrice)}
                      </span>
                    </p>
                    {payment && (
                      <p className="text-sm text-left mt-1">
                        Thanh toán:{' '}
                        <span className="font-semibold">
                          {payment.status === 'PAID' && 'Đã thanh toán'}
                          {payment.status === 'REFUNDED' &&
                            `Đã hoàn ${formatVnd(payment.refundAmount ?? 0)}`}
                          {payment.status === 'PENDING' && 'Chờ thanh toán'}
                        </span>
                      </p>
                    )}
                  </div>
                  <div className="text-right">
                    <span className="inline-block bg-blue-50 text-blue-600 px-3 py-1 rounded-full text-sm">
                      {booking.status}
                    </span>
                    {canPay && (
                      <button
                        onClick={() => handlePay(booking.id)}
                        className="block mt-2 btn btn-primary text-sm"
                      >
                        Thanh toán ngay
                      </button>
                    )}
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
            )
          })}
        </div>
      )}
    </div>
  )
}
