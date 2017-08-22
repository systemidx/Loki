using System.IO;
using System.Net.Sockets;

namespace Loki.Interfaces.Frame
{
    public interface IWebSocketFrameReader
    {
        /// <summary>
        /// Gets or sets the stream.
        /// </summary>
        /// <value>
        /// The stream.
        /// </value>
        Stream Stream { get; set; }

        /// <summary>
        /// Reads the specified connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <returns></returns>
        IWebSocketFrame Read(TcpClient connection);
    }
}