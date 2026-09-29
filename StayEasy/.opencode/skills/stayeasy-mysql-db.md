# stayeasy-mysql-db

## Skill quản lý Cơ sở dữ liệu MySQL & Entity Framework Core

### Khi nào sử dụng
- Cấu hình kết nối MySQL với Pomelo.EntityFrameworkCore.MySql
- Tạo và chạy EF Core migrations
- Tối ưu hóa index cho các bảng
- Xử lý ràng buộc khóa ngoại (Foreign Keys)
- Seeding data mẫu

---

## Cấu hình kết nối MySQL

### appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3307;Database=StayEasy;User=stayeasy;Password=stayeasy123;"
  }
}
```

### Program.cs
```csharp
builder.Services.AddDbContext<StayEasyDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));
```

---

## EF Core Migration Pattern

### Tạo migration
```bash
# Tạo migration mới
dotnet ef migrations add MigrationName

# Chạy migration
dotnet ef database update

# Xóa migration cuối cùng
dotnet ef migrations remove

# Xem danh sách migrations
dotnet ef migrations list
```

### Migration File Structure
```csharp
public partial class AddLocationAndAmenity : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Tạo bảng mới
        migrationBuilder.CreateTable(
            name: "locations",
            columns: table => new
            {
                id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", 
                        MySqlValueGenerationStrategy.IdentityColumn),
                name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                address = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable = true),
                description = table.Column<string>(type: "longtext", nullable = true),
                created_at = table.Column<DateTime>(type: "datetime", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_locations")
                    .HasName("PRIMARY");
            });

        // Thêm index
        migrationBuilder.CreateIndex(
            name: "IX_locations_name",
            table: "locations",
            column: "name");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Xóa bảng
        migrationBuilder.DropTable(name: "locations");
    }
}
```

---

## Index Optimization

### Các cần index
```sql
-- Bảng rooms
CREATE INDEX idx_rooms_location_id ON rooms(location_id);
CREATE INDEX idx_rooms_status ON rooms(status);
CREATE INDEX idx_rooms_price_per_day ON rooms(price_per_day);

-- Bảng bookings
CREATE INDEX idx_bookings_user_id ON bookings(user_id);
CREATE INDEX idx_bookings_room_id ON bookings(room_id);
CREATE INDEX idx_bookings_status ON bookings(status);
CREATE INDEX idx_bookings_check_in ON bookings(check_in);
CREATE INDEX idx_bookings_check_out ON bookings(check_out);

-- Bảng users
CREATE INDEX idx_users_email ON users(email);
CREATE INDEX idx_users_role ON users(role);
```

### Composite Index
```sql
-- Index cho tìm kiếm phòng theo location + status
CREATE INDEX idx_rooms_location_status ON rooms(location_id, status);

-- Index cho tìm kiếm booking theo room + thời gian
CREATE INDEX idx_bookings_room_time ON bookings(room_id, check_in, check_out);
```

---

## Foreign Key Constraints

### Cascade Delete
```csharp
// Khi xóa Room, tự động xóa RoomImages
modelBuilder.Entity<RoomImage>(entity =>
{
    entity.HasOne(e => e.Room)
        .WithMany(e => e.RoomImages)
        .HasForeignKey(e => e.RoomId)
        .OnDelete(DeleteBehavior.Cascade);
});
```

### Restrict Delete
```csharp
// Không cho xóa Location nếu còn Room
modelBuilder.Entity<Room>(entity =>
{
    entity.HasOne(e => e.Location)
        .WithMany()
        .HasForeignKey(e => e.LocationId)
        .OnDelete(DeleteBehavior.Restrict);
});
```

### Set Null
```csharp
// Khi xóa Location, set LocationId = null cho Room
modelBuilder.Entity<Room>(entity =>
{
    entity.HasOne(e => e.Location)
        .WithMany()
        .HasForeignKey(e => e.LocationId)
        .OnDelete(DeleteBehavior.SetNull);
});
```

---

## Seeding Data Pattern

### Trong DbContext
```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    // Seed Locations
    modelBuilder.Entity<Location>().HasData(
        new Location { Id = 1, Name = "Hà Nội - Hoàn Kiếm", Address = "123 Phố Cổ, Hoàn Kiếm, Hà Nội" },
        new Location { Id = 2, Name = "Hà Nội - Ba Đình", Address = "456 Đội Cấn, Ba Đình, Hà Nội" },
        new Location { Id = 3, Name = "TP.HCM - Quận 1", Address = "789 Nguyễn Huệ, Quận 1, TP.HCM" }
    );

    // Seed Amenities
    modelBuilder.Entity<Amenity>().HasData(
        new Amenity { Id = 1, Name = "WiFi", Description = "Internet tốc độ cao" },
        new Amenity { Id = 2, Name = "Máy lạnh", Description = "Điều hòa nhiệt độ" },
        new Amenity { Id = 3, Name = "Tủ lạnh", Description = "Tủ lạnh mini" }
    );

    // Seed Users (mật khẩu: 123456)
    modelBuilder.Entity<User>().HasData(
        new User 
        { 
            Id = 1, 
            Email = "admin@stayeasy.com", 
            PasswordHash = "8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92",
            FullName = "Admin StayEasy",
            Role = "Admin"
        }
    );
}
```

### Trong Migration
```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    // Tạo bảng
    modelBuilder.CreateTable(...);

    // Seed data
    migrationBuilder.InsertData(
        table: "locations",
        columns: new[] { "id", "name", "address" },
        values: new object[,]
        {
            { 1, "Hà Nội - Hoàn Kiếm", "123 Phố Cổ, Hoàn Kiếm, Hà Nội" },
            { 2, "Hà Nội - Ba Đình", "456 Đội Cấn, Ba Đình, Hà Nội" }
        });
}
```

---

## Query Optimization

### Eager Loading
```csharp
// Include related entities
var rooms = await _context.Rooms
    .Include(r => r.Location)
    .Include(r => r.RoomImages)
    .Include(r => r.RoomAmenities)
        .ThenInclude(ra => ra.Amenity)
    .ToListAsync();
```

### Projection
```csharp
// Chỉ lấy các trường cần thiết
var roomDtos = await _context.Rooms
    .Select(r => new RoomDto
    {
        Id = r.Id,
        Name = r.Name,
        LocationName = r.Location.Name,
        PricePerDay = r.PricePerDay
    })
    .ToListAsync();
```

### Raw SQL
```csharp
// Query phức tạp
var results = await _context.Bookings
    .FromSqlRaw(@"
        SELECT b.*, u.full_name as user_name, r.name as room_name
        FROM bookings b
        JOIN users u ON b.user_id = u.id
        JOIN rooms r ON b.room_id = r.id
        WHERE b.status = 'CONFIRMED'
        AND b.check_in >= {0}", DateTime.Now)
    .ToListAsync();
```

---

## Database Schema

### Bảng chính
| Bảng | Mô tả | Index chính |
|------|-------|-------------|
| users | Người dùng | email, role |
| locations | Địa điểm | name |
| rooms | Phòng | location_id, status, price_per_day |
| bookings | Đặt phòng | user_id, room_id, status, check_in |
| amenities | Tiện nghi | name |
| room_amenities | Liên kết phòng-tiện nghi | room_id, amenity_id |
| reviews | Đánh giá | room_id, user_id, rating |
| notifications | Thông báo | user_id, is_read |

---

## Docker MySQL Commands

```bash
# Kết nối đến MySQL container
docker exec -it stayeasy-mysql mysql -u root -proot123

# Tạo database
CREATE DATABASE IF NOT EXISTS StayEasy;

# Chạy script SQL
docker exec -i stayeasy-mysql mysql -u root -proot123 < init_database.sql

# Backup database
docker exec stayeasy-mysql mysqldump -u root -proot123 StayEasy > backup.sql

# Restore database
docker exec -i stayeasy-mysql mysql -u root -proot123 StayEasy < backup.sql
```
