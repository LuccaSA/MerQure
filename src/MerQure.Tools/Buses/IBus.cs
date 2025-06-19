using MerQure.Messages;
using MerQure.Tools.Configurations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MerQure.Tools.Buses;

public interface IBus<T> where T : IDelivered
{
    Task PublishAsync(Channel channel, T message, bool applyDeliveryDeplay = false);
    Task PublishWithTransactionAsync(Channel channel, IEnumerable<T> messages, bool applyDeliveryDeplay = false);
    Task<MessageInformations> PublishForceRetryAttemptNumberAsync(Channel channel, T message, int attemptNumber);
    Task OnMessageReceivedAsync(Channel channel, EventHandler<T> callback);
    Task<MessageInformations> ApplyRetryStrategyAsync(Channel channel, T deliveredMessage);
    Task AcknowlegdeDeliveredMessageAsync(Channel channel, T deliveredMessage);
    Task RejectDeliveredMessageAsync(Channel channel, T deliveredMessage);
    Task SendDeliveredMessageToErrorBusAsync(Channel channel, T deliveredMessage);
}