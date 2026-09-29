using server.Models.DTOs;

namespace server.Services
{
    // ============================================================================
    // STATISTICS SERVICE INTERFACE
    // ============================================================================
    public interface IStatisticsService
    {
        Task<List<RevenuePointDto>> GetRevenueAsync(int year);
        Task<OccupancyDto> GetOccupancyAsync(int year, int month);
        Task<List<TopRoomDto>> GetTopRoomsAsync(int top = 5);
        Task<SummaryDto> GetSummaryAsync();
    }
}
