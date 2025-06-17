using MerQure.Messages;
using MerQure.Tools.Configurations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MerQure.Tools.Buses;

internal class Bus<T> : IBus<T> where T : IDelivered
{
    internal Publisher<T> Producer { get; set; }
    internal RetryConsumer<T> Consumer { get; set; }

    public Bus(Publisher<T> producer, RetryConsumer<T> consumer)
    {
        Producer = producer;
        Consumer = consumer;
    }

    public Task AcknowlegdeDeliveredMessageAsync(Channel channel, T deliveredMessage)
    {
        return Consumer.AcknowlegdeDeliveredMessageAsync(channel, deliveredMessage);
    }

    public Task OnMessageReceivedAsync(Channel channel, EventHandler<T> callback)
    {
        return Consumer.ConsumeAsync(channel, callback);
    }

    public Task PublishAsync(Channel channel, T message, bool applyDeliveryDeplay = false)
    {
        return Producer.PublishAsync(channel, message, applyDeliveryDeplay);
    }

    public Task PublishWithTransactionAsync (Channel channel, IEnumerable<T> messages, bool applyDeliveryDeplay = false)
    {
        return Producer.PublishWithTransactionAsync(channel, messages, applyDeliveryDeplay);
    }

    public Task RejectDeliveredMessageAsync(Channel channel, T deliveredMessage)
    {
        return Consumer.RejectDeliveredMessageAsync(channel, deliveredMessage);
    }

    public Task<MessageInformations> ApplyRetryStrategyAsync(Channel channel, T deliveredMessage)
    {
        return Consumer.ApplyRetryStrategyAsync(channel, deliveredMessage);
    }

    public Task SendDeliveredMessageToErrorBusAsync(Channel channel, T deliveredMessage)
    {
        return Consumer.SendToErrorExchangeAsync(channel, deliveredMessage);
    }

    public Task<MessageInformations> PublishForceRetryAttemptNumberAsync(Channel channel, T message, int attemptNumber)
    {
        return Consumer.ForceRetryStrategyAsync(channel, message, attemptNumber);
    }
}