using MerQure.RbMQ.Clients;
using MerQure.RbMQ.Config;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MerQure.RbMQ;

public class MessagingService : IMessagingService
{
    public string ExchangeType { get; set; } = RabbitMQ.Client.ExchangeType.Topic;
    public bool Durable { get; set; }
    public bool AutoDeleteQueue { get; set; }
    public ushort DefaultPrefetchCount { get; set; }
    public long PublisherAcknowledgementsTimeoutInMilliseconds { get; set; }

    protected Task<IConnection> CurrentConnection => _sharedConnection.CurrentConnection;

    private readonly SharedConnection _sharedConnection;

    public MessagingService(IOptions<MerQureConfiguration> merQureConfiguration, SharedConnection sharedConnection)
    {
        _sharedConnection = sharedConnection;

        if (merQureConfiguration == null)
        {
            return;
        }
        Durable = merQureConfiguration.Value.Durable;
        AutoDeleteQueue = merQureConfiguration.Value.AutoDeleteQueue;
        DefaultPrefetchCount = merQureConfiguration.Value.DefaultPrefetchCount;
        PublisherAcknowledgementsTimeoutInMilliseconds = merQureConfiguration.Value.PublisherAcknowledgementsTimeoutInMilliseconds;
    }

    public Task DeclareExchangeAsync(string exchangeName)
    {
        return DeclareExchangeAsync(exchangeName, ExchangeType);
    }

    public async Task DeclareExchangeAsync(string exchangeName, string exchangeType)
    {
        if (string.IsNullOrWhiteSpace(exchangeName)) throw new ArgumentNullException(nameof(exchangeName));

        var connection = await CurrentConnection;
        await using var channel = await connection.CreateChannelAsync();
        await channel.ExchangeDeclareAsync(exchangeName.ToLowerInvariant(), exchangeType, this.Durable);
    }

    public Task<string> DeclareQueueAsync(string queueName, byte maxPriority, bool isQuorum)
    {
        var queueArgs = new Dictionary<string, object> {
            { Constants.QueueMaxPriority, maxPriority }
        };
        return DeclareQueueAsync(queueName, queueArgs, isQuorum);
    }

    public Task<string> DeclareQueueAsync(string queueName, bool isQuorum)
    {
        return DeclareQueueAsync(queueName, new Dictionary<string, object>(), isQuorum);
    }

    public Task<string> DeclareQueueWithDeadLetterPolicyAsync(string queueName, string deadLetterExchange, int messageTimeToLive, string deadLetterRoutingKey, bool isQuorum)
    {
        if (string.IsNullOrWhiteSpace(deadLetterExchange)) throw new ArgumentNullException(nameof(deadLetterExchange));
        if (messageTimeToLive <= 0) throw new ArgumentOutOfRangeException(nameof(messageTimeToLive));

        var queueArgs = new Dictionary<string, object> {
            { Constants.QueueDeadLetterExchange, deadLetterExchange },
            { Constants.QueueMessageTTL, messageTimeToLive }
        };
        if (!string.IsNullOrEmpty(deadLetterRoutingKey))
        {
            queueArgs.Add(Constants.QueueDeadLetterRoutingKey, deadLetterRoutingKey);
        }
        return DeclareQueueAsync(queueName, queueArgs, isQuorum);
    }
    private const string QuorumQueueNameSuffix ="-q";

    public async Task<string> DeclareQueueAsync(string queueName, Dictionary<string, object> queueArgs, bool isQuorum)
    {
        if (string.IsNullOrWhiteSpace(queueName)) throw new ArgumentNullException(nameof(queueName));
        if (queueArgs == null) throw new ArgumentNullException(nameof(queueArgs));

        Dictionary<string, object> effectiveQueueArgs;
        string effectiveQueueName;
        if(isQuorum)
        {
            if(!Durable || AutoDeleteQueue)
            {
                throw new ArgumentException("Quorum queues must be durable and non-auto-delete");
            }
            effectiveQueueArgs = new Dictionary<string, object>(queueArgs)
            {
                { Constants.HeaderQueueType, Constants.HeaderQueueTypeQuorumValue }
            };
            effectiveQueueName = $"{queueName}{QuorumQueueNameSuffix}";
        }
        else
        {
            effectiveQueueArgs = queueArgs;
            effectiveQueueName = queueName;
        }

        var connection = await CurrentConnection;
        await using var channel = await connection.CreateChannelAsync();
        await channel.QueueDeclareAsync(effectiveQueueName.ToLowerInvariant(), this.Durable, false, this.AutoDeleteQueue, effectiveQueueArgs);

        return effectiveQueueName;
    }

    public Task DeclareBindingAsync(string exchangeName, string queueName, string routingKey)
    {
        return DeclareBindingAsync(exchangeName, queueName, routingKey, null);
    }
    public async Task DeclareBindingAsync(string exchangeName, string queueName, string routingKey, Dictionary<string, object> headerBindings)
    {
        if (string.IsNullOrWhiteSpace(exchangeName)) throw new ArgumentNullException(nameof(exchangeName));
        if (string.IsNullOrWhiteSpace(queueName)) throw new ArgumentNullException(nameof(queueName));
        if (routingKey == null) throw new ArgumentNullException(nameof(routingKey));

        var connection = await CurrentConnection;
        await using var channel = await connection.CreateChannelAsync();
        await channel.QueueBindAsync(queueName.ToLowerInvariant(), exchangeName.ToLowerInvariant(), routingKey.ToLowerInvariant(), headerBindings);
    }

    public async Task CancelBindingAsync(string exchangeName, string queueName, string routingKey)
    {
        if (string.IsNullOrWhiteSpace(exchangeName)) throw new ArgumentNullException(nameof(exchangeName));
        if (string.IsNullOrWhiteSpace(queueName)) throw new ArgumentNullException(nameof(queueName));
        if (routingKey == null) throw new ArgumentNullException(nameof(routingKey));

        var connection = await CurrentConnection;
        await using var channel = await connection.CreateChannelAsync();
        await channel.QueueUnbindAsync(queueName.ToLowerInvariant(), exchangeName.ToLowerInvariant(), routingKey.ToLowerInvariant());
    }

    public Task<IPublisher> GetPublisherAsync(string exchangeName)
    {
        return GetPublisherAsync(exchangeName, false);
    }

    public async Task<IPublisher> GetPublisherAsync(string exchangeName, bool enablePublisherAcknowledgements)
    {
        if (string.IsNullOrWhiteSpace(exchangeName)) throw new ArgumentNullException(nameof(exchangeName));

        var channelOptions = new CreateChannelOptions(enablePublisherAcknowledgements, enablePublisherAcknowledgements);
        var connection = await CurrentConnection;
        var channel = await connection.CreateChannelAsync(channelOptions);

        return new Publisher(channel, exchangeName, PublisherAcknowledgementsTimeoutInMilliseconds);
    }

    public Task<IConsumer> GetConsumerAsync(string queueName)
    {
        return GetConsumerAsync(queueName, DefaultPrefetchCount);
    }

    public async Task<IConsumer> GetConsumerAsync(string queueName, ushort prefetchCount)
    {
        if (string.IsNullOrWhiteSpace(queueName)) throw new ArgumentNullException(nameof(queueName));

        var connection = await CurrentConnection;
        var channel = await connection.CreateChannelAsync();
        return new Consumer(channel, queueName, prefetchCount);
    }
}