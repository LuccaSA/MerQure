using MerQure.Configuration;
using MerQure.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerQure
{
    public interface IRetryExchangeService
    {
        IRetryExchange<T> Get<T>(RetryExchangeConfiguration retryExchangeConfiguration) where T : IAmqpIdentity;
    }
}
