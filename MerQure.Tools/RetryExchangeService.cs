using MerQure;
using MerQure.Tools.Configurations;
using MerQure.Tools.Exchanges;
using MerQure.Tools.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerQure.Tools
{
    public class RetryExchangeService
    {        
        private readonly IMessagingService _messagingService;
        
        public RetryExchangeService(IMessagingService messagingService)
        {
            _messagingService = messagingService;
        }

        public IExchange<T> CreateNewExchange<T>(RetryExchangeConfiguration configuration) where T : IAMQPIdentity
        {
            ApplyNewConfiguration(configuration);

            Publisher<T> producer = new Publisher<T>(_messagingService, configuration);
            RetryConsumer<T> consumer = new RetryConsumer<T>(_messagingService, configuration, producer);

            return new Exchange<T>(producer, consumer);
        }

        private void ApplyNewConfiguration(RetryExchangeConfiguration configuration)
        {
            if (configuration.Channels == null || !configuration.Channels.Any())
                throw new ArgumentNullException(nameof(configuration.Channels));

            CreateMainExchange(configuration);
            CreateRetryExchangeIfNecessary(configuration);
            CreateErrorExchange(configuration);
        }

        private void CreateErrorExchange(RetryExchangeConfiguration configuration)
        {
            string errorExchangeName = $"{configuration.ExchangeName}.error";
            _messagingService.DeclareExchange(errorExchangeName, MerQure.Constants.ExchangeTypeDirect);
            foreach (Channel channel in configuration.Channels)
            {
                string errorQueueName = $"{channel.Value}.error";
                _messagingService.DeclareQueue(errorQueueName);
                _messagingService.DeclareBinding(errorExchangeName, errorQueueName, errorQueueName);
            }
        }

        private void CreateRetryExchangeIfNecessary(RetryExchangeConfiguration configuration)
        {
            if (configuration.DelaysInMsBetweenEachRetry.Any())
            {
                string retryExchangeName = $"{configuration.ExchangeName}.retry";
                _messagingService.DeclareExchange(retryExchangeName, MerQure.Constants.ExchangeTypeDirect);
                foreach (int delay in configuration.DelaysInMsBetweenEachRetry)
                {
                    foreach (Channel channel in configuration.Channels)
                    {
                        string retryQueueName = $"{channel.Value}.{delay}";
                        _messagingService.DeclareQueueWithDeadLetterPolicy(retryQueueName, configuration.ExchangeName, delay, null);
                        _messagingService.DeclareBinding(retryExchangeName, retryQueueName, $"{channel.Value}.{delay}", null);
                    }
                }
            }
        }

        private void CreateMainExchange(RetryExchangeConfiguration configuration)
        {
            _messagingService.DeclareExchange(configuration.ExchangeName, Constants.ExchangeTypeTopic);
            foreach (Channel channel in configuration.Channels)
            {
                _messagingService.DeclareQueue(channel.Value);
                _messagingService.DeclareBinding(configuration.ExchangeName, channel.Value, $"{channel.Value}.#");
            }
        }

    }
}
