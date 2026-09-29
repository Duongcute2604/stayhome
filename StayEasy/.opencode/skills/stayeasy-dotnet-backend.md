# stayeasy-dotnet-backend

## Skill phát triển Backend API (ASP.NET Core Web API)

### Khi nào sử dụng
- Tạo mới Controller, Service, Repository
- Viết business logic với EF Core
- Xử lý JWT authentication & authorization
- Cấu hình Swagger/OpenAPI
- Validation nghiệp vụ (booking conflict, tính giá)

---

## Quy tắc bắt buộc

### 1. Ẩn hoàn toàn ID/UI
```csharp
// ❌ KHÔNG trả id trong list endpoints
public async Task<IActionResult> GetAll()
{
    var items = await _service.GetAllAsync();
    return Ok(items); // Không bao gồm id
}

// ✅ Chỉ trả id trong detail endpoint
public async Task<IActionResult> GetById(int id)
{
    var item = await _service.GetByIdAsync(id);
    return Ok(item); // Có id nhưng FE không hiển thị
}

// ✅ Dùng DTO để kiểm soát fields trả về
public class SomeListItemDto
{
    public string Name { get; set; }
    public string? Description { get; set; }
    // Không có Id
}
```

### 2. Format Response
```csharp
// API Response Format
public class ApiResponse<T>
{
    public T Data { get; set; }
    public string Message { get; set; }
    public int StatusCode { get; set; }
}

// Sử dụng
return Ok(new ApiResponse<List<SomeDto>>
{
    Data = items,
    Message = "Success",
    StatusCode = 200
});
```

### 3. Image Storage (Local Filesystem)
```csharp
// Lưu đường dẫn relative path trong DB
public class Room
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? ImageUrl { get; set; } // "/images/rooms/room1.jpg"
}

// Upload image
public async Task<string> UploadImage(IFormFile file)
{
    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
    var filePath = Path.Combine("wwwroot/images", fileName);
    
    using (var stream = new FileStream(filePath, FileMode.Create))
    {
        await file.CopyToAsync(stream);
    }
    
    return $"/images/{fileName}";
}
```

---

## Cấu trúc thư mục

```
server/
├── Controllers/              # API Controllers
├── Services/                 # Business logic
│   ├── I*Service.cs         # Interfaces
│   └── *Service.cs          # Implementations
├── Models/
│   ├── Entities/             # Database entities
│   └── DTOs/                 # Data Transfer Objects
├── Data/
│   ├── StayEasyDbContext.cs # EF Core DbContext
│   └── Migrations/           # EF Core migrations
├── Middleware/               # Custom middleware
├── Helpers/                  # Helper classes
├── wwwroot/
│   └── images/               # Uploaded images
├── Program.cs                # Entry point
└── StayEasy.csproj          # Project file
```

---

## Controller Pattern

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Models.DTOs;
using server.Services;

namespace server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SomeController : ControllerBase
    {
        private readonly ISomeService _someService;

        public SomeController(ISomeService someService)
        {
            _someService = someService;
        }

        // GET: api/some (không trả id)
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _someService.GetAllAsync();
            return Ok(new ApiResponse<List<SomeListItemDto>>
            {
                Data = items,
                Message = "Success",
                StatusCode = 200
            });
        }

        // GET: api/some/5 (detail, có id)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _someService.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Message = "Không tìm thấy",
                    StatusCode = 404
                });
            }
            return Ok(new ApiResponse<SomeDto>
            {
                Data = item,
                Message = "Success",
                StatusCode = 200
            });
        }

        // POST: api/some
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateSomeRequest request)
        {
            try
            {
                var item = await _someService.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = item.Id }, 
                    new ApiResponse<SomeDto>
                    {
                        Data = item,
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

        // PUT: api/some/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateSomeRequest request)
        {
            var item = await _someService.UpdateAsync(id, request);
            if (item == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Message = "Không tìm thấy",
                    StatusCode = 404
                });
            }
            return Ok(new ApiResponse<SomeDto>
            {
                Data = item,
                Message = "Cập nhật thành công",
                StatusCode = 200
            });
        }

        // DELETE: api/some/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _someService.DeleteAsync(id);
            if (!result)
            {
                return NotFound(new ApiResponse<object>
                {
                    Message = "Không tìm thấy",
                    StatusCode = 404
                });
            }
            return NoContent();
        }
    }
}
```

---

## Service Pattern

```csharp
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Models.DTOs;
using server.Models.Entities;

namespace server.Services
{
    public interface ISomeService
    {
        Task<List<SomeListItemDto>> GetAllAsync();
        Task<SomeDto?> GetByIdAsync(int id);
        Task<SomeDto> CreateAsync(CreateSomeRequest request);
        Task<SomeDto?> UpdateAsync(int id, UpdateSomeRequest request);
        Task<bool> DeleteAsync(int id);
    }

    public class SomeService : ISomeService
    {
        private readonly StayEasyDbContext _context;

        public SomeService(StayEasyDbContext context)
        {
            _context = context;
        }

        public async Task<List<SomeListItemDto>> GetAllAsync()
        {
            var items = await _context.Somes
                .Include(s => s.RelatedEntity)
                .ToListAsync();
            
            // Map sang DTO không có id
            return items.Select(item => new SomeListItemDto
            {
                Name = item.Name,
                Description = item.Description
            }).ToList();
        }

        public async Task<SomeDto?> GetByIdAsync(int id)
        {
            var item = await _context.Somes
                .Include(s => s.RelatedEntity)
                .FirstOrDefaultAsync(s => s.Id == id);
            
            if (item == null) return null;
            
            return new SomeDto
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description
            };
        }

        public async Task<SomeDto> CreateAsync(CreateSomeRequest request)
        {
            // Validation
            if (await _context.Somes.AnyAsync(s => s.Name == request.Name))
            {
                throw new Exception("Tên đã tồn tại");
            }

            var item = new Some
            {
                Name = request.Name,
                Description = request.Description,
            };

            _context.Somes.Add(item);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(item.Id) ?? throw new Exception("Không thể tạo");
        }

        public async Task<SomeDto?> UpdateAsync(int id, UpdateSomeRequest request)
        {
            var item = await _context.Somes.FindAsync(id);
            if (item == null) return null;

            if (request.Name != null) item.Name = request.Name;
            if (request.Description != null) item.Description = request.Description;

            await _context.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var item = await _context.Somes.FindAsync(id);
            if (item == null) return false;

            _context.Somes.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
```

---

## JWT Authentication Pattern (Access + Refresh Token)

```csharp
// Program.cs - Cấu hình JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!))
        };
    });

// Tạo Access Token
private string GenerateAccessToken(User user)
{
    var securityKey = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]!));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var claims = new[]
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.Role),
        new Claim(ClaimTypes.Name, user.FullName)
    };

    var token = new JwtSecurityToken(
        issuer: _configuration["Jwt:Issuer"],
        audience: _configuration["Jwt:Audience"],
        claims: claims,
        expires: DateTime.Now.AddHours(24),
        signingCredentials: credentials
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}

// Tạo Refresh Token
private string GenerateRefreshToken()
{
    var randomNumber = new byte[32];
    using var rng = RandomNumberGenerator.Create();
    rng.GetBytes(randomNumber);
    return Convert.ToBase64String(randomNumber);
}

// Protect endpoints
[Authorize(Roles = "Admin")]
public async Task<IActionResult> AdminOnly() { }

[Authorize(Roles = "Admin,Employee")]
public async Task<IActionResult> AdminOrEmployee() { }
```

---

## Swagger/OpenAPI Pattern

```csharp
// Program.cs
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "StayEasy API",
        Version = "v1",
        Description = "Hệ thống đặt phòng và quản lý homestay"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
```

---

## Business Logic Patterns

### Booking Conflict Validation
```csharp
public async Task<bool> HasConflictAsync(int roomId, DateTime checkIn, DateTime checkOut)
{
    return await _context.Bookings.AnyAsync(b =>
        b.RoomId == roomId &&
        b.Status != "CANCELLED" &&
        b.CheckIn < checkOut &&
        b.CheckOut > checkIn);
}
```

### Price Calculation
```csharp
public decimal CalculatePrice(Room room, string bookingType, DateTime checkIn, DateTime checkOut)
{
    if (bookingType == "HOURLY")
    {
        var hours = Math.Max(3, (decimal)(checkOut - checkIn).TotalHours);
        return hours * room.PricePerHour;
    }
    else
    {
        var days = (checkOut - checkIn).Days;
        return days * room.PricePerDay;
    }
}
```

### Password Hashing
```csharp
private static string HashPassword(string password)
{
    using var sha256 = SHA256.Create();
    var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
    return Convert.ToBase64String(bytes);
}

private static bool VerifyPassword(string password, string hash)
{
    return HashPassword(password) == hash;
}
```

---

## DTO Pattern

```csharp
// Entity
public class Some
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// List DTO (không có id)
public class SomeListItemDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

// Detail DTO (có id)
public class SomeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

// Create Request
public class CreateSomeRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

// Update Request
public class UpdateSomeRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
}
```

---

## Naming Conventions

- **Controllers**: PascalCase with `Controller` suffix (e.g., `RoomController`)
- **Services**: PascalCase with `Service` suffix (e.g., `RoomService`)
- **Interfaces**: PascalCase with `I` prefix (e.g., `IRoomService`)
- **DTOs**: PascalCase with `Dto` suffix (e.g., `RoomDto`)
- **Entities**: PascalCase (e.g., `Room`, `Booking`)
- **Methods**: PascalCase (e.g., `GetByIdAsync`, `CreateAsync`)
- **Async methods**: Suffix `Async` (e.g., `GetAllAsync`)
