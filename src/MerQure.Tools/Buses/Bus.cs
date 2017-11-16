using MerQure.Messages;
using MerQure.Tools.Configurations;
using MerQure.Tools.Messages;
using System;
using System.Collections.Generic;
using System.Text;

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

        public void Publish(Channel channel, T message)
        {
            Producer.Publish(channel, message);
        }

        public void PublishWithTransaction(Channel channel, IEnumerable<T> messages)
        {
            Producer.PublishWithTransaction(channel, messages);
        }

        public void RejectDeliveredMessage(Channel channel, T deliveredMessage)
        {
            Consumer.RejectDeliveredMessage(channel, deliveredMessage);
        }

        public void ApplyRetryStrategy(Channel channel, T deliveredMessage)
        {
            Consumer.ApplyRetryStrategy(channel, deliveredMessage);
        }

        public void SendDeliveredMessageToErrorBus(Channel channel, T deliveredMessage)
        {
            Consumer.SendToErrorExchange(channel, deliveredMessage);
        }

        public void PublishForceRetryAttemptNumber(Channel channel, T message, int attemptNumber)
        {
            Consumer.ForceRetryStrategy(channel, message, attemptNumber);
        }
    }
}
