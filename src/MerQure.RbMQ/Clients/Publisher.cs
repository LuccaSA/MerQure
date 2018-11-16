using MerQure.RbMQ.Content;
using MerQure.RbMQ.Helpers;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;

namespace MerQure.RbMQ.Clients
{
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
        public TimeSpan Timeout { get; set; }

        public Publisher(IModel channel, string exchangeName, long acknowledgementsTimeoutInMilliseconds) : base(channel)
        {
            if (string.IsNullOrWhiteSpace(exchangeName))
            {
                throw new ArgumentException("exchangeName cannot be null or empty", nameof(exchangeName));
            }
            Timeout = new TimeSpan(acknowledgementsTimeoutInMilliseconds * TimeSpan.TicksPerMillisecond);
            ExchangeName = exchangeName.ToLowerInvariant();
        }

        public void PublishWithTransaction(string queueName, IEnumerable<string> messages)
        {
            Channel.TxSelect();
            try
            {
                foreach (string message in messages)
                {
                    Publish(new Message(queueName, message));
                }
            }
            catch
            {
                Channel.TxRollback();
                throw;
            }
            Channel.TxCommit();
        }

        public bool PublishWithAcknowledgement(IMessage message)
        {
            Publish(message);
            return Channel.WaitForConfirms(Timeout);
        }

        public bool PublishWithAcknowledgement(string queueName, string message)
        {
            return PublishWithAcknowledgement(new Message(queueName, message));
        }

        public void Publish(IMessage message)
        {
            IBasicProperties basicProperties = CreateBasicProperties(message);
            Channel.BasicPublish(ExchangeName, message.GetRoutingKey(), basicProperties, message.GetBody().ToByte());
        }

        private IBasicProperties CreateBasicProperties(IMessage message)
        {
            IBasicProperties basicProperties = Channel.CreateBasicProperties();
            basicProperties.DeliveryMode = (byte)DeliveryMode.Persistent;
            basicProperties.CorrelationId = Guid.NewGuid().ToString();
            basicProperties.Headers = message.GetHeader().GetProperties();

            var priority = message.GetPriority();
            if (priority != null)
            {
                basicProperties.Priority = priority.Value;
            }

            return basicProperties;
        }
    }
}
