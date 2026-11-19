namespace Notification.Infrastructure.Messaging;

public sealed record NotificationMessage(int DeliveryId, int CampaignId);
