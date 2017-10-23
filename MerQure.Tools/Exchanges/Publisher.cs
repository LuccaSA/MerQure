﻿using MerQure;
using MerQure.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using MerQure.Tools.Messages;
using MerQure.Tools.Configurations;

namespace MerQure.Tools.Exchanges
{
    internal class Publisher<T> where T : IAMQPIdentity
    {
        private readonly RetryExchangeConfiguration _messageBrokerConfiguration;
        private readonly IMessagingService _messagingService;

        public Publisher(IMessagingService messagingService, RetryExchangeConfiguration retryConfiguration)
        {
            _messagingService = messagingService;
            _messageBrokerConfiguration = retryConfiguration;
        }

        public void Publish(Channel channel, T message)
        {
            using (var publisher = _messagingService.GetPublisher(_messageBrokerConfiguration.ExchangeName, true))
            {
                var encapsuledMessage = new EncapsuledMessage<T>
                {
                    TechnicalInformation = new MessageTechnicalInformations
                    {
                        NumberOfRetry = 0
                    },
                    OriginalMessage = message
                };
                TryPublishWithBrokerAcknowledgement(publisher, channel.Value, JsonConvert.SerializeObject(encapsuledMessage));
            }
        }

        internal void PublishOnRetryExchange(Channel channel, T message, MessageTechnicalInformations technicalInformations)
        {
            technicalInformations.NumberOfRetry++;
            EncapsuledMessage<T> encapsuledMessage = new EncapsuledMessage<T>
            {
                OriginalMessage = message,
                TechnicalInformation = technicalInformations
            };
            List<int> delays = _messageBrokerConfiguration.DelaysInMsBetweenEachRetry;
            int delay = 0;
            if (delays.Count() >= technicalInformations.NumberOfRetry)
            {
                delay = delays[technicalInformations.NumberOfRetry - 1];
            }
            else
            {
                delay = delays.Last();
            }

            string bindingValue = $"{channel.Value}.{delay}";
            using (var publisher = _messagingService.GetPublisher($"{_messageBrokerConfiguration.ExchangeName}.retry", true))
            {
                TryPublishWithBrokerAcknowledgement(publisher, bindingValue, JsonConvert.SerializeObject(encapsuledMessage));
            }
        }
        
        internal void PublishOnErrorExchange(Channel channel, T message, MessageTechnicalInformations technicalInformations)
        {
            string errorChanel = $"{channel.Value}.error";
            EncapsuledMessage<T> encapsuledMessage = new EncapsuledMessage<T>
            {
                OriginalMessage = message,
                TechnicalInformation = technicalInformations
            };
            using (var publisher = _messagingService.GetPublisher($"{_messageBrokerConfiguration.ExchangeName}.error", true))
            {
                TryPublishWithBrokerAcknowledgement(publisher, errorChanel, JsonConvert.SerializeObject(encapsuledMessage));
            }
        }

        internal void TryPublishWithBrokerAcknowledgement(IPublisher publisher, string channelName, string message)
        {
            bool published = publisher.PublishWithAcknowledgement(channelName, message);
            if(published == false)
            {
                throw new MerqureToolsException($"unable to send message to the broker. {Environment.NewLine}Channel : {channelName}{Environment.NewLine}Message : {message}");
            }
        }
    }
}