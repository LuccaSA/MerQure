using MerQure.RabbitMQ;
using MerQure.RabbitMQ.Clients;
using MerQure.RabbitMQ.Content;
using MerQure.RabbitMQ.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace MerQure.Clients
{
    class Consumer : RabbitMQClient, IConsumer
    {
        private EventingBasicConsumer consumer;

        public string QueueName { get; }

        public Consumer(IModel channel, string queueName)
            : base(channel)
        {
            this.QueueName = queueName.ToLowerInvariant();
        }

        public void Consume(EventHandler<IMessagingEvent> onMessageReceived)
        {
            this.Channel.BasicQos(0, 1, false);

            consumer = new EventingBasicConsumer(Channel);
            consumer.Received += ((object sender, BasicDeliverEventArgs args) =>
            {
                if (onMessageReceived != null)
                {
                    var message = new Message(
                        args.RoutingKey,
                        new Header(args.BasicProperties.Headers),
                        Encoding.UTF8.GetString(args.Body)
                    );
                    var messageEventArgs = new MessagingEvent(message, args.DeliveryTag.ToString());
                    onMessageReceived(sender, messageEventArgs);
                }
            });

            Channel.BasicConsume(this.QueueName, false, consumer);
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
