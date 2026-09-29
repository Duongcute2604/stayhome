// ============================================================================
// FORMAT UTILS - Hàm định dạng dùng chung (theo skill stayeasy-shared-types)
// ============================================================================
// Quy tắc bắt buộc: formatVnd dùng Intl vi-VN + ' ₫', căn lề qua class CSS
// ============================================================================

/**
 * Định dạng số tiền sang VND
 * @param value - Số tiền cần định dạng
 * @returns Chuỗi định dạng VND (vd: 1.500.000 ₫)
 */
export const formatVnd = (value: number): string => {
  return new Intl.NumberFormat('vi-VN').format(value) + ' ₫'
}

/**
 * Định dạng số tiền rút gọn (vd: 1.5M)
 */
export function formatVndShort(value: number): string {
  if (value >= 1_000_000_000) return `${(value / 1_000_000_000).toFixed(1)}B`
  if (value >= 1_000_000) return `${(value / 1_000_000).toFixed(1)}M`
  if (value >= 1_000) return `${(value / 1_000).toFixed(1)}K`
  return value.toString()
}
