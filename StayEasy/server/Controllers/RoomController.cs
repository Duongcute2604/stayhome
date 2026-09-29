using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Models.Common;
using server.Models.DTOs;
using server.Services;

namespace server.Controllers
{
    // ============================================================================
    // ROOM CONTROLLER - Xử lý API phòng (route số nhiều /api/rooms cho khớp FE)
    // ============================================================================
    [ApiController]
    [Route("api/rooms")]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        // ============================================================================
        // GET /api/room - Lấy tất cả phòng
        // ============================================================================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var rooms = await _roomService.GetAllAsync();
            return Ok(rooms);
        }

        // ============================================================================
        // GET /api/rooms/search - Tìm kiếm & lọc phòng (mới, trả ApiResponse chuẩn)
        // ============================================================================
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] RoomSearchRequest request)
        {
            var result = await _roomService.SearchAsync(request);
            return Ok(new ApiResponse<PagedResult<RoomDto>>
            {
                Data = result,
                Message = "Success",
                StatusCode = 200
            });
        }

        // ============================================================================
        // GET /api/rooms/{id} - Lấy chi tiết phòng
        // ============================================================================
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var room = await _roomService.GetByIdAsync(id);
            if (room == null)
            {
                return NotFound(new { message = "Không tìm thấy phòng" });
            }
            return Ok(room);
        }

        // ============================================================================
        // POST /api/room - Tạo phòng mới (chỉ Admin)
        // ============================================================================
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateRoomRequest request)
        {
            try
            {
                var room = await _roomService.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = room.Id }, room);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ============================================================================
        // PUT /api/room/{id} - Cập nhật phòng (chỉ Admin)
        // ============================================================================
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateRoomRequest request)
        {
            var room = await _roomService.UpdateAsync(id, request);
            if (room == null)
            {
                return NotFound(new { message = "Không tìm thấy phòng" });
            }
            return Ok(room);
        }

        // ============================================================================
        // DELETE /api/room/{id} - Xóa phòng (chỉ Admin)
        // ============================================================================
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _roomService.DeleteAsync(id);
            if (!result)
            {
                return NotFound(new { message = "Không tìm thấy phòng" });
            }
            return NoContent();
        }

        // ============================================================================
        // GET /api/room/{id}/availability - Kiểm tra phòng trống
        // ============================================================================
        [HttpGet("{id:int}/availability")]
        public async Task<IActionResult> CheckAvailability(int id, [FromQuery] DateTime checkIn, [FromQuery] DateTime checkOut)
        {
            var isAvailable = await _roomService.CheckAvailabilityAsync(id, checkIn, checkOut);
            return Ok(isAvailable);
        }
    }
}
