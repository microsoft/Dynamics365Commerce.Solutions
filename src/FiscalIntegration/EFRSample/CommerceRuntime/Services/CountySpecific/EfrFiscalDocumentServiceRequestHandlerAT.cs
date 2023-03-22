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
        using Contoso.CommerceRuntime.DocumentProvider.DataModelEFR.Constants;
        using Contoso.CommerceRuntime.DocumentProvider.DataModelEFR.Documents;
        using Contoso.CommerceRuntime.DocumentProvider.EFRSample.DocumentHelpers;
        using Contoso.CommerceRuntime.DocumentProvider.EFRSample.Extensions;
        using Contoso.CommerceRuntime.DocumentProvider.EFRSample.Messages;
        using Contoso.CommerceRuntime.DocumentProvider.EFRSample.Messages.Austria;

        public sealed class EfrFiscalDocumentServiceRequestHandlerAT : IRequestHandlerAsync, ICountryRegionAware
        {
            private const decimal ExemptedTaxPercentage = decimal.Zero;

            public IEnumerable<string> SupportedCountryRegions
            {
                get
                {
                    return new[]
                    {
                        nameof(CountryRegionISOCode.AT),
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
                        typeof(GetSalesOrderAdjustmentTypeRequest),
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
                    case GetSalesOrderAdjustmentTypeRequest getSalesOrderAdjustmentTypeRequest:
                        return await GetSalesOrderAdjustmentType(getSalesOrderAdjustmentTypeRequest).ConfigureAwait(false);
                    default:
                        throw new NotSupportedException(string.Format("Request '{0}' is not supported.", request.GetType()));
                }
            }

            private static Task<Response> GetEfrIsSalesTransactionDocumentGenerationRequired(GetEfrIsSalesTransactionDocumentGenerationRequiredRequest request)
            {
                var isSalesTransactionDocumentGenerationRequired = true;

                if (request.SalesOrder.IsDepositProcessing())
                {
                    // Checks if there is any payments to be registered (e.g. line comment or zero deposit) and can be avoided
                    isSalesTransactionDocumentGenerationRequired = request.SalesOrder.ActiveTenderLines.Count != 0;
                }

                Response response = new SingleEntityDataServiceResponse<bool>(isSalesTransactionDocumentGenerationRequired);

                return Task.FromResult(response);
            }

            private static Task<Response> GetEfrSalesTransactionTotalAmount(GetEfrSalesTransactionTotalAmountRequest request)
            {
                Response response = new SingleEntityDataServiceResponse<decimal>(request.SalesOrder.CalcTotalPaymentAmountWithPrepayment());

                return Task.FromResult(response);
            }

            private static async Task<Response> GetEfrNonFiscalTransactionType(GetEfrNonFiscalTransactionTypeRequest request)
            {
                var salesOrder = request.SalesOrder;
                var requestContext = request.RequestContext;

                var nonFiscalTransactionTypeMessage = salesOrder.NonFiscalTransactionType(requestContext);
                string nonFiscalTransactionType = await TranslateAsync(requestContext, requestContext.LanguageId, nonFiscalTransactionTypeMessage).ConfigureAwait(false);

                return new SingleEntityDataServiceResponse<string>(nonFiscalTransactionType);
            }

            private static Task<Response> GetEfrCanСreateReceiptPositions(GetEfrCanСreateReceiptPositionsRequest request)
            {
                var salesOrder = request.SalesOrder;
                var canСreateReceiptPositions = !(salesOrder.IsDepositProcessing() && salesOrder.HasDepositLinesOnly(request.RequestContext));
                Response response = new SingleEntityDataServiceResponse<bool>(canСreateReceiptPositions);
                return Task.FromResult(response);
            }

            private static async Task<Response> GetEfrReceiptPayments(GetEfrReceiptPaymentsRequest request)
            {
                List<ReceiptPayment> receiptPayments = await CreateReceiptPaymentsWithoutCurrencyAsync(request.RequestContext, request.SalesOrder, request.FiscalIntegrationFunctionalityProfile).ConfigureAwait(false);

                return new SingleEntityDataServiceResponse<List<ReceiptPayment>>(receiptPayments);
            }

            private static Task<Response> GetEfrSalesLines(GetEfrSalesLinesRequest request)
            {
                var salesOrder = request.SalesOrder;
                IEnumerable<SalesLine> salesLines = salesOrder.SalesLinesWithNonZeroQuantity();
                Response response = new SingleEntityDataServiceResponse<IEnumerable<SalesLine>>(salesLines);
                return Task.FromResult(response);
            }

            private static Task<Response> GetSalesLineTaxGroup(GetEfrSalesLineTaxGroupsRequest request)
            {
                var salesLine = request.SalesLine;
                string taxGroups = string.Empty;

                var taxCodes = salesLine.TaxLines.Select(taxLine => taxLine.IsExempt 
                    ? request.FiscalIntegrationFunctionalityProfile.GetTaxGroupByRate(ExemptedTaxPercentage)
                    : request.FiscalIntegrationFunctionalityProfile.GetTaxGroupByRate(taxLine.Percentage));

                taxGroups = EfrCommonFunctions.JoinTaxCodes(taxCodes);

                Response response = new SingleEntityDataServiceResponse<string>(taxGroups);

                return Task.FromResult(response);
            }

            private static async Task<Response> GetDepositTenderLine(GetEfrDepositTenderLineRequest request)
            {
                var receiptPayment = new ReceiptPayment
                {
                    Description = await TranslateAsync(request.RequestContext, request.RequestContext.LanguageId, DataModelEFR.Constants.SalesTransactionLocalizationConstants.CustomerDeposit).ConfigureAwait(false),
                    Amount = request.SalesOrder.PrepaymentAmountAppliedOnPickup,
                    UniqueIdentifier = string.Empty,
                    PaymentTypeGroup = string.Empty
                };

                Response response = new SingleEntityDataServiceResponse<ReceiptPayment>(receiptPayment);

                return response;
            }

            private static async Task<Response> GetEfrCustomerAccountDepositTransactionType(GetEfrCustomerAccountDepositTransactionTypeRequest request)
            {
                var message = await TranslateAsync(request.RequestContext, request.RequestContext.LanguageId, SalesTransactionLocalizationConstants.Deposit).ConfigureAwait(false);
                return new SingleEntityDataServiceResponse<string>(message);
            }

            private static Task<Response> GetEfrCustomerAccountDepositPositions(GetEfrCustomerAccountDepositPositionsRequest request)
            {
                Response response = new SingleEntityDataServiceResponse<List<ReceiptPosition>>(request.ReceiptPositions);
                return Task.FromResult(response);
            }            

            private static Task<Response> GetEfrCustomerAccountDepositReceiptTaxes(GetEfrCustomerAccountDepositReceiptTaxesRequest request)
            {
                var receiptTaxes = new List<ReceiptTax>();
                Response response = new SingleEntityDataServiceResponse<List<ReceiptTax>>(receiptTaxes);
                return Task.FromResult(response);
            }

            private static Task<Response> GetSalesOrderAdjustmentType(GetSalesOrderAdjustmentTypeRequest request)
            {
                var salesOrder = request.SalesOrder;
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

                Response response = new SingleEntityDataServiceResponse<FiscalIntegrationSalesOrderAdjustmentType>(adjustmentType);
                return Task.FromResult(response);
            }

            private static async Task<Response> GetEfrReceiptTaxes(GetEfrReceiptTaxesRequest request)
            {
                List<ReceiptTax> receiptTaxes = await GetEfrReceiptTaxes(request.RequestContext, request.SalesOrder, request.FiscalIntegrationFunctionalityProfile).ConfigureAwait(false);

                Response response = new SingleEntityDataServiceResponse<IEnumerable<ReceiptTax>>(receiptTaxes);

                return response;
            }

            private static async Task<List<ReceiptTax>> GetEfrReceiptTaxes(RequestContext requestContext, SalesOrder salesOrder, FiscalIntegrationFunctionalityProfile fiscalIntegrationFunctionalityProfile)
            {
                List<ReceiptTax> receiptTaxes = new List<ReceiptTax>();

                if (salesOrder.IsDepositProcessing() && salesOrder.HasDepositLinesOnly(requestContext))
                {
                    return receiptTaxes;
                }


                var salesLines = await GetSalesLines(requestContext, salesOrder).ConfigureAwait(false);
                var taxLinesGrpByPercentage = salesLines
                    .SelectMany(salesLine => salesLine.TaxLines.Select(tl =>
                    new
                    {
                        IsReturnLine = salesLine.IsReturnLine(),
                        TaxLine = tl
                    }))
                    .GroupBy(tl =>
                        tl.TaxLine.IsExempt ? ExemptedTaxPercentage : tl.TaxLine.Percentage
                    );

                foreach (var taxLine in taxLinesGrpByPercentage)
                {
                    decimal netAmount = taxLine.Sum(l => (l.IsReturnLine ? -1 : 1) * Math.Abs(l.TaxLine.TaxBasis));
                    decimal taxAmount = taxLine.Sum(l => l.TaxLine.IsExempt ? decimal.Zero : l.TaxLine.Amount);
                    receiptTaxes.Add(
                        new ReceiptTax
                        {
                            TaxGroup = fiscalIntegrationFunctionalityProfile.GetTaxGroupByRate(taxLine.Key),
                            TaxPercent = taxLine.Key,
                            NetAmount = netAmount,
                            TaxAmount = taxAmount,
                            GrossAmount = netAmount + taxAmount
                        });
                }

                return receiptTaxes;
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

            private static async Task<IEnumerable<SalesLine>> GetSalesLines(RequestContext requestContext, SalesOrder salesOrder)
            {
                var request = new GetEfrSalesLinesRequest(salesOrder);
                return (await requestContext.ExecuteAsync<SingleEntityDataServiceResponse<IEnumerable<SalesLine>>>(request).ConfigureAwait(false)).Entity;
            }

            private static async Task<string> TranslateAsync(RequestContext requestContext, string cultureName, string textId)
            {
                return (await requestContext.ExecuteAsync<SingleEntityDataServiceResponse<string>>(new LocalizeEfrResourceRequest(cultureName, textId)).ConfigureAwait(false)).Entity;
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

            /// <summary>
            /// Gets tax groups from sales line.
            /// </summary>
            /// <param name="functionalityProfile">The functionality profile.</param>
            /// <param name="salesLine">The sales line.</param>
            /// <returns>The tax groups.</returns>
            private static IEnumerable<string> GetTaxGroupsFromSalesLine(FiscalIntegrationFunctionalityProfile functionalityProfile, SalesLine salesLine)
            {
                ThrowIf.Null(salesLine, nameof(salesLine));

                TaxRatesMapping taxRatesMapping = functionalityProfile.GetTaxRatesMapping();
                return salesLine.TaxLines.Select(taxLine => taxRatesMapping[taxLine.Percentage]);
            }

            private static async Task<string> GetEfrTenderTypeName(RequestContext requestContext, string tenderTypeId)
            {
                var request = new GetEfrGetTenderTypeNameRequest(tenderTypeId);
                return (await requestContext.ExecuteAsync<SingleEntityDataServiceResponse<string>>(request).ConfigureAwait(false)).Entity;
            }
        }
    }
}
