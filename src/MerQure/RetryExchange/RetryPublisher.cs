using MerQure.Configuration;
using MerQure.Content;
using MerQure.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerQure.RetryExchange
{
    internal class RetryPublisher<T> where T : IAmqpIdentity
    {

        private readonly RetryExchangeConfiguration _retryExchangeConfiguration;
        private readonly IMessagingService _messagingService;

        public RetryPublisher(IMessagingService messagingService, RetryExchangeConfiguration messageBrokerConfiguration)
        {
            _retryExchangeConfiguration = messageBrokerConfiguration;
            _messagingService = messagingService;
        }


        public void Publish(string queueName, T message)
        {
            using (var publisher = _messagingService.GetPublisher(_retryExchangeConfiguration.ExchangeName, true))
            {
                var encapsuledMessage = new EncapsuledMessage<T>
                {
                    TechnicalInformation = new MessageTechnicalInformations
                    {
                        NumberOfRetry = 0
                    },
                    OriginalMessage = message
                };
                TryPublishWithBrokerAcknowledgement(publisher, queueName, JsonConvert.SerializeObject(encapsuledMessage));
            }
        }

        public void PublishOnRetryExchange(string queueName, T message, MessageTechnicalInformations technicalInformations)
        {
            technicalInformations.NumberOfRetry++;
            EncapsuledMessage<T> encapsuledMessage = new EncapsuledMessage<T>
            {
                OriginalMessage = message,
                TechnicalInformation = technicalInformations
            };
            List<int> delays = _retryExchangeConfiguration.DelaysInMillisecondsBetweenEachRetry;
            int delay;
            if (delays.Count >= technicalInformations.NumberOfRetry)
            {
                delay = delays[technicalInformations.NumberOfRetry - 1];
            }
            else
            {
                delay = delays.Last();
            }

            string queueRenamed = $"{queueName}.{delay}";
            using (var publisher = _messagingService.GetPublisher($"{_retryExchangeConfiguration.ExchangeName}.retry", true))
            {
                TryPublishWithBrokerAcknowledgement(publisher, queueRenamed, JsonConvert.SerializeObject(encapsuledMessage));
            }
        }

        public void PublishOnErrorExchange(string queueName, T message, MessageTechnicalInformations technicalInformations)
        {
            EncapsuledMessage<T> encapsuledMessage = new EncapsuledMessage<T>
            {
                OriginalMessage = message,
                TechnicalInformation = technicalInformations
            };
            string queueRenamed = $"{queueName}.error";
            using (var publisher = _messagingService.GetPublisher($"{_retryExchangeConfiguration.ExchangeName}.error", true))
            {
                TryPublishWithBrokerAcknowledgement(publisher, queueRenamed, JsonConvert.SerializeObject(encapsuledMessage));
            }
        }

        public void TryPublishWithBrokerAcknowledgement(IPublisher publisher, string queueName, string message)
        {
            bool published = publisher.PublishWithAcknowledgement(queueName, message);
            if (!published)
            {
                throw new MerQureException($"unable to send message to the broker. {Environment.NewLine}Channel : {queueName}{Environment.NewLine}Message : {message}");
            }
        }
    }
}
