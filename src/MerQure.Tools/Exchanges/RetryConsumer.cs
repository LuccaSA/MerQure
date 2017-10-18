using System;
using System.Collections.Generic;
using System.Linq;
using MerQure;
using MerQure.Tools.Messages;
using MerQure.Tools.Configurations;
using MerQure.Exceptions;
using Newtonsoft.Json;

namespace MerQure.Tools.Exchanges
{
    internal class RetryConsumer<T> : Consumer<T> where T : IAMQPIdentity 
    {
        private readonly ConsumerProvider _consumerProvider;
        private readonly Publisher<T> _producer;

        private RetryExchangeConfiguration _retryConfiguration;

        public RetryConsumer(IMessagingService messagingService, RetryExchangeConfiguration retryConfiguration, Publisher<T> producer) : base(messagingService)
        {
            _retryConfiguration = retryConfiguration;
            _producer = producer;
        }

        internal void SendToErrorExchange(Channel channel, T delivredMessage)
        {
            if (String.IsNullOrEmpty(delivredMessage.DeliveryTag))
                throw new ArgumentNullException(nameof(delivredMessage.DeliveryTag));
            if (!TechnicalInformations.ContainsKey(delivredMessage.DeliveryTag))
                throw new MerqureToolsException($"unknown delivery tag {delivredMessage.DeliveryTag}");

            MessageTechnicalInformations technicalInformations = TechnicalInformations[delivredMessage.DeliveryTag];

            _producer.PublishOnErrorExchange(channel, delivredMessage, technicalInformations);
             AcknowlegdeDelivredMessage(channel, delivredMessage);
            TechnicalInformations.Remove(delivredMessage.DeliveryTag);
        }

        public void ApplyRetryStrategy(Channel channel, T delivredMessage)
        {
            if (String.IsNullOrEmpty(delivredMessage.DeliveryTag))
                throw new ArgumentNullException(nameof(delivredMessage.DeliveryTag));
            if (!TechnicalInformations.ContainsKey(delivredMessage.DeliveryTag))
                throw new MerqureToolsException($"unknown delivery tag {delivredMessage.DeliveryTag}");

            MessageTechnicalInformations technicalInformations = TechnicalInformations[delivredMessage.DeliveryTag];

            if (IsGoingToErrorExchange(technicalInformations))
            {
                _producer.PublishOnErrorExchange(channel, delivredMessage, technicalInformations);
            }
            else
            {
                _producer.PublishOnRetryExchange(channel, delivredMessage, technicalInformations);
            }
            AcknowlegdeDelivredMessage(channel, delivredMessage);
            TechnicalInformations.Remove(delivredMessage.DeliveryTag);
        }


        private bool IsGoingToErrorExchange(MessageTechnicalInformations technicalInformations)
        {
            if (!_retryConfiguration.DelaysInMsBetweenEachRetry.Any())
            {
                return true;
            }
            if (_retryConfiguration.EndOnErrorExchange
                   && technicalInformations.NumberOfRetry == _retryConfiguration.DelaysInMsBetweenEachRetry.Count())
            {
                return true;
            }
            return false;
        }
    }
}
