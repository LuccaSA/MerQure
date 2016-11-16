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

            messagingService.GetPublisher("simple.exchange").Declare();

            messagingService.GetSubscriber("simple.queue").DeclareSubscribtion("simple.exchange", "simple.key");

            var headerProp = new Dictionary<string, object> { { "header-property", "property-value" } };
            var header = new Header(headerProp);
            var message = new Message("simple.key", header, "TEST_BODY");
            messagingService.GetPublisher("simple.exchange").Publish(message);

            messagingService.GetConsumer("simple.queue").Consume((object sender, IMessagingEvent e) => 
            {
                Console.WriteLine(e.Message.Body);
            });
        }
    }
}
