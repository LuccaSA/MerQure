using MerQure.RbMQ.Clients;
using MerQure.RbMQ.Config;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;

namespace MerQure.RbMQ
{
    public class MessagingService : IMessagingService
    {
        public string ExchangeType { get; set; } = RabbitMQ.Client.ExchangeType.Topic;
        public bool Durable { get; set; }
        public bool AutoDeleteQueue { get; set; }
        public ushort DefaultPrefetchCount { get; set; }
        public long PublisherAcknowledgementsTimeoutInMilliseconds { get; set; }

        protected IConnection CurrentConnection => _sharedConnection.CurrentConnection;

        private readonly SharedConnection _sharedConnection;

        public MessagingService(IOptions<MerQureConfiguration> merQureConfiguration, SharedConnection sharedConnection)
        {
            _sharedConnection = sharedConnection;

            if (merQureConfiguration == null)
            {
                return;
            }
            Durable = merQureConfiguration.Value.Durable;
            AutoDeleteQueue = merQureConfiguration.Value.AutoDeleteQueue;
            DefaultPrefetchCount = merQureConfiguration.Value.DefaultPrefetchCount;
            PublisherAcknowledgementsTimeoutInMilliseconds = merQureConfiguration.Value.PublisherAcknowledgementsTimeoutInMilliseconds;
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

        public string DeclareQueue(string queueName, byte maxPriority, bool isQuorum)
        {
            var queueArgs = new Dictionary<string, object> {
                { Constants.QueueMaxPriority, maxPriority }
            };
            return DeclareQueue(queueName, queueArgs, isQuorum);
        }

        public string DeclareQueue(string queueName, bool isQuorum)
        {
            return DeclareQueue(queueName, new Dictionary<string, object>(), isQuorum);
        }

        public string DeclareQueueWithDeadLetterPolicy(string queueName, string deadLetterExchange, int messageTimeToLive, string deadLetterRoutingKey, bool isQuorum)
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
            return DeclareQueue(queueName, queueArgs, isQuorum);
        }
        private const string QuorumQueueNameSuffix ="-q";

        public string DeclareQueue(string queueName, Dictionary<string, object> queueArgs, bool isQuorum)
        {
            if (string.IsNullOrWhiteSpace(queueName)) throw new ArgumentNullException(nameof(queueName));
            if (queueArgs == null) throw new ArgumentNullException(nameof(queueArgs));

            Dictionary<string, object> effectiveQueueArgs;
            string effectiveQueueName;
            if(isQuorum)
            {
                if(!Durable || AutoDeleteQueue)
                {
                    throw new ArgumentException("Quorum queues must be durable and non-auto-delete");
                }
                effectiveQueueArgs = new Dictionary<string, object>(queueArgs)
                {
                    { Constants.HeaderQueueType, Constants.HeaderQueueTypeQuorumValue }
                };
                effectiveQueueName = $"{queueName}{QuorumQueueNameSuffix}";
            }
            else
            {
                effectiveQueueArgs = queueArgs;
                effectiveQueueName = queueName;
            }

            using (var channel = CurrentConnection.CreateModel())
            {
                channel.QueueDeclare(effectiveQueueName.ToLowerInvariant(), this.Durable, false, this.AutoDeleteQueue, effectiveQueueArgs);
            }

            return effectiveQueueName;
        }

        public void DeclareBinding(string exchangeName, string queueName, string routingKey)
        {
            DeclareBinding(exchangeName, queueName, routingKey, null);
        }
        public void DeclareBinding(string exchangeName, string queueName, string routingKey, Dictionary<string, object> headerBindings)
        {
            if (string.IsNullOrWhiteSpace(exchangeName)) throw new ArgumentNullException(nameof(exchangeName));
            if (string.IsNullOrWhiteSpace(queueName)) throw new ArgumentNullException(nameof(queueName));
            if (routingKey == null) throw new ArgumentNullException(nameof(routingKey));

            using (var channel = CurrentConnection.CreateModel())
            {
                channel.QueueBind(queueName.ToLowerInvariant(), exchangeName.ToLowerInvariant(), routingKey.ToLowerInvariant(), headerBindings);
            }
        }

        public void CancelBinding(string exchangeName, string queueName, string routingKey)
        {
            if (string.IsNullOrWhiteSpace(exchangeName)) throw new ArgumentNullException(nameof(exchangeName));
            if (string.IsNullOrWhiteSpace(queueName)) throw new ArgumentNullException(nameof(queueName));
            if (routingKey == null) throw new ArgumentNullException(nameof(routingKey));

            using (var channel = CurrentConnection.CreateModel())
            {
                channel.QueueUnbind(queueName.ToLowerInvariant(), exchangeName.ToLowerInvariant(), routingKey.ToLowerInvariant());
            }
        }

        public IPublisher GetPublisher(string exchangeName)
        {
            return GetPublisher(exchangeName, false);
        }

        public IPublisher GetPublisher(string exchangeName, bool enablePublisherAcknowledgements)
        {
            if (string.IsNullOrWhiteSpace(exchangeName)) throw new ArgumentNullException(nameof(exchangeName));

            var channel = CurrentConnection.CreateModel();
            if (enablePublisherAcknowledgements)
            {
                channel.ConfirmSelect();
            }

            return new Publisher(channel, exchangeName, PublisherAcknowledgementsTimeoutInMilliseconds);
        }

        public IConsumer GetConsumer(string queueName)
        {
            return GetConsumer(queueName, DefaultPrefetchCount);
        }

        public IConsumer GetConsumer(string queueName, ushort prefetchCount)
        {
            if (string.IsNullOrWhiteSpace(queueName)) throw new ArgumentNullException(nameof(queueName));

            var channel = CurrentConnection.CreateModel();
            return new Consumer(channel, queueName, prefetchCount);
        }
    }
}
