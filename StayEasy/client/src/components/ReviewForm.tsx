import { useState, type FormEvent } from 'react'
import { reviewService } from '../services/reviewService'

// ============================================================================
// REVIEW FORM - Form gửi đánh giá (sao + nhận xét)
// ============================================================================
// Lưu ý: backend chặn nếu chưa CHECKED_OUT hoặc đã review rồi,
// FE hiển thị message lỗi đó cho user.
// ============================================================================

interface Props {
  roomId: number
  onSuccess: () => void
}

export default function ReviewForm({ roomId, onSuccess }: Props) {
  const [rating, setRating] = useState(5)
  const [comment, setComment] = useState('')
  const [error, setError] = useState('')
  const [isLoading, setIsLoading] = useState(false)

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault()
    setError('')
    setIsLoading(true)

    try {
      await reviewService.create({ roomId, rating, comment })
      setComment('')
      setRating(5)
      onSuccess()
    } catch (err: unknown) {
      const msg =
        (err as { response?: { data?: { message?: string } } })?.response?.data?.message ??
        'Gửi đánh giá thất bại'
      setError(msg)
    } finally {
      setIsLoading(false)
    }
  }

  return (
    <form onSubmit={handleSubmit} className="card mb-6">
      <h3 className="font-semibold mb-4 text-left">Viết đánh giá</h3>

      {error && <div className="bg-red-50 text-red-600 p-3 rounded mb-4 text-left">{error}</div>}

      <div className="mb-4">
        <label className="block text-sm font-medium mb-1 text-left">Số sao</label>
        <div className="flex gap-1">
          {[1, 2, 3, 4, 5].map((star) => (
            <button
              key={star}
              type="button"
              onClick={() => setRating(star)}
              className={`text-2xl ${star <= rating ? 'text-yellow-500' : 'text-gray-300'}`}
            >
              ★
            </button>
          ))}
        </div>
      </div>

      <div className="mb-4">
        <label className="block text-sm font-medium mb-1 text-left">Nhận xét</label>
        <textarea
          className="input"
          value={comment}
          onChange={(e) => setComment(e.target.value)}
          rows={3}
          placeholder="Chia sẻ trải nghiệm của bạn..."
        />
      </div>

      <button type="submit" className="btn btn-primary" disabled={isLoading}>
        {isLoading ? 'Đang gửi...' : 'Gửi đánh giá'}
      </button>
    </form>
  )
}
