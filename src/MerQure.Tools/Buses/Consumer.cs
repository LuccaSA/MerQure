using MerQure.Messages;
using MerQure.Tools.Configurations;
using MerQure.Tools.Messages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MerQure.Tools.Buses;

public class Consumer<T> where T : IDelivered
{
    private readonly ConsumerProvider _consumerProvider;
    public Dictionary<string, RetryInformations> RetryInformations { get; } = new Dictionary<string, RetryInformations>();

    public Consumer(IMessagingService messagingService)
    {
        _consumerProvider = new ConsumerProvider(messagingService);
    }

    public async Task ConsumeAsync(Channel channel, EventHandler<T> callback)
    {
        var consumer = await _consumerProvider.GetAsync(channel);
        await consumer.ConsumeAsync((_, messagingEvent) =>
        {
            OnMessageReceived(callback, messagingEvent);
        });
    }

    public async Task AcknowlegdeDeliveredMessageAsync(Channel channel, IDelivered deliveredMessage)
    {
        deliveredMessage.DeliveryTag = DecodeDeliveryTag(deliveredMessage.DeliveryTag);
        var consumer = await _consumerProvider.GetAsync(channel);
        await consumer.AcknowlegdeDeliveredMessageAsync(deliveredMessage);
        RetryInformations.Remove(deliveredMessage.DeliveryTag);
    }

    public async Task RejectDeliveredMessageAsync(Channel channel, IDelivered deliveredMessage)
    {
        deliveredMessage.DeliveryTag = DecodeDeliveryTag(deliveredMessage.DeliveryTag);
        var consumer = await _consumerProvider.GetAsync(channel);
        await consumer.RejectDeliveredMessageAsync(deliveredMessage);
        RetryInformations.Remove(deliveredMessage.DeliveryTag);
    }

    public void OnMessageReceived(EventHandler<T> callback, IMessagingEvent messagingEvent)
    {
        RetryMessage<T> retryMessage = JsonConvert.DeserializeObject<RetryMessage<T>>(messagingEvent.Message.GetBody());
        retryMessage.OriginalMessage.DeliveryTag = EncodeDeliveryTag(messagingEvent.DeliveryTag);
        RetryInformations.Add(retryMessage.OriginalMessage.DeliveryTag, retryMessage.RetryInformations);

        callback(this, retryMessage.OriginalMessage);
    }

    private static string EncodeDeliveryTag(string deliveryTag) //TODO CLEAN !! this is just a fast fix ...
    {
        return $"{deliveryTag}_{Guid.NewGuid().ToString()}";
    }

    private static string DecodeDeliveryTag(string deliveryTag) //TODO CLEAN !! this is just a fast fix
    {
        return deliveryTag.Split('_')[0];
    }
}