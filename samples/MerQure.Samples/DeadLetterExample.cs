using MerQure.RbMQ.Content;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MerQure.Samples;

public class DeadLetterExample
{
    private readonly IMessagingService _messagingService;

    public DeadLetterExample(IMessagingService messagingService)
    {
        _messagingService = messagingService;
    }

    public async Task RunAsync()
    {
        // RabbitMQ init
        await _messagingService.DeclareExchangeAsync("deadletter.exchange", Constants.ExchangeTypeFanout);
        await _messagingService.DeclareQueueAsync("deadletter.queue", isQuorum: true);
        await _messagingService.DeclareBindingAsync("deadletter.exchange", "deadletter.queue", "#");

        await _messagingService.DeclareExchangeAsync("delay.exchange", Constants.ExchangeTypeHeaders);
        await _messagingService.DeclareQueueWithDeadLetterPolicyAsync("deadletter.queue.5", "deadletter.exchange", 5000, null, isQuorum: true);
        await _messagingService.DeclareQueueWithDeadLetterPolicyAsync("deadletter.queue.30", "deadletter.exchange", 30000, null, isQuorum: true);

        await _messagingService.DeclareBindingAsync("delay.exchange", "deadletter.queue.5", "#", new Dictionary<string, object> { { "delay", 5 } });
        await _messagingService.DeclareBindingAsync("delay.exchange", "deadletter.queue.30", "#", new Dictionary<string, object> { { "delay", 30 } });

        var dateStart = DateTime.Now;

        // Get the publisher and publish messages
        await using (var publisher = await _messagingService.GetPublisherAsync("delay.exchange"))
        {
            // publish messages waiting 5s
            for (int i = 0; i <= 10; i++)
            {
                var message = new Message($"delay.5.message.{i}", $"Waited 5s: {i} !");
                message.Header.GetProperties().Add("delay", 5);
                await publisher.PublishAsync(message);
            }
            // publish messages waiting 30s
            for (int i = 0; i <= 10; i++)
            {
                var message = new Message($"delay.30.message.{i}", $"Waited 30s: {i} !");
                message.Header.GetProperties().Add("delay", 30);
                await publisher.PublishAsync(message);
            }
        }

        // Get the consumer on the existing queue and consume its messages
        var consumer = await _messagingService.GetConsumerAsync("deadletter.queue");
        await consumer.ConsumeAsync((object sender, IMessagingEvent args) =>
        {
            var realDelay = DateTime.Now.Subtract(dateStart).TotalSeconds;
            Console.WriteLine(string.Format("{0} received after {1:#.##}s.", args.Message.GetRoutingKey(), realDelay));
            // send ACK: acknowlegdment to the queue
            consumer.AcknowlegdeDeliveredMessageAsync(args);
        });
    }
}