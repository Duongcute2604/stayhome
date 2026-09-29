import { useState, useEffect } from 'react'
import { Link } from 'react-router-dom'
import { roomService } from '../services/roomService'
import type { Room } from '../types/room'

// ============================================================================
// ROOMS PAGE - Trang danh sách phòng
// ============================================================================

export default function Rooms() {
  const [rooms, setRooms] = useState<Room[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    loadRooms()
  }, [])

  const loadRooms = async () => {
    try {
      const data = await roomService.getRooms()
      setRooms(data)
    } catch (err) {
      setError('Không thể tải danh sách phòng')
    } finally {
      setIsLoading(false)
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
      <h1 className="text-2xl font-bold mb-6">Danh sách phòng</h1>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {rooms.map((room) => (
          <div key={room.id} className="card">
            <div className="aspect-video bg-gray-200 rounded mb-4">
              {room.images[0] && (
                <img
                  src={room.images[0]}
                  alt={room.name}
                  className="w-full h-full object-cover rounded"
                />
              )}
            </div>
            <h3 className="text-lg font-semibold mb-2">{room.name}</h3>
            <p className="text-gray-600 text-sm mb-2">{room.description}</p>
            <div className="flex justify-between items-center">
              <span className="text-blue-600 font-semibold">
                {room.pricePerDay.toLocaleString('vi-VN')}đ/đêm
              </span>
              <Link
                to={`/rooms/${room.id}`}
                className="btn btn-primary text-sm"
              >
                Xem chi tiết
              </Link>
            </div>
          </div>
        ))}
      </div>
    </div>
  )
}
