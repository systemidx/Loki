using System;
using Loki.Interfaces.Data;

namespace Loki.Interfaces.Connections
{
    /// <summary>
    /// 
    /// </summary>
    public interface IWebSocketConnection : IDisposable
    {
        /// <summary>
        /// Gets the client identifier.
        /// </summary>
        /// <value>
        /// The client identifier.
        /// </value>
        string ClientIdentifier { get; }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        Guid UniqueIdentifier { get; }
        
        /// <summary>
        /// Gets a value indicating whether this instance is alive.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is alive; otherwise, <c>false</c>.
        /// </value>
        bool IsAlive { get; }
        
        /// <summary>
        /// Gets the HTTP metadata.
        /// </summary>
        /// <value>
        /// The HTTP metadata.
        /// </value>
        IHttpMetadata HttpMetadata { get; }

        /// <summary>
        /// Blocks the and receive.
        /// </summary>
        /// <returns></returns>
        void Listen();

        /// <summary>
        /// Sends the text.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object.</param>
        void SendText<T>(T obj);

        /// <summary>
        /// Sends the binary.
        /// </summary>
        /// <param name="obj">The object.</param>
        void SendBinary(byte[] obj);
    }
}