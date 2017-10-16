using MerQure.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerQure.RetryExchange
{
    internal class RetryExchange<T> : IRetryExchange<T> where T : IAmqpIdentity
    {
        internal RetryPublisher<T> Publisher { get; set; }
        internal RetryConsumer<T> Consumer { get; set; }

        internal RetryExchange(RetryPublisher<T> producer, RetryConsumer<T> consumer)
        {
            Publisher = producer;
            Consumer = consumer;
        }

        public void AcknowlegdeDelivredMessage(string queueName, T delivredMessage)
        {
            Consumer.AcknowlegdeDelivredMessage(queueName, delivredMessage);
        }

        public void Consume(string queueName, EventHandler<T> callback)
        {
            Consumer.Consume(queueName, callback);
        }

        public void Publish(string queueName, T message)
        {
            Publisher.Publish(queueName, message);
        }

        public void RejectDeliveredMessage(string queueName, T delivredMessage)
        {
            Consumer.RejectDeliveredMessage(queueName, delivredMessage);
        }

        public void ApplyRetryStrategy(string queueName, T delivredMessage)
        {
            Consumer.ApplyRetryStrategy(queueName, delivredMessage);
        }

        public void SendDelivredMessageToErrorExchange(string queueName, T delivredMessage)
        {
            Consumer.SendToErrorExchange(queueName, delivredMessage);
        }
    }
}
