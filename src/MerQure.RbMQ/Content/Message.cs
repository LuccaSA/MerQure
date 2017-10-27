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
            this.RoutingKey = routingKey;
            this.Header = header;
            this.Body = body;
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

        public string GetRoutingKey()
        {
            return RoutingKey;
        }

        public IHeader GetHeader()
        {
            return Header;
        }

        public string GetBody()
        {
            return Body;
        }

        public byte? GetPriority()
        {
            return Priority;
        }
    }
}
