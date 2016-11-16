using MerQure.Content;
using System.Collections.Generic;

namespace MerQure.RMQ.Content
{
    class Header : IHeader
    {
        private IDictionary<string, object> HeaderProperties;

        public Header(IDictionary<string, object> headerProperties)
        {
            this.HeaderProperties = headerProperties;
        }

        public IDictionary<string, object> GetHeaderProperties()
        {
            return HeaderProperties;
        }
    }
}
