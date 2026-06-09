using MerQure.Messages;
using MerQure.RbMQ.Content;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MerQure.RbMQ.Clients;

class Consumer : RabbitMqClient, IConsumer
{
    public string QueueName { get; }

    private AsyncEventingBasicConsumer _consumer;
    private readonly ushort _prefetchCount;
    private readonly SemaphoreSlim _consumingLock;

    public Consumer(IChannel channel, string queueName, ushort prefetchCount)
        : base(channel)
    {
        QueueName = queueName.ToLowerInvariant();
        _prefetchCount = prefetchCount;
        _consumingLock = new SemaphoreSlim(1, 1);
    }

    public async Task ConsumeAsync(AsyncEventHandler<MessagingEvent> onMessageReceived)
    {
        await Channel.BasicQosAsync(0, _prefetchCount, false);

        _consumer = new AsyncEventingBasicConsumer(Channel);
        _consumer.ReceivedAsync += async (sender, args) =>
        {
            if (onMessageReceived != null)
            {
                await _consumingLock.WaitAsync();
                try
                {
                    var message = ParseDeliveredMessage(args);
                    var messageEventArgs = new MessagingEvent(message, args.DeliveryTag.ToString());
                    await onMessageReceived(sender, messageEventArgs);
                }
                finally
                {
                    _consumingLock.Release();
                }
            }
        };

        await Channel.BasicConsumeAsync(QueueName, false, _consumer);
    }

    private IMessage ParseDeliveredMessage(BasicDeliverEventArgs args)
    {
        return new Message(
            args.RoutingKey,
            ParseHeader(args),
            Encoding.UTF8.GetString(args.Body.ToArray())
        );
    }

    private Header ParseHeader(BasicDeliverEventArgs args)
    {
        return new Header(
            args.BasicProperties.Headers?
                .ToDictionary(kvp => kvp.Key, kvp => ParseHeaderProperty(kvp.Value))
            ?? new Dictionary<string, object>());
    }
    /// <summary>
    /// Parse header properties, ignoring complex type as "Nested Table"
    /// </summary>
    /// <param name="propertyValue"></param>
    /// <returns></returns>
    /// <see cref="https://www.rabbitmq.com/amqp-0-9-1-errata.html"/>
    private object ParseHeaderProperty(object propertyValue)
    {
        if (propertyValue is byte[])
        {
            return Encoding.UTF8.GetString((byte[])propertyValue);
        }
        return propertyValue;
    }

    public bool IsConsuming()
    {
        return _consumer != null && _consumer.IsRunning;
    }

    public async Task StopConsuming(AsyncEventHandler<ConsumerEventArgs> onConsumerStopped)
    {
        if (IsConsuming())
        {
            await _consumingLock.WaitAsync();
            try
            {
                if (onConsumerStopped != null)
                {
                    _consumer.UnregisteredAsync += onConsumerStopped;
                }
            }
            finally
            {
                _consumingLock.Release();
            }

            // Must be outside the lock to avoid deadlock
            foreach (var tag in _consumer.ConsumerTags)
            {
                await Channel.BasicCancelAsync(tag);
            }
        }
    }

    public ValueTask AcknowlegdeDeliveredMessageAsync(IDelivered deliveredMessage)
    {
        return Channel.BasicAckAsync(ulong.Parse(deliveredMessage.DeliveryTag), false);
    }


    public ValueTask RejectDeliveredMessageAsync(IDelivered deliveredMessage)
    {
        return Channel.BasicNackAsync(ulong.Parse(deliveredMessage.DeliveryTag), false, true);
    }
}