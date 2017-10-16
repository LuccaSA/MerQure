using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerQure.Configuration
{
    public class RetryExchangeConfiguration
    {
        public RetryExchangeConfiguration()
        {
            DelaysInMillisecondsBetweenEachRetry = new List<int>();
        }

        public string ExchangeName { get; set; }
        public List<string> QueuesName { get; set; }

        /// <summary>
        /// Empty list => 0 retry 
        /// 
        /// </summary>
        public List<int> DelaysInMillisecondsBetweenEachRetry { get; set; }

        public bool EndOnErrorExchange { get; set; }
    }
}
