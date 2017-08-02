using System;
using System.IO;
using System.Text;
using Loki.Server.Helpers;
using Xunit;

namespace Loki.UnitTests.Helpers
{
    public class BinaryReaderWriterTests
    {
        #region Read Tests

        [Fact]
        public void BinaryReaderReturnsEmptyArrayIfLengthIsZero()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] bytes = BinaryReaderWriter.ReadExactly(0, ms);
                Assert.Empty(bytes);
            }
        }

        [Fact]
        public void BinaryReaderReadsExactlyRightStringFromStream()
        {
            string expected = Guid.NewGuid().ToString();

            Encoding encoding = Encoding.ASCII;
            
            using (MemoryStream ms = new MemoryStream())
            {
                using (StreamWriter sw = new StreamWriter(ms, encoding))
                {
                    sw.Write(expected);
                    sw.Flush();

                    ms.Seek(0, SeekOrigin.Begin);

                    byte[] bytes = BinaryReaderWriter.ReadExactly(expected.Length, ms);

                    Assert.Equal(expected.Length, bytes.Length);
                    Assert.Equal(expected, encoding.GetString(bytes));
                }
            }
        }

        [Fact]
        public void BinaryReaderThrowsExceptionWhenLengthIsGreaterThanStream()
        {
            string expected = Guid.NewGuid().ToString();

            Encoding encoding = Encoding.ASCII;

            using (MemoryStream ms = new MemoryStream())
            {
                using (StreamWriter sw = new StreamWriter(ms, encoding))
                {
                    sw.Write(expected);
                    sw.Flush();

                    ms.Seek(0, SeekOrigin.Begin);

                    Assert.Throws(typeof(EndOfStreamException), () => BinaryReaderWriter.ReadExactly(expected.Length + 1, ms));
                }
            }
        }

        [Fact]
        public void BinaryReaderUShortReadsExactlyCorrectlyAsLittleEndian()
        {
            const UInt16 EXPECTED = 12345;

            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter sw = new BinaryWriter(ms))
                {
                    sw.Write(EXPECTED);
                    sw.Flush();

                    ms.Seek(0, SeekOrigin.Begin);

                    UInt16 actual = BinaryReaderWriter.ReadUShortExactly(ms, true);

                    Assert.Equal(EXPECTED, actual);
                }
            }
        }

        [Fact]
        public void BinaryReaderUShortReadsExactlyCorrectlyAsBigEndian()
        {
            const UInt16 INPUT = 12345;
            const UInt16 EXPECTED = 14640;

            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter sw = new BinaryWriter(ms))
                {
                    sw.Write(INPUT);
                    sw.Flush();

                    ms.Seek(0, SeekOrigin.Begin);

                    UInt16 actual = BinaryReaderWriter.ReadUShortExactly(ms, false);

                    Assert.Equal(EXPECTED, actual);
                }
            }
        }

        [Fact]
        public void BinaryReaderULongReadsExactlyCorrectlyAsLittleEndian()
        {
            const UInt64 EXPECTED = UInt64.MaxValue - 1;

            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter sw = new BinaryWriter(ms))
                {
                    sw.Write(EXPECTED);
                    sw.Flush();

                    ms.Seek(0, SeekOrigin.Begin);

                    UInt64 actual = BinaryReaderWriter.ReadULongExactly(ms, true);

                    Assert.Equal(EXPECTED, actual);
                }
            }
        }

        [Fact]
        public void BinaryReaderULongReadsExactlyCorrectlyAsBigEndian()
        {
            const UInt64 INPUT = UInt64.MaxValue - 1;
            const UInt64 EXPECTED = 18374686479671623679;

            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter sw = new BinaryWriter(ms))
                {
                    sw.Write(INPUT);
                    sw.Flush();

                    ms.Seek(0, SeekOrigin.Begin);

                    UInt64 actual = BinaryReaderWriter.ReadULongExactly(ms, false);

                    Assert.Equal(EXPECTED, actual);
                }
            }
        }

        [Fact]
        public void BinaryReaderLongReadsExactlyCorrectlyAsLittleEndian()
        {
            const long EXPECTED = long.MaxValue - 1;

            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter sw = new BinaryWriter(ms))
                {
                    sw.Write(EXPECTED);
                    sw.Flush();

                    ms.Seek(0, SeekOrigin.Begin);

                    long actual = BinaryReaderWriter.ReadLongExactly(ms, true);

                    Assert.Equal(EXPECTED, actual);
                }
            }
        }

        [Fact]
        public void BinaryReaderLongReadsExactlyCorrectlyAsBigEndian()
        {
            const long INPUT = long.MaxValue - 1;
            const long EXPECTED = -72057594037928065;

            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter sw = new BinaryWriter(ms))
                {
                    sw.Write(INPUT);
                    sw.Flush();

                    ms.Seek(0, SeekOrigin.Begin);

                    long actual = BinaryReaderWriter.ReadLongExactly(ms, false);

                    Assert.Equal(EXPECTED, actual);
                }
            }
        }

        #endregion

        #region Write Tests

        [Fact]
        public void BinaryWriterWritesULongToStream()
        {
            const ulong EXPECTED = ulong.MaxValue - 1;

            using (MemoryStream ms = new MemoryStream())
            {
                BinaryReaderWriter.WriteULong(EXPECTED, ms, true);

                ms.Seek(0, SeekOrigin.Begin);

                ulong actual = BinaryReaderWriter.ReadULongExactly(ms, true);

                Assert.Equal(EXPECTED, actual);
            }
        }

        [Fact]
        public void BinaryWriterWritesULongToStreamAsBigEndian()
        {
            ulong expected = ulong.MaxValue;

            using (MemoryStream ms = new MemoryStream())
            {
                BinaryReaderWriter.WriteULong(expected, ms, false);

                ms.Seek(0, SeekOrigin.Begin);

                ulong actual = BinaryReaderWriter.ReadULongExactly(ms, false);

                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void BinaryWriterWritesLongToStream()
        {
            const long EXPECTED = long.MaxValue - 1;

            using (MemoryStream ms = new MemoryStream())
            {
                BinaryReaderWriter.WriteULong(EXPECTED, ms, true);

                ms.Seek(0, SeekOrigin.Begin);

                long actual = BinaryReaderWriter.ReadLongExactly(ms, true);

                Assert.Equal(EXPECTED, actual);
            }
        }

        [Fact]
        public void BinaryWriterWritesLongToStreamAsBigEndian()
        {
            long expected = long.MaxValue;

            using (MemoryStream ms = new MemoryStream())
            {
                BinaryReaderWriter.WriteLong(expected, ms, false);

                ms.Seek(0, SeekOrigin.Begin);

                long actual = BinaryReaderWriter.ReadLongExactly(ms, false);

                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void BinaryWriterWritesUShortToStream()
        {
            const ushort EXPECTED = ushort.MaxValue - 1;

            using (MemoryStream ms = new MemoryStream())
            {
                BinaryReaderWriter.WriteUShort(EXPECTED, ms, true);

                ms.Seek(0, SeekOrigin.Begin);

                ushort actual = BinaryReaderWriter.ReadUShortExactly(ms, true);

                Assert.Equal(EXPECTED, actual);
            }
        }

        [Fact]
        public void BinaryWriterWritesUShortToStreamAsBigEndian()
        {
            const ushort EXPECTED = ushort.MaxValue - 1;

            using (MemoryStream ms = new MemoryStream())
            {
                BinaryReaderWriter.WriteUShort(EXPECTED, ms, false);

                ms.Seek(0, SeekOrigin.Begin);

                ushort actual = BinaryReaderWriter.ReadUShortExactly(ms, false);

                Assert.Equal(EXPECTED, actual);
            }
        }

        #endregion
    }
}
