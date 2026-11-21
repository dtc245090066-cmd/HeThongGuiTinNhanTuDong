namespace Campaign.Api.Models;

public sealed record CampaignModel(
    Guid Id,
    string Name,
    string Content,
    DateTimeOffset CreatedAt,
    string Status);
