using MerQure.Messages;
using MerQure.RbMQ.Content;
using MerQure.RbMQ.Events;
using MerQure.RbMQ.Helpers;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Linq;
using System.Text;

namespace MerQure.RbMQ.Clients
{
    internal class Consumer : RabbitMqClient, IConsumer
    {
        public string QueueName { get; }

        private EventingBasicConsumer _consumer;
        private readonly ushort _prefetchCount;
        private readonly object _consumingLock;

        public Consumer(IModel channel, string queueName, ushort prefetchCount)
            : base(channel)
        {
            QueueName = queueName.ToLowerInvariant();
            _prefetchCount = prefetchCount;
            _consumingLock = new object();
        }

        public void Consume(EventHandler<IMessagingEvent> onMessageReceived)
        {
            Channel.BasicQos(0, _prefetchCount, false);

            _consumer = new EventingBasicConsumer(Channel);
            _consumer.Received += (object sender, BasicDeliverEventArgs args) =>
            {
                if (onMessageReceived != null)
                {
                    lock (_consumingLock)
                    {
                        var message = ParseDeliveredMessage(args);
                        var messageEventArgs = new MessagingEvent(message, args.DeliveryTag.ToString());
                        onMessageReceived(sender, messageEventArgs);
                    }
                }
            };

            Channel.BasicConsume(QueueName, false, _consumer);
        }

        private IMessage ParseDeliveredMessage(BasicDeliverEventArgs args)
        {
            return new Message(
                args.RoutingKey,
                ParseHeader(args),
                EncodingHelper.ToString(args.Body)
            );
        }

        private Header ParseHeader(BasicDeliverEventArgs args)
        {
            return new Header(args.BasicProperties.Headers.ToDictionary(kvp => kvp.Key, kvp => ParseHeaderProperty(kvp.Value)));
        }

        /// <summary>
        /// Parse header properties, ignoring complex type as "Nested Table"
        /// </summary>
        /// <param name="propertyValue"></param>
        /// <returns></returns>
        /// <see cref="https://www.rabbitmq.com/amqp-0-9-1-errata.html"/>
        private object ParseHeaderProperty(object propertyValue)
        {
            if (propertyValue is byte[] bytes)
            {
                return EncodingHelper.ToString(bytes);
            }
            return propertyValue;
        }

        public bool IsConsuming()
        {
            return _consumer != null && _consumer.IsRunning;
        }

        public void StopConsuming(EventHandler onConsumerStopped)
        {
            if (IsConsuming())
            {
                lock (_consumingLock)
                {
                    if (onConsumerStopped != null)
                    {
                        _consumer.ConsumerCancelled += (object sender, ConsumerEventArgs e) =>
                        {
                            onConsumerStopped(sender, e);
                        };
                    }
                    Channel.BasicCancel(_consumer.ConsumerTag);
                }
            }
        }

        public void AcknowlegdeDeliveredMessage(IDelivered deliveredMessage)
        {
            Channel.BasicAck(ulong.Parse(deliveredMessage.DeliveryTag), false);
        }

        public void RejectDeliveredMessage(IDelivered deliveredMessage)
        {
            Channel.BasicNack(ulong.Parse(deliveredMessage.DeliveryTag), false, true);
        }
    }
}