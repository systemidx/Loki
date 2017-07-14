using System;

namespace Loki.Interfaces
{
    public interface IServer : IDisposable
    {
        #region Properties

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        string Id { get; }

        /// <summary>
        /// The host address.
        /// </summary>
        string Host { get; }

        /// <summary>
        /// The port.
        /// </summary>
        int Port { get; }
        
        /// <summary>
        /// Determines whether the server is running.
        /// </summary>
        bool IsRunning { get; }

        #endregion

        /// <summary>
        /// Starts the server.
        /// </summary>
        /// <returns>Task.</returns>
        void RunAndBlock();
        
        /// <summary>
        /// Stops the server.
        /// </summary>
        /// <returns>Task.</returns>
        void Stop();
    }
}