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
    namespace CommerceRuntime.DocumentProvider.EFRSample
    {
        using System;
        using System.Collections.Generic;
        using System.Linq;
        using System.Threading.Tasks;
        using Contoso.CommerceRuntime.DocumentProvider.DataModelEFR.Documents;
        using Contoso.CommerceRuntime.DocumentProvider.EFRSample.DocumentBuilders;
        using Contoso.CommerceRuntime.DocumentProvider.EFRSample.DocumentBuilders.Parameters;
        using Contoso.CommerceRuntime.DocumentProvider.EFRSample.Extensions;
        using Contoso.CommerceRuntime.DocumentProvider.EFRSample.Serializers;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.FiscalIntegration.DocumentProvider.Messages;
        using Microsoft.Dynamics.Commerce.Runtime.Handlers;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;
        using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;

        /// <summary>
        /// The document provider for EFSTA (European Fiscal Standards Association) Fiscal Register (version 1.8.3) specific for Austria.
        /// </summary>
        public class DocumentProviderEFRFiscalCZ : INamedRequestHandlerAsync
        {
            private const string ServiceName = "EFR";
            private const string SignatureElementName = "Sign";
            private const string SecurityCodeElementName = "Sec";
            private const string InfoElementName = "Info";

            /// <summary>
            /// Gets the unique name for this request handler.
            /// </summary>
            public string HandlerName => "FiscalEFRSampleCZE";

            /// <summary>
            /// Gets the supported fiscal integration event type.
            /// </summary>
            public IEnumerable<int> SupportedRegistrableFiscalEventsId => new[]
            {
                (int)FiscalIntegrationEventType.Sale,
                (int)FiscalIntegrationEventType.CreateCustomerOrder,
                (int)FiscalIntegrationEventType.EditCustomerOrder,
                (int)FiscalIntegrationEventType.CancelCustomerOrder,
                (int)FiscalIntegrationEventType.CustomerAccountDeposit,
            };

            /// <summary>
            /// Gets the supported non-fiscal integration event type.
            /// </summary>
            public IEnumerable<int> SupportedRegistrableNonFiscalEventsId => Enumerable.Empty<int>();

            /// <summary>
            /// Gets the collection of supported request types by this handler.
            /// </summary>
            public IEnumerable<Type> SupportedRequestTypes => new[]
            {
                typeof(GetFiscalDocumentDocumentProviderRequest),
                typeof(GetSupportedRegistrableEventsDocumentProviderRequest),
                typeof(GetFiscalRegisterResponseToSaveDocumentProviderRequest),
                typeof(GetFiscalTransactionExtendedDataDocumentProviderRequest),
            };

            /// <summary>
            /// Executes the specified request using the specified request context and handler.
            /// </summary>
            /// <param name="request">The request to execute.</param>
            /// <returns>The response of the request from the request handler.</returns>
            public async Task<Response> Execute(Request request)
            {
                ThrowIf.Null(request, nameof(request));

                switch (request)
                {
                    case GetSupportedRegistrableEventsDocumentProviderRequest _:
                        return await GetSupportedRegistrableEventsAsync().ConfigureAwait(false);

                    case GetFiscalDocumentDocumentProviderRequest getFiscalDocumentDocumentProviderRequest:
                        return await GetFiscalDocumentResponseAsync(getFiscalDocumentDocumentProviderRequest).ConfigureAwait(false);

                    case GetFiscalRegisterResponseToSaveDocumentProviderRequest getFiscalRegisterResponseToSaveDocumentProviderRequest:
                        return await GetFiscalRegisterResponseToSaveAsync(getFiscalRegisterResponseToSaveDocumentProviderRequest).ConfigureAwait(false);

                    case GetFiscalTransactionExtendedDataDocumentProviderRequest getFiscalTransactionExtendedDataDocumentProviderRequest:
                        return await Task.FromResult<Response>(GetFiscalTransactionExtendedData(getFiscalTransactionExtendedDataDocumentProviderRequest)).ConfigureAwait(false);

                    default:
                        throw new NotSupportedException(string.Format("Request '{0}' is not supported.", request.GetType()));
                }
            }

            /// <summary>
            /// Gets the fiscal registration result.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <returns>The response.</returns>
            private static Task<Response> GetFiscalRegisterResponseToSaveAsync(GetFiscalRegisterResponseToSaveDocumentProviderRequest request)
            {
                Response response = null;

                if ((request.FiscalRegistrationResult.RegistrationStatus != FiscalIntegrationRegistrationStatus.Completed && string.IsNullOrWhiteSpace(request.FiscalRegistrationResult.Response))
                    || string.IsNullOrWhiteSpace(request.FiscalRegistrationResult.TransactionID))
                {
                    response = new GetFiscalRegisterResponseToSaveDocumentProviderResponse(string.Empty);
                }
                else
                {
                    response = new GetFiscalRegisterResponseToSaveDocumentProviderResponse(request.FiscalRegistrationResult.GetFiscalResponseStringCZ());
                }

                return Task.FromResult(response);
            }

            /// <summary>
            /// Gets the fiscal document document provider response.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <returns>The fiscal document document provider response.</returns>
            private static async Task<Response> GetFiscalDocumentResponseAsync(GetFiscalDocumentDocumentProviderRequest request)
            {
                var documentBuilderData = DocumentBuilderData.FromRequest(request);
                GetAdjustedSalesOrderFiscalIntegrationServiceResponse adjustedSalesOrderResponse = null;
                IDocumentBuilder builder = null;

                switch (request.FiscalDocumentRetrievalCriteria.FiscalRegistrationEventType)
                {
                    case FiscalIntegrationEventType.CreateCustomerOrder:
                    case FiscalIntegrationEventType.EditCustomerOrder:
                    case FiscalIntegrationEventType.CancelCustomerOrder:
                    case FiscalIntegrationEventType.Sale:
                        FiscalIntegrationSalesOrderAdjustmentType adjustmentType = GetSalesOrderAdjustmentType(request.SalesOrder);
                        SalesOrder adjustedSalesOrder = null;

                        if (adjustmentType != FiscalIntegrationSalesOrderAdjustmentType.None)
                        {
                            adjustedSalesOrderResponse = await request.SalesOrder.GetAdjustedSalesOrderAsync(request.RequestContext, adjustmentType).ConfigureAwait(false);
                        }

                        adjustedSalesOrder = adjustedSalesOrderResponse == null ? request.SalesOrder : adjustedSalesOrderResponse.SalesOrder;

                        if (adjustedSalesOrder.ActiveSalesLines.Any())
                        {
                            documentBuilderData.SalesOrder = adjustedSalesOrder;
                            builder = new SalesTransactionBuilder(documentBuilderData);
                        }

                        break;
                    case FiscalIntegrationEventType.CustomerAccountDeposit:
                        builder = new CustomerAccountDepositTransactionBuilder(documentBuilderData);
                        break;

                    case FiscalIntegrationEventType.CloseShift:
                        builder = new ZReportBuilder(documentBuilderData);
                        break;

                    default:
                        throw new NotSupportedException(string.Format("Fiscal integration event type '{0}' is not supported.", request.FiscalDocumentRetrievalCriteria.FiscalRegistrationEventType));
                }

                IFiscalIntegrationDocument document = null;
                if (builder != null)
                {
                    document = await builder.BuildAsync().ConfigureAwait(false);
                }

                FiscalIntegrationDocument fiscalIntegrationDocument = document != null ?
                    new FiscalIntegrationDocument(FiscalDocumentSerializer.Serialize(document), FiscalIntegrationDocumentGenerationResultType.Succeeded) :
                    new FiscalIntegrationDocument(document: string.Empty,
                                                  resultType: FiscalIntegrationDocumentGenerationResultType.NotRequired);

                return new GetFiscalDocumentDocumentProviderResponse(fiscalIntegrationDocument);
            }

            /// <summary>
            /// Gets the fiscal transaction extended data.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <returns>The fiscal transaction extended data.</returns>
            private static GetFiscalTransactionExtendedDataDocumentProviderResponse GetFiscalTransactionExtendedData(GetFiscalTransactionExtendedDataDocumentProviderRequest request)
            {
                var extendedData = new List<CommerceProperty>();
                string documentNumber = string.Empty;

                // If the response string cannot be parsed, extendedData and documentNumber will be empty and no exception will be thrown.
                if (XmlSerializer<SalesTransactionRegistrationResponse>.TryDeserialize(request.FiscalRegistrationResult?.Response, out var salesTransactionResponse))
                {
                    string GetFiscalTagFieldValue(string elementName) => salesTransactionResponse.FiscalData?.FiscalTags?.SingleOrDefault(ft => ft.FieldName == elementName)?.FieldValue;

                    CommerceProperty[] commerceProperties = new[]
                    {
                        new CommerceProperty(ExtensibleFiscalRegistrationExtendedDataType.ServiceTransactionId.Name, salesTransactionResponse.Receipt?.TransactionNumber),
                        new CommerceProperty(ExtensibleFiscalRegistrationExtendedDataType.TransactionEnd.Name, salesTransactionResponse.Receipt?.ReceiptDateTimeStringValue),
                        new CommerceProperty(ExtensibleFiscalRegistrationExtendedDataType.Signature.Name, GetFiscalTagFieldValue(SignatureElementName)),
                        new CommerceProperty(ExtensibleFiscalRegistrationExtendedDataType.SequenceNumber.Name, salesTransactionResponse.SequenceNumber),
                        new CommerceProperty(ExtensibleFiscalRegistrationExtendedDataType.Info.Name, GetFiscalTagFieldValue(InfoElementName)),
                        new CommerceProperty(ExtensibleFiscalRegistrationExtendedDataType.ServiceSecurityCode.Name, GetFiscalTagFieldValue(SecurityCodeElementName)),
                    };
                    extendedData.AddRange(commerceProperties);
                    documentNumber = salesTransactionResponse.Receipt?.TransactionNumber;
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
            /// Gets sales order adjustment type.
            /// </summary>
            /// <param name="salesOrder">The sales order.</param>
            /// <returns>The sales order adjustment type.</returns>
            private static FiscalIntegrationSalesOrderAdjustmentType GetSalesOrderAdjustmentType(SalesOrder salesOrder)
            {
                return FiscalIntegrationSalesOrderAdjustmentType.None;
            }

            /// <summary>
            /// Gets the supported registerable events document provider response.
            /// </summary>
            /// <returns>The supported registerable events document provider response.</returns>
            private Task<Response> GetSupportedRegistrableEventsAsync()
            {
                var response = new GetSupportedRegistrableEventsDocumentProviderResponse(this.SupportedRegistrableFiscalEventsId.ToList(), this.SupportedRegistrableNonFiscalEventsId.ToList());
                return Task.FromResult<Response>(response);
            }
        }
    }
}
