# ============================================================================
# STAYEASY FRONTEND - DOCKERFILE
# ============================================================================
# Mục đích: Build và chạy ứng dụng React + TypeScript + Vite trong Docker
# Cách dùng: docker build -t stayeasy-frontend .
# ============================================================================

# ============================================================================
# STAGE 1: BUILD - Build ứng dụng React
# ============================================================================
# Tại sao cần 2 stage: Giảm kích thước image cuối cùng
# Stage 1 chỉ dùng để build, stage 2 chỉ chứa file tĩnh
# ============================================================================

# Sử dụng Node 20 Alpine (nhẹ, bảo mật)
FROM node:20-alpine AS builder

# Thiết lập thư mục làm việc trong container
WORKDIR /app

# Copy package.json và package-lock.json trước
# Tại sao: Tận dụng cache của Docker, chỉ cài lại khi dependencies thay đổi
COPY package*.json ./

# Cài đặt dependencies
# Tại sao dùng npm ci: Nhanh hơn npm install, đảm bảo nhất quán
RUN npm ci

# Copy toàn bộ source code
COPY . .

# Build ứng dụng cho production
# Tại sao: Tạo file tĩnh tối ưu, sẵn sàng serve
RUN npm run build

# ============================================================================
# STAGE 2: SERVE - Phục vụ file tĩnh
# ============================================================================
# Tại sao dùng Nginx: Nhẹ, nhanh, phù hợp serve file tĩnh
# ============================================================================

FROM nginx:alpine

# Copy file build từ stage 1 sang Nginx
# Tại sao: Chỉ lấy file tĩnh đã build, không cần Node.js
COPY --from=builder /app/dist /usr/share/nginx/html

# Copy cấu hình Nginx tùy chỉnh
# Tại sao: Cần cấu hình SPA routing (tất cả route về index.html)
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Expose port 80 (Nginx mặc định)
EXPOSE 80

# Khởi động Nginx
CMD ["nginx", "-g", "daemon off;"]
