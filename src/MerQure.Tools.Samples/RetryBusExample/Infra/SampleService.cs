using MerQure.Tools;
using MerQure.Tools.Configurations;
using MerQure.Tools.Buses;
using MerQure.Tools.Samples.RetryBusExample.Domain;
using MerQure.Tools.Samples.RetryBusExample.Infra;
using System;
using System.Collections.Generic;


namespace MerQure.Tools.Samples.RetryBusExample.RetryExchangeExample.Infra
{
    public class SampleService : ISampleService
    {
        public const string BusName = "sample";
        private readonly RetryBusService _retryBusService;

        public IBus<Sample> SampleBus { get; set; }

        public SampleService(RetryBusService retryBusService)
        {
            _retryBusService = retryBusService;
            RetryStrategyConfiguration sampleConfiguration = new RetryStrategyConfiguration
            {
                Channels = SampleChannels.GetAllChannels(),
                BusName = BusName,
                DelaysInMsBetweenEachRetry = new List<int>
                    {
                        {5000 },
                        {6000 },
                        {10000 }
                    },
                MessageIsGoingIntoErrorBusAfterAllRepeat = true
            };

            SampleBus = _retryBusService.CreateNewBus<Sample>(sampleConfiguration);
        }

        public void Send(Sample sample)
        {
            SampleBus.Publish(SampleChannels.MessageSended, sample);
        }

        public void Consume(EventHandler<Sample> callback)
        {
            SampleBus.OnMessageReceived(SampleChannels.MessageSended, (object sender, Sample sample) =>
            {
                callback(this, sample);
            });
        }

        public void RetryLater(Sample sample)
        {
            SampleBus.ApplyRetryStrategy(SampleChannels.MessageSended, sample);
        }

        public void Acknowlegde(Sample sample)
        {
            SampleBus.AcknowlegdeDeliveredMessage(SampleChannels.MessageSended, sample);
        }

        public void SendOnError(Sample sample)
        {
            SampleBus.SendDeliveredMessageToErrorBus(SampleChannels.MessageSended, sample);
        }
    }
}
