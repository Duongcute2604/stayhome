using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Models.Common;
using server.Models.DTOs;
using server.Services;

namespace server.Controllers
{
    // ============================================================================
    // AMENITY CONTROLLER - API tiện nghi
    // GET public, POST/PUT/DELETE chỉ Admin
    // ============================================================================
    [ApiController]
    [Route("api/amenities")]
    public class AmenityController : ControllerBase
    {
        private readonly IAmenityService _amenityService;

        public AmenityController(IAmenityService amenityService)
        {
            _amenityService = amenityService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _amenityService.GetAllAsync();
            return Ok(new ApiResponse<List<AmenityDto>> { Data = items });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _amenityService.GetByIdAsync(id);
            if (item == null)
                return NotFound(new ApiResponse<object> { Message = "Không tìm thấy tiện nghi", StatusCode = 404 });
            return Ok(new ApiResponse<AmenityDto> { Data = item });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateAmenityRequest request)
        {
            try
            {
                var item = await _amenityService.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = item.Id },
                    new ApiResponse<AmenityDto> { Data = item, Message = "Tạo thành công", StatusCode = 201 });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object> { Message = ex.Message, StatusCode = 400 });
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAmenityRequest request)
        {
            try
            {
                var item = await _amenityService.UpdateAsync(id, request);
                if (item == null)
                    return NotFound(new ApiResponse<object> { Message = "Không tìm thấy tiện nghi", StatusCode = 404 });
                return Ok(new ApiResponse<AmenityDto> { Data = item, Message = "Cập nhật thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object> { Message = ex.Message, StatusCode = 400 });
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _amenityService.DeleteAsync(id);
                if (!result)
                    return NotFound(new ApiResponse<object> { Message = "Không tìm thấy tiện nghi", StatusCode = 404 });
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object> { Message = ex.Message, StatusCode = 400 });
            }
        }
    }
}
