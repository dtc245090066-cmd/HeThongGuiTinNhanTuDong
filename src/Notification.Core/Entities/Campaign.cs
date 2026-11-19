using System.ComponentModel.DataAnnotations;
using Notification.Core.Enums;

namespace Notification.Core.Entities;

public class Campaign
{
    public int Ma { get; set; }

    [Required]
    [StringLength(200)]
    public string TenChienDich { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string TieuDe { get; set; } = string.Empty;

    [Required]
    public string NoiDung { get; set; } = string.Empty;

    public CampaignStatus TrangThai { get; set; } = CampaignStatus.Draft;

    public DateTimeOffset NgayTao { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? NgayBatDau { get; set; }

    public DateTimeOffset? NgayHoanThanh { get; set; }

    public ICollection<Recipient> Recipients { get; set; } = new List<Recipient>();

    public ICollection<Delivery> Deliveries { get; set; } = new List<Delivery>();
}
