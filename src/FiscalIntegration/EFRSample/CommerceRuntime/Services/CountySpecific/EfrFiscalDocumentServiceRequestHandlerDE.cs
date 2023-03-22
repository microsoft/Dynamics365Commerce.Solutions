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
    namespace CommerceRuntime.DocumentProvider.EFRSample.Services.CountySpecific
    {
        using System;
        using System.Collections.Generic;
        using System.Linq;
        using System.Threading.Tasks;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;
        using Contoso.CommerceRuntime.DocumentProvider.DataModelEFR.Documents;
        using Contoso.CommerceRuntime.DocumentProvider.EFRSample.DocumentHelpers;
        using Contoso.CommerceRuntime.DocumentProvider.EFRSample.Extensions;
        using Contoso.CommerceRuntime.DocumentProvider.EFRSample.Messages;

        public sealed class EfrFiscalDocumentServiceRequestHandlerDE : IRequestHandlerAsync, ICountryRegionAware
        {
            private const decimal ExemptTaxPercentage = decimal.Zero;
            private static readonly IDictionary<FiscalIntegrationEventType, string> NonFiscalTransactionTypeDictionary =
                new Dictionary<FiscalIntegrationEventType, string>
                {
                    { FiscalIntegrationEventType.IncomeAccounts, "PAY" },
                    { FiscalIntegrationEventType.ExpenseAccounts, "PAY" },
                    { FiscalIntegrationEventType.SafeDrop, "TRANSFER" },
                    { FiscalIntegrationEventType.BankDrop, "TRANSFER" }
                };
            private const string AdvancePaymentType = "Adv";

            public IEnumerable<string> SupportedCountryRegions
            {
                get
                {
                    return new[]
                    {
                        nameof(CountryRegionISOCode.DE),
                    };
                }
            }

            public IEnumerable<Type> SupportedRequestTypes
            {
                get
                {
                    return new[]
                    {
                        typeof(GetEfrIsSalesTransactionDocumentGenerationRequiredRequest),
                        typeof(GetEfrSalesTransactionTotalAmountRequest),
                        typeof(GetEfrNonFiscalTransactionTypeRequest),
                        typeof(GetEfrCanСreateReceiptPositionsRequest),
                        typeof(GetEfrReceiptPaymentsRequest),
                        typeof(GetEfrDepositTenderLineRequest),
                        typeof(GetEfrSalesLinesRequest),
                        typeof(GetEfrSalesLineTaxGroupsRequest),
                        typeof(GetEfrReceiptTaxesRequest),
                        typeof(GetEfrCustomerAccountDepositTransactionTypeRequest),
                        typeof(GetEfrCustomerAccountDepositPositionsRequest),
                        typeof(GetEfrCustomerAccountDepositReceiptTaxesRequest),
                        typeof(GetEfrIncomeExpenseAccountsReceiptPaymentsRequest),
                    };
                }
            }

            public async Task<Response> Execute(Request request)
            {
                switch (request)
                {
                    case GetEfrIsSalesTransactionDocumentGenerationRequiredRequest getEfrIsSalesTransactionDocumentGenerationRequiredRequest:
                        return await GetEfrIsSalesTransactionDocumentGenerationRequired(getEfrIsSalesTransactionDocumentGenerationRequiredRequest).ConfigureAwait(false);
                    case GetEfrSalesTransactionTotalAmountRequest getEfrSalesTransactionTotalAmountRequest:
                        return await GetEfrSalesTransactionTotalAmount(getEfrSalesTransactionTotalAmountRequest).ConfigureAwait(false);
                    case GetEfrNonFiscalTransactionTypeRequest getEfrNonFiscalTransactionTypeRequest:
                        return await GetEfrNonFiscalTransactionType(getEfrNonFiscalTransactionTypeRequest).ConfigureAwait(false);
                    case GetEfrCanСreateReceiptPositionsRequest getEfrCanСreateReceiptPositionsRequest:
                        return await GetEfrCanСreateReceiptPositions(getEfrCanСreateReceiptPositionsRequest).ConfigureAwait(false);
                    case GetEfrReceiptPaymentsRequest getEfrReceiptPaymentsRequest:
                        return await GetEfrReceiptPayments(getEfrReceiptPaymentsRequest).ConfigureAwait(false);
                    case GetEfrSalesLinesRequest getEfrSalesLinesRequest:
                        return await GetEfrSalesLines(getEfrSalesLinesRequest).ConfigureAwait(false);
                    case GetEfrSalesLineTaxGroupsRequest getSalesLineTaxGroupRequest:
                        return await GetSalesLineTaxGroup(getSalesLineTaxGroupRequest).ConfigureAwait(false);
                    case GetEfrReceiptTaxesRequest getEfrReceiptTaxesRequest:
                        return await GetEfrReceiptTaxes(getEfrReceiptTaxesRequest).ConfigureAwait(false);
                    case GetEfrDepositTenderLineRequest getDepositTenderLineRequest:
                        return await GetDepositTenderLine(getDepositTenderLineRequest).ConfigureAwait(false);
                    case GetEfrCustomerAccountDepositTransactionTypeRequest getEfrCustomerAccountDepositTransactionTypeRequest:
                        return await GetEfrCustomerAccountDepositTransactionType(getEfrCustomerAccountDepositTransactionTypeRequest).ConfigureAwait(false);
                    case GetEfrCustomerAccountDepositPositionsRequest getEfrCustomerAccountDepositPositionsRequest:
                        return await GetEfrCustomerAccountDepositPositions(getEfrCustomerAccountDepositPositionsRequest).ConfigureAwait(false);
                    case GetEfrCustomerAccountDepositReceiptTaxesRequest getEfrCustomerAccountDepositReceiptTaxesRequest:
                        return await GetEfrCustomerAccountDepositReceiptTaxes(getEfrCustomerAccountDepositReceiptTaxesRequest).ConfigureAwait(false);
                    case GetEfrIncomeExpenseAccountsReceiptPaymentsRequest getIncomeExpenseAccountsReceiptPaymentsRequest:
                        return await GetIncomeExpenseAccountsReceiptPayments(getIncomeExpenseAccountsReceiptPaymentsRequest).ConfigureAwait(false);
                    default:
                        throw new NotSupportedException(string.Format("Request '{0}' is not supported.", request.GetType()));
                }
            }

            private static Task<Response> GetEfrIsSalesTransactionDocumentGenerationRequired(GetEfrIsSalesTransactionDocumentGenerationRequiredRequest request)
            {
                Response response = new SingleEntityDataServiceResponse<bool>(true);

                return Task.FromResult(response);
            }

            private static Task<Response> GetEfrSalesTransactionTotalAmount(GetEfrSalesTransactionTotalAmountRequest request)
            {
                Response response = new SingleEntityDataServiceResponse<decimal>(request.SalesOrder.CalcTotalPaymentAmount());

                return Task.FromResult(response);
            }

            private static Task<Response> GetEfrNonFiscalTransactionType(GetEfrNonFiscalTransactionTypeRequest request)
            {
                string nonFiscalTransactionType = string.Empty;

                if (NonFiscalTransactionTypeDictionary.TryGetValue(request.FiscalRegistrationEventType, out string transactionType))
                {
                    nonFiscalTransactionType = transactionType;
                }

                Response response = new SingleEntityDataServiceResponse<string>(nonFiscalTransactionType);

                return Task.FromResult(response);
            }

            private static Task<Response> GetEfrCanСreateReceiptPositions(GetEfrCanСreateReceiptPositionsRequest request)
            {
                Response response = new SingleEntityDataServiceResponse<bool>(true);
                return Task.FromResult(response);
            }

            private static async Task<Response> GetEfrReceiptPayments(GetEfrReceiptPaymentsRequest request)
            {
                Response response = null;
                List<ReceiptPayment> receiptPayments = await CreateReceiptPaymentsWithCurrencyAsync(request.RequestContext, request.SalesOrder, request.FiscalIntegrationFunctionalityProfile).ConfigureAwait(false);

                if (response == null)
                {
                    response = new SingleEntityDataServiceResponse<List<ReceiptPayment>>(receiptPayments);
                }

                return response;
            }

            private static Task<Response> GetEfrSalesLines(GetEfrSalesLinesRequest request)
            {
                IEnumerable<SalesLine> salesLines = null;
                var salesOrder = request.SalesOrder;

                if (salesOrder.IsDepositProcessing())
                {
                    salesLines = salesOrder.GetCarryOutLines(request.RequestContext);
                }
                else
                {
                    salesLines = salesOrder.GetNonGiftCardLines();
                }

                Response response = new SingleEntityDataServiceResponse<IEnumerable<SalesLine>>(salesLines);

                return Task.FromResult(response);
            }

            private static Task<Response> GetSalesLineTaxGroup(GetEfrSalesLineTaxGroupsRequest request)
            {
                var salesLine = request.SalesLine;
                string taxGroups = string.Empty;

                var taxExemptTypeGroup = ConfigurationController.GetExemptTaxGroup(request.FiscalIntegrationFunctionalityProfile);

                var taxCodes = salesLine.TaxLines.Select(taxLine => taxLine.IsExempt ? taxExemptTypeGroup
                        : request.FiscalIntegrationFunctionalityProfile.GetTaxGroupByRate(taxLine.Percentage));

                taxGroups = EfrCommonFunctions.JoinTaxCodes(taxCodes);

                Response response = new SingleEntityDataServiceResponse<string>(taxGroups);

                return Task.FromResult(response);
            }

            private static async Task<Response> GetDepositTenderLine(GetEfrDepositTenderLineRequest request)
            {
                var getChannelTenderTypesDataRequest = new GetChannelTenderTypesDataRequest(request.RequestContext.GetPrincipal().ChannelId, QueryResultSettings.AllRecords);
                var channelTenderTypes = (await request.RequestContext.ExecuteAsync<EntityDataServiceResponse<TenderType>>(getChannelTenderTypesDataRequest).ConfigureAwait(false)).PagedEntityCollection;

                var customerAccountTenderTypeId = channelTenderTypes.Single(c => c.OperationType == RetailOperation.PayCustomerAccount || c.OperationType == RetailOperation.PayCustomerAccountExact).TenderTypeId;
                var receiptPayment = new ReceiptPayment
                {
                    Description = await TranslateAsync(request.RequestContext, request.RequestContext.LanguageId, DataModelEFR.Constants.SalesTransactionLocalizationConstants.CustomerDeposit).ConfigureAwait(false),
                    Amount = request.SalesOrder.PrepaymentAmountAppliedOnPickup,
                    UniqueIdentifier = string.Empty,
                    PaymentTypeGroup = ConfigurationController.GetTenderTypeMapping(request.FiscalIntegrationFunctionalityProfile)[customerAccountTenderTypeId],
                };

                Response response = new SingleEntityDataServiceResponse<ReceiptPayment>(receiptPayment);

                return response;
            }

            

            private static async Task<Response> GetEfrReceiptTaxes(GetEfrReceiptTaxesRequest request)
            {
                IEnumerable<ReceiptTax> receiptTaxes = await GetEfrReceiptTaxes(request.RequestContext, request.SalesOrder, request.FiscalIntegrationFunctionalityProfile).ConfigureAwait(false);

                Response response = new SingleEntityDataServiceResponse<IEnumerable<ReceiptTax>>(receiptTaxes);

                return response;
            }

            private static Task<Response> GetEfrCustomerAccountDepositTransactionType(GetEfrCustomerAccountDepositTransactionTypeRequest request)
            {
                Response response = new SingleEntityDataServiceResponse<string>(string.Empty);

                return Task.FromResult(response);
            }

            private static async Task<Response> GetEfrCustomerAccountDepositPositions(GetEfrCustomerAccountDepositPositionsRequest request)
            {
                List<ReceiptPosition> receiptPositions = request.ReceiptPositions;
                var receiptPoisition = new ReceiptPosition()
                {
                    ItemNumber = request.SalesOrder.CustomerId,
                    PositionNumber = EfrCommonFunctions.GetDepositPositionNumber(receiptPositions),
                    Description = await TranslateAsync(request.RequestContext, request.RequestContext.LanguageId, DataModelEFR.Constants.SalesTransactionLocalizationConstants.Deposit).ConfigureAwait(false),
                    TaxGroup = ConfigurationController.GetDepositTaxGroup(request.FiscalIntegrationFunctionalityProfile),
                    Amount = request.SalesOrder.CustomerAccountDepositLines.Sum(c => c.Amount),
                    PositionType = AdvancePaymentType
                };

                receiptPositions.Add(receiptPoisition);

                return new SingleEntityDataServiceResponse<List<ReceiptPosition>>(receiptPositions);
            }

            private static Task<Response> GetEfrCustomerAccountDepositReceiptTaxes(GetEfrCustomerAccountDepositReceiptTaxesRequest request)
            {
                var receiptTaxes = new List<ReceiptTax>();
                Response response = new SingleEntityDataServiceResponse<List<ReceiptTax>>(receiptTaxes);
                return Task.FromResult(response);
            }

            private static async Task<Response> GetIncomeExpenseAccountsReceiptPayments(GetEfrIncomeExpenseAccountsReceiptPaymentsRequest request)
            {
                var result = new List<ReceiptPayment>();
                var tenderLinesGrpByTenderTypeIdAndCurrency = request.SalesOrder.ActiveTenderLines.GroupBy(tl => new { tl.TenderTypeId, tl.Currency });

                foreach (var g in tenderLinesGrpByTenderTypeIdAndCurrency)
                {
                    var receiptPayment = new ReceiptPayment
                    {
                        Description = await GetEfrTenderTypeName(request.RequestContext, g.Key.TenderTypeId).ConfigureAwait(false),
                        PaymentTypeGroup = request.FiscalIntegrationFunctionalityProfile.GetPaymentTypeGroup(g.Key.TenderTypeId),
                        Amount = g.Sum(l => l.Amount)
                    };

                    if (g.Key.Currency != request.SalesOrder.CurrencyCode)
                    {
                        receiptPayment.ForeignCurrencyCode = g.Key.Currency;
                        receiptPayment.ForeignAmount = g.Sum(tl => tl.AmountInTenderedCurrency);
                    }
                    result.Add(receiptPayment);
                }

                return new SingleEntityDataServiceResponse<List<ReceiptPayment>>(result);
            }

            private static async Task<IEnumerable<ReceiptTax>> GetEfrReceiptTaxes(RequestContext requestContext, SalesOrder salesOrder, FiscalIntegrationFunctionalityProfile fiscalIntegrationFunctionalityProfile)
            {
                List<ReceiptTax> receiptTaxes = await GetSalesOrderReceiptTaxes(requestContext, salesOrder, fiscalIntegrationFunctionalityProfile).ConfigureAwait(false);

                var receiptTaxesWithGiftCards = await GetGiftCardsReceiptTaxes(requestContext, salesOrder, fiscalIntegrationFunctionalityProfile).ConfigureAwait(false);

                return receiptTaxes.Concat(receiptTaxesWithGiftCards);
            }

            /// <summary>
            /// Gets receipt taxes for sales transaction.
            /// </summary>
            /// <returns>The receipt taxes collection.</returns>
            private static async Task<List<ReceiptTax>> GetSalesOrderReceiptTaxes(RequestContext requestContext, SalesOrder salesOrder, FiscalIntegrationFunctionalityProfile fiscalIntegrationFunctionalityProfile)
            {
                List<ReceiptTax> receiptTaxes = new List<ReceiptTax>();

                var salesLines = await GetSalesLines(requestContext, salesOrder).ConfigureAwait(false);
                var taxLinesGroupsByPercentageAndExemptSign = salesLines
                    .SelectMany(salesLine => salesLine.TaxLines.Select(tl => new
                    {
                        IsReturnLine = salesLine.IsReturnLine(),
                        TaxLine = tl
                    }))
                    .GroupBy(tl => new
                    {
                        Percentage = tl.TaxLine.IsExempt ? ExemptTaxPercentage : tl.TaxLine.Percentage,
                        tl.TaxLine.IsExempt
                    });

                foreach (var group in taxLinesGroupsByPercentageAndExemptSign)
                {
                    decimal netAmount = group.Sum(l => (l.IsReturnLine ? -1 : 1) * Math.Abs(l.TaxLine.TaxBasis));
                    decimal taxAmount = group.Key.IsExempt ? decimal.Zero : group.Sum(l => l.TaxLine.Amount);
                    receiptTaxes.Add(
                        new ReceiptTax
                        {
                            TaxGroup = group.Key.IsExempt
                                ? ConfigurationController.GetExemptTaxGroup(fiscalIntegrationFunctionalityProfile)
                                : fiscalIntegrationFunctionalityProfile.GetTaxGroupByRate(group.Key.Percentage),
                            TaxPercent = group.Key.Percentage,
                            NetAmount = netAmount,
                            TaxAmount = taxAmount,
                            GrossAmount = netAmount + taxAmount
                        });
                }

                await AddTaxesByCustomerOrderDeposit(receiptTaxes, requestContext, salesOrder, fiscalIntegrationFunctionalityProfile).ConfigureAwait(false);
                AddTaxLineForCustomerDepositByPrepayment(receiptTaxes, salesOrder, fiscalIntegrationFunctionalityProfile);

                return receiptTaxes;
            }

            /// <summary>
            /// Gets receipt taxes of gift card lines.
            /// </summary>
            /// <returns>The receipt taxes collection.</returns>
            private static async Task<ReceiptTax[]> GetGiftCardsReceiptTaxes(RequestContext requestContext, SalesOrder salesOrder, FiscalIntegrationFunctionalityProfile fiscalIntegrationFunctionalityProfile)
            {
                var filterOrders = salesOrder.ActiveSalesLines.Where(c => c.IsGiftCardLine);

                var receipTaxes = new List<ReceiptTax>();

                foreach (var sl in filterOrders)
                {
                    var lineTaxGroup = await GetSalesLineTaxGroups(requestContext, sl, fiscalIntegrationFunctionalityProfile).ConfigureAwait(false);
                    if (string.IsNullOrEmpty(lineTaxGroup))
                    {
                        lineTaxGroup = ConfigurationController.GetDepositTaxGroup(fiscalIntegrationFunctionalityProfile);
                    }

                    var tax = new ReceiptTax
                    {
                        TaxGroup = lineTaxGroup,
                        NetAmount = sl.NetAmount,
                        GrossAmount = sl.GrossAmount
                    };

                    receipTaxes.Add(tax);
                }

                return receipTaxes
                    .GroupBy(rt => rt.TaxGroup)
                    .Select(rtGrouped => new ReceiptTax
                    {
                        TaxGroup = rtGrouped.Key,
                        NetAmount = rtGrouped.Sum(r => r.NetAmount),
                        GrossAmount = rtGrouped.Sum(r => r.GrossAmount)
                    })
                    .ToArray();
            }

            /// <summary>
            /// Adds taxes for customer order deposit.
            /// </summary>
            /// <param name="receiptTaxes">The receipt taxes.</param>
            private static async Task AddTaxesByCustomerOrderDeposit(List<ReceiptTax> receiptTaxes, RequestContext requestContext, SalesOrder salesOrder, FiscalIntegrationFunctionalityProfile fiscalIntegrationFunctionalityProfile)
            {
                if (!salesOrder.IsDepositProcessing())
                {
                    return;
                }

                var depositSum = await CalculateDepositSumForOrder(requestContext, salesOrder).ConfigureAwait(false);

                if (depositSum != 0)
                {
                    receiptTaxes.Add(new ReceiptTax
                    {
                        TaxGroup = ConfigurationController.GetDepositTaxGroup(fiscalIntegrationFunctionalityProfile),
                        NetAmount = depositSum,
                        TaxAmount = 0, //Tax amount is always 0, because non-zero taxes are not supported in DEU localization.
                        GrossAmount = depositSum
                    });
                }
            }

            /// <summary>
            /// Adds tax line for customer deposit by prepayment.
            /// </summary>
            /// <param name="receiptTaxes">The receipt taxes.</param>
            private static void AddTaxLineForCustomerDepositByPrepayment(List<ReceiptTax> receiptTaxes, SalesOrder salesOrder, FiscalIntegrationFunctionalityProfile fiscalIntegrationFunctionalityProfile)
            {
                if (salesOrder.PrepaymentAmountAppliedOnPickup == 0)
                {
                    return;
                }

                receiptTaxes.Add(new ReceiptTax
                {
                    TaxGroup = ConfigurationController.GetDepositTaxGroup(fiscalIntegrationFunctionalityProfile),
                    NetAmount = salesOrder.PrepaymentAmountAppliedOnPickup * -1,
                    TaxAmount = 0, //Tax amount is always 0, because non-zero taxes are not supported in DEU localization.
                    GrossAmount = salesOrder.PrepaymentAmountAppliedOnPickup * -1
                });
            }

            /// <summary>
            /// Calculates the deposit sum for sales order.
            /// </summary>
            /// <returns>The deposit sum.</returns>
            private static async Task<decimal> CalculateDepositSumForOrder(RequestContext requestContext, SalesOrder salesOrder)
            {
                var request = new GetEfrDepositOrderSumRequest(salesOrder);
                return (await requestContext.ExecuteAsync<SingleEntityDataServiceResponse<decimal>>(request).ConfigureAwait(false)).Entity;
            }

            /// <summary>
            /// Creates receipt payments with currency information.
            /// </summary>
            /// <returns>The receipt payments.</returns>
            private static async Task<List<ReceiptPayment>> CreateReceiptPaymentsWithCurrencyAsync(RequestContext requestContext, SalesOrder salesOrder, FiscalIntegrationFunctionalityProfile fiscalIntegrationFunctionalityProfile)
            {
                List<ReceiptPayment> receiptPayments = new List<ReceiptPayment>();
                var tenderLines = salesOrder.ActiveTenderLines;

                var getChannelTenderTypesDataRequest = new GetChannelTenderTypesDataRequest(requestContext.GetPrincipal().ChannelId, QueryResultSettings.AllRecords);
                var channelTenderTypes = (await requestContext.ExecuteAsync<EntityDataServiceResponse<TenderType>>(getChannelTenderTypesDataRequest).ConfigureAwait(false)).PagedEntityCollection;
                var giftCardTenderTypeIds = channelTenderTypes.Where(c => c.OperationType == RetailOperation.PayGiftCertificate || c.OperationType == RetailOperation.PayGiftCardExact).Select(c => c.TenderTypeId);

                foreach (var line in tenderLines)
                {
                    var payment = new ReceiptPayment
                    {
                        Description = await GetEfrTenderTypeName(requestContext, line.TenderTypeId).ConfigureAwait(false),
                        Amount = line.Amount,
                        UniqueIdentifier = line.CreditMemoId,
                        PaymentTypeGroup = ConfigurationController.GetTenderTypeMapping(fiscalIntegrationFunctionalityProfile)[line.TenderTypeId]
                    };

                    if (salesOrder.CurrencyCode != line.Currency)
                    {
                        payment.ForeignCurrencyCode = line.Currency;
                        payment.ForeignAmount = line.AmountInTenderedCurrency;
                    }

                    if (giftCardTenderTypeIds.Contains(line.TenderTypeId))
                    {
                        payment.UniqueIdentifier = line.GiftCardId;
                    }
                    receiptPayments.Add(payment);
                }

                return receiptPayments;
            }

            private static async Task<string> GetSalesLineTaxGroups(RequestContext requestContext, SalesLine salesLine, FiscalIntegrationFunctionalityProfile fiscalIntegrationFunctionalityProfile)
            {
                var request = new GetEfrSalesLineTaxGroupsRequest(salesLine, fiscalIntegrationFunctionalityProfile);
                return (await requestContext.ExecuteAsync<SingleEntityDataServiceResponse<string>>(request).ConfigureAwait(false)).Entity;
            }

            private static async Task<IEnumerable<SalesLine>> GetSalesLines(RequestContext requestContext, SalesOrder salesOrder)
            {
                var request = new GetEfrSalesLinesRequest(salesOrder);
                return (await requestContext.ExecuteAsync<SingleEntityDataServiceResponse<IEnumerable<SalesLine>>>(request).ConfigureAwait(false)).Entity;
            }
            
            private static async Task<string> TranslateAsync(RequestContext requestContext, string cultureName, string textId)
            {
                return (await requestContext.ExecuteAsync<SingleEntityDataServiceResponse<string>>(new LocalizeEfrResourceRequest(cultureName, textId)).ConfigureAwait(false)).Entity;
            }

            private static async Task<string> GetEfrTenderTypeName(RequestContext requestContext, string tenderTypeId)
            {
                var request = new GetEfrGetTenderTypeNameRequest(tenderTypeId);
                return (await requestContext.ExecuteAsync<SingleEntityDataServiceResponse<string>>(request).ConfigureAwait(false)).Entity;
            }
        }
    }
}
