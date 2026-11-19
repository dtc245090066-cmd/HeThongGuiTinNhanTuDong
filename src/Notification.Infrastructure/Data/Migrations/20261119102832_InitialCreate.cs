using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Notification.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Campaigns",
                columns: table => new
                {
                    Ma = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenChienDich = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TieuDe = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    NgayTao = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    NgayBatDau = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    NgayHoanThanh = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Campaigns", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "Recipients",
                columns: table => new
                {
                    Ma = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaChienDich = table.Column<int>(type: "int", nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DiaChiEmail = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                    NgayTao = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recipients", x => x.Ma);
                    table.UniqueConstraint("AK_Recipients_Ma_MaChienDich", x => new { x.Ma, x.MaChienDich });
                    table.ForeignKey(
                        name: "FK_Recipients_Campaigns_MaChienDich",
                        column: x => x.MaChienDich,
                        principalTable: "Campaigns",
                        principalColumn: "Ma",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Deliveries",
                columns: table => new
                {
                    Ma = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaChienDich = table.Column<int>(type: "int", nullable: false),
                    MaNguoiNhan = table.Column<int>(type: "int", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    SoLanThuLai = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    ThongBaoLoi = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    NgayGui = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    NgayTao = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deliveries", x => x.Ma);
                    table.CheckConstraint("CK_Deliveries_SoLanThuLai_NonNegative", "[SoLanThuLai] >= 0");
                    table.ForeignKey(
                        name: "FK_Deliveries_Campaigns_MaChienDich",
                        column: x => x.MaChienDich,
                        principalTable: "Campaigns",
                        principalColumn: "Ma",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Deliveries_Recipients_MaNguoiNhan_MaChienDich",
                        columns: x => new { x.MaNguoiNhan, x.MaChienDich },
                        principalTable: "Recipients",
                        principalColumns: new[] { "Ma", "MaChienDich" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_NgayTao",
                table: "Campaigns",
                column: "NgayTao");

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_TrangThai",
                table: "Campaigns",
                column: "TrangThai");

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_MaChienDich_TrangThai",
                table: "Deliveries",
                columns: new[] { "MaChienDich", "TrangThai" });

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_MaNguoiNhan_MaChienDich",
                table: "Deliveries",
                columns: new[] { "MaNguoiNhan", "MaChienDich" });

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_MaNguoiNhan_TrangThai",
                table: "Deliveries",
                columns: new[] { "MaNguoiNhan", "TrangThai" });

            migrationBuilder.CreateIndex(
                name: "IX_Recipients_MaChienDich_DiaChiEmail",
                table: "Recipients",
                columns: new[] { "MaChienDich", "DiaChiEmail" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Deliveries");

            migrationBuilder.DropTable(
                name: "Recipients");

            migrationBuilder.DropTable(
                name: "Campaigns");
        }
    }
}
