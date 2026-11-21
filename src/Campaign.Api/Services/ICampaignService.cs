using Campaign.Api.Models;

namespace Campaign.Api.Services;

public interface ICampaignService
{
    CampaignModel Create(string name, string content);

    IReadOnlyCollection<CampaignModel> GetAll();
}
