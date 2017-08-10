using System;
using System.IO;
using System.Net.Sockets;
using Loki.Common.Enum.Frame;
using Loki.Interfaces.Dependency;
using Loki.Interfaces.Frame;
using Loki.Interfaces.Logging;
using Loki.Server.Helpers;

namespace Loki.Server.Frame
{
    public class WebSocketFrameReader : IWebSocketFrameReader
    {
        #region Readonly Variables

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
        /// Initializes a new instance of the <see cref="WebSocketFrameReader"/> class.
        /// </summary>
        /// <param name="dependencyUtility">The dependency utility.</param>
        public WebSocketFrameReader(IDependencyUtility dependencyUtility)
        {
            _logger = dependencyUtility.Resolve<ILogger>();
        }

        #endregion
        /// <summary>
        /// Reads the specified stream.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <returns></returns>
        public IWebSocketFrame Read(TcpClient connection)
        {
            const byte FIN_BIT_FLAG = 0x80;
            const byte OP_CODE_FLAG = 0x0F;
            const byte MASK_FLAG = 0x80;
            const int KEY_LENGTH = 4;

            if (!connection.Connected)
                return null;

            //Process the first byte of the packet
            byte firstByte;
            try
            {
                firstByte = (byte)Stream.ReadByte();
            }
            catch (IOException ex)
            {
                _logger.Error(ex);
                return null;
            }

            bool isFinBitSet = (firstByte & FIN_BIT_FLAG) == FIN_BIT_FLAG;

            //Gets the op code which tells us what kind of payload we're looking at (binary, message, etc)
            WebSocketOpCode opCode = (WebSocketOpCode)(firstByte & OP_CODE_FLAG);

            //Process the second byte of the packet
            byte secondByte = (byte)Stream.ReadByte();

            //Get the mask bit and toss an exception if it's false. Per the RFC this should never be false.
            bool isMaskBitSet = (secondByte & MASK_FLAG) == MASK_FLAG;
            if (!isMaskBitSet)
                return null;

            //Grab the payload length
            uint payloadLength = ReadPayloadLength(secondByte, Stream);

            //Grab the crypto key from the stream
            byte[] maskKey = StreamHelper.ReadExactly(KEY_LENGTH, Stream);

            //Grab the encrypted payload
            byte[] encodedPayload = StreamHelper.ReadExactly((int)payloadLength, Stream);

            //Decrypt the payload
            byte[] decodedPayload = new byte[payloadLength];
            for (int i = 0; i < encodedPayload.Length; i++)
                decodedPayload[i] = (Byte)(encodedPayload[i] ^ maskKey[i % KEY_LENGTH]);

            return new WebSocketFrame(isFinBitSet, true, opCode, decodedPayload);
        }

        /// <summary>
        /// Reads this instance.
        /// </summary>
        /// <returns></returns>
        public IWebSocketFrame Read()
        {
            const byte FIN_BIT_FLAG = 0x80;
            const byte OP_CODE_FLAG = 0x0F;
            const byte MASK_FLAG = 0x80;
            const int KEY_LENGTH = 4;

            //Process the first byte of the packet
            byte firstByte;
            try
            {
                firstByte = (byte)Stream.ReadByte();
            }
            catch (IOException)
            {
                return null;
            }

            bool isFinBitSet = (firstByte & FIN_BIT_FLAG) == FIN_BIT_FLAG;

            //Gets the op code which tells us what kind of payload we're looking at (binary, message, etc)
            WebSocketOpCode opCode = (WebSocketOpCode)(firstByte & OP_CODE_FLAG);

            //Process the second byte of the packet
            byte secondByte = (byte)Stream.ReadByte();

            //Get the mask bit and toss an exception if it's false. Per the RFC this should never be false.
            bool isMaskBitSet = (secondByte & MASK_FLAG) == MASK_FLAG;
            if (!isMaskBitSet)
                return null;

            //Grab the payload length
            uint payloadLength = ReadPayloadLength(secondByte, Stream);

            //Grab the crypto key from the stream
            byte[] maskKey = StreamHelper.ReadExactly(KEY_LENGTH, Stream);

            //Grab the encrypted payload
            byte[] encodedPayload = StreamHelper.ReadExactly((int)payloadLength, Stream);

            //Decrypt the payload
            byte[] decodedPayload = new byte[payloadLength];
            for (int i = 0; i < encodedPayload.Length; i++)
                decodedPayload[i] = (Byte)(encodedPayload[i] ^ maskKey[i % KEY_LENGTH]);

            return new WebSocketFrame(isFinBitSet, true, opCode, decodedPayload);
        }

        /// <summary>
        /// Reads the length.
        /// </summary>
        /// <param name="secondByte">The second byte.</param>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        private static uint ReadPayloadLength(byte secondByte, Stream stream)
        {
            const byte PAYLOAD_LEN_FLAG = 0x7F;
            const uint PAYLOAD_MAX_LENGTH = 2147483648; // 2GB

            uint payloadLength = (uint)(secondByte & PAYLOAD_LEN_FLAG);

            //If the value is 126, we need to read the next two bytes as per the RFC
            if (payloadLength == 126)
                payloadLength = StreamHelper.ReadUShortExactly(stream, false);

            //If the value is 127, we need to read the next eight bytes as per the RFC
            else if (payloadLength == 127)
            {
                payloadLength = (uint)StreamHelper.ReadULongExactly(stream, false);

                // protect ourselves against bad data
                if (payloadLength > PAYLOAD_MAX_LENGTH)
                    throw new ArgumentOutOfRangeException($"Payload length out of range. Min 0 max 2GB. Actual {payloadLength:#,##0} bytes.");
            }

            return payloadLength;
        }
    }
}