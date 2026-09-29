// ============================================================================
// DATE UTILS - Hàm định dạng ngày tháng (theo skill stayeasy-shared-types)
// ============================================================================

/**
 * Định dạng ngày tháng sang định dạng Việt Nam (vd: 25/12/2026)
 */
export function formatDate(date: string | Date): string {
  return new Date(date).toLocaleDateString('vi-VN')
}

/**
 * Định dạng ngày tháng có giờ (vd: 25/12/2026 14:30)
 */
export function formatDateTime(date: string | Date): string {
  return new Date(date).toLocaleString('vi-VN')
}

/**
 * Tính số ngày giữa 2 ngày
 */
export function daysBetween(startDate: string | Date, endDate: string | Date): number {
  const diff = Math.abs(new Date(endDate).getTime() - new Date(startDate).getTime())
  return Math.ceil(diff / (1000 * 60 * 60 * 24))
}
