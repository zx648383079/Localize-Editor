using System.Collections.Generic;

namespace ZoDream.Shared.Routes
{
    public interface IQueryAttributable
    {
        public void ApplyQueryAttributes(IDictionary<string, object> queries);
    }
}
