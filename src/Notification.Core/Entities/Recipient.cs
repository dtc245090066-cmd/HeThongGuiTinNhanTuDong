using System.ComponentModel.DataAnnotations;

namespace Notification.Core.Entities;

public class Recipient
{
    public int Ma { get; set; }

    public int MaChienDich { get; set; }

    [Required]
    [StringLength(200)]
    public string HoTen { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(320)]
    public string DiaChiEmail { get; set; } = string.Empty;

    public DateTimeOffset NgayTao { get; set; } = DateTimeOffset.UtcNow;

    public Campaign Campaign { get; set; } = null!;

    public ICollection<Delivery> Deliveries { get; set; } = new List<Delivery>();
}
