using MerQure.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerQure.Samples.RetryExchange.Domain
{
    public class Something : IAmqpIdentity
    {
        public string DeliveryTag { get; set; }
        public string Name { get; set; }
    }
}
