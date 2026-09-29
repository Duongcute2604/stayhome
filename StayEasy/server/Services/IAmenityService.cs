using server.Models.DTOs;

namespace server.Services
{
    // ============================================================================
    // AMENITY SERVICE INTERFACE
    // ============================================================================
    public interface IAmenityService
    {
        Task<List<AmenityDto>> GetAllAsync();
        Task<AmenityDto?> GetByIdAsync(int id);
        Task<AmenityDto> CreateAsync(CreateAmenityRequest request);
        Task<AmenityDto?> UpdateAsync(int id, UpdateAmenityRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
