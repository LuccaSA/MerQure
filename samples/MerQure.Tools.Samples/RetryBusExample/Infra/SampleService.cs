using MerQure.Tools.Buses;
using MerQure.Tools.Configurations;
using MerQure.Tools.Samples.RetryBusExample.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MerQure.Tools.Samples.RetryBusExample.Infra;

public class SampleService : ISampleService
{
    public const string BusName = "sample";
    private readonly IRetryBusService _retryBusService;

    public SampleService(IRetryBusService retryBusService)
    {
        _retryBusService = retryBusService;
    }

    private Task<IBus<Sample>> CreateSampleBus()
    {
        RetryStrategyConfiguration sampleConfiguration = new RetryStrategyConfiguration
        {
            Channels = SampleChannels.GetAllChannels(),
            BusName = BusName,
            DelaysInMsBetweenEachRetry = new List<int>
            {
                { 5000 },
                { 6000 },
                { 10000 }
            },
            MessageIsGoingIntoErrorBusAfterAllRepeat = true,
            DeliveryDelayInMilliseconds = 1000
        };

        return _retryBusService.CreateNewBusAsync<Sample>(sampleConfiguration, isQuorum: true);
    }

    public async Task SendAsync(Sample sample)
    {
        var sampleBus = await CreateSampleBus();
        await sampleBus.PublishAsync(SampleChannels.MessageSended, sample, true);
    }

    public async Task ConsumeAsync(EventHandler<Sample> onSampleReceived)
    {
        var sampleBus = await CreateSampleBus();
        await sampleBus.OnMessageReceivedAsync(SampleChannels.MessageSended, (object sender, Sample sample) =>
        {
            onSampleReceived(this, sample);
        });
    }

    public async Task RetryLaterAsync(Sample sample)
    {
        var sampleBus = await CreateSampleBus();
        await sampleBus.ApplyRetryStrategyAsync(SampleChannels.MessageSended, sample);
    }

    public async Task AcknowlegdeAsync(Sample sample)
    {
        var sampleBus = await CreateSampleBus();
        await sampleBus.AcknowlegdeDeliveredMessageAsync(SampleChannels.MessageSended, sample);
    }

    public async Task SendOnErrorAsync(Sample sample)
    {
        var sampleBus = await CreateSampleBus();
        await sampleBus.SendDeliveredMessageToErrorBusAsync(SampleChannels.MessageSended, sample);
    }
}