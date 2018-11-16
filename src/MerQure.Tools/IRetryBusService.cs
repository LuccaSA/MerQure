using MerQure.Messages;
using MerQure.Tools.Buses;
using MerQure.Tools.Configurations;

namespace MerQure.Tools
{
    public interface IRetryBusService
    {
        IBus<T> CreateNewBus<T>(RetryStrategyConfiguration configuration) where T : IDelivered;
    }
}
