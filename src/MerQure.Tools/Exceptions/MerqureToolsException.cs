using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerQure.Exceptions
{
    public class MerqureToolsException : Exception
    {
        public MerqureToolsException()
        {
        }

        public MerqureToolsException(string message)
        : base(message)
        {
        }

        public MerqureToolsException(string message, Exception inner)
        : base(message, inner)
        {
        }
    }
}
