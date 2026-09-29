import { useState, useEffect, useCallback } from 'react'
import { Link } from 'react-router-dom'
import { roomService } from '../services/roomService'
import { formatVnd } from '../utils/format'
import SearchFilters from '../components/SearchFilters'
import type { Room, RoomSearchParams } from '../types/room'

// ============================================================================
// ROOMS PAGE - Trang danh sách + tìm kiếm phòng
// ============================================================================
// Quy tắc: tên/mô tả text-left, giá text-right + .number-vn + formatVnd
// ============================================================================

const PAGE_SIZE = 9

export default function Rooms() {
  const [rooms, setRooms] = useState<Room[]>([])
  const [totalPages, setTotalPages] = useState(1)
  const [totalCount, setTotalCount] = useState(0)
  const [filters, setFilters] = useState<RoomSearchParams>({ sortBy: 'newest', pageSize: PAGE_SIZE })
  const [page, setPage] = useState(1)
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState('')

  const loadRooms = useCallback(async (f: RoomSearchParams, p: number) => {
    setIsLoading(true)
    setError('')
    try {
      const result = await roomService.searchRooms({ ...f, page: p, pageSize: PAGE_SIZE })
      setRooms(result.items)
      setTotalPages(result.totalPages)
      setTotalCount(result.totalCount)
    } catch (err) {
      setError('Không thể tải danh sách phòng')
    } finally {
      setIsLoading(false)
    }
  }, [])

  useEffect(() => {
    loadRooms(filters, page)
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [page])

  const handleSearch = () => {
    if (page === 1) {
      loadRooms(filters, 1)
    } else {
      setPage(1) // useEffect sẽ load lại với page mới
    }
  }

  return (
    <div className="container py-8">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold text-left">Danh sách phòng</h1>
        <span className="text-sm text-gray-600 text-right">{totalCount} phòng</span>
      </div>

      <SearchFilters
        filters={filters}
        onChange={setFilters}
        onSearch={handleSearch}
        isLoading={isLoading}
      />

      {isLoading && <div className="text-left">Đang tải...</div>}
      {error && <div className="text-red-600 text-left">{error}</div>}

      {!isLoading && !error && rooms.length === 0 && (
        <p className="text-gray-600 text-left">Không tìm thấy phòng phù hợp.</p>
      )}

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
            <h3 className="text-lg font-semibold mb-2 text-left">{room.name}</h3>
            <p className="text-gray-600 text-sm mb-2 text-left">{room.description}</p>
            <div className="flex justify-between items-center">
              <span className="text-blue-600 font-semibold number-vn text-right">
                {formatVnd(room.pricePerDay)}/đêm
              </span>
              <Link to={`/rooms/${room.id}`} className="btn btn-primary text-sm">
                Xem chi tiết
              </Link>
            </div>
          </div>
        ))}
      </div>

      {totalPages > 1 && (
        <div className="flex justify-center items-center gap-4 mt-8">
          <button
            className="btn btn-secondary"
            disabled={page <= 1 || isLoading}
            onClick={() => setPage((p) => Math.max(1, p - 1))}
          >
            Trang trước
          </button>
          <span className="text-sm text-gray-600">
            Trang {page}/{totalPages}
          </span>
          <button
            className="btn btn-secondary"
            disabled={page >= totalPages || isLoading}
            onClick={() => setPage((p) => Math.min(totalPages, p + 1))}
          >
            Trang sau
          </button>
        </div>
      )}
    </div>
  )
}
