using MerQure.Content;

namespace MerQure.RabbitMQ
{
    public class Message : IMessage
    {
        public string RoutingKey { get; }
        public IHeader Header { get; }
        public string Body { get; set; }

        /// <summary>
        /// Create a RabbitMQ Message
        /// </summary>
        /// <param name="routingKey"></param>
        /// <param name="header"></param>
        /// <param name="body"></param>
        public Message(string routingKey, IHeader header, string body)
        {
            this.RoutingKey = RoutingKey;
            this.Header = Header;
            this.Body = Body;
        }
    }
}
