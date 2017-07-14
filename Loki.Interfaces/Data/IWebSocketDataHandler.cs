using Loki.Common.Events;
using Loki.Interfaces.Connections;

namespace Loki.Interfaces.Data
{
    public interface IWebSocketDataHandler
    {
        /// <summary>
        /// Called when [open].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="ConnectionOpenedEventArgs"/> instance containing the event data.</param>
        void OnOpen(IWebSocketConnection sender, ConnectionOpenedEventArgs args);

        /// <summary>
        /// Raises the Close event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="ConnectionClosedEventArgs"/> instance containing the event data.</param>
        void OnClose(IWebSocketConnection sender, ConnectionClosedEventArgs args);

        /// <summary>
        /// Called when [text].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="TextFrameEventArgs"/> instance containing the event data.</param>
        void OnText(IWebSocketConnection sender, TextFrameEventArgs args);

        /// <summary>
        /// Called when [text part].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="TextMultiFrameEventArgs"/> instance containing the event data.</param>
        void OnTextPart(IWebSocketConnection sender, TextMultiFrameEventArgs args);

        /// <summary>
        /// Called when [binary].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="BinaryFrameEventArgs"/> instance containing the event data.</param>
        void OnBinary(IWebSocketConnection sender, BinaryFrameEventArgs args);

        /// <summary>
        /// Called when [binary part].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="BinaryMultiFrameEventArgs"/> instance containing the event data.</param>
        void OnBinaryPart(IWebSocketConnection sender, BinaryMultiFrameEventArgs args);

        /// <summary>
        /// Called when [ping].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="PingEventArgs"/> instance containing the event data.</param>
        void OnPing(IWebSocketConnection sender, PingEventArgs args);

        /// <summary>
        /// Called when [pong].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="PingEventArgs"/> instance containing the event data.</param>
        void OnPong(IWebSocketConnection sender, PingEventArgs args);
    }
}