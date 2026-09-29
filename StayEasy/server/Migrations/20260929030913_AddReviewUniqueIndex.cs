using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StayEasy.Migrations
{
    /// <inheritdoc />
    public partial class AddReviewUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Tạo composite trước rồi mới xóa index cũ (MySQL cấm xóa index đang dùng cho FK)
            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UserId_RoomId",
                table: "Reviews",
                columns: new[] { "UserId", "RoomId" },
                unique: true);

            migrationBuilder.DropIndex(
                name: "IX_Reviews_UserId",
                table: "Reviews");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UserId",
                table: "Reviews",
                column: "UserId");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_UserId_RoomId",
                table: "Reviews");
        }
    }
}
