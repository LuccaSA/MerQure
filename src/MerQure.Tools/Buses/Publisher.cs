using MerQure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using MerQure.Tools.Messages;
using MerQure.Tools.Configurations;
using MerQure.Tools.Exceptions;
using MerQure.Messages;

namespace MerQure.Tools.Buses
{
    internal class Publisher<T> where T : IDelivered
    {
        private readonly RetryStrategyConfiguration _messageBrokerConfiguration;
        private readonly IMessagingService _messagingService;


        public Publisher(IMessagingService messagingService, RetryStrategyConfiguration retryConfiguration)
        {
            _messagingService = messagingService;
            _messageBrokerConfiguration = retryConfiguration;
        }

        public void PublishWithTransaction(Channel channel, IEnumerable<T> messages, bool applyDeliveryDeplay)
        {
            var serializedMessages = new List<string>();
            foreach (T message in messages)
            {
                serializedMessages.Add(JsonConvert.SerializeObject(CreateRetryMessage(message)));
            }
            string bindingValue = channel.Value;
            string busName = _messageBrokerConfiguration.BusName;
            if (applyDeliveryDeplay && _messageBrokerConfiguration.DeliveryDelayInMilliseconds > 0)
            {
                bindingValue = $"{bindingValue}.{_messageBrokerConfiguration.DeliveryDelayInMilliseconds}";
                busName = $"{busName}.{ RetryStrategyConfiguration.RetryExchangeSuffix}";
            }
            using (var publisher = _messagingService.GetPublisher(busName, false))
            {
                PublishWithTransaction(publisher, bindingValue, serializedMessages);
            }
        }

        public void Publish(Channel channel, T message, bool applyDeliveryDeplay)
        {
            var encapsuledMessage = CreateRetryMessage(message);
            string bindingValue = channel.Value;
            string busName = _messageBrokerConfiguration.BusName;
            if (applyDeliveryDeplay && _messageBrokerConfiguration.DeliveryDelayInMilliseconds > 0)
            {
                bindingValue = $"{bindingValue}.{_messageBrokerConfiguration.DeliveryDelayInMilliseconds}";
                busName = $"{busName}.{ RetryStrategyConfiguration.RetryExchangeSuffix}";
            }
            using (var publisher = _messagingService.GetPublisher(busName, true))
            {
                TryPublishWithBrokerAcknowledgement(publisher, bindingValue, JsonConvert.SerializeObject(encapsuledMessage));
            }
        }
        
        internal void PublishOnRetryExchange(Channel channel, T message, RetryInformations retryInformations)
        {
            List<int> delays = _messageBrokerConfiguration.DelaysInMsBetweenEachRetry;
            int delay = 0;
            if (delays.Count() >= retryInformations.NumberOfRetry)
            {
                delay = delays[retryInformations.NumberOfRetry];
            }
            else
            {
                delay = delays.Last();
            }
            retryInformations.NumberOfRetry++;
            RetryMessage<T> retryMessage = new RetryMessage<T>
            {
                OriginalMessage = message,
                RetryInformations = retryInformations
            };

            string bindingValue = $"{channel.Value}.{delay}";
            using (var publisher = _messagingService.GetPublisher($"{_messageBrokerConfiguration.BusName}.{RetryStrategyConfiguration.RetryExchangeSuffix}", true))
            {
                TryPublishWithBrokerAcknowledgement(publisher, bindingValue, JsonConvert.SerializeObject(retryMessage));
            }
        }

        internal void PublishOnErrorExchange(Channel channel, T message, RetryInformations technicalInformations)
        {
            string errorChanel = $"{channel.Value}.error";
            RetryMessage<T> retryMessage = new RetryMessage<T>
            {
                OriginalMessage = message,
                RetryInformations = technicalInformations
            };
            using (var publisher = _messagingService.GetPublisher($"{_messageBrokerConfiguration.BusName}.{RetryStrategyConfiguration.ErrorExchangeSuffix}", true))
            {
                TryPublishWithBrokerAcknowledgement(publisher, errorChanel, JsonConvert.SerializeObject(retryMessage));
            }
        }

        internal void TryPublishWithBrokerAcknowledgement(IPublisher publisher, string channelName, string message)
        {
            bool published = publisher.PublishWithAcknowledgement(channelName, message);
            if (published == false)
            {
                throw new MerqureToolsException($"unable to send message to the broker. {Environment.NewLine}Channel : {channelName}{Environment.NewLine}Message : {message}");
            }
        }

        internal void PublishWithTransaction(IPublisher publisher, string channelName, IEnumerable<string> messages)
        {
            try
            {
                publisher.PublishWithTransaction(channelName, messages);
            }
            catch (Exception e)
            {
                throw new MerqureToolsException($"unable to send messages to the broker. {Environment.NewLine}Channel : {channelName}{Environment.NewLine}Messages : {string.Join(" | ", messages.ToArray())}", e);
            }
        }

        private RetryMessage<T> CreateRetryMessage(T message)
        {
            return new RetryMessage<T>
            {
                RetryInformations = new RetryInformations
                {
                    NumberOfRetry = 0
                },
                OriginalMessage = message
            };
        }
    }
}
