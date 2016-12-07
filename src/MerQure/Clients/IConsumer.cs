using System;

namespace MerQure
{
    /// <summary>
    /// Consumer interface. Used to receive messages from a queue by subscription.
    /// </summary>
    public interface IConsumer
    {
        /// <summary>
        /// name of the queue subscribed by the consumer
        /// </summary>
        string QueueName { get; }

        /// <summary>
        /// Start listening on the queue
        /// </summary>
        /// <param name="OnMessageReceived">Handler called each time a message arrives for this consumer.</param>
        void Consume(EventHandler<IMessagingEvent> OnMessageReceived);

        /// <summary>
        /// Indicates if the Consumer is registred on the queue and waiting for messages
        /// </summary>
        bool IsConsuming();

        /// <summary>
        /// Unregister Consumer from the queue
        /// </summary>
        /// <param name="onConsumerStopped">Handler called when queu has unregistered the consumer</param>
        void StopConsuming(EventHandler onConsumerStopped);

        /// <summary>
        /// Acknowledge a delivered message.
        /// </summary>
        void AcknowlegdeDeliveredMessage(IMessagingEvent args);

        /// <summary>
        /// Reject a delivered message.
        /// </summary>
        void RejectDeliveredMessage(IMessagingEvent args);
    }
}
