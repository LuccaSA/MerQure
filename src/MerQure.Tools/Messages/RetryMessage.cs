using MerQure.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerQure.Tools.Messages
{
    public class RetryMessage<T> where T : IDelivered
    {
        public RetryInformations RetryInformations { get; set; }
        public T OriginalMessage { get; set; }
    }
}
