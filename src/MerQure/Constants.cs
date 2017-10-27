namespace MerQure
{
    public static class Constants
    {
        /// <summary>
        /// Binding messages matching exactly the routing key
        /// </summary>
        public const string ExchangeTypeDirect = "direct";
        /// <summary>
        /// simple broadcasting, without binding capabilities
        /// </summary>
        public const string ExchangeTypeFanout = "fanout";
        /// <summary>
        /// binding messages matching the header's properties pattern
        /// </summary>
        public const string ExchangeTypeHeaders = "headers";
        /// <summary>
        /// binding messages matching the routingKey pattern
        /// </summary>
        public const string ExchangeTypeTopic = "topic";
    }
}
