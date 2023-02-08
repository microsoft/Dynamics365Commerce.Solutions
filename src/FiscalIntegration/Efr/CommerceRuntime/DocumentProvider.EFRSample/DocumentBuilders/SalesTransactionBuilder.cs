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
        using System;
        using System.Collections.Generic;
        using System.Linq;
        using System.Threading.Tasks;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
        using Contoso.CommerceRuntime.DocumentProvider.DataModelEFR.Documents;
        using Contoso.CommerceRuntime.DocumentProvider.EFRSample.DocumentBuilders.Parameters;
        using Contoso.CommerceRuntime.DocumentProvider.EFRSample.DocumentHelpers;
        using Contoso.CommerceRuntime.DocumentProvider.EFRSample.Extensions;
        using Contoso.CommerceRuntime.DocumentProvider.EFRSample.Messages;

        /// <summary>
        /// Builds a sales transaction registration request document.
        /// </summary>
        public class SalesTransactionBuilder : IDocumentBuilder
        {
            private readonly DocumentBuilderData documentBuilderData;

            /// <summary>
            /// Initializes a new instance of the <see cref="SalesTransactionBuilder"/> class.
            /// </summary>
            /// <param name="documentBuilderData">The request.</param>
            public SalesTransactionBuilder(DocumentBuilderData documentBuilderData)
            {
                ThrowIf.Null(documentBuilderData, nameof(documentBuilderData));

                this.documentBuilderData = documentBuilderData;
            }

            /// <summary>
            /// Builds fiscal integration document.
            /// </summary>
            /// <returns>The sales transaction receipt document.</returns>
            public async Task<IFiscalIntegrationDocument> BuildAsync()
            {
                if (!(await this.GetEfrIsSalesTransactionDocumentGenerationRequired().ConfigureAwait(false)))
                {
                    return null;
                }
                var receipt = await this.CreateReceiptAsync().ConfigureAwait(false);
                receipt.Header = this.CreateReceiptHeader();
                receipt.PositionLines = await this.CreateReceiptPositionLines().ConfigureAwait(false);
                receipt.Payments = await CreateReceiptPaymentsAsync().ConfigureAwait(false);
                receipt.Taxes = await this.CreateReceiptTaxes().ConfigureAwait(false);
                receipt.Footer = this.CreateReceiptFooter();
                return new SalesTransactionRegistrationRequest
                {
                    Receipt = receipt
                };
            }

            /// <summary>
            /// Creates receipt.
            /// </summary>
            /// <returns>The receipt.</returns>
            private async Task<DataModelEFR.Documents.Receipt> CreateReceiptAsync()
            {
                DataModelEFR.Documents.Receipt receipt = new DataModelEFR.Documents.Receipt
                {
                    CountryRegionISOCode = this.documentBuilderData.RequestContext.GetChannelConfiguration().CountryRegionISOCode,
                    TransactionLocation = this.documentBuilderData.SalesOrder.StoreId,
                    TransactionTerminal = this.documentBuilderData.SalesOrder.TerminalId,
                    TotalAmount = await this.GetTotalAmount().ConfigureAwait(false),
                    OperatorId = this.documentBuilderData.SalesOrder.StaffId,
                    OperatorName = await this.GetOperatorName().ConfigureAwait(false),
                    NonFiscalTransactionType = await this.GetEfrNonFiscalTransactionType().ConfigureAwait(false),
                    IsTrainingTransaction = 0
                };
                await PopulateEfrLocalizationInfo(receipt).ConfigureAwait(false);
                await PopulateEfrCustomerData(receipt).ConfigureAwait(false);

                return receipt;
            }

            /// <summary>
            /// Creates receipt header.
            /// </summary>
            /// <returns>The receipt header.</returns>
            private ReceiptHeader CreateReceiptHeader()
            {
                if (string.IsNullOrWhiteSpace(this.documentBuilderData.SalesOrder.CustomerId))
                {
                    return null;
                }

                return new ReceiptHeader
                {
                    Txt = this.documentBuilderData.SalesOrder.CustomerId
                };
            }

            /// <summary>
            /// Creates receipt position lines.
            /// </summary>
            /// <returns>The receipt position lines.</returns>
            private async Task<ReceiptPositionLines> CreateReceiptPositionLines()
            {
                if (!(await this.CanСreateReceiptPosition().ConfigureAwait(false)))
                {
                    return null;
                }
                // get sales lines here && pass to methods
                var salesLines = (await GetSalesLines().ConfigureAwait(false)).ToList();

                ReceiptPositionLines receiptPositionLines = new ReceiptPositionLines
                {
                    ReceiptPositions = await this.CreateReceiptPositionsAsync(salesLines).ConfigureAwait(false),
                    ReceiptPositionModifiers = this.CreateReceiptPositionModifiers(salesLines)
                };

                return receiptPositionLines;
            }

            /// <summary>
            /// Creates receipt positions.
            /// </summary>
            /// <returns>The receipt positions.</returns>
            private async Task<List<ReceiptPosition>> CreateReceiptPositionsAsync(List<SalesLine> salesLines)
            {
                List<ReceiptPosition> receiptPositions = new List<ReceiptPosition>();
                IEnumerable<string> itemIds = salesLines.Select(l => l.ItemId);

                var itemsDictionary = itemIds.Any()
                    ? await GetProducts(this.documentBuilderData.RequestContext, itemIds).ConfigureAwait(false)
                    : new Dictionary<string, Item>();

                foreach (SalesLine salesLine in salesLines)
                {
                    itemsDictionary.TryGetValue(salesLine.ItemId, out var item);

                    ReceiptPosition receiptPosition = new ReceiptPosition
                    {
                        PositionNumber = EfrCommonFunctions.ConvertLineNumberToPositionNumber(salesLine.LineNumber),
                        ItemNumber = salesLine.ItemId,
                        ItemIdentity = salesLine.ItemIdentity(),
                        Description = item?.Description,
                        TaxGroup = await GetSalesLineTaxGroups(salesLine).ConfigureAwait(false),
                        Amount = salesLine.GrossAmount,
                        Quantity = salesLine.Quantity,
                        QuantityUnit = salesLine.UnitOfMeasureSymbol,
                        UnitPrice = salesLine.Price
                    };
                    await this.SetReferenceFieldsAsync(receiptPosition, salesLine).ConfigureAwait(false);
                    receiptPositions.Add(receiptPosition);
                }
                await PopulateCountryRegionSpecificPositions(receiptPositions).ConfigureAwait(false);

                return receiptPositions;
            }

            /// <summary>
            /// Sets reference fields.
            /// </summary>
            /// <param name="receiptPosition">The receipt position.</param>
            /// <param name="salesLine">The sales line.</param>
            private async Task SetReferenceFieldsAsync(ReceiptPosition receiptPosition, SalesLine salesLine)
            {
                receiptPosition.ReferenceDateTime = DateTime.MinValue;
                receiptPosition.ReferenceTransactionLocation = string.Empty;
                receiptPosition.ReferenceTransactionTerminal = string.Empty;
                receiptPosition.ReferenceTransactionNumber = string.Empty;
                receiptPosition.ReferencePositionNumber = 0;

                if (!salesLine.IsReturnLine())
                {
                    return;
                }

                if (this.documentBuilderData.SalesOrder.ExtensibleSalesTransactionType == ExtensibleSalesTransactionType.Sales)
                {
                    await PopulateReferenceFields(receiptPosition, salesLine).ConfigureAwait(false);
                }
            }

            /// <summary>
            /// Creates receipt position modifiers.
            /// </summary>
            /// <returns>The receipt position modifiers.</returns>
            private List<ReceiptPositionModifier> CreateReceiptPositionModifiers(IEnumerable<SalesLine> salesLines)
            {
                List<ReceiptPositionModifier> receiptPositionModifiers = new List<ReceiptPositionModifier>();

                foreach (SalesLine salesLine in salesLines)
                {
                    IEnumerable<DiscountLine> sortedDiscounts = salesLine.DiscountLines
                        .OrderBy(pct => pct.Percentage)
                        .ThenBy(amnt => amnt.EffectiveAmount);

                    foreach (DiscountLine discountLine in sortedDiscounts)
                    {
                        receiptPositionModifiers.Add(
                            new ReceiptPositionModifier
                            {
                                PositionNumber = EfrCommonFunctions.ConvertLineNumberToPositionNumber(salesLine.LineNumber),
                                Description = discountLine.DiscountName(),
                                Amount = -discountLine.EffectiveAmount
                            });
                    }
                }

                return receiptPositionModifiers;
            }

            /// <summary>
            /// Creates receipt taxes.
            /// </summary>
            /// <returns>The receipt taxes.</returns>
            private async Task<List<ReceiptTax>> CreateReceiptTaxes()
            {
                var receiptTaxes = await GetReceiptTaxes().ConfigureAwait(false);

                return receiptTaxes.ToList();
            }

            /// <summary>
            /// Creates receipt footer.
            /// </summary>
            /// <returns>The receipt footer.</returns>
            private ReceiptFooter CreateReceiptFooter()
            {
                return null;
            }

            private async Task<bool> GetEfrIsSalesTransactionDocumentGenerationRequired()
            {
                var request = new GetEfrIsSalesTransactionDocumentGenerationRequiredRequest(this.documentBuilderData.SalesOrder);
                return (await this.documentBuilderData.RequestContext.ExecuteAsync<SingleEntityDataServiceResponse<bool>>(request).ConfigureAwait(false)).Entity;
            }

            private async Task<decimal> GetTotalAmount()
            {
                var request = new GetEfrSalesTransactionTotalAmountRequest(this.documentBuilderData.SalesOrder);
                return (await this.documentBuilderData.RequestContext.ExecuteAsync<SingleEntityDataServiceResponse<decimal>>(request).ConfigureAwait(false)).Entity;
            }

            private async Task<string> GetOperatorName()
            {
                var request = new GetEfrOperatorNameRequest(this.documentBuilderData.SalesOrder.StaffId);
                return (await this.documentBuilderData.RequestContext.ExecuteAsync<SingleEntityDataServiceResponse<string>>(request).ConfigureAwait(false)).Entity;
            }

            private async Task<string> GetEfrNonFiscalTransactionType()
            {
                var request = new GetEfrNonFiscalTransactionTypeRequest(this.documentBuilderData.FiscalDocumentRetrievalCriteria.FiscalRegistrationEventType, this.documentBuilderData.SalesOrder);
                return (await this.documentBuilderData.RequestContext.ExecuteAsync<SingleEntityDataServiceResponse<string>>(request).ConfigureAwait(false)).Entity;
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

            private async Task<bool> CanСreateReceiptPosition()
            {
                var request = new GetEfrCanСreateReceiptPositionsRequest(this.documentBuilderData.SalesOrder);
                return (await this.documentBuilderData.RequestContext.ExecuteAsync<SingleEntityDataServiceResponse<bool>>(request).ConfigureAwait(false)).Entity;
            }

            private async Task<List<ReceiptPayment>> CreateReceiptPaymentsAsync()
            {
                var request = new GetEfrReceiptPaymentsRequest(this.documentBuilderData.SalesOrder, this.documentBuilderData.FiscalIntegrationFunctionalityProfile);
                return (await this.documentBuilderData.RequestContext.ExecuteAsync<SingleEntityDataServiceResponse<List<ReceiptPayment>>>(request).ConfigureAwait(false)).Entity;
            }

            private async Task<IEnumerable<SalesLine>> GetSalesLines()
            {
                var request = new GetEfrSalesLinesRequest(this.documentBuilderData.SalesOrder);
                return (await this.documentBuilderData.RequestContext.ExecuteAsync<SingleEntityDataServiceResponse<IEnumerable<SalesLine>>>(request).ConfigureAwait(false)).Entity;
            }

            private async Task<string> GetSalesLineTaxGroups(SalesLine salesLine)
            {
                var request = new GetEfrSalesLineTaxGroupsRequest(salesLine, this.documentBuilderData.FiscalIntegrationFunctionalityProfile);
                return (await this.documentBuilderData.RequestContext.ExecuteAsync<SingleEntityDataServiceResponse<string>>(request).ConfigureAwait(false)).Entity;
            }

            private async Task<List<ReceiptPosition>> PopulateCountryRegionSpecificPositions(List<ReceiptPosition> receiptPositions)
            {
                var request = new PopulateCountryRegionSpecificPositionsRequest(receiptPositions, this.documentBuilderData.SalesOrder, this.documentBuilderData.FiscalIntegrationFunctionalityProfile);
                return (await this.documentBuilderData.RequestContext.ExecuteAsync<SingleEntityDataServiceResponse<List<ReceiptPosition>>>(request).ConfigureAwait(false)).Entity;
            }

            private async Task<ReceiptPosition> PopulateReferenceFields(ReceiptPosition receiptPosition, SalesLine salesLine)
            {
                var request = new PopulateEfrReferenceFieldsRequest(receiptPosition, salesLine);
                return (await this.documentBuilderData.RequestContext.ExecuteAsync<SingleEntityDataServiceResponse<ReceiptPosition>>(request).ConfigureAwait(false)).Entity;
            }

            private async Task<IEnumerable<ReceiptTax>> GetReceiptTaxes()
            {
                var request = new GetEfrReceiptTaxesRequest(this.documentBuilderData.SalesOrder, this.documentBuilderData.FiscalIntegrationFunctionalityProfile);
                return (await this.documentBuilderData.RequestContext.ExecuteAsync<SingleEntityDataServiceResponse<IEnumerable<ReceiptTax>>>(request).ConfigureAwait(false)).Entity;
            }

            /// <summary>
            /// Gets products by their item identifiers.
            /// </summary>
            /// <param name="context">The request context.</param>
            /// <param name="itemIds">The product item identifiers.</param>
            /// <returns>The list of products.</returns>
            private static async Task<IDictionary<string, Item>> GetProducts(RequestContext context, IEnumerable<string> itemIds)
            {
                ThrowIf.Null(context, nameof(context));

                GetItemsDataRequest getItemsDataRequest = new GetItemsDataRequest(itemIds);
                GetItemsDataResponse getItemsResponse = await context.ExecuteAsync<GetItemsDataResponse>(getItemsDataRequest).ConfigureAwait(false);

                return getItemsResponse.Items.IsNullOrEmpty()
                    ? new Dictionary<string, Item>()
                    : getItemsResponse.Items.ToDictionary(item => item.ItemId);
            }
        }
    }
}
