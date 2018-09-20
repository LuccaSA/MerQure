using RabbitMQ.Client;
using System;
using MerQure.RbMQ.Helpers;
using MerQure.RbMQ.Content;
using System.Collections.Generic;
using System.Threading;

namespace MerQure.RbMQ.Clients
{
    public class Publisher : RabbitMqClient, IPublisher
    {
        internal enum DeliveryMode : byte
        {
            NonPersistent = 1,
            Persistent = 2
        }

        public string ExchangeName { get; private set; }
        public string ExchangeType { get; private set; }
        public bool Durable { get; private set; }
        public long TimeoutInMilliseconds { get; set; }

        public Publisher(IModel channel, string exchangeName, long acknowledgementsTimeoutInMilliseconds) : base(channel)
        {
            if (String.IsNullOrWhiteSpace(exchangeName))
            {
                throw new ArgumentException("exchangeName cannot be null or empty", nameof(exchangeName));
            }
            this.TimeoutInMilliseconds = acknowledgementsTimeoutInMilliseconds;
            this.ExchangeName = exchangeName.ToLowerInvariant();
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
            catch (Exception e)
            {
                Channel.TxRollback();
                throw;
            }
            Channel.TxCommit();
        }

        public bool PublishWithAcknowledgement(IMessage message)
        {
            Publish(message);
            return this.Channel.WaitForConfirms(new TimeSpan(TimeoutInMilliseconds * TimeSpan.TicksPerMillisecond));
        }

        public bool PublishWithAcknowledgement(string queueName, string message)
        {
            return PublishWithAcknowledgement(new Message(queueName, message));
        }

        public void Publish(IMessage message)
        {
            IBasicProperties basicProperties = CreateBasicProperties(message);
            this.Channel.BasicPublish(ExchangeName, message.GetRoutingKey(), basicProperties, message.GetBody().ToByte());
        }

        private IBasicProperties CreateBasicProperties(IMessage message)
        {
            IBasicProperties basicProperties = this.Channel.CreateBasicProperties();
            basicProperties.DeliveryMode = (byte)DeliveryMode.Persistent;
            basicProperties.CorrelationId = Guid.NewGuid().ToString();
            basicProperties.Headers = message.GetHeader().GetProperties();
            if (message.GetPriority() != null)
            {
                basicProperties.Priority = message.GetPriority().Value;
            }

            return basicProperties;
        }


    }
}
