using MerQure.Tools.Configurations;
using System.Collections.Generic;

namespace MerQure.Tools.Samples.RetryBusExample.Infra
{
    public static class SampleChannels
    {
        public static readonly Channel MessageSent = new Channel("v1.sample.messagesent");

        public static readonly Channel OtherAction = new Channel("v1.sample.otheraction");

        public static List<Channel> GetAllChannels()
        {
            return new List<Channel> { MessageSent, OtherAction };
        }
    }
}