using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using server.Data;
using server.Middleware;
using server.Services;

var builder = WebApplication.CreateBuilder(args);

// ============================================================================
// DATABASE CONTEXT
// ============================================================================
// Tại sao dùng UseMySql: Kết nối với MySQL thông qua EF Core
// ServerVersion.AutoDetect: Tự động phát hiện phiên bản MySQL
// ============================================================================
builder.Services.AddDbContext<StayEasyDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));

// ============================================================================
// JWT AUTHENTICATION
// ============================================================================
// Tại sao cần: Xác thực người dùng thông qua JWT token
// ============================================================================
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

// ============================================================================
// CORS
// ============================================================================
// Tại sao cần: Cho phép frontend gọi API từ domain khác
// ============================================================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()!)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// ============================================================================
// SERVICES
// ============================================================================
// Tại sao cần: Đăng ký các services để Dependency Injection
// ============================================================================
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IBookingService, BookingService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Health check cho Docker (backend healthcheck gọi /health)
builder.Services.AddHealthChecks();

// ============================================================================
// SWAGGER
// ============================================================================
// Tại sao cần: Tự động tạo tài liệu API và UI để test
// ============================================================================
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "StayEasy API",
        Version = "v1",
        Description = "Hệ thống đặt phòng và quản lý homestay"
    });

    // Thêm JWT authentication vào Swagger
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

var app = builder.Build();

// ============================================================================
// MIDDLEWARE PIPELINE
// ============================================================================
// Thứ tự quan trọng: CORS → Auth → Authorization → Controllers
// ============================================================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.UseExceptionMiddleware();
app.UseStaticFiles(); // Phục vụ ảnh local trong wwwroot/images
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
