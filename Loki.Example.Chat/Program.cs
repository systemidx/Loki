using System;
using System.Net;
using Loki.Common.Events;
using Loki.Interfaces;
using Loki.Interfaces.Dependency;
using Loki.Interfaces.Logging;
using Loki.Server.Dependency;
using Loki.Server.Logging;

namespace Loki.Example.Chat
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