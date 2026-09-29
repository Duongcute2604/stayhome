using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Models.DTOs;
using server.Models.Entities;

namespace server.Services
{
    // ============================================================================
    // PAYMENT SERVICE - Thanh toán MOCK (giả lập cổng thanh toán, luôn thành công)
    // ============================================================================
    public class PaymentService : IPaymentService
    {
        private readonly StayEasyDbContext _context;

        public PaymentService(StayEasyDbContext context)
        {
            _context = context;
        }

        // Thanh toán cho booking (mock: tạo payment PAID ngay)
        public async Task<PaymentDto> PayAsync(int userId, PayRequest request)
        {
            var booking = await _context.Bookings.FindAsync(request.BookingId);
            if (booking == null)
                throw new Exception("Không tìm thấy đặt phòng");

            if (booking.UserId != userId)
                throw new Exception("Bạn không có quyền thanh toán đặt phòng này");

            if (booking.Status == "CANCELLED" || booking.Status == "CHECKED_OUT")
                throw new Exception("Không thể thanh toán đặt phòng đã hủy hoặc đã trả");

            if (await _context.Payments.AnyAsync(p => p.BookingId == request.BookingId && p.Status == "PAID"))
                throw new Exception("Đặt phòng này đã được thanh toán");

            var payment = new Payment
            {
                BookingId = booking.Id,
                UserId = userId,
                Amount = booking.TotalPrice,
                Status = "PAID",
                Method = request.Method,
                TransactionId = "MOCK-" + Guid.NewGuid().ToString("N")[..12].ToUpper(),
                PaidAt = DateTime.UtcNow
            };

            _context.Payments.Add(payment);

            _context.Notifications.Add(new Notification
            {
                UserId = userId,
                Title = "Thanh toán thành công",
                Message = $"Bạn đã thanh toán {booking.TotalPrice:N0}đ cho đặt phòng #{booking.Id}",
                Type = "INFO"
            });

            await _context.SaveChangesAsync();

            return MapToDto(payment);
        }

        public async Task<List<PaymentDto>> GetMyAsync(int userId)
        {
            return await _context.Payments
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new PaymentDto
                {
                    Id = p.Id,
                    BookingId = p.BookingId,
                    Amount = p.Amount,
                    Status = p.Status,
                    Method = p.Method,
                    TransactionId = p.TransactionId,
                    RefundAmount = p.RefundAmount,
                    PaidAt = p.PaidAt,
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<PaymentDto?> GetByBookingAsync(int bookingId, int userId)
        {
            var payment = await _context.Payments
                .Where(p => p.BookingId == bookingId && p.UserId == userId && p.Status == "PAID")
                .OrderByDescending(p => p.CreatedAt)
                .FirstOrDefaultAsync();

            return payment == null ? null : MapToDto(payment);
        }

        // Hoàn tiền thủ công (Admin): chỉ hoàn payment PAID, hoàn 100%
        public async Task<PaymentDto?> RefundAsync(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null) return null;

            if (payment.Status != "PAID")
                throw new Exception("Chỉ hoàn tiền được payment đã thanh toán");

            payment.Status = "REFUNDED";
            payment.RefundAmount = payment.Amount;

            _context.Notifications.Add(new Notification
            {
                UserId = payment.UserId,
                Title = "Đã hoàn tiền",
                Message = $"Đã hoàn {payment.Amount:N0}đ cho đặt phòng #{payment.BookingId}",
                Type = "INFO"
            });

            await _context.SaveChangesAsync();
            return MapToDto(payment);
        }

        // Tính tiền hoàn khi HỦY booking: trước 24h check-in hoàn 100%, còn lại 50%
        public static decimal CalcRefund(Booking booking)
        {
            var hoursLeft = (booking.CheckIn - DateTime.Now).TotalHours;
            return hoursLeft >= 24 ? booking.TotalPrice : Math.Round(booking.TotalPrice / 2, 0);
        }

        private static PaymentDto MapToDto(Payment p)
        {
            return new PaymentDto
            {
                Id = p.Id,
                BookingId = p.BookingId,
                Amount = p.Amount,
                Status = p.Status,
                Method = p.Method,
                TransactionId = p.TransactionId,
                RefundAmount = p.RefundAmount,
                PaidAt = p.PaidAt,
                CreatedAt = p.CreatedAt
            };
        }
    }
}
