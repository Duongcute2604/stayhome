-- ============================================================================
-- STAYEASY - Chuyen du lieu tu schema cu (snake_case) sang schema EF (PascalCase)
-- ============================================================================
-- Van de: init_database.sql tao bang snake_case (users, rooms...),
--   nhung EF Core mac dinh dung PascalCase (Users, Rooms...).
--   Migration AddLocationAmenityFields da tao bang PascalCase (dang TRONG).
-- File nay: copy seed data sang bang EF roi xoa bang cu de khoi nham lan.
-- Cach dung (1 lan duy nhat tren DB dev):
--   Get-Content docs/database/migrate_lowercase_to_ef.sql -Raw `
--     | docker exec -i stayeasy-mysql mysql -u root -proot123
-- Sau nay: EF la source of truth (dotnet ef migrations), backend tu Migrate()
--   khi khoi dong (xem Program.cs).
-- ============================================================================

USE StayEasy;

-- Users / Locations / Amenities (khong phu thuoc)
INSERT IGNORE INTO Users (Id, Email, PasswordHash, FullName, Phone, Role, CreatedAt)
  SELECT id, email, password_hash, full_name, phone, role, created_at FROM users;

INSERT IGNORE INTO Locations (Id, Name, Address, Description, ImageUrl, CreatedAt)
  SELECT id, name, address, description, NULL, created_at FROM locations;

INSERT IGNORE INTO Amenities (Id, Name, Description, Icon, Category)
  SELECT id, name, description, NULL, NULL FROM amenities;

-- Rooms + lien quan
INSERT IGNORE INTO Rooms (Id, LocationId, Name, Description, PricePerHour, PricePerDay, Capacity, Status, CreatedAt)
  SELECT id, location_id, name, description, price_per_hour, price_per_day, capacity, status, created_at FROM rooms;

INSERT IGNORE INTO RoomImages (Id, RoomId, ImageUrl)
  SELECT id, room_id, image_url FROM room_images;

INSERT IGNORE INTO RoomAmenities (RoomId, AmenityId)
  SELECT room_id, amenity_id FROM room_amenities;

-- Bookings + lich su
INSERT IGNORE INTO Bookings (Id, UserId, RoomId, BookingType, CheckIn, CheckOut, TotalPrice, Status, CreatedAt)
  SELECT id, user_id, room_id, booking_type, check_in, check_out, total_price, status, created_at FROM bookings;

INSERT IGNORE INTO BookingStatusHistories (Id, BookingId, Status, Note, ChangedAt)
  SELECT id, booking_id, status, note, changed_at FROM booking_status_history;

-- Reviews
INSERT IGNORE INTO Reviews (Id, UserId, RoomId, Rating, Comment, CreatedAt)
  SELECT id, user_id, room_id, rating, comment, created_at FROM reviews;

INSERT IGNORE INTO ReviewImages (Id, ReviewId, ImageUrl)
  SELECT id, review_id, image_url FROM review_images;

-- Notifications
INSERT IGNORE INTO Notifications (Id, UserId, Title, Message, Type, IsRead, CreatedAt)
  SELECT id, user_id, title, message, type, is_read, created_at FROM notifications;

-- Xoa bang cu de khoi nham (da copy xong)
DROP TABLE IF EXISTS review_images;
DROP TABLE IF EXISTS reviews;
DROP TABLE IF EXISTS booking_status_history;
DROP TABLE IF EXISTS bookings;
DROP TABLE IF EXISTS room_amenities;
DROP TABLE IF EXISTS room_images;
DROP TABLE IF EXISTS rooms;
DROP TABLE IF EXISTS amenities;
DROP TABLE IF EXISTS locations;
DROP TABLE IF EXISTS notifications;
DROP TABLE IF EXISTS users;
