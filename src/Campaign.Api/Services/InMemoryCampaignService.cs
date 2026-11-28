using System.Collections.Concurrent;
using Campaign.Api.Models;

namespace Campaign.Api.Services;

public sealed class InMemoryCampaignService : ICampaignService
{
    private readonly ConcurrentDictionary<Guid, CampaignModel> _campaigns = new();
    private readonly ConcurrentDictionary<Guid, MessageModel> _messages = new();

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

    public CampaignModel? GetById(Guid id) =>
        _campaigns.TryGetValue(id, out var campaign) ? campaign : null;

    public IReadOnlyCollection<CampaignModel> GetAll() => _campaigns.Values.ToArray();

    public IReadOnlyCollection<MessageModel>? GetMessages(Guid campaignId)
    {
        if (!_campaigns.ContainsKey(campaignId))
        {
            return null;
        }

        return _messages.Values
            .Where(message => message.CampaignId == campaignId)
            .ToArray();
    }

    public MessageModel? CreateMessage(Guid campaignId, string content)
    {
        if (!_campaigns.ContainsKey(campaignId))
        {
            return null;
        }

        var message = new MessageModel(
            Guid.NewGuid(),
            campaignId,
            content,
            DateTimeOffset.UtcNow);

        _messages.TryAdd(message.Id, message);
        return message;
    }

    public MessageModel? GetMessageById(Guid id) =>
        _messages.TryGetValue(id, out var message) ? message : null;
}
