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

            ConnectionFactory connectionFactory = new ConnectionFactory
            {
                Uri = rabbitMqConnection.ConnectionString,
                AutomaticRecoveryEnabled = rabbitMqConnection.AutomaticRecoveryEnabled,
                TopologyRecoveryEnabled = rabbitMqConnection.TopologyRecoveryEnabled
            };

            return connectionFactory.CreateConnection();
        }
        private static readonly Lazy<IConnection> currentConnection = new Lazy<IConnection>(GetRabbitMqConnection);
        public static IConnection CurrentConnection => currentConnection.Value;

        public bool Durable { get; private set; }
        public bool AutoDeleteQueue { get; private set; }
        public string ExchangeType { get; private set; }

        public MessagingService()
        {
            var rabbitMqConfig = Config.RabbitMqConfiguration.GetConfig();

            this.Durable = rabbitMqConfig.Durable;
            this.AutoDeleteQueue = rabbitMqConfig.AutoDeleteQueue;
            this.ExchangeType = RabbitMQ.Client.ExchangeType.Topic;
        }
        
        public void DeclareExchange(string exchangeName)
        {
            DeclareExchange(exchangeName, this.ExchangeType);
        }

        public void DeclareExchange(string exchangeName, string exchangeType)
        {
            if (string.IsNullOrWhiteSpace(exchangeName)) throw new ArgumentNullException(nameof(exchangeName));

            using (var channel = CurrentConnection.CreateModel())
            {
                channel.ExchangeDeclare(exchangeName.ToLowerInvariant(), exchangeType, this.Durable);
            }
        }

        public void DeclareQueue(string queueName, byte maxPriority)
        {
            var queueArgs = new Dictionary<string, object> {
                { Constants.QueueMaxPriority, maxPriority }
            };
            DeclareQueue(queueName, queueArgs);
        }

        public void DeclareQueue(string queueName)
        {
            DeclareQueue(queueName, new Dictionary<string, object>());
        }
        public void DeclareQueueWithDeadLetterPolicy(string queueName, string deadLetterExchange, int messageTimeToLive, string deadLetterRoutingKey)
        {
            if (string.IsNullOrWhiteSpace(deadLetterExchange)) throw new ArgumentNullException(nameof(deadLetterExchange));
            if (messageTimeToLive <= 0) throw new ArgumentOutOfRangeException(nameof(messageTimeToLive));

            var queueArgs = new Dictionary<string, object> {
                { Constants.QueueDeadLetterExchange, deadLetterExchange },
                { Constants.QueueMessageTTL, messageTimeToLive }
            };
            if (!string.IsNullOrEmpty(deadLetterRoutingKey))
            {
                queueArgs.Add(Constants.QueueDeadLetterRoutingKey, deadLetterRoutingKey);
            }
            DeclareQueue(queueName, queueArgs);
        }
        public void DeclareQueue(string queueName, Dictionary<string, object> queueArgs)
        {
            if (string.IsNullOrWhiteSpace(queueName)) throw new ArgumentNullException(nameof(queueName));
            if (queueArgs == null) throw new ArgumentNullException(nameof(queueArgs));

            using (var channel = CurrentConnection.CreateModel())
            {
                channel.QueueDeclare(queueName.ToLowerInvariant(), this.Durable, false, this.AutoDeleteQueue, queueArgs);
            }
        }

        public void DeclareBinding(string exchangeName, string queueName, string routingKey)
        {
            DeclareBinding(exchangeName, queueName, routingKey, null);
        }
        public void DeclareBinding(string exchangeName, string queueName, string routingKey, Dictionary<string, object> headerBindings)
        {
            if (string.IsNullOrWhiteSpace(exchangeName)) throw new ArgumentNullException(nameof(exchangeName));
            if (string.IsNullOrWhiteSpace(queueName)) throw new ArgumentNullException(nameof(queueName));
            if (string.IsNullOrWhiteSpace(routingKey)) throw new ArgumentNullException(nameof(routingKey));

            using (var channel = CurrentConnection.CreateModel())
            {
                channel.QueueBind(queueName.ToLowerInvariant(), exchangeName.ToLowerInvariant(), routingKey.ToLowerInvariant(), headerBindings);
            }
        }

        public void CancelBinding(string exchangeName, string queueName, string routingKey)
        {
            if (string.IsNullOrWhiteSpace(exchangeName)) throw new ArgumentNullException(nameof(exchangeName));
            if (string.IsNullOrWhiteSpace(queueName)) throw new ArgumentNullException(nameof(queueName));
            if (string.IsNullOrWhiteSpace(routingKey)) throw new ArgumentNullException(nameof(routingKey));

            using (var channel = CurrentConnection.CreateModel())
            {
                channel.QueueUnbind(queueName.ToLowerInvariant(), exchangeName.ToLowerInvariant(), routingKey.ToLowerInvariant());
            }
        }

        public IPublisher GetPublisher(string exchangeName)
        {
            if (string.IsNullOrWhiteSpace(exchangeName)) throw new ArgumentNullException(nameof(exchangeName));

            var channel = CurrentConnection.CreateModel();
            return new Publisher(channel, exchangeName);
        }

        public IConsumer GetConsumer(string queueName)
        {
           return GetConsumer(queueName, 1);
        }

        public IConsumer GetConsumer(string queueName, ushort prefetchCount)
        {
            if (string.IsNullOrWhiteSpace(queueName)) throw new ArgumentNullException(nameof(queueName));

            var channel = CurrentConnection.CreateModel();
            return new Consumer(channel, queueName, prefetchCount);
        }

    }
}
