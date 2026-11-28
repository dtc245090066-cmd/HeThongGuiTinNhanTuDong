namespace Campaign.Api.Models;

public sealed record MessageModel(
    Guid Id,
    Guid CampaignId,
    string Content,
    DateTimeOffset CreatedAt);
