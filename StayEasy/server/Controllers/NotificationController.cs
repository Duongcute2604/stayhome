using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Models.Common;
using server.Models.DTOs;
using server.Services;
using System.Security.Claims;

namespace server.Controllers
{
    // ============================================================================
    // NOTIFICATION CONTROLLER - API thông báo (cần đăng nhập, đúng chủ sở hữu)
    // ============================================================================
    [ApiController]
    [Route("api/notifications")]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // GET /api/notifications - Thông báo của tôi
        [HttpGet]
        public async Task<IActionResult> GetMy()
        {
            var items = await _notificationService.GetMyAsync(GetCurrentUserId());
            return Ok(new ApiResponse<List<NotificationDto>> { Data = items });
        }

        // GET /api/notifications/unread-count - Số chưa đọc (cho chuông)
        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var count = await _notificationService.GetUnreadCountAsync(GetCurrentUserId());
            return Ok(new ApiResponse<int> { Data = count });
        }

        // PUT /api/notifications/{id}/read - Đánh dấu đã đọc
        [HttpPut("{id:int}/read")]
        public async Task<IActionResult> MarkRead(int id)
        {
            var result = await _notificationService.MarkReadAsync(id, GetCurrentUserId());
            if (!result)
                return NotFound(new ApiResponse<object> { Message = "Không tìm thấy thông báo", StatusCode = 404 });
            return Ok(new ApiResponse<object> { Message = "Đã đánh dấu đã đọc" });
        }

        // PUT /api/notifications/read-all - Đánh dấu tất cả đã đọc
        [HttpPut("read-all")]
        public async Task<IActionResult> MarkAllRead()
        {
            await _notificationService.MarkAllReadAsync(GetCurrentUserId());
            return Ok(new ApiResponse<object> { Message = "Đã đánh dấu tất cả" });
        }

        // DELETE /api/notifications/{id} - Xóa thông báo
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _notificationService.DeleteAsync(id, GetCurrentUserId());
            if (!result)
                return NotFound(new ApiResponse<object> { Message = "Không tìm thấy thông báo", StatusCode = 404 });
            return NoContent();
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        }
    }
}
