using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerQure.Samples.RetryExchange.Infra
{
    public static class SomethingQueuesName
    {
        public const string MessageSended = "v1.something.messagesended";
        public const string OtherAction = "v1.something.otheraction";

        public static List<string> GetAll()
        {
            return new List<string>()
            {
                {MessageSended },
                {OtherAction }
            };
        }
    }
}
