using MerQure.RbMQ.Content;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MerQure.Samples;

public class StopExample
{
    private readonly IMessagingService _messagingService;

    public StopExample(IMessagingService messagingService)
    {
        _messagingService = messagingService;
    }

    public async Task RunAsync()
    {
        // RabbitMQ init
        await _messagingService.DeclareExchangeAsync("stop.exchange");
        await _messagingService.DeclareQueueAsync("stop.queue", isQuorum: true);
        await _messagingService.DeclareBindingAsync("stop.exchange", "stop.queue", "stop.message.* ");

        // Get the publisher and declare Exhange where publish messages
        await using var publisher = await _messagingService.GetPublisherAsync("stop.exchange");
        // publish messages
        for (int i = 0; i <= 20; i++)
        {
            await publisher.PublishAsync(new Message("stop.message.test" + i, $"Hello world {i} !"));
        }

        // Get the consumer on the existing queue and consume its messages
        var consumer = await _messagingService.GetConsumerAsync("stop.queue");
        await consumer.ConsumeAsync((_, args) =>
        {
            Console.WriteLine("Consumer Working: " + consumer.IsConsuming());

            // Process one message max every 10 ms
            Thread.Sleep(10);
            Console.WriteLine(args.Message.GetBody());
            // send ACK: acknowlegdment to the queue
            consumer.AcknowlegdeDeliveredMessageAsync(args);
        });

        // Stop Consuming after 100 ms ~ 10 messages
        Thread.Sleep(100);
        Console.WriteLine("Consumer Stopping ...");
        await consumer.StopConsuming((_, _) =>
        {
            Console.WriteLine("Consumer Stopped: " + consumer.IsConsuming());
            return Task.CompletedTask;
        });
    }
}