using server.Models.DTOs;

namespace server.Services
{
    // ============================================================================
    // LOCATION SERVICE INTERFACE
    // ============================================================================
    public interface ILocationService
    {
        Task<List<LocationDto>> GetAllAsync();
        Task<LocationDto?> GetByIdAsync(int id);
        Task<LocationDto> CreateAsync(CreateLocationRequest request);
        Task<LocationDto?> UpdateAsync(int id, UpdateLocationRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
