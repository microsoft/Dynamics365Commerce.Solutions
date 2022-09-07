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
        using Contoso.CommerceRuntime.DocumentProvider.EFRSample.DocumentHelpers;
        using Contoso.CommerceRuntime.DocumentProvider.EFRSample.Extensions;
        using Contoso.CommerceRuntime.DocumentProvider.EFRSample.Messages;
        using Receipt = DataModelEFR.Documents.Receipt;

        /// <summary>
        /// Encapsulates the document generation logic for the income accounts event.
        /// </summary>
        public class IncomeAccountsBuilder : IDocumentBuilder
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
            private IncomeAccountsBuilder(DocumentBuilderData documentBuilderData)
            {
                ThrowIf.Null(documentBuilderData, nameof(documentBuilderData));

                this.documentBuilderData = documentBuilderData;
            }

            public static async Task<IncomeAccountsBuilder> Create(DocumentBuilderData documentBuilderData)
            {
                var instance = new IncomeAccountsBuilder(documentBuilderData);
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
                var receipt = this.CreateReceiptAsync().ConfigureAwait(false);
                return new SalesTransactionRegistrationRequest
                {
                    Receipt = await receipt
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
                    TotalAmount = salesOrder.CalcTotalPaymentAmountWithPrepayment(),
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
                var filteredIncomeExpenseLines = salesOrder.IncomeExpenseLines.Where(incomeExpenseLine => incomeExpenseLine.AccountType == IncomeExpenseAccountType.Income);

                return filteredIncomeExpenseLines.Select(iel =>
                new ReceiptPosition
                {
                    PositionNumber = EfrCommonFunctions.ConvertLineNumberToPositionNumber(iel.LineNumber),
                    Description = iel.AccountType.ToString(),
                    Amount = iel.Amount
                })
                .ToList();
            }

            /// <summary>
            /// Creates payments.
            /// </summary>
            /// <returns>The payments.</returns>
            private async Task<List<ReceiptPayment>> CreateReceiptPaymentsAsync()
            {
                var request = new GetEfrIncomeExpenseAccountsReceiptPaymentsRequest(this.salesOrder, this.documentBuilderData.FiscalIntegrationFunctionalityProfile);
                return (await this.documentBuilderData.RequestContext.ExecuteAsync<SingleEntityDataServiceResponse<List<ReceiptPayment>>>(request).ConfigureAwait(false)).Entity;
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
        }
    }
}