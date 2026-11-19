namespace Notification.Infrastructure.Messaging;

public sealed class RabbitMqOptions
{
    public string HostName { get; set; } = "localhost";

    public int Port { get; set; } = 5672;

    public string UserName { get; set; } = "bulknotify_dev";

    public string Password { get; set; } = string.Empty;
}
