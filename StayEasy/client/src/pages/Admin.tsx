import { useState, useEffect } from 'react'
import { roomService } from '../services/roomService'
import { bookingService } from '../services/bookingService'
import type { Room } from '../types/room'
import type { Booking } from '../types/booking'

// ============================================================================
// ADMIN PAGE - Trang quản trị
// ============================================================================

export default function Admin() {
  const [activeTab, setActiveTab] = useState<'rooms' | 'bookings'>('rooms')
  const [rooms, setRooms] = useState<Room[]>([])
  const [bookings, setBookings] = useState<Booking[]>([])
  const [isLoading, setIsLoading] = useState(true)

  useEffect(() => {
    loadData()
  }, [])

  const loadData = async () => {
    setIsLoading(true)
    try {
      const [roomsData, bookingsData] = await Promise.all([
        roomService.getRooms(),
        bookingService.getAllBookings(),
      ])
      setRooms(roomsData)
      setBookings(bookingsData)
    } catch (err) {
      console.error('Lỗi tải dữ liệu:', err)
    } finally {
      setIsLoading(false)
    }
  }

  if (isLoading) {
    return <div className="container py-8">Đang tải...</div>
  }

  return (
    <div className="container py-8">
      <h1 className="text-2xl font-bold mb-6">Quản trị</h1>

      {/* Tabs */}
      <div className="flex gap-4 mb-6">
        <button
          onClick={() => setActiveTab('rooms')}
          className={`btn ${activeTab === 'rooms' ? 'btn-primary' : 'btn-secondary'}`}
        >
          Quản lý phòng
        </button>
        <button
          onClick={() => setActiveTab('bookings')}
          className={`btn ${activeTab === 'bookings' ? 'btn-primary' : 'btn-secondary'}`}
        >
          Quản lý đặt phòng
        </button>
      </div>

      {/* Rooms Tab */}
      {activeTab === 'rooms' && (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          {rooms.map((room) => (
            <div key={room.id} className="card">
              <h3 className="font-semibold mb-2">{room.name}</h3>
              <p className="text-sm text-gray-600 mb-2">{room.description}</p>
              <p className="text-sm">Trạng thái: {room.status}</p>
            </div>
          ))}
        </div>
      )}

      {/* Bookings Tab */}
      {activeTab === 'bookings' && (
        <div className="space-y-4">
          {bookings.map((booking) => (
            <div key={booking.id} className="card">
              <div className="flex justify-between">
                <div>
                  <h3 className="font-semibold">{booking.roomName}</h3>
                  <p className="text-sm text-gray-600">
                    {new Date(booking.checkIn).toLocaleString('vi-VN')} -{' '}
                    {new Date(booking.checkOut).toLocaleString('vi-VN')}
                  </p>
                </div>
                <span className="bg-blue-50 text-blue-600 px-3 py-1 rounded-full text-sm">
                  {booking.status}
                </span>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  )
}
