import type { FormEvent } from 'react'
import type { RoomSearchParams } from '../types/room'

// ============================================================================
// SEARCH FILTERS - Form tìm kiếm & lọc phòng
// ============================================================================
// Quy tắc: label/input căn trái (text-left), giá trị số căn phải khi hiển thị
// TODO: thêm dropdown địa điểm + checkbox tiện nghi khi có Location/Amenity API
// ============================================================================

interface Props {
  filters: RoomSearchParams
  onChange: (filters: RoomSearchParams) => void
  onSearch: () => void
  isLoading: boolean
}

export default function SearchFilters({ filters, onChange, onSearch, isLoading }: Props) {
  const set = (patch: Partial<RoomSearchParams>) => {
    onChange({ ...filters, ...patch })
  }

  const handleSubmit = (e: FormEvent) => {
    e.preventDefault()
    onSearch()
  }

  return (
    <form onSubmit={handleSubmit} className="card mb-6">
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
        <div>
          <label className="block text-sm font-medium mb-1 text-left">Từ khóa</label>
          <input
            type="text"
            className="input"
            placeholder="Tên phòng..."
            value={filters.search ?? ''}
            onChange={(e) => set({ search: e.target.value })}
          />
        </div>

        <div>
          <label className="block text-sm font-medium mb-1 text-left">Giá tối thiểu (₫)</label>
          <input
            type="number"
            className="input text-right"
            min={0}
            value={filters.minPrice ?? ''}
            onChange={(e) => set({ minPrice: e.target.value ? Number(e.target.value) : undefined })}
          />
        </div>

        <div>
          <label className="block text-sm font-medium mb-1 text-left">Giá tối đa (₫)</label>
          <input
            type="number"
            className="input text-right"
            min={0}
            value={filters.maxPrice ?? ''}
            onChange={(e) => set({ maxPrice: e.target.value ? Number(e.target.value) : undefined })}
          />
        </div>

        <div>
          <label className="block text-sm font-medium mb-1 text-left">Sức chứa (khách)</label>
          <input
            type="number"
            className="input text-right"
            min={1}
            value={filters.capacity ?? ''}
            onChange={(e) => set({ capacity: e.target.value ? Number(e.target.value) : undefined })}
          />
        </div>

        <div>
          <label className="block text-sm font-medium mb-1 text-left">Nhận phòng</label>
          <input
            type="datetime-local"
            className="input"
            value={filters.checkIn ?? ''}
            onChange={(e) => set({ checkIn: e.target.value || undefined })}
          />
        </div>

        <div>
          <label className="block text-sm font-medium mb-1 text-left">Trả phòng</label>
          <input
            type="datetime-local"
            className="input"
            value={filters.checkOut ?? ''}
            onChange={(e) => set({ checkOut: e.target.value || undefined })}
          />
        </div>

        <div>
          <label className="block text-sm font-medium mb-1 text-left">Sắp xếp</label>
          <select
            className="input"
            value={filters.sortBy ?? 'newest'}
            onChange={(e) => set({ sortBy: e.target.value })}
          >
            <option value="newest">Mới nhất</option>
            <option value="price_asc">Giá tăng dần</option>
            <option value="price_desc">Giá giảm dần</option>
          </select>
        </div>

        <div className="flex items-end">
          <button type="submit" className="btn btn-primary w-full" disabled={isLoading}>
            {isLoading ? 'Đang tìm...' : 'Tìm kiếm'}
          </button>
        </div>
      </div>
    </form>
  )
}
