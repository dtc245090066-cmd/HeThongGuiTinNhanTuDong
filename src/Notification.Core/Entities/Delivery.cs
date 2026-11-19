using System.ComponentModel.DataAnnotations;
using Notification.Core.Enums;

namespace Notification.Core.Entities;

public class Delivery
{
    public int Ma { get; set; }

    public int MaChienDich { get; set; }

    public int MaNguoiNhan { get; set; }

    public DeliveryStatus TrangThai { get; set; } = DeliveryStatus.Pending;

    public bool DaDuaVaoHangDoi { get; set; }

    [StringLength(2000)]
    public string? ThongBaoLoiDayVaoHangDoi { get; set; }

    [Range(0, int.MaxValue)]
    public int SoLanThuLai { get; set; }

    public string? ThongBaoLoi { get; set; }

    public DateTimeOffset? NgayGui { get; set; }

    public DateTimeOffset NgayTao { get; set; } = DateTimeOffset.UtcNow;

    public Campaign Campaign { get; set; } = null!;

    public Recipient Recipient { get; set; } = null!;
}
