using server.Models.DTOs;

namespace server.Services
{
    // ============================================================================
    // PAYMENT SERVICE INTERFACE (MOCK gateway - luôn thành công)
    // ============================================================================
    public interface IPaymentService
    {
        Task<PaymentDto> PayAsync(int userId, PayRequest request);
        Task<List<PaymentDto>> GetMyAsync(int userId);
        Task<PaymentDto?> GetByBookingAsync(int bookingId, int userId);
        Task<PaymentDto?> RefundAsync(int id);
    }
}
