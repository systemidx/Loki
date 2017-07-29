using System;
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
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            //Mock<ITcpHandler> fakeTcpHandler = new Mock<ITcpHandler>();
            
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
    }
}
