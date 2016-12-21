using MerQure.RbMQ.Clients;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;

namespace MerQure.RbMQ
{
    public class MessagingService : IMessagingService
    {
        private static IConnection GetRabbitMqConnection()
        {
            var rabbitMqConnection = Config.RabbitMqConfiguration.GetConfig().Connection;

            ConnectionFactory connectionFactory = new ConnectionFactory {
                Uri = rabbitMqConnection.ConnectionString,
                AutomaticRecoveryEnabled = rabbitMqConnection.AutomaticRecoveryEnabled,
                TopologyRecoveryEnabled = rabbitMqConnection.TopologyRecoveryEnabled
            };

            return connectionFactory.CreateConnection();
        }
        private static Lazy<IConnection> currentConnection = new Lazy<IConnection>(() => GetRabbitMqConnection());
        public static IConnection CurrentConnection
        {
            get { return currentConnection.Value; }
        }

        public bool Durable { get; set; }
        public bool AutoDeleteQueue { get; set; }
        public string ExchangeType { get; set; }

        public MessagingService()
        {
            var rabbitMqConfig = Config.RabbitMqConfiguration.GetConfig();

            this.Durable = rabbitMqConfig.Durable;
            this.AutoDeleteQueue = rabbitMqConfig.AutoDeleteQueue;
            this.ExchangeType = RabbitMQ.Client.ExchangeType.Topic;
        }

        public void DeclareExchange(string exchangeName)
        {
            var channel = CurrentConnection.CreateModel();

            if (String.IsNullOrWhiteSpace(exchangeName))
            {
                throw new Exception("exchangeName cannot be null or empty");
            }
            else if (String.IsNullOrWhiteSpace(this.ExchangeType))
            {
                throw new Exception("exchangeType cannot be null or empty");
            }

            channel.ExchangeDeclare(exchangeName.ToLowerInvariant(), this.ExchangeType, this.Durable);
        }

        public void DeclareQueue(string queueName)
        {
            var channel = CurrentConnection.CreateModel();

            var queueArgs = new Dictionary<string, object>();
            channel.QueueDeclare(queueName, this.Durable, false, this.AutoDeleteQueue, queueArgs);
        }

        public void DeclareBinding(string exchangeName, string routingKey, string queueName)
        {
            var channel = CurrentConnection.CreateModel();

            channel.QueueBind(queueName, exchangeName.ToLowerInvariant(), routingKey.ToLowerInvariant());
        }

        public void CancelBinding(string exchangeName, string routingKey, string queueName)
        {
            var channel = CurrentConnection.CreateModel();

            channel.QueueUnbind(queueName, exchangeName.ToLowerInvariant(), routingKey.ToLowerInvariant());
        }

        public IPublisher GetPublisher(string exchangeName)
        {
            var channel = CurrentConnection.CreateModel();
            return new Publisher(channel, exchangeName);
        }

        public IConsumer GetConsumer(string queueName)
        {
            var channel = CurrentConnection.CreateModel();
            return new Consumer(channel, queueName);
        }
    }
}
