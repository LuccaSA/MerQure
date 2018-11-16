using System.Text;

namespace MerQure.RbMQ.Helpers
{
    public static class EncodingHelper
    {
        /// <summary>
        /// Encodes the string into a sequence of bytes.
        /// </summary>
        public static byte[] ToByte(this string value) => Encoding.UTF8.GetBytes(value);

        /// <summary>
        /// Encodes the sequence of bytes into a string.
        /// </summary>
        public static string ToString(this byte[] value) => Encoding.UTF8.GetString(value);
    }
}
