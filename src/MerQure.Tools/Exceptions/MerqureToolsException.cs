using System;

namespace MerQure.Tools.Exceptions
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