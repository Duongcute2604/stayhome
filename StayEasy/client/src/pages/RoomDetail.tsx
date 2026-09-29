import { useState, useEffect } from 'react'
import { useParams, Link } from 'react-router-dom'
import { roomService } from '../services/roomService'
import { useAuth } from '../hooks/useAuth'
import type { Room } from '../types/room'

// ============================================================================
// ROOM DETAIL PAGE - Trang chi tiết phòng
// ============================================================================

export default function RoomDetail() {
  const { id } = useParams<{ id: string }>()
  const { isAuthenticated } = useAuth()
  const [room, setRoom] = useState<Room | null>(null)
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    if (id) {
      loadRoom(parseInt(id))
    }
  }, [id])

  const loadRoom = async (roomId: number) => {
    try {
      const data = await roomService.getRoom(roomId)
      setRoom(data)
    } catch (err) {
      setError('Không thể tải thông tin phòng')
    } finally {
      setIsLoading(false)
    }
  }

  if (isLoading) {
    return <div className="container py-8">Đang tải...</div>
  }

  if (error || !room) {
    return <div className="container py-8 text-red-600">{error || 'Không tìm thấy phòng'}</div>
  }

  return (
    <div className="container py-8">
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
        {/* Hình ảnh */}
        <div>
          <div className="aspect-video bg-gray-200 rounded-lg mb-4">
            {room.images[0] && (
              <img
                src={room.images[0]}
                alt={room.name}
                className="w-full h-full object-cover rounded-lg"
              />
            )}
          </div>
        </div>

        {/* Thông tin */}
        <div>
          <h1 className="text-2xl font-bold mb-4">{room.name}</h1>
          <p className="text-gray-600 mb-4">{room.description}</p>

          <div className="space-y-2 mb-6">
            <p><strong>Sức chứa:</strong> {room.capacity} khách</p>
            <p><strong>Giá theo giờ:</strong> {room.pricePerHour.toLocaleString('vi-VN')}đ</p>
            <p><strong>Giá theo ngày:</strong> {room.pricePerDay.toLocaleString('vi-VN')}đ</p>
            <p><strong>Trạng thái:</strong> {room.status}</p>
          </div>

          {/* Tiện nghi */}
          {room.amenities.length > 0 && (
            <div className="mb-6">
              <h3 className="font-semibold mb-2">Tiện nghi</h3>
              <div className="flex flex-wrap gap-2">
                {room.amenities.map((amenity) => (
                  <span
                    key={amenity.id}
                    className="bg-blue-50 text-blue-600 px-3 py-1 rounded-full text-sm"
                  >
                    {amenity.name}
                  </span>
                ))}
              </div>
            </div>
          )}

          {/* Nút đặt phòng */}
          {isAuthenticated && room.status === 'AVAILABLE' && (
            <Link
              to={`/booking/${room.id}`}
              className="btn btn-primary"
            >
              Đặt phòng ngay
            </Link>
          )}
        </div>
      </div>
    </div>
  )
}
