using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerQure.Exceptions
{
    public class MerQureException : Exception
    {
        public MerQureException()
        {
        }

        public MerQureException(string message)
        : base(message)
        {
        }

        public MerQureException(string message, Exception inner)
        : base(message, inner)
        {
        }
    }
}
