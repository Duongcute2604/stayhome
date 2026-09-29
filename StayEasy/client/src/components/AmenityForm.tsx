import { useState, type FormEvent } from 'react'
import { amenityService } from '../services/amenityService'
import type { Amenity, CreateAmenityRequest } from '../types/amenity'

// ============================================================================
// AMENITY FORM - Form thêm/sửa tiện nghi (dùng trong Modal)
// ============================================================================

interface Props {
  amenity: Amenity | null
  onClose: () => void
  onSuccess: () => void
}

const CATEGORIES = ['WiFi', 'Nấu nướng', 'Giải trí', 'An toàn', 'Khác']

export default function AmenityForm({ amenity, onClose, onSuccess }: Props) {
  const [name, setName] = useState(amenity?.name ?? '')
  const [description, setDescription] = useState(amenity?.description ?? '')
  const [icon, setIcon] = useState(amenity?.icon ?? '')
  const [category, setCategory] = useState(amenity?.category ?? '')
  const [isLoading, setIsLoading] = useState(false)
  const [error, setError] = useState('')

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault()
    setError('')
    setIsLoading(true)

    try {
      const data: CreateAmenityRequest = { name, description, icon, category }
      if (amenity) {
        await amenityService.update(amenity.id, data)
      } else {
        await amenityService.create(data)
      }
      onSuccess()
    } catch (err: unknown) {
      const msg =
        (err as { response?: { data?: { message?: string } } })?.response?.data?.message ??
        'Lưu thất bại'
      setError(msg)
    } finally {
      setIsLoading(false)
    }
  }

  return (
    <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
      <div className="card w-full max-w-md">
        <h2 className="text-xl font-bold mb-4 text-left">
          {amenity ? 'Sửa tiện nghi' : 'Thêm tiện nghi'}
        </h2>

        {error && <div className="bg-red-50 text-red-600 p-3 rounded mb-4 text-left">{error}</div>}

        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium mb-1 text-left">Tên tiện nghi</label>
            <input
              type="text"
              className="input"
              value={name}
              onChange={(e) => setName(e.target.value)}
              required
            />
          </div>

          <div>
            <label className="block text-sm font-medium mb-1 text-left">Mô tả</label>
            <textarea
              className="input"
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              rows={2}
            />
          </div>

          <div>
            <label className="block text-sm font-medium mb-1 text-left">Icon (emoji)</label>
            <input
              type="text"
              className="input"
              value={icon}
              onChange={(e) => setIcon(e.target.value)}
              placeholder="📶"
            />
          </div>

          <div>
            <label className="block text-sm font-medium mb-1 text-left">Danh mục</label>
            <select className="input" value={category} onChange={(e) => setCategory(e.target.value)}>
              <option value="">Chọn danh mục</option>
              {CATEGORIES.map((cat) => (
                <option key={cat} value={cat}>
                  {cat}
                </option>
              ))}
            </select>
          </div>

          <div className="flex gap-2">
            <button type="button" onClick={onClose} className="btn btn-secondary flex-1">
              Hủy
            </button>
            <button type="submit" className="btn btn-primary flex-1" disabled={isLoading}>
              {isLoading ? 'Đang lưu...' : amenity ? 'Cập nhật' : 'Thêm'}
            </button>
          </div>
        </form>
      </div>
    </div>
  )
}
