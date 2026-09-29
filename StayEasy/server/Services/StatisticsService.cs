using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Models.DTOs;

namespace server.Services
{
    // ============================================================================
    // STATISTICS SERVICE - Thống kê doanh thu, lấp đầy, top phòng
    // Quy tắc: doanh thu chỉ tính booking CHECKED_OUT
    // ============================================================================
    public class StatisticsService : IStatisticsService
    {
        private readonly StayEasyDbContext _context;

        public StatisticsService(StayEasyDbContext context)
        {
            _context = context;
        }

        // Doanh thu theo tháng trong năm
        public async Task<List<RevenuePointDto>> GetRevenueAsync(int year)
        {
            var points = await _context.Bookings
                .Where(b => b.Status == "CHECKED_OUT" && b.CheckIn.Year == year)
                .GroupBy(b => b.CheckIn.Month)
                .Select(g => new RevenuePointDto
                {
                    Year = year,
                    Month = g.Key,
                    Revenue = g.Sum(b => b.TotalPrice),
                    BookingCount = g.Count()
                })
                .ToListAsync();

            // Đủ 12 tháng (tháng không có thì 0) để vẽ biểu đồ
            var result = Enumerable.Range(1, 12)
                .Select(m => points.FirstOrDefault(p => p.Month == m)
                    ?? new RevenuePointDto { Year = year, Month = m, Revenue = 0, BookingCount = 0 })
                .ToList();

            return result;
        }

        // Tỷ lệ lấp đầy tháng: số ngày-phòng đã đặt / tổng ngày-phòng
        public async Task<OccupancyDto> GetOccupancyAsync(int year, int month)
        {
            var roomCount = await _context.Rooms.CountAsync();
            var monthStart = new DateTime(year, month, 1);
            var monthEnd = monthStart.AddMonths(1);
            var daysInMonth = DateTime.DaysInMonth(year, month);

            var bookings = await _context.Bookings
                .Where(b => b.Status != "CANCELLED" &&
                    b.CheckIn < monthEnd && b.CheckOut > monthStart)
                .Select(b => new { b.CheckIn, b.CheckOut })
                .ToListAsync();

            var bookedDays = bookings.Sum(b =>
            {
                var start = b.CheckIn > monthStart ? b.CheckIn : monthStart;
                var end = b.CheckOut < monthEnd ? b.CheckOut : monthEnd;
                return Math.Max(0, (end - start).Days);
            });

            var totalDays = roomCount * daysInMonth;

            return new OccupancyDto
            {
                Year = year,
                Month = month,
                BookedDays = bookedDays,
                TotalDays = totalDays,
                Rate = totalDays == 0 ? 0 : Math.Round(bookedDays * 100.0 / totalDays, 1)
            };
        }

        // Top phòng được đặt nhiều nhất
        public async Task<List<TopRoomDto>> GetTopRoomsAsync(int top = 5)
        {
            if (top < 1) top = 5;
            if (top > 20) top = 20;

            return await _context.Bookings
                .Where(b => b.Status != "CANCELLED")
                .Include(b => b.Room)
                .GroupBy(b => new { b.RoomId, RoomName = b.Room!.Name })
                .Select(g => new TopRoomDto
                {
                    RoomId = g.Key.RoomId,
                    RoomName = g.Key.RoomName,
                    BookingCount = g.Count(),
                    Revenue = g.Where(b => b.Status == "CHECKED_OUT").Sum(b => b.TotalPrice)
                })
                .OrderByDescending(t => t.BookingCount)
                .Take(top)
                .ToListAsync();
        }

        // Tổng quan dashboard
        public async Task<SummaryDto> GetSummaryAsync()
        {
            var now = DateTime.Now;
            var occupancy = await GetOccupancyAsync(now.Year, now.Month);

            return new SummaryDto
            {
                TotalRooms = await _context.Rooms.CountAsync(),
                TotalBookings = await _context.Bookings.CountAsync(),
                PendingBookings = await _context.Bookings.CountAsync(b => b.Status == "PENDING"),
                TotalRevenue = await _context.Bookings
                    .Where(b => b.Status == "CHECKED_OUT")
                    .SumAsync(b => (decimal?)b.TotalPrice) ?? 0,
                OccupancyRate = occupancy.Rate
            };
        }
    }
}
