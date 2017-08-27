using System.IO;
using Loki.Common.Enum.Frame;

namespace Loki.Interfaces.Frame
{
    public interface IWebSocketFrameWriter
    {
        /// <summary>
        /// Gets or sets the stream.
        /// </summary>
        /// <value>
        /// The stream.
        /// </value>
        Stream Stream { set; }

        /// <summary>
        /// Writes the close.
        /// </summary>
        void WriteClose();

        /// <summary>
        /// Writes the binary.
        /// </summary>
        /// <param name="payload">The payload.</param>
        void WriteBinary(byte[] payload);
        
        /// <summary>
        /// Writes the text.
        /// </summary>
        /// <param name="text">The text.</param>
        void WriteText(string text);

        /// <summary>
        /// Writes the specified op code.
        /// </summary>
        /// <param name="opCode">The op code.</param>
        /// <param name="payload">The payload.</param>
        /// <param name="isLastFrame">if set to <c>true</c> [is last frame].</param>
        void Write(WebSocketOpCode opCode, byte[] payload, bool isLastFrame);
    }
}