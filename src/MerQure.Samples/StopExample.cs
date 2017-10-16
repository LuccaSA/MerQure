using MerQure.RbMQ;
using MerQure.RbMQ.Content;
using System;
using System.Collections.Generic;

namespace MerQure.Samples
{
    public static class StopExample
    {
        public static void Run()
        {
            var messagingService = new MessagingService();

            // RabbitMQ init
            messagingService.DeclareExchange("stop.exchange");
            messagingService.DeclareQueue("stop.queue");
            messagingService.DeclareBinding("stop.exchange", "stop.queue", "stop.message.* ");

            // Get the publisher and declare Exhange where publish messages
            using (var publisher = messagingService.GetPublisher("stop.exchange"))
            {
                // publish messages
                for (int i = 0; i <= 20; i++)
                {
                    publisher.Publish(new Message("stop.message.test" + i, $"Hello world {i} !"));
                }
            }

            // Get the consumer on the existing queue and consume its messages
            var consumer = messagingService.GetOrCreateConsumer("stop.queue");
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
