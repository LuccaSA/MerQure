using MerQure.Messages;
using MerQure.Tools.Configurations;
using MerQure.Tools.Exceptions;
using MerQure.Tools.Messages;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MerQure.Tools.Buses
{
	internal class RetryConsumer<T> : Consumer<T> where T : IDelivered
    {
        private readonly Publisher<T> _producer;

        private readonly RetryStrategyConfiguration _retryConfiguration;

        public RetryConsumer(IMessagingService messagingService, RetryStrategyConfiguration retryConfiguration, Publisher<T> producer) : base(messagingService)
        {
            _retryConfiguration = retryConfiguration;
            _producer = producer;
        }

        internal async Task SendToErrorExchangeAsync(Channel channel, T deliveredMessage)
        {
            if (String.IsNullOrEmpty(deliveredMessage.DeliveryTag))
                throw new ArgumentNullException(nameof(deliveredMessage.DeliveryTag));
            if (!RetryInformations.ContainsKey(deliveredMessage.DeliveryTag))
                throw new MerqureToolsException($"unknown delivery tag {deliveredMessage.DeliveryTag}");

            RetryInformations retryInformations = RetryInformations[deliveredMessage.DeliveryTag];

            await _producer.PublishOnErrorExchangeAsync(channel, deliveredMessage, retryInformations);
            await AcknowlegdeDeliveredMessageAsync(channel, deliveredMessage);
            RetryInformations.Remove(deliveredMessage.DeliveryTag);
        }

        public async Task<MessageInformations> ForceRetryStrategyAsync(Channel channel, T message, int attemptNumber)
        {
            var retryInformations = new RetryInformations
            {
                NumberOfRetry = attemptNumber > 0 ? attemptNumber - 1 : 0
            };
            var messageInformations = new MessageInformations();

            if (IsGoingToErrorExchange(retryInformations))
            {
                await _producer.PublishOnErrorExchangeAsync(channel, message, retryInformations);
                messageInformations.IsOnErrorBus = true;
            }
            else
            {
                await _producer.PublishOnRetryExchangeAsync(channel, message, retryInformations);
            }
            return messageInformations;
        }

        public async Task<MessageInformations> ApplyRetryStrategyAsync(Channel channel, T deliveredMessage)
        {
            if (String.IsNullOrEmpty(deliveredMessage.DeliveryTag))
                throw new ArgumentNullException(nameof(deliveredMessage.DeliveryTag));
            if (!RetryInformations.ContainsKey(deliveredMessage.DeliveryTag))
                throw new MerqureToolsException($"unknown delivery tag {deliveredMessage.DeliveryTag}");

            RetryInformations retryInformations = RetryInformations[deliveredMessage.DeliveryTag];
            var messageInformations = new MessageInformations();
            if (IsGoingToErrorExchange(retryInformations))
            {
                await _producer.PublishOnErrorExchangeAsync(channel, deliveredMessage, retryInformations);
                messageInformations.IsOnErrorBus = true;
            }
            else
            {
                await _producer.PublishOnRetryExchangeAsync(channel, deliveredMessage, retryInformations);
            }

            await AcknowlegdeDeliveredMessageAsync(channel, deliveredMessage);
            RetryInformations.Remove(deliveredMessage.DeliveryTag);
            return messageInformations;
        }


        private bool IsGoingToErrorExchange(RetryInformations technicalInformations)
        {
            if (!_retryConfiguration.DelaysInMsBetweenEachRetry.Any())
            {
                return true;
            }
            if (_retryConfiguration.MessageIsGoingIntoErrorBusAfterAllRepeat
                   && technicalInformations.NumberOfRetry == _retryConfiguration.DelaysInMsBetweenEachRetry.Count)
            {
                return true;
            }
            return false;
        }
    }
}