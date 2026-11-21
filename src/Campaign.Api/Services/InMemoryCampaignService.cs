using System.Collections.Concurrent;
using Campaign.Api.Models;

namespace Campaign.Api.Services;

public sealed class InMemoryCampaignService : ICampaignService
{
    private readonly ConcurrentDictionary<Guid, CampaignModel> _campaigns = new();

    public CampaignModel Create(string name, string content)
    {
        var campaign = new CampaignModel(
            Guid.NewGuid(),
            name,
            content,
            DateTimeOffset.UtcNow,
            "Draft");

        _campaigns.TryAdd(campaign.Id, campaign);
        return campaign;
    }

    public IReadOnlyCollection<CampaignModel> GetAll() => _campaigns.Values.ToArray();
}
