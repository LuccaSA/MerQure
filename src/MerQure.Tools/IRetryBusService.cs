using MerQure.Messages;
using MerQure.Tools.Buses;
using MerQure.Tools.Configurations;
using System.Threading.Tasks;

namespace MerQure.Tools;

public interface IRetryBusService
{
    Task<IBus<T>> CreateNewBusAsync<T>(RetryStrategyConfiguration configuration, bool isQuorum) where T : IDelivered;
}