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
    namespace Commerce.Runtime.DocumentProvider.PosnetSample
    {
        using System;
        using System.Collections.Generic;
        using System.Linq;
        using System.Threading.Tasks;
        using Contoso.Commerce.Runtime.DocumentProvider.PosnetSample.DocumentBuilder;
        using Contoso.Commerce.Runtime.DocumentProvider.PosnetSample.DocumentModel;
        using Contoso.Commerce.Runtime.DocumentProvider.PosnetSample.Documents;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.FiscalIntegration.DocumentProvider.Messages;
        using Microsoft.Dynamics.Commerce.Runtime.Handlers;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;
        using Microsoft.Dynamics.Retail.Diagnostics;
        using Newtonsoft.Json;

        /// <summary>
        /// Sample document provider for fiscal printers supported POSNET protocol.
        /// </summary>
        public class DocumentProviderPosnetProtocol : INamedRequestHandlerAsync
        {
            private const string ServiceName = "POSNET";

            private enum Events
            {
                FiscalRegistrationResultResponseDeserializationFailed,
            }

            /// <summary>
            /// Gets the unique name for this request handler.
            /// </summary>
            public string HandlerName => "POSNETFiscalPrinterSample";

            /// <summary>
            /// Gets the supported fiscal integration event type.
            /// </summary>
            public IEnumerable<int> SupportedRegistrableEventsId
            {
                get
                {
                    return new[]
                    {
                        (int)FiscalIntegrationEventType.Sale,
                        (int)FiscalIntegrationEventType.FiscalXReport,
                        (int)FiscalIntegrationEventType.FiscalZReport,
                    };
                }
            }

            /// <summary>
            /// Gets the collection of supported request types by this handler.
            /// </summary>
            public IEnumerable<Type> SupportedRequestTypes
            {
                get
                {
                    return new[]
                    {
                        typeof(GetFiscalDocumentDocumentProviderRequest),
                        typeof(GetSupportedRegistrableEventsDocumentProviderRequest),
                        typeof(GetFiscalTransactionExtendedDataDocumentProviderRequest),
                    };
                }
            }

            /// <summary>
            /// Executes the specified request using the request.
            /// </summary>
            /// <param name="request">The request to execute.</param>
            /// <returns>The response of the request from the handler.</returns>
            public async Task<Response> Execute(Request request)
            {
                ThrowIf.Null(request, nameof(request));

                switch (request)
                {
                    case GetFiscalDocumentDocumentProviderRequest getFiscalDocumentDocumentProviderRequest:
                        return await this.GetFiscalDocumentAsync(getFiscalDocumentDocumentProviderRequest).ConfigureAwait(false);

                    case GetSupportedRegistrableEventsDocumentProviderRequest getSupportedRegistrableEventsDocumentProviderRequest:
                        return await Task.FromResult(this.GetSupportedRegisterableEvents(getSupportedRegistrableEventsDocumentProviderRequest));

                    case GetFiscalTransactionExtendedDataDocumentProviderRequest getFiscalTransactionExtendedDataDocumentProviderRequest:
                        return await Task.FromResult(this.GetFiscalTransactionExtendedData((GetFiscalTransactionExtendedDataDocumentProviderRequest)request));

                    default:
                        throw new NotSupportedException(string.Format("Request '{0}' is not supported.", request.GetType()));
                }
            }

            /// <summary>
            /// Gets the document builder according to fiscal event type.
            /// </summary>
            /// <param name="request">The fiscal document provider request.</param>
            /// <returns>Concrete builder.</returns>
            private IFiscalDocumentRequestBuilder GetPosnetDocumentRequestBuilder(GetFiscalDocumentDocumentProviderRequest request)
            {
                FiscalIntegrationEventType eventType = request.FiscalDocumentRetrievalCriteria.FiscalRegistrationEventType;

                IFiscalDocumentRequestBuilder builder;
                switch (eventType)
                {
                    case FiscalIntegrationEventType.Sale:
                        builder = new SalesReceiptBuilder(request);
                        break;

                    case FiscalIntegrationEventType.FiscalXReport:
                    case FiscalIntegrationEventType.FiscalZReport:
                        builder = new XZReportBuilder(request);
                        break;

                    default:
                        throw new NotSupportedException(string.Format("Fiscal integration event type '{0}' is not supported.", eventType));
                }

                return builder;
            }

            /// <summary>
            /// Gets the fiscal document document provider response.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <returns>The fiscal document document provider response.</returns>
            private async Task<Response> GetFiscalDocumentAsync(GetFiscalDocumentDocumentProviderRequest request)
            {
                ThrowIf.Null(request, nameof(request));

                string jsonDocument = string.Empty;
                var builtResult = await this.GetPosnetDocumentRequestBuilder(request).BuildAsync().ConfigureAwait(false);

                if (builtResult.DocumentGenerationResult == FiscalIntegrationDocumentGenerationResultType.Succeeded)
                {
                    jsonDocument = JsonConvert.SerializeObject(builtResult.Document);
                }

                FiscalIntegrationDocument fiscalIntegrationDocument = new FiscalIntegrationDocument(jsonDocument, builtResult.DocumentGenerationResult)
                {
                    DocumentAdjustment = builtResult.DocumentAdjustment,
                };

                GetFiscalDocumentDocumentProviderResponse response = new GetFiscalDocumentDocumentProviderResponse(fiscalIntegrationDocument);
                return response;
            }

            /// <summary>
            /// Gets the supported registerable events document provider response.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <returns>The supported registerable events document provider response.</returns>
            private Response GetSupportedRegisterableEvents(GetSupportedRegistrableEventsDocumentProviderRequest request)
            {
                ICollection<int> supportedFiscalEventTypes = this.SupportedRegistrableEventsId.ToList();
                GetSupportedRegistrableEventsDocumentProviderResponse response = new GetSupportedRegistrableEventsDocumentProviderResponse(supportedFiscalEventTypes, new List<int>());
                return response;
            }

            /// <summary>
            /// Gets the fiscal transaction extended data.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <returns>The fiscal transaction extended data..</returns>
            private GetFiscalTransactionExtendedDataDocumentProviderResponse GetFiscalTransactionExtendedData(GetFiscalTransactionExtendedDataDocumentProviderRequest request)
            {
                var extendedData = new List<CommerceProperty>();
                string documentNumber = string.Empty;
                string fiscalRegistrationResultResponse = request.FiscalRegistrationResult?.Response;

                // If the response string cannot be parsed, extendedData and documentNumber will be empty and no exception will be thrown.
                if (!string.IsNullOrWhiteSpace(fiscalRegistrationResultResponse) && this.DeserializeJson(fiscalRegistrationResultResponse, out var salesTransactionResponse))
                {
                    CommerceProperty[] commerceProperties = new[]
                    {
                        new CommerceProperty(ExtensibleFiscalRegistrationExtendedDataType.ServiceTransactionId.Name, salesTransactionResponse.RegistrationResult?.TransactionID),
                        new CommerceProperty(ExtensibleFiscalRegistrationExtendedDataType.Info.Name, salesTransactionResponse.RegistrationResult?.RegisterInfo),
                        new CommerceProperty(ExtensibleFiscalRegistrationExtendedDataType.SubmittedDocument.Name, salesTransactionResponse.RegistrationResult?.SubmittedDocument),
                    };
                    extendedData.AddRange(commerceProperties);
                    documentNumber = salesTransactionResponse.RegistrationResult?.TransactionID;
                }

                var registrationType = ExtensibleFiscalRegistrationType.CashSale;

                return new GetFiscalTransactionExtendedDataDocumentProviderResponse(
                    documentNumber ?? string.Empty,
                    registrationType,
                    ServiceName,
                    request.RequestContext.GetPrincipal().CountryRegionIsoCode,
                    extendedData);
            }

            /// <summary>
            /// Deserializes JSON string into PosnetTransactionRegistrationResponse object.
            /// </summary>
            /// <param name="source">The JSON string.</param>
            /// <param name="result">The deserialized object.</param>
            /// <returns>True if deserialized successfully; false otherwise.</returns>
            private bool DeserializeJson(string source, out PosnetTransactionRegistrationResponse result)
            {
                try
                {
                    result = JsonConvert.DeserializeObject<PosnetTransactionRegistrationResponse>(source);
                    return result != null;
                }
                catch (JsonException ex)
                {
                    result = default(PosnetTransactionRegistrationResponse);
                    RetailLogger.Log.LogDebug(Events.FiscalRegistrationResultResponseDeserializationFailed, ex, "Deserialization of fiscal registration response has failed.");

                    return false;
                }
            }
        }
    }
}
