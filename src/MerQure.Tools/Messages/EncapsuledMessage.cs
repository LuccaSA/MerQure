using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerQure.Tools.Messages
{
    internal class EncapsuledMessage<T> where T : IDelivered
    {
        public MessageTechnicalInformations TechnicalInformation { get; set; }
        public T OriginalMessage { get; set; }
    }
}
