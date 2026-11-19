namespace Notification.Api.DTOs;

public class CampaignDetailResponse
{
    public int Ma { get; set; }

    public string TenChienDich { get; set; } = string.Empty;

    public string TieuDe { get; set; } = string.Empty;

    public string NoiDung { get; set; } = string.Empty;

    public string TrangThai { get; set; } = string.Empty;

    public DateTimeOffset NgayTao { get; set; }

    public DateTimeOffset? NgayBatDau { get; set; }

    public DateTimeOffset? NgayHoanThanh { get; set; }

    public int SoLuongNguoiNhan { get; set; }

    public List<RecipientResponse> NguoiNhans { get; set; } = [];
}
