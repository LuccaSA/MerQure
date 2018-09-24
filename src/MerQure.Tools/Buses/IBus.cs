using MerQure.Messages;
using MerQure.Tools.Configurations;
using System;
using System.Collections.Generic;

namespace MerQure.Tools.Buses
{
	public interface IBus<T> where T : IDelivered
    {
        void Publish(Channel channel, T message, bool applyDeliveryDeplay = false);
        void PublishWithTransaction(Channel channel, IEnumerable<T> messages, bool applyDeliveryDeplay = false);
        MessageInformations PublishForceRetryAttemptNumber(Channel channel, T message, int attemptNumber);
        void OnMessageReceived(Channel channel, EventHandler<T> callback);
        MessageInformations ApplyRetryStrategy(Channel channel, T deliveredMessage);
        void AcknowlegdeDeliveredMessage(Channel channel, T deliveredMessage);
        void RejectDeliveredMessage(Channel channel, T deliveredMessage);
        void SendDeliveredMessageToErrorBus(Channel channel, T deliveredMessage);
    }
}
