using System.Net;
using System.Text.Json;
using Campaign.Client;
using Campaign.Client.Models;

var baseAddressText = args.Length > 0 ? args[0] : "http://localhost:5000";
if (!Uri.TryCreate(baseAddressText, UriKind.Absolute, out var baseAddress)
    || (baseAddress.Scheme != Uri.UriSchemeHttp && baseAddress.Scheme != Uri.UriSchemeHttps))
{
    Console.Error.WriteLine("API URL must be an absolute HTTP or HTTPS address.");
    return 2;
}

if (!baseAddress.AbsoluteUri.EndsWith('/'))
{
    baseAddress = new Uri($"{baseAddress.AbsoluteUri}/");
}

using var httpClient = new HttpClient
{
    BaseAddress = baseAddress,
    Timeout = TimeSpan.FromSeconds(10)
};

var campaignApiClient = new CampaignApiClient(httpClient);
Console.WriteLine($"Campaign API: {baseAddress}");

if (!await TryListCampaignsAsync(campaignApiClient))
{
    return 1;
}

while (true)
{
    Console.WriteLine();
    Console.WriteLine("1. List campaigns");
    Console.WriteLine("2. Create campaign");
    Console.WriteLine("0. Exit");
    Console.Write("Select an option: ");

    var option = Console.ReadLine();
    if (option is null)
    {
        return 0;
    }

    switch (option)
    {
        case "1":
            await TryListCampaignsAsync(campaignApiClient);
            break;
        case "2":
            await CreateCampaignAsync(campaignApiClient);
            break;
        case "0":
            return 0;
        default:
            Console.WriteLine("Please select 1, 2, or 0.");
            break;
    }
}

static async Task<bool> TryListCampaignsAsync(CampaignApiClient campaignApiClient)
{
    try
    {
        var campaigns = await campaignApiClient.GetCampaignsAsync();
        Console.WriteLine();
        Console.WriteLine($"Campaigns ({campaigns.Count}):");

        if (campaigns.Count == 0)
        {
            Console.WriteLine("No campaigns found.");
            return true;
        }

        foreach (var campaign in campaigns)
        {
            Console.WriteLine($"- {campaign.Name} [{campaign.Status}]");
            Console.WriteLine($"  Id: {campaign.Id}");
            Console.WriteLine($"  Created: {campaign.CreatedAt:O}");
            Console.WriteLine($"  Content: {campaign.Content}");
        }

        return true;
    }
    catch (HttpRequestException exception)
    {
        WriteHttpRequestError(exception);
    }
    catch (TaskCanceledException)
    {
        Console.Error.WriteLine("The request timed out. Check that the Campaign API is running and try again.");
    }
    catch (JsonException exception)
    {
        Console.Error.WriteLine($"The API returned invalid or unreadable JSON: {exception.Message}");
    }
    catch (InvalidOperationException exception)
    {
        Console.Error.WriteLine($"The API response could not be read as JSON: {exception.Message}");
    }
    catch (NotSupportedException exception)
    {
        Console.Error.WriteLine($"The API response format is not supported: {exception.Message}");
    }

    return false;
}

static async Task CreateCampaignAsync(CampaignApiClient campaignApiClient)
{
    Console.Write("Campaign name: ");
    var name = Console.ReadLine();
    if (name is null)
    {
        return;
    }

    Console.Write("Notification content: ");
    var content = Console.ReadLine();
    if (content is null)
    {
        return;
    }

    try
    {
        var campaign = await campaignApiClient.CreateCampaignAsync(new CreateCampaignRequest(name, content));
        Console.WriteLine($"Campaign created successfully (HTTP 201): {campaign.Name}");
        Console.WriteLine($"Id: {campaign.Id}");
        Console.WriteLine($"Status: {campaign.Status}");
        Console.WriteLine($"Created: {campaign.CreatedAt:O}");
    }
    catch (CampaignApiException exception)
    {
        WriteCampaignApiError(exception);
    }
    catch (HttpRequestException exception)
    {
        WriteHttpRequestError(exception);
    }
    catch (TaskCanceledException)
    {
        Console.Error.WriteLine("The request timed out. Check that the Campaign API is running and try again.");
    }
    catch (JsonException exception)
    {
        Console.Error.WriteLine($"The API returned invalid or unreadable JSON: {exception.Message}");
    }
    catch (InvalidOperationException exception)
    {
        Console.Error.WriteLine($"The API response could not be read as JSON: {exception.Message}");
    }
    catch (NotSupportedException exception)
    {
        Console.Error.WriteLine($"The API response format is not supported: {exception.Message}");
    }
}

static void WriteHttpRequestError(HttpRequestException exception)
{
    if (exception.StatusCode == HttpStatusCode.BadRequest)
    {
        Console.Error.WriteLine($"The API rejected the request (HTTP 400): {exception.Message}");
    }
    else if (exception.StatusCode == HttpStatusCode.InternalServerError)
    {
        Console.Error.WriteLine("The Campaign API encountered an internal server error (HTTP 500). Try again later.");
    }
    else if (exception.StatusCode is { } statusCode)
    {
        Console.Error.WriteLine($"The Campaign API returned HTTP {(int)statusCode} ({statusCode}).");
    }
    else
    {
        Console.Error.WriteLine("Could not connect to the Campaign API. Make sure it is running and the API URL is correct.");
    }
}

static void WriteCampaignApiError(CampaignApiException exception)
{
    if (exception.StatusCode == HttpStatusCode.BadRequest)
    {
        Console.Error.WriteLine($"The API rejected the campaign data (HTTP 400): {exception.ResponseBody}");
    }
    else if (exception.StatusCode == HttpStatusCode.InternalServerError)
    {
        Console.Error.WriteLine($"The Campaign API encountered an internal server error (HTTP 500): {exception.ResponseBody}");
    }
    else
    {
        Console.Error.WriteLine($"The Campaign API returned HTTP {(int)exception.StatusCode} ({exception.StatusCode}): {exception.ResponseBody}");
    }
}
