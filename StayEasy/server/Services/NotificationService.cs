using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Models.DTOs;

namespace server.Services
{
    // ============================================================================
    // NOTIFICATION SERVICE - Quản lý thông báo của user
    // Thông báo được hệ thống tự tạo (đặt phòng, xác nhận, hủy, chào mừng)
    // ============================================================================
    public class NotificationService : INotificationService
    {
        private readonly StayEasyDbContext _context;

        public NotificationService(StayEasyDbContext context)
        {
            _context = context;
        }

        public async Task<List<NotificationDto>> GetMyAsync(int userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new NotificationDto
                {
                    Id = n.Id,
                    Title = n.Title,
                    Message = n.Message,
                    Type = n.Type,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<int> GetUnreadCountAsync(int userId)
        {
            return await _context.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead);
        }

        public async Task<bool> MarkReadAsync(int id, int userId)
        {
            var item = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);
            if (item == null) return false;

            item.IsRead = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task MarkAllReadAsync(int userId)
        {
            var items = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var item in items)
                item.IsRead = true;

            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var item = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);
            if (item == null) return false;

            _context.Notifications.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
