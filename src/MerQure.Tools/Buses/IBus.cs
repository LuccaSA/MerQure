using MerQure.Messages;
using MerQure.Tools.Configurations;
using MerQure.Tools.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace MerQure.Tools.Buses
{
    public interface IBus<T> where T : IDelivered
    {
        void Publish(Channel channel, T message);
        void PublishWithTransaction(Channel channel, IEnumerable<T> message);
        void Consume(Channel channel, EventHandler<T> callback);
        void ApplyRetryStrategy(Channel channel, T message);
        void AcknowlegdeDelivredMessage(Channel channel, T delivredMessage);
        void RejectDeliveredMessage(Channel channel, T delivredMessage);
        void SendDelivredMessageToErrorBus(Channel channel, T delivredMessage);
    }
}
