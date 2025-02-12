using MerQure.Messages;
using MerQure.Tools.Configurations;
using MerQure.Tools.Buses;
using System;
using System.Linq;

namespace MerQure.Tools
{
    public class RetryBusService : IRetryBusService
    {
        private readonly IMessagingService _messagingService;

        public RetryBusService(IMessagingService messagingService)
        {
            _messagingService = messagingService;
        }

        public IBus<T> CreateNewBus<T>(RetryStrategyConfiguration configuration, bool isQuorum) where T : IDelivered
        {
            ApplyNewConfiguration(configuration, isQuorum);

            Publisher<T> producer = new Publisher<T>(_messagingService, configuration);
            RetryConsumer<T> consumer = new RetryConsumer<T>(_messagingService, configuration, producer);

            return new Bus<T>(producer, consumer);
        }

        private void ApplyNewConfiguration(RetryStrategyConfiguration configuration, bool isQuorum)
        {
            if (configuration.Channels == null || !configuration.Channels.Any())
            {
                throw new ArgumentNullException(nameof(configuration.Channels));
            }

            if(configuration.DelaysInMsBetweenEachRetry == null)
            {
                throw new ArgumentNullException(nameof(configuration.Channels));
            }

            CreateMainExchange(configuration, isQuorum);
            CreateRetryExchangeIfNecessary(configuration, isQuorum);
            CreateErrorExchange(configuration, isQuorum);
        }

        private void CreateErrorExchange(RetryStrategyConfiguration configuration, bool isQuorum)
        {
            string errorExchangeName = $"{configuration.BusName}.{RetryStrategyConfiguration.ErrorExchangeSuffix}";
            _messagingService.DeclareExchange(errorExchangeName, Constants.ExchangeTypeDirect);
            foreach (Channel channel in configuration.Channels)
            {
                string errorQueueName = $"{channel.Value}.{RetryStrategyConfiguration.ErrorExchangeSuffix}";
                var effectiveQueueName = _messagingService.DeclareQueue(errorQueueName, isQuorum);
                _messagingService.DeclareBinding(errorExchangeName, effectiveQueueName , errorQueueName);
            }
        }

        /// <summary>
        /// Retry exchange is also used to delivery message with delay
        /// </summary>
        private void CreateRetryExchangeIfNecessary(RetryStrategyConfiguration configuration, bool isQuorum)
        {
            if (configuration.DelaysInMsBetweenEachRetry.Any() || configuration.DeliveryDelayInMilliseconds != 0)
            {
                string retryExchangeName = $"{configuration.BusName}.{RetryStrategyConfiguration.RetryExchangeSuffix}";
                _messagingService.DeclareExchange(retryExchangeName, Constants.ExchangeTypeDirect);
                foreach (int delay in configuration.DelaysInMsBetweenEachRetry)
                {
                    CreateRetryChannelsForOneDelay(configuration, retryExchangeName, delay, isQuorum);
                }

                if (!configuration.DelaysInMsBetweenEachRetry.Contains(configuration.DeliveryDelayInMilliseconds) && configuration.DeliveryDelayInMilliseconds != 0)
                {
                    CreateRetryChannelsForOneDelay(configuration, retryExchangeName, configuration.DeliveryDelayInMilliseconds, isQuorum);
                }
            }
        }

        private void CreateRetryChannelsForOneDelay(RetryStrategyConfiguration configuration, string retryExchangeName, int delay, bool isQuorum)
        {
            foreach (Channel channel in configuration.Channels)
            {
                string retryQueueName = $"{channel.Value}.{delay}";
                var effectiveQueueName = _messagingService.DeclareQueueWithDeadLetterPolicy(retryQueueName, configuration.BusName, delay, null, isQuorum);
                _messagingService.DeclareBinding(retryExchangeName, effectiveQueueName, $"{channel.Value}.{delay}", null);
            }
        }

        private void CreateMainExchange(RetryStrategyConfiguration configuration, bool isQuorum)
        {
            _messagingService.DeclareExchange(configuration.BusName, Constants.ExchangeTypeTopic);
            foreach (Channel channel in configuration.Channels)
            {
                var effectiveQueueName = _messagingService.DeclareQueue(channel.Value, isQuorum);
                _messagingService.DeclareBinding(configuration.BusName, effectiveQueueName, $"{channel.Value}.#");
            }
        }
    }
}
