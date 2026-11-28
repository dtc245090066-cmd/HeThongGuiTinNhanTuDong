using Campaign.Api.Models;

namespace Campaign.Api.Services;

public interface ICampaignService
{
    CampaignModel Create(string name, string content);

    CampaignModel? GetById(Guid id);

    IReadOnlyCollection<CampaignModel> GetAll();

    IReadOnlyCollection<MessageModel>? GetMessages(Guid campaignId);

    MessageModel? CreateMessage(Guid campaignId, string content);

    MessageModel? GetMessageById(Guid id);
}
