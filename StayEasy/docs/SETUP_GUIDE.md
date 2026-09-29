# StayEasy — Hướng dẫn cài đặt chi tiết

## Tổng quan quy trình

```
BƯỚC 1: Cài .NET SDK 8.0
    ↓
BƯỚC 2: Khởi tạo Frontend (React + TypeScript + Vite)
    ↓
BƯỚC 3: Khởi tạo Backend (ASP.NET Core Web API)
    ↓
BƯỚC 4: Cấu hình Database (MySQL)
    ↓
BƯỚC 5: Chạy và kiểm tra
```

---

## BƯỚC 1: Cài .NET SDK 8.0

### 1.1. Kiểm tra đã cài chưa

```powershell
dotnet --version
```

Nếu hiển thị phiên bản (ví dụ: `8.0.425`) → Bỏ qua bước này.

### 1.2. Cài đặt

**Cách 1: Chạy script tự đột (Khuyến nghị)**

```powershell
# Tải script cài đặt
Invoke-WebRequest -Uri "https://dot.net/v1/dotnet-install.ps1" -OutFile "$env:TEMP\dotnet-install.ps1"

# Chại script
& "$env:TEMP\dotnet-install.ps1" -Channel 8.0
```

**Cách 2: Cài thủ công**

1. Truy cập: https://dotnet.microsoft.com/download
2. Tải .NET SDK 8.0 cho Windows x64
3. Chại file cài đặt
4. Khởi đột lại máy tính

### 1.3. Kiểm tra

```powershell
dotnet --version
# Kết quả mong đợi: 8.0.xxx
```

---

## BƯỚC 2: Khởi tạo Frontend

### 2.1. Tạo project Vite + React + TypeScript

```powershell
cd "D:\bai tap lon\Đồ án 4- Đặt phòng\StayEasy"
npm create vite@latest client -- --template react-ts
```

### 2.2. Cài đặt dependencies

```powershell
cd client
npm install
```

### 2.3. Cài đặt thêm các thư viện cần thiết

```powershell
# React Router cho điều hướng
npm install react-router-dom

# Axios cho gọi API
npm install axios

# Tailwind CSS (tùy chọn, cho styling nhanh)
npm install -D tailwindcss postcss autoprefixer
npx tailwindcss init -p
```

### 2.4. Kiểm tra

```powershell
npm run dev
# Chạy tại: http://localhost:5173
```

---

## BƯỚC 3: Khởi tạo Backend

### 3.1. Tạo project ASP.NET Core Web API

```powershell
cd "D:\bai tap lon\Đồ án 4- Đặt phòng\StayEasy"
dotnet new webapi -n server
cd server
```

### 3.2. Cài đặt các package cần thiết

```powershell
# Entity Framework Core cho MySQL
dotnet add package Pomelo.EntityFrameworkCore.MySql

# EF Core Tools cho migration
dotnet add package Microsoft.EntityFrameworkCore.Design

# JWT Authentication
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer

# Swagger cho API documentation
dotnet add package Swashbuckle.AspNetCore
```

### 3.3. Cấu hình trong Program.cs

```csharp
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using server.Data;

var builder = WebApplication.CreateBuilder(args);

// ============================================================================
// DATABASE CONTEXT
// ============================================================================
builder.Services.AddDbContext<StayEasyDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));

// ============================================================================
// JWT AUTHENTICATION
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
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

### 3.4. Cấu hình appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=StayEasy;User=stayeasy;Password=stayeasy123;"
  },
  "Jwt": {
    "Secret": "your-super-secret-key-change-in-production-min-32-chars",
    "Issuer": "StayEasy",
    "Audience": "StayEasyUsers",
    "ExpirationHours": 24
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

---

## BƯỚC 4: Cấu hình Database

### 4.1. Tạo migration đầu tiên

```powershell
cd server
dotnet ef migrations add InitialCreate
```

### 4.2. Áp dụng migration (tạo database)

```powershell
dotnet ef database update
```

### 4.3. Kiểm tra

```powershell
# Kiểm tra các migration
dotnet ef migrations list
```

---

## BƯỚC 5: Chạy và kiểm tra

### 5.1. Chạy Backend

```powershell
cd server
dotnet run
# Chạy tại: http://localhost:5000
# Swagger: http://localhost:5000/swagger
```

### 5.2. Chạy Frontend

```powershell
cd client
npm run dev
# Chạy tại: http://localhost:5173
```

### 5.3. Kiểm tra Docker (tùy chọn)

```powershell
# Build và chạy toàn bộ bằng Docker
docker compose up -d

# Kiểm tra
docker compose ps
docker compose logs -f
```

---

## Xử lý lỗi thường gặp

### Lỗi: "dotnet" không được nhận dạng
**Nguyên nhân**: Chưa cài .NET SDK hoặc chưa khởi đột lại terminal
**Cách sửa**: Khởi đột lại terminal hoặc máy tính

### Lỗi: Không kết nối được MySQL
**Nguyên nhân**: MySQL chưa chạy hoặc sai thông tin kết nối
**Cách sửa**: Kiểm tra `appsettings.json` và đảm bảo MySQL đang chạy

### Lỗi: CORS
**Nguyên nhân**: Frontend gọi API bị chặn
**Cách sửa**: Kiểm tra cấu hình CORS trong `Program.cs`

### Lỗi: JWT token hết hạn
**Nguyên nhân**: Token đã hết hạn
**Cách sửa**: Đăng nhập lại để nhận token mới

---

## Cấu trúc project hoàn chỉnh

```
StayEasy/
├── client/
│   ├── src/
│   │   ├── components/
│   │   ├── pages/
│   │   ├── services/
│   │   ├── types/
│   │   └── utils/
│   ├── Dockerfile
│   ├── nginx.conf
│   └── package.json
├── server/
│   ├── Controllers/
│   ├── Services/
│   ├── Models/
│   ├── Data/
│   ├── Migrations/
│   ├── Dockerfile
│   ├── Program.cs
│   ├── appsettings.json
│   └── StayEasy.csproj
├── docs/
│   └── SETUP_GUIDE.md
├── docker-compose.yml
├── .env.example
├── .gitignore
└── README.md
```
