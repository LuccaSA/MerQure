using MerQure.Messages;
using RabbitMQ.Client.Events;
using System;
using System.Threading.Tasks;

namespace MerQure
{
    /// <summary>
    /// Consumer interface. Used to receive messages from a queue by subscription.
    /// </summary>
    public interface IConsumer : IAsyncDisposable
    {
        /// <summary>
        /// name of the queue subscribed by the consumer
        /// </summary>
        string QueueName { get; }

        /// <summary>
        /// Start listening on the queue
        /// </summary>
        /// <param name="onMessageReceived">Handler called each time a message arrives for this consumer.</param>
        Task ConsumeAsync(EventHandler<IMessagingEvent> onMessageReceived);

        /// <summary>
        /// Indicates if the Consumer is registred on the queue and waiting for messages
        /// </summary>
        bool IsConsuming();

        /// <summary>
        /// Unregister Consumer from the queue
        /// </summary>
        /// <param name="onConsumerStopped">Handler called when queu has unregistered the consumer</param>
        Task StopConsuming(AsyncEventHandler<ConsumerEventArgs> onConsumerStopped);

        /// <summary>
        /// Acknowledge a delivered message.
        /// </summary>
        /// <param name="deliveredMessage">delivered message</param>
        ValueTask AcknowlegdeDeliveredMessageAsync(IDelivered deliveredMessage);

        /// <summary>
        /// Reject a delivered message.
        /// </summary>
        /// <param name="deliveredMessage">delivered message</param>
        ValueTask RejectDeliveredMessageAsync(IDelivered deliveredMessage);
    }
}