using System;
using Loki.Interfaces.Threading;
using Loki.Server.Threading;
using Moq;
using Xunit;

namespace Loki.UnitTests.Threading
{
    public class ThreadHelperTests
    {
        [Fact]
        public void CreateAndRunThrowsExceptionWithNullTarget()
        {
            IThreadHelper threadHelper = new ThreadHelper();
            Assert.Throws(typeof(ArgumentNullException), () => threadHelper.CreateAndRun(null));
        }

        [Fact]
        public void CreateWithNoParamsThrowsExceptionWithNullTarget()
        {
            IThreadHelper threadHelper = new ThreadHelper();
            Assert.Throws(typeof(ArgumentNullException), () => threadHelper.Create(null));
        }

        [Fact]
        public void CreateThrowsExceptionWithNullTarget()
        {
            IThreadHelper threadHelper = new ThreadHelper();
            Assert.Throws(typeof(ArgumentNullException), () => threadHelper.Create(null, It.IsAny<object>()));
        }
    }
}
