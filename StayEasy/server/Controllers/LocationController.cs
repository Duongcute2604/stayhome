using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Models.Common;
using server.Models.DTOs;
using server.Services;

namespace server.Controllers
{
    // ============================================================================
    // LOCATION CONTROLLER - API địa điểm
    // GET public, POST/PUT/DELETE chỉ Admin
    // ============================================================================
    [ApiController]
    [Route("api/locations")]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService _locationService;

        public LocationController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _locationService.GetAllAsync();
            return Ok(new ApiResponse<List<LocationDto>> { Data = items });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _locationService.GetByIdAsync(id);
            if (item == null)
                return NotFound(new ApiResponse<object> { Message = "Không tìm thấy địa điểm", StatusCode = 404 });
            return Ok(new ApiResponse<LocationDto> { Data = item });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateLocationRequest request)
        {
            try
            {
                var item = await _locationService.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = item.Id },
                    new ApiResponse<LocationDto> { Data = item, Message = "Tạo thành công", StatusCode = 201 });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object> { Message = ex.Message, StatusCode = 400 });
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateLocationRequest request)
        {
            try
            {
                var item = await _locationService.UpdateAsync(id, request);
                if (item == null)
                    return NotFound(new ApiResponse<object> { Message = "Không tìm thấy địa điểm", StatusCode = 404 });
                return Ok(new ApiResponse<LocationDto> { Data = item, Message = "Cập nhật thành công" });
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
                var result = await _locationService.DeleteAsync(id);
                if (!result)
                    return NotFound(new ApiResponse<object> { Message = "Không tìm thấy địa điểm", StatusCode = 404 });
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object> { Message = ex.Message, StatusCode = 400 });
            }
        }
    }
}
