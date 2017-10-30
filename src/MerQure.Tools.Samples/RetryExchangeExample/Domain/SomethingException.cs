using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerQure.Tools.Samples.RetryExchangeExample.Domain
{
    public class SomethingException : Exception
    {
        public SomethingException()
        {
        }

        public SomethingException(string message)
        : base(message)
        {
        }

        public SomethingException(string message, Exception inner)
        : base(message, inner)
        {
        }
    }
}
