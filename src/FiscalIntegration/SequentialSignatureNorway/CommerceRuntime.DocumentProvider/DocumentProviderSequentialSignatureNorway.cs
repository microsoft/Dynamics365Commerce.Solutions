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
    namespace CommerceRuntime.DocumentProvider.SequentialSignNorway
    {
        using System;
        using System.Collections.Generic;
        using System.Linq;
        using System.Threading.Tasks;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.FiscalIntegration.DocumentProvider.Messages;
        using Microsoft.Dynamics.Commerce.Runtime.Handlers;
        using Microsoft.Dynamics.Commerce.Runtime.Localization.Entities.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;
        using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;
        using Newtonsoft.Json;

        /// <summary>
        /// Sequential signature document provider.
        /// </summary>
        public class DocumentProviderSequentialSignatureNorway : INamedRequestHandlerAsync
        {
            /// <summary>
            /// Gets the unique name for this request handler.
            /// </summary>
            public string HandlerName => "DocumentProviderSequentialSignatureNorwaySample";

            /// <summary>
            /// Gets the supported fiscal integration event type.
            /// </summary>
            public IEnumerable<int> SupportedRegistrableEventIds => new[]
            {
                (int)FiscalIntegrationEventType.Sale,
                (int)FiscalIntegrationEventType.CreateCustomerOrder,
                (int)FiscalIntegrationEventType.EditCustomerOrder,
            };

            /// <summary>
            /// Gets the collection of supported request types by this handler.
            /// </summary>
            public IEnumerable<Type> SupportedRequestTypes => new[]
            {
                typeof(GetFiscalDocumentDocumentProviderRequest),
                typeof(GetSupportedRegistrableEventsDocumentProviderRequest),
                typeof(GetFiscalIntegrationSequentialKeysDocumentProviderRequest),
                typeof(GetFiscalRegisterResponseToSaveDocumentProviderRequest),
                typeof(GetFiscalTransactionExtendedDataDocumentProviderRequest),
            };

            /// <summary>
            /// Executes the specified request using the specified request context and handler.
            /// </summary>
            /// <param name="request">The request to execute.</param>
            /// <returns>The response of the request from the request handler.</returns>
            public Task<Response> Execute(Request request)
            {
                ThrowIf.Null(request, nameof(request));

                switch (request)
                {
                    case GetFiscalDocumentDocumentProviderRequest getFiscalDocumentDocumentProviderRequest:
                        return this.GetFiscalDocument(getFiscalDocumentDocumentProviderRequest);
                    case GetSupportedRegistrableEventsDocumentProviderRequest _:
                        return Task.FromResult(this.GetSupportedRegisterableEvents());
                    case GetFiscalIntegrationSequentialKeysDocumentProviderRequest _:
                        return Task.FromResult(this.GetFiscalIntegrationSequentialKeys());
                    case GetFiscalRegisterResponseToSaveDocumentProviderRequest getFiscalRegisterResponseToSaveDocumentProviderRequest:
                        return Task.FromResult(this.GetFiscalRegisterResponseToSave(getFiscalRegisterResponseToSaveDocumentProviderRequest));
                    case GetFiscalTransactionExtendedDataDocumentProviderRequest extendedDataRequest:
                        return this.GetFiscalTransactionExtendedData(extendedDataRequest);
                    default:
                        throw new NotSupportedException(string.Format("Request '{0}' is not supported.", request.GetType()));
                }
            }

            /// <summary>
            /// Gets the fiscal document.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <returns>The fiscal document provider response.</returns>
            private async Task<Response> GetFiscalDocument(GetFiscalDocumentDocumentProviderRequest request)
            {
                ThrowIf.Null(request.FiscalDocumentRetrievalCriteria, nameof(request.FiscalDocumentRetrievalCriteria));

                var eventType = request.FiscalDocumentRetrievalCriteria.FiscalRegistrationEventType;
                var fiscalIntegrationDocument = new FiscalIntegrationDocument(document: string.Empty, resultType: FiscalIntegrationDocumentGenerationResultType.NotRequired);

                switch (eventType)
                {
                    case FiscalIntegrationEventType.Sale:
                    case FiscalIntegrationEventType.CreateCustomerOrder:
                    case FiscalIntegrationEventType.EditCustomerOrder:
                        if (request.SalesOrder.ActiveSalesLines.Any())
                        {
                            fiscalIntegrationDocument = await this.GetSalesOrderFiscalDocument(request).ConfigureAwait(false);
                        }

                        break;

                    default:
                        throw new NotSupportedException(string.Format("Fiscal integration event type '{0}' is not supported.", eventType));
                }

                return new GetFiscalDocumentDocumentProviderResponse(fiscalIntegrationDocument);
            }

            /// <summary>
            /// Gets the response from fiscal service to save.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <returns>The response.</returns>
            private Response GetFiscalRegisterResponseToSave(GetFiscalRegisterResponseToSaveDocumentProviderRequest request)
            {
                ThrowIf.Null(request.FiscalRegistrationResult, nameof(request.FiscalRegistrationResult));

                if (request.FiscalRegistrationResult.RegistrationStatus != FiscalIntegrationRegistrationStatus.Completed &&
                    string.IsNullOrWhiteSpace(request.FiscalRegistrationResult.Response))
                {
                    return new GetFiscalRegisterResponseToSaveDocumentProviderResponse(string.Empty);
                }

                return new GetFiscalRegisterResponseToSaveDocumentProviderResponse(this.GetFiscalResponseString(request.FiscalRegistrationResult));
            }

            /// <summary>
            /// Gets the fiscal response string.
            /// </summary>
            /// <param name="fiscalRegistrationResult">The fiscal registration result.</param>
            /// <returns>The fiscal response string.</returns>
            private string GetFiscalResponseString(FiscalIntegrationRegistrationResult fiscalRegistrationResult)
            {
                var signatureData = JsonConvert.DeserializeObject<SequentialSignatureRegistrationResult>(fiscalRegistrationResult.Response);

                var fiscalServiceResponse = new FiscalRegisterResult()
                {
                    SequentialNumber = fiscalRegistrationResult.SequentialSignatureData?.SequentialNumber ?? 0,
                    Signature = signatureData.Signature,
                    KeyThumbprint = signatureData.CertificateThumbprint,
                    DataToSign = fiscalRegistrationResult.SubmittedDocument,
                };

                return JsonConvert.SerializeObject(fiscalServiceResponse);
            }

            /// <summary>
            /// Gets the fiscal transaction extended data.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <returns>The response.</returns>
            private async Task<Response> GetFiscalTransactionExtendedData(GetFiscalTransactionExtendedDataDocumentProviderRequest request)
            {
                var signatureData = JsonConvert.DeserializeObject<SequentialSignatureRegistrationResult>(request.FiscalRegistrationResult.Response);
                var salesOrderRequest = new GetSalesOrderDetailsByTransactionIdServiceRequest(request.FiscalRegistrationResult.TransactionID, SearchLocation.Local);
                var salesOrderResponse = await request.RequestContext.ExecuteAsync<GetSalesOrderDetailsServiceResponse>(salesOrderRequest).ConfigureAwait(false);

                var extendedData = new List<CommerceProperty>
                {
                    new CommerceProperty(ExtensibleFiscalRegistrationExtendedDataType.SequenceNumber.Name, request.FiscalRegistrationResult.SequentialSignatureData?.SequentialNumber.ToString()),
                    new CommerceProperty(ExtensibleFiscalRegistrationExtendedDataType.Signature.Name, signatureData?.Signature),
                    new CommerceProperty(ExtensibleFiscalRegistrationExtendedDataType.CertificateThumbprint.Name, signatureData?.CertificateThumbprint),
                    new CommerceProperty(ExtensibleFiscalRegistrationExtendedDataType.HashAlgorithm.Name, signatureData?.HashAlgorithm),
                };

                if (salesOrderResponse.SalesOrder != null)
                {
                    extendedData.AddRange(
                        new[]
                        {
                            new CommerceProperty(ExtensibleFiscalRegistrationExtendedDataType.ServiceTransactionType.Name, salesOrderResponse.SalesOrder.ExtensibleSalesTransactionType.Name),
                            new CommerceProperty(ExtensibleFiscalRegistrationExtendedDataType.LineCount.Name, salesOrderResponse.SalesOrder.ActiveSalesLines.Count.ToString()),
                        });
                }

                return new GetFiscalTransactionExtendedDataDocumentProviderResponse(
                    documentNumber: string.Empty,
                    registrationType: ExtensibleFiscalRegistrationType.CashSale,
                    serviceName: string.Empty,
                    countryRegionIsoCode: request.RequestContext.GetPrincipal().CountryRegionIsoCode,
                    extendedData: extendedData);
            }

            /// <summary>
            /// Gets a sales order fiscal document.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <returns>The fiscal document.</returns>
            private async Task<FiscalIntegrationDocument> GetSalesOrderFiscalDocument(GetFiscalDocumentDocumentProviderRequest request)
            {
                var fiscalIntegrationDocument = new FiscalIntegrationDocument(document: string.Empty, resultType: FiscalIntegrationDocumentGenerationResultType.NotRequired);
                var salesOrderAdjustmentResponse = await this.AdjustSalesOrderAsync(request.SalesOrder, request.RequestContext).ConfigureAwait(false);

                var salesOrder = salesOrderAdjustmentResponse == null ? request.SalesOrder : salesOrderAdjustmentResponse.SalesOrder;
                fiscalIntegrationDocument.DocumentAdjustment = salesOrderAdjustmentResponse?.DocumentAdjustment;

                if (!salesOrder.ActiveSalesLines.Any())
                {
                    return fiscalIntegrationDocument;
                }

                var documentBuilder = new DocumentBuilder(salesOrder, request.FiscalDocumentRetrievalCriteria.DocumentContext);
                fiscalIntegrationDocument.Document = await documentBuilder.GetDataToRegister().ConfigureAwait(false);
                fiscalIntegrationDocument.DocumentGenerationResultType = FiscalIntegrationDocumentGenerationResultType.Succeeded;
                fiscalIntegrationDocument.SequentialSignatureDataContext = new FiscalIntegrationSignatureDataContext()
                {
                    SequentialNumber = documentBuilder.SequentialNumber,
                    SequentialSignatureKey = DocumentBuilder.SequenceKey,
                };

                return fiscalIntegrationDocument;
            }

            /// <summary>
            /// Gets the supported registerable events document provider response.
            /// </summary>
            /// <returns>The supported registerable events document provider response.</returns>
            private Response GetSupportedRegisterableEvents() =>
                new GetSupportedRegistrableEventsDocumentProviderResponse(this.SupportedRegistrableEventIds.ToList(), new List<int>());

            /// <summary>
            /// Gets the fiscal integration sequential signature keys document provider response.
            /// </summary>
            /// <returns>The supported registerable events document provider response.</returns>
            private Response GetFiscalIntegrationSequentialKeys() => new GetFiscalIntegrationSequentialKeysDocumentProviderResponse(
                        new HashSet<string>(new string[] { DocumentBuilder.SequenceKey }));

            /// <summary>
            /// Gets adjusted sales order.
            /// </summary>
            /// <param name="salesOrder">The sales order.</param>
            /// <param name="requestContext">The request context.</param>
            /// <returns>The sales order adjustment response.</returns>
            private async Task<GetAdjustedSalesOrderFiscalIntegrationServiceResponse> AdjustSalesOrderAsync(SalesOrder salesOrder, RequestContext requestContext)
            {
                ThrowIf.Null(requestContext, nameof(requestContext));
                ThrowIf.Null(salesOrder, nameof(salesOrder));

                FiscalIntegrationSalesOrderAdjustmentType adjustmentType = FiscalIntegrationSalesOrderAdjustmentType.None;

                if (salesOrder.ExtensibleSalesTransactionType == ExtensibleSalesTransactionType.Sales)
                {
                    adjustmentType = FiscalIntegrationSalesOrderAdjustmentType.ExcludeGiftCards;
                }
                else if (salesOrder.ExtensibleSalesTransactionType == ExtensibleSalesTransactionType.CustomerOrder ||
                    salesOrder.ExtensibleSalesTransactionType == ExtensibleSalesTransactionType.AsyncCustomerOrder)
                {
                    adjustmentType = FiscalIntegrationSalesOrderAdjustmentType.ExcludeDeposit;
                }

                if (adjustmentType != FiscalIntegrationSalesOrderAdjustmentType.None)
                {
                    var getAdjustedSalesOrderRequest = new GetAdjustedSalesOrderFiscalIntegrationServiceRequest(salesOrder, adjustmentType);
                    return await requestContext.ExecuteAsync<GetAdjustedSalesOrderFiscalIntegrationServiceResponse>(getAdjustedSalesOrderRequest).ConfigureAwait(false);
                }

                return null;
            }
        }
    }
}
