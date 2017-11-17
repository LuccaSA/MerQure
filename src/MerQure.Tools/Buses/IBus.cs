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
        MessageInformations PublishForceRetryAttemptNumber(Channel channel, T message, int attemptNumber);
        void PublishWithTransaction(Channel channel, IEnumerable<T> message);
        void OnMessageReceived(Channel channel, EventHandler<T> callback);
        MessageInformations ApplyRetryStrategy(Channel channel, T deliveredMessage);
        void AcknowlegdeDeliveredMessage(Channel channel, T deliveredMessage);
        void RejectDeliveredMessage(Channel channel, T deliveredMessage);
        void SendDeliveredMessageToErrorBus(Channel channel, T deliveredMessage);
    }
}
