using MerQure.RbMQ.Clients;
using RabbitMQ.Client;
using System;

namespace MerQure.RbMQ
{
    public class MessagingService : IMessagingService
    {
        private const string DefaultRabbitMqConnectionUri = "amqp://guest:guest@localhost:5672/";
        private static string RabbitMqConnectionUri 
        {
            get
            {
                var connectionStringSetting = System.Configuration.ConfigurationManager.ConnectionStrings["RabbitMQ"];
                return connectionStringSetting != null ? connectionStringSetting.ConnectionString : DefaultRabbitMqConnectionUri;
            }
        }
        private static IConnection GetRabbitMqConnection()
        {
            ConnectionFactory connectionFactory = new ConnectionFactory {
                Uri = RabbitMqConnectionUri,
                AutomaticRecoveryEnabled = true,
                TopologyRecoveryEnabled = true
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
            // TODO Laod Parameters from config
            this.Durable = true;
            this.AutoDeleteQueue = false;
            this.ExchangeType = RabbitMQ.Client.ExchangeType.Topic;
        }

        public IPublisher GetPublisher(string exchangeName)
        {
            var channel = CurrentConnection.CreateModel();
            return new Publisher(channel, exchangeName, this.ExchangeType, this.Durable);
        }

        public ISubscriber GetSubscriber(string queueName)
        {
            var channel = CurrentConnection.CreateModel();
            return new Subscriber(channel, queueName, this.Durable, this.AutoDeleteQueue);
        }

        public IConsumer GetConsumer(string queueName)
        {
            var channel = CurrentConnection.CreateModel();
            return new Consumer(channel, queueName);
        }
    }
}
