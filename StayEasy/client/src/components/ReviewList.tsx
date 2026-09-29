import { formatDate } from '../utils/date'
import type { Review } from '../types/review'

// ============================================================================
// REVIEW LIST - Danh sách đánh giá của phòng
// ============================================================================

interface Props {
  reviews: Review[]
  currentUserId?: number
  isAdmin?: boolean
  onDelete: (id: number) => void
}

function Stars({ value }: { value: number }) {
  return (
    <span className="text-yellow-500">
      {'★'.repeat(value)}
      <span className="text-gray-300">{'★'.repeat(5 - value)}</span>
    </span>
  )
}

export default function ReviewList({ reviews, currentUserId, isAdmin, onDelete }: Props) {
  if (reviews.length === 0) {
    return <p className="text-gray-600 text-left">Chưa có đánh giá nào.</p>
  }

  return (
    <div className="space-y-4">
      {reviews.map((review) => (
        <div key={review.id} className="card">
          <div className="flex justify-between items-start">
            <div>
              <p className="font-semibold text-left">{review.userName ?? 'Khách hàng'}</p>
              <Stars value={review.rating} />
            </div>
            <div className="text-right">
              <p className="text-sm text-gray-500">{formatDate(review.createdAt)}</p>
              {(isAdmin || currentUserId === review.userId) && (
                <button
                  onClick={() => onDelete(review.id)}
                  className="text-red-600 text-sm hover:underline"
                >
                  Xóa
                </button>
              )}
            </div>
          </div>
          {review.comment && <p className="text-gray-700 mt-2 text-left">{review.comment}</p>}
        </div>
      ))}
    </div>
  )
}
