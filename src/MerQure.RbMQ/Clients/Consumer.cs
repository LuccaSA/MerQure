using MerQure.RbMQ.Content;
using MerQure.RbMQ.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Linq;
using System.Text;

namespace MerQure.RbMQ.Clients
{
    class Consumer : RabbitMqClient, IConsumer
    {
        public string QueueName { get; }
        private EventingBasicConsumer _consumer;
        private static Object _consumingLock = new Object();

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
                        var message = new Message(
                            args.RoutingKey,
                            new Header(args.BasicProperties.Headers.ToDictionary(kvp => kvp.Key, kvp => Encoding.UTF8.GetString((byte[])kvp.Value))),
                            Encoding.UTF8.GetString(args.Body)
                        );
                        var messageEventArgs = new MessagingEvent(message, args.DeliveryTag.ToString());
                        onMessageReceived(sender, messageEventArgs);
                    }
                }
            });

            Channel.BasicConsume(this.QueueName, false, _consumer);
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

        public void RejectDeliveredMessage(IMessagingEvent args)
        {
            this.Channel.BasicNack(ulong.Parse(args.DeliveryTag), false, true);
        }
    }
}
