# Module 4: Location & Amenity Management — Kế hoạch chi tiết

## Quyết định đã chốt

| Quyết định | Lựa chọn |
|------------|----------|
| Location fields | Đầy đủ: tên, địa chỉ, mô tả, hình ảnh, số phòng, rating TB, giá TB, tiện nghi phổ biến |
| Amenity fields | Đầy đủ: tên, mô tả, icon/emoji, danh mục, số phòng có |
| Phân quyền | Chỉ Admin quản lý, còn lại chỉ xem |
| Xóa Location | Không cho xóa nếu còn phòng |
| Xóa Amenity | Không cho xóa nếu đang được gán |
| Tên trùng | Không cho phép trùng tên |

---

## Quy tắc bắt buộc (từ Technical Overview)

### 1. Ẩn hoàn toàn ID/UI
- **Backend**: Không trả `id` trong list endpoints
- **Frontend**: Tự tính STT = `index + 1 + page * size`

### 2. Căn lề (Alignment)
- Text → `text-left`
- Number (giá tiền, số phòng) → `text-right`

### 3. Format VND
- `500000` → `"500.000 ₫"`
- Dùng `Intl.NumberFormat('vi-VN')`

### 4. UI Components
- `.number-vn` cho số tiền (căn phải)
- `.text-left` cho tên, địa chỉ

---

## 1. Database Changes

### 1.1. Bảng `locations` (cập nhật)
```sql
-- Thêm cột mới
ALTER TABLE locations ADD COLUMN image_url VARCHAR(500);
ALTER TABLE locations ADD COLUMN room_count INT DEFAULT 0;
ALTER TABLE locations ADD COLUMN avg_rating DECIMAL(3,2) DEFAULT 0;
ALTER TABLE locations ADD COLUMN avg_price DECIMAL(18,2) DEFAULT 0;
```

### 1.2. Bảng `amenities` (cập nhật)
```sql
-- Thêm cột mới
ALTER TABLE amenities ADD COLUMN icon VARCHAR(50);
ALTER TABLE amenities ADD COLUMN category VARCHAR(50);
ALTER TABLE amenities ADD COLUMN room_count INT DEFAULT 0;
```

### 1.3. Bảng `location_images` (mới)
```sql
CREATE TABLE IF NOT EXISTS location_images (
    id INT AUTO_INCREMENT PRIMARY KEY,
    location_id INT NOT NULL,
    image_url VARCHAR(500) NOT NULL,
    FOREIGN KEY (location_id) REFERENCES locations(id) ON DELETE CASCADE
) ENGINE=InnoDB;
```

---

## 2. Backend Implementation

### 2.1. Entities (cập nhật)

**Location.cs** (cập nhật):
```csharp
public class Location
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public int RoomCount { get; set; }
    public decimal AvgRating { get; set; }
    public decimal AvgPrice { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public ICollection<LocationImage> LocationImages { get; set; } = new List<LocationImage>();
    public ICollection<Room> Rooms { get; set; } = new List<Room>();
}
```

**Amenity.cs** (cập nhật):
```csharp
public class Amenity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public string? Category { get; set; }
    public int RoomCount { get; set; }
    
    public ICollection<RoomAmenity> RoomAmenities { get; set; } = new List<RoomAmenity>();
}
```

**LocationImage.cs** (mới):
```csharp
public class LocationImage
{
    public int Id { get; set; }
    public int LocationId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public Location? Location { get; set; }
}
```

### 2.2. DTOs

**LocationDtos.cs**:
```csharp
// List DTO (không có id)
public class LocationListItemDto
{
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public int RoomCount { get; set; }
    public decimal AvgRating { get; set; }
    public decimal AvgPrice { get; set; }
    public List<string> Images { get; set; } = new();
    public List<AmenityDto> TopAmenities { get; set; } = new();
}

// Detail DTO (có id)
public class LocationDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public int RoomCount { get; set; }
    public decimal AvgRating { get; set; }
    public decimal AvgPrice { get; set; }
    public List<string> Images { get; set; } = new();
    public List<AmenityDto> TopAmenities { get; set; } = new();
}

public class CreateLocationRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public List<string>? Images { get; set; }
}

public class UpdateLocationRequest
{
    public string? Name { get; set; }
    public string? Address { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public List<string>? Images { get; set; }
}
```

**AmenityDtos.cs**:
```csharp
// List DTO (không có id)
public class AmenityListItemDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public string? Category { get; set; }
    public int RoomCount { get; set; }
}

// Detail DTO (có id)
public class AmenityDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public string? Category { get; set; }
    public int RoomCount { get; set; }
}

public class CreateAmenityRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public string? Category { get; set; }
}

public class UpdateAmenityRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public string? Category { get; set; }
}
```

### 2.3. Services

**ILocationService.cs**:
```csharp
public interface ILocationService
{
    Task<List<LocationListItemDto>> GetAllAsync();
    Task<LocationDetailDto?> GetByIdAsync(int id);
    Task<LocationDetailDto> CreateAsync(CreateLocationRequest request);
    Task<LocationDetailDto?> UpdateAsync(int id, UpdateLocationRequest request);
    Task<bool> DeleteAsync(int id);
}
```

**LocationService.cs**:
```csharp
public class LocationService : ILocationService
{
    private readonly StayEasyDbContext _context;

    public LocationService(StayEasyDbContext context)
    {
        _context = context;
    }

    // List - không trả id
    public async Task<List<LocationListItemDto>> GetAllAsync()
    {
        var locations = await _context.Locations
            .Include(l => l.LocationImages)
            .Include(l => l.Rooms)
            .ToListAsync();

        return locations.Select(MapToListItemDto).ToList();
    }

    // Detail - có id
    public async Task<LocationDetailDto?> GetByIdAsync(int id)
    {
        var location = await _context.Locations
            .Include(l => l.LocationImages)
            .Include(l => l.Rooms)
            .ThenInclude(r => r.RoomAmenities)
            .ThenInclude(ra => ra.Amenity)
            .FirstOrDefaultAsync(l => l.Id == id);

        return location == null ? null : MapToDetailDto(location);
    }

    public async Task<LocationDetailDto> CreateAsync(CreateLocationRequest request)
    {
        // Kiểm tra tên trùng
        if (await _context.Locations.AnyAsync(l => l.Name == request.Name))
        {
            throw new Exception("Tên địa điểm đã tồn tại");
        }

        var location = new Location
        {
            Name = request.Name,
            Address = request.Address,
            Description = request.Description,
            ImageUrl = request.ImageUrl
        };

        if (request.Images != null)
        {
            location.LocationImages = request.Images
                .Select(url => new LocationImage { ImageUrl = url })
                .ToList();
        }

        _context.Locations.Add(location);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(location.Id) ?? throw new Exception("Không thể tạo location");
    }

    public async Task<LocationDetailDto?> UpdateAsync(int id, UpdateLocationRequest request)
    {
        var location = await _context.Locations.FindAsync(id);
        if (location == null) return null;

        // Kiểm tra tên trùng (nếu đổi tên)
        if (request.Name != null && request.Name != location.Name)
        {
            if (await _context.Locations.AnyAsync(l => l.Name == request.Name))
            {
                throw new Exception("Tên địa điểm đã tồn tại");
            }
            location.Name = request.Name;
        }

        if (request.Address != null) location.Address = request.Address;
        if (request.Description != null) location.Description = request.Description;
        if (request.ImageUrl != null) location.ImageUrl = request.ImageUrl;

        // Cập nhật hình ảnh
        if (request.Images != null)
        {
            _context.LocationImages.RemoveRange(location.LocationImages);
            location.LocationImages = request.Images
                .Select(url => new LocationImage { ImageUrl = url })
                .ToList();
        }

        await _context.SaveChangesAsync();
        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var location = await _context.Locations
            .Include(l => l.Rooms)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (location == null) return false;

        // Không cho xóa nếu còn phòng
        if (location.Rooms.Any())
        {
            throw new Exception("Không thể xóa địa điểm còn phòng");
        }

        _context.Locations.Remove(location);
        await _context.SaveChangesAsync();
        return true;
    }

    // Map to List DTO (không có id)
    private static LocationListItemDto MapToListItemDto(Location location)
    {
        var roomCount = location.Rooms.Count;
        var avgRating = location.Rooms
            .SelectMany(r => r.Reviews)
            .DefaultIfEmpty()
            .Average(r => r?.Rating ?? 0);
        var avgPrice = location.Rooms.DefaultIfEmpty().Average(r => r?.PricePerDay ?? 0);

        var topAmenities = location.Rooms
            .SelectMany(r => r.RoomAmenities)
            .GroupBy(ra => ra.Amenity)
            .OrderByDescending(g => g.Count())
            .Take(5)
            .Select(g => new AmenityDto
            {
                Id = g.Key!.Id,
                Name = g.Key.Name,
                Description = g.Key.Description,
                Icon = g.Key.Icon,
                Category = g.Key.Category,
                RoomCount = g.Count()
            })
            .ToList();

        return new LocationListItemDto
        {
            Name = location.Name,
            Address = location.Address,
            Description = location.Description,
            ImageUrl = location.ImageUrl,
            RoomCount = roomCount,
            AvgRating = avgRating,
            AvgPrice = avgPrice,
            Images = location.LocationImages.Select(i => i.ImageUrl).ToList(),
            TopAmenities = topAmenities
        };
    }

    // Map to Detail DTO (có id)
    private static LocationDetailDto MapToDetailDto(Location location)
    {
        var dto = new LocationDetailDto
        {
            Id = location.Id,
            Name = location.Name,
            Address = location.Address,
            Description = location.Description,
            ImageUrl = location.ImageUrl,
            RoomCount = location.Rooms.Count,
            Images = location.LocationImages.Select(i => i.ImageUrl).ToList()
        };

        // Tính toán thống kê
        var reviews = location.Rooms.SelectMany(r => r.Reviews).ToList();
        dto.AvgRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0;
        dto.AvgPrice = location.Rooms.Any() ? location.Rooms.Average(r => r.PricePerDay) : 0;

        // Top 5 tiện nghi
        dto.TopAmenities = location.Rooms
            .SelectMany(r => r.RoomAmenities)
            .GroupBy(ra => ra.Amenity)
            .OrderByDescending(g => g.Count())
            .Take(5)
            .Select(g => new AmenityDto
            {
                Id = g.Key!.Id,
                Name = g.Key.Name,
                Description = g.Key.Description,
                Icon = g.Key.Icon,
                Category = g.Key.Category,
                RoomCount = g.Count()
            })
            .ToList();

        return dto;
    }
}
```

**IAmenityService.cs**:
```csharp
public interface IAmenityService
{
    Task<List<AmenityListItemDto>> GetAllAsync();
    Task<AmenityDto?> GetByIdAsync(int id);
    Task<AmenityDto> CreateAsync(CreateAmenityRequest request);
    Task<AmenityDto?> UpdateAsync(int id, UpdateAmenityRequest request);
    Task<bool> DeleteAsync(int id);
}
```

**AmenityService.cs**:
```csharp
public class AmenityService : IAmenityService
{
    private readonly StayEasyDbContext _context;

    public AmenityService(StayEasyDbContext context)
    {
        _context = context;
    }

    public async Task<List<AmenityListItemDto>> GetAllAsync()
    {
        var amenities = await _context.Amenities
            .Include(a => a.RoomAmenities)
            .ToListAsync();

        return amenities.Select(a => new AmenityListItemDto
        {
            Name = a.Name,
            Description = a.Description,
            Icon = a.Icon,
            Category = a.Category,
            RoomCount = a.RoomAmenities.Count
        }).ToList();
    }

    public async Task<AmenityDto?> GetByIdAsync(int id)
    {
        var amenity = await _context.Amenities
            .Include(a => a.RoomAmenities)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (amenity == null) return null;

        return new AmenityDto
        {
            Id = amenity.Id,
            Name = amenity.Name,
            Description = amenity.Description,
            Icon = amenity.Icon,
            Category = amenity.Category,
            RoomCount = amenity.RoomAmenities.Count
        };
    }

    public async Task<AmenityDto> CreateAsync(CreateAmenityRequest request)
    {
        // Kiểm tra tên trùng
        if (await _context.Amenities.AnyAsync(a => a.Name == request.Name))
        {
            throw new Exception("Tên tiện nghi đã tồn tại");
        }

        var amenity = new Amenity
        {
            Name = request.Name,
            Description = request.Description,
            Icon = request.Icon,
            Category = request.Category
        };

        _context.Amenities.Add(amenity);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(amenity.Id) ?? throw new Exception("Không thể tạo tiện nghi");
    }

    public async Task<AmenityDto?> UpdateAsync(int id, UpdateAmenityRequest request)
    {
        var amenity = await _context.Amenities.FindAsync(id);
        if (amenity == null) return null;

        // Kiểm tra tên trùng (nếu đổi tên)
        if (request.Name != null && request.Name != amenity.Name)
        {
            if (await _context.Amenities.AnyAsync(a => a.Name == request.Name))
            {
                throw new Exception("Tên tiện nghi đã tồn tại");
            }
            amenity.Name = request.Name;
        }

        if (request.Description != null) amenity.Description = request.Description;
        if (request.Icon != null) amenity.Icon = request.Icon;
        if (request.Category != null) amenity.Category = request.Category;

        await _context.SaveChangesAsync();
        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var amenity = await _context.Amenities
            .Include(a => a.RoomAmenities)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (amenity == null) return false;

        // Không cho xóa nếu đang được gán
        if (amenity.RoomAmenities.Any())
        {
            throw new Exception("Không thể xóa tiện nghi đang được sử dụng");
        }

        _context.Amenities.Remove(amenity);
        await _context.SaveChangesAsync();
        return true;
    }
}
```

### 2.4. Controllers

**LocationController.cs**:
```csharp
[ApiController]
[Route("api/[controller]")]
public class LocationController : ControllerBase
{
    private readonly ILocationService _locationService;

    public LocationController(ILocationService locationService)
    {
        _locationService = locationService;
    }

    // GET: api/location (không trả id)
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var locations = await _locationService.GetAllAsync();
        return Ok(new ApiResponse<List<LocationListItemDto>>
        {
            Data = locations,
            Message = "Success",
            StatusCode = 200
        });
    }

    // GET: api/location/5 (detail, có id)
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var location = await _locationService.GetByIdAsync(id);
        if (location == null)
        {
            return NotFound(new ApiResponse<object>
            {
                Message = "Không tìm thấy địa điểm",
                StatusCode = 404
            });
        }
        return Ok(new ApiResponse<LocationDetailDto>
        {
            Data = location,
            Message = "Success",
            StatusCode = 200
        });
    }

    // POST: api/location
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateLocationRequest request)
    {
        try
        {
            var location = await _locationService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = location.Id }, 
                new ApiResponse<LocationDetailDto>
                {
                    Data = location,
                    Message = "Tạo thành công",
                    StatusCode = 201
                });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<object>
            {
                Message = ex.Message,
                StatusCode = 400
            });
        }
    }

    // PUT: api/location/5
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateLocationRequest request)
    {
        try
        {
            var location = await _locationService.UpdateAsync(id, request);
            if (location == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Message = "Không tìm thấy địa điểm",
                    StatusCode = 404
                });
            }
            return Ok(new ApiResponse<LocationDetailDto>
            {
                Data = location,
                Message = "Cập nhật thành công",
                StatusCode = 200
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<object>
            {
                Message = ex.Message,
                StatusCode = 400
            });
        }
    }

    // DELETE: api/location/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var result = await _locationService.DeleteAsync(id);
            if (!result)
            {
                return NotFound(new ApiResponse<object>
                {
                    Message = "Không tìm thấy địa điểm",
                    StatusCode = 404
                });
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<object>
            {
                Message = ex.Message,
                StatusCode = 400
            });
        }
    }
}
```

**AmenityController.cs**:
```csharp
[ApiController]
[Route("api/[controller]")]
public class AmenityController : ControllerBase
{
    private readonly IAmenityService _amenityService;

    public AmenityController(IAmenityService amenityService)
    {
        _amenityService = amenityService;
    }

    // GET: api/amenity (không trả id)
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var amenities = await _amenityService.GetAllAsync();
        return Ok(new ApiResponse<List<AmenityListItemDto>>
        {
            Data = amenities,
            Message = "Success",
            StatusCode = 200
        });
    }

    // GET: api/amenity/5 (detail, có id)
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var amenity = await _amenityService.GetByIdAsync(id);
        if (amenity == null)
        {
            return NotFound(new ApiResponse<object>
            {
                Message = "Không tìm thấy tiện nghi",
                StatusCode = 404
            });
        }
        return Ok(new ApiResponse<AmenityDto>
        {
            Data = amenity,
            Message = "Success",
            StatusCode = 200
        });
    }

    // POST: api/amenity
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateAmenityRequest request)
    {
        try
        {
            var amenity = await _amenityService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = amenity.Id }, 
                new ApiResponse<AmenityDto>
                {
                    Data = amenity,
                    Message = "Tạo thành công",
                    StatusCode = 201
                });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<object>
            {
                Message = ex.Message,
                StatusCode = 400
            });
        }
    }

    // PUT: api/amenity/5
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAmenityRequest request)
    {
        try
        {
            var amenity = await _amenityService.UpdateAsync(id, request);
            if (amenity == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Message = "Không tìm thấy tiện nghi",
                    StatusCode = 404
                });
            }
            return Ok(new ApiResponse<AmenityDto>
            {
                Data = amenity,
                Message = "Cập nhật thành công",
                StatusCode = 200
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<object>
            {
                Message = ex.Message,
                StatusCode = 400
            });
        }
    }

    // DELETE: api/amenity/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var result = await _amenityService.DeleteAsync(id);
            if (!result)
            {
                return NotFound(new ApiResponse<object>
                {
                    Message = "Không tìm thấy tiện nghi",
                    StatusCode = 404
                });
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<object>
            {
                Message = ex.Message,
                StatusCode = 400
            });
        }
    }
}
```

### 2.5. Program.cs (cập nhật)

```csharp
// Thêm vào phần Services
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<IAmenityService, AmenityService>();
```

### 2.6. StayEasyDbContext.cs (cập nhật)

```csharp
// Thêm DbSet mới
public DbSet<LocationImage> LocationImages { get; set; }

// Cập nhật OnModelCreating
modelBuilder.Entity<LocationImage>(entity =>
{
    entity.HasKey(e => e.Id);
    entity.Property(e => e.ImageUrl).IsRequired().HasMaxLength(500);
    
    entity.HasOne(e => e.Location)
        .WithMany(e => e.LocationImages)
        .HasForeignKey(e => e.LocationId)
        .OnDelete(DeleteBehavior.Cascade);
});
```

---

## 3. Frontend Implementation

### 3.1. Types

**location.ts**:
```typescript
// List item (không có id)
export interface LocationListItem {
  name: string
  address?: string
  description?: string
  imageUrl?: string
  roomCount: number
  avgRating: number
  avgPrice: number
  images: string[]
  topAmenities: Amenity[]
}

// Detail (có id)
export interface LocationDetail {
  id: number
  name: string
  address?: string
  description?: string
  imageUrl?: string
  roomCount: number
  avgRating: number
  avgPrice: number
  images: string[]
  topAmenities: Amenity[]
}

export interface CreateLocationRequest {
  name: string
  address?: string
  description?: string
  imageUrl?: string
  images?: string[]
}

export interface UpdateLocationRequest {
  name?: string
  address?: string
  description?: string
  imageUrl?: string
  images?: string[]
}
```

**amenity.ts**:
```typescript
// List item (không có id)
export interface AmenityListItem {
  name: string
  description?: string
  icon?: string
  category?: string
  roomCount: number
}

// Detail (có id)
export interface Amenity {
  id: number
  name: string
  description?: string
  icon?: string
  category?: string
  roomCount: number
}

export interface CreateAmenityRequest {
  name: string
  description?: string
  icon?: string
  category?: string
}

export interface UpdateAmenityRequest {
  name?: string
  description?: string
  icon?: string
  category?: string
}
```

### 3.2. Services

**locationService.ts**:
```typescript
import api from './api'
import type { LocationListItem, LocationDetail, CreateLocationRequest, UpdateLocationRequest } from '../types/location'

export const locationService = {
  // List - không trả id
  async getAll(): Promise<LocationListItem[]> {
    const response = await api.get('/locations')
    return response.data
  },

  // Detail - có id
  async getById(id: number): Promise<LocationDetail> {
    const response = await api.get(`/locations/${id}`)
    return response.data
  },

  async create(data: CreateLocationRequest): Promise<LocationDetail> {
    const response = await api.post('/locations', data)
    return response.data
  },

  async update(id: number, data: UpdateLocationRequest): Promise<LocationDetail> {
    const response = await api.put(`/locations/${id}`, data)
    return response.data
  },

  async delete(id: number): Promise<void> {
    await api.delete(`/locations/${id}`)
  },
}
```

**amenityService.ts**:
```typescript
import api from './api'
import type { AmenityListItem, Amenity, CreateAmenityRequest, UpdateAmenityRequest } from '../types/amenity'

export const amenityService = {
  // List - không trả id
  async getAll(): Promise<AmenityListItem[]> {
    const response = await api.get('/amenities')
    return response.data
  },

  // Detail - có id
  async getById(id: number): Promise<Amenity> {
    const response = await api.get(`/amenities/${id}`)
    return response.data
  },

  async create(data: CreateAmenityRequest): Promise<Amenity> {
    const response = await api.post('/amenities', data)
    return response.data
  },

  async update(id: number, data: UpdateAmenityRequest): Promise<Amenity> {
    const response = await api.put(`/amenities/${id}`, data)
    return response.data
  },

  async delete(id: number): Promise<void> {
    await api.delete(`/amenities/${id}`)
  },
}
```

### 3.3. Utils

**format.ts**:
```typescript
export const formatVnd = (value: number): string => {
  return new Intl.NumberFormat('vi-VN').format(value) + ' ₫'
}
```

### 3.4. Pages

**LocationManager.tsx** (Admin only):
```typescript
import { useState, useEffect } from 'react'
import { locationService } from '../services/locationService'
import { formatVnd } from '../utils/format'
import type { LocationListItem } from '../types/location'

export default function LocationManager() {
  const [locations, setLocations] = useState<LocationListItem[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [showForm, setShowForm] = useState(false)
  const [editingLocation, setEditingLocation] = useState<LocationListItem | null>(null)

  useEffect(() => {
    loadLocations()
  }, [])

  const loadLocations = async () => {
    try {
      const data = await locationService.getAll()
      setLocations(data)
    } catch (error) {
      console.error('Lỗi tải locations:', error)
    } finally {
      setIsLoading(false)
    }
  }

  const handleDelete = async (id: number) => {
    if (!confirm('Bạn có chắc muốn xóa địa điểm này?')) return
    try {
      await locationService.delete(id)
      loadLocations()
    } catch (error) {
      alert('Không thể xóa: ' + (error as Error).message)
    }
  }

  if (isLoading) return <div>Đang tải...</div>

  return (
    <div className="container py-8">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold text-left">Quản lý địa điểm</h1>
        <button
          onClick={() => {
            setEditingLocation(null)
            setShowForm(true)
          }}
          className="btn btn-primary"
        >
          Thêm địa điểm
        </button>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {locations.map((location, index) => (
          <div key={index} className="card">
            {location.imageUrl && (
              <img
                src={location.imageUrl}
                alt={location.name}
                className="w-full h-48 object-cover rounded mb-4"
              />
            )}
            <h3 className="text-lg font-semibold mb-2 text-left">{location.name}</h3>
            <p className="text-gray-600 text-sm text-left mb-2">{location.address}</p>
            <div className="flex justify-between text-sm mb-4">
              <span className="text-left">{location.roomCount} phòng</span>
              <span className="text-right">⭐ {location.avgRating.toFixed(1)}</span>
            </div>
            <div className="flex justify-between items-center mb-4">
              <span className="text-sm text-gray-500 text-left">Giá TB</span>
              <span className="number-vn text-right">{formatVnd(location.avgPrice)}</span>
            </div>
            <div className="flex gap-2">
              <button
                onClick={() => {
                  setEditingLocation(location)
                  setShowForm(true)
                }}
                className="btn btn-secondary flex-1"
              >
                Sửa
              </button>
              <button
                onClick={() => handleDelete(location.id)}
                className="btn btn-danger flex-1"
              >
                Xóa
              </button>
            </div>
          </div>
        ))}
      </div>

      {showForm && (
        <LocationForm
          location={editingLocation}
          onClose={() => setShowForm(false)}
          onSuccess={() => {
            setShowForm(false)
            loadLocations()
          }}
        />
      )}
    </div>
  )
}
```

**AmenityManager.tsx** (Admin only):
```typescript
import { useState, useEffect } from 'react'
import { amenityService } from '../services/amenityService'
import type { AmenityListItem } from '../types/amenity'

export default function AmenityManager() {
  const [amenities, setAmenities] = useState<AmenityListItem[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [showForm, setShowForm] = useState(false)
  const [editingAmenity, setEditingAmenity] = useState<AmenityListItem | null>(null)

  useEffect(() => {
    loadAmenities()
  }, [])

  const loadAmenities = async () => {
    try {
      const data = await amenityService.getAll()
      setAmenities(data)
    } catch (error) {
      console.error('Lỗi tải amenities:', error)
    } finally {
      setIsLoading(false)
    }
  }

  const handleDelete = async (id: number) => {
    if (!confirm('Bạn có chắc muốn xóa tiện nghi này?')) return
    try {
      await amenityService.delete(id)
      loadAmenities()
    } catch (error) {
      alert('Không thể xóa: ' + (error as Error).message)
    }
  }

  if (isLoading) return <div>Đang tải...</div>

  return (
    <div className="container py-8">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold text-left">Quản lý tiện nghi</h1>
        <button
          onClick={() => {
            setEditingAmenity(null)
            setShowForm(true)
          }}
          className="btn btn-primary"
        >
          Thêm tiện nghi
        </button>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
        {amenities.map((amenity, index) => (
          <div key={index} className="card">
            <div className="flex items-center gap-3 mb-2">
              {amenity.icon && <span className="text-2xl">{amenity.icon}</span>}
              <h3 className="text-lg font-semibold text-left">{amenity.name}</h3>
            </div>
            <p className="text-gray-600 text-sm text-left mb-2">{amenity.description}</p>
            <div className="flex justify-between text-sm mb-4">
              <span className="text-left">{amenity.category}</span>
              <span className="text-right">{amenity.roomCount} phòng</span>
            </div>
            <div className="flex gap-2">
              <button
                onClick={() => {
                  setEditingAmenity(amenity)
                  setShowForm(true)
                }}
                className="btn btn-secondary flex-1"
              >
                Sửa
              </button>
              <button
                onClick={() => handleDelete(amenity.id)}
                className="btn btn-danger flex-1"
              >
                Xóa
              </button>
            </div>
          </div>
        ))}
      </div>

      {showForm && (
        <AmenityForm
          amenity={editingAmenity}
          onClose={() => setShowForm(false)}
          onSuccess={() => {
            setShowForm(false)
            loadAmenities()
          }}
        />
      )}
    </div>
  )
}
```

### 3.5. Components

**LocationForm.tsx**:
```typescript
import { useState, type FormEvent } from 'react'
import { locationService } from '../services/locationService'
import type { LocationListItem, CreateLocationRequest } from '../types/location'

interface Props {
  location: LocationListItem | null
  onClose: () => void
  onSuccess: () => void
}

export default function LocationForm({ location, onClose, onSuccess }: Props) {
  const [name, setName] = useState(location?.name ?? '')
  const [address, setAddress] = useState(location?.address ?? '')
  const [description, setDescription] = useState(location?.description ?? '')
  const [imageUrl, setImageUrl] = useState(location?.imageUrl ?? '')
  const [isLoading, setIsLoading] = useState(false)
  const [error, setError] = useState('')

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault()
    setError('')
    setIsLoading(true)

    try {
      const data: CreateLocationRequest = {
        name,
        address,
        description,
        imageUrl,
      }

      if (location) {
        await locationService.update(location.id, data)
      } else {
        await locationService.create(data)
      }
      onSuccess()
    } catch (err) {
      setError((err as Error).message)
    } finally {
      setIsLoading(false)
    }
  }

  return (
    <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
      <div className="card w-full max-w-md">
        <h2 className="text-xl font-bold mb-4 text-left">
          {location ? 'Sửa địa điểm' : 'Thêm địa điểm'}
        </h2>

        {error && (
          <div className="bg-red-50 text-red-600 p-3 rounded mb-4 text-left">{error}</div>
        )}

        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium mb-1 text-left">Tên địa điểm</label>
            <input
              type="text"
              className="input"
              value={name}
              onChange={(e) => setName(e.target.value)}
              required
            />
          </div>

          <div>
            <label className="block text-sm font-medium mb-1 text-left">Địa chỉ</label>
            <input
              type="text"
              className="input"
              value={address}
              onChange={(e) => setAddress(e.target.value)}
            />
          </div>

          <div>
            <label className="block text-sm font-medium mb-1 text-left">Mô tả</label>
            <textarea
              className="input"
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              rows={3}
            />
          </div>

          <div>
            <label className="block text-sm font-medium mb-1 text-left">URL hình ảnh</label>
            <input
              type="url"
              className="input"
              value={imageUrl}
              onChange={(e) => setImageUrl(e.target.value)}
            />
          </div>

          <div className="flex gap-2">
            <button
              type="button"
              onClick={onClose}
              className="btn btn-secondary flex-1"
            >
              Hủy
            </button>
            <button
              type="submit"
              className="btn btn-primary flex-1"
              disabled={isLoading}
            >
              {isLoading ? 'Đang lưu...' : location ? 'Cập nhật' : 'Thêm'}
            </button>
          </div>
        </form>
      </div>
    </div>
  )
}
```

**AmenityForm.tsx**:
```typescript
import { useState, type FormEvent } from 'react'
import { amenityService } from '../services/amenityService'
import type { AmenityListItem, CreateAmenityRequest } from '../types/amenity'

interface Props {
  amenity: AmenityListItem | null
  onClose: () => void
  onSuccess: () => void
}

const CATEGORIES = ['WiFi', 'Nấu nướng', 'Giải trí', 'An toàn', 'Khác']

export default function AmenityForm({ amenity, onClose, onSuccess }: Props) {
  const [name, setName] = useState(amenity?.name ?? '')
  const [description, setDescription] = useState(amenity?.description ?? '')
  const [icon, setIcon] = useState(amenity?.icon ?? '')
  const [category, setCategory] = useState(amenity?.category ?? '')
  const [isLoading, setIsLoading] = useState(false)
  const [error, setError] = useState('')

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault()
    setError('')
    setIsLoading(true)

    try {
      const data: CreateAmenityRequest = {
        name,
        description,
        icon,
        category,
      }

      if (amenity) {
        await amenityService.update(amenity.id, data)
      } else {
        await amenityService.create(data)
      }
      onSuccess()
    } catch (err) {
      setError((err as Error).message)
    } finally {
      setIsLoading(false)
    }
  }

  return (
    <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
      <div className="card w-full max-w-md">
        <h2 className="text-xl font-bold mb-4 text-left">
          {amenity ? 'Sửa tiện nghi' : 'Thêm tiện nghi'}
        </h2>

        {error && (
          <div className="bg-red-50 text-red-600 p-3 rounded mb-4 text-left">{error}</div>
        )}

        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium mb-1 text-left">Tên tiện nghi</label>
            <input
              type="text"
              className="input"
              value={name}
              onChange={(e) => setName(e.target.value)}
              required
            />
          </div>

          <div>
            <label className="block text-sm font-medium mb-1 text-left">Mô tả</label>
            <textarea
              className="input"
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              rows={2}
            />
          </div>

          <div>
            <label className="block text-sm font-medium mb-1 text-left">Icon (emoji)</label>
            <input
              type="text"
              className="input"
              value={icon}
              onChange={(e) => setIcon(e.target.value)}
              placeholder="📶"
            />
          </div>

          <div>
            <label className="block text-sm font-medium mb-1 text-left">Danh mục</label>
            <select
              className="input"
              value={category}
              onChange={(e) => setCategory(e.target.value)}
            >
              <option value="">Chọn danh mục</option>
              {CATEGORIES.map((cat) => (
                <option key={cat} value={cat}>
                  {cat}
                </option>
              ))}
            </select>
          </div>

          <div className="flex gap-2">
            <button
              type="button"
              onClick={onClose}
              className="btn btn-secondary flex-1"
            >
              Hủy
            </button>
            <button
              type="submit"
              className="btn btn-primary flex-1"
              disabled={isLoading}
            >
              {isLoading ? 'Đang lưu...' : amenity ? 'Cập nhật' : 'Thêm'}
            </button>
          </div>
        </form>
      </div>
    </div>
  )
}
```

---

## 4. API Endpoints Summary

### Locations
| Method | Endpoint | Auth | Mô tả |
|--------|----------|------|-------|
| GET | /api/location | Public | Danh sách địa điểm (không id) |
| GET | /api/location/{id} | Public | Chi tiết địa điểm (có id) |
| POST | /api/location | Admin | Tạo địa điểm |
| PUT | /api/location/{id} | Admin | Cập nhật địa điểm |
| DELETE | /api/location/{id} | Admin | Xóa địa điểm |

### Amenities
| Method | Endpoint | Auth | Mô tả |
|--------|----------|------|-------|
| GET | /api/amenity | Public | Danh sách tiện nghi (không id) |
| GET | /api/amenity/{id} | Public | Chi tiết tiện nghi (có id) |
| POST | /api/amenity | Admin | Tạo tiện nghi |
| PUT | /api/amenity/{id} | Admin | Cập nhật tiện nghi |
| DELETE | /api/amenity/{id} | Admin | Xóa tiện nghi |

---

## 5. Files cần tạo

### Backend (10 files)
```
server/
├── Controllers/
│   ├── LocationController.cs      # Mới
│   └── AmenityController.cs       # Mới
├── Services/
│   ├── ILocationService.cs        # Mới
│   ├── LocationService.cs         # Mới
│   ├── IAmenityService.cs         # Mới
│   └── AmenityService.cs          # Mới
├── Models/
│   ├── DTOs/
│   │   ├── LocationDtos.cs        # Mới
│   │   └── AmenityDtos.cs         # Mới
│   └── Entities/
│       ├── Location.cs            # Cập nhật
│       ├── Amenity.cs             # Cập nhật
│       └── LocationImage.cs       # Mới
├── Data/
│   └── StayEasyDbContext.cs       # Cập nhật
└── Program.cs                     # Cập nhật
```

### Frontend (7 files)
```
client/src/
├── services/
│   ├── locationService.ts         # Mới
│   └── amenityService.ts          # Mới
├── types/
│   ├── location.ts                # Mới
│   └── amenity.ts                 # Mới
├── utils/
│   └── format.ts                  # Mới (formatVnd)
├── pages/
│   ├── LocationManager.tsx        # Mới
│   └── AmenityManager.tsx         # Mới
└── components/
    ├── LocationForm.tsx           # Mới
    └── AmenityForm.tsx            # Mới
```

### Database (1 file)
```
docs/database/
└── migration_location_amenity.sql # Mới
```

---

## 6. Thứ tự implementation

1. **Database**: Tạo migration + cập nhật schema
2. **Backend Entities**: Cập nhật Location.cs, Amenity.cs, thêm LocationImage.cs
3. **Backend DTOs**: Tạo LocationDtos.cs, AmenityDtos.cs
4. **Backend Services**: Tạo ILocationService, LocationService, IAmenityService, AmenityService
5. **Backend Controllers**: Tạo LocationController, AmenityController
6. **Backend Program.cs**: Đăng ký services
7. **Frontend Types**: Tạo location.ts, amenity.ts
8. **Frontend Utils**: Tạo format.ts (formatVnd)
9. **Frontend Services**: Tạo locationService.ts, amenityService.ts
10. **Frontend Components**: Tạo LocationForm.tsx, AmenityForm.tsx
11. **Frontend Pages**: Tạo LocationManager.tsx, AmenityManager.tsx
12. **Test**: Test API endpoints + UI

---

## 7. Kiểm tra

Sau khi implement xong, kiểm tra:
- [ ] Tạo location thành công
- [ ] Tạo location trùng tên → báo lỗi
- [ ] Xóa location còn phòng → báo lỗi
- [ ] Tạo amenity thành công
- [ ] Tạo amenity trùng tên → báo lỗi
- [ ] Xóa amenity đang dùng → báo lỗi
- [ ] Hiển thị thống kê (roomCount, avgRating, avgPrice)
- [ ] Hiển thị top amenities
- [ ] Phân quyền: chỉ Admin mới thêm/sửa/xóa
- [ ] Format VND hiển thị đúng
- [ ] Căn lề: text-left, number text-right
- [ ] Không hiển thị ID trong list

---

**Bạn duyệt kế hoạch này và bắt đầu implement không?**