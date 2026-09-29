using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Models.Common;
using server.Models.DTOs;
using server.Services;
using System.Security.Claims;

namespace server.Controllers
{
    // ============================================================================
    // PAYMENT CONTROLLER - API thanh toán MOCK (cần đăng nhập)
    // ============================================================================
    [ApiController]
    [Route("api/payments")]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        // POST /api/payments/pay - Thanh toán booking
        [HttpPost("pay")]
        public async Task<IActionResult> Pay([FromBody] PayRequest request)
        {
            try
            {
                var item = await _paymentService.PayAsync(GetCurrentUserId(), request);
                return Ok(new ApiResponse<PaymentDto> { Data = item, Message = "Thanh toán thành công", StatusCode = 201 });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object> { Message = ex.Message, StatusCode = 400 });
            }
        }

        // GET /api/payments/my - Lịch sử thanh toán của tôi
        [HttpGet("my")]
        public async Task<IActionResult> GetMy()
        {
            var items = await _paymentService.GetMyAsync(GetCurrentUserId());
            return Ok(new ApiResponse<List<PaymentDto>> { Data = items });
        }

        // GET /api/payments/booking/{bookingId} - Payment của booking
        [HttpGet("booking/{bookingId:int}")]
        public async Task<IActionResult> GetByBooking(int bookingId)
        {
            var item = await _paymentService.GetByBookingAsync(bookingId, GetCurrentUserId());
            if (item == null)
                return NotFound(new ApiResponse<object> { Message = "Chưa thanh toán", StatusCode = 404 });
            return Ok(new ApiResponse<PaymentDto> { Data = item });
        }

        // POST /api/payments/{id}/refund - Hoàn tiền thủ công (Admin)
        [HttpPost("{id:int}/refund")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Refund(int id)
        {
            try
            {
                var item = await _paymentService.RefundAsync(id);
                if (item == null)
                    return NotFound(new ApiResponse<object> { Message = "Không tìm thấy payment", StatusCode = 404 });
                return Ok(new ApiResponse<PaymentDto> { Data = item, Message = "Hoàn tiền thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object> { Message = ex.Message, StatusCode = 400 });
            }
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        }
    }
}
