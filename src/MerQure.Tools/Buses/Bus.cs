using MerQure.Messages;
using MerQure.Tools.Configurations;
using System;
using System.Collections.Generic;

namespace MerQure.Tools.Buses
{
    internal class Bus<T> : IBus<T> where T : IDelivered
    {
        internal Publisher<T> Producer { get; set; }
        internal RetryConsumer<T> Consumer { get; set; }

        public Bus(Publisher<T> producer, RetryConsumer<T> consumer)
        {
            Producer = producer;
            Consumer = consumer;
        }

        public void AcknowlegdeDeliveredMessage(Channel channel, T deliveredMessage)
        {
            Consumer.AcknowlegdeDeliveredMessage(channel, deliveredMessage);
        }

        public void OnMessageReceived(Channel channel, EventHandler<T> callback)
        {
            Consumer.Consume(channel, callback);
        }

        public void Publish(Channel channel, T message, bool applyDeliveryDeplay = false)
        {
            Producer.Publish(channel, message, applyDeliveryDeplay);
        }

        public void PublishWithTransaction(Channel channel, IEnumerable<T> messages, bool applyDeliveryDeplay = false)
        {
            Producer.PublishWithTransaction(channel, messages, applyDeliveryDeplay);
        }

        public void RejectDeliveredMessage(Channel channel, T deliveredMessage)
        {
            Consumer.RejectDeliveredMessage(channel, deliveredMessage);
        }

        public MessageInformations ApplyRetryStrategy(Channel channel, T deliveredMessage)
        {
            return Consumer.ApplyRetryStrategy(channel, deliveredMessage);
        }

        public void SendDeliveredMessageToErrorBus(Channel channel, T deliveredMessage)
        {
            Consumer.SendToErrorExchange(channel, deliveredMessage);
        }

        public MessageInformations PublishForceRetryAttemptNumber(Channel channel, T message, int attemptNumber)
        {
            return Consumer.ForceRetryStrategy(channel, message, attemptNumber);
        }
    }
}
