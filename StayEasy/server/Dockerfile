# ============================================================================
# STAYEASY BACKEND - DOCKERFILE
# ============================================================================
# Mục đích: Build và chạy ứng dụng ASP.NET Core Web API trong Docker
# Cách dùng: docker build -t stayeasy-backend .
# ============================================================================

# ============================================================================
# STAGE 1: BUILD - Build ứng dụng .NET
# ============================================================================
# Tại sao cần 2 stage: Giảm kích thước image cuối cùng
# ============================================================================

# Sử dụng SDK image để build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Thiết lập thư mục làm việc
WORKDIR /src

# Copy project file trước
# Tại sao: Tận dụng cache của Docker
COPY *.csproj ./
RUN dotnet restore

# Copy toàn bộ source code
COPY . .

# Build ứng dụng
# Tại sao dùng Release: Tối ưu hiệu năng, nhỏ hơn Debug
RUN dotnet build -c Release -o /app/build

# ============================================================================
# STAGE 2: PUBLISH - Publish ứng dụng
# ============================================================================
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# ============================================================================
# STAGE 3: RUNTIME - Chạy ứng dụng
# ============================================================================
# Tại sao dùng aspnet: Image nhẹ hơn SDK, chỉ chứa runtime
# ============================================================================

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Thiết lập thư mục làm việc
WORKDIR /app

# Copy file publish từ stage trước
COPY --from=publish /app/publish .

# Expose port 5000
EXPOSE 5000

# Biến môi trường
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

# Khởi động ứng dụng
ENTRYPOINT ["dotnet", "StayEasy.dll"]
