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
    namespace CommerceRuntime.DocumentProvider.AtolSample.Handlers
    {
        using System;
        using System.Collections.Generic;
        using System.Threading.Tasks;
        using Contoso.CommerceRuntime.DocumentProvider.AtolSample.DataModel.AtolTask;
        using Contoso.CommerceRuntime.DocumentProvider.AtolSample.Messages;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.FiscalIntegration.DocumentProvider.Messages;
        using Microsoft.Dynamics.Commerce.Runtime.Handlers;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;
        using Microsoft.Dynamics.Retail.Diagnostics;
        using Newtonsoft.Json;

        /// <summary>
        /// Document provider service for Atol fiscal printer.
        /// </summary>
        public class DocumentProviderAtolService : INamedRequestHandlerAsync
        {
            private const string ServiceName = "Atol";

            private enum Events
            {
                FiscalRegistrationResultResponseDeserializationFailed,
            }

            /// <summary>
            /// Gets handler name.
            /// </summary>
            public string HandlerName => "AtolSample";

            /// <summary>
            /// Gets the list of the support fiscal events.
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
            /// Gets supported requests.
            /// </summary>
            IEnumerable<Type> IRequestHandlerAsync.SupportedRequestTypes => new[]
            {
                typeof(GetFiscalDocumentDocumentProviderRequest),
                typeof(GetSupportedRegistrableEventsDocumentProviderRequest),
                typeof(GetFiscalTransactionExtendedDataDocumentProviderRequest),
            };

            /// <summary>
            /// Represents an enty point for the request handler.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <returns>The response.</returns>
            public async Task<Response> Execute(Request request)
            {
                ThrowIf.Null(request, nameof(request));

                switch (request)
                {
                    case GetFiscalDocumentDocumentProviderRequest getFiscalDocumentDocumentProviderRequest:
                        return await this.GetFiscalDocumentAsync(getFiscalDocumentDocumentProviderRequest).ConfigureAwait(false);

                    case GetSupportedRegistrableEventsDocumentProviderRequest getSupportedRegistrableEventsDocumentProviderRequest:
                        return this.GetSupportedRegisterableEvents(getSupportedRegistrableEventsDocumentProviderRequest);

                    case GetFiscalTransactionExtendedDataDocumentProviderRequest getFiscalTransactionExtendedDataDocumentProviderRequest:
                        return this.GetFiscalTransactionExtendedData(getFiscalTransactionExtendedDataDocumentProviderRequest);

                    default:
                        throw new NotSupportedException(string.Format("Request '{0}' is not supported.", request.GetType()));
                }
            }

            /// <summary>
            /// Gets the fiscal document document provider response.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <returns>The fiscal document document provider response.</returns>
            private async Task<GetFiscalDocumentDocumentProviderResponse> GetFiscalDocumentAsync(GetFiscalDocumentDocumentProviderRequest request)
            {
                FiscalIntegrationEventType eventType = request.FiscalDocumentRetrievalCriteria.FiscalRegistrationEventType;
                switch (eventType)
                {
                    case FiscalIntegrationEventType.Sale:
                        var getSalesOrderDocumentReceiptAtolResponse = await request.RequestContext.ExecuteAsync<GetSalesOrderDocumentReceiptAtolResponse>(new GetSalesOrderDocumentReceiptAtolRequest(request.FiscalIntegrationFunctionalityProfile, request.SalesOrder)).ConfigureAwait(false);
                        return new GetFiscalDocumentDocumentProviderResponse(getSalesOrderDocumentReceiptAtolResponse.FiscalIntegrationDocument);
                    case FiscalIntegrationEventType.FiscalXReport:
                        var configuredTaskAwaitable = (await request.RequestContext.ExecuteAsync<GetReportXTaskDocumentProviderAtolResponse>(new GetReportXTaskDocumentProviderAtolRequest()).ConfigureAwait(false)).FiscalIntegrationDocument;
                        return new GetFiscalDocumentDocumentProviderResponse(configuredTaskAwaitable);
                    case FiscalIntegrationEventType.FiscalZReport:
                        GetCloseShiftTaskDocumentProviderAtolResponse getCloseShiftTaskDocumentProviderAtolResponse = await request.RequestContext.ExecuteAsync<GetCloseShiftTaskDocumentProviderAtolResponse>(new GetCloseShiftTaskDocumentProviderAtolRequest(request.FiscalIntegrationFunctionalityProfile)).ConfigureAwait(false);
                        return new GetFiscalDocumentDocumentProviderResponse(getCloseShiftTaskDocumentProviderAtolResponse.FiscalIntegrationDocument);
                }

                return null;
            }

            /// <summary>
            /// Gets the supported registerable events.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <returns>The supported registerable events.</returns>
            private GetSupportedRegistrableEventsDocumentProviderResponse GetSupportedRegisterableEvents(GetSupportedRegistrableEventsDocumentProviderRequest request)
            {
                ICollection<int> supportedFiscalEventTypes = new List<int>(this.SupportedRegistrableEventsId);
                GetSupportedRegistrableEventsDocumentProviderResponse response = new GetSupportedRegistrableEventsDocumentProviderResponse(
                        supportedFiscalEventTypes,
                        new List<int>());

                return response;
            }

            /// <summary>
            /// Gets the fiscal transaction extended data.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <returns>The the fiscal transaction extended data.</returns>
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
                        new CommerceProperty(ExtensibleFiscalRegistrationExtendedDataType.TransactionEnd.Name, salesTransactionResponse.FiscalParameters?.FiscalDocumentDateTime),
                        new CommerceProperty(ExtensibleFiscalRegistrationExtendedDataType.Signature.Name, salesTransactionResponse.FiscalParameters?.FiscalDocumentSignature),
                        new CommerceProperty(ExtensibleFiscalRegistrationExtendedDataType.RegistrationNumber.Name, salesTransactionResponse.FiscalParameters?.RegistrationNumber),
                        new CommerceProperty(ExtensibleFiscalRegistrationExtendedDataType.DeviceSerialNumber.Name, salesTransactionResponse.FiscalParameters?.FnNumber),
                        new CommerceProperty(ExtensibleFiscalRegistrationExtendedDataType.ServiceTransactionId.Name, salesTransactionResponse.FiscalParameters?.FiscalReceiptNumber),
                    };
                    extendedData.AddRange(commerceProperties);
                    documentNumber = salesTransactionResponse.FiscalParameters?.FiscalDocumentNumber.ToString();
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
            /// Deserializes JSON string into AtolTransactionRegistrationResponse object.
            /// </summary>
            /// <param name="source">The JSON string.</param>
            /// <param name="result">The deserialized object.</param>
            /// <returns>True if deserialized successfully; false otherwise.</returns>
            private bool DeserializeJson(string source, out AtolTransactionRegistrationResponse result)
            {
                try
                {
                    result = JsonConvert.DeserializeObject<AtolTransactionRegistrationResponse>(source);
                    return result != null;
                }
                catch (JsonException ex)
                {
                    result = default(AtolTransactionRegistrationResponse);
                    RetailLogger.Log.LogDebug(Events.FiscalRegistrationResultResponseDeserializationFailed, ex, "Deserialization of fiscal registration response has failed.");

                    return false;
                }
            }
        }
    }
}
