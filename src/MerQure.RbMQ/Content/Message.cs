using MerQure.Content;

namespace MerQure.RbMQ.Content
{
    public class Message : IMessage
    {
        public string RoutingKey { get; }
        public IHeader Header { get; }
        public string Body { get; set; }
        public byte? Priority { get; set; }

        /// <summary>
        /// Create a RabbitMQ Message
        /// </summary>
        /// <param name="routingKey"></param>
        /// <param name="header"></param>
        /// <param name="body"></param>
        public Message(string routingKey, IHeader header, string body)
        {
            RoutingKey = routingKey;
            Header = header;
            Body = body;
        }
        /// <summary>
        /// Create a RabbitMQ Message
        /// </summary>
        /// <param name="routingKey"></param>
        /// <param name="body"></param>
        public Message(string routingKey, string body)
            : this(routingKey, new Header(), body)
        {
        }

        public string GetRoutingKey() => RoutingKey;

        public IHeader GetHeader() => Header;

        public string GetBody() => Body;

        public byte? GetPriority() => Priority;
    }
}