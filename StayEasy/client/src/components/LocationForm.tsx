import { useState, type FormEvent } from 'react'
import { locationService } from '../services/locationService'
import type { Location, CreateLocationRequest } from '../types/location'

// ============================================================================
// LOCATION FORM - Form thêm/sửa địa điểm (dùng trong Modal)
// ============================================================================

interface Props {
  location: Location | null
  onClose: () => void
  onSuccess: () => void
}

export default function LocationForm({ location, onClose, onSuccess }: Props) {
  const [name, setName] = useState(location?.name ?? '')
  const [address, setAddress] = useState(location?.address ?? '')
  const [description, setDescription] = useState(location?.description ?? '')
  const [imageUrl, setImageUrl] = useState(location?.imageUrl ?? '')
  const [isLoading, setIsLoading] = useState(false)
  const [error, setError] = useState('')

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault()
    setError('')
    setIsLoading(true)

    try {
      // Hiển thị message lỗi thân thiện từ backend (trùng tên, ...)
      const data: CreateLocationRequest = { name, address, description, imageUrl }
      if (location) {
        await locationService.update(location.id, data)
      } else {
        await locationService.create(data)
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
          {location ? 'Sửa địa điểm' : 'Thêm địa điểm'}
        </h2>

        {error && <div className="bg-red-50 text-red-600 p-3 rounded mb-4 text-left">{error}</div>}

        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium mb-1 text-left">Tên địa điểm</label>
            <input
              type="text"
              className="input"
              value={name}
              onChange={(e) => setName(e.target.value)}
              required
            />
          </div>

          <div>
            <label className="block text-sm font-medium mb-1 text-left">Địa chỉ</label>
            <input
              type="text"
              className="input"
              value={address}
              onChange={(e) => setAddress(e.target.value)}
            />
          </div>

          <div>
            <label className="block text-sm font-medium mb-1 text-left">Mô tả</label>
            <textarea
              className="input"
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              rows={3}
            />
          </div>

          <div>
            <label className="block text-sm font-medium mb-1 text-left">URL hình ảnh</label>
            <input
              type="url"
              className="input"
              value={imageUrl}
              onChange={(e) => setImageUrl(e.target.value)}
            />
          </div>

          <div className="flex gap-2">
            <button type="button" onClick={onClose} className="btn btn-secondary flex-1">
              Hủy
            </button>
            <button type="submit" className="btn btn-primary flex-1" disabled={isLoading}>
              {isLoading ? 'Đang lưu...' : location ? 'Cập nhật' : 'Thêm'}
            </button>
          </div>
        </form>
      </div>
    </div>
  )
}
