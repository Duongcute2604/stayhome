import { useState, useEffect } from 'react'
import { amenityService } from '../services/amenityService'
import AmenityForm from '../components/AmenityForm'
import { useToast } from '../hooks/useToast'
import type { Amenity } from '../types/amenity'

// ============================================================================
// AMENITY MANAGER - Trang quản lý tiện nghi (chỉ Admin)
// ============================================================================

export default function AmenityManager() {
  const { toast } = useToast()
  const [amenities, setAmenities] = useState<Amenity[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState('')
  const [showForm, setShowForm] = useState(false)
  const [editing, setEditing] = useState<Amenity | null>(null)

  const loadAmenities = async () => {
    setIsLoading(true)
    setError('')
    try {
      setAmenities(await amenityService.getAll())
    } catch {
      setError('Không thể tải danh sách tiện nghi')
    } finally {
      setIsLoading(false)
    }
  }

  useEffect(() => {
    loadAmenities()
  }, [])

  const handleDelete = async (id: number) => {
    if (!confirm('Bạn có chắc muốn xóa tiện nghi này?')) return
    try {
      await amenityService.delete(id)
      toast('Đã xóa tiện nghi', 'success')
      loadAmenities()
    } catch (err: unknown) {
      const msg =
        (err as { response?: { data?: { message?: string } } })?.response?.data?.message ??
        'Không thể xóa tiện nghi'
      toast(msg, 'error')
    }
  }

  return (
    <div className="container py-8">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold text-left">Quản lý tiện nghi</h1>
        <button
          onClick={() => {
            setEditing(null)
            setShowForm(true)
          }}
          className="btn btn-primary"
        >
          Thêm tiện nghi
        </button>
      </div>

      {isLoading && <div className="text-left">Đang tải...</div>}
      {error && <div className="text-red-600 text-left">{error}</div>}

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
        {amenities.map((amenity) => (
          <div key={amenity.id} className="card">
            <div className="flex items-center gap-3 mb-2">
              {amenity.icon && <span className="text-2xl">{amenity.icon}</span>}
              <h3 className="text-lg font-semibold text-left">{amenity.name}</h3>
            </div>
            <p className="text-gray-600 text-sm mb-2 text-left">{amenity.description}</p>
            <div className="flex justify-between text-sm mb-4">
              <span className="text-left">{amenity.category}</span>
              <span className="text-right">{amenity.roomCount} phòng</span>
            </div>
            <div className="flex gap-2">
              <button
                onClick={() => {
                  setEditing(amenity)
                  setShowForm(true)
                }}
                className="btn btn-secondary flex-1"
              >
                Sửa
              </button>
              <button onClick={() => handleDelete(amenity.id)} className="btn btn-danger flex-1">
                Xóa
              </button>
            </div>
          </div>
        ))}
      </div>

      {showForm && (
        <AmenityForm
          amenity={editing}
          onClose={() => setShowForm(false)}
          onSuccess={() => {
            setShowForm(false)
            toast('Lưu thành công', 'success')
            loadAmenities()
          }}
        />
      )}
    </div>
  )
}
