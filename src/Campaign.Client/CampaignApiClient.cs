using System.Net.Http.Json;
using System.Text.Json;
using Campaign.Client.Models;

namespace Campaign.Client;

public sealed class CampaignApiClient(HttpClient httpClient)
{
    public async Task<IReadOnlyList<CampaignResponse>> GetCampaignsAsync(
        CancellationToken cancellationToken = default)
    {
        var campaigns = await httpClient.GetFromJsonAsync<List<CampaignResponse>>(
            "api/campaigns",
            cancellationToken);

        if (campaigns is null)
        {
            throw new JsonException("The API returned an empty campaign list response.");
        }

        return campaigns.Select(ValidateCampaign).ToArray();
    }

    public async Task<CampaignResponse> CreateCampaignAsync(
        CreateCampaignRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync(
            "api/campaigns",
            request,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new CampaignApiException(response.StatusCode, responseBody);
        }

        var campaign = await response.Content.ReadFromJsonAsync<CampaignResponse>(
            cancellationToken: cancellationToken);

        return ValidateCampaign(campaign);
    }

    private static CampaignResponse ValidateCampaign(CampaignResponse? campaign)
    {
        if (campaign is null
            || campaign.Id == Guid.Empty
            || string.IsNullOrWhiteSpace(campaign.Name)
            || string.IsNullOrWhiteSpace(campaign.Content)
            || campaign.CreatedAt == default
            || string.IsNullOrWhiteSpace(campaign.Status))
        {
            throw new JsonException("The API returned a campaign with missing or invalid fields.");
        }

        return campaign;
    }
}
