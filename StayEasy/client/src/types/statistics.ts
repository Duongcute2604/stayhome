// ============================================================================
// STATISTICS TYPES (khớp StatisticsDtos backend)
// ============================================================================

export interface RevenuePoint {
  year: number
  month: number
  revenue: number
  bookingCount: number
}

export interface Occupancy {
  year: number
  month: number
  rate: number
  bookedDays: number
  totalDays: number
}

export interface TopRoom {
  roomId: number
  roomName: string
  bookingCount: number
  revenue: number
}

export interface Summary {
  totalRooms: number
  totalBookings: number
  pendingBookings: number
  totalRevenue: number
  occupancyRate: number
}
