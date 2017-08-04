using System;
using System.IO;
using System.Text;
using Loki.Server.Helpers;
using Xunit;

namespace Loki.UnitTests.Helpers
{
    public class StreamHelperTests
    {
        #region Read Tests

        [Fact]
        public void BinaryReaderReturnsEmptyArrayIfLengthIsZero()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] bytes = StreamHelper.ReadExactly(0, ms);
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

                    byte[] bytes = StreamHelper.ReadExactly(expected.Length, ms);

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

                    Assert.Throws(typeof(EndOfStreamException), () => StreamHelper.ReadExactly(expected.Length + 1, ms));
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

                    UInt16 actual = StreamHelper.ReadUShortExactly(ms, true);

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

                    UInt16 actual = StreamHelper.ReadUShortExactly(ms, false);

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

                    UInt64 actual = StreamHelper.ReadULongExactly(ms, true);

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

                    UInt64 actual = StreamHelper.ReadULongExactly(ms, false);

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

                    long actual = StreamHelper.ReadLongExactly(ms, true);

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

                    long actual = StreamHelper.ReadLongExactly(ms, false);

                    Assert.Equal(EXPECTED, actual);
                }
            }
        }

        #endregion
    }
}
