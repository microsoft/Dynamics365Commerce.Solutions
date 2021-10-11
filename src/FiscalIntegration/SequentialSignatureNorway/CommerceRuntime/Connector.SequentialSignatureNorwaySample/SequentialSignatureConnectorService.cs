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
        using System.Collections.Generic;
        using System.Security.Cryptography;
        using System.Text;
        using System.Threading.Tasks;
        using Contoso.CommerceRuntime.Connector.SequentialSignatureNorwaySample.Configuration;
        using Contoso.CommerceRuntime.Connector.SequentialSignatureNorwaySample.Entities;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.FiscalIntegration.Connector.Messages;
        using Microsoft.Dynamics.Commerce.Runtime.Handlers;
        using Microsoft.Dynamics.Commerce.Runtime.Localization.Entities.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.Localization.Services.Messages;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;
        using Newtonsoft.Json;

        /// <summary>
        /// The sequential signature connector service.
        /// </summary>
        public class SequentialSignatureConnectorService : INamedRequestHandlerAsync
        {
            /// <summary>
            /// Gets name of the handler.
            /// </summary>
            public string HandlerName => "ConnectorSequentialSignatureNorwaySample";

            /// <summary>
            /// Gets supported types.
            /// </summary>
            public IEnumerable<Type> SupportedRequestTypes => new[]
            {
                typeof(GetServiceStatusConnectorRequest),
                typeof(SubmitDocumentConnectorRequest),
            };

            /// <summary>
            /// Represents an entry point for the request handler.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <returns>The response.</returns>
            public Task<Response> Execute(Request request)
            {
                ThrowIf.Null(request, nameof(request));

                switch (request)
                {
                    case SubmitDocumentConnectorRequest submitDocumentConnectorRequest:
                        return SubmitDocument(submitDocumentConnectorRequest);
                    case GetServiceStatusConnectorRequest getServiceStatusConnectorRequest:
                        return IsReady(getServiceStatusConnectorRequest);
                    default:
                        throw new NotSupportedException($"Request '{request.GetType()}' is not supported.");
                }
            }

            /// <summary>
            /// Submits document to the service.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <returns>The response.</returns>
            private static async Task<Response> SubmitDocument(SubmitDocumentConnectorRequest request)
            {
                ThrowIf.NullOrWhiteSpace(request.Document, $"{nameof(request)}.{nameof(request.Document)}");
                ThrowIf.Null(request.FiscalIntegrationServiceInfo, $"{nameof(request)}.{nameof(request.FiscalIntegrationServiceInfo)}");
                ThrowIf.NullOrWhiteSpace(request.FiscalIntegrationServiceInfo.ServiceProperties, $"{nameof(request)}.{nameof(request.FiscalIntegrationServiceInfo)}.{nameof(request.FiscalIntegrationServiceInfo.ServiceProperties)}");

                SubmitDocumentConnectorResponse response = null;
                string configurationHashAlgorithmName = ConfigurationController.GetHashAlgorithmName(request.FiscalIntegrationServiceInfo.ServiceProperties).ToUpperInvariant();
                HashAlgorithmName hashAlgorithmName = new HashAlgorithmName(configurationHashAlgorithmName);
                GetCertificateResult getCertificateResult = await CertificateProvider
                    .GetCertificate(request.RequestContext, request.FiscalIntegrationServiceInfo.ServiceProperties)
                    .ConfigureAwait(false);

                if (getCertificateResult.FailureDetails != null)
                {
                    return CreateFailedResponse(getCertificateResult.FailureDetails);
                }

                try
                {
                    var getSignedDataRequest = new GetDataSignatureRequest(request.Document, getCertificateResult.Certificate, hashAlgorithmName, Encoding.UTF8);
                    response = await SignData(getSignedDataRequest, request.RequestContext).ConfigureAwait(false);
                }
                catch (NotSupportedException)
                {
                    var signingFailureDetails = new FiscalIntegrationServiceFailureDetails
                    {
                        ErrorCode = ErrorCodeConstants.SigningError,
                        ErrorMessage = "An error occurred while trying to sign the data. The certificate does not meet the requirements of the hash algorithm used.",
                        FailureType = FiscalIntegrationServiceFailureType.Other,
                    };

                    return CreateFailedResponse(signingFailureDetails);
                }

                return response;
            }

            /// <summary>
            /// Checks if the service is available.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <returns>The response.</returns>
            private static async Task<Response> IsReady(GetServiceStatusConnectorRequest request)
            {
                ThrowIf.NullOrWhiteSpace(request.FiscalIntegrationServiceInfo.ServiceProperties, $"{nameof(request)}.{nameof(request.FiscalIntegrationServiceInfo)}.{nameof(request.FiscalIntegrationServiceInfo.ServiceProperties)}");

                bool isReady = true;
                bool isReadyRequired = ConfigurationController.GetIsReadyRequired(request.FiscalIntegrationServiceInfo.ServiceProperties);

                if (isReadyRequired)
                {
                    try
                    {
                        GetCertificateResult getCertificateResult = await CertificateProvider
                                .GetCertificate(request.RequestContext, request.FiscalIntegrationServiceInfo.ServiceProperties)
                                .ConfigureAwait(false);
                        if (getCertificateResult.FailureDetails != null)
                        {
                            isReady = false;
                        }
                    }
                    catch (Exception)
                    {
                        isReady = false;
                    }
                }

                return new GetServiceStatusConnectorResponse(new FiscalIntegrationServiceStatus { IsReady = isReady });
            }


            /// <summary>
            /// Signs data and forms a response.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <param name="requestContext">The request context.</param>
            /// <returns>The submit document connector response.</returns>
            private static async Task<SubmitDocumentConnectorResponse> SignData(GetDataSignatureRequest request, RequestContext requestContext)
            {
                var response = await requestContext.ExecuteAsync<GetDataSignatureResponse>(request).ConfigureAwait(false);

                var signature = Convert.ToBase64String(response.Signature);
                var sequentialSignatureRegistrationResult = new SequentialSignatureRegistrationResult
                {
                    Signature = signature,
                    CertificateThumbprint = request.Certificate.Thumbprint,
                    HashAlgorithm = response.HashAlgorithmName.Name,
                };
                var responseString = JsonConvert.SerializeObject(sequentialSignatureRegistrationResult);

                return new SubmitDocumentConnectorResponse(new FiscalIntegrationServiceCommunicationResult(
                    response: responseString,
                    communicationResultType: FiscalIntegrationServiceCommunicationResultType.Succeeded,
                    failureDetails: new FiscalIntegrationServiceFailureDetails(),
                    fiscalIntegrationServiceInfo: string.Empty));
            }

            /// <summary>
            /// Creates submit document connector response with failed data.
            /// </summary>
            /// <param name="failureDetails">The fiscal integration failure details.</param>
            /// <returns>The failed submit document connector response.</returns>
            private static SubmitDocumentConnectorResponse CreateFailedResponse(FiscalIntegrationServiceFailureDetails failureDetails)
            {
                return new SubmitDocumentConnectorResponse(
                    new FiscalIntegrationServiceCommunicationResult(
                        response: string.Empty,
                        communicationResultType: FiscalIntegrationServiceCommunicationResultType.Failed,
                        failureDetails: failureDetails,
                        fiscalIntegrationServiceInfo: string.Empty));
            }
        }
    }
}