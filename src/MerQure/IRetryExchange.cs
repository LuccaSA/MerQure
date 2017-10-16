using MerQure.Configuration;
using MerQure.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerQure
{
    public interface IRetryExchange<T> where T : IAmqpIdentity
    {
        void Publish(string queueName, T message);
        void Consume(string queueName, EventHandler<T> callback);
        void ApplyRetryStrategy(string queueName, T message);
        void AcknowlegdeDelivredMessage(string queueName, T delivredMessage);
        void RejectDeliveredMessage(string queueName, T delivredMessage);
        void SendDelivredMessageToErrorExchange(string queueName, T delivredMessage);
    }
}
