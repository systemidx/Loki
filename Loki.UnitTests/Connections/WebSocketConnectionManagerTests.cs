using System.Net.Sockets;
using System.Threading;
using Loki.Interfaces.Connections;
using Loki.Interfaces.Dependency;
using Loki.Interfaces.Threading;
using Loki.Server.Connections;
using Loki.Server.Dependency;
using Moq;
using Xunit;

namespace Loki.UnitTests.Connections
{
    public class WebSocketConnectionManagerTests
    {
        [Fact]
        public void ConnectionManagerRegistersAndStartsClientThread()
        {
            Mock<IThreadHelper> threadHelper = new Mock<IThreadHelper>();
            Mock<TcpClient> client = new Mock<TcpClient>();

            IDependencyUtility utility = new DependencyUtility();
            utility.Register<IThreadHelper>(threadHelper.Object);

            IWebSocketConnectionManager connectionManager = new WebSocketConnectionManager(utility);
            connectionManager.RegisterConnection(client.Object);

            threadHelper.Verify(x => x.CreateAndRun(It.IsAny<ThreadStart>()), Times.Once);
            Assert.Equal(1, connectionManager.TotalConnections);
        }

        [Fact]
        public void RemoveDeadConnectionsCleansUpCorrectly()
        {
            Mock<IThreadHelper> threadHelper = new Mock<IThreadHelper>();
            Mock<TcpClient> client = new Mock<TcpClient>();

            IDependencyUtility utility = new DependencyUtility();
            utility.Register<IThreadHelper>(threadHelper.Object);

            IWebSocketConnectionManager connectionManager = new WebSocketConnectionManager(utility);
            connectionManager.RegisterConnection(client.Object);
            connectionManager.RemoveDeadConnections();

            Assert.Equal(0, connectionManager.TotalConnections);
        }
    }
}