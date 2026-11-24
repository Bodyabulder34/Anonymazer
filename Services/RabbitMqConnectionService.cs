using RabbitMQ.Client;

namespace Anonymazer.Services;

public class RabbitMqConnectionService : IAsyncDisposable
{
    private readonly IConnection _connection;

    public RabbitMqConnectionService(IConfiguration configuration)
    {
        var settings = configuration.GetSection("RabbitMQ");

        var factory = new ConnectionFactory
        {
            HostName = settings["Host"] ?? "localhost",
            Port = settings.GetValue<int>("Port", 5672),
            UserName = settings["Username"]
        ?? throw new InvalidOperationException("RabbitMQ Username не настроен."),
            Password = settings["Password"]
        ?? throw new InvalidOperationException("RabbitMQ Password не настроен.")
        };

        _connection = factory.CreateConnectionAsync()
            .GetAwaiter()
            .GetResult();
    }

    public async Task<IChannel> CreateChannelAsync()
    {
        return await _connection.CreateChannelAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _connection.DisposeAsync();
    }

    public async Task DeclareQueueAsync(string queueName)
    {
        await using var channel = await CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);
    }

}