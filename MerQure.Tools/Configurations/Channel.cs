using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerQure.Tools.Configurations
{
    public class Channel
    {
        public string Value { get; internal set; }
        public Channel(string value)
        {
            Value = value;
        }
    }
}
