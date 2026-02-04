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
        using Contoso.CommerceRuntime.DocumentProvider.DataModelEFR.Documents;
        using Contoso.CommerceRuntime.DocumentProvider.EFRSample.DocumentHelpers;
        using Contoso.CommerceRuntime.DocumentProvider.EFRSample.Extensions;
        using Contoso.CommerceRuntime.DocumentProvider.EFRSample.Messages;
        using Contoso.CommerceRuntime.DocumentProvider.EFRSample.Serializers;
        using ReceiptsCzech = Microsoft.Dynamics.Commerce.Runtime.Localization.Entities.Czechia;

        public sealed class EfrFiscalDocumentServiceTriggersCZ : ICountryRegionAware, IRequestTriggerAsync
        {
            private const int PositionCzField = 23;
            private const int PaymentCzField = 24;

            public IEnumerable<Type> SupportedRequestTypes
            {
                get
                {
                    return new[]
                    {
                        typeof(PopulateEfrReferenceFieldsRequest),
                        typeof(PopulateEfrLocalizationInfoRequest),
                        typeof(PopulateCountryRegionSpecificPositionsRequest),
                        typeof(PopulateEfrLocalizationInfoToPaymentRequest),
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
                        nameof(CountryRegionISOCode.CZ),
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
                    case PopulateCountryRegionSpecificPositionsRequest populateCountryRegionSpecificPositionsRequest:
                        await PopulateCountryRegionSpecificPositions(populateCountryRegionSpecificPositionsRequest, (SingleEntityDataServiceResponse<List<ReceiptPosition>>)response).ConfigureAwait(false);
                        break;
                    case PopulateEfrLocalizationInfoToPaymentRequest populateEfrLocalizationInfoToPaymentRequest:
                        PopulateEfrLocalizationInfoToPayment(populateEfrLocalizationInfoToPaymentRequest, (SingleEntityDataServiceResponse<ReceiptPayment>)response);
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
                var originalSalesOrder = await request.SalesLine.GetOriginSalesOrderAsync(request.RequestContext).ConfigureAwait(false);
                if (originalSalesOrder == null)
                {
                    return;
                }

                ReceiptsCzech.FiscalServiceResponse fiscalServiceResponse = null;
                var fiscalResponseString = originalSalesOrder.FiscalTransactions.Where(c => !string.IsNullOrWhiteSpace(c.RegisterResponse)).FirstOrDefault()?.RegisterResponse;

                if (string.IsNullOrWhiteSpace(fiscalResponseString))
                {
                    return;
                }

                fiscalServiceResponse = XmlSerializer<ReceiptsCzech.FiscalServiceResponse>.Deserialize(fiscalResponseString);

                if (fiscalServiceResponse == null)
                {
                    return;
                }
                receiptPosition.ReferenceDateTimeStringValue = fiscalServiceResponse.TransactionDate;
                receiptPosition.ReferenceTransactionNumber = fiscalServiceResponse.TransactionNumber;
            }

            private static void PopulateEfrLocalizationInfo(PopulateEfrLocalizationInfoRequest request, SingleEntityDataServiceResponse<DataModelEFR.Documents.Receipt> response)
            {
                var receipt = response.Entity;
                TaxRegistration taxRegistration = request.RequestContext.GetOrgUnit()?.TaxRegistrations.Where(taxReg => taxReg.Type == TaxRegistrationType.BusinessPremiseId).FirstOrDefault();
                var registrationId = taxRegistration?.RegistrationNumber ?? string.Empty;

                receipt.RegistrationId = registrationId;
            }

            private static async Task PopulateCountryRegionSpecificPositions(PopulateCountryRegionSpecificPositionsRequest request, SingleEntityDataServiceResponse<List<ReceiptPosition>> response)
            {
                var salesOrder = request.SalesOrder;
                var requestContext = request.RequestContext;
                var receiptPositions = request.ReceiptPositions;
                var fiscalIntegrationFunctionalityProfile = request.FiscalIntegrationFunctionalityProfile;

                if (salesOrder.IsDepositProcessing())
                {
                    var depositSum = await CalculateDepositSumForOrder(requestContext, salesOrder).ConfigureAwait(false);

                    if (depositSum != 0)
                    {
                        receiptPositions.Add(new ReceiptPosition()
                        {
                            Quantity = depositSum > 0 ? 1 : -1,
                            Description = await TranslateAsync(requestContext, requestContext.LanguageId, DataModelEFR.Constants.SalesTransactionLocalizationConstants.Deposit).ConfigureAwait(false),
                            TaxGroup = ConfigurationController.GetDepositTaxGroup(fiscalIntegrationFunctionalityProfile),
                            PositionNumber = EfrCommonFunctions.GetDepositPositionNumber(receiptPositions),
                            UnitPrice = Math.Abs(depositSum),
                            Amount = depositSum,
                            CZField = PositionCzField,
                        });
                    }
                }

                foreach (var giftCardLine in salesOrder.ActiveSalesLines.Where(c => c.IsGiftCardLine))
                {
                    var giftCardPosition = new ReceiptPosition()
                    {
                        Quantity = giftCardLine.Quantity,
                        Description = await TranslateAsync(requestContext, requestContext.LanguageId, DataModelEFR.Constants.SalesTransactionLocalizationConstants.GiftCard).ConfigureAwait(false),
                        ItemIdentity = giftCardLine.ItemIdentity(),
                        PositionNumber = EfrCommonFunctions.ConvertLineNumberToPositionNumber(giftCardLine.LineNumber),
                        Amount = giftCardLine.GrossAmount,
                        UnitPrice = giftCardLine.Price,
                        CZField = PositionCzField,
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

            private static void PopulateEfrLocalizationInfoToPayment(PopulateEfrLocalizationInfoToPaymentRequest request, SingleEntityDataServiceResponse<ReceiptPayment> response)
            {
                response.Entity.CZField = PaymentCzField;
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