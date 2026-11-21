using System.ComponentModel.DataAnnotations;

namespace Campaign.Api.DTOs;

public sealed class CreateCampaignRequest
{
    [Required]
    public string Name { get; init; } = string.Empty;

    [Required]
    public string Content { get; init; } = string.Empty;
}
