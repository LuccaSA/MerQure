using MerQure.Messages;
using MerQure.Tools.Configurations;
using MerQure.Tools.Buses;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MerQure.Tools;

public class RetryBusService : IRetryBusService
{
    private readonly IMessagingService _messagingService;

    public RetryBusService(IMessagingService messagingService)
    {
        _messagingService = messagingService;
    }

    public async Task<IBus<T>> CreateNewBusAsync<T>(RetryStrategyConfiguration configuration, bool isQuorum) where T : IDelivered
    {
        await ApplyNewConfigurationAsync(configuration, isQuorum);

        Publisher<T> producer = new Publisher<T>(_messagingService, configuration);
        RetryConsumer<T> consumer = new RetryConsumer<T>(_messagingService, configuration, producer);

        return new Bus<T>(producer, consumer);
    }

    private async Task ApplyNewConfigurationAsync(RetryStrategyConfiguration configuration, bool isQuorum)
    {
        if (configuration.Channels == null || !configuration.Channels.Any())
        {
            throw new ArgumentNullException(nameof(configuration.Channels));
        }

        if(configuration.DelaysInMsBetweenEachRetry == null)
        {
            throw new ArgumentNullException(nameof(configuration.Channels));
        }

        await CreateMainExchange(configuration, isQuorum);
        await CreateRetryExchangeIfNecessaryAsync(configuration, isQuorum);
        await CreateErrorExchange(configuration, isQuorum);
    }

    private async Task CreateErrorExchange(RetryStrategyConfiguration configuration, bool isQuorum)
    {
        string errorExchangeName = $"{configuration.BusName}.{RetryStrategyConfiguration.ErrorExchangeSuffix}";
        await _messagingService.DeclareExchangeAsync(errorExchangeName, Constants.ExchangeTypeDirect);
        foreach (var channelValue in configuration.Channels.Select(channel => channel.Value ))
        {
            string errorQueueName = $"{channelValue}.{RetryStrategyConfiguration.ErrorExchangeSuffix}";
            var effectiveQueueName = await _messagingService.DeclareQueueAsync(errorQueueName, isQuorum);
            await _messagingService.DeclareBindingAsync(errorExchangeName, effectiveQueueName , errorQueueName);
        }
    }

    /// <summary>
    /// Retry exchange is also used to delivery message with delay
    /// </summary>
    private async Task CreateRetryExchangeIfNecessaryAsync(RetryStrategyConfiguration configuration, bool isQuorum)
    {
        if (configuration.DelaysInMsBetweenEachRetry.Any() || configuration.DeliveryDelayInMilliseconds != 0)
        {
            string retryExchangeName = $"{configuration.BusName}.{RetryStrategyConfiguration.RetryExchangeSuffix}";
            await _messagingService.DeclareExchangeAsync(retryExchangeName, Constants.ExchangeTypeDirect);
            foreach (int delay in configuration.DelaysInMsBetweenEachRetry)
            {
                await CreateRetryChannelsForOneDelayAsync(configuration, retryExchangeName, delay, isQuorum);
            }

            if (!configuration.DelaysInMsBetweenEachRetry.Contains(configuration.DeliveryDelayInMilliseconds) && configuration.DeliveryDelayInMilliseconds != 0)
            {
                await CreateRetryChannelsForOneDelayAsync(configuration, retryExchangeName, configuration.DeliveryDelayInMilliseconds, isQuorum);
            }
        }
    }

    private async Task CreateRetryChannelsForOneDelayAsync(RetryStrategyConfiguration configuration, string retryExchangeName, int delay, bool isQuorum)
    {
        foreach (var channelValue in configuration.Channels.Select(channel => channel.Value ))
        {
            string retryQueueName = $"{channelValue}.{delay}";
            var effectiveQueueName = await _messagingService.DeclareQueueWithDeadLetterPolicyAsync(retryQueueName, configuration.BusName, delay, null, isQuorum);
            await _messagingService.DeclareBindingAsync(retryExchangeName, effectiveQueueName, $"{channelValue}.{delay}", null);
        }
    }

    private async Task CreateMainExchange(RetryStrategyConfiguration configuration, bool isQuorum)
    {
        await _messagingService.DeclareExchangeAsync(configuration.BusName, Constants.ExchangeTypeTopic);
        foreach (var channelValue in configuration.Channels.Select(channel => channel.Value ))
        {
            var effectiveQueueName = await _messagingService.DeclareQueueAsync(channelValue, isQuorum);
            await _messagingService.DeclareBindingAsync(configuration.BusName, effectiveQueueName, $"{channelValue}.#");
        }
    }
}