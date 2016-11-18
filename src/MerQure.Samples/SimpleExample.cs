using MerQure.RMQ;
using MerQure.RMQ.Content;
using System;
using System.Collections.Generic;

namespace MerQure.Samples
{
    class SimpleExample
    {
        static void Main(string[] args)
        {
            var messagingService = new MessagingService();

            var publisher = messagingService.GetPublisher("simple.exchange");
            publisher.Declare();

            messagingService.GetSubscriber("simple.queue").DeclareSubscribtion("simple.exchange", "simple.key");

            var message = new Message("simple.key", "TEST_BODY");
            publisher.Publish(message);

            messagingService.GetConsumer("simple.queue").Consume((object sender, IMessagingEvent e) =>
            {
                Console.WriteLine(e.Message.Body);
            });
        }
    }
}
