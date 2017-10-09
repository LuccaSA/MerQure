using MerQure.RbMQ.Content;
using MerQure.RbMQ.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MerQure.RbMQ.Clients
{
    class Consumer : RabbitMqClient, IConsumer
    {
        public string QueueName { get; }
        private EventingBasicConsumer _consumer;
        private static readonly object _consumingLock = new object();

        public Consumer(IModel channel, string queueName)
            : base(channel)
        {
            this.QueueName = queueName.ToLowerInvariant();
        }

        public void Consume(EventHandler<IMessagingEvent> onMessageReceived)
        {
            this.Channel.BasicQos(0, 1, false);

            _consumer = new EventingBasicConsumer(Channel);
            _consumer.Received += ((object sender, BasicDeliverEventArgs args) =>
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
            });

            Channel.BasicConsume(this.QueueName, false, _consumer);
        }

        private IMessage ParseDeliveredMessage(BasicDeliverEventArgs args)
        {
            return new Message(
                args.RoutingKey,
                ParseHeader(args),
                Encoding.UTF8.GetString(args.Body)
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
            if (propertyValue is byte[])
            {
                return Encoding.UTF8.GetString((byte[])propertyValue);
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
                        _consumer.ConsumerCancelled += ((object sender, ConsumerEventArgs e) => 
                        {
                            onConsumerStopped(sender, e);
                        });
                    }
                    this.Channel.BasicCancel(_consumer.ConsumerTag);
                }
            }
        }

        public void AcknowlegdeDeliveredMessage(IMessagingEvent args)
        {
            this.Channel.BasicAck(ulong.Parse(args.DeliveryTag), false);
        }

        public void AcknowlegdeDeliveredMessage(string deliveryTag)
        {
            this.Channel.BasicAck(ulong.Parse(deliveryTag), false);
        }

        public void RejectDeliveredMessage(IMessagingEvent args)
        {
            this.Channel.BasicNack(ulong.Parse(args.DeliveryTag), false, true);
        }

        public void RejectDeliveredMessage(string deliveryTag)
        {
            this.Channel.BasicNack(ulong.Parse(deliveryTag), false, true);
        }

    }
}
