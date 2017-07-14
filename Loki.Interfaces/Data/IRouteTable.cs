using System.Collections.Generic;

namespace Loki.Interfaces.Data
{
    public interface IRouteTable
    {
        /// <summary>
        /// Gets or sets the <see cref="IWebSocketDataHandler"/> with the specified key.
        /// </summary>
        /// <value>
        /// The <see cref="IWebSocketDataHandler"/>.
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        IWebSocketDataHandler this[string key] { get; set; }
    }
}