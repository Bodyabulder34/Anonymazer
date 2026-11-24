
using System.Text;
using System.Text.Json;
using Anonymazer.Models;
using Anonymazer.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Anonymazer.Worker;

public class Worker(
    ILogger<Worker> logger,
    RabbitMqConnectionService rabbitMq,
    IServiceScopeFactory scopeFactory) : BackgroundService
{
    private const string QueueName = "anonymization-queue";

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        await using var channel = await rabbitMq.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        await channel.BasicQosAsync(
            prefetchSize: 0,
            prefetchCount: 1,
            global: false,
            cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (_, eventArgs) =>
        {
            AnonymizationMessage? message = null;

            try
            {
                var json = Encoding.UTF8.GetString(
                    eventArgs.Body.ToArray());

                message = JsonSerializer
                    .Deserialize<AnonymizationMessage>(json);

                if (message == null)
                {
                    throw new InvalidOperationException(
                        "Получено пустое сообщение.");
                }

                using var scope = scopeFactory.CreateScope();

                var anonymizationService =
                    scope.ServiceProvider
                        .GetRequiredService<AnonymizationService>();

                var historyService =
                    scope.ServiceProvider
                        .GetRequiredService<AnonymizationHistoryService>();

                var jobService =
                    scope.ServiceProvider
                        .GetRequiredService<AnonymizationJobService>();

                // Обновляем статус задания
                await jobService.UpdateStatusAsync(
                    message.JobId,
                    "Processing");

                // Выполняем анонимизацию
                var result = await anonymizationService
                    .AnonymizeWithStatisticsAsync(message.Text);

                // Сохраняем результат в историю
                await historyService.SaveAsync(
                    result,
                    message.UserId);

                // Сохраняем результат задания
                await jobService.UpdateStatusAsync(
                    message.JobId,
                    "Completed",
                    result.AnonymizedText);

                logger.LogInformation(
                    "Задание {JobId} успешно обработано.",
                    message.JobId);

                await channel.BasicAckAsync(
                    eventArgs.DeliveryTag,
                    multiple: false);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Ошибка обработки задания {JobId}.",
                    message?.JobId);

                if (message != null)
                {
                    try
                    {
                        using var scope = scopeFactory.CreateScope();

                        var jobService =
                            scope.ServiceProvider
                                .GetRequiredService<AnonymizationJobService>();

                        await jobService.UpdateStatusAsync(
                            message.JobId,
                            "Failed",
                            errorMessage: "Не удалось обработать задание.");
                    }
                    catch (Exception updateException)
                    {
                        logger.LogError(
                            updateException,
                            "Не удалось обновить статус задания {JobId}.",
                            message.JobId);
                    }
                }

                await channel.BasicNackAsync(
                    eventArgs.DeliveryTag,
                    multiple: false,
                    requeue: true);
            }
        };

        await channel.BasicConsumeAsync(
            queue: QueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);

        logger.LogInformation("RabbitMQ Consumer запущен.");

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}
