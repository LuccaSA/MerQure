using MerQure.Messages;
using MerQure.Tools.Configurations;
using MerQure.Tools.Exceptions;
using MerQure.Tools.Messages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MerQure.Tools.Buses;

public class Publisher<T> where T : IDelivered
{
    private readonly RetryStrategyConfiguration _messageBrokerConfiguration;
    private readonly IMessagingService _messagingService;


    public Publisher(IMessagingService messagingService, RetryStrategyConfiguration retryConfiguration)
    {
        _messagingService = messagingService;
        _messageBrokerConfiguration = retryConfiguration;
    }

    public async Task PublishWithTransactionAsync(Channel channel, IEnumerable<T> messages, bool applyDeliveryDeplay)
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

        await using var publisher = await _messagingService.GetPublisherAsync(busName, false);
        await PublishWithTransactionAsync(publisher, bindingValue, serializedMessages);
    }

    public async Task PublishAsync(Channel channel, T message, bool applyDeliveryDeplay)
    {
        var encapsuledMessage = CreateRetryMessage(message);
        string bindingValue = channel.Value;
        string busName = _messageBrokerConfiguration.BusName;
        if (applyDeliveryDeplay && _messageBrokerConfiguration.DeliveryDelayInMilliseconds > 0)
        {
            bindingValue = $"{bindingValue}.{_messageBrokerConfiguration.DeliveryDelayInMilliseconds}";
            busName = $"{busName}.{ RetryStrategyConfiguration.RetryExchangeSuffix}";
        }

        await using var publisher = await _messagingService.GetPublisherAsync(busName, true);
        await TryPublishWithBrokerAcknowledgementAsync(publisher, bindingValue, JsonConvert.SerializeObject(encapsuledMessage));
    }

    public async Task PublishOnRetryExchangeAsync(Channel channel, T message, RetryInformations retryInformations)
    {
        List<int> delays = _messageBrokerConfiguration.DelaysInMsBetweenEachRetry;
        int delay = 0;
        if (delays.Count >= retryInformations.NumberOfRetry)
        {
            delay = delays[retryInformations.NumberOfRetry];
        }
        else
        {
            delay = delays[delays.Count-1];
        }
        retryInformations.NumberOfRetry++;
        RetryMessage<T> retryMessage = new RetryMessage<T>
        {
            OriginalMessage = message,
            RetryInformations = retryInformations
        };

        string bindingValue = $"{channel.Value}.{delay}";
        await using var publisher = await _messagingService.GetPublisherAsync($"{_messageBrokerConfiguration.BusName}.{RetryStrategyConfiguration.RetryExchangeSuffix}", true);
        await TryPublishWithBrokerAcknowledgementAsync(publisher, bindingValue, JsonConvert.SerializeObject(retryMessage));
    }

    internal async Task PublishOnErrorExchangeAsync(Channel channel, T message, RetryInformations technicalInformations)
    {
        string errorChanel = $"{channel.Value}.error";
        RetryMessage<T> retryMessage = new RetryMessage<T>
        {
            OriginalMessage = message,
            RetryInformations = technicalInformations
        };
        await using var publisher = await _messagingService.GetPublisherAsync($"{_messageBrokerConfiguration.BusName}.{RetryStrategyConfiguration.ErrorExchangeSuffix}", true);
        await TryPublishWithBrokerAcknowledgementAsync(publisher, errorChanel, JsonConvert.SerializeObject(retryMessage));
    }

    private static async Task TryPublishWithBrokerAcknowledgementAsync(IPublisher publisher, string channelName, string message)
    {
        try
        {
            await publisher.PublishWithAcknowledgementAsync(channelName, message);
        }
        catch
        {
            throw new MerqureToolsException($"unable to send message to the broker. {Environment.NewLine}Channel : {channelName}{Environment.NewLine}Message : {message}");
        }
    }

    private static Task PublishWithTransactionAsync(IPublisher publisher, string channelName, List<string> messages)
    {
        try
        {
            return publisher.PublishWithTransactionAsync(channelName, messages);
        }
        catch (Exception e)
        {
            throw new MerqureToolsException($"unable to send messages to the broker. {Environment.NewLine}Channel : {channelName}{Environment.NewLine}Messages : {string.Join(" | ", messages.ToArray())}", e);
        }
    }

    private static RetryMessage<T> CreateRetryMessage(T message)
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