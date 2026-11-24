using System.Text;
using System.Text.Json;
using Anonymazer.Models;
using RabbitMQ.Client;

namespace Anonymazer.Services;

public class RabbitMqProducer
{
    private readonly RabbitMqConnectionService _connectionService;

    public RabbitMqProducer(RabbitMqConnectionService connectionService)
    {
        _connectionService = connectionService;
    }

    public async Task PublishAsync(AnonymizationMessage message)
    {
        await using var channel =
            await _connectionService.CreateChannelAsync();

        var body = Encoding.UTF8.GetBytes(
            JsonSerializer.Serialize(message));

        var properties = new BasicProperties
        {
            Persistent = true
        };

        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: "anonymization-queue",
            mandatory: true,
            basicProperties: properties,
            body: body);
    }
}