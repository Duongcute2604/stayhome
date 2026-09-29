using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Models.DTOs;
using server.Models.Entities;

namespace server.Services
{
    // ============================================================================
    // REVIEW SERVICE - Logic đánh giá phòng
    // Quy tắc: chỉ review khi đã CHECKED_OUT, 1 user 1 review/phòng
    // ============================================================================
    public class ReviewService : IReviewService
    {
        private readonly StayEasyDbContext _context;

        public ReviewService(StayEasyDbContext context)
        {
            _context = context;
        }

        public async Task<List<ReviewDto>> GetByRoomAsync(int roomId)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.RoomId == roomId)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new ReviewDto
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    UserName = r.User!.FullName,
                    RoomId = r.RoomId,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<ReviewDto> CreateAsync(int userId, CreateReviewRequest request)
        {
            // Phòng phải tồn tại
            if (!await _context.Rooms.AnyAsync(r => r.Id == request.RoomId))
                throw new Exception("Không tìm thấy phòng");

            // Rating 1-5 (DataAnnotations cũng check ở Controller)
            if (request.Rating < 1 || request.Rating > 5)
                throw new Exception("Đánh giá phải từ 1 đến 5 sao");

            // Chỉ review khi đã trả phòng (CHECKED_OUT)
            var stayed = await _context.Bookings.AnyAsync(b =>
                b.UserId == userId &&
                b.RoomId == request.RoomId &&
                b.Status == "CHECKED_OUT");
            if (!stayed)
                throw new Exception("Bạn chỉ có thể đánh giá sau khi đã trả phòng");

            // 1 user 1 review/phòng (DB cũng có unique index chống song song)
            if (await _context.Reviews.AnyAsync(r => r.UserId == userId && r.RoomId == request.RoomId))
                throw new Exception("Bạn đã đánh giá phòng này rồi");

            var review = new Review
            {
                UserId = userId,
                RoomId = request.RoomId,
                Rating = request.Rating,
                Comment = request.Comment
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            var user = await _context.Users.FindAsync(userId);
            return new ReviewDto
            {
                Id = review.Id,
                UserId = review.UserId,
                UserName = user?.FullName,
                RoomId = review.RoomId,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt
            };
        }

        public async Task<bool> DeleteAsync(int id, int userId, bool isAdmin)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null) return false;

            // Chỉ chủ review hoặc Admin được xóa
            if (!isAdmin && review.UserId != userId)
                throw new Exception("Bạn không có quyền xóa đánh giá này");

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
