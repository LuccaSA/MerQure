using MerQure;
using MerQure.Messages;
using MerQure.Tools.Configurations;
using MerQure.Tools.Buses;
using MerQure.Tools.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerQure.Tools
{
    public class RetryBusService : IRetryBusService
    {
        private readonly IMessagingService _messagingService;

        public RetryBusService(IMessagingService messagingService)
        {
            _messagingService = messagingService;
        }

        public IBus<T> CreateNewBus<T>(RetryStrategyConfiguration configuration) where T : IDelivered
        {
            ApplyNewConfiguration(configuration);

            Publisher<T> producer = new Publisher<T>(_messagingService, configuration);
            RetryConsumer<T> consumer = new RetryConsumer<T>(_messagingService, configuration, producer);

            return new Bus<T>(producer, consumer);
        }

        private void ApplyNewConfiguration(RetryStrategyConfiguration configuration)
        {
            if (configuration.Channels == null || !configuration.Channels.Any())
                throw new ArgumentNullException(nameof(configuration.Channels));

            CreateMainExchange(configuration);
            CreateRetryExchangeIfNecessary(configuration);
            CreateErrorExchange(configuration);
        }

        private void CreateErrorExchange(RetryStrategyConfiguration configuration)
        {
            string errorExchangeName = $"{configuration.BusName}.{RetryStrategyConfiguration.ErrorExchangeSuffix}";
            _messagingService.DeclareExchange(errorExchangeName, Constants.ExchangeTypeDirect);
            foreach (Channel channel in configuration.Channels)
            {
                string errorQueueName = $"{channel.Value}.{RetryStrategyConfiguration.ErrorExchangeSuffix}";
                _messagingService.DeclareQueue(errorQueueName);
                _messagingService.DeclareBinding(errorExchangeName, errorQueueName, errorQueueName);
            }
        }

        /// <summary>
        /// Retry exchange is also used to delivery message with delay
        /// </summary>
        private void CreateRetryExchangeIfNecessary(RetryStrategyConfiguration configuration)
        {
            if (configuration.DelaysInMsBetweenEachRetry.Any() || configuration.DeliveryDelayInMilliseconds != 0)
            {
                string retryExchangeName = $"{configuration.BusName}.{RetryStrategyConfiguration.RetryExchangeSuffix}";
                _messagingService.DeclareExchange(retryExchangeName, Constants.ExchangeTypeDirect);
                foreach (int delay in configuration.DelaysInMsBetweenEachRetry)
                {
                    CreateRetryChannelsForOneDelay(configuration, retryExchangeName, delay);
                }

                if (!configuration.DelaysInMsBetweenEachRetry.Contains(configuration.DeliveryDelayInMilliseconds))
                {
                    CreateRetryChannelsForOneDelay(configuration, retryExchangeName, configuration.DeliveryDelayInMilliseconds);
                }
            }
        }

        private void CreateRetryChannelsForOneDelay(RetryStrategyConfiguration configuration, string retryExchangeName, int delay)
        {
            foreach (Channel channel in configuration.Channels)
            {
                string retryQueueName = $"{channel.Value}.{delay}";
                _messagingService.DeclareQueueWithDeadLetterPolicy(retryQueueName, configuration.BusName, delay, null);
                _messagingService.DeclareBinding(retryExchangeName, retryQueueName, $"{channel.Value}.{delay}", null);
            }
        }

        private void CreateMainExchange(RetryStrategyConfiguration configuration)
        {
            _messagingService.DeclareExchange(configuration.BusName, Constants.ExchangeTypeTopic);
            foreach (Channel channel in configuration.Channels)
            {
                _messagingService.DeclareQueue(channel.Value);
                _messagingService.DeclareBinding(configuration.BusName, channel.Value, $"{channel.Value}.#");
            }
        }

    }
}
