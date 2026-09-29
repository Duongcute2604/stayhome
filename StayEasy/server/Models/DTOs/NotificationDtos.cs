namespace server.Models.DTOs
{
    // ============================================================================
    // NOTIFICATION DTOs
    // ============================================================================

    public class NotificationDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Message { get; set; }
        public string Type { get; set; } = "INFO";
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
