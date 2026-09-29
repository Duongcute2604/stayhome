import { useState, type FormEvent } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import { bookingService } from '../services/bookingService'
import type { BookingType } from '../types/booking'

// ============================================================================
// BOOKING PAGE - Trang đặt phòng
// ============================================================================

export default function Booking() {
  const { roomId } = useParams<{ roomId: string }>()
  const navigate = useNavigate()
  const [bookingType, setBookingType] = useState<BookingType>('DAILY')
  const [checkIn, setCheckIn] = useState('')
  const [checkOut, setCheckOut] = useState('')
  const [error, setError] = useState('')
  const [isLoading, setIsLoading] = useState(false)

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault()
    setError('')
    setIsLoading(true)

    try {
      await bookingService.createBooking({
        roomId: parseInt(roomId!),
        bookingType,
        checkIn,
        checkOut,
      })
      navigate('/my-bookings')
    } catch (err: unknown) {
      const message = err instanceof Error ? err.message : 'Đặt phòng thất bại'
      setError(message)
    } finally {
      setIsLoading(false)
    }
  }

  return (
    <div className="container py-8 max-w-2xl">
      <h1 className="text-2xl font-bold mb-6">Đặt phòng</h1>

      {error && (
        <div className="bg-red-50 text-red-600 p-3 rounded mb-4">
          {error}
        </div>
      )}

      <form onSubmit={handleSubmit} className="card space-y-4">
        <div>
          <label className="block text-sm font-medium mb-1">Loại đặt phòng</label>
          <select
            className="input"
            value={bookingType}
            onChange={(e) => setBookingType(e.target.value as BookingType)}
          >
            <option value="DAILY">Theo ngày</option>
            <option value="HOURLY">Theo giờ</option>
          </select>
        </div>

        <div>
          <label className="block text-sm font-medium mb-1">Ngày nhận phòng</label>
          <input
            type="datetime-local"
            className="input"
            value={checkIn}
            onChange={(e) => setCheckIn(e.target.value)}
            required
          />
        </div>

        <div>
          <label className="block text-sm font-medium mb-1">Ngày trả phòng</label>
          <input
            type="datetime-local"
            className="input"
            value={checkOut}
            onChange={(e) => setCheckOut(e.target.value)}
            required
          />
        </div>

        <button
          type="submit"
          className="btn btn-primary w-full"
          disabled={isLoading}
        >
          {isLoading ? 'Đang đặt...' : 'Xác nhận đặt phòng'}
        </button>
      </form>
    </div>
  )
}
