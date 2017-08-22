using System;
using Loki.Interfaces.Data;

namespace Loki.Interfaces.Connections
{
    public interface IWebSocketConnection : IDisposable
    {
        #region Properties

        /// <summary>
        /// Gets the client identifier.
        /// </summary>
        /// <value>
        /// The client identifier.
        /// </value>
        string ClientIdentifier { get; set; }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        Guid UniqueIdentifier { get; }

        /// <summary>
        /// Gets the unique client identifier.
        /// </summary>
        /// <value>
        /// The unique client identifier.
        /// </value>
        string UniqueClientIdentifier { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is alive.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is alive; otherwise, <c>false</c>.
        /// </value>
        bool IsAlive { get; }

        /// <summary>
        /// Returns true if ... is valid.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </value>
        bool IsValid { get; }

        /// <summary>
        /// Gets the HTTP metadata.
        /// </summary>
        /// <value>
        /// The HTTP metadata.
        /// </value>
        IHttpMetadata HttpMetadata { get; }

        #endregion

        #region Operational Methods

        /// <summary>
        /// Blocks the and receive.
        /// </summary>
        /// <returns></returns>
        void Listen();
        
        /// <summary>
        /// Closes this instance.
        /// </summary>
        void Close();

        #endregion

        #region Sending Methods

        /// <summary>
        /// Sends the text.
        /// </summary>
        /// <param name="message">The message.</param>
        void SendText(string message);

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

        /// <summary>
        /// Sends the ping.
        /// </summary>
        void SendPing();

        /// <summary>
        /// Sends the pong.
        /// </summary>
        void SendPong();

        #endregion

    }
}