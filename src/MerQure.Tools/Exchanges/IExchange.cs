using MerQure.Tools.Configurations;
using MerQure.Tools.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace MerQure.Tools.Exchanges
{
    public interface IExchange<T> where T : IDelivered
    {
        void Publish(Channel binding, T message);
        void PublishWithTransaction(Channel binding, IEnumerable<T> message);
        void Consume(Channel binding, EventHandler<T> callback);
        void ApplyRetryStrategy(Channel binding, T message);
        void AcknowlegdeDelivredMessage(Channel channel, T delivredMessage);
        void RejectDeliveredMessage(Channel channel, T delivredMessage);
        void SendDelivredMessageToErrorExchange(Channel channel, T delivredMessage);
    }
}
