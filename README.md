# Loki
A C# based websocket server designed for high throughput, efficiency, and modularization.

## Targets
------
* .NET Core 1.1
* .NET Standard 1.6
* .NET Framework 4.5.2
* .NET Framework 4.6
* .NET Framework 4.6.1
* .NET Framework 4.6.2

## Project Status
------
This project is currently in **active development**. As such, please think twice before using it in production! For a more mature project, please take a look at [WebSocket-Sharp](https://github.com/sta/websocket-sharp).

## Project Rationale
------
The reason for the creation of this project comes mainly from frustration with existing projects (isn't that always how it is?). The particularly use case for Loki's creation is to implement a WebSocket server which would scale well, accept modular drop-ins for enhancing the engine, and be RFC 6455 compliant.

## Examples
------

###Minimum Setup
------
For minimum setup, you need to do two things. The first is to create the default route. 

```cs
using Loki.Common.Events;
using Loki.Interfaces.Connections;
using Loki.Interfaces.Logging;
using Loki.Server.Attributes;
using Loki.Server.Data;

namespace LokiCoreServer.Routes
{
    [ConnectionRoute("/")]
    public class Default : WebSocketDataHandler
    {
        public override void OnOpen(IWebSocketConnection sender, ConnectionOpenedEventArgs args)
        {
            Logger.Info($"JOIN {sender.UniqueIdentifier}/{sender.ClientIdentifier}");
            base.OnOpen(sender, args);
        }

        public override void OnClose(IWebSocketConnection sender, ConnectionClosedEventArgs args)
        {
            Logger.Info($"DISC {sender.UniqueIdentifier}/{sender.ClientIdentifier}");
            base.OnClose(sender, args);
        }

        public override void OnText(IWebSocketConnection sender, TextFrameEventArgs args)
        {
            Logger.Debug($"RECV {sender.UniqueIdentifier}/{sender.ClientIdentifier} {args.Message.Length * 2} bytes");

            //Respond with the same text
            sender.SendText(args.Message);

            base.OnText(sender, args);
        }

        public Default(ILogger logger) : base(logger)
        {
            Logger.Debug("Route created: / => Default");
        }
    }
}
```