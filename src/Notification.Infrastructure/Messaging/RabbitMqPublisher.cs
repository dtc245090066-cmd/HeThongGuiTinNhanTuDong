using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Notification.Infrastructure.Messaging;

public sealed class RabbitMqPublisher : INotificationPublisher, IAsyncDisposable
{
    public const string QueueName = "notification_queue";

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly RabbitMqOptions _options;
    private readonly SemaphoreSlim _channelLock = new(1, 1);
    private readonly ILogger<RabbitMqPublisher> _logger;
    private IConnection? _connection;
    private IChannel? _channel;

    public RabbitMqPublisher(
        IOptions<RabbitMqOptions> options,
        ILogger<RabbitMqPublisher> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task PublishAsync(NotificationMessage message, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_options.Password))
        {
            throw new InvalidOperationException(
                "Chua cau hinh mat khau RabbitMQ. Hay dat bien moi truong RabbitMq__Password.");
        }

        await _channelLock.WaitAsync(cancellationToken);

        try
        {
            await EnsureChannelAsync(cancellationToken);

            var noiDung = JsonSerializer.SerializeToUtf8Bytes(message, JsonOptions);
            var thuocTinh = new BasicProperties
            {
                ContentType = "application/json",
                DeliveryMode = DeliveryModes.Persistent,
                MessageId = message.DeliveryId.ToString()
            };

            await _channel!.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: QueueName,
                mandatory: true,
                basicProperties: thuocTinh,
                body: noiDung,
                cancellationToken: cancellationToken);
        }
        catch (Exception loi)
        {
            _logger.LogError(
                loi,
                "Khong the publish Delivery {DeliveryId} vao RabbitMQ queue {QueueName}.",
                message.DeliveryId,
                QueueName);
            throw;
        }
        finally
        {
            _channelLock.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _channelLock.WaitAsync();

        try
        {
            if (_channel is not null)
            {
                await _channel.DisposeAsync();
            }

            if (_connection is not null)
            {
                await _connection.DisposeAsync();
            }
        }
        finally
        {
            _channelLock.Release();
            _channelLock.Dispose();
        }
    }

    private async Task EnsureChannelAsync(CancellationToken cancellationToken)
    {
        if (_connection?.IsOpen == true && _channel?.IsOpen == true)
        {
            return;
        }

        if (_channel is not null)
        {
            await _channel.DisposeAsync();
            _channel = null;
        }

        if (_connection is not null)
        {
            await _connection.DisposeAsync();
            _connection = null;
        }

        var nhaMay = new ConnectionFactory
        {
            HostName = _options.HostName,
            Port = _options.Port,
            UserName = _options.UserName,
            Password = _options.Password,
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(5),
            RequestedConnectionTimeout = TimeSpan.FromSeconds(5)
        };

        _connection = await nhaMay.CreateConnectionAsync(cancellationToken);
        _channel = await _connection.CreateChannelAsync(
            new CreateChannelOptions(
                publisherConfirmationsEnabled: true,
                publisherConfirmationTrackingEnabled: true),
            cancellationToken);

        await _channel.QueueDeclareAsync(
            queue: QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);
    }
}
