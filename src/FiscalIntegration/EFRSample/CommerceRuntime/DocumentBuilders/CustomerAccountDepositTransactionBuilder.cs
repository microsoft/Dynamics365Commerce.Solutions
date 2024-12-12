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
    namespace CommerceRuntime.DocumentProvider.EFRSample.DocumentBuilders
    {
        using System.Collections.Generic;
        using System.Linq;
        using System.Threading.Tasks;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
        using Contoso.CommerceRuntime.DocumentProvider.DataModelEFR.Documents;
        using Contoso.CommerceRuntime.DocumentProvider.EFRSample.DocumentBuilders.Parameters;
        using Contoso.CommerceRuntime.DocumentProvider.EFRSample.Extensions;
        using Contoso.CommerceRuntime.DocumentProvider.EFRSample.Messages;
        using Receipt = DataModelEFR.Documents.Receipt;
        using SalesTransactionRegistrationRequest = DataModelEFR.Documents.SalesTransactionRegistrationRequest;

        /// <summary>
        /// Builds a customer account deposit transaction document.
        /// </summary>
        public class CustomerAccountDepositTransactionBuilder : IDocumentBuilder
        {
            private readonly DocumentBuilderData documentBuilderData;

            /// <summary>
            /// Initializes a new instance of the <see cref="CustomerAccountDepositTransactionBuilder"/> class.
            /// </summary>
            /// <param name="documentBuilderData">The request.</param>
            public CustomerAccountDepositTransactionBuilder(DocumentBuilderData documentBuilderData)
            {
                ThrowIf.Null(documentBuilderData, nameof(documentBuilderData));

                this.documentBuilderData = documentBuilderData;
            }

            /// <summary>
            /// Builds fiscal integration document.
            /// </summary>
            /// <returns>The customer account deposit receipt document.</returns>
            public async Task<IFiscalIntegrationDocument> BuildAsync()
            {
                return new SalesTransactionRegistrationRequest
                {
                    Receipt = await this.CreateReceiptAsync().ConfigureAwait(false)
                };
            }

            /// <summary>
            /// Creates receipt.
            /// </summary>
            /// <returns>The receipt.</returns>
            private async Task<Receipt> CreateReceiptAsync()
            {
                Receipt receipt = new Receipt
                {
                    CountryRegionISOCode = this.documentBuilderData.RequestContext.GetChannelConfiguration().CountryRegionISOCode,
                    ReceiptDateTime = this.documentBuilderData.SalesOrder.CreatedDateTime.DateTime,
                    TransactionLocation = this.documentBuilderData.SalesOrder.StoreId,
                    TransactionTerminal = this.documentBuilderData.SalesOrder.TerminalId,
                    OperatorId = this.documentBuilderData.SalesOrder.StaffId,
                    OperatorName = await GetOperatorName().ConfigureAwait(false),
                    NonFiscalTransactionType = await GetCustomerAccountDepositTransactionType().ConfigureAwait(false),
                    IsTrainingTransaction = 0,
                    PositionLines = await this.CreateReceiptPositionLines().ConfigureAwait(false),
                    Payments = await this.CreateReceiptPaymentsAsync().ConfigureAwait(false),
                    Taxes = await this.CreateReceiptTaxes().ConfigureAwait(false),
                };
                if (receipt.PositionLines.ReceiptPositions.Count > 0)
                    receipt.TotalAmount = receipt.PositionLines.ReceiptPositions.Sum(c => c.Amount);
                await PopulateEfrLocalizationInfo(receipt).ConfigureAwait(false);
                await PopulateEfrCustomerData(receipt).ConfigureAwait(false);

                return receipt;
            }

            /// <summary>
            /// Creates receipt position lines.
            /// </summary>
            /// <returns>The receipt position lines.</returns>
            private async Task<ReceiptPositionLines> CreateReceiptPositionLines()
            {
                if (this.documentBuilderData.SalesOrder.IsDepositProcessing() && this.documentBuilderData.SalesOrder.HasDepositLinesOnly(this.documentBuilderData.RequestContext))
                {
                    return null;
                }

                ReceiptPositionLines receiptPositionLines = new ReceiptPositionLines
                {
                    ReceiptPositions = await this.CreateReceiptPositions().ConfigureAwait(false),
                };

                return receiptPositionLines;
            }

            /// <summary>
            /// Creates receipt positions.
            /// </summary>
            /// <returns>The receipt positions.</returns>
            private async Task<List<ReceiptPosition>> CreateReceiptPositions()
            {
                List<ReceiptPosition> positions = new List<ReceiptPosition>();
                await GetCustomerAccountDepositPositions(positions).ConfigureAwait(false);
                return positions;
            }

            /// <summary>
            /// Creates receipt payments.
            /// </summary>
            /// <returns>The receipt payments.</returns>
            private async Task<List<ReceiptPayment>> CreateReceiptPaymentsAsync()
            {
                var request = new GetEfrReceiptPaymentsRequest(this.documentBuilderData.SalesOrder, this.documentBuilderData.FiscalIntegrationFunctionalityProfile);
                return (await this.documentBuilderData.RequestContext.ExecuteAsync<SingleEntityDataServiceResponse<List<ReceiptPayment>>>(request).ConfigureAwait(false)).Entity;
            }

            /// <summary>
            /// Creates receipt taxes.
            /// </summary>
            /// <returns>The receipt taxes.</returns>
            private async Task<List<ReceiptTax>> CreateReceiptTaxes()
            {
                return await GetEfrCustomerAccountDepositReceiptTaxes().ConfigureAwait(false);
            }

            private async Task<DataModelEFR.Documents.Receipt> PopulateEfrLocalizationInfo(DataModelEFR.Documents.Receipt receipt)
            {
                var request = new PopulateEfrLocalizationInfoRequest(receipt, this.documentBuilderData.SalesOrder);
                return (await this.documentBuilderData.RequestContext.ExecuteAsync<SingleEntityDataServiceResponse<DataModelEFR.Documents.Receipt>>(request).ConfigureAwait(false)).Entity;
            }

            private async Task<DataModelEFR.Documents.Receipt> PopulateEfrCustomerData(DataModelEFR.Documents.Receipt receipt)
            {
                var request = new PopulateEfrCustomerDataRequest(receipt, this.documentBuilderData.SalesOrder, this.documentBuilderData.FiscalIntegrationFunctionalityProfile);
                return (await this.documentBuilderData.RequestContext.ExecuteAsync<SingleEntityDataServiceResponse<DataModelEFR.Documents.Receipt>>(request).ConfigureAwait(false)).Entity;
            }

            private async Task<string> GetCustomerAccountDepositTransactionType()
            {
                var request = new GetEfrCustomerAccountDepositTransactionTypeRequest();
                return (await this.documentBuilderData.RequestContext.ExecuteAsync<SingleEntityDataServiceResponse<string>>(request).ConfigureAwait(false)).Entity;
            }

            private async Task<List<ReceiptPosition>> GetCustomerAccountDepositPositions(List<ReceiptPosition> receiptPositions)
            {
                var request = new GetEfrCustomerAccountDepositPositionsRequest(receiptPositions, this.documentBuilderData.SalesOrder, this.documentBuilderData.FiscalIntegrationFunctionalityProfile);
                return (await this.documentBuilderData.RequestContext.ExecuteAsync<SingleEntityDataServiceResponse<List<ReceiptPosition>>>(request).ConfigureAwait(false)).Entity;
            }

            private async Task<List<ReceiptTax>> GetEfrCustomerAccountDepositReceiptTaxes()
            {
                var request = new GetEfrCustomerAccountDepositReceiptTaxesRequest(this.documentBuilderData.SalesOrder, this.documentBuilderData.FiscalIntegrationFunctionalityProfile);
                return (await this.documentBuilderData.RequestContext.ExecuteAsync<SingleEntityDataServiceResponse<List<ReceiptTax>>>(request).ConfigureAwait(false)).Entity;
            }

            private async Task<string> GetOperatorName()
            {
                var request = new GetEfrOperatorNameRequest(this.documentBuilderData.SalesOrder.StaffId);
                return (await this.documentBuilderData.RequestContext.ExecuteAsync<SingleEntityDataServiceResponse<string>>(request).ConfigureAwait(false)).Entity;
            }
        }
    }
}
