using MerQure.Tools;
using MerQure.Tools.Configurations;
using MerQure.Tools.Exchanges;
using MerQure.Tools.Samples.RetryExchangeExample.Domain;
using MerQure.Tools.Samples.RetryExchangeExample.Infra;
using System;
using System.Collections.Generic;


namespace MerQure.Tools.Samples.RetryExchangeExample.RetryExchangeExample.Infra
{
    public class SomethingService : ISomethingService
    {
        public const string ExchangeName = "something";
        private readonly RetryExchangeService _retryExchangeService;

        public IExchange<Something> SomethingExchange { get; set; }

        public SomethingService(RetryExchangeService retryExchangeService)
        {
            _retryExchangeService = retryExchangeService;
            RetryExchangeConfiguration somethingConfiguration = new RetryExchangeConfiguration
            {
                Channels = SomethingChannels.GetAllChannels(),
                ExchangeName = ExchangeName,
                DelaysInMsBetweenEachRetry = new List<int>
                    {
                        {5000 },
                        {6000 },
                        {10000 }
                    },
                EndOnErrorExchange = true
            };

            SomethingExchange = _retryExchangeService.CreateNewExchange<Something>(somethingConfiguration);
        }

        public void Send(Something something)
        {
            SomethingExchange.Publish(SomethingChannels.MessageSended, something);
        }

        public void Consume(EventHandler<Something> callback)
        {
            SomethingExchange.Consume(SomethingChannels.MessageSended, (object sender, Something something) =>
            {
                callback(this, something);
            });
        }

        public void RetryLater(Something something)
        {
            SomethingExchange.ApplyRetryStrategy(SomethingChannels.MessageSended, something);
        }

        public void Acknowlegde(Something something)
        {
            SomethingExchange.AcknowlegdeDelivredMessage(SomethingChannels.MessageSended, something);
        }

        public void SendOnError(Something something)
        {
            SomethingExchange.SendDelivredMessageToErrorExchange(SomethingChannels.MessageSended, something);
        }
    }
}
