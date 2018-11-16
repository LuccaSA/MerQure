﻿namespace MerQure
{
    public static class Constants
    {
        /// <summary>
        /// Binding messages matching exactly the routing key
        /// </summary>
        public const string ExchangeTypeDirect = "direct";
        /// <summary>
        /// Simple broadcasting, without binding capabilities
        /// </summary>
        public const string ExchangeTypeFanout = "fanout";
        /// <summary>
        /// Binding messages matching the header's properties pattern
        /// </summary>
        public const string ExchangeTypeHeaders = "headers";
        /// <summary>
        /// Binding messages matching the routingKey pattern
        /// </summary>
        public const string ExchangeTypeTopic = "topic";
    }
}
