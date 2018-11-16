using MerQure.Tools.Buses;
using MerQure.Tools.Configurations;
using MerQure.Tools.Samples.RetryBusExample.Domain;
using MerQure.Tools.Samples.RetryBusExample.Infra;
using System;
using System.Collections.Generic;

namespace MerQure.Tools.Samples.RetryBusExample.RetryExchangeExample.Infra
{
	public class SampleService : ISampleService
    {
        public const string BusName = "sample";
        private readonly IRetryBusService _retryBusService;

        public IBus<Sample> SampleBus { get; set; }

        public SampleService(IRetryBusService retryBusService)
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
                MessageIsGoingIntoErrorBusAfterAllRepeat = true,
                DeliveryDelayInMilliseconds = 1000
            };

            SampleBus = _retryBusService.CreateNewBus<Sample>(sampleConfiguration);
        }

        public void Send(Sample sample)
        {
            SampleBus.Publish(SampleChannels.MessageSent, sample, true);
            Console.WriteLine($"Send: {sample.DeliveryTag}");
        }

        public void Consume(EventHandler<Sample> onSampleReceived)
        {
            SampleBus.OnMessageReceived(SampleChannels.MessageSent, (object sender, Sample sample) =>
            {
                onSampleReceived(this, sample);
                Console.WriteLine($"Received: {sample.DeliveryTag}");
            });
        }

        public void RetryLater(Sample sample)
        {
            SampleBus.ApplyRetryStrategy(SampleChannels.MessageSent, sample);
        }

        public void Acknowlegde(Sample sample)
        {
            SampleBus.AcknowlegdeDeliveredMessage(SampleChannels.MessageSent, sample);
        }

        public void SendOnError(Sample sample)
        {
            SampleBus.SendDeliveredMessageToErrorBus(SampleChannels.MessageSent, sample);
        }
    }
}
