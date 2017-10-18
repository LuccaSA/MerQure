using MerQure.Tools.Configurations;
using System.Collections.Generic;

namespace MerQure.Tools.Samples.RetryExchangeExample.Infra
{
    public static class SomethingChannels
    { 
        public static Channel MessageSended = new Channel("v1.something.messagesended");

        public static Channel OtherAction = new Channel("v1.something.otheraction");

        public static List<Channel> GetAllChannels()
        {
            return new List<Channel>()
            {
                {MessageSended },
                {OtherAction }
            };
        }
    }
}
