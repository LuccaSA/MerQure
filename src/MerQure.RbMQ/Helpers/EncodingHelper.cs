using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerQure.RbMQ.Helpers
{
    public static class EncodingHelper
    {
        /// <summary>
        /// Encodes the string into a sequence of bytes.
        /// </summary>
        public static byte[] ToByte(this string value)
        {
            return Encoding.UTF8.GetBytes(value);
        }
    }
}
