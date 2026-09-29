// ============================================================================
// PAYMENT TYPES (khớp PaymentDto backend)
// ============================================================================

export interface Payment {
  id: number
  bookingId: number
  amount: number
  status: string // PENDING, PAID, REFUNDED
  method: string
  transactionId?: string
  refundAmount?: number
  paidAt?: string
  createdAt: string
}

export interface PayRequest {
  bookingId: number
  method: string
}
