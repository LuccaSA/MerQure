using MerQure.RbMQ.Content;
using System;
using System.Threading.Tasks;

namespace MerQure.Samples;

public class SimpleExample
{
    private readonly IMessagingService _messagingService;

    public SimpleExample(IMessagingService messagingService)
    {
        _messagingService = messagingService;
    }

    public async Task RunAsync()
    {
        // RabbitMQ init
        await _messagingService.DeclareExchangeAsync("simple.exchange");
        await _messagingService.DeclareQueueAsync("simple.queue", isQuorum: true);
        await _messagingService.DeclareBindingAsync("simple.exchange", "simple.queue", "simple.message.*");

        // Get the publisher and declare Exhange where publish messages
        await using var publisher = await _messagingService.GetPublisherAsync("simple.exchange");
        // publish messages
        for (int i = 0; i <= 10; i++)
        {
            await publisher.PublishAsync(new Message($"simple.message.test{i}", $"Hello world {i} !"));
        }


        // Get the consumer on the existing queue and consume its messages
        var consumer = await _messagingService.GetConsumerAsync("simple.queue");
        var random = new Random();
        await consumer.ConsumeAsync((object sender, IMessagingEvent args) =>
        {
            // we simulate the delivery success
            if (random.Next() % 2 == 0)
            {
                Console.WriteLine("Retry " + args.Message.GetRoutingKey());
                // send NACK: negative acknowlegdment to the queue
                consumer.RejectDeliveredMessageAsync(args);
            }
            else
            {
                Console.WriteLine(args.Message.GetBody());
                // send ACK: acknowlegdment to the queue
                consumer.AcknowlegdeDeliveredMessageAsync(args);
            }
        });
    }
}