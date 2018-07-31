using MerQure.RbMQ;
using MerQure.RbMQ.Content;
using System;

namespace MerQure.Samples
{
    public class StopExample
    {
        private readonly IMessagingService _messagingService;

        public StopExample(IMessagingService messagingService)
        {
            _messagingService = messagingService;
        }

        public void Run()
        {
            // RabbitMQ init
            _messagingService.DeclareExchange("stop.exchange");
            _messagingService.DeclareQueue("stop.queue");
            _messagingService.DeclareBinding("stop.exchange", "stop.queue", "stop.message.* ");

            // Get the publisher and declare Exhange where publish messages
            using (var publisher = _messagingService.GetPublisher("stop.exchange"))
            {
                // publish messages
                for (int i = 0; i <= 20; i++)
                {
                    publisher.Publish(new Message("stop.message.test" + i, $"Hello world {i} !"));
                }
            }

            // Get the consumer on the existing queue and consume its messages
            var consumer = _messagingService.GetConsumer("stop.queue");
            consumer.Consume((object sender, IMessagingEvent args) =>
            {
                Console.WriteLine("Consumer Working: " + consumer.IsConsuming());

                // Process one message max every 10 ms
                System.Threading.Thread.Sleep(10);
                Console.WriteLine(args.Message.GetBody());
                // send ACK: acknowlegdment to the queue 
                consumer.AcknowlegdeDeliveredMessage(args);
            });

            // Stop Consuming after 100 ms ~ 10 messages
            System.Threading.Thread.Sleep(100);
            Console.WriteLine("Consumer Stopping ...");
            consumer.StopConsuming((object sender, EventArgs args) =>
            {
                Console.WriteLine("Consumer Stopped: " + consumer.IsConsuming().ToString());
            });
        }
    }
}
