using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerQure
{
    public static class Constants
    {
        /// <summary>
        /// Priority of a message between 0 and 255
        /// </summary>
        public const string HeaderPriority = "x-priority";
        /// <summary>
        /// Header property with the dead lettering reason (of type 'NESTED TABLE')
        /// </summary>
        public const string HeaderDeath = "x-death";

        public const string AlternateExchange = "alternate-exchange";

        /// <summary>
        /// Maximum length of messages in bytes
        /// </summary>
        public const string QueueMaxLengthInBytes = "x-max-length-bytes";
        /// <summary>
        /// Queue TTL
        /// </summary>
        /// <see cref="https://www.rabbitmq.com/ttl.html"/>
        public const string QueueExpires = "x-expires";
        /// <summary>
        /// Queue Length Limit, Maximum number of messages
        /// </summary>
        /// <see cref="https://www.rabbitmq.com/maxlength.html"/>
        public const string QueueMaxLength = "x-max-length";
        /// <summary>
        /// Priority Queue Support
        /// </summary>
        /// <see cref="https://www.rabbitmq.com/priority.html"/>
        public const string QueueMaxPriority = "x-max-priority";
        /// <summary>
        /// Per-Queue Message TTL
        /// </summary>
        /// <see cref="https://www.rabbitmq.com/ttl.html"/>
        public const string QueueMessageTTL = "x-message-ttl";
        /// <summary>
        /// Dead Letter Exchanges
        /// </summary>
        /// <see cref="https://www.rabbitmq.com/dlx.html"/>
        public const string QueueDeadLetterExchange = "x-dead-letter-exchange";
        /// <summary>
        /// Routing Dead-Lettered Messages
        /// </summary>
        public const string QueueDeadLetterRoutingKey = "x-dead-letter-routing-key";
    }
}
