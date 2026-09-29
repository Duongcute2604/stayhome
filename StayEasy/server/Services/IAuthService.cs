using server.Models.DTOs;
using server.Models.Entities;

namespace server.Services
{
    // ============================================================================
    // AUTH SERVICE INTERFACE - Định nghĩa các phương thức xác thực
    // ============================================================================
    public interface IAuthService
    {
        Task<LoginResponse> RegisterAsync(RegisterRequest request);
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<User?> GetUserByIdAsync(int id);
    }
}
