using MerQure.RbMQ.Content;
using MerQure.RbMQ.Helpers;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MerQure.RbMQ.Clients;

internal class Publisher : RabbitMqClient, IPublisher
{
    internal enum DeliveryMode : byte
    {
        NonPersistent = 1,
        Persistent = 2
    }

    public string ExchangeName { get; private set; }
    public string ExchangeType { get; private set; }
    public bool Durable { get; private set; }
    public long TimeoutInMilliseconds { get; set; }

    public Publisher(IChannel channel, string exchangeName, long acknowledgementsTimeoutInMilliseconds) : base(channel)
    {
        if (String.IsNullOrWhiteSpace(exchangeName))
        {
            throw new ArgumentException("exchangeName cannot be null or empty", nameof(exchangeName));
        }
        this.TimeoutInMilliseconds = acknowledgementsTimeoutInMilliseconds;
        this.ExchangeName = exchangeName.ToLowerInvariant();
    }

    public async Task PublishWithTransactionAsync(string queueName, IEnumerable<string> messages)
    {
        await Channel.TxSelectAsync();
        try
        {
            foreach (string message in messages)
            {
                await PublishAsync(new Message(queueName, message));
            }
        }
        catch (Exception)
        {
            await Channel.TxRollbackAsync();
            throw;
        }
        await Channel.TxCommitAsync();
    }

    public async Task PublishWithAcknowledgementAsync(IMessage message)
    {
        // Confirmation is now handled by the Publish method
        // cf : https://github.com/rabbitmq/rabbitmq-dotnet-client/discussions/1720#discussioncomment-11250853
        await PublishAsync(message);
    }

    public Task PublishWithAcknowledgementAsync(string queueName, string message)
    {
        return PublishWithAcknowledgementAsync(new Message(queueName, message));
    }

    public Task PublishAsync(IMessage message)
    {
        var basicProperties = CreateBasicProperties(message);
        var addr = new PublicationAddress("", ExchangeName, message.GetRoutingKey());
        using var cts = new CancellationTokenSource(new TimeSpan(TimeoutInMilliseconds * TimeSpan.TicksPerMillisecond));
        return Channel.BasicPublishAsync(
            addr,
            basicProperties,
            message.GetBody().ToByte(),
            cancellationToken: cts.Token)
            .AsTask();
    }

    private BasicProperties CreateBasicProperties(IMessage message)
    {
        var basicProperties = new BasicProperties
        {
            DeliveryMode = DeliveryModes.Persistent,
            CorrelationId = Guid.NewGuid().ToString(),
            Headers = message.GetHeader().GetProperties()
        };
        if (message.GetPriority() != null)
        {
            basicProperties.Priority = message.GetPriority().Value;
        }

        return basicProperties;
    }
}