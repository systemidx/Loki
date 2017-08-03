using Loki.Common.Events;
using Loki.Interfaces.Connections;
using Loki.Interfaces.Dependency;
using Loki.Interfaces.Logging;
using Loki.Server.Attributes;
using Loki.Server.Data;

namespace Loki.Example.Logging
{
    [ConnectionRoute("/")]
    public class DefaultRoute : WebSocketDataHandler
    {
        public DefaultRoute(IDependencyUtility dependencyUtility) : base(dependencyUtility)
        {
        }

        public override void OnText(IWebSocketConnection sender, TextFrameEventArgs args)
        {
            Logger.Info($"{sender.ClientIdentifier}/{sender.UniqueIdentifier} sent {args.Message.Length * 2} bytes");
            
            base.OnText(sender, args);
        }
    }
}
