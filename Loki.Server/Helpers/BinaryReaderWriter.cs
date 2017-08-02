using System;
using System.IO;

namespace Loki.Server.Helpers
{
    public class BinaryReaderWriter
    {
        #region Read Methods

        /// <summary>
        /// Reads the stream an exact length and returns a byte array.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        public static byte[] ReadExactly(int length, Stream stream)
        {
            byte[] buffer = new byte[length];
            if (length == 0)
                return buffer;

            int offset = 0;
            do
            {
                int bytesRead = stream.Read(buffer, offset, length - offset);
                if (bytesRead == 0)
                    throw new EndOfStreamException($"Unexpected end of stream encountered whilst attempting to read {length:#,##0} bytes");

                offset += bytesRead;
            } while (offset < length);

            return buffer;
        }

        /// <summary>
        /// Reads the u short exactly.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="isLittleEndian">if set to <c>true</c> [is little endian].</param>
        /// <returns></returns>
        public static ushort ReadUShortExactly(Stream stream, bool isLittleEndian)
        {
            byte[] lenBuffer = BinaryReaderWriter.ReadExactly(2, stream);

            if (!isLittleEndian)
                Array.Reverse(lenBuffer);

            return BitConverter.ToUInt16(lenBuffer, 0);
        }

        /// <summary>
        /// Reads the u long exactly.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="isLittleEndian">if set to <c>true</c> [is little endian].</param>
        /// <returns></returns>
        public static ulong ReadULongExactly(Stream stream, bool isLittleEndian)
        {
            byte[] lenBuffer = BinaryReaderWriter.ReadExactly(8, stream);

            if (!isLittleEndian)
                Array.Reverse(lenBuffer);

            return BitConverter.ToUInt64(lenBuffer, 0);
        }

        /// <summary>
        /// Reads the long exactly.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="isLittleEndian">if set to <c>true</c> [is little endian].</param>
        /// <returns></returns>
        public static long ReadLongExactly(Stream stream, bool isLittleEndian)
        {
            byte[] lenBuffer = BinaryReaderWriter.ReadExactly(8, stream);

            if (!isLittleEndian)
                Array.Reverse(lenBuffer);

            return BitConverter.ToInt64(lenBuffer, 0);
        }

        #endregion

        #region Write Methods

        /// <summary>
        /// Writes the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="isLittleEndian">if set to <c>true</c> [is little endian].</param>
        private static void Write(byte[] buffer, Stream stream, bool isLittleEndian)
        {
            if (!isLittleEndian)
                Array.Reverse(buffer);

            stream.Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Writes the u long.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="isLittleEndian">if set to <c>true</c> [is little endian].</param>
        public static void WriteULong(ulong value, Stream stream, bool isLittleEndian)
        {
            byte[] buffer = BitConverter.GetBytes(value);

            Write(buffer, stream, isLittleEndian);
        }

        /// <summary>
        /// Writes the long.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="isLittleEndian">if set to <c>true</c> [is little endian].</param>
        public static void WriteLong(long value, Stream stream, bool isLittleEndian)
        {
            byte[] buffer = BitConverter.GetBytes(value);

            Write(buffer, stream, isLittleEndian);
        }

        /// <summary>
        /// Writes the u short.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="isLittleEndian">if set to <c>true</c> [is little endian].</param>
        public static void WriteUShort(ushort value, Stream stream, bool isLittleEndian)
        {
            byte[] buffer = BitConverter.GetBytes(value);

            Write(buffer, stream, isLittleEndian);
        }

        #endregion
    }
}
