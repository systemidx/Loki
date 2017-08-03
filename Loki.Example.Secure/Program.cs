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