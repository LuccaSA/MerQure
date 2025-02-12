using MerQure.RbMQ.Content;
using System;
using System.Collections.Generic;

namespace MerQure.Samples
{
    public class DeadLetterExample
    {
        private readonly IMessagingService _messagingService;

        public DeadLetterExample(IMessagingService messagingService)
        {
            _messagingService = messagingService;
        }

        public void Run()
        {
            // RabbitMQ init
            _messagingService.DeclareExchange("deadletter.exchange", Constants.ExchangeTypeFanout);
            _messagingService.DeclareQueue("deadletter.queue", isQuorum: true);
            _messagingService.DeclareBinding("deadletter.exchange", "deadletter.queue", "#");

            _messagingService.DeclareExchange("delay.exchange", Constants.ExchangeTypeHeaders);
            _messagingService.DeclareQueueWithDeadLetterPolicy("deadletter.queue.5", "deadletter.exchange", 5000, null, isQuorum: true);
            _messagingService.DeclareQueueWithDeadLetterPolicy("deadletter.queue.30", "deadletter.exchange", 30000, null, isQuorum: true);

            _messagingService.DeclareBinding("delay.exchange", "deadletter.queue.5", "#", new Dictionary<string, object> { { "delay", 5 } });
            _messagingService.DeclareBinding("delay.exchange", "deadletter.queue.30", "#", new Dictionary<string, object> { { "delay", 30 } });

            var dateStart = DateTime.Now;

            // Get the publisher and publish messages
            using (var publisher = _messagingService.GetPublisher("delay.exchange"))
            {
                // publish messages waiting 5s
                for (int i = 0; i <= 10; i++)
                {
                    var message = new Message($"delay.5.message.{i}", $"Waited 5s: {i} !");
                    message.Header.GetProperties().Add("delay", 5);
                    publisher.Publish(message);
                }
                // publish messages waiting 30s
                for (int i = 0; i <= 10; i++)
                {
                    var message = new Message($"delay.30.message.{i}", $"Waited 30s: {i} !");
                    message.Header.GetProperties().Add("delay", 30);
                    publisher.Publish(message);
                }
            }

            // Get the consumer on the existing queue and consume its messages
            var consumer = _messagingService.GetConsumer("deadletter.queue");
            consumer.Consume((object sender, IMessagingEvent args) =>
            {
                var realDelay = DateTime.Now.Subtract(dateStart).TotalSeconds;
                Console.WriteLine(string.Format("{0} received after {1:#.##}s.", args.Message.GetRoutingKey(), realDelay));
                // send ACK: acknowlegdment to the queue 
                consumer.AcknowlegdeDeliveredMessage(args);
            });
        }
    }
}
