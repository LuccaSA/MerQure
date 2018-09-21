using MerQure.Tools.Configurations;
using System.Collections.Generic;

namespace MerQure.Tools.Samples.RetryBusExample.Infra
{
    public static class SampleChannels
    { 
        public static readonly Channel MessageSended = new Channel("v1.sample.messagesended");

        public static readonly Channel OtherAction = new Channel("v1.sample.otheraction");

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
