using MerQure.Configuration;
using MerQure.RbMQ;
using MerQure.Samples.RetryExchange.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerQure.Samples.RetryExchange.Infra
{
    public class SomethingService : ISomethingService
    {
        public const string ExchangeName = "something";
        public IRetryExchange<Something> SomethingExchange { get; set; }

        public SomethingService()
        {
            var somethingConfiguration = new RetryExchangeConfiguration()
            {
                QueuesName = SomethingQueuesName.GetAll(),
                ExchangeName = ExchangeName,
                DelaysInMillisecondsBetweenEachRetry = new List<int>
                    {
                        {6000 },
                        {10000 },
                        {100000 }
                    },
                EndOnErrorExchange = true
            };
            IMessagingService messagingService = new MessagingService();
            SomethingExchange = new RetryExchangeService(messagingService).Get<Something>(somethingConfiguration);
        }

        public void Send(Something something)
        {
            SomethingExchange.Publish(SomethingQueuesName.MessageSended, something);
        }

        public void Consume(EventHandler<Something> callback)
        {
            SomethingExchange.Consume(SomethingQueuesName.MessageSended, (object sender, Something something) =>
            {
                callback(this, something);
            });
        }

        public void RetryLater(Something something)
        {
            SomethingExchange.ApplyRetryStrategy(SomethingQueuesName.MessageSended, something);
        }

        public void Acknowlegde(Something something)
        {
            SomethingExchange.AcknowlegdeDelivredMessage(SomethingQueuesName.MessageSended, something);
        }

        public void SendOnError(Something something)
        {
            SomethingExchange.SendDelivredMessageToErrorExchange(SomethingQueuesName.MessageSended, something);
        }
    }
}
