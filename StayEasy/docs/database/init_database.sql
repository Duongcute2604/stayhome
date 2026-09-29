-- ============================================================================
-- STAYEASY - DATABASE INITIALIZATION SCRIPT
-- ============================================================================
-- Mục đích: Tạo database và các bảng cho hệ thống StayEasy
-- Cách dùng: 
--   1. Mở MySQL Workbench hoặc DBeaver
--   2. Kết nối đến MySQL server
--   3. Chạy script này
--   Hoặc dùng command line:
--   mysql -u root -p < init_database.sql
-- ============================================================================

-- ============================================================================
-- BƯỚC 1: TẠO DATABASE
-- ============================================================================
-- Tại sao: Tạo database riêng cho StayEasy, tránh xung đột với DB khác
-- ============================================================================

CREATE DATABASE IF NOT EXISTS StayEasy
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_unicode_ci;

USE StayEasy;

-- ============================================================================
-- BƯỚC 2: TẠO BẢNG USERS
-- ============================================================================
-- Mục đích: Lưu thông tin người dùng (Customer, Employee, Admin)
-- ============================================================================

CREATE TABLE IF NOT EXISTS users (
    id INT AUTO_INCREMENT PRIMARY KEY,
    email VARCHAR(255) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    full_name VARCHAR(255) NOT NULL,
    phone VARCHAR(20),
    role VARCHAR(50) NOT NULL DEFAULT 'Customer',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    INDEX idx_email (email),
    INDEX idx_role (role)
) ENGINE=InnoDB;

-- ============================================================================
-- BƯỚC 3: TẠO BẢNG LOCATIONS
-- ============================================================================
-- Mục đích: Lưu thông tin địa điểm homestay
-- ============================================================================

CREATE TABLE IF NOT EXISTS locations (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    address VARCHAR(500),
    description TEXT,
    image_url VARCHAR(500),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UNIQUE INDEX uq_locations_name (name)
) ENGINE=InnoDB;

-- ============================================================================
-- BƯỚC 4: TẠO BẢNG AMENITIES
-- ============================================================================
-- Mục đích: Lưu danh sách tiện nghi (WiFi, hồ bơi, máy lạnh, etc.)
-- ============================================================================

CREATE TABLE IF NOT EXISTS amenities (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    description VARCHAR(500),
    icon VARCHAR(50),
    category VARCHAR(50),
    UNIQUE INDEX uq_amenities_name (name)
) ENGINE=InnoDB;

-- ============================================================================
-- BƯỚC 5: TẠO BẢNG ROOMS
-- ============================================================================
-- Mục đích: Lưu thông tin phòng
-- ============================================================================

CREATE TABLE IF NOT EXISTS rooms (
    id INT AUTO_INCREMENT PRIMARY KEY,
    location_id INT NOT NULL,
    name VARCHAR(255) NOT NULL,
    description TEXT,
    price_per_hour DECIMAL(18,2) NOT NULL,
    price_per_day DECIMAL(18,2) NOT NULL,
    capacity INT NOT NULL,
    status VARCHAR(50) NOT NULL DEFAULT 'AVAILABLE',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (location_id) REFERENCES locations(id) ON DELETE RESTRICT,
    INDEX idx_location (location_id),
    INDEX idx_status (status),
    INDEX idx_price (price_per_day)
) ENGINE=InnoDB;

-- ============================================================================
-- BƯỚC 6: TẠO BẢNG ROOM_IMAGES
-- ============================================================================
-- Mục đích: Lưu hình ảnh phòng (1 phòng có nhiều ảnh)
-- ============================================================================

CREATE TABLE IF NOT EXISTS room_images (
    id INT AUTO_INCREMENT PRIMARY KEY,
    room_id INT NOT NULL,
    image_url VARCHAR(500) NOT NULL,
    
    FOREIGN KEY (room_id) REFERENCES rooms(id) ON DELETE CASCADE
) ENGINE=InnoDB;

-- ============================================================================
-- BƯỚC 7: TẠO BẢNG ROOM_AMENITIES (Many-to-Many)
-- ============================================================================
-- Mục đích: Liên kết phòng với tiện nghi (1 phòng có nhiều tiện nghi)
-- ============================================================================

CREATE TABLE IF NOT EXISTS room_amenities (
    room_id INT NOT NULL,
    amenity_id INT NOT NULL,
    
    PRIMARY KEY (room_id, amenity_id),
    FOREIGN KEY (room_id) REFERENCES rooms(id) ON DELETE CASCADE,
    FOREIGN KEY (amenity_id) REFERENCES amenities(id) ON DELETE CASCADE
) ENGINE=InnoDB;

-- ============================================================================
-- BƯỚC 8: TẠO BẢNG BOOKINGS
-- ============================================================================
-- Mục đích: Lưu thông tin đặt phòng
-- ============================================================================

CREATE TABLE IF NOT EXISTS bookings (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
    room_id INT NOT NULL,
    booking_type VARCHAR(50) NOT NULL DEFAULT 'DAILY',
    check_in DATETIME NOT NULL,
    check_out DATETIME NOT NULL,
    total_price DECIMAL(18,2) NOT NULL,
    status VARCHAR(50) NOT NULL DEFAULT 'PENDING',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE RESTRICT,
    FOREIGN KEY (room_id) REFERENCES rooms(id) ON DELETE RESTRICT,
    INDEX idx_user (user_id),
    INDEX idx_room (room_id),
    INDEX idx_status (status),
    INDEX idx_checkin (check_in),
    INDEX idx_checkout (check_out)
) ENGINE=InnoDB;

-- ============================================================================
-- BƯỚC 9: TẠO BẢNG BOOKING_STATUS_HISTORY
-- ============================================================================
-- Mục đích: Lưu lịch sử thay đổi trạng thái đặt phòng
-- ============================================================================

CREATE TABLE IF NOT EXISTS booking_status_history (
    id INT AUTO_INCREMENT PRIMARY KEY,
    booking_id INT NOT NULL,
    status VARCHAR(50) NOT NULL,
    note VARCHAR(500),
    changed_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (booking_id) REFERENCES bookings(id) ON DELETE CASCADE
) ENGINE=InnoDB;

-- ============================================================================
-- BƯỚC 10: TẠO BẢNG REVIEWS
-- ============================================================================
-- Mục đích: Lưu đánh giá của khách hàng về phòng
-- ============================================================================

CREATE TABLE IF NOT EXISTS reviews (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
    room_id INT NOT NULL,
    rating INT NOT NULL CHECK (rating >= 1 AND rating <= 5),
    comment TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    FOREIGN KEY (room_id) REFERENCES rooms(id) ON DELETE CASCADE,
    INDEX idx_room (room_id),
    INDEX idx_rating (rating)
) ENGINE=InnoDB;

-- ============================================================================
-- BƯỚC 11: TẠO BẢNG REVIEW_IMAGES
-- ============================================================================
-- Mục đích: Lưu hình ảnh đánh giá
-- ============================================================================

CREATE TABLE IF NOT EXISTS review_images (
    id INT AUTO_INCREMENT PRIMARY KEY,
    review_id INT NOT NULL,
    image_url VARCHAR(500) NOT NULL,
    
    FOREIGN KEY (review_id) REFERENCES reviews(id) ON DELETE CASCADE
) ENGINE=InnoDB;

-- ============================================================================
-- BƯỚC 12: TẠO BẢNG NOTIFICATIONS
-- ============================================================================
-- Mục đích: Lưu thông báo cho người dùng
-- ============================================================================

CREATE TABLE IF NOT EXISTS notifications (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
    title VARCHAR(255) NOT NULL,
    message TEXT,
    type VARCHAR(50) NOT NULL DEFAULT 'INFO',
    is_read BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    INDEX idx_user (user_id),
    INDEX idx_read (is_read)
) ENGINE=InnoDB;

-- ============================================================================
-- BƯỚC 13: INSERT DỮ LIỆU MẪU (SEED DATA)
-- ============================================================================
-- Mục đích: Tạo dữ liệu mẫu để test hệ thống
-- ============================================================================

-- 13.1. Users (mật khẩu: 123456)
INSERT INTO users (email, password_hash, full_name, phone, role) VALUES
('admin@stayeasy.com', '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', 'Admin StayEasy', '0901234567', 'Admin'),
('employee@stayeasy.com', '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', 'Nhân viên StayEasy', '0901234568', 'Employee'),
('customer1@gmail.com', '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', 'Nguyễn Văn A', '0901234569', 'Customer'),
('customer2@gmail.com', '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', 'Trần Thị B', '0901234570', 'Customer');

-- 13.2. Locations
INSERT INTO locations (name, address, description) VALUES
('Hà Nội - Hoàn Kiếm', '123 Phố Cổ, Hoàn Kiếm, Hà Nội', 'Khu vực trung tâm Hà Nội, gần Hồ Gươm'),
('Hà Nội - Ba Đình', '456 Đội Cấn, Ba Đình, Hà Nội', 'Khu vực yên tĩnh, gần Quảng trường Ba Đình'),
('TP.HCM - Quận 1', '789 Nguyễn Huệ, Quận 1, TP.HCM', 'Trung tâm TP.HCM, gần Bến Nhà Rồng'),
('Đà Nẵng - Hải Châu', '321 Trần Phú, Hải Châu, Đà Nẵng', 'Gần biển Mỹ Khê, khu vực du lịch');

-- 13.3. Amenities
INSERT INTO amenities (name, description) VALUES
('WiFi', 'Internet tốc độ cao'),
('Máy lạnh', 'Điều hòa nhiệt độ'),
('Tủ lạnh', 'Tủ lạnh mini'),
('TV', 'TV màn hình phẳng'),
('Máy giặt', 'Máy giặt tự phục vụ'),
('Bồi tắm', 'Bồi tắm riêng'),
('Hồ bơi', 'Hồ bơi chung'),
('Bãi đỗ xe', 'Bãi đỗ xe miễn phí'),
('Phòng tập', 'Phòng tập gym'),
('Nhà hàng', 'Nhà hàng phục vụ ăn sáng');

-- 13.4. Rooms
INSERT INTO rooms (location_id, name, description, price_per_hour, price_per_day, capacity, status) VALUES
(1, 'Phòng Standard 101', 'Phòng tiêu chuẩn với giường đôi, view thành phố', 150000, 800000, 2, 'AVAILABLE'),
(1, 'Phòng Deluxe 102', 'Phòng cao cấp với ban công, view Hồ Gươm', 200000, 1200000, 2, 'AVAILABLE'),
(1, 'Phòng Suite 103', 'Phòng suite rộng rãi, phòng khách riêng', 300000, 1800000, 4, 'AVAILABLE'),
(2, 'Phòng Standard 201', 'Phòng tiêu chuẩn gần Quảng trường Ba Đình', 120000, 700000, 2, 'AVAILABLE'),
(2, 'Phòng Deluxe 202', 'Phòng cao cấp với view công viên', 180000, 1000000, 2, 'AVAILABLE'),
(3, 'Phòng Standard 301', 'Phòng tiêu chuẩn trung tâm Quận 1', 130000, 750000, 2, 'AVAILABLE'),
(3, 'Phòng Deluxe 302', 'Phòng cao cấp gần Bến Nhà Rồng', 190000, 1100000, 2, 'AVAILABLE'),
(4, 'Phòng Standard 401', 'Phòng tiêu chuẩn gần biển Mỹ Khê', 140000, 850000, 2, 'AVAILABLE'),
(4, 'Phòng Deluxe 402', 'Phòng cao cấp view biển', 220000, 1300000, 2, 'AVAILABLE'),
(4, 'Phòng Suite 403', 'Phòng suite view biển, có bồi tắm', 350000, 2000000, 4, 'AVAILABLE');

-- 13.5. Room Images
INSERT INTO room_images (room_id, image_url) VALUES
(1, 'https://example.com/rooms/101_1.jpg'),
(1, 'https://example.com/rooms/101_2.jpg'),
(2, 'https://example.com/rooms/102_1.jpg'),
(2, 'https://example.com/rooms/102_2.jpg'),
(3, 'https://example.com/rooms/103_1.jpg'),
(4, 'https://example.com/rooms/201_1.jpg'),
(5, 'https://example.com/rooms/202_1.jpg'),
(6, 'https://example.com/rooms/301_1.jpg'),
(7, 'https://example.com/rooms/302_1.jpg'),
(8, 'https://example.com/rooms/401_1.jpg'),
(9, 'https://example.com/rooms/402_1.jpg'),
(10, 'https://example.com/rooms/403_1.jpg');

-- 13.6. Room Amenities
INSERT INTO room_amenities (room_id, amenity_id) VALUES
(1, 1), (1, 2), (1, 3), (1, 4),  -- Standard 101: WiFi, AC, TV, Tủ lạnh
(2, 1), (2, 2), (2, 3), (2, 4), (2, 8),  -- Deluxe 102: + Bãi đỗ xe
(3, 1), (3, 2), (3, 3), (3, 4), (3, 6), (3, 8), (3, 9),  -- Suite 103: + Bồi tắm, Gym
(4, 1), (4, 2), (4, 4),
(5, 1), (5, 2), (5, 4), (5, 8),
(6, 1), (6, 2), (6, 3), (6, 4),
(7, 1), (7, 2), (7, 4), (7, 8),
(8, 1), (8, 2), (8, 4),
(9, 1), (9, 2), (9, 4), (9, 6), (9, 8),
(10, 1), (10, 2), (10, 3), (10, 4), (10, 6), (10, 7), (10, 8), (10, 9), (10, 10);  -- Suite 403: đầy đủ

-- 13.7. Bookings (dữ liệu mẫu)
INSERT INTO bookings (user_id, room_id, booking_type, check_in, check_out, total_price, status) VALUES
(3, 1, 'DAILY', '2026-10-01 14:00:00', '2026-10-03 12:00:00', 1600000, 'CONFIRMED'),
(3, 2, 'HOURLY', '2026-10-05 10:00:00', '2026-10-05 14:00:00', 800000, 'PENDING'),
(4, 3, 'DAILY', '2026-10-02 14:00:00', '2026-10-04 12:00:00', 3600000, 'CONFIRMED'),
(4, 1, 'DAILY', '2026-10-10 14:00:00', '2026-10-12 12:00:00', 1600000, 'PENDING');

-- 13.8. Booking Status History
INSERT INTO booking_status_history (booking_id, status, note) VALUES
(1, 'PENDING', 'Tạo đặt phòng mới'),
(1, 'CONFIRMED', 'Admin xác nhận đặt phòng'),
(2, 'PENDING', 'Tạo đặt phòng mới'),
(3, 'PENDING', 'Tạo đặt phòng mới'),
(3, 'CONFIRMED', 'Admin xác nhận đặt phòng'),
(4, 'PENDING', 'Tạo đặt phòng mới');

-- 13.9. Reviews
INSERT INTO reviews (user_id, room_id, rating, comment) VALUES
(3, 1, 5, 'Phòng sạch sẽ, nhân viên nhiệt tình, view đẹp!'),
(3, 2, 4, 'Phòng đẹp, giá hơi cao nhưng đáng đồng tiền'),
(4, 3, 5, 'Phòng suite rất rộng, view Hồ Gươm tuyệt vời'),
(4, 1, 4, 'Phòng ổn, sẽ quay lại lần sau');

-- 13.10. Review Images
INSERT INTO review_images (review_id, image_url) VALUES
(1, 'https://example.com/reviews/1_1.jpg'),
(1, 'https://example.com/reviews/1_2.jpg'),
(3, 'https://example.com/reviews/3_1.jpg');

-- 13.11. Notifications
INSERT INTO notifications (user_id, title, message, type) VALUES
(3, 'Đặt phòng thành công', 'Bạn đã đặt phòng Standard 101 thành công', 'INFO'),
(3, 'Xác nhận đặt phòng', 'Đặt phòng Deluxe 102 đã được xác nhận', 'INFO'),
(4, 'Chào mừng', 'Cảm ơn bạn đã đăng ký StayEasy', 'INFO');

-- ============================================================================
-- HOÀN TẤT
-- ============================================================================
-- Tổng số bảng: 11
-- Tổng số dữ liệu mẫu:
--   - Users: 4
--   - Locations: 4
--   - Amenities: 10
--   - Rooms: 10
--   - Room Images: 12
--   - Room Amenities: 30
--   - Bookings: 4
--   - Booking Status History: 6
--   - Reviews: 4
--   - Review Images: 3
--   - Notifications: 3
-- ============================================================================
