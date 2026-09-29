using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Models.DTOs;
using server.Services;
using System.Security.Claims;

namespace server.Controllers
{
    // ============================================================================
    // BOOKING CONTROLLER - Xử lý API đặt phòng
    // ============================================================================
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        // ============================================================================
        // GET /api/booking - Lấy tất cả đặt phòng (Admin/Employee)
        // ============================================================================
        [HttpGet]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> GetAll()
        {
            var bookings = await _bookingService.GetAllAsync();
            return Ok(bookings);
        }

        // ============================================================================
        // GET /api/booking/my - Lấy đặt phòng của user hiện tại
        // ============================================================================
        [HttpGet("my")]
        public async Task<IActionResult> GetMyBookings()
        {
            var userId = GetCurrentUserId();
            var bookings = await _bookingService.GetMyBookingsAsync(userId);
            return Ok(bookings);
        }

        // ============================================================================
        // GET /api/booking/{id} - Lấy chi tiết đặt phòng
        // ============================================================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var booking = await _bookingService.GetByIdAsync(id);
            if (booking == null)
            {
                return NotFound(new { message = "Không tìm thấy đặt phòng" });
            }
            return Ok(booking);
        }

        // ============================================================================
        // POST /api/booking - Tạo đặt phòng mới
        // ============================================================================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBookingRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var booking = await _bookingService.CreateAsync(userId, request);
                return CreatedAtAction(nameof(GetById), new { id = booking.Id }, booking);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ============================================================================
        // PUT /api/booking/{id}/status - Cập nhật trạng thái (Admin/Employee)
        // ============================================================================
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateBookingStatusRequest request)
        {
            var booking = await _bookingService.UpdateStatusAsync(id, request);
            if (booking == null)
            {
                return NotFound(new { message = "Không tìm thấy đặt phòng" });
            }
            return Ok(booking);
        }

        // ============================================================================
        // DELETE /api/booking/{id} - Hủy đặt phòng
        // ============================================================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _bookingService.CancelAsync(id, userId);
                if (!result)
                {
                    return NotFound(new { message = "Không tìm thấy đặt phòng" });
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ============================================================================
        // HELPER: Lấy user ID từ JWT token
        // ============================================================================
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim!);
        }
    }
}
