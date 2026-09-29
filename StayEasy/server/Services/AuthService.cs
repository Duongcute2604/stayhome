using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using server.Data;
using server.Models.DTOs;
using server.Models.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace server.Services
{
    // ============================================================================
    // AUTH SERVICE - Xử lý logic xác thực
    // ============================================================================
    public class AuthService : IAuthService
    {
        private readonly StayEasyDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(StayEasyDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // ============================================================================
        // ĐĂNG KÝ
        // ============================================================================
        public async Task<LoginResponse> RegisterAsync(RegisterRequest request)
        {
            // Kiểm tra email đã tồn tại chưa
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                throw new Exception("Email đã tồn tại");
            }

            // Tạo user mới
            var user = new User
            {
                Email = request.Email,
                PasswordHash = HashPassword(request.Password),
                FullName = request.FullName,
                Phone = request.Phone,
                Role = "Customer"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Tạo JWT token
            var token = GenerateJwtToken(user);

            return new LoginResponse
            {
                Token = token,
                User = MapToDto(user)
            };
        }

        // ============================================================================
        // ĐĂNG NHẬP
        // ============================================================================
        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
            {
                throw new Exception("Email hoặc mật khẩu không đúng");
            }

            var token = GenerateJwtToken(user);

            return new LoginResponse
            {
                Token = token,
                User = MapToDto(user)
            };
        }

        // ============================================================================
        // LẤY USER THEO ID
        // ============================================================================
        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        // ============================================================================
        // HELPER METHODS
        // ============================================================================

        // Hash password bằng SHA256 (hex lowercase, khớp seed data trong init SQL)
        private static string HashPassword(string password)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }

        // Verify password
        private static bool VerifyPassword(string password, string hash)
        {
            return HashPassword(password) == hash;
        }

        // Tạo JWT token
        private string GenerateJwtToken(User user)
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
                expires: DateTime.Now.AddHours(double.Parse(_configuration["Jwt:ExpirationHours"]!)),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Map entity sang DTO
        private static UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Phone = user.Phone,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            };
        }
    }
}
