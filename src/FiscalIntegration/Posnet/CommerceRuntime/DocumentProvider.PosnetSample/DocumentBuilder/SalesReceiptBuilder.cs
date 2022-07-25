namespace Contoso
{
    namespace Commerce.Runtime.DocumentProvider.PosnetSample.DocumentBuilder
    {
        using System;
        using System.Collections.Generic;
        using System.Collections.ObjectModel;
        using System.Linq;
        using System.Threading.Tasks;
        using Contoso.Commerce.Runtime.DocumentProvider.PosnetSample.Configuration;
        using Contoso.Commerce.Runtime.DocumentProvider.PosnetSample.DocumentModel;
        using Contoso.Commerce.Runtime.DocumentProvider.PosnetSample.DocumentModel.Protocol;
        using Contoso.Commerce.Runtime.DocumentProvider.PosnetSample.Helpers;
        using Contoso.Commerce.Runtime.DocumentProvider.PosnetSample.PosnetProtocol;
        using Contoso.Commerce.Runtime.DocumentProvider.PosnetSample.PosnetProtocol.Requests;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel.Poland.Entities;
        using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
        using Microsoft.Dynamics.Commerce.Runtime.FiscalIntegration.DocumentProvider.Messages;
        using Microsoft.Dynamics.Commerce.Runtime.Localization.Data.Services.Messages.Poland;
        using Microsoft.Dynamics.Commerce.Runtime.Localization.Entities.Poland;

        /// <summary>
        /// Implements builder for sales receipt.
        /// </summary>
        public class SalesReceiptBuilder : IFiscalDocumentRequestBuilder
        {
            private const string TaxRegistrationIdFeatureName = "Dynamics.AX.Application.RetailTaxRegistrationIdEnableFeature_PL";

            private Dictionary<string, int> tenderTypeMapping;
            private IEnumerable<Tuple<decimal, int>> vatRates;
            private ReadOnlyCollection<Item> productsInOrder;
            private Lazy<Task<List<FiscalIntegrationSalesDiscountFiscalText>>> discountFiscalTexts;
            private GetFiscalDocumentDocumentProviderRequest Request;

            public SalesReceiptBuilder(GetFiscalDocumentDocumentProviderRequest request)
            {
                this.Request = request;
            }

            /// <summary>
            /// Builds fiscal document for sales receipt.
            /// </summary>
            /// <returns>The <see cref="PosnetFiscalDocumentBuildResult"/> instance.</returns>
            public async Task<PosnetFiscalDocumentBuildResult> BuildAsync()
            {
                IEnumerable<IPosnetCommandRequest> commands = new List<IPosnetCommandRequest>();
                var generationResultType = FiscalIntegrationDocumentGenerationResultType.None;

                var adjustedData = await SalesOrderHelper.AdjustSalesOrderAsync(Request).ConfigureAwait(false);
                SalesOrder adjustedSalesOrder = adjustedData.Item1;

                if (this.IsDocumentGenerationRequired(adjustedSalesOrder))
                {
                    if (SalesOrderHelper.IsReturn(adjustedSalesOrder))
                    {
                        commands = this.ReturnOfGoods(adjustedSalesOrder);
                    }
                    else
                    {
                        IEnumerable<SalesLine> salesLines = SalesOrderHelper.GetSalesLines(adjustedSalesOrder, Request);
                        productsInOrder = await SalesOrderHelper.GetProductsInSalesLinesAsync(salesLines, Request.RequestContext).ConfigureAwait(false);
                        discountFiscalTexts = new Lazy<Task<List<FiscalIntegrationSalesDiscountFiscalText>>>(() => SalesOrderHelper.GetDiscountFiscalTextForSalesOrder(Request));

                        commands = this.TransactionInit()
                            .Concat(await this.TransactionNipSet(adjustedSalesOrder).ConfigureAwait(false))
                            .Concat(await this.GetCommands(salesLines, this.SalesLineToCommands).ConfigureAwait(false))
                            .Concat(this.GetDepositPayment(adjustedSalesOrder, Request.FiscalIntegrationFunctionalityProfile))
                            .Concat(await this.GetCommands(adjustedSalesOrder.ActiveTenderLines, this.TenderLineToCommands).ConfigureAwait(false))
                            .Concat(this.TransactionEnd(adjustedSalesOrder))
                            .Concat(this.CounterStatus());
                    }

                    commands = commands.Concat(this.CounterStatus());
                    generationResultType = FiscalIntegrationDocumentGenerationResultType.Succeeded;
                }
                else
                {
                    generationResultType = FiscalIntegrationDocumentGenerationResultType.NotRequired;
                }

                return new PosnetFiscalDocumentBuildResult
                {
                    Document = new PosnetDocumentRequest(commands),
                    DocumentAdjustment = adjustedData.Item2,
                    DocumentGenerationResult = generationResultType
                };
            }

            /// <summary>
            /// Return of goods command.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <returns>An array with retrun od goods request.</returns>
            private IEnumerable<IPosnetCommandRequest> ReturnOfGoods(SalesOrder salesOrder)
            {
                var returnOfGoods = new ReturnOfGoods
                {
                    AmountOfPurchase = Math.Abs(salesOrder.AmountPaid)
                };

                return new IPosnetCommandRequest[]
                {
                    PrinterRequestBuilder.BuildRequestCommand(returnOfGoods)
                };
            }

            /// <summary>
            /// Applies mapping function retail entities collection to command requests.
            /// </summary>
            /// <typeparam name="TSource">Type or entity.</typeparam>
            /// <param name="source">Collection of objects of type TSource.</param>
            /// <param name="mapper">Function to be applied.</param>
            /// <param name="request">A request.</param>
            /// <returns>Collection of commands requests.</returns>
            private async Task<IEnumerable<IPosnetCommandRequest>> GetCommands<TSource>(IEnumerable<TSource> source,
                Func<TSource, Task<IEnumerable<IPosnetCommandRequest>>> mapper)
            {
                var result = Enumerable.Empty<IPosnetCommandRequest>();

                foreach (var item in source)
                {
                    result = result.Concat(await mapper(item).ConfigureAwait(false));
                }

                return result;
            }

            /// <summary>
            /// Maps <see cref="SalesLine"/> to POSNET commands.
            /// </summary>
            /// <param name="salesLine">A sales line.</param>            
            /// <returns>POSNET commands collection.</returns>
            private async Task<IEnumerable<IPosnetCommandRequest>> SalesLineToCommands(SalesLine salesLine)
            {
                List<IPosnetCommandRequest> requests = new List<IPosnetCommandRequest>();

                TransactionLine transactionLineRequest = new TransactionLine
                {
                    NameOfGoods = SalesOrderHelper.GetProductNameByItemID(salesLine.ItemId, productsInOrder),
                    VATRateId = this.GetVatRateId(Request.FiscalIntegrationFunctionalityProfile, salesLine.TaxRatePercent),
                    NumberOfGoods = Math.Abs(salesLine.Quantity),
                    Price = Math.Round(salesLine.Price, SalesOrderHelper.AmountFractionalDigits)
                };

                await this.InitializeSalesLineDiscountDetails(transactionLineRequest, salesLine).ConfigureAwait(false);

                requests.Add(PrinterRequestBuilder.BuildRequestCommand(transactionLineRequest));
                return requests;
            }

            /// <summary>
            /// Fills discount fields in request for sales line.
            /// </summary>
            /// <param name="transactionLineRequest">The line transaction request.</param>
            /// <param name="salesLine">A sales line.</param>
            private async Task InitializeSalesLineDiscountDetails(TransactionLine transactionLineRequest, SalesLine salesLine)
            {
                if (salesLine.DiscountAmount != decimal.Zero && salesLine.DiscountLines.Any())
                {
                    var discountDetails = SalesOrderHelper.GetLineDiscountDetails(salesLine, await discountFiscalTexts.Value.ConfigureAwait(false));
                    transactionLineRequest.DiscountSurcharge = true;
                    transactionLineRequest.DiscountSurchargeName = discountDetails.Item2;
                    transactionLineRequest.DuscountSurchargeAmount = discountDetails.Item1;
                }
            }

            /// <summary>
            /// Maps <see cref="TenderLine"/> to POSNET commands.
            /// </summary>
            /// <param name="tenderLine">A tender line.</param>
            /// <returns>POSNET commands collection.</returns>
            private Task<IEnumerable<IPosnetCommandRequest>> TenderLineToCommands(TenderLine tenderLine)
            {
                List<IPosnetCommandRequest> requests = new List<IPosnetCommandRequest>();

                TransactionPayment paymTrans = new TransactionPayment
                {
                    PaymentFormType = this.GetPaymentForm(Request.FiscalIntegrationFunctionalityProfile, tenderLine.TenderTypeId),
                    PaymentValue = Math.Abs(Math.Round(tenderLine.Amount, SalesOrderHelper.AmountFractionalDigits)),
                    IsChange = tenderLine.IsChangeLine
                };

                requests.Add(PrinterRequestBuilder.BuildRequestCommand(paymTrans));

                return Task.FromResult<IEnumerable<IPosnetCommandRequest>>(requests);
            }

            /// <summary>
            /// Counter status command.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <returns>An array with counter status after transaction completed request.</returns>
            private IEnumerable<IPosnetCommandRequest> CounterStatus()
            {
                return new IPosnetCommandRequest[]
                {
                    PrinterRequestBuilder.BuildRequestCommand(new CounterStatus())
                };
            }

            /// <summary>
            /// Initialize transaction command.
            /// </summary>
            /// <returns>An array with initialize of transaction request.</returns>
            private IEnumerable<IPosnetCommandRequest> TransactionInit()
            {
                return new IPosnetCommandRequest[]
                {
                    PrinterRequestBuilder.BuildRequestCommand(new TransactionInit())
                };
            }

            /// <summary>
            /// Ends of transaction command.
            /// </summary>
            /// <param name="salesOrder">The sales order.</param>
            /// <returns>An array with end of transaction request.</returns>
            private IEnumerable<IPosnetCommandRequest> TransactionEnd(SalesOrder salesOrder)
            {
                TransactionEnd req = new TransactionEnd
                {
                    FiscalValue = Math.Round(SalesOrderHelper.GetFiscalAmount(salesOrder), SalesOrderHelper.AmountFractionalDigits),
                    PaymentValue = Math.Round(SalesOrderHelper.GetTotalPaidAmount(salesOrder), SalesOrderHelper.AmountFractionalDigits)
                };

                decimal changeAmount = SalesOrderHelper.GetTotalChangeAmount(salesOrder);

                if (changeAmount != decimal.Zero)
                {
                    req.ChangeValue = changeAmount;
                }

                return new IPosnetCommandRequest[] { PrinterRequestBuilder.BuildRequestCommand(req) };
            }

            /// <summary>
            /// Gets payment for deposit payment.
            /// </summary>
            /// <param name="salesOrder">The sales order.</param>
            /// <param name="functionalityProfile">The functionality profile.</param>
            /// <returns>The request for deposit payment if exists otherwise empty collection.</returns>
            private IEnumerable<IPosnetCommandRequest> GetDepositPayment(SalesOrder salesOrder, FiscalIntegrationFunctionalityProfile functionalityProfile)
            {
                List<IPosnetCommandRequest> requests = new List<IPosnetCommandRequest>();

                if (SalesOrderHelper.IsPrepaymentAmountAppliedOnPickup(salesOrder))
                {
                    int paymentForm = ConfigurationController.ParseDepositPaymentType(functionalityProfile);
                    TransactionPayment paymTrans = new TransactionPayment
                    {
                        PaymentFormType = paymentForm,
                        PaymentValue = SalesOrderHelper.GetPrepaymentDepositAmount(salesOrder),
                        IsChange = false
                    };

                    requests.Add(PrinterRequestBuilder.BuildRequestCommand(paymTrans));
                }

                return requests;
            }

            /// <summary>
            /// Gets the payment type and index attribute of the fiscal printer by the payment method supported in demo data.
            /// </summary>
            /// <param name="functionalityProfile">The functionality profile.</param>
            /// <param name="tenderTypeId">The tender type ID in demo data.</param>
            /// <returns>Integer value according to printer settings.</returns>
            private int GetPaymentForm(FiscalIntegrationFunctionalityProfile functionalityProfile, string tenderTypeId)
            {
                ThrowIf.Null(functionalityProfile, nameof(functionalityProfile));

                if (tenderTypeMapping == null)
                {
                    tenderTypeMapping = ConfigurationController.ParseSupportedTenderTypeMappings(functionalityProfile);
                }

                int tenderTypeNum = 0;

                if (!tenderTypeMapping.TryGetValue(tenderTypeId, out tenderTypeNum))
                {
                    throw new Exception($"Payment form mapping not found for value '{tenderTypeId}'.");
                }

                return tenderTypeNum;
            }

            /// <summary>
            /// Gets the VAT rate number of fiscsal printer.
            /// </summary>
            /// <param name="functionalityProfile">The functionality profile.</param>
            /// <param name="taxRatePercent">Rate percent value.</param>
            /// <returns>VAT rate numbers defined in printer. 0 - VAT A, 1 - VAT B, etc.</returns>
            private int GetVatRateId(FiscalIntegrationFunctionalityProfile functionalityProfile, decimal taxRatePercent)
            {
                ThrowIf.Null(functionalityProfile, nameof(functionalityProfile));

                decimal taxRatePercentSalesLine = Math.Round(taxRatePercent, SalesOrderHelper.AmountFractionalDigits);

                if (vatRates == null)
                {
                    vatRates = ConfigurationController.ParseSupportedVATRates(functionalityProfile);
                }

                int taxRateId;

                int[] rateIds = vatRates
                        .Where(t => t.Item1 == taxRatePercentSalesLine)
                        .Select(t => t.Item2)
                        .ToArray();

                if (rateIds.Length == 0)
                {
                    throw new Exception($"VAT rate mapping not found for value '{taxRatePercentSalesLine}'.");
                }
                else
                {
                    taxRateId = rateIds[0];
                }

                return taxRateId;
            }

            /// <summary>
            /// Gets whether document generation is required for a sales order.
            /// </summary>
            /// <param name="adjustedSalesOrder">The sales order.</param>
            /// <returns>True if document generation is required, otherwise false.</returns>
            /// <remarks>The sales order should be adjusted.</remarks>
            private bool IsDocumentGenerationRequired(SalesOrder adjustedSalesOrder)
            {
                return adjustedSalesOrder.ActiveSalesLines.Any();
            }

            /// <summary>
            /// Sets the transaction's VAT number (NIP).
            /// </summary>
            /// <param name="salesOrder">The sales order.</param>
            /// <returns>The POSNET command collection.</returns>
            private async Task<IEnumerable<IPosnetCommandRequest>> TransactionNipSet(SalesOrder salesOrder)
            {
                if (this.Request.RequestContext.Runtime.GetRequestHandlers<IRequestHandler>(typeof(GetFiscalCustomerDataDataRequest)).IsNullOrEmpty())
                {
                    return Enumerable.Empty<IPosnetCommandRequest>();
                }

                var request = new GetFiscalCustomerDataDataRequest(salesOrder.Id, salesOrder.TerminalId);
                if (await this.Request.RequestContext.IsFeatureEnabledAsync(TaxRegistrationIdFeatureName, defaultValue: false).ConfigureAwait(false))
                {
                    var fiscalCustomer = (await this.Request.RequestContext.ExecuteAsync<SingleEntityDataServiceResponse<FiscalCustomer>>(request).ConfigureAwait(false)).Entity;

                    if (fiscalCustomer == null)
                    {
                        return Enumerable.Empty<IPosnetCommandRequest>();
                    }

                    var trNipSetRequest = new TransactionNipSet { Nip = fiscalCustomer.VatId };
                    return new[] { PrinterRequestBuilder.BuildRequestCommand(trNipSetRequest) };
                }
                else
                {
                    var fiscalCustomerData = (await this.Request.RequestContext.ExecuteAsync<SingleEntityDataServiceResponse<FiscalCustomerData>>(request).ConfigureAwait(false)).Entity;

                    if (fiscalCustomerData == null)
                    {
                        return Enumerable.Empty<IPosnetCommandRequest>();
                    }

                    var trNipSetRequest = new TransactionNipSet { Nip = fiscalCustomerData.VatId };
                    return new[] { PrinterRequestBuilder.BuildRequestCommand(trNipSetRequest) };
                }
            }
        }
    }
}
