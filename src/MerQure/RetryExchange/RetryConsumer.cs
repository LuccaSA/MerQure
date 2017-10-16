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
    internal class RetryConsumer<T> where T : IAmqpIdentity
    {
        private readonly IMessagingService _messagingService;
        private readonly RetryPublisher<T> _producer;
        private readonly RetryExchangeConfiguration _exchangeConfiguration;

        private Dictionary<string, MessageTechnicalInformations> _technicalInformations;

        public RetryConsumer(IMessagingService messagingService, RetryExchangeConfiguration exchangeConfiguration, RetryPublisher<T> publisher)
        {
            _messagingService = messagingService;
            _exchangeConfiguration = exchangeConfiguration;
            _producer = publisher;
            _technicalInformations = new Dictionary<string, MessageTechnicalInformations>();
        }

        public void Consume(string queueName, EventHandler<T> callback)
        {
            _messagingService.GetOrCreateConsumer(queueName).Consume((object sender, IMessagingEvent messagingEvent) =>
            {
                EncapsuledMessage<T> deserializedMessage = JsonConvert.DeserializeObject<EncapsuledMessage<T>>(messagingEvent.Message.GetBody());
                deserializedMessage.OriginalMessage.DeliveryTag = messagingEvent.DeliveryTag;
                _technicalInformations.Add(messagingEvent.DeliveryTag, deserializedMessage.TechnicalInformation);

                callback(this, deserializedMessage.OriginalMessage);
            });
        }

        public void SendToErrorExchange(string queueName, T delivredMessage)
        {
            if (String.IsNullOrEmpty(delivredMessage.DeliveryTag))
                throw new ArgumentNullException(nameof(delivredMessage.DeliveryTag));
            if (!_technicalInformations.ContainsKey(delivredMessage.DeliveryTag))
                throw new MerQureException($"unknown delivery tag {delivredMessage.DeliveryTag}");

            MessageTechnicalInformations technicalInformations = _technicalInformations[delivredMessage.DeliveryTag];

            _producer.PublishOnErrorExchange(queueName, delivredMessage, technicalInformations);
            AcknowlegdeDelivredMessage(queueName, delivredMessage);
            _technicalInformations.Remove(delivredMessage.DeliveryTag);
        }

        public void ApplyRetryStrategy(string queueName, T delivredMessage)
        {
            if (String.IsNullOrEmpty(delivredMessage.DeliveryTag))
                throw new ArgumentNullException(nameof(delivredMessage.DeliveryTag));
            if (!_technicalInformations.ContainsKey(delivredMessage.DeliveryTag))
                throw new MerQureException($"unknown delivery tag {delivredMessage.DeliveryTag}");

            MessageTechnicalInformations technicalInformations = _technicalInformations[delivredMessage.DeliveryTag];

            if (IsGoingToErrorExchange(technicalInformations))
            {
                _producer.PublishOnErrorExchange(queueName, delivredMessage, technicalInformations);
            }
            else
            {
                _producer.PublishOnRetryExchange(queueName, delivredMessage, technicalInformations);
            }
            AcknowlegdeDelivredMessage(queueName, delivredMessage);
            _technicalInformations.Remove(delivredMessage.DeliveryTag);
        }

        private bool IsGoingToErrorExchange(MessageTechnicalInformations technicalInformations)
        {
            if (!_exchangeConfiguration.DelaysInMillisecondsBetweenEachRetry.Any())
            {
                return true;
            }
            if (_exchangeConfiguration.EndOnErrorExchange
                   && technicalInformations.NumberOfRetry == _exchangeConfiguration.DelaysInMillisecondsBetweenEachRetry.Count)
            {
                return true;
            }
            return false;
        }

        internal void AcknowlegdeDelivredMessage(string queueName, IAmqpIdentity delivredMessage)
        {
            _messagingService.GetOrCreateConsumer(queueName).AcknowlegdeDeliveredMessage(delivredMessage.DeliveryTag);
        }

        internal void RejectDeliveredMessage(string queueName, IAmqpIdentity delivredMessage)
        {
            _messagingService.GetOrCreateConsumer(queueName).RejectDeliveredMessage(delivredMessage.DeliveryTag);
        }
    }
}
