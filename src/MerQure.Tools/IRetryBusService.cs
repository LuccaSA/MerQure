using MerQure.Messages;
using MerQure.Tools.Buses;
using MerQure.Tools.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerQure.Tools
{
    public interface IRetryBusService
    {
        IBus<T> CreateNewBus<T>(RetryStrategyConfiguration configuration) where T : IDelivered;
    }
}
