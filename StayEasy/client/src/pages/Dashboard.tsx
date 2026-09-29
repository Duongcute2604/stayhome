import { useState, useEffect } from 'react'
import { statisticsService } from '../services/statisticsService'
import { formatVnd } from '../utils/format'
import type { Summary, RevenuePoint, TopRoom } from '../types/statistics'

// ============================================================================
// DASHBOARD PAGE - Thống kê tổng quan (Admin/Employee)
// Biểu đồ cột vẽ bằng CSS thuần (không thêm lib)
// ============================================================================

export default function Dashboard() {
  const [summary, setSummary] = useState<Summary | null>(null)
  const [revenue, setRevenue] = useState<RevenuePoint[]>([])
  const [topRooms, setTopRooms] = useState<TopRoom[]>([])
  const [year, setYear] = useState(new Date().getFullYear())
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState('')

  const loadData = async (y: number) => {
    setIsLoading(true)
    setError('')
    try {
      const [s, r, t] = await Promise.all([
        statisticsService.getSummary(),
        statisticsService.getRevenue(y),
        statisticsService.getTopRooms(),
      ])
      setSummary(s)
      setRevenue(r)
      setTopRooms(t)
    } catch {
      setError('Không thể tải thống kê (cần quyền Admin/Employee)')
    } finally {
      setIsLoading(false)
    }
  }

  useEffect(() => {
    loadData(year)
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [])

  const handleYearChange = (y: number) => {
    setYear(y)
    loadData(y)
  }

  const maxRevenue = Math.max(1, ...revenue.map((p) => p.revenue))

  return (
    <div className="container py-8">
      <h1 className="text-2xl font-bold mb-6 text-left">Dashboard thống kê</h1>

      {isLoading && <div className="text-left">Đang tải...</div>}
      {error && <div className="text-red-600 text-left">{error}</div>}

      {!isLoading && !error && summary && (
        <>
          {/* Thẻ tổng quan */}
          <div className="grid grid-cols-2 lg:grid-cols-4 gap-4 mb-8">
            <div className="card">
              <p className="text-sm text-gray-600 text-left">Tổng số phòng</p>
              <p className="text-2xl font-bold text-right">{summary.totalRooms}</p>
            </div>
            <div className="card">
              <p className="text-sm text-gray-600 text-left">Tổng đặt phòng</p>
              <p className="text-2xl font-bold text-right">{summary.totalBookings}</p>
            </div>
            <div className="card">
              <p className="text-sm text-gray-600 text-left">Chờ xác nhận</p>
              <p className="text-2xl font-bold text-right">{summary.pendingBookings}</p>
            </div>
            <div className="card">
              <p className="text-sm text-gray-600 text-left">Doanh thu (đã trả phòng)</p>
              <p className="text-2xl font-bold text-right number-vn">
                {formatVnd(summary.totalRevenue)}
              </p>
            </div>
          </div>

          {/* Doanh thu theo tháng */}
          <div className="card mb-8">
            <div className="flex justify-between items-center mb-4">
              <h2 className="text-lg font-semibold text-left">
                Doanh thu năm {year} — Lấp đầy tháng này: {summary.occupancyRate}%
              </h2>
              <input
                type="number"
                className="input text-right"
                style={{ width: '100px' }}
                value={year}
                onChange={(e) => handleYearChange(Number(e.target.value))}
              />
            </div>
            <div className="flex items-end gap-1" style={{ height: '180px' }}>
              {revenue.map((p) => (
                <div key={p.month} className="flex-1 flex flex-col items-center justify-end h-full">
                  <span className="text-xs text-gray-500 mb-1">
                    {p.revenue > 0 ? `${(p.revenue / 1000000).toFixed(1)}M` : ''}
                  </span>
                  <div
                    className="w-full bg-blue-600 rounded-t"
                    style={{ height: `${Math.max(2, (p.revenue / maxRevenue) * 140)}px` }}
                    title={`T${p.month}: ${formatVnd(p.revenue)}`}
                  />
                  <span className="text-xs mt-1">T{p.month}</span>
                </div>
              ))}
            </div>
          </div>

          {/* Top phòng */}
          <div className="card">
            <h2 className="text-lg font-semibold mb-4 text-left">Top phòng được đặt nhiều</h2>
            {topRooms.length === 0 ? (
              <p className="text-gray-600 text-left">Chưa có dữ liệu.</p>
            ) : (
              <table className="w-full">
                <thead>
                  <tr className="border-b">
                    <th className="text-left py-2">STT</th>
                    <th className="text-left py-2">Phòng</th>
                    <th className="text-right py-2">Lượt đặt</th>
                    <th className="text-right py-2">Doanh thu</th>
                  </tr>
                </thead>
                <tbody>
                  {topRooms.map((room, index) => (
                    <tr key={room.roomId} className="border-b">
                      <td className="text-left py-2">{index + 1}</td>
                      <td className="text-left py-2">{room.roomName}</td>
                      <td className="text-right py-2">{room.bookingCount}</td>
                      <td className="text-right py-2 number-vn">{formatVnd(room.revenue)}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            )}
          </div>
        </>
      )}
    </div>
  )
}
