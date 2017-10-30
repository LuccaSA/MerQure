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

        public void AcknowlegdeDelivredMessage(Channel channel, T delivredMessage)
        {
            Consumer.AcknowlegdeDelivredMessage(channel, delivredMessage);
        }

        public void Consume(Channel channel, EventHandler<T> callback)
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

        public void RejectDeliveredMessage(Channel channel, T delivredMessage)
        {
            Consumer.RejectDeliveredMessage(channel, delivredMessage);
        }

        public void ApplyRetryStrategy(Channel channel, T delivredMessage)
        {
            Consumer.ApplyRetryStrategy(channel, delivredMessage);
        }

        public void SendDelivredMessageToErrorBus(Channel channel, T delivredMessage)
        {
            Consumer.SendToErrorExchange(channel, delivredMessage);
        }
    }
}
