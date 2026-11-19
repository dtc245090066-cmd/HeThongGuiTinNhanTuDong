using Notification.Api.DTOs;

namespace Notification.Api.Services;

public enum CampaignStartFailure
{
    None,
    NotFound,
    NoRecipients,
    AlreadyStarted,
    QueueUnavailable
}

public sealed record CampaignStartResult(
    CampaignStartFailure Failure,
    StartCampaignResponse Response);
