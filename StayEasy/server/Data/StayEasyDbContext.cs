using Microsoft.EntityFrameworkCore;
using server.Models.Entities;

namespace server.Data
{
    // ============================================================================
    // DATABASE CONTEXT - Quản lý kết nối và các bảng trong database
    // ============================================================================
    // Tại sao cần: Là cầu nối giữa code và database
    // ============================================================================
    public class StayEasyDbContext : DbContext
    {
        public StayEasyDbContext(DbContextOptions<StayEasyDbContext> options)
            : base(options)
        {
        }

        // Các bảng trong database
        public DbSet<User> Users { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomImage> RoomImages { get; set; }
        public DbSet<Amenity> Amenities { get; set; }
        public DbSet<RoomAmenity> RoomAmenities { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingStatusHistory> BookingStatusHistories { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ReviewImage> ReviewImages { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ============================================================================
            // USER CONFIGURATION
            // ============================================================================
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.Role).IsRequired().HasMaxLength(50);
            });

            // ============================================================================
            // ROOM CONFIGURATION
            // ============================================================================
            modelBuilder.Entity<Room>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Description).HasMaxLength(2000);
                entity.Property(e => e.PricePerHour).HasPrecision(18, 2);
                entity.Property(e => e.PricePerDay).HasPrecision(18, 2);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(50);

                // Foreign key: Room → Location
                entity.HasOne(e => e.Location)
                    .WithMany()
                    .HasForeignKey(e => e.LocationId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ============================================================================
            // BOOKING CONFIGURATION
            // ============================================================================
            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TotalPrice).HasPrecision(18, 2);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
                entity.Property(e => e.BookingType).IsRequired().HasMaxLength(50);

                // Foreign key: Booking → User
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Foreign key: Booking → Room
                entity.HasOne(e => e.Room)
                    .WithMany()
                    .HasForeignKey(e => e.RoomId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ============================================================================
            // AMENITY CONFIGURATION
            // ============================================================================
            modelBuilder.Entity<Amenity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                entity.HasIndex(e => e.Name).IsUnique(); // Không cho trùng tên tiện nghi
            });

            // ============================================================================
            // LOCATION CONFIGURATION
            // ============================================================================
            modelBuilder.Entity<Location>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                entity.HasIndex(e => e.Name).IsUnique(); // Không cho trùng tên địa điểm
            });

            // ============================================================================
            // ROOM-AMENITY (Many-to-Many)
            // ============================================================================
            // Room → RoomAmenity: Cascade (xóa phòng thì xóa link)
            // Amenity → RoomAmenity: Restrict (không cho xóa tiện nghi đang gán)
            // ============================================================================
            modelBuilder.Entity<RoomAmenity>(entity =>
            {
                entity.HasKey(e => new { e.RoomId, e.AmenityId });

                entity.HasOne(e => e.Room)
                    .WithMany(e => e.RoomAmenities)
                    .HasForeignKey(e => e.RoomId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Amenity)
                    .WithMany(e => e.RoomAmenities)
                    .HasForeignKey(e => e.AmenityId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ============================================================================
            // REVIEW CONFIGURATION - 1 user chỉ 1 review/phòng
            // ============================================================================
            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.UserId, e.RoomId }).IsUnique();
            });

            // ============================================================================
            // PAYMENT CONFIGURATION - 1 booking 1 payment (chống trả trùng ở DB)
            // ============================================================================
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.BookingId).IsUnique();
                entity.Property(e => e.Amount).HasPrecision(18, 2);
                entity.Property(e => e.RefundAmount).HasPrecision(18, 2);

                entity.HasOne(e => e.Booking)
                    .WithMany()
                    .HasForeignKey(e => e.BookingId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ============================================================================
            // INDEX TỐI ƯU SEARCH (theo skill stayeasy-mysql-db)
            // ============================================================================
            modelBuilder.Entity<Room>(entity =>
            {
                entity.HasIndex(e => new { e.LocationId, e.Status });
            });

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.Status);
                // Composite index chống trùng + tìm phòng trống nhanh
                entity.HasIndex(e => new { e.RoomId, e.CheckIn, e.CheckOut });
            });
        }
    }
}
