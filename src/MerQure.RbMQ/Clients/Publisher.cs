using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;

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

        /// <summary>
        /// Create a RabbitMQ Publisher
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="exchangeName"></param>
        /// <param name="exchangeType"></param>
        public Publisher(IModel channel, string exchangeName)
            : base(channel)
        {
            if(String.IsNullOrWhiteSpace(exchangeName))
            {
                throw new ArgumentException("exchangeName cannot be null or empty", nameof(exchangeName));
            }

            this.ExchangeName = exchangeName.ToLowerInvariant();
        }

        public void Publish(IMessage message)
        {
            IBasicProperties basicProperties = this.Channel.CreateBasicProperties();
            basicProperties.DeliveryMode = (byte)DeliveryMode.Persistent;
            basicProperties.CorrelationId = Guid.NewGuid().ToString();
            basicProperties.Headers = message.GetHeader().GetProperties();
            if (message.GetPriority() != null)
            {
                basicProperties.Priority = message.GetPriority().Value;
            }

            byte[] messageBytes = Encoding.UTF8.GetBytes(message.GetBody());

            this.Channel.BasicPublish(ExchangeName, message.GetRoutingKey(), basicProperties, messageBytes);
        }
    }
}
