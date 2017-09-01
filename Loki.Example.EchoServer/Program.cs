using System;
using System.Net;
using Loki.Interfaces;

namespace Loki.Example.EchoServer
{
    class Program
    {
        static void Main(string[] args)
        {
            //Set port and host
            int port = Convert.ToInt32(1337);
            IPAddress host = IPAddress.Parse("0.0.0.0");

            //Start the server
            using (IServer server = new Server.WebSocketServer("MyServerName", host, port))
            {
                //Start listening and blocking the main thread
                server.Run();
            }
        }
    }
}