using System.Collections.Generic;
using Loki.Interfaces.Data;

namespace Loki.Server.Data
{
    public class RouteTable : IRouteTable
    {
        private readonly Dictionary<string, IWebSocketDataHandler> _routes = new Dictionary<string, IWebSocketDataHandler>();

        public IWebSocketDataHandler this[string key]
        {
            get => _routes.ContainsKey(key) ? _routes[key] : null;
            set => _routes[key] = value;
        }
    }
}
