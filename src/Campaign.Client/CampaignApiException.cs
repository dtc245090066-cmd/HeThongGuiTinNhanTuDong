using System.Net;

namespace Campaign.Client;

public sealed class CampaignApiException(HttpStatusCode statusCode, string responseBody)
    : Exception($"Campaign API returned HTTP {(int)statusCode} ({statusCode}).")
{
    public HttpStatusCode StatusCode { get; } = statusCode;

    public string ResponseBody { get; } = responseBody;
}
