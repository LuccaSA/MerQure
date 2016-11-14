using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerQure
{
    interface IConsumer
    {
        string QueueName { get; }

        bool WithAcknowledgement { get; }

        void Consume(EventHandler<IMessagingEvent> OnMessageReceived);

        void AcknowlegdeDeliveredMessage(IMessagingEvent args);

        void RejectDeliveredMessage(IMessagingEvent args);
    }
}
