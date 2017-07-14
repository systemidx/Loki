using Loki.Common.Enum.Frame;
using Loki.Interfaces.Frame;

namespace Loki.Server.Frame
{
    public class WebSocketFrame : IWebSocketFrame
    {
        /// <summary>
        /// Gets a value indicating whether this instance is entire payload.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is entire payload; otherwise, <c>false</c>.
        /// </value>
        public bool IsEntirePayload { get; }

        /// <summary>
        /// Returns true if ... is valid.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </value>
        public bool IsValid { get; }

        /// <summary>
        /// Gets the decoded payload.
        /// </summary>
        /// <value>
        /// The decoded payload.
        /// </value>
        public byte[] DecodedPayload { get; }

        /// <summary>
        /// Gets the op code.
        /// </summary>
        /// <value>
        /// The op code.
        /// </value>
        public WebSocketOpCode OpCode { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebSocketFrame"/> class.
        /// </summary>
        /// <param name="isEntirePayload">if set to <c>true</c> [is entire payload].</param>
        /// <param name="isValid">if set to <c>true</c> [is valid].</param>
        /// <param name="opCode">The op code.</param>
        /// <param name="decodedPayload">The decoded payload.</param>
        public WebSocketFrame(bool isEntirePayload, bool isValid, WebSocketOpCode opCode, byte[] decodedPayload)
        {
            IsEntirePayload = isEntirePayload;
            IsValid = isValid;
            OpCode = opCode;
            DecodedPayload = decodedPayload;
        }
    }
}
