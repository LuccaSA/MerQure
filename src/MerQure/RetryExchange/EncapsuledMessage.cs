using MerQure.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerQure.RetryExchange
{
    internal class EncapsuledMessage<T> where T : IAmqpIdentity
    {
        public MessageTechnicalInformations TechnicalInformation { get; set; }
        public T OriginalMessage { get; set; }
    }
}
