# Loki
A C# based websocket server designed for high throughput, efficiency, and modularization.

## Targets
* .NET Core 1.1
* .NET Standard 1.6
* .NET Framework 4.5.2
* .NET Framework 4.6
* .NET Framework 4.6.1
* .NET Framework 4.6.2

## Project Status
This project is currently in **active development**. As such, please think twice before using it in production! For a more mature project, please take a look at [WebSocket-Sharp](https://github.com/sta/websocket-sharp).

## Project Rationale
The reason for the creation of this project comes mainly from frustration with existing projects (isn't that always how it is?). The particularly use case for Loki's creation is to implement a WebSocket server which would scale well, accept modular drop-ins for enhancing the engine, and be RFC 6455 compliant.

## Examples

### Minimum Setup
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
            Logger.Info($"QUIT {sender.UniqueIdentifier}/{sender.ClientIdentifier}");
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

This tells the server that any request that comes in with the `/` route will be handled by that particular WebSocketDataHandler. The route object is subscribed to a multitude of events. It handles both text and binary transmissions as well as partial and full frames. Loki can also handle multiple routes per server instance. The routes are picked up through reflection on server start by annotating the class with the ConnectionRoute attribute. 

The next step is instantiating the server itself. The code below binds the server to any IPv4 address available.

```cs
using System;
using System.Net;
using Loki.Interfaces;
using Loki.Server;

namespace LokiCoreServer
{
    class Program
    {
        static void Main(string[] args)
        {
            //Set port and host
            int port = Convert.ToInt32(1337);
            IPAddress host = IPAddress.Parse("0.0.0.0");
            
            //Start the server
            using (IServer server = new Server("MyServerName", host, port))
            {
                //Start listening and blocking the main thread
                server.Run();
            }
        }
    }
}
```

### Dependency Injection / Inversion of Control
To combat some of the difficulty around unit testing a networked application, and to avoid having needless dependencies; Loki uses its own very simplistic DI class called `DependencyUtility`. It is passed into requisite classes via its interface `IDependencyUtility`.

```cs
using System;
using System.Net;
using Loki.Common.Events;
using Loki.Interfaces;
using Loki.Interfaces.Dependency;
using Loki.Interfaces.Logging;
using Loki.Server.Dependency;
using Loki.Server.Logging;

namespace Loki.Example.Logging
{
    class Program
    {
        static void Main(string[] args)
        {
            //Create our log wrapper
            ILogger logger = new Logger();

            //Create our dependency utility and register the logger
            IDependencyUtility dependencyUtility = new DependencyUtility();
            dependencyUtility.Register<ILogger>(logger);

            //Set port and host
            int port = Convert.ToInt32(1337);
            IPAddress host = IPAddress.Parse("0.0.0.0");

            //Start the server
            using (IServer server = new Server.Server("MyServerName", host, port, dependencyUtility))
            {
                //Start listening and blocking the main thread
                server.Run();
            }
        }
    }
}
```

### Logging
Loki comes equipped with an event-based logging class which you can use with a pre-existing library like Log4Net or roll your own.

```cs
using System;
using System.Net;
using Loki.Common.Events;
using Loki.Interfaces;
using Loki.Interfaces.Dependency;
using Loki.Interfaces.Logging;
using Loki.Server.Dependency;
using Loki.Server.Logging;

namespace Loki.Example.Logging
{
    class Program
    {
        static void Main(string[] args)
        {
            //Create our log wrapper and events
            ILogger logger = new Logger();
            logger.OnError += OnError;
            logger.OnDebug += OnDebug;
            logger.OnWarn += OnWarn;
            logger.OnInfo += OnInfo;

            //Create our dependency utility and register the logger
            IDependencyUtility dependencyUtility = new DependencyUtility();
            dependencyUtility.Register<ILogger>(logger);

            //Set port and host
            int port = Convert.ToInt32(1337);
            IPAddress host = IPAddress.Parse("0.0.0.0");

            //Start the server
            using (IServer server = new Server.Server("MyServerName", host, port, dependencyUtility))
            {
                //Start listening and blocking the main thread
                server.Run();
            }
        }

        private static void OnError(object sender, LokiErrorEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[{e.EventTimeStamp}]\tERROR\t{e.Exception}");
            Console.ResetColor();
        }

        private static void OnDebug(object sender, LokiDebugEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"[{e.EventTimeStamp}]\tDEBUG\t{e.Message}");
            Console.ResetColor();
        }

        private static void OnInfo(object sender, LokiInfoEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"[{e.EventTimeStamp}]\tDEBUG\t{e.Message}");
            Console.ResetColor();
        }
        private static void OnWarn(object sender, LokiWarnEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[{e.EventTimeStamp}]\tWARN\t{e.Message}");
            Console.ResetColor();
        }
    }
}
```

### Secure WebSockets
Loki can consume an X509Certificate2 object via an `ISecurityContainer` to encrypt client to server traffic.

```cs
using System;
using System.Net;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using Loki.Interfaces;
using Loki.Interfaces.Dependency;
using Loki.Interfaces.Security;
using Loki.Server.Dependency;
using Loki.Server.Security;

namespace Loki.Example.Secure
{
    class Program
    {
        static void Main(string[] args)
        {
            //Create our dependency utility
            IDependencyUtility dependencyUtility = new DependencyUtility();

            //Create our security container
            ISecurityContainer securityContainer = GenerateSecurityContainer();
            
            //Register dependencies
            dependencyUtility.Register<ISecurityContainer>(securityContainer);

            //Set port and host
            int port = Convert.ToInt32(1337);
            IPAddress host = IPAddress.Parse("0.0.0.0");

            //Start the server
            using (IServer server = new Server.Server("MyServerName", host, port, dependencyUtility))
            {
                //Start listening and blocking the main thread
                server.Run();
            }
        }

        private static ISecurityContainer GenerateSecurityContainer()
        {
            X509Certificate2 certificate = new X509Certificate2("path/to/cert.pfx", "pfxpassword");

            //Certificate, Protocols, ClientCertRequired?, CertificateRevokationEnabled?, Enabled?
            return new SecurityContainer(certificate, SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12, false, true, true);
        }
    }
}
```
