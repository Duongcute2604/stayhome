import { useState, useEffect } from 'react'
import { locationService } from '../services/locationService'
import LocationForm from '../components/LocationForm'
import { useToast } from '../hooks/useToast'
import type { Location } from '../types/location'

// ============================================================================
// LOCATION MANAGER - Trang quản lý địa điểm (chỉ Admin)
// ============================================================================
// Lưu ý: id chỉ dùng làm key nội bộ, không hiển thị ra UI
// ============================================================================

export default function LocationManager() {
  const { toast } = useToast()
  const [locations, setLocations] = useState<Location[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState('')
  const [showForm, setShowForm] = useState(false)
  const [editing, setEditing] = useState<Location | null>(null)

  const loadLocations = async () => {
    setIsLoading(true)
    setError('')
    try {
      setLocations(await locationService.getAll())
    } catch {
      setError('Không thể tải danh sách địa điểm')
    } finally {
      setIsLoading(false)
    }
  }

  useEffect(() => {
    loadLocations()
  }, [])

  const handleDelete = async (id: number) => {
    if (!confirm('Bạn có chắc muốn xóa địa điểm này?')) return
    try {
      await locationService.delete(id)
      toast('Đã xóa địa điểm', 'success')
      loadLocations()
    } catch (err: unknown) {
      const msg =
        (err as { response?: { data?: { message?: string } } })?.response?.data?.message ??
        'Không thể xóa địa điểm'
      toast(msg, 'error')
    }
  }

  return (
    <div className="container py-8">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold text-left">Quản lý địa điểm</h1>
        <button
          onClick={() => {
            setEditing(null)
            setShowForm(true)
          }}
          className="btn btn-primary"
        >
          Thêm địa điểm
        </button>
      </div>

      {isLoading && <div className="text-left">Đang tải...</div>}
      {error && <div className="text-red-600 text-left">{error}</div>}

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {locations.map((location) => (
          <div key={location.id} className="card">
            {location.imageUrl && (
              <img
                src={location.imageUrl}
                alt={location.name}
                className="w-full h-48 object-cover rounded mb-4"
              />
            )}
            <h3 className="text-lg font-semibold mb-2 text-left">{location.name}</h3>
            <p className="text-gray-600 text-sm mb-2 text-left">{location.address}</p>
            <p className="text-sm mb-4 text-right">{location.roomCount} phòng</p>
            <div className="flex gap-2">
              <button
                onClick={() => {
                  setEditing(location)
                  setShowForm(true)
                }}
                className="btn btn-secondary flex-1"
              >
                Sửa
              </button>
              <button onClick={() => handleDelete(location.id)} className="btn btn-danger flex-1">
                Xóa
              </button>
            </div>
          </div>
        ))}
      </div>

      {showForm && (
        <LocationForm
          location={editing}
          onClose={() => setShowForm(false)}
          onSuccess={() => {
            setShowForm(false)
            toast('Lưu thành công', 'success')
            loadLocations()
          }}
        />
      )}
    </div>
  )
}
