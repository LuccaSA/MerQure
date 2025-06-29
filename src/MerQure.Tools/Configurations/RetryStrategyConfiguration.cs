﻿using System.Collections.Generic;

namespace MerQure.Tools.Configurations
{
    public class RetryStrategyConfiguration
    {
        public const string RetryExchangeSuffix = "retry";
        internal const string ErrorExchangeSuffix = "error";

        public string BusName { get; set; }
        public List<Channel> Channels { get; set; } = new List<Channel>();

        /// <summary>
        /// Empty list = 0 retry 
        /// </summary>
        public List<int> DelaysInMsBetweenEachRetry { get; set; }

        /// <summary>
        /// If true, the message go into error bus after all delayed retry. 
        /// Otherwise, the message will loop with last delays value 
        /// </summary>
        public bool MessageIsGoingIntoErrorBusAfterAllRepeat { get; set; }

        public int DeliveryDelayInMilliseconds { get; set; }
    }
}
