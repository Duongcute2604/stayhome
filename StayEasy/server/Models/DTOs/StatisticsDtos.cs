namespace server.Models.DTOs
{
    // ============================================================================
    // STATISTICS DTOs - Dữ liệu thống kê cho Dashboard Admin
    // Doanh thu chỉ tính từ booking CHECKED_OUT
    // ============================================================================

    public class RevenuePointDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal Revenue { get; set; }
        public int BookingCount { get; set; }
    }

    public class OccupancyDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public double Rate { get; set; } // % lấp đầy
        public int BookedDays { get; set; }
        public int TotalDays { get; set; }
    }

    public class TopRoomDto
    {
        public int RoomId { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public int BookingCount { get; set; }
        public decimal Revenue { get; set; }
    }

    public class SummaryDto
    {
        public int TotalRooms { get; set; }
        public int TotalBookings { get; set; }
        public int PendingBookings { get; set; }
        public decimal TotalRevenue { get; set; }
        public double OccupancyRate { get; set; }
    }
}
