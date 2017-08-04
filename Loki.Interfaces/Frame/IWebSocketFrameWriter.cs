using System.IO;

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
        Stream Stream { get; set; }

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
    }
}