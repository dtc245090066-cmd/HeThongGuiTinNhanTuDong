namespace Notification.Api.DTOs;

public class StartCampaignResponse
{
    public int MaChienDich { get; set; }

    public string TenChienDich { get; set; } = string.Empty;

    public int TongSoNguoiNhan { get; set; }

    public int SoDeliveryDuocTao { get; set; }

    public int SoMessageDaDuaVaoQueue { get; set; }

    public int SoMessageChuaDuaVaoQueue { get; set; }

    public string TrangThai { get; set; } = string.Empty;

    public string ThongBao { get; set; } = string.Empty;
}
