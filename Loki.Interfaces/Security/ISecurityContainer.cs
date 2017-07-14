using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace Loki.Interfaces.Security
{
    public interface ISecurityContainer
    {
        X509Certificate2 Certificate { get; }
        SslProtocols EnabledProtocols { get; }
        bool Enabled { get; }
        bool ClientCertificateRequired { get; }
        bool CertificateRevocationEnabled { get; }
    }
}