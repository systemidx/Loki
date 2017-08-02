using System;
using System.Threading;
using Loki.Interfaces;
using Loki.Interfaces.Connections;
using Loki.Interfaces.Data;
using Loki.Interfaces.Dependency;
using Loki.Interfaces.Logging;
using Loki.Interfaces.Security;
using Loki.Interfaces.Threading;
using Loki.Server;
using Loki.Server.Data;
using Loki.Server.Dependency;
using Moq;
using Xunit;

namespace Loki.UnitTests
{
    public class ServerTests
    {
        [Fact]
        public void ServerRunningFlagReturnsCorrectStatus()
        {
            IDependencyUtility dependencyUtility = new DependencyUtility();

            dependencyUtility.Register<ILogger>(new Mock<ILogger>().Object);
            dependencyUtility.Register<IRouteTable>(new Mock<IRouteTable>().Object);
            dependencyUtility.Register<ISecurityContainer>(new Mock<ISecurityContainer>().Object);
            dependencyUtility.Register<ITcpHandler>(new Mock<ITcpHandler>().Object);
            dependencyUtility.Register<IThreadHelper>(new Mock<IThreadHelper>().Object);

            using (IServer server = new Server.Server(string.Empty, string.Empty, 0, dependencyUtility, 1, 3))
            {
                server.Run(false);

                Assert.True(server.IsRunning);

                server.Stop();

                Assert.False(server.IsRunning);
            }
         }

        [Fact]
        public void ServerRunningSpawnsCorrectAmountOfThreads()
        {
            IDependencyUtility dependencyUtility = new DependencyUtility();

            Mock<IThreadHelper> threadHelper = new Mock<IThreadHelper>();

            dependencyUtility.Register<ILogger>(new Mock<ILogger>().Object);
            dependencyUtility.Register<IRouteTable>(new Mock<IRouteTable>().Object);
            dependencyUtility.Register<ISecurityContainer>(new Mock<ISecurityContainer>().Object);
            dependencyUtility.Register<ITcpHandler>(new Mock<ITcpHandler>().Object);
            dependencyUtility.Register<IThreadHelper>(threadHelper.Object);

            using (IServer server = new Server.Server(string.Empty, string.Empty, 0, dependencyUtility, 2, 4))
            {
                server.Run(false);

                //Listener threads: 2
                //Client threads: 2 * 4 (multiplicand)
                //Dead Connection thread: 1
                // = 11
                threadHelper.Verify(x => x.CreateAndRun(It.IsAny<ThreadStart>()), Times.Exactly(11));
                server.Stop();
            }
        }
    }
}
