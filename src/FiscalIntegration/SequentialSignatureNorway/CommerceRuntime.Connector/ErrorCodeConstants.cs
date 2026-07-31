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
    namespace CommerceRuntime.Connector.SequentialSignNorway
    {
        /// <summary>
        /// The service failure details error codes.
        /// </summary>
        internal static class ErrorCodeConstants
        {
            /// <summary>
            /// The error code for the key vault certificate retrieval error.
            /// </summary>
            internal const string KeyVaultCertificateRetrievalError = "002";

            /// <summary>
            /// The error code for the local certificate retrieval error.
            /// </summary>
            internal const string LocalCertificateRetrievalError = "003";

            /// <summary>
            /// The error code for the signing error.
            /// </summary>
            internal const string SigningError = "001";
        }
    }
}
