namespace Notification.Infrastructure.Messaging;

public interface INotificationPublisher
{
    Task PublishAsync(NotificationMessage message, CancellationToken cancellationToken);
}
