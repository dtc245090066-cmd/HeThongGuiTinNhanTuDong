using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Notification.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDeliveryQueueTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DaDuaVaoHangDoi",
                table: "Deliveries",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ThongBaoLoiDayVaoHangDoi",
                table: "Deliveries",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_MaChienDich_MaNguoiNhan",
                table: "Deliveries",
                columns: new[] { "MaChienDich", "MaNguoiNhan" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Deliveries_MaChienDich_MaNguoiNhan",
                table: "Deliveries");

            migrationBuilder.DropColumn(
                name: "DaDuaVaoHangDoi",
                table: "Deliveries");

            migrationBuilder.DropColumn(
                name: "ThongBaoLoiDayVaoHangDoi",
                table: "Deliveries");
        }
    }
}
