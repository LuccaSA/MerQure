using MerQure.Configuration;
using MerQure.Content;
using MerQure.RetryExchange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerQure.RbMQ
{
    public class RetryExchangeService : IRetryExchangeService
    {
        private readonly IMessagingService _messagingService;

        public RetryExchangeService(IMessagingService messagingService)
        {
            _messagingService = messagingService;
        }

        public IRetryExchange<T> Get<T>(RetryExchangeConfiguration configuration) where T : IAmqpIdentity
        {
            ApplyNewConfiguration(configuration);

            RetryPublisher<T> publisher = new RetryPublisher<T>(_messagingService, configuration);
            RetryConsumer<T> consumer = new RetryConsumer<T>(_messagingService, configuration, publisher);

            return new RetryExchange<T>(publisher, consumer);
        }

        private void ApplyNewConfiguration(RetryExchangeConfiguration configuration)
        {
            if (configuration.QueuesName == null || !configuration.QueuesName.Any())
                throw new ArgumentNullException(nameof(configuration.QueuesName));

            CreateMainExchange(configuration);
            CreateRetryExchangeIfNecessary(configuration);
            CreateErrorExchange(configuration);
        }

        private void CreateErrorExchange(RetryExchangeConfiguration configuration)
        {
            string errorExchangeName = $"{configuration.ExchangeName}.error";
            _messagingService.DeclareExchange(errorExchangeName, Constants.ExchangeTypeDirect);
            foreach (string queueName in configuration.QueuesName)
            {
                string errorQueueName = $"{queueName}.error";
                _messagingService.DeclareQueue(errorQueueName);
                _messagingService.DeclareBinding(errorExchangeName, errorQueueName, errorQueueName);
            }
        }

        private void CreateRetryExchangeIfNecessary(RetryExchangeConfiguration configuration)
        {
            if (configuration.DelaysInMillisecondsBetweenEachRetry.Any())
            {
                string retryExchangeName = $"{configuration.ExchangeName}.retry";
                _messagingService.DeclareExchange(retryExchangeName, Constants.ExchangeTypeDirect);
                foreach (int delay in configuration.DelaysInMillisecondsBetweenEachRetry)
                {
                    foreach (string queueName in configuration.QueuesName)
                    {
                        string retryQueueName = $"{queueName}.{delay}";
                        _messagingService.DeclareQueueWithDeadLetterPolicy(retryQueueName, configuration.ExchangeName, delay, null);
                        _messagingService.DeclareBinding(retryExchangeName, retryQueueName, $"{queueName}.{delay}", null);
                    }
                }
            }
        }

        private void CreateMainExchange(RetryExchangeConfiguration configuration)
        {
            _messagingService.DeclareExchange(configuration.ExchangeName, Constants.ExchangeTypeTopic);
            foreach (string queueName in configuration.QueuesName)
            {
                _messagingService.DeclareQueue(queueName);
                _messagingService.DeclareBinding(configuration.ExchangeName, queueName, $"{queueName}.#");
            }
        }
    }
}
