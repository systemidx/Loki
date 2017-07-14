using System;

namespace Loki.Server.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ConnectionRouteAttribute : Attribute
    {
        public readonly string Route;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionRouteAttribute"/> class.
        /// </summary>
        /// <param name="route">The route.</param>
        public ConnectionRouteAttribute(string route)
        {
            Route = route;
        }
    }
}
