namespace server.Models.Common
{
    // ============================================================================
    // API RESPONSE - Chuẩn response cho mọi endpoint (theo skill stayeasy-dotnet-backend)
    // ============================================================================
    // Tại sao cần: Frontend xử lý thống nhất, không phải đoán shape mỗi controller
    // ============================================================================
    public class ApiResponse<T>
    {
        public T? Data { get; set; }
        public string Message { get; set; } = "Success";
        public int StatusCode { get; set; } = 200;
    }
}
