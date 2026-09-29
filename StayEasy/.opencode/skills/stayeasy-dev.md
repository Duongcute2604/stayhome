# StayEasy Development Skill

## Project Overview
- **Tên dự án**: StayEasy - Hệ thống đặt phòng và quản lý homestay
- **Stack**: React + TypeScript + Vite (Frontend), ASP.NET Core Web API + EF Core + MySQL (Backend)
- **Port**: Frontend 3000, Backend 5000, MySQL 3307
- **Actors**: Customer, Employee, Admin

## Project Structure

```
StayEasy/
├── client/                    # Frontend (React + TypeScript + Vite)
│   ├── src/
│   │   ├── components/        # Reusable components
│   │   ├── context/           # React Context (AuthContext)
│   │   ├── hooks/             # Custom hooks (useAuth)
│   │   ├── pages/             # Page components
│   │   ├── services/          # API services (axios)
│   │   ├── types/             # TypeScript types
│   │   ├── App.tsx            # Root component + routes
│   │   ├── main.tsx           # Entry point
│   │   └── index.css          # Global styles
│   ├── Dockerfile
│   ├── nginx.conf
│   ├── package.json
│   ├── tsconfig.json
│   └── vite.config.ts
│
├── server/                    # Backend (ASP.NET Core Web API)
│   ├── Controllers/           # API controllers
│   ├── Data/                  # DbContext
│   ├── Middleware/            # Custom middleware
│   ├── Models/
│   │   ├── DTOs/              # Data Transfer Objects
│   │   └── Entities/          # Database entities
│   ├── Services/              # Business logic
│   ├── Dockerfile
│   ├── Program.cs             # Entry point
│   ├── appsettings.json
│   └── StayEasy.csproj
│
├── docs/
│   ├── database/              # SQL scripts
│   ├── api/                   # API documentation
│   └── diagrams/              # Architecture diagrams
│
├── docker-compose.yml
├── .env.example
└── README.md
```

## Naming Conventions

### Frontend (React + TypeScript)
- **Components**: PascalCase (e.g., `RoomCard.tsx`, `BookingForm.tsx`)
- **Hooks**: camelCase with `use` prefix (e.g., `useAuth.ts`, `useRooms.ts`)
- **Services**: camelCase with `Service` suffix (e.g., `authService.ts`, `roomService.ts`)
- **Types**: PascalCase with `Type` suffix (e.g., `RoomType`, `BookingType`)
- **Constants**: UPPER_SNAKE_CASE (e.g., `API_BASE_URL`, `MAX_RETRY_COUNT`)

### Backend (C#)
- **Controllers**: PascalCase with `Controller` suffix (e.g., `AuthController`, `RoomController`)
- **Services**: PascalCase with `Service` suffix (e.g., `AuthService`, `RoomService`)
- **Interfaces**: PascalCase with `I` prefix (e.g., `IAuthService`, `IRoomService`)
- **DTOs**: PascalCase with `Dto` suffix (e.g., `RoomDto`, `BookingDto`)
- **Entities**: PascalCase (e.g., `User`, `Room`, `Booking`)
- **Methods**: PascalCase (e.g., `GetUserById`, `CreateBooking`)

## Code Style

### Frontend
```typescript
// Services pattern
export const roomService = {
  async getRooms(): Promise<Room[]> {
    const response = await api.get('/rooms')
    return response.data
  },
}

// Hooks pattern
export function useAuth(): AuthContextType {
  const context = useContext(AuthContext)
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider')
  }
  return context
}

// Types pattern
export interface Room {
  id: number
  name: string
  pricePerDay: number
  status: RoomStatus
}
```

### Backend
```csharp
// Controller pattern
[ApiController]
[Route("api/[controller]")]
public class RoomController : ControllerBase
{
    private readonly IRoomService _roomService;

    public RoomController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var rooms = await _roomService.GetAllAsync();
        return Ok(rooms);
    }
}

// Service pattern
public class RoomService : IRoomService
{
    private readonly StayEasyDbContext _context;

    public RoomService(StayEasyDbContext context)
    {
        _context = context;
    }

    public async Task<List<RoomDto>> GetAllAsync()
    {
        var rooms = await _context.Rooms
            .Include(r => r.Location)
            .ToListAsync();
        return rooms.Select(MapToDto).ToList();
    }
}
```

## Common Patterns

### API Response Format
```json
{
  "data": { },
  "message": "Success",
  "statusCode": 200
}
```

### Error Handling
```csharp
// Backend - Exception Middleware
catch (Exception ex)
{
    return BadRequest(new { message = ex.Message });
}

// Frontend - Axios Interceptor
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('token')
      window.location.href = '/login'
    }
    return Promise.reject(error)
  }
)
```

### JWT Authentication
```csharp
// Generate token
var token = new JwtSecurityToken(
    issuer: _configuration["Jwt:Issuer"],
    audience: _configuration["Jwt:Audience"],
    claims: claims,
    expires: DateTime.Now.AddHours(24),
    signingCredentials: credentials
);

// Use in controller
[Authorize(Roles = "Admin")]
public async Task<IActionResult> Create([FromBody] CreateRoomRequest request)
```

## Business Rules

### Booking Rules
- Đặt phòng phải trước ít nhất **2 giờ**
- Đặt theo giờ: tối thiểu **3 giờ**
- Check-in: **14:00**, Check-out: **12:00**
- Sau check-out: phòng **CLEANING 2 giờ**

### Room Status
- `AVAILABLE` - Sẵn sàng đặt
- `OCCUPIED` - Đang có khách
- `CLEANING` - Đang dọn dẹp
- `MAINTENANCE` - Đang bảo trì

### Booking Status
- `PENDING` - Chờ xác nhận
- `CONFIRMED` - Đã xác nhận
- `CHECKED_IN` - Đã nhận phòng
- `CHECKED_OUT` - Đã trả phòng
- `CANCELLED` - Đã hủy

## Git Workflow

### Branch Naming
- `main` - Production code
- `develop` - Development code
- `feature/xxx` - New features (e.g., `feature/booking-payment`)
- `fix/xxx` - Bug fixes (e.g., `fix/login-redirect`)
- `hotfix/xxx` - Urgent fixes

### Commit Messages
```
feat: add booking payment integration
fix: resolve login redirect loop
docs: update API documentation
refactor: optimize room search query
test: add unit tests for booking service
```

## Docker Commands

```bash
# Start all services
docker-compose up -d

# View logs
docker-compose logs -f

# Stop all services
docker-compose down

# Rebuild and start
docker-compose up -d --build

# Access MySQL container
docker exec -it stayeasy-mysql mysql -u root -proot123

# Run EF Core migrations
dotnet ef migrations add MigrationName
dotnet ef database update
```

## Database

### Connection
- Host: `localhost`
- Port: `3307`
- Database: `StayEasy`
- User: `stayeasy`
- Password: `stayeasy123`

### Key Tables
- `users` - Người dùng
- `rooms` - Phòng
- `bookings` - Đặt phòng
- `locations` - Địa điểm
- `amenities` - Tiện nghi
- `reviews` - Đánh giá

## Troubleshooting

### Common Issues

**1. CORS Error**
```csharp
// Check CORS config in Program.cs
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
```

**2. JWT Token Expired**
```csharp
// Check token expiration
var token = new JwtSecurityTokenHandler().ReadJwtToken(tokenString);
var exp = token.ValidTo;
```

**3. Database Connection Failed**
```bash
# Check if MySQL container is running
docker ps | grep mysql

# Check logs
docker logs stayeasy-mysql
```

## Extension Points

Khi cần thêm skill mới, tạo file trong `.opencode/skills/`:
- `stayeasy-testing.md` - Testing patterns
- `stayeasy-cicd.md` - CI/CD pipeline
- `stayeasy-deployment.md` - Deployment guide
