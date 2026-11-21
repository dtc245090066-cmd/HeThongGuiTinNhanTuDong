using Campaign.Api.Models;

namespace Campaign.Api.DTOs;

public sealed record CampaignResponse(
    Guid Id,
    string Name,
    string Content,
    DateTimeOffset CreatedAt,
    string Status)
{
    public static CampaignResponse FromModel(CampaignModel campaign) =>
        new(campaign.Id, campaign.Name, campaign.Content, campaign.CreatedAt, campaign.Status);
}
