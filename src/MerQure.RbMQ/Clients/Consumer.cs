using MerQure.RbMQ.Content;
using MerQure.RbMQ.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Linq;
using System.Text;

namespace MerQure.RbMQ.Clients
{
    class Consumer : RabbitMQClient, IConsumer
    {
        public string QueueName { get; }

        public Consumer(IModel channel, string queueName)
            : base(channel)
        {
            this.QueueName = queueName.ToLowerInvariant();
        }

        public void Consume(EventHandler<IMessagingEvent> onMessageReceived)
        {
            this.Channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(Channel);
            consumer.Received += ((object sender, BasicDeliverEventArgs args) =>
            {
                if (onMessageReceived != null)
                {
                    var message = new Message(
                        args.RoutingKey,
                        new Header(args.BasicProperties.Headers.ToDictionary(kvp => kvp.Key, kvp => Encoding.UTF8.GetString((byte[])kvp.Value))),
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
