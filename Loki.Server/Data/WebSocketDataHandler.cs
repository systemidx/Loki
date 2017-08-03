using System;
using Loki.Common.Events;
using Loki.Interfaces.Connections;
using Loki.Interfaces.Data;
using Loki.Interfaces.Dependency;
using Loki.Interfaces.Logging;

namespace Loki.Server.Data
{
    public abstract class WebSocketDataHandler : IWebSocketDataHandler
    {
        /// <summary>
        /// The logger
        /// </summary>
        protected readonly ILogger Logger;

        /// <summary>
        /// The connection manager
        /// </summary>
        protected readonly IWebSocketConnectionManager ConnectionManager;

        /// <summary>
        /// The dependency utility
        /// </summary>
        private readonly IDependencyUtility _dependencyUtility;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebSocketDataHandler" /> class.
        /// </summary>
        /// <param name="dependencyUtility">The dependency utility.</param>
        protected WebSocketDataHandler(IDependencyUtility dependencyUtility)
        {
            _dependencyUtility = dependencyUtility ?? throw new ArgumentNullException(nameof(dependencyUtility));

            Logger = _dependencyUtility.Resolve<ILogger>();
            ConnectionManager = _dependencyUtility.Resolve<IWebSocketConnectionManager>();
        }

        /// <summary>
        /// Called when [open].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="T:PWebSocketServer.Common.Events.ConnectionOpenedEventArgs" /> instance containing the event data.</param>
        public virtual void OnOpen(IWebSocketConnection sender, ConnectionOpenedEventArgs args)
        {
        }

        /// <summary>
        /// Raises the Close event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="T:PWebSocketServer.Common.Events.ConnectionClosedEventArgs" /> instance containing the event data.</param>
        public virtual void OnClose(IWebSocketConnection sender, ConnectionClosedEventArgs args)
        {
        }

        /// <summary>
        /// Called when [text].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="T:PWebSocketServer.Common.Events.TextFrameEventArgs" /> instance containing the event data.</param>
        public virtual void OnText(IWebSocketConnection sender, TextFrameEventArgs args)
        {
        }

        /// <summary>
        /// Called when [text part].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="T:PWebSocketServer.Common.Events.TextMultiFrameEventArgs" /> instance containing the event data.</param>
        public virtual void OnTextPart(IWebSocketConnection sender, TextMultiFrameEventArgs args)
        {
        }

        /// <summary>
        /// Called when [binary].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="T:PWebSocketServer.Common.Events.BinaryFrameEventArgs" /> instance containing the event data.</param>
        public virtual void OnBinary(IWebSocketConnection sender, BinaryFrameEventArgs args)
        {
        }

        /// <summary>
        /// Called when [binary part].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="T:PWebSocketServer.Common.Events.BinaryMultiFrameEventArgs" /> instance containing the event data.</param>
        public virtual void OnBinaryPart(IWebSocketConnection sender, BinaryMultiFrameEventArgs args)
        {
        }

        /// <summary>
        /// Called when [ping].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="T:PWebSocketServer.Common.Events.PingEventArgs" /> instance containing the event data.</param>
        public virtual void OnPing(IWebSocketConnection sender, PingEventArgs args)
        {
        }

        /// <summary>
        /// Called when [pong].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="T:PWebSocketServer.Common.Events.PingEventArgs" /> instance containing the event data.</param>
        public virtual void OnPong(IWebSocketConnection sender, PingEventArgs args)
        {
        }
    }
}