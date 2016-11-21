using MerQure.Content;
using System.Collections.Generic;

namespace MerQure.RbMQ.Content
{
    public class Header : IHeader
    {
        private IDictionary<string, object> HeaderProperties;

        public Header()
        {
            HeaderProperties = new Dictionary<string, object>();
        }
        internal Header(IDictionary<string, object> headerProperties)
            : this()
        {
            foreach (var headerProperty in headerProperties)
            {
                Add(headerProperty.Key, headerProperty.Value);
            }
        }
        public Header(IHeader header)
            : this(header.GetHeaderProperties())
        {
        }

        public void Add(string propertyName, object value)
        {
            HeaderProperties.Add(propertyName, value);
        }

        public IDictionary<string, object> GetHeaderProperties()
        {
            return HeaderProperties;
        }
    }
}
