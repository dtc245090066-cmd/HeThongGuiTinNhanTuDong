using Microsoft.EntityFrameworkCore;
using Notification.Core.Entities;
using Notification.Core.Enums;

namespace Notification.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Campaign> Campaigns => Set<Campaign>();

    public DbSet<Recipient> Recipients => Set<Recipient>();

    public DbSet<Delivery> Deliveries => Set<Delivery>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Campaign>(entity =>
        {
            entity.ToTable("Campaigns");
            entity.HasKey(chienDich => chienDich.Ma);

            entity.Property(chienDich => chienDich.TenChienDich)
                .HasMaxLength(200)
                .IsRequired();
            entity.Property(chienDich => chienDich.TieuDe)
                .HasMaxLength(255)
                .IsRequired();
            entity.Property(chienDich => chienDich.NoiDung)
                .HasColumnType("nvarchar(max)")
                .IsRequired();
            entity.Property(chienDich => chienDich.TrangThai)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();
            entity.Property(chienDich => chienDich.NgayTao)
                .HasDefaultValueSql("SYSUTCDATETIME()");

            entity.HasIndex(chienDich => chienDich.TrangThai);
            entity.HasIndex(chienDich => chienDich.NgayTao);
        });

        modelBuilder.Entity<Recipient>(entity =>
        {
            entity.ToTable("Recipients");
            entity.HasKey(nguoiNhan => nguoiNhan.Ma);
            entity.HasAlternateKey(nguoiNhan => new { nguoiNhan.Ma, nguoiNhan.MaChienDich });

            entity.Property(nguoiNhan => nguoiNhan.HoTen)
                .HasMaxLength(200)
                .IsRequired();
            entity.Property(nguoiNhan => nguoiNhan.DiaChiEmail)
                .HasMaxLength(320)
                .IsRequired();
            entity.Property(nguoiNhan => nguoiNhan.NgayTao)
                .HasDefaultValueSql("SYSUTCDATETIME()");

            entity.HasIndex(nguoiNhan => new { nguoiNhan.MaChienDich, nguoiNhan.DiaChiEmail });

            entity.HasOne(nguoiNhan => nguoiNhan.Campaign)
                .WithMany(chienDich => chienDich.Recipients)
                .HasForeignKey(nguoiNhan => nguoiNhan.MaChienDich)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Delivery>(entity =>
        {
            entity.ToTable("Deliveries", bang =>
                bang.HasCheckConstraint(
                    "CK_Deliveries_SoLanThuLai_NonNegative",
                    "[SoLanThuLai] >= 0"));
            entity.HasKey(lanGui => lanGui.Ma);

            entity.Property(lanGui => lanGui.TrangThai)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();
            entity.Property(lanGui => lanGui.DaDuaVaoHangDoi)
                .HasDefaultValue(false);
            entity.Property(lanGui => lanGui.SoLanThuLai)
                .HasDefaultValue(0);
            entity.Property(lanGui => lanGui.ThongBaoLoi)
                .HasMaxLength(2000);
            entity.Property(lanGui => lanGui.ThongBaoLoiDayVaoHangDoi)
                .HasMaxLength(2000);
            entity.Property(lanGui => lanGui.NgayTao)
                .HasDefaultValueSql("SYSUTCDATETIME()");

            entity.HasIndex(lanGui => new { lanGui.MaChienDich, lanGui.MaNguoiNhan })
                .IsUnique();
            entity.HasIndex(lanGui => new { lanGui.MaChienDich, lanGui.TrangThai });
            entity.HasIndex(lanGui => new { lanGui.MaNguoiNhan, lanGui.TrangThai });

            entity.HasOne(lanGui => lanGui.Campaign)
                .WithMany(chienDich => chienDich.Deliveries)
                .HasForeignKey(lanGui => lanGui.MaChienDich)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(lanGui => lanGui.Recipient)
                .WithMany(nguoiNhan => nguoiNhan.Deliveries)
                .HasForeignKey(lanGui => new { lanGui.MaNguoiNhan, lanGui.MaChienDich })
                .HasPrincipalKey(nguoiNhan => new { nguoiNhan.Ma, nguoiNhan.MaChienDich })
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
