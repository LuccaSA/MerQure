﻿using System;
using System.Collections.Generic;
using System.Linq;
using MerQure;
using MerQure.Tools.Messages;
using MerQure.Tools.Configurations;
using Newtonsoft.Json;
using MerQure.Messages;
using MerQure.Tools.Exceptions;

namespace MerQure.Tools.Buses
{
    internal class RetryConsumer<T> : Consumer<T> where T : IDelivered
    {
        private readonly Publisher<T> _producer;

        private RetryStrategyConfiguration _retryConfiguration;

        public RetryConsumer(IMessagingService messagingService, RetryStrategyConfiguration retryConfiguration, Publisher<T> producer) : base(messagingService)
        {
            _retryConfiguration = retryConfiguration;
            _producer = producer;
        }

        internal void SendToErrorExchange(Channel channel, T deliveredMessage)
        {
            if (String.IsNullOrEmpty(deliveredMessage.DeliveryTag))
                throw new ArgumentNullException(nameof(deliveredMessage.DeliveryTag));
            if (!RetryInformations.ContainsKey(deliveredMessage.DeliveryTag))
                throw new MerqureToolsException($"unknown delivery tag {deliveredMessage.DeliveryTag}");

            RetryInformations retryInformations = RetryInformations[deliveredMessage.DeliveryTag];

            _producer.PublishOnErrorExchange(channel, deliveredMessage, retryInformations);
            AcknowlegdeDeliveredMessage(channel, deliveredMessage);
            RetryInformations.Remove(deliveredMessage.DeliveryTag);
        }

        public void ForceRetryStrategy(Channel channel, T message, int attemptNumber)
        {
            var retryInformations = new RetryInformations()
            {
                NumberOfRetry = attemptNumber - 1
            };
            if (IsGoingToErrorExchange(retryInformations))
            {
                _producer.PublishOnErrorExchange(channel, message, retryInformations);
            }
            else
            {
                _producer.PublishOnRetryExchange(channel, message, retryInformations);
            }
        }
        
        public void ApplyRetryStrategy(Channel channel, T deliveredMessage)
        {
            if (String.IsNullOrEmpty(deliveredMessage.DeliveryTag))
                throw new ArgumentNullException(nameof(deliveredMessage.DeliveryTag));
            if (!RetryInformations.ContainsKey(deliveredMessage.DeliveryTag))
                throw new MerqureToolsException($"unknown delivery tag {deliveredMessage.DeliveryTag}"); 

            RetryInformations retryInformations = RetryInformations[deliveredMessage.DeliveryTag];

            if (IsGoingToErrorExchange(retryInformations))
            {
                _producer.PublishOnErrorExchange(channel, deliveredMessage, retryInformations);
            }
            else
            {
                _producer.PublishOnRetryExchange(channel, deliveredMessage, retryInformations);
            }
            AcknowlegdeDeliveredMessage(channel, deliveredMessage);
            RetryInformations.Remove(deliveredMessage.DeliveryTag);
        }


        private bool IsGoingToErrorExchange(RetryInformations technicalInformations)
        {
            if (!_retryConfiguration.DelaysInMsBetweenEachRetry.Any())
            {
                return true;
            }
            if (_retryConfiguration.MessageIsGoingIntoErrorBusAfterAllRepeat
                   && technicalInformations.NumberOfRetry == _retryConfiguration.DelaysInMsBetweenEachRetry.Count())
            {
                return true;
            }
            return false;
        }
    }
}
