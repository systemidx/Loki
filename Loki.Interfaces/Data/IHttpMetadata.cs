using System.Collections.Generic;
using System.Collections.Specialized;

namespace Loki.Interfaces.Data
{
    public interface IHttpMetadata
    {
        /// <summary>
        /// Gets the headers.
        /// </summary>
        /// <value>
        /// The headers.
        /// </value>
        Dictionary<string, string> Headers { get; }

        /// <summary>
        /// Gets the query strings.
        /// </summary>
        /// <value>
        /// The query strings.
        /// </value>
        NameValueCollection QueryStrings { get; }

        /// <summary>
        /// Gets the route.
        /// </summary>
        /// <value>
        /// The route.
        /// </value>
        string Route { get; }
    }
}