using MerQure.Content;
using System.Collections.Generic;

namespace MerQure.RbMQ.Content
{
    public class Header : IHeader
    {
        private readonly IDictionary<string, object> HeaderProperties;

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
            : this(header.GetProperties())
        {
        }

        public void Add(string propertyName, object value)
        {
            HeaderProperties.Add(propertyName, value);
        }

        public IDictionary<string, object> GetProperties() => HeaderProperties;
    }
}
