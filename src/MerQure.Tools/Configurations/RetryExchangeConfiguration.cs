using MerQure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerQure.Tools.Configurations
{
    public class RetryExchangeConfiguration
    {
        public string ExchangeName { get; set; }
        public List<Channel> Channels { get; set; }

        /// <summary>
        /// Empty list = 0 retry 
        /// </summary>
        public List<int> DelaysInMsBetweenEachRetry { get; set; }

        public bool EndOnErrorExchange { get; set; }

        public RetryExchangeConfiguration()
        {
            Channels = new List<Channel>();
        }
    }
}
