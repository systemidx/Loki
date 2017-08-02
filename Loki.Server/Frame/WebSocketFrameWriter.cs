using System;
using System.IO;
using System.Text;
using Loki.Common.Enum.Frame;
using Loki.Interfaces.Frame;
using Loki.Server.Helpers;

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
            // best to write everything to a memory stream before we push it onto the wire
            // not really necessary but I like it this way
            using (MemoryStream memoryStream = new MemoryStream())
            {
                byte finBitSetAsByte = isLastFrame ? (byte) 0x80 : (byte) 0x00;
                byte byte1 = (byte) (finBitSetAsByte | (byte) opCode);
                memoryStream.WriteByte(byte1);

                // NB, dont set the mask flag. No need to mask data from server to client
                // depending on the size of the length we want to write it as a byte, ushort or ulong
                if (payload.Length < 126)
                {
                    byte byte2 = (byte) payload.Length;
                    memoryStream.WriteByte(byte2);
                }
                else if (payload.Length <= ushort.MaxValue)
                {
                    byte byte2 = 126;
                    memoryStream.WriteByte(byte2);
                    StreamHelper.WriteUShort((ushort) payload.Length, memoryStream, false);
                }
                else
                {
                    byte byte2 = 127;
                    memoryStream.WriteByte(byte2);
                    StreamHelper.WriteULong((ulong) payload.Length, memoryStream, false);
                }

                memoryStream.Write(payload, 0, payload.Length);
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
