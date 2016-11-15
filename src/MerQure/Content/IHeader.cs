using System.Collections.Generic;

namespace MerQure.Content
{
    public interface IHeader
    {
        IDictionary<string, object> GetHeaderProperties();
    }
}
