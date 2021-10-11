/**
 * SAMPLE CODE NOTICE
 *
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

namespace Contoso
{
    namespace CommerceRuntime.Connector.SequentialSignatureNorwaySample.Entities
    {
        using System.Security.Cryptography.X509Certificates;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;

        /// <summary>
        /// Represents the certificate result.
        /// </summary>
        internal class GetCertificateResult
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="GetCertificateResult"/> class.
            /// </summary>
            internal GetCertificateResult()
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="GetCertificateResult"/> class.
            /// </summary>
            /// <param name="certificate">The certificate.</param>
            /// <param name="failureDetails">The fiscal integration service failure details.</param>
            internal GetCertificateResult(X509Certificate2 certificate, FiscalIntegrationServiceFailureDetails failureDetails)
            {
                this.Certificate = certificate;
                this.FailureDetails = failureDetails;
            }

            /// <summary>
            /// Gets the certificate.
            /// </summary>
            internal X509Certificate2 Certificate { get; }

            /// <summary>
            /// Gets the fiscal integration service failure details.
            /// </summary>
            internal FiscalIntegrationServiceFailureDetails FailureDetails { get; }
        }
    }
}