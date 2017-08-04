using System;
using System.IO;
using System.Text;
using Loki.Common.Enum.Frame;
using Loki.Interfaces.Frame;

namespace Loki.Server.Frame
{
    public class WebSocketFrameWriter : IWebSocketFrameWriter
    {        
        public Stream Stream { get; set; }
        
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
        private void Write(WebSocketOpCode opCode, byte[] payload, bool isLastFrame)
        {
            if (Stream == null || !Stream.CanWrite)
                return;
            
            using (MemoryStream memoryStream = new MemoryStream())
            {
                byte fin = isLastFrame ? (byte) 0x80 : (byte) 0x00;
                byte firstByte = (byte) (fin | (byte) opCode);
                
                //Write fin -> opcode
                memoryStream.WriteByte(firstByte);

                if (payload != null)
                {
                    //Write mask & payload length
                    byte secondByte;
                    if (payload.Length < 126)
                        secondByte = (byte)payload.Length;
                    else if (payload.Length <= ushort.MaxValue)
                        secondByte = 126;
                    else
                        secondByte = 127;

                    memoryStream.WriteByte(secondByte);

                    //Write extended payload length if necessary
                    if (secondByte > 125)
                    {
                        byte[] payloadLengthBuffer = secondByte > 126 ? 
                            BitConverter.GetBytes((ulong)payload.Length) :
                            BitConverter.GetBytes((ushort)payload.Length);

                        memoryStream.Write(payloadLengthBuffer, 0, payloadLengthBuffer.Length);
                    }

                    //Write Payload
                    memoryStream.Write(payload, 0, payload.Length);
                }

                byte[] buffer = memoryStream.ToArray();

                try
                {
                    if (Stream != null && Stream.CanWrite)
                        Stream?.Write(buffer, 0, buffer.Length);
                }
                catch (ObjectDisposedException)
                {
                    //Console.WriteLine("EXCEPTION");
                }
                catch (IOException)
                {
                    //Console.WriteLine("EXCEPTION");
                }
            }
        }
    }
}
