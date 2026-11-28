namespace Campaign.Client.Models;

public sealed record MessageResponse(
    Guid Id,
    Guid CampaignId,
    string Content,
    DateTimeOffset CreatedAt);
