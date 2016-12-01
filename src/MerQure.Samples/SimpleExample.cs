using MerQure.RbMQ;
using MerQure.RbMQ.Content;
using System;
using System.Collections.Generic;

namespace MerQure.Samples
{
    static class SimpleExample
    {
        static void Main()
        {
            var messagingService = new MessagingService();

            // Get the publisher and declare Exhange where publish messages
            var publisher = messagingService.GetPublisher("simple.exchange");
            publisher.Declare();

            // Get the subscriber on a queue and declare a subscription on the existing exchange
            messagingService.GetSubscriber("simple.queue").DeclareSubscribtion("simple.exchange", "simple.message.*");

            // publish messages
            publisher.Publish(new Message("simple.message.test1", "Hello world !"));
            publisher.Publish(new Message("simple.message.test2", "John Doe was here."));

            // Get the consumer on the existing queue and consume its messages
            messagingService.GetConsumer("simple.queue").Consume((object sender, IMessagingEvent e) =>
            {
                Console.WriteLine(e.Message.Body);
            });
        }
    }
}
