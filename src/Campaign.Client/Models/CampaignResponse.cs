namespace Campaign.Client.Models;

public sealed record CampaignResponse(
    Guid Id,
    string Name,
    string Content,
    DateTimeOffset CreatedAt,
    string Status);
