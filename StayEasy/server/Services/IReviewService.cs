using server.Models.DTOs;

namespace server.Services
{
    // ============================================================================
    // REVIEW SERVICE INTERFACE
    // ============================================================================
    public interface IReviewService
    {
        Task<List<ReviewDto>> GetByRoomAsync(int roomId);
        Task<ReviewDto> CreateAsync(int userId, CreateReviewRequest request);
        Task<bool> DeleteAsync(int id, int userId, bool isAdmin);
    }
}
