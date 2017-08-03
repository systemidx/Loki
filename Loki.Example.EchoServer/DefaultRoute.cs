using Loki.Common.Events;
using Loki.Interfaces.Connections;
using Loki.Interfaces.Logging;
using Loki.Server.Attributes;
using Loki.Server.Data;

namespace Loki.Example.EchoServer
{
    [ConnectionRoute("/")]
    public class DefaultRoute : WebSocketDataHandler
    {
        public DefaultRoute(ILogger logger) : base(logger)
        {
        }

        public override void OnText(IWebSocketConnection sender, TextFrameEventArgs args)
        {
            //Echo the text back to the client
            sender.SendText(args.Message);

            base.OnText(sender, args);
        }
    }
}
