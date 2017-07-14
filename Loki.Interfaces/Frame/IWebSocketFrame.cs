using Loki.Common.Enum.Frame;

namespace Loki.Interfaces.Frame
{
    public interface IWebSocketFrame
    {
        /// <summary>
        /// Gets the decoded payload.
        /// </summary>
        /// <value>
        /// The decoded payload.
        /// </value>
        byte[] DecodedPayload { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is entire payload.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is entire payload; otherwise, <c>false</c>.
        /// </value>
        bool IsEntirePayload { get; }

        /// <summary>
        /// Returns true if ... is valid.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </value>
        bool IsValid { get; }

        /// <summary>
        /// Gets the op code.
        /// </summary>
        /// <value>
        /// The op code.
        /// </value>
        WebSocketOpCode OpCode { get; }
    }
}