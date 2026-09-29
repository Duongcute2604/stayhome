using server.Models.DTOs;

namespace server.Services
{
    // ============================================================================
    // NOTIFICATION SERVICE INTERFACE
    // ============================================================================
    public interface INotificationService
    {
        Task<List<NotificationDto>> GetMyAsync(int userId);
        Task<int> GetUnreadCountAsync(int userId);
        Task<bool> MarkReadAsync(int id, int userId);
        Task MarkAllReadAsync(int userId);
        Task<bool> DeleteAsync(int id, int userId);
    }
}
