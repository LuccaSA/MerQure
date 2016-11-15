using System;

namespace MerQure
{
    public interface IConsumer
    {
        string QueueName { get; }

        bool WithAcknowledgement { get; }

        void Consume(EventHandler<IMessagingEvent> OnMessageReceived);

        void AcknowlegdeDeliveredMessage(IMessagingEvent args);

        void RejectDeliveredMessage(IMessagingEvent args);
    }
}
