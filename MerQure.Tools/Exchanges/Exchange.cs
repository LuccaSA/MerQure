using MerQure.Tools.Configurations;
using MerQure.Tools.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace MerQure.Tools.Exchanges
{
    internal class Exchange<T> : IExchange<T> where T : IAMQPIdentity
    {
        internal Publisher<T> Producer { get; set; }
        internal RetryConsumer<T> Consumer { get; set; }

        public Exchange(Publisher<T> producer, RetryConsumer<T> consumer)
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

        public void RejectDeliveredMessage(Channel channel, T delivredMessage)
        {
            Consumer.RejectDeliveredMessage(channel, delivredMessage);
        }

        public void ApplyRetryStrategy(Channel channel, T delivred)
        {
            Consumer.ApplyRetryStrategy(channel, delivred);
        }

        public void SendDelivredMessageToErrorExchange(Channel channel, T delivredMessage)
        {
            Consumer.SendToErrorExchange(channel, delivredMessage);
        }
    }
}
