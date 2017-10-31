
using MerQure.Messages;
using MerQure.Tools.Messages;

namespace MerQure.Tools.Samples.RetryBusExample.Domain
{
    public class Sample : IDelivered
    {
        public string DeliveryTag { get; set; }
        public string Name { get; set; }
    }
}
