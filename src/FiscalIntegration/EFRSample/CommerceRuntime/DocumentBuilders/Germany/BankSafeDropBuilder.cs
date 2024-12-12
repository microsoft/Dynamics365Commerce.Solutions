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
    namespace CommerceRuntime.DocumentProvider.EFRSample.DocumentBuilders.GermanyBuilders
    {
        using System.Collections.Generic;
        using System.Linq;
        using System.Threading.Tasks;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
        using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;
        using Contoso.CommerceRuntime.DocumentProvider.DataModelEFR.Documents;
        using Contoso.CommerceRuntime.DocumentProvider.EFRSample.DocumentBuilders.Parameters;
        using Contoso.CommerceRuntime.DocumentProvider.EFRSample.Extensions;
        using Contoso.CommerceRuntime.DocumentProvider.EFRSample.Messages;
        using Receipt = DataModelEFR.Documents.Receipt;

        /// <summary>
        /// Incapsulates the document generation logic for the safe drop event.
        /// </summary>
        public class BankSafeDropBuilder : IDocumentBuilder
        {
            /// <summary>
            /// The request.
            /// </summary>
            private readonly DocumentBuilderData documentBuilderData;

            /// <summary>
            /// The sales order.
            /// </summary>
            private SalesOrder salesOrder;

            /// <summary>
            /// Initializes a new instance of the <see cref="BeginSaleBuilder"/> class.
            /// </summary>
            /// <param name="documentBuilderData">The request.</param>
            private BankSafeDropBuilder(DocumentBuilderData documentBuilderData)
            {
                ThrowIf.Null(documentBuilderData, nameof(documentBuilderData));

                this.documentBuilderData = documentBuilderData;
            }

            public static async Task<BankSafeDropBuilder> Create(DocumentBuilderData documentBuilderData)
            {
                var instance = new BankSafeDropBuilder(documentBuilderData); ;
                SearchLocation searchLocationType = documentBuilderData.FiscalDocumentRetrievalCriteria.IsRemoteTransaction ? SearchLocation.All : SearchLocation.Local;
                var transactionId = documentBuilderData.FiscalDocumentRetrievalCriteria.TransactionId;
                var getSalesOrderRequest = new GetSalesOrderDetailsByTransactionIdServiceRequest(transactionId, searchLocationType);
                instance.salesOrder = (await documentBuilderData.RequestContext.ExecuteAsync<GetSalesOrderDetailsServiceResponse>(getSalesOrderRequest).ConfigureAwait(false)).SalesOrder;
                return instance;
            }

            /// <summary>
            /// Builds fiscal integration document.
            /// </summary>
            /// <returns> The fiscal integration receipt document.</returns>
            public async Task<IFiscalIntegrationDocument> BuildAsync()
            {
                var receipt = await this.CreateReceiptAsync().ConfigureAwait(false);
                return new SalesTransactionRegistrationRequest
                {
                    Receipt = receipt
                };
            }

            /// <summary>
            /// Creates a receipt.
            /// </summary>
            /// <returns>The receipt.</returns>
            private async Task<Receipt> CreateReceiptAsync()
            {
                var receipt = new Receipt
                {
                    TransactionLocation = salesOrder.StoreId,
                    TransactionTerminal = salesOrder.TerminalId,
                    TransactionNumber = salesOrder.Id,
                    TotalAmount = -1 * salesOrder.TotalAmount,
                    NonFiscalTransactionType = await this.GetEfrNonFiscalTransactionType().ConfigureAwait(false),
                    PositionLines = await this.CreateReceiptPositionLines().ConfigureAwait(false),
                    Payments = await this.CreateReceiptPaymentsAsync().ConfigureAwait(false),
                };
                return receipt;
            }

            /// <summary>
            /// Creates position lines.
            /// </summary>
            /// <returns>The receipt position lines.</returns>
            private async Task<ReceiptPositionLines> CreateReceiptPositionLines()
            {
                if (!(await this.CanСreateReceiptPosition().ConfigureAwait(false)))
                {
                    return null;
                }

                ReceiptPositionLines receiptPositionLines = new ReceiptPositionLines
                {
                    ReceiptPositions = this.CreateReceiptPositions(),
                };

                return receiptPositionLines;
            }

            /// <summary>
            /// Creates positions.
            /// </summary>
            /// <returns>The receipt positions.</returns>
            private List<ReceiptPosition> CreateReceiptPositions()
            {
                var position = new ReceiptPosition
                {
                    PositionNumber = 1,
                    Description = salesOrder.ExtensibleSalesTransactionType.Name,
                    Amount = -1 * salesOrder.TotalAmount
                };

                return new List<ReceiptPosition>(new[] { position });
            }

            /// <summary>
            /// Creates payments.
            /// </summary>
            /// <returns>The payments.</returns>
            private async Task<List<ReceiptPayment>> CreateReceiptPaymentsAsync()
            {
                var getDropAndDeclareTransactionTenderDetailsDataRequest = new GetDropAndDeclareTransactionTenderDetailsDataRequest(salesOrder.Id, QueryResultSettings.AllRecords);
                var tenderLines = (await this.documentBuilderData.RequestContext.ExecuteAsync<EntityDataServiceResponse<TenderDetail>>(getDropAndDeclareTransactionTenderDetailsDataRequest).ConfigureAwait(false)).PagedEntityCollection.Results;

                var tenderLinesGrpByTenderTypeId = tenderLines.GroupBy(tl => new
                {
                    tl.TenderTypeId,
                    tl.ForeignCurrency
                });

                return (await Task.WhenAll(
                    tenderLinesGrpByTenderTypeId.Select(async g =>
                    {
                        var payment = new ReceiptPayment
                        {
                            Description = await GetEfrTenderTypeName(g.Key.TenderTypeId).ConfigureAwait(false),
                            PaymentTypeGroup = this.documentBuilderData.FiscalIntegrationFunctionalityProfile.GetPaymentTypeGroup(g.Key.TenderTypeId),
                            Amount = -1 * g.Sum(l => l.Amount)
                        };

                        if (salesOrder.CurrencyCode != g.Key.ForeignCurrency)
                        {
                            payment.ForeignAmount = -1 * g.Sum(l => l.AmountInForeignCurrency);
                            payment.ForeignCurrencyCode = g.Key.ForeignCurrency;
                        }

                        return payment;
                    })).ConfigureAwait(false)
                ).ToList();
            }

            private async Task<bool> CanСreateReceiptPosition()
            {
                var request = new GetEfrCanСreateReceiptPositionsRequest(this.documentBuilderData.SalesOrder);
                return (await this.documentBuilderData.RequestContext.ExecuteAsync<SingleEntityDataServiceResponse<bool>>(request).ConfigureAwait(false)).Entity;
            }

            private async Task<string> GetEfrNonFiscalTransactionType()
            {
                var request = new GetEfrNonFiscalTransactionTypeRequest(this.documentBuilderData.FiscalDocumentRetrievalCriteria.FiscalRegistrationEventType, this.documentBuilderData.SalesOrder);
                return (await this.documentBuilderData.RequestContext.ExecuteAsync<SingleEntityDataServiceResponse<string>>(request).ConfigureAwait(false)).Entity;
            }

            private async Task<string> GetEfrTenderTypeName(string tenderTypeId)
            {
                var request = new GetEfrGetTenderTypeNameRequest(tenderTypeId);
                return (await this.documentBuilderData.RequestContext.ExecuteAsync<SingleEntityDataServiceResponse<string>>(request).ConfigureAwait(false)).Entity;
            }
        }
    }
}