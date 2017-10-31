using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerQure.Tools.Samples.RetryBusExample.Domain
{
    public class SampleException : Exception
    {
        public SampleException()
        {
        }

        public SampleException(string message)
        : base(message)
        {
        }

        public SampleException(string message, Exception inner)
        : base(message, inner)
        {
        }
    }
}
