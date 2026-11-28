using System.Net.Http.Json;
using System.Text.Json;
using Campaign.Client.Models;

namespace Campaign.Client;

public sealed class CampaignApiClient(HttpClient httpClient)
{
    public async Task<IReadOnlyList<CampaignResponse>> GetCampaignsAsync(
        CancellationToken cancellationToken = default)
    {
        var campaigns = await GetAsync<List<CampaignResponse>>("api/campaigns", cancellationToken);
        return campaigns.Select(ValidateCampaign).ToArray();
    }

    public async Task<CampaignResponse> GetCampaignAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        ValidateCampaign(await GetAsync<CampaignResponse>(
            $"api/campaigns/{id}",
            cancellationToken));

    public async Task<IReadOnlyList<MessageResponse>> GetMessagesAsync(
        Guid campaignId,
        CancellationToken cancellationToken = default)
    {
        var messages = await GetAsync<List<MessageResponse>>(
            $"api/campaigns/{campaignId}/messages",
            cancellationToken);
        return messages.Select(ValidateMessage).ToArray();
    }

    public async Task<CampaignResponse> CreateCampaignAsync(
        CreateCampaignRequest request,
        CancellationToken cancellationToken = default)
    {
        return ValidateCampaign(await PostAsync<CampaignResponse>(
            "api/campaigns",
            request,
            cancellationToken));
    }

    public async Task<MessageResponse> CreateMessageAsync(
        Guid campaignId,
        CreateMessageRequest request,
        CancellationToken cancellationToken = default) =>
        ValidateMessage(await PostAsync<MessageResponse>(
            $"api/campaigns/{campaignId}/messages",
            request,
            cancellationToken));

    public async Task<MessageResponse> GetMessageAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        ValidateMessage(await GetAsync<MessageResponse>(
            $"api/messages/{id}",
            cancellationToken));

    private async Task<T> GetAsync<T>(string requestUri, CancellationToken cancellationToken)
    {
        using var response = await httpClient.GetAsync(requestUri, cancellationToken);
        return await ReadResponseAsync<T>(response, cancellationToken);
    }

    private async Task<T> PostAsync<T>(
        string requestUri,
        object request,
        CancellationToken cancellationToken)
    {
        using var response = await httpClient.PostAsJsonAsync(requestUri, request, cancellationToken);
        return await ReadResponseAsync<T>(response, cancellationToken);
    }

    private static async Task<T> ReadResponseAsync<T>(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        if (!response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new CampaignApiException(response.StatusCode, responseBody);
        }

        var result = await response.Content.ReadFromJsonAsync<T>(
            cancellationToken: cancellationToken);
        return result ?? throw new JsonException("The API returned an empty JSON response.");
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

    private static MessageResponse ValidateMessage(MessageResponse? message)
    {
        if (message is null
            || message.Id == Guid.Empty
            || message.CampaignId == Guid.Empty
            || string.IsNullOrWhiteSpace(message.Content)
            || message.CreatedAt == default)
        {
            throw new JsonException("The API returned a message with missing or invalid fields.");
        }

        return message;
    }
}
