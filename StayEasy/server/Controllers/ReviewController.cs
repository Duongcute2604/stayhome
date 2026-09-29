using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Models.Common;
using server.Models.DTOs;
using server.Services;
using System.Security.Claims;

namespace server.Controllers
{
    // ============================================================================
    // REVIEW CONTROLLER - API đánh giá phòng
    // GET public, POST cần đăng nhập, DELETE chủ review hoặc Admin
    // ============================================================================
    [ApiController]
    [Route("api/reviews")]
    [Authorize]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        // GET /api/reviews/room/5 - Reviews của phòng (public)
        [HttpGet("room/{roomId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByRoom(int roomId)
        {
            var items = await _reviewService.GetByRoomAsync(roomId);
            return Ok(new ApiResponse<List<ReviewDto>> { Data = items });
        }

        // POST /api/reviews - Tạo review
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReviewRequest request)
        {
            try
            {
                var item = await _reviewService.CreateAsync(GetCurrentUserId(), request);
                return Ok(new ApiResponse<ReviewDto> { Data = item, Message = "Tạo thành công", StatusCode = 201 });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object> { Message = ex.Message, StatusCode = 400 });
            }
        }

        // DELETE /api/reviews/5 - Xóa review (chủ hoặc Admin)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var isAdmin = User.IsInRole("Admin");
                var result = await _reviewService.DeleteAsync(id, GetCurrentUserId(), isAdmin);
                if (!result)
                    return NotFound(new ApiResponse<object> { Message = "Không tìm thấy đánh giá", StatusCode = 404 });
                return NoContent();
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
