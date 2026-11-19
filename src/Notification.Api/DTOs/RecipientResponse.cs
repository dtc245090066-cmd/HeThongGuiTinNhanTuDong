namespace Notification.Api.DTOs;

public class RecipientResponse
{
    public int Ma { get; set; }

    public string HoTen { get; set; } = string.Empty;

    public string DiaChiEmail { get; set; } = string.Empty;

    public DateTimeOffset NgayTao { get; set; }
}
