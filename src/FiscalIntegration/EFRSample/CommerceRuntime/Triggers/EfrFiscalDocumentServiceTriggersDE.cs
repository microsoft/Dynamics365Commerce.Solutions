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
    namespace CommerceRuntime.DocumentProvider.EFRSample.Triggers
    {
        using System;
        using System.Collections.Generic;
        using System.Linq;
        using System.Threading.Tasks;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;
        using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;
        using Contoso.CommerceRuntime.DocumentProvider.DataModelEFR.Documents;
        using Contoso.CommerceRuntime.DocumentProvider.EFRSample.DocumentHelpers;
        using Contoso.CommerceRuntime.DocumentProvider.EFRSample.Extensions;
        using Contoso.CommerceRuntime.DocumentProvider.EFRSample.Messages;

        public sealed class EfrFiscalDocumentServiceTriggersDE : ICountryRegionAware, IRequestTriggerAsync
        {
            private const string AdvancePaymentType = "Adv";
            private const string VoucherType = "Vou";

            public IEnumerable<Type> SupportedRequestTypes
            {
                get
                {
                    return new[]
                    {
                        typeof(PopulateEfrReferenceFieldsRequest),
                        typeof(PopulateEfrLocalizationInfoRequest),
                        typeof(PopulateEfrCustomerDataRequest),
                        typeof(PopulateCountryRegionSpecificPositionsRequest),
                        typeof(GetExtensionPackageDefinitionsRequest),
                    };
                }
            }

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

            public async Task OnExecuted(Request request, Response response)
            {
                switch (request)
                {
                    case PopulateEfrReferenceFieldsRequest populateReferenceFieldsRequest:
                        await PopulateReferenceFields(populateReferenceFieldsRequest, (SingleEntityDataServiceResponse<ReceiptPosition>)response).ConfigureAwait(false);
                        break;
                    case PopulateEfrLocalizationInfoRequest populateEfrLocalizationInfoRequest:
                        PopulateEfrLocalizationInfo(populateEfrLocalizationInfoRequest, (SingleEntityDataServiceResponse<DataModelEFR.Documents.Receipt>)response);
                        break;
                    case PopulateEfrCustomerDataRequest populateEfrCustomerDataRequest:
                        await PopulateEfrCustomerData(populateEfrCustomerDataRequest, (SingleEntityDataServiceResponse<DataModelEFR.Documents.Receipt>)response).ConfigureAwait(false);
                        break;
                    case PopulateCountryRegionSpecificPositionsRequest populateCountryRegionSpecificPositionsRequest:
                        await PopulateCountryRegionSpecificPositions(populateCountryRegionSpecificPositionsRequest, (SingleEntityDataServiceResponse<List<ReceiptPosition>>)response).ConfigureAwait(false);
                        break;
                    case GetExtensionPackageDefinitionsRequest getExtensionPackageDefinitionsRequest:
                        GetExtensionPackageDefinitions(getExtensionPackageDefinitionsRequest, response);
                        break;
                }
            }

            public Task OnExecuting(Request request)
            {
                return Task.CompletedTask;
            }

            private static async Task PopulateReferenceFields(PopulateEfrReferenceFieldsRequest request, SingleEntityDataServiceResponse<ReceiptPosition> response)
            {
                var receiptPosition = response.Entity;
                var originSalesOrderResponse = await request.SalesLine.GetOriginSalesOrderAsync(request.RequestContext).ConfigureAwait(false);
                SetReferenceDate(receiptPosition, originSalesOrderResponse);
                receiptPosition.Void = true;
            }

            /// <summary>
            /// Sets reference registration date in EFR of the original transaction.
            /// </summary>
            /// <param name="receiptPosition">The receipt position.</param>
            /// <param name="salesOrder">The original sales transaction.</param>
            private static void SetReferenceDate(ReceiptPosition receiptPosition, SalesOrder salesOrder)
            {
                if (salesOrder == null)
                {
                    receiptPosition.ReferenceDateTime = DateTime.MinValue;
                    return;
                }

                FiscalTransaction fiscalTransaction = null;
                var selectedFiscalTransactions = salesOrder.FiscalTransactions
                    .Where(ft => ft.RegistrationType == ExtensibleFiscalRegistrationType.CashSale)
                    .Where(ft => ft.CountryRegionIsoCode == CountryRegionISOCode.DE.ToString());

                DateTime maxDateTime = DateTime.MinValue;
                foreach (var ft in selectedFiscalTransactions)
                {
                    var endDateTime = ft.ExtensionProperties.SingleOrDefault(p => p.Key == ExtensibleFiscalRegistrationExtendedDataType.TransactionEnd.Name)?.Value.StringValue;
                    if (DateTime.TryParse(endDateTime, out var dateTime) && dateTime > maxDateTime)
                    {
                        fiscalTransaction = ft;
                        maxDateTime = dateTime;
                    }
                }
                receiptPosition.ReferenceDateTime = maxDateTime;
            }

            private static void PopulateEfrLocalizationInfo(PopulateEfrLocalizationInfoRequest request, SingleEntityDataServiceResponse<DataModelEFR.Documents.Receipt> response)
            {
                var receipt = response.Entity;

                FiscalTransaction fiscalTransaction = FindBeginSaleFiscalTransaction(request.RequestContext, request.SalesOrder);

                if (fiscalTransaction != null)
                {
                    receipt.TransactionId = fiscalTransaction.DocumentNumber;
                    receipt.TransactionStartDateTime = Convert.ToDateTime(fiscalTransaction.ExtensionProperties.SingleOrDefault(p => p.Key == ExtensibleFiscalRegistrationExtendedDataType.TransactionStart.Name)?.Value.StringValue);
                }

                receipt.NetTaxFlag = !request.SalesOrder.IsTaxIncludedInPrice;
            }

            /// <summary>
            /// Searches for the latest fiscal transaction for the BeginSale event.
            /// </summary>
            /// <returns>The latest BeginSale fiscal transaction or null if not found.</returns>
            private static FiscalTransaction FindBeginSaleFiscalTransaction(RequestContext requestContext, SalesOrder salesOrder)
            {
                FiscalTransaction fiscalTransaction = null;
                var selectedFiscalTransactions = salesOrder.FiscalTransactions
                    .Where(ft => ft.RegistrationType == ExtensibleFiscalRegistrationType.None)
                    .Where(ft => ft.CountryRegionIsoCode == requestContext.GetPrincipal().CountryRegionIsoCode);

                var maxDateTime = DateTime.MinValue;
                foreach (var ft in selectedFiscalTransactions)
                {
                    var startDateTime = ft.ExtensionProperties.SingleOrDefault(p => p.Key == ExtensibleFiscalRegistrationExtendedDataType.TransactionStart.Name)?.Value.StringValue;
                    if (DateTime.TryParse(startDateTime, out var dateTime) && dateTime > maxDateTime)
                    {
                        fiscalTransaction = ft;
                        maxDateTime = dateTime;
                    }
                }

                return fiscalTransaction;
            }

            private static async Task PopulateEfrCustomerData(PopulateEfrCustomerDataRequest request, SingleEntityDataServiceResponse<DataModelEFR.Documents.Receipt> response)
            {
                var receipt = response.Entity;

                if (ConfigurationController.ParsePrintCustomerDataInReceipt(request.FiscalIntegrationFunctionalityProfile))
                {
                    if (!string.IsNullOrWhiteSpace(request.SalesOrder.CustomerId))
                    {
                        var customersServiceRequest = new GetCustomersServiceRequest(QueryResultSettings.SingleRecord, request.SalesOrder.CustomerId);
                        var customersServiceResponse = await request.RequestContext.ExecuteAsync<GetCustomersServiceResponse>(customersServiceRequest).ConfigureAwait(false);

                        var ctm = customersServiceResponse.Customers.FirstOrDefault();

                        receipt.Customer = new ReceiptCustomer
                        {
                            Address = ctm?.Addresses?.SingleOrDefault(address => address.IsPrimary)?.FullAddress,
                            CustomerName = ctm?.Name,
                            CustomerNumber = ctm?.AccountNumber,
                            VatNumber = ctm?.VatNumber
                        };
                    }
                    else
                    {
                        receipt.Customer = new ReceiptCustomer
                        {
                            Address = null,
                            CustomerName = string.Empty,
                            CustomerNumber = string.Empty,
                            VatNumber = string.Empty
                        };
                    }
                }
            }

            private static async Task PopulateCountryRegionSpecificPositions(PopulateCountryRegionSpecificPositionsRequest request, SingleEntityDataServiceResponse<List<ReceiptPosition>> response)
            {
                var requestContext = request.RequestContext;
                var fiscalIntegrationFunctionalityProfile = request.FiscalIntegrationFunctionalityProfile;
                var receiptPositions = response.Entity;
                var salesOrder = request.SalesOrder;

                await AddPositionsByCustomerOrderDeposit(requestContext, fiscalIntegrationFunctionalityProfile, receiptPositions, salesOrder).ConfigureAwait(false);
                await AddPositionLineForCustomerDepositByPrepayment(requestContext, fiscalIntegrationFunctionalityProfile, receiptPositions, salesOrder).ConfigureAwait(false);
                await AddPositionsByGiftCard(requestContext, fiscalIntegrationFunctionalityProfile, receiptPositions, salesOrder).ConfigureAwait(false);
            }

            /// <summary>
            /// Adds positions for customer order deposit.
            /// </summary>
            /// <param name="positions">The receipt positions.</param>
            private static async Task AddPositionsByCustomerOrderDeposit(RequestContext requestContext, FiscalIntegrationFunctionalityProfile fiscalIntegrationFunctionalityProfile, List<ReceiptPosition> receiptPositions, SalesOrder salesOrder)
            {
                if (!salesOrder.IsDepositProcessing())
                {
                    return;
                }

                var depositSum = await CalculateDepositSumForOrder(requestContext, salesOrder).ConfigureAwait(false);

                if (depositSum != 0)
                {
                    receiptPositions.Add(new ReceiptPosition()
                    {
                        Description = await TranslateAsync(requestContext, requestContext.LanguageId, DataModelEFR.Constants.SalesTransactionLocalizationConstants.Deposit).ConfigureAwait(false),
                        TaxGroup = ConfigurationController.GetDepositTaxGroup(fiscalIntegrationFunctionalityProfile),
                        PositionNumber = EfrCommonFunctions.GetDepositPositionNumber(receiptPositions),
                        Amount = depositSum,
                        PositionType = AdvancePaymentType
                    });
                }
            }

            /// <summary>
            /// Adds position line for customer deposit by prepayment.
            /// </summary>
            /// <param name="positions">The receipt positions.</param>
            private static async Task AddPositionLineForCustomerDepositByPrepayment(RequestContext requestContext, FiscalIntegrationFunctionalityProfile fiscalIntegrationFunctionalityProfile, List<ReceiptPosition> receiptPositions, SalesOrder salesOrder)
            {
                if (salesOrder.PrepaymentAmountAppliedOnPickup == 0)
                {
                    return;
                }

                receiptPositions.Add(new ReceiptPosition()
                {
                    Description = await TranslateAsync(requestContext, requestContext.LanguageId, DataModelEFR.Constants.SalesTransactionLocalizationConstants.CustomerDeposit).ConfigureAwait(false),
                    TaxGroup = ConfigurationController.GetDepositTaxGroup(fiscalIntegrationFunctionalityProfile),
                    PositionNumber = EfrCommonFunctions.GetDepositPositionNumber(receiptPositions),
                    Amount = salesOrder.PrepaymentAmountAppliedOnPickup * -1,
                    PositionType = AdvancePaymentType
                });
            }

            /// <summary>
            /// Adds positions for gift cards.
            /// </summary>
            /// <param name="positions">The receipt positions.</param>
            private static async Task AddPositionsByGiftCard(RequestContext requestContext, FiscalIntegrationFunctionalityProfile fiscalIntegrationFunctionalityProfile, List<ReceiptPosition> receiptPositions, SalesOrder salesOrder)
            {
                foreach (var giftCardLine in salesOrder.ActiveSalesLines.Where(c => c.IsGiftCardLine))
                {
                    var giftCardPosition = new ReceiptPosition()
                    {
                        Description = await TranslateAsync(requestContext, requestContext.LanguageId, DataModelEFR.Constants.SalesTransactionLocalizationConstants.GiftCard).ConfigureAwait(false),
                        PositionNumber = EfrCommonFunctions.ConvertLineNumberToPositionNumber(giftCardLine.LineNumber),
                        Amount = giftCardLine.GrossAmount,
                        VoucherId = giftCardLine.GiftCardId,
                        PositionType = VoucherType
                    };

                    var lineTaxGroup = await GetSalesLineTaxGroups(requestContext, giftCardLine, fiscalIntegrationFunctionalityProfile).ConfigureAwait(false);
                    if (string.IsNullOrEmpty(lineTaxGroup))
                    {
                        lineTaxGroup = ConfigurationController.GetDepositTaxGroup(fiscalIntegrationFunctionalityProfile);
                    }
                    giftCardPosition.TaxGroup = lineTaxGroup;

                    receiptPositions.Add(giftCardPosition);
                }
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

            private static async Task<string> TranslateAsync(RequestContext requestContext, string cultureName, string textId)
            {
                return (await requestContext.ExecuteAsync<SingleEntityDataServiceResponse<string>>(new LocalizeEfrResourceRequest(cultureName, textId)).ConfigureAwait(false)).Entity;
            }

            private static async Task<string> GetSalesLineTaxGroups(RequestContext requestContext, SalesLine salesLine, FiscalIntegrationFunctionalityProfile fiscalIntegrationFunctionalityProfile)
            {
                var request = new GetEfrSalesLineTaxGroupsRequest(salesLine, fiscalIntegrationFunctionalityProfile);
                return (await requestContext.ExecuteAsync<SingleEntityDataServiceResponse<string>>(request).ConfigureAwait(false)).Entity;
            }

            private static void GetExtensionPackageDefinitions(Request request, Response response)
            {
                ThrowIf.Null(request, "request");
                ThrowIf.Null(response, "response");

                var extensionPackageDefinition = new ExtensionPackageDefinition();

                // Should match the PackageName used when packaging the customization package (i.e. in CustomizationPackage.props).
                extensionPackageDefinition.Name = "Contoso.EFRSample";
                extensionPackageDefinition.Publisher = "Contoso";
                extensionPackageDefinition.IsEnabled = true;

                var getExtensionsResponse = (GetExtensionPackageDefinitionsResponse)response;
                getExtensionsResponse.ExtensionPackageDefinitions.Add(extensionPackageDefinition);
            }
        }
    }
}