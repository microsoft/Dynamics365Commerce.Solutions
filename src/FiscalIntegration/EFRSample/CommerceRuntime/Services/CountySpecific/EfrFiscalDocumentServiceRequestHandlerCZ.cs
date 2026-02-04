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
        using System.Collections.ObjectModel;
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

        public sealed class EfrFiscalDocumentServiceRequestHandlerCZ : IRequestHandlerAsync, ICountryRegionAware
        {            
            private const int PositionCzField = 23;
            private const int PaymentCzField = 24;
            private const int RoundPrecision = 2;

            public IEnumerable<string> SupportedCountryRegions
            {
                get
                {
                    return new[]
                    {
                        nameof(CountryRegionISOCode.CZ),
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
                Response response = new SingleEntityDataServiceResponse<decimal>(request.SalesOrder.CalcTotalPaymentAmountWithPrepayment());

                return Task.FromResult(response);
            }

            private static Task<Response> GetEfrNonFiscalTransactionType(GetEfrNonFiscalTransactionTypeRequest request)
            {
                string nonFiscalTransactionType = string.Empty;

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
                List<ReceiptPayment> receiptPayments = await CreateReceiptPaymentsWithoutCurrencyAsync(request.RequestContext, request.SalesOrder, request.FiscalIntegrationFunctionalityProfile).ConfigureAwait(false);

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

                var taxGroups = EfrCommonFunctions.JoinTaxCodes(GetTaxGroupsFromSalesLineWithDefault(request.FiscalIntegrationFunctionalityProfile, salesLine));

                Response response = new SingleEntityDataServiceResponse<string>(taxGroups);

                return Task.FromResult(response);
            }

            private static async Task<Response> GetDepositTenderLine(GetEfrDepositTenderLineRequest request)
            {
                var receiptPayment = new ReceiptPayment
                {
                    Description = await TranslateAsync(request.RequestContext, request.RequestContext.LanguageId, DataModelEFR.Constants.SalesTransactionLocalizationConstants.Deposit).ConfigureAwait(false),
                    Amount = request.SalesOrder.PrepaymentAmountAppliedOnPickup,
                    UniqueIdentifier = string.Empty,
                    PaymentTypeGroup = string.Empty,
                    CZField = PaymentCzField
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
                    Description = await TranslateAsync(request.RequestContext, request.RequestContext.LanguageId, DataModelEFR.Constants.SalesTransactionLocalizationConstants.Deposit).ConfigureAwait(false),
                    Quantity = 1,
                    TaxGroup = ConfigurationController.GetDepositTaxGroup(request.FiscalIntegrationFunctionalityProfile),
                    CZField = PositionCzField,
                    Amount = request.SalesOrder.CustomerAccountDepositLines.Sum(c => c.Amount),
                    UnitPrice = request.SalesOrder.CustomerAccountDepositLines.Sum(c => c.Amount),
                };
                receiptPositions.Add(receiptPoisition);

                return new SingleEntityDataServiceResponse<List<ReceiptPosition>>(receiptPositions);
            }

            private static async Task<Response> GetEfrCustomerAccountDepositReceiptTaxes(GetEfrCustomerAccountDepositReceiptTaxesRequest request)
            {
                var receiptTaxes = new List<ReceiptTax>();

                var depositTaxGroup = GetDepositTaxGroup(request.FiscalIntegrationFunctionalityProfile);
                var depositSum = await CalculateDepositSumForOrder(request.RequestContext, request.SalesOrder).ConfigureAwait(false);

                if (depositSum > 0)
                {
                    var taxAmount = CalculateDepositTaxAmount(depositSum, depositTaxGroup.Item1);
                    receiptTaxes.Add(
                        new ReceiptTax
                        {
                            TaxGroup = depositTaxGroup.Item2,
                            TaxPercent = depositTaxGroup.Item1,
                            NetAmount = depositSum - taxAmount,
                            TaxAmount = taxAmount,
                            GrossAmount = depositSum
                        }
                    );
                }

                return new SingleEntityDataServiceResponse<List<ReceiptTax>>(receiptTaxes);
            }

            private static async Task<List<ReceiptTax>> GetEfrReceiptTaxes(RequestContext requestContext, SalesOrder salesOrder, FiscalIntegrationFunctionalityProfile fiscalIntegrationFunctionalityProfile)
            {
                List<ReceiptTax> receiptTaxes = new List<ReceiptTax>();

                var salesLines = await GetSalesLines(requestContext, salesOrder).ConfigureAwait(false);
                receiptTaxes = GetTaxes(salesLines, fiscalIntegrationFunctionalityProfile);

                var depositTaxGroup = GetDepositTaxGroup(fiscalIntegrationFunctionalityProfile);
                var depositSum = await CalculateDepositSumForOrder(requestContext, salesOrder).ConfigureAwait(false);

                if (salesOrder.IsDepositProcessing() && depositSum != 0)
                {
                    receiptTaxes.Add(CreateReceiptTax(depositSum, depositTaxGroup));
                }

                foreach (var giftCardLine in salesOrder.ActiveSalesLines.Where(c => c.IsGiftCardLine))
                {
                    if (giftCardLine.TaxLines.Any())
                    {
                        receiptTaxes.AddRange(GetTaxes(new SalesLine[] { giftCardLine }, fiscalIntegrationFunctionalityProfile));
                    }
                    else
                    {
                        receiptTaxes.Add(CreateReceiptTax(giftCardLine.TotalAmount, depositTaxGroup));
                    }
                }

                return receiptTaxes.GroupBy(c => new { c.TaxGroup, c.TaxPercent })
                    .Select(c => new ReceiptTax
                    {
                        TaxGroup = c.Key.TaxGroup,
                        TaxPercent = c.Key.TaxPercent,
                        NetAmount = c.Sum(b => b.NetAmount),
                        TaxAmount = c.Sum(b => b.TaxAmount),
                        GrossAmount = c.Sum(b => b.GrossAmount)
                    }).ToList();
            }

            private static ReceiptTax CreateReceiptTax(decimal totalAmount, Tuple<decimal, string> depositTax)
            {
                var taxAmount = CalculateDepositTaxAmount(totalAmount, depositTax.Item1);
                return new ReceiptTax
                {
                    TaxGroup = depositTax.Item2,
                    TaxPercent = depositTax.Item1,
                    NetAmount = totalAmount - taxAmount,
                    TaxAmount = taxAmount,
                    GrossAmount = totalAmount
                };
            }

            /// <summary>
            /// Transforms the collection of sales lines to receipt taxes.
            /// </summary>
            /// <param name="salesLineCollection">The collection of sales lines.</param>
            /// <returns>The collection of receipt taxes.</returns>
            private static List<ReceiptTax> GetTaxes(IEnumerable<SalesLine> salesLineCollection, FiscalIntegrationFunctionalityProfile fiscalIntegrationFunctionalityProfile)
            {
                var taxLinesGrpByPercentage = salesLineCollection
                    .SelectMany(salesLine => salesLine.TaxLines.Select(tl =>
                    new
                    {
                        IsReturnLine = salesLine.IsReturnLine(),
                        TaxLine = tl
                    }))
                    .GroupBy(tl =>
                        tl.TaxLine.Percentage
                    );

                return taxLinesGrpByPercentage.Select(group =>
                    new ReceiptTax
                    {
                        TaxGroup = GetTaxGroupByRateOrDefault(fiscalIntegrationFunctionalityProfile, group.Key),
                        TaxPercent = group.Key,
                        NetAmount = group.Sum(x => (x.IsReturnLine ? -1 : 1) * Math.Abs(x.TaxLine.TaxBasis)),
                        TaxAmount = group.Sum(x => x.TaxLine.Amount),
                        GrossAmount = group.Sum(x => (x.IsReturnLine ? -1 : 1) * Math.Abs(x.TaxLine.TaxBasis) + x.TaxLine.Amount)
                    }
                ).ToList();
            }

            /// <summary>
            /// Creates receipt payments without currency information.
            /// </summary>
            /// <returns>The receipt payments.</returns>
            private static async Task<List<ReceiptPayment>> CreateReceiptPaymentsWithoutCurrencyAsync(RequestContext requestContext, SalesOrder salesOrder, FiscalIntegrationFunctionalityProfile fiscalIntegrationFunctionalityProfile)
            {
                List<ReceiptPayment> receiptPayments = new List<ReceiptPayment>();

                var tenderLinesGrpByTenderTypeId = salesOrder.TenderLinesGrpByTenderTypeId();

                var getChannelTenderTypesDataRequest = new GetChannelTenderTypesDataRequest(requestContext.GetPrincipal().ChannelId, QueryResultSettings.AllRecords);
                ReadOnlyCollection<TenderType> channelTenderTypes = (await requestContext.ExecuteAsync<EntityDataServiceResponse<TenderType>>(getChannelTenderTypesDataRequest).ConfigureAwait(false)).PagedEntityCollection.Results;
                var payCustomerAccountTypes = channelTenderTypes.Where(c => c.OperationType == RetailOperation.PayCustomerAccount || c.OperationType == RetailOperation.PayCustomerAccountExact);

                foreach (var tenderLine in tenderLinesGrpByTenderTypeId)
                {
                    var payment = new ReceiptPayment
                    {
                        Description = await GetEfrTenderTypeName(requestContext, tenderLine.Key).ConfigureAwait(false),
                        Amount = tenderLine.Sum(l => l.Amount),
                        UniqueIdentifier = string.Empty,
                        PaymentTypeGroup = string.Empty
                    };

                    if (tenderLine.Any(c => !string.IsNullOrWhiteSpace(c.GiftCardId)) || payCustomerAccountTypes.Any(c => c.TenderTypeId == tenderLine.Key))
                    {
                        await PopulateEfrLocalizationInfoToPayment(requestContext, payment).ConfigureAwait(false);
                    }
                    receiptPayments.Add(payment);
                }

                if (salesOrder.PrepaymentAmountAppliedOnPickup != 0)
                {
                    var depositTenderLines = await GetDepositTenderLine(requestContext, salesOrder, fiscalIntegrationFunctionalityProfile).ConfigureAwait(false);
                    receiptPayments.Add(depositTenderLines);
                }

                return receiptPayments;
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

            private static async Task<IEnumerable<SalesLine>> GetSalesLines(RequestContext requestContext, SalesOrder salesOrder)
            {
                var request = new GetEfrSalesLinesRequest(salesOrder);
                return (await requestContext.ExecuteAsync<SingleEntityDataServiceResponse<IEnumerable<SalesLine>>>(request).ConfigureAwait(false)).Entity;
            }

            private static async Task<ReceiptPayment> PopulateEfrLocalizationInfoToPayment(RequestContext requestContext, ReceiptPayment receiptPayment)
            {
                var request = new PopulateEfrLocalizationInfoToPaymentRequest(receiptPayment);
                return (await requestContext.ExecuteAsync<SingleEntityDataServiceResponse<ReceiptPayment>>(request).ConfigureAwait(false)).Entity;
            }

            private static async Task<ReceiptPayment> GetDepositTenderLine(RequestContext requestContext, SalesOrder salesOrder, FiscalIntegrationFunctionalityProfile fiscalIntegrationFunctionalityProfile)
            {
                var request = new GetEfrDepositTenderLineRequest(salesOrder, fiscalIntegrationFunctionalityProfile);
                return (await requestContext.ExecuteAsync<SingleEntityDataServiceResponse<ReceiptPayment>>(request).ConfigureAwait(false)).Entity;
            }

            private static async Task<string> TranslateAsync(RequestContext requestContext, string cultureName, string textId)
            {
                return (await requestContext.ExecuteAsync<SingleEntityDataServiceResponse<string>>(new LocalizeEfrResourceRequest(cultureName, textId)).ConfigureAwait(false)).Entity;
            }

            /// <summary>
            /// Gets tax groups from sales line.
            /// </summary>
            /// <param name="functionalityProfile">The functionality profile.</param>
            /// <param name="salesLine">The sales line.</param>
            /// <returns>The tax groups.</returns>
            public static Tuple<decimal, string> GetDepositTaxGroup(FiscalIntegrationFunctionalityProfile functionalityProfile)
            {
                TaxRatesMapping taxRatesMapping = functionalityProfile.GetTaxRatesMapping();
                string depositGroup = ConfigurationController.GetDepositTaxGroup(functionalityProfile);
                if (!taxRatesMapping.ContainsTax(depositGroup)) throw new DataValidationException(DataValidationErrors.Microsoft_Dynamics_Commerce_Runtime_RequiredValueNotFound, "Deposit tax is not set");
                return new Tuple<decimal, string>(taxRatesMapping.GetSignleRateByName(depositGroup), depositGroup);
            }

            /// <summary>
            /// Gets tax groups from sales line.
            /// </summary>
            /// <param name="functionalityProfile">The functionality profile.</param>
            /// <param name="salesLine">The sales line.</param>
            /// <returns>The tax groups.</returns>
            private static IEnumerable<string> GetTaxGroupsFromSalesLineWithDefault(FiscalIntegrationFunctionalityProfile functionalityProfile, SalesLine salesLine)
            {
                ThrowIf.Null(salesLine, nameof(salesLine));

                TaxRatesMapping taxRatesMapping = functionalityProfile.GetTaxRatesMapping();
                string defaultGroup = ConfigurationController.GetDefaultTaxGroup(functionalityProfile);
                if (!taxRatesMapping.ContainsTax(defaultGroup)) throw new DataValidationException(DataValidationErrors.Microsoft_Dynamics_Commerce_Runtime_RequiredValueNotFound, "Default tax is not set");
                return salesLine.TaxLines
                        .Select(taxLine =>
                        {
                            var group = taxRatesMapping[taxLine.Percentage];
                            if (string.IsNullOrWhiteSpace(group)) return defaultGroup;
                            return group;
                        }).Distinct();
            }

            /// <summary>
            /// Gets tax group by tax percentage.
            /// </summary>
            /// <param name="functionalityProfile">The functionality profile.</param>
            /// <param name="percentage">The tax rate.</param>
            /// <returns>The tax groups.</returns>
            private static string GetTaxGroupByRateOrDefault(FiscalIntegrationFunctionalityProfile functionalityProfile, decimal percentage)
            {
                var taxRatesMapping = functionalityProfile.GetTaxRatesMapping();
                var group = taxRatesMapping[percentage];
                if (string.IsNullOrWhiteSpace(group))
                {
                    string defaultGroup = ConfigurationController.GetDefaultTaxGroup(functionalityProfile);
                    if (!taxRatesMapping.ContainsTax(defaultGroup)) throw new DataValidationException(DataValidationErrors.Microsoft_Dynamics_Commerce_Runtime_RequiredValueNotFound, "Default tax is not set");
                    return defaultGroup;
                }
                return group;
            }

            /// <summary>
            /// Calculates the amount of deposit tax.
            /// </summary>
            /// <param name="amount">The amount.</param>
            /// <param name="taxRange">The tax range.</param>
            /// <param name="roundPrecision">The round precision.</param>
            /// <returns>The tax amount value.</returns>
            private static decimal CalculateDepositTaxAmount(decimal amount, decimal taxRange)
            {
                return Math.Round(amount * taxRange / 100, RoundPrecision);
            }

            private static async Task<string> GetEfrTenderTypeName(RequestContext requestContext, string tenderTypeId)
            {
                var request = new GetEfrGetTenderTypeNameRequest(tenderTypeId);
                return (await requestContext.ExecuteAsync<SingleEntityDataServiceResponse<string>>(request).ConfigureAwait(false)).Entity;
            }
        }
    }
}
