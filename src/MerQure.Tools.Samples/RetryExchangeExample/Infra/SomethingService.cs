using MerQure.Tools;
using MerQure.Tools.Configurations;
using MerQure.Tools.Buses;
using MerQure.Tools.Samples.RetryExchangeExample.Domain;
using MerQure.Tools.Samples.RetryExchangeExample.Infra;
using System;
using System.Collections.Generic;


namespace MerQure.Tools.Samples.RetryExchangeExample.RetryExchangeExample.Infra
{
    public class SomethingService : ISomethingService
    {
        public const string BusName = "something";
        private readonly RetryBusService _retryBusService;

        public IBus<Something> SomethingBus { get; set; }

        public SomethingService(RetryBusService retryBusService)
        {
            _retryBusService = retryBusService;
            RetryStrategyConfiguration somethingConfiguration = new RetryStrategyConfiguration
            {
                Channels = SomethingChannels.GetAllChannels(),
                BusName = BusName,
                DelaysInMsBetweenEachRetry = new List<int>
                    {
                        {5000 },
                        {6000 },
                        {10000 }
                    },
                MessageIsGoingIntoErrorBusAfterAllRepeat = true
            };

            SomethingBus = _retryBusService.CreateNewBus<Something>(somethingConfiguration);
        }

        public void Send(Something something)
        {
            SomethingBus.Publish(SomethingChannels.MessageSended, something);
        }

        public void Consume(EventHandler<Something> callback)
        {
            SomethingBus.Consume(SomethingChannels.MessageSended, (object sender, Something something) =>
            {
                callback(this, something);
            });
        }

        public void RetryLater(Something something)
        {
            SomethingBus.ApplyRetryStrategy(SomethingChannels.MessageSended, something);
        }

        public void Acknowlegde(Something something)
        {
            SomethingBus.AcknowlegdeDelivredMessage(SomethingChannels.MessageSended, something);
        }

        public void SendOnError(Something something)
        {
            SomethingBus.SendDelivredMessageToErrorBus(SomethingChannels.MessageSended, something);
        }
    }
}
