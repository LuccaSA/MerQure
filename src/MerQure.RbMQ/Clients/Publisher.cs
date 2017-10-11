using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;
using MerQure.RbMQ.Helpers;

namespace MerQure.RbMQ.Clients
{
    class Publisher : RabbitMqClient, IPublisher
    {
        internal enum DeliveryMode : byte
        {
            NonPersistent = 1,
            Persistent = 2
        }

        public string ExchangeName { get; private set; }
        public string ExchangeType { get; private set; }
        public bool Durable { get; private set; }
        public long TimeoutInMs { get; set; }

        public Publisher(IModel channel, string exchangeName, long acknowledgementsTimeoutInMS) : base(channel)
        {
            if (String.IsNullOrWhiteSpace(exchangeName))
            {
                throw new ArgumentException("exchangeName cannot be null or empty", nameof(exchangeName));
            }
            this.TimeoutInMs = acknowledgementsTimeoutInMS;
            this.ExchangeName = exchangeName.ToLowerInvariant();
        }


        public bool PublishWithAcknowledgement(IMessage message)
        {
            Publish(message);
            return this.Channel.WaitForConfirms(new TimeSpan(TimeoutInMs * TimeSpan.TicksPerMillisecond));
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
