using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using Loki.Interfaces.Security;

namespace Loki.Server.Security
{
    public class SecurityContainer : ISecurityContainer
    {
        #region Properties

        /// <summary>
        /// Gets the certificate.
        /// </summary>
        /// <value>
        /// The certificate.
        /// </value>
        public X509Certificate2 Certificate { get; }

        /// <summary>
        /// Gets the enabled protocols.
        /// </summary>
        /// <value>
        /// The enabled protocols.
        /// </value>
        public SslProtocols EnabledProtocols { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="SecurityContainer"/> is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        public bool Enabled { get; }

        /// <summary>
        /// Gets a value indicating whether [client certificate required].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [client certificate required]; otherwise, <c>false</c>.
        /// </value>
        public bool ClientCertificateRequired { get; }

        /// <summary>
        /// Gets a value indicating whether [certificate revocation enabled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [certificate revocation enabled]; otherwise, <c>false</c>.
        /// </value>
        public bool CertificateRevocationEnabled { get; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityContainer" /> class.
        /// </summary>
        /// <param name="certificate">The certificate.</param>
        /// <param name="enabledProtocols">The enabled protocols.</param>
        /// <param name="clientCertificateRequired">if set to <c>true</c> [client certificate required].</param>
        /// <param name="certificateRevocationEnabled">if set to <c>true</c> [certificate revocation enabled].</param>
        /// <param name="enabled">if set to <c>true</c> [enabled].</param>
        public SecurityContainer(X509Certificate2 certificate, SslProtocols enabledProtocols, bool clientCertificateRequired, bool certificateRevocationEnabled, bool enabled)
        {
            Certificate = certificate;
            EnabledProtocols = enabledProtocols;
            ClientCertificateRequired = clientCertificateRequired;
            CertificateRevocationEnabled = certificateRevocationEnabled;
            Enabled = enabled;
        }
    }
}
