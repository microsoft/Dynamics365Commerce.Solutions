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
    namespace CommerceRuntime.Connector.SequentialSignatureNorwaySample
    {
        using System;
        using System.Security.Cryptography;
        using System.Security.Cryptography.X509Certificates;
        using System.Threading.Tasks;
        using Contoso.CommerceRuntime.Connector.SequentialSignatureNorwaySample.Configuration;
        using Contoso.CommerceRuntime.Connector.SequentialSignatureNorwaySample.Entities;
        using Contoso.CommerceRuntime.Connector.SequentialSignatureNorwaySample.Enums;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;

        /// <summary>
        /// Provides the certificate from the Key Vault secret string or from the local storage.
        /// </summary>
        internal static class CertificateProvider
        {
            /// <summary>
            /// Gets the certificate from the Key Vault or from the local storage.
            /// </summary>
            /// <param name="context">The request context.</param>
            /// <param name="serviceProperties">The service properties.</param>
            /// <returns>The certificate and error information.</returns>
            internal static async Task<GetCertificateResult> GetCertificate(RequestContext context, string serviceProperties)
            {
                ThrowIf.Null(context, nameof(context));
                ThrowIf.NullOrWhiteSpace(serviceProperties, nameof(serviceProperties));

                GetCertificateResult getCertificateResult = new GetCertificateResult();
                bool isLocalFirst = ConfigurationController.GetTryLocalCertificateFirst(serviceProperties);

                CertificateStorageType[] certificateStorageTypes = isLocalFirst
                    ? new[] { CertificateStorageType.LocalStorage, CertificateStorageType.KeyVault }
                    : new[] { CertificateStorageType.KeyVault, CertificateStorageType.LocalStorage };

                foreach (var certificateStorageType in certificateStorageTypes)
                {
                    if (getCertificateResult.Certificate == null)
                    {
                        switch (certificateStorageType)
                        {
                            case CertificateStorageType.LocalStorage:
                                getCertificateResult = await TryGetLocalCertificate(context, serviceProperties).ConfigureAwait(false);
                                break;
                            case CertificateStorageType.KeyVault:
                                getCertificateResult = await TryGetCertificateFromKeyVault(context, serviceProperties).ConfigureAwait(false);
                                break;
                        }
                    }
                }

                return getCertificateResult;
            }

            /// <summary>
            /// Gets the certificate from the Key Vault.
            /// </summary>
            /// <param name="context">The request context.</param>
            /// <param name="serviceProperties">The service properties.</param>
            /// <returns>The certificate and error information.</returns>
            private static async Task<GetCertificateResult> TryGetCertificateFromKeyVault(RequestContext context, string serviceProperties)
            {
                string secretName = ConfigurationController.GetSecretName(serviceProperties);
                X509Certificate2 certificate = null;
                FiscalIntegrationServiceFailureDetails failureDetails = null;
                var getCertRequest = new GetUserDefinedSecretStringValueServiceRequest(secretName);
                try
                {
                    var result = await context.ExecuteAsync<GetUserDefinedSecretStringValueServiceResponse>(getCertRequest).ConfigureAwait(false);
                    byte[] certificateBytes = Convert.FromBase64String(result.SecretStringValue);
                    certificate = new X509Certificate2(certificateBytes);
                }
                catch (Exception e)
                {
                    failureDetails = new FiscalIntegrationServiceFailureDetails
                    {
                        ErrorCode = ErrorCodeConstants.KeyVaultCertificateRetrievalError,
                        ErrorMessage = e.Message,
                        FailureType = FiscalIntegrationServiceFailureType.Other,
                    };
                }

                if (certificate != null && !certificate.HasPrivateKey)
                {
                    throw new CryptographicException($"The certificate with {secretName} secret name has no private key. Ensure that certificate was exported with a private key into Key Vault.");
                }

                return new GetCertificateResult(certificate, failureDetails);
            }

            /// <summary>
            /// Gets the certificate from the local certificate storage.
            /// </summary>
            /// <param name="context">The request context.</param>
            /// <param name="serviceProperties">The service properties.</param>
            /// <returns>The certificate and error information.</returns>
            private static async Task<GetCertificateResult> TryGetLocalCertificate(RequestContext context, string serviceProperties)
            {
                string thumbprint = ConfigurationController.GetLocalCertificateThumbprint(serviceProperties);
                string storeName = ConfigurationController.GetCertificateStoreName(serviceProperties);
                string storeLocation = ConfigurationController.GetCertificateStoreLocation(serviceProperties);
                X509Certificate2 certificate = null;
                FiscalIntegrationServiceFailureDetails failureDetails = null;
                var request = new GetUserDefinedSecretCertificateServiceRequest(string.Empty, string.Empty, thumbprint, storeName, storeLocation, null);

                try
                {
                    var result = await context.ExecuteAsync<GetUserDefinedSecretCertificateServiceResponse>(request).ConfigureAwait(false);
                    certificate = result.Certificate;
                }
                catch (Exception e)
                {
                    failureDetails = new FiscalIntegrationServiceFailureDetails
                    {
                        ErrorCode = ErrorCodeConstants.LocalCertificateRetrievalError,
                        ErrorMessage = e.Message,
                        FailureType = FiscalIntegrationServiceFailureType.Other,
                    };
                }

                if (certificate != null && !certificate.HasPrivateKey)
                {
                    throw new CryptographicException($"The certificate with {thumbprint} thumbprint has no private key. Ensure that the certificate has a private key and the required permissions are granted.");
                }

                return new GetCertificateResult(certificate, failureDetails);
            }
        }
    }
}