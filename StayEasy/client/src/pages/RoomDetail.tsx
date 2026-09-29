import { useState, useEffect } from 'react'
import { useParams, Link } from 'react-router-dom'
import { roomService } from '../services/roomService'
import { reviewService } from '../services/reviewService'
import { formatVnd } from '../utils/format'
import { useAuth } from '../hooks/useAuth'
import ReviewList from '../components/ReviewList'
import ReviewForm from '../components/ReviewForm'
import { useToast } from '../hooks/useToast'
import type { Room } from '../types/room'
import type { Review } from '../types/review'

// ============================================================================
// ROOM DETAIL PAGE - Trang chi tiết phòng
// ============================================================================

export default function RoomDetail() {
  const { id } = useParams<{ id: string }>()
  const { isAuthenticated, user } = useAuth()
  const { toast } = useToast()
  const [room, setRoom] = useState<Room | null>(null)
  const [reviews, setReviews] = useState<Review[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    if (id) {
      const roomId = parseInt(id)
      loadRoom(roomId)
      loadReviews(roomId)
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

  const loadReviews = async (roomId: number) => {
    try {
      setReviews(await reviewService.getByRoom(roomId))
    } catch {
      // Không chặn trang nếu lỗi tải reviews
    }
  }

  const handleDeleteReview = async (reviewId: number) => {
    if (!confirm('Bạn có chắc muốn xóa đánh giá này?')) return
    try {
      await reviewService.remove(reviewId)
      if (id) {
        const roomId = parseInt(id)
        loadReviews(roomId)
        loadRoom(roomId) // Cập nhật lại rating trung bình
      }
    } catch (err) {
      toast('Không thể xóa đánh giá', 'error')
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
          <h1 className="text-2xl font-bold mb-4 text-left">{room.name}</h1>
          <p className="text-gray-600 mb-4 text-left">{room.description}</p>

          <div className="space-y-2 mb-6">
            <div className="flex justify-between">
              <span className="text-left"><strong>Sức chứa:</strong></span>
              <span className="text-right">{room.capacity} khách</span>
            </div>
            <div className="flex justify-between">
              <span className="text-left"><strong>Giá theo giờ:</strong></span>
              <span className="number-vn text-right">{formatVnd(room.pricePerHour)}</span>
            </div>
            <div className="flex justify-between">
              <span className="text-left"><strong>Giá theo ngày:</strong></span>
              <span className="number-vn text-right">{formatVnd(room.pricePerDay)}</span>
            </div>
            <div className="flex justify-between">
              <span className="text-left"><strong>Trạng thái:</strong></span>
              <span className="text-right">{room.status}</span>
            </div>
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

      {/* Đánh giá */}
      <div className="mt-12">
        <h2 className="text-xl font-bold mb-4 text-left">
          Đánh giá ⭐ {room.avgRating.toFixed(1)} ({room.reviewCount} đánh giá)
        </h2>

        {isAuthenticated && (
          <ReviewForm
            roomId={room.id}
            onSuccess={() => {
              loadReviews(room.id)
              loadRoom(room.id)
            }}
          />
        )}

        <ReviewList
          reviews={reviews}
          currentUserId={user?.id}
          isAdmin={user?.role === 'Admin'}
          onDelete={handleDeleteReview}
        />
      </div>
    </div>
  )
}
