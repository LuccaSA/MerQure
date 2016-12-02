using MerQure.Content;
using System.Collections.Generic;

namespace MerQure.RbMQ.Content
{
    public class Header : IHeader
    {
        private readonly IDictionary<string, string> HeaderProperties;

        public Header()
        {
            HeaderProperties = new Dictionary<string, string>();
        }
        internal Header(IDictionary<string, string> headerProperties)
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

        public void Add(string propertyName, string value)
        {
            HeaderProperties.Add(propertyName, value);
        }

        public IDictionary<string, string> GetProperties()
        {
            return HeaderProperties;
        }
    }
}
