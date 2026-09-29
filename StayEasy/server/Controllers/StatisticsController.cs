using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Models.Common;
using server.Models.DTOs;
using server.Services;

namespace server.Controllers
{
    // ============================================================================
    // STATISTICS CONTROLLER - API thống kê (chỉ Admin/Employee)
    // ============================================================================
    [ApiController]
    [Route("api/statistics")]
    [Authorize(Roles = "Admin,Employee")]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsService _statisticsService;

        public StatisticsController(IStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        // GET /api/statistics/summary - Tổng quan dashboard
        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var data = await _statisticsService.GetSummaryAsync();
            return Ok(new ApiResponse<SummaryDto> { Data = data });
        }

        // GET /api/statistics/revenue?year=2026 - Doanh thu theo tháng
        [HttpGet("revenue")]
        public async Task<IActionResult> GetRevenue([FromQuery] int? year)
        {
            var data = await _statisticsService.GetRevenueAsync(year ?? DateTime.Now.Year);
            return Ok(new ApiResponse<List<RevenuePointDto>> { Data = data });
        }

        // GET /api/statistics/occupancy?year=2026&month=9 - Tỷ lệ lấp đầy
        [HttpGet("occupancy")]
        public async Task<IActionResult> GetOccupancy([FromQuery] int? year, [FromQuery] int? month)
        {
            var now = DateTime.Now;
            var data = await _statisticsService.GetOccupancyAsync(year ?? now.Year, month ?? now.Month);
            return Ok(new ApiResponse<OccupancyDto> { Data = data });
        }

        // GET /api/statistics/top-rooms?top=5 - Top phòng
        [HttpGet("top-rooms")]
        public async Task<IActionResult> GetTopRooms([FromQuery] int? top)
        {
            var data = await _statisticsService.GetTopRoomsAsync(top ?? 5);
            return Ok(new ApiResponse<List<TopRoomDto>> { Data = data });
        }
    }
}
