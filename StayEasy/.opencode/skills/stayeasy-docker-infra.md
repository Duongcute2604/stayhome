# stayeasy-docker-infra

## Skill quản lý hạ tầng và triển khai bằng Docker

### Khi nào sử dụng
- Cấu hình docker-compose.yml
- Quản lý biến môi trường (.env)
- Health checks và volume persistent
- Debug khi containers không hoạt động
- Deploy và rollback

---

## Docker Compose Configuration

### docker-compose.yml
```yaml
version: '3.8'

services:
  # Frontend - React + Vite + Nginx
  frontend:
    build:
      context: ./client
      dockerfile: Dockerfile
    ports:
      - "3000:80"
    depends_on:
      - backend
    networks:
      - stayeasy-network
    restart: unless-stopped

  # Backend - ASP.NET Core Web API
  backend:
    build:
      context: ./server
      dockerfile: Dockerfile
    ports:
      - "5000:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Server=mysql;Port=3306;Database=StayEasy;User=stayeasy;Password=stayeasy123;
      - Jwt__Secret=your-super-secret-key-change-in-production-min-32-chars
      - Jwt__Issuer=StayEasy
      - Jwt__Audience=StayEasyUsers
      - Jwt__ExpirationHours=24
      - Cors__AllowedOrigins__0=http://localhost:3000
      - Cors__AllowedOrigins__1=http://localhost:5173
    depends_on:
      mysql:
        condition: service_healthy
    networks:
      - stayeasy-network
    restart: unless-stopped

  # Database - MySQL 8.0
  mysql:
    image: mysql:8.0
    ports:
      - "3307:3306"
    environment:
      - MYSQL_ROOT_PASSWORD=root123
      - MYSQL_DATABASE=StayEasy
      - MYSQL_USER=stayeasy
      - MYSQL_PASSWORD=stayeasy123
    volumes:
      - mysql-data:/var/lib/mysql
      - ./docs/database/init_database.sql:/docker-entrypoint-initdb.d/init.sql
    healthcheck:
      test: ["CMD", "mysqladmin", "ping", "-h", "localhost", "-u", "root", "-proot123"]
      interval: 10s
      timeout: 5s
      retries: 5
    networks:
      - stayeasy-network
    restart: unless-stopped

networks:
  stayeasy-network:
    driver: bridge

volumes:
  mysql-data:
```

---

## Environment Variables

### .env.example
```bash
# Database
MYSQL_ROOT_PASSWORD=root123
MYSQL_DATABASE=StayEasy
MYSQL_USER=stayeasy
MYSQL_PASSWORD=stayeasy123

# Backend
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://+:80

# JWT
JWT_SECRET=your-super-secret-key-change-in-production-min-32-chars
JWT_ISSUER=StayEasy
JWT_AUDIENCE=StayEasyUsers
JWT_EXPIRATION_HOURS=24

# CORS
CORS_ALLOWED_ORIGINS=http://localhost:3000,http://localhost:5173

# Frontend
VITE_API_URL=http://localhost:5000/api
```

### Sử dụng .env
```bash
# Copy file mẫu
cp .env.example .env

# Chỉnh sửa file .env với thông tin thật
```

---

## Health Checks

### Backend Health Check
```csharp
// Program.cs
builder.Services.AddHealthChecks()
    .AddDbContextCheck<StayEasyDbContext>("database");

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        var result = report.Status == HealthStatus.Healthy
            ? "Healthy"
            : "Unhealthy";
        await context.Response.WriteAsync(result);
    }
});
```

### Docker Compose Health Check
```yaml
services:
  backend:
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:80/health"]
      interval: 30s
      timeout: 10s
      retries: 3
```

---

## Volume Persistent

### Named Volumes
```yaml
volumes:
  mysql-data:
    driver: local
```

### Backup/Restore
```bash
# Backup
docker exec stayeasy-mysql mysqldump -u root -proot123 StayEasy > backup_$(date +%Y%m%d).sql

# Restore
docker exec -i stayeasy-mysql mysql -u root -proot123 StayEasy < backup_20260929.sql
```

---

## Docker Commands

### Build & Run
```bash
# Build và start tất cả services
docker-compose up -d --build

# Xem logs
docker-compose logs -f

# Xem logs của service cụ thể
docker-compose logs -f backend

# Stop tất cả services
docker-compose down

# Stop và xóa volumes (cẩn thận!)
docker-compose down -v

# Restart service
docker-compose restart backend

# Rebuild service cụ thể
docker-compose up -d --build backend
```

### Debug
```bash
# Kiểm tra trạng thái containers
docker-compose ps

# Kiểm tra logs
docker-compose logs --tail=100

# Kết nối vào container
docker exec -it stayeasy-backend /bin/bash
docker exec -it stayeasy-mysql mysql -u root -proot123

# Kiểm tra network
docker network inspect stayeasy_stayeasy-network

# Kiểm tra volumes
docker volume ls
docker volume inspect stayeasy_mysql-data
```

---

## Dockerfile Patterns

### Frontend Dockerfile
```dockerfile
# Stage 1: Build
FROM node:20-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
RUN npm run build

# Stage 2: Serve with Nginx
FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
```

### Backend Dockerfile
```dockerfile
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 80
ENTRYPOINT ["dotnet", "server.dll"]
```

---

## Nginx Configuration

```nginx
server {
    listen 80;
    server_name localhost;

    root /usr/share/nginx/html;
    index index.html;

    # SPA routing
    location / {
        try_files $uri $uri/ /index.html;
    }

    # API proxy
    location /api/ {
        proxy_pass http://backend:80/;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection 'upgrade';
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
    }

    # Static files
    location ~* \.(js|css|png|jpg|jpeg|gif|ico|svg)$ {
        expires 1y;
        add_header Cache-Control "public, immutable";
    }
}
```

---

## Deployment Checklist

### Pre-deployment
- [ ] Test locally với `docker-compose up`
- [ ] Kiểm tra health checks
- [ ] Backup database hiện tại
- [ ] Chuẩn bị rollback plan

### Deployment
```bash
# Pull latest code
git pull origin main

# Build và deploy
docker-compose down
docker-compose up -d --build

# Kiểm tra
docker-compose ps
docker-compose logs -f
```

### Post-deployment
- [ ] Kiểm tra health endpoint
- [ ] Kiểm tra database connection
- [ ] Test critical flows (login, booking)
- [ ] Monitor logs

---

## Troubleshooting

### Container không start
```bash
# Kiểm tra logs
docker-compose logs service-name

# Kiểm tra config
docker-compose config

# Thử build lại
docker-compose build --no-cache service-name
```

### Database connection failed
```bash
# Kiểm tra MySQL container
docker exec -it stayeasy-mysql mysql -u root -proot123

# Kiểm tra network
docker network inspect stayeasy_stayeasy-network

# Kiểm tra environment variables
docker-compose exec backend env | grep Connection
```

### Port conflicts
```bash
# Kiểm tra port đang sử dụng
netstat -ano | findstr :3000
netstat -ano | findstr :5000
netstat -ano | findstr :3307

# Kill process sử dụng port
taskkill /PID <pid> /F
```
