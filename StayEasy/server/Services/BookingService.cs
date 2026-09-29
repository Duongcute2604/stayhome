using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Models.DTOs;
using server.Models.Entities;

namespace server.Services
{
    // ============================================================================
    // BOOKING SERVICE - Xử lý logic quản lý đặt phòng
    // ============================================================================
    public class BookingService : IBookingService
    {
        private readonly StayEasyDbContext _context;

        public BookingService(StayEasyDbContext context)
        {
            _context = context;
        }

        // ============================================================================
        // LẤY TẤT CẢ ĐẶT PHÒNG
        // ============================================================================
        public async Task<List<BookingDto>> GetAllAsync()
        {
            var bookings = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Room)
                .ToListAsync();

            return bookings.Select(MapToDto).ToList();
        }

        // ============================================================================
        // LẤY ĐẶT PHÒNG CỦA USER
        // ============================================================================
        public async Task<List<BookingDto>> GetMyBookingsAsync(int userId)
        {
            var bookings = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Room)
                .Where(b => b.UserId == userId)
                .ToListAsync();

            return bookings.Select(MapToDto).ToList();
        }

        // ============================================================================
        // LẤY CHI TIẾT ĐẶT PHÒNG
        // ============================================================================
        public async Task<BookingDto?> GetByIdAsync(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Room)
                .FirstOrDefaultAsync(b => b.Id == id);

            return booking == null ? null : MapToDto(booking);
        }

        // ============================================================================
        // TẠO ĐẶT PHÒNG MỚI
        // ============================================================================
        public async Task<BookingDto> CreateAsync(int userId, CreateBookingRequest request)
        {
            // Kiểm tra phòng tồn tại
            var room = await _context.Rooms.FindAsync(request.RoomId);
            if (room == null)
            {
                throw new Exception("Không tìm thấy phòng");
            }

            // Kiểm tra thời gian đặt trước 2 giờ
            if (request.CheckIn < DateTime.Now.AddHours(2))
            {
                throw new Exception("Phải đặt phòng trước ít nhất 2 giờ");
            }

            // Kiểm tra thời gian hợp lệ
            if (request.CheckOut <= request.CheckIn)
            {
                throw new Exception("Thời gian trả phòng phải sau thời gian nhận phòng");
            }

            // Kiểm tra số giờ tối thiểu (nếu đặt theo giờ)
            if (request.BookingType == "HOURLY")
            {
                var hours = (request.CheckOut - request.CheckIn).TotalHours;
                if (hours < 3)
                {
                    throw new Exception("Đặt phòng theo giờ tối thiểu 3 giờ");
                }
            }

            // Kiểm tra phòng trống
            var hasConflict = await _context.Bookings.AnyAsync(b =>
                b.RoomId == request.RoomId &&
                b.Status != "CANCELLED" &&
                b.CheckIn < request.CheckOut &&
                b.CheckOut > request.CheckIn);

            if (hasConflict)
            {
                throw new Exception("Phòng đã được đặt trong thời gian này");
            }

            // Tính tổng tiền
            decimal totalPrice;
            if (request.BookingType == "HOURLY")
            {
                var hours = Math.Max(3, (decimal)(request.CheckOut - request.CheckIn).TotalHours);
                totalPrice = hours * room.PricePerHour;
            }
            else
            {
                var days = (request.CheckOut - request.CheckIn).Days;
                totalPrice = days * room.PricePerDay;
            }

            // Tạo booking
            var booking = new Booking
            {
                UserId = userId,
                RoomId = request.RoomId,
                BookingType = request.BookingType,
                CheckIn = request.CheckIn,
                CheckOut = request.CheckOut,
                TotalPrice = totalPrice,
                Status = "PENDING"
            };

            _context.Bookings.Add(booking);

            // Thêm vào lịch sử trạng thái
            _context.BookingStatusHistories.Add(new BookingStatusHistory
            {
                BookingId = booking.Id,
                Status = "PENDING",
                Note = "Tạo đặt phòng mới"
            });

            await _context.SaveChangesAsync();

            return await GetByIdAsync(booking.Id) ?? throw new Exception("Không thể tạo đặt phòng");
        }

        // ============================================================================
        // CẬP NHẬT TRẠNG THÁI
        // ============================================================================
        public async Task<BookingDto?> UpdateStatusAsync(int id, UpdateBookingStatusRequest request)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return null;

            booking.Status = request.Status;

            // Thêm vào lịch sử trạng thái
            _context.BookingStatusHistories.Add(new BookingStatusHistory
            {
                BookingId = booking.Id,
                Status = request.Status,
                Note = "Cập nhật trạng thái"
            });

            await _context.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        // ============================================================================
        // HỦY ĐẶT PHÒNG
        // ============================================================================
        public async Task<bool> CancelAsync(int id, int userId)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return false;

            // Kiểm tra quyền hủy
            if (booking.UserId != userId)
            {
                throw new Exception("Bạn không có quyền hủy đặt phòng này");
            }

            // Chỉ cho hủy nếu chưa check-in
            if (booking.Status == "CHECKED_IN" || booking.Status == "CHECKED_OUT")
            {
                throw new Exception("Không thể hủy đặt phòng đã check-in");
            }

            booking.Status = "CANCELLED";

            // Thêm vào lịch sử trạng thái
            _context.BookingStatusHistories.Add(new BookingStatusHistory
            {
                BookingId = booking.Id,
                Status = "CANCELLED",
                Note = "Hủy đặt phòng"
            });

            await _context.SaveChangesAsync();
            return true;
        }

        // ============================================================================
        // HELPER: Map entity sang DTO
        // ============================================================================
        private static BookingDto MapToDto(Booking booking)
        {
            return new BookingDto
            {
                Id = booking.Id,
                UserId = booking.UserId,
                UserName = booking.User?.FullName,
                RoomId = booking.RoomId,
                RoomName = booking.Room?.Name,
                BookingType = booking.BookingType,
                CheckIn = booking.CheckIn,
                CheckOut = booking.CheckOut,
                TotalPrice = booking.TotalPrice,
                Status = booking.Status,
                CreatedAt = booking.CreatedAt
            };
        }
    }
}
