using System;
using System.IO;
using System.Text;
using Loki.Common.Enum.Frame;
using Loki.Interfaces.Dependency;
using Loki.Interfaces.Frame;
using Loki.Interfaces.Logging;

namespace Loki.Server.Frame
{
    public class WebSocketFrameWriter : IWebSocketFrameWriter
    {
        #region Readonly Variables

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger _logger;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the stream.
        /// </summary>
        /// <value>
        /// The stream.
        /// </value>
        public Stream Stream { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="WebSocketFrameWriter"/> class.
        /// </summary>
        /// <param name="dependencyUtility">The dependency utility.</param>
        public WebSocketFrameWriter(IDependencyUtility dependencyUtility)
        {
            _logger = dependencyUtility.Resolve<ILogger>();
        }

        #endregion

        /// <summary>
        /// Writes the specified stream.
        /// </summary>
        /// <param name="opCode">The op code.</param>
        /// <param name="payload">The payload.</param>
        public void Write(WebSocketOpCode opCode, byte[] payload)
        {
            Write(opCode, payload, true);
        }

        /// <summary>
        /// Writes the binary.
        /// </summary>
        /// <param name="payload">The payload.</param>
        public void WriteBinary(byte[] payload)
        {
            Write(WebSocketOpCode.BinaryFrame, payload);
        }

        /// <summary>
        /// Writes the close.
        /// </summary>
        public void WriteClose()
        {
            Write(WebSocketOpCode.ConnectionClose, null, true); 
        }

        /// <summary>
        /// Writes the text.
        /// </summary>
        /// <param name="text">The text.</param>
        public void WriteText(string text)
        {
            byte[] responseBytes = Encoding.UTF8.GetBytes(text);

            Write(WebSocketOpCode.TextFrame, responseBytes);
        }

        /// <summary>
        /// Writes the specified stream.
        /// </summary>
        /// <param name="opCode">The op code.</param>
        /// <param name="payload">The payload.</param>
        /// <param name="isLastFrame">if set to <c>true</c> [is last frame].</param>
        public void Write(WebSocketOpCode opCode, byte[] payload, bool isLastFrame)
        {
            if (Stream == null || !Stream.CanWrite)
                return;
            
            const int FIN = 0x1;  //We are not supporting continuation frames from the server
            const int MASK = 0x0; //We cannot mask data flowing from the server to the client
            const int RSV = 0x0;  //RSVs are currently unused in the WebSocket specification

            using (MemoryStream memoryStream = new MemoryStream())
            {
                if (payload == null)
                    payload = new byte[0];

                //Get the payload length as per RFC 6455 (https://tools.ietf.org/html/rfc6455#section-5.2)
                //If it's below 126 bytes, just identify the length
                //If it's between 126 and 65535, use 126
                //If it's larger, use 127
                //This becomes important later as well when we need to identify the payload's length size (1 bytes, 2 bytes, or 8 bytes)
                byte payloadLength = (byte) (payload.Length < 126 ? payload.Length : (payload.Length <= UInt16.MaxValue ? 126 : 127));
                
                int header = FIN; //FIN (1 bit)
                header = (header << 1) + RSV; //RSV1 (1 bit, unused)
                header = (header << 1) + RSV; //RSV2 (1 bit, unused)
                header = (header << 1) + RSV; //RSV3 (1 bit, unused)
                header = (header << 4) + (int) opCode; //OP Code (4 bits)

                header = (header << 1) + MASK; //Mask (1 bit)
                header = (header << 7) + payloadLength; //Payload length

                //Write the first two bytes to the stream
                byte[] headerBytes = ConvertToMultiBytes((ushort)header, false);
                memoryStream.Write(headerBytes, 0, 2);

                //If we need to define the extended payload length
                if (payloadLength > 125)
                {
                    //Get the payload's entire length as either a 2 byte array or an 8 byte array
                    byte[] length = payloadLength == 126 ? 
                        ConvertToMultiBytes((ushort)payload.Length, false) : 
                        ConvertToMultiBytes((ulong)payload.Length, false);

                    //Write the variable bytes to a stream
                    memoryStream.Write(length, 0, payloadLength == 126 ? 2 : 8);
                }
                    
                //Write the payload to the stream
                if (payload.Length > 0)
                    memoryStream.Write(payload, 0, payload.Length);
                
                //Push entire frame to a byte array and send it to the client
                byte[] bufferArray = memoryStream.ToArray();

                try
                {
                    if (Stream != null && Stream.CanWrite)
                        Stream?.Write(bufferArray, 0, bufferArray.Length);
                }
                catch (ObjectDisposedException ex)
                {
                    _logger.Error(ex);
                }
                catch (IOException ex)
                {
                    _logger.Error(ex);
                }
            }
        }

        /// <summary>
        /// Converts to multi bytes.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="littleEndian">if set to <c>true</c> [little endian].</param>
        /// <returns></returns>
        private byte[] ConvertToMultiBytes(ushort value, bool littleEndian)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            if (BitConverter.IsLittleEndian && !littleEndian)
                Array.Reverse(bytes);

            return bytes;
        }

        /// <summary>
        /// Converts to multi bytes.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="littleEndian">if set to <c>true</c> [little endian].</param>
        /// <returns></returns>
        private byte[] ConvertToMultiBytes(ulong value, bool littleEndian)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            if (BitConverter.IsLittleEndian && !littleEndian)
                Array.Reverse(bytes);

            return bytes;
        }
    }
}
