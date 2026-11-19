namespace Notification.Api.DTOs;

public class CampaignResponse
{
    public int Ma { get; set; }

    public string TenChienDich { get; set; } = string.Empty;

    public string TieuDe { get; set; } = string.Empty;

    public string TrangThai { get; set; } = string.Empty;

    public DateTimeOffset NgayTao { get; set; }

    public int SoLuongNguoiNhan { get; set; }
}
