
using MerQure.Tools.Messages;

namespace MerQure.Tools.Samples.RetryExchangeExample.Domain
{
    public class Something : IAMQPIdentity
    {
        public string DeliveryTag { get; set; }
        public string Name { get; set; }
    }
}
