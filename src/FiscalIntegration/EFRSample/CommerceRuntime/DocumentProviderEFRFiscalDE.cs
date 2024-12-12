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
        using Contoso.CommerceRuntime.DocumentProvider.EFRSample.DocumentBuilders.GermanyBuilders;
        using Contoso.CommerceRuntime.DocumentProvider.EFRSample.DocumentBuilders.Parameters;
        using Contoso.CommerceRuntime.DocumentProvider.EFRSample.Serializers;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.FiscalIntegration.DocumentProvider.Messages;
        using Microsoft.Dynamics.Commerce.Runtime.Handlers;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;
        using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;

        /// <summary>
        /// The document provider for EFSTA (European Fiscal Standards Association) Fiscal Register (version 1.8.3) specific for Germany.
        /// </summary>
        public class DocumentProviderEFRFiscalDE : INamedRequestHandlerAsync
        {
            private const string ServiceName = "EFR";
            private const string StartDateTimeElementName = "StartD";
            private const string FinishDateTimeElementName = "FinishD";
            private const string SecurityCodeElementName = "Serial";
            private const string SequenceNumberElementName = "SignCnt";
            private const string SignatureElementName = "Sign";
            private const string InfoElementName = "Info";

            /// <summary>
            /// Gets the unique name for this request handler.
            /// </summary>
            public string HandlerName => "FiscalEFRSampleDEU";

            /// <summary>
            /// Gets the supported fiscal integration event type.
            /// </summary>
            public IEnumerable<int> SupportedRegistrableFiscalEventsId => new[]
            {
                (int)FiscalIntegrationEventType.BeginSale,
                (int)FiscalIntegrationEventType.Sale,
                (int)FiscalIntegrationEventType.CustomerAccountDeposit,
                (int)FiscalIntegrationEventType.CreateCustomerOrder,
                (int)FiscalIntegrationEventType.EditCustomerOrder,
                (int)FiscalIntegrationEventType.CancelCustomerOrder,
                (int)FiscalIntegrationEventType.VoidTransaction,
                (int)FiscalIntegrationEventType.SuspendTransaction,
                (int)FiscalIntegrationEventType.FloatEntry,
                (int)FiscalIntegrationEventType.SafeDrop,
                (int)FiscalIntegrationEventType.BankDrop,
                (int)FiscalIntegrationEventType.RemoveTender,
                (int)FiscalIntegrationEventType.StartingAmount,
                (int)FiscalIntegrationEventType.IncomeAccounts,
                (int)FiscalIntegrationEventType.ExpenseAccounts,
                (int)FiscalIntegrationEventType.CloseShift,
                (int)FiscalIntegrationEventType.RecallTransaction,
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
            public Task<Response> Execute(Request request)
            {
                ThrowIf.Null(request, nameof(request));

                switch (request)
                {
                    case GetSupportedRegistrableEventsDocumentProviderRequest _:
                        return GetSupportedRegistrableEventsAsync();

                    case GetFiscalDocumentDocumentProviderRequest getFiscalDocumentDocumentProviderRequest:
                        return GetFiscalDocumentResponseAsync(getFiscalDocumentDocumentProviderRequest);

                    case GetFiscalTransactionExtendedDataDocumentProviderRequest getFiscalTransactionExtendedDataDocumentProviderRequest:
                        return Task.FromResult<Response>(GetFiscalTransactionExtendedData(getFiscalTransactionExtendedDataDocumentProviderRequest));

                    case GetFiscalRegisterResponseToSaveDocumentProviderRequest getFiscalRegisterResponseToSaveDocumentProviderRequest:
                        return GetFiscalRegisterResponseToSaveAsync(getFiscalRegisterResponseToSaveDocumentProviderRequest);

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
                var response = new GetFiscalRegisterResponseToSaveDocumentProviderResponse(request.FiscalRegistrationResult.Response);
                return Task.FromResult<Response>(response);
            }

            /// <summary>
            /// Gets the GetFiscalTransactionExtendedDataDocumentProviderResponse.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <returns>The GetFiscalTransactionExtendedDataDocumentProviderResponse.</returns>
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
                        new CommerceProperty(ExtensibleFiscalRegistrationExtendedDataType.QRCode.Name, salesTransactionResponse.FiscalData?.FiscalQRCode),
                        new CommerceProperty(ExtensibleFiscalRegistrationExtendedDataType.ServiceTransactionId.Name, salesTransactionResponse.FiscalData?.TransactionId),
                        new CommerceProperty(ExtensibleFiscalRegistrationExtendedDataType.ServiceSecurityCode.Name, GetFiscalTagFieldValue(SecurityCodeElementName)),
                        new CommerceProperty(ExtensibleFiscalRegistrationExtendedDataType.TransactionStart.Name, GetFiscalTagFieldValue(StartDateTimeElementName)),
                        new CommerceProperty(ExtensibleFiscalRegistrationExtendedDataType.TransactionEnd.Name, GetFiscalTagFieldValue(FinishDateTimeElementName)),
                        new CommerceProperty(ExtensibleFiscalRegistrationExtendedDataType.SequenceNumber.Name, GetFiscalTagFieldValue(SequenceNumberElementName)),
                        new CommerceProperty(ExtensibleFiscalRegistrationExtendedDataType.Signature.Name, GetFiscalTagFieldValue(SignatureElementName)),
                        new CommerceProperty(ExtensibleFiscalRegistrationExtendedDataType.Info.Name, GetFiscalTagFieldValue(InfoElementName)),
                    };
                    extendedData.AddRange(commerceProperties);
                    documentNumber = salesTransactionResponse.FiscalData?.TransactionId;
                }
                else if (XmlSerializer<BeginSaleRegistrationResponse>.TryDeserialize(request.FiscalRegistrationResult?.Response, out var beginSaleRegistrationResponse))
                {
                    documentNumber = beginSaleRegistrationResponse.FiscalData?.TransactionId;
                    extendedData.Add(new CommerceProperty(ExtensibleFiscalRegistrationExtendedDataType.TransactionStart.Name, beginSaleRegistrationResponse.FiscalData?.StartDateTime));
                }

                var noneRegistrationTypes = new[] { FiscalIntegrationEventType.BeginSale, FiscalIntegrationEventType.RecallTransaction };
                var registrationType = noneRegistrationTypes.Contains(request.FiscalRegistrationResult.FiscalRegistrationEventType)
                    ? ExtensibleFiscalRegistrationType.None
                    : ExtensibleFiscalRegistrationType.CashSale;

                return new GetFiscalTransactionExtendedDataDocumentProviderResponse(
                    documentNumber ?? string.Empty,
                    registrationType,
                    ServiceName,
                    request.RequestContext.GetPrincipal().CountryRegionIsoCode,
                    extendedData);
            }

            /// <summary>
            /// Gets the fiscal document document provider response.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <returns>The fiscal document document provider response.</returns>
            private static async Task<Response> GetFiscalDocumentResponseAsync(GetFiscalDocumentDocumentProviderRequest request)
            {
                var documentBuilderData = DocumentBuilderData.FromRequest(request);
                IDocumentBuilder builder = null;

                switch (request.FiscalDocumentRetrievalCriteria.FiscalRegistrationEventType)
                {
                    case FiscalIntegrationEventType.CreateCustomerOrder:
                    case FiscalIntegrationEventType.EditCustomerOrder:
                    case FiscalIntegrationEventType.CancelCustomerOrder:
                    case FiscalIntegrationEventType.Sale:
                        if (request.SalesOrder.ActiveSalesLines.Any())
                        {
                            builder = new SalesTransactionBuilder(documentBuilderData);
                        }

                        break;

                    case FiscalIntegrationEventType.CustomerAccountDeposit:
                        builder = new CustomerAccountDepositTransactionBuilder(documentBuilderData);
                        break;

                    case FiscalIntegrationEventType.BeginSale:
                        builder = await BeginSaleBuilder.Create(documentBuilderData).ConfigureAwait(false);
                        break;

                    case FiscalIntegrationEventType.CloseShift:
                        builder = new ZReportBuilder(documentBuilderData);
                        break;

                    case FiscalIntegrationEventType.FloatEntry:
                    case FiscalIntegrationEventType.RemoveTender:
                    case FiscalIntegrationEventType.StartingAmount:
                        builder = new NonSalesTransactionBuilder(documentBuilderData);
                        break;

                    case FiscalIntegrationEventType.BankDrop:
                    case FiscalIntegrationEventType.SafeDrop:
                        builder = await BankSafeDropBuilder.Create(documentBuilderData).ConfigureAwait(false);
                        break;

                    case FiscalIntegrationEventType.IncomeAccounts:
                        builder = await IncomeAccountsBuilder.Create(documentBuilderData).ConfigureAwait(false);
                        break;

                    case FiscalIntegrationEventType.ExpenseAccounts:
                        builder = await ExpenseAccountsBuilder.Create(documentBuilderData).ConfigureAwait(false);
                        break;

                    case FiscalIntegrationEventType.VoidTransaction:
                    case FiscalIntegrationEventType.SuspendTransaction:
                        builder = await GetVoidTransactionDocumentBuilderAsync(documentBuilderData).ConfigureAwait(false);
                        break;

                    case FiscalIntegrationEventType.RecallTransaction:
                        builder = await GetFiscalDocumentBuilderForRecallEventAsync(documentBuilderData).ConfigureAwait(false);
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
            /// Gets the fiscal integration document builder for void or suspend event types.
            /// </summary>
            /// <param name="documentBuilderData">The document builder data.</param>
            /// <returns>The fiscal integration receipt document builder.</returns>
            private static async Task<IDocumentBuilder> GetVoidTransactionDocumentBuilderAsync(DocumentBuilderData documentBuilderData)
            {
                var salesOrder = await GetSalesOrderAsync(documentBuilderData.RequestContext, documentBuilderData.FiscalDocumentRetrievalCriteria.IsRemoteTransaction, documentBuilderData.FiscalDocumentRetrievalCriteria.TransactionId).ConfigureAwait(false);

                bool shouldSkipRegister = salesOrder.ExtensibleSalesTransactionType == ExtensibleSalesTransactionType.IncomeExpense ||
                    salesOrder.CustomerOrderType == CustomerOrderType.Quote;

                if (shouldSkipRegister)
                {
                    return null;
                }

                documentBuilderData.SalesOrder = salesOrder;

                return new VoidTransactionBuilder(documentBuilderData);
            }

            /// <summary>
            /// Gets the fiscal integration document builder for recall event type.
            /// </summary>
            /// <param name="documentBuilderData">The document builder data.</param>
            /// <returns>The fiscal integration receipt document builder.</returns>
            private static async Task<IDocumentBuilder> GetFiscalDocumentBuilderForRecallEventAsync(DocumentBuilderData documentBuilderData)
            {
                var cartSearchCriteria = new CartSearchCriteria(documentBuilderData.FiscalDocumentRetrievalCriteria.TransactionId);
                var getCartServiceRequest = new GetCartServiceRequest(cartSearchCriteria, QueryResultSettings.SingleRecord);
                var cart = (await documentBuilderData.RequestContext.ExecuteAsync<GetCartServiceResponse>(getCartServiceRequest).ConfigureAwait(false)).Carts.Single();

                bool shouldSkipRegister = cart.ExtensibleSalesTransactionType == ExtensibleSalesTransactionType.IncomeExpense ||
                    cart.CustomerOrderMode == CustomerOrderMode.QuoteCreateOrEdit;

                if (shouldSkipRegister)
                {
                    return null;
                }

                return await BeginSaleBuilder.Create(documentBuilderData).ConfigureAwait(false);
            }

            /// <summary>
            /// Gets the sales order.
            /// </summary>
            /// <param name="requestContext">The request context.</param>
            /// <param name="isRemoteTransaction">Gets or sets a value indicating whether the fiscal document is for Remote transaction or not.</param>
            /// <param name="transactionId">The transaction identifier.</param>
            /// <returns>The sales order.</returns>
            private static async Task<SalesOrder> GetSalesOrderAsync(RequestContext requestContext, bool isRemoteTransaction, string transactionId)
            {
                SearchLocation searchLocationType = isRemoteTransaction ? SearchLocation.All : SearchLocation.Local;
                var getSalesOrderRequest = new GetSalesOrderDetailsByTransactionIdServiceRequest(transactionId, searchLocationType);

                return (await requestContext.ExecuteAsync<GetSalesOrderDetailsServiceResponse>(getSalesOrderRequest).ConfigureAwait(false)).SalesOrder;
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
