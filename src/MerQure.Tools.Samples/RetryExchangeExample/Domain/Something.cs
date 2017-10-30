
using MerQure.Messages;
using MerQure.Tools.Messages;

namespace MerQure.Tools.Samples.RetryExchangeExample.Domain
{
    public class Something : IDelivered
    {
        public string DeliveryTag { get; set; }
        public string Name { get; set; }
    }
}
