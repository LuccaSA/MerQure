using RabbitMQ.Client;
using System;
using System.Text;

namespace MerQure.RabbitMQ.Clients
{
    class Publisher : RabbitMQClient, IPublisher
    {
        internal enum DeliveryMode : byte
        {
            NonPersistent = 1,
            Persistent = 2
        }

        public string ExchangeName { get; private set; }
        public string ExchangeType { get; private set; }

        /// <summary>
        /// Create a RabbitMQ Publisher
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="exchangeName"></param>
        /// <param name="exchangeType"></param>
        public Publisher(IModel channel, string exchangeName, string exchangeType)
            : base(channel)
        {
            if(String.IsNullOrWhiteSpace(exchangeName))
            {
                throw new Exception("exchangeName cannot be null or empty");
            } else if (String.IsNullOrWhiteSpace(exchangeType))
            {
                throw new Exception("exchangeType cannot be null or empty");
            }
            this.ExchangeName = exchangeName.ToLowerInvariant();
            this.ExchangeType = exchangeType;
        }

        public void Declare()
        {
            this.Channel.ExchangeDeclare(this.ExchangeName, this.ExchangeType, durable: true);
        }

        public void Publish(IMessage message)
        {
            IBasicProperties basicProperties = this.Channel.CreateBasicProperties();
            basicProperties.DeliveryMode = (byte)DeliveryMode.Persistent;
            basicProperties.CorrelationId = Guid.NewGuid().ToString();
            basicProperties.Headers = message.Header.GetHeaderProperties();

            byte[] messageBytes = Encoding.UTF8.GetBytes(message.Body);

            this.Channel.BasicPublish(ExchangeName, message.RoutingKey, basicProperties, messageBytes);
        }
    }
}
