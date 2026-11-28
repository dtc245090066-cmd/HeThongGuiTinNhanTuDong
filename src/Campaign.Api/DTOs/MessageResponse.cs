using Campaign.Api.Models;

namespace Campaign.Api.DTOs;

public sealed record MessageResponse(
    Guid Id,
    Guid CampaignId,
    string Content,
    DateTimeOffset CreatedAt)
{
    public static MessageResponse FromModel(MessageModel message) =>
        new(message.Id, message.CampaignId, message.Content, message.CreatedAt);
}
