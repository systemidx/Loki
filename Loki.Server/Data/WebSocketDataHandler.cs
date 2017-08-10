using System;
using System.Collections.Concurrent;
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
        /// The partial text messages
        /// </summary>
        private readonly ConcurrentDictionary<string, string> _partialTextMessages = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// The partial text messages
        /// </summary>
        private readonly ConcurrentDictionary<string, byte[]> _partialBinaryMessages = new ConcurrentDictionary<string, byte[]>();

        #region Constructor

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

        #endregion

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
            if (!_partialTextMessages.ContainsKey(sender.UniqueClientIdentifier))
                _partialTextMessages[sender.UniqueClientIdentifier] = args.Message;
            else
                _partialTextMessages[sender.UniqueClientIdentifier] += args.Message;

            if (!args.IsLastFrame)
                return;

            OnText(sender, new TextFrameEventArgs(_partialTextMessages[sender.UniqueClientIdentifier]));

            _partialTextMessages[sender.UniqueClientIdentifier] = null;
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
            if (!_partialBinaryMessages.ContainsKey(sender.UniqueClientIdentifier))
                _partialBinaryMessages[sender.UniqueClientIdentifier] = args.Payload;
            else
            {
                byte[] tempStorage = new byte[_partialBinaryMessages[sender.UniqueClientIdentifier].Length + args.Payload.Length];

                Array.Copy(_partialBinaryMessages[sender.UniqueClientIdentifier], tempStorage, _partialBinaryMessages[sender.UniqueClientIdentifier].Length);
                Array.Copy(args.Payload, tempStorage, args.Payload.Length);

                _partialBinaryMessages[sender.UniqueClientIdentifier] = tempStorage;
            }
            
            if (!args.IsLastFrame)
                return;

            OnBinary(sender, new BinaryFrameEventArgs(_partialBinaryMessages[sender.UniqueClientIdentifier]));

            _partialBinaryMessages[sender.UniqueClientIdentifier] = null;
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