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
    namespace CommerceRuntime.DocumentProvider.EpsonFP90IIISample.DocumentBuilders
    {
        using System;
        using System.Collections.Generic;
        using System.Linq;
        using System.Threading.Tasks;
        using Contoso.CommerceRuntime.DocumentProvider.EpsonFP90IIISample.Constants;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;

        /// <summary>
        /// RT2 version of resolver for creating PaymentInfo element for the store payment method.
        /// </summary>
        public class PaymentInfoResolverRT2 : IPaymentInfoResolver
        {
            /// <summary>
            /// Gets payment method configured for deposit payment.
            /// </summary>
            public PaymentMethod DepositPaymentMethod { get; }

            /// <summary>
            /// Gets payment methods mapping.
            /// </summary>
            public Dictionary<string, PaymentMethod> PaymentTypeMappings { get; }

            /// <summary>
            /// Constructor for PaymentInfoResolverRT2 class.
            /// </summary>
            /// <param name="functionalityProfile">Fiscal integration functionality profile.</param>
            public PaymentInfoResolverRT2(FiscalIntegrationFunctionalityProfile functionalityProfile)
            {
                PaymentTypeMappings = ConfigurationController.ParseSupportedPaymentTypeMappings(functionalityProfile, out PaymentMethod depositPaymentMethod);
                DepositPaymentMethod = depositPaymentMethod;
            }

            /// <summary>
            /// Gets the payment information for fiscal printer by the store tenderTypeId.
            /// </summary>
            /// <param name="context">The current request context.</param>
            /// <param name="functionalityProfile">The functionality profile.</param>
            /// <param name="tenderTypeId">The tender type ID in HQ.</param>
            /// <returns>Information about payment.</returns>
            public async Task<PaymentInfo> GetPaymentInfoAsync(RequestContext context, FiscalIntegrationFunctionalityProfile functionalityProfile, string tenderTypeId)
            {
                ThrowIf.Null(functionalityProfile, nameof(functionalityProfile));

                PaymentInfo paymentInfo;
                if (tenderTypeId == DocumentAttributeConstants.DiscountPaymentCode)
                {
                    paymentInfo = new PaymentInfo(string.Empty, DocumentAttributeConstants.DefaultPaymentTypeForDiscount, DocumentAttributeConstants.DefaultPaymentIndexForDiscount);
                }
                else
                {
                    var getChannelTenderTypesDataRequest = new GetChannelTenderTypesDataRequest(context.GetPrincipal().ChannelId, QueryResultSettings.AllRecords);
                    var tenderTypesResponse = await context.Runtime.ExecuteAsync<EntityDataServiceResponse<TenderType>>(getChannelTenderTypesDataRequest, context).ConfigureAwait(false);
                    var tenderTypes = tenderTypesResponse.PagedEntityCollection.Results;
                    TenderType tenderType = tenderTypes.SingleOrDefault(type => type.TenderTypeId == tenderTypeId);
                    bool isPrinterPaymentMethodResolved = ResolvePrinterTenderType(PaymentTypeMappings, tenderType, out PaymentMethod printerPaymentMethod);
                    string paymentType;
                    string paymentIndex;

                    if (isPrinterPaymentMethodResolved)
                    {
                        paymentType = printerPaymentMethod.PrinterPaymentType;
                        paymentIndex = printerPaymentMethod.PrinterPaymentIndex;
                    }
                    else
                    {
                        AutoresolvePaymentMethod(tenderType?.OperationId, out paymentType, out paymentIndex);
                    }
                    if (string.IsNullOrEmpty(paymentType))
                    {
                        throw new Exception("Payment type not found.");
                    }
                    else if (paymentIndex == null)
                    {
                        paymentIndex = string.Empty;
                    }
                    paymentInfo = new PaymentInfo(tenderType?.Name ?? string.Empty, paymentType, paymentIndex);
                }
                return paymentInfo;
            }

            /// <summary>
            /// Gets the payment information for fiscal printer by deposit payment method.
            /// </summary>
            /// <param name="depositPaymentMethod">Deposit payment method description.</param>
            /// <returns>Information about payment.</returns>
            public PaymentInfo GetDepositPaymentMethod(string depositPaymentMethod)
            {
                if (!string.IsNullOrEmpty(DepositPaymentMethod?.PrinterPaymentType) && !string.IsNullOrEmpty(DepositPaymentMethod?.PrinterPaymentIndex))
                {
                    return new PaymentInfo(string.Empty, DepositPaymentMethod.PrinterPaymentType, DepositPaymentMethod.PrinterPaymentIndex);
                }
                else
                {
                    return GetDefaultDepositPaymentMethod();
                }
            }

            /// <summary>
            /// Autoresolves PaymentType and PaymentIndex by store operation Id.
            /// </summary>
            /// <param name="operationId">Store operation Id.</param>
            /// <param name="defaultPaymentType">Autoresolved PaymentType.</param>
            /// <param name="defaultPaymentIndex">Autoresolved PaymentIndex.</param>
            private void AutoresolvePaymentMethod(int? operationId, out string defaultPaymentType, out string defaultPaymentIndex)
            {
                switch (operationId)
                {
                    case (int)RetailOperation.PayCash:
                        defaultPaymentType = DocumentAttributeConstants.DefaultPaymentTypeForPayCash;
                        defaultPaymentIndex = DocumentAttributeConstants.DefaultPaymentIndexForPayCash;
                        break;
                    case (int)RetailOperation.PayCard:
                        defaultPaymentType = DocumentAttributeConstants.DefaultPaymentTypeForPayCard;
                        defaultPaymentIndex = DocumentAttributeConstants.DefaultPaymentIndexForPayCard;
                        break;
                    case (int)RetailOperation.PayCheck:
                        defaultPaymentType = DocumentAttributeConstants.DefaultPaymentTypeForPayCheck;
                        defaultPaymentIndex = DocumentAttributeConstants.DefaultPaymentIndexForPayCheck;
                        break;
                    case (int)RetailOperation.PayCreditMemo:
                        defaultPaymentType = DocumentAttributeConstants.DefaultPaymentTypeForPayCreditMemo;
                        defaultPaymentIndex = DocumentAttributeConstants.DefaultPaymentIndexForPayCreditMemo;
                        break;
                    case (int)RetailOperation.PayCurrency:
                        defaultPaymentType = DocumentAttributeConstants.DefaultPaymentTypeForPayCurrency;
                        defaultPaymentIndex = DocumentAttributeConstants.DefaultPaymentIndexForPayCurrency;
                        break;
                    case (int)RetailOperation.PayCustomerAccount:
                        defaultPaymentType = DocumentAttributeConstants.DefaultPaymentTypeForPayCustomerAccount;
                        defaultPaymentIndex = DocumentAttributeConstants.DefaultPaymentIndexForPayCustomerAccount;
                        break;
                    case (int)RetailOperation.PayGiftCertificate:
                        defaultPaymentType = DocumentAttributeConstants.DefaultPaymentTypeForPayGiftCard;
                        defaultPaymentIndex = DocumentAttributeConstants.DefaultPaymentIndexForPayGiftCard;
                        break;
                    case (int)RetailOperation.PayLoyalty:
                        defaultPaymentType = DocumentAttributeConstants.DefaultPaymentTypeForPayLoyaltyCard;
                        defaultPaymentIndex = DocumentAttributeConstants.DefaultPaymentIndexForPayLoyaltyCard;
                        break;
                    default:
                        throw new Exception("Payment type not found.");
                }
            }

            /// <summary>
            /// Gets default values of PaymentType and PaymentIndex for deposit payment. 
            /// </summary>
            /// <param name="depositPaymentType">Default value PaymentType for deposit payment.</param>
            /// <param name="depositPaymentIndex">Default PaymentIndex for deposit payment.</param>
            private PaymentInfo GetDefaultDepositPaymentMethod()
            {
                return new PaymentInfo(string.Empty, DocumentAttributeConstants.DefaultPaymentTypeForDepositPayment, DocumentAttributeConstants.DefaultPaymentIndexForDepositPayment);
            }

            /// <summary>
            /// Resolves the printer tender type according tender type mappings and operation.
            /// </summary>
            /// <param name="paymentTypeMappings">The payment methods mappings from configuration.</param>
            /// <param name="tenderType">The tender type.</param>
            /// <param name="paymentMethod">The resolved printer payment method.</param>
            /// <returns>True if resolving was successful otherwise False.</returns>
            private bool ResolvePrinterTenderType(Dictionary<string, PaymentMethod> paymentTypeMappings, TenderType tenderType, out PaymentMethod paymentMethod)
            {
                bool isResolved = false;
                var resolvedPaymentMethod = new PaymentMethod();
                if (paymentTypeMappings != null && tenderType != null)
                {
                    isResolved = paymentTypeMappings.TryGetValue(tenderType.TenderTypeId, out resolvedPaymentMethod);
                }
                paymentMethod = resolvedPaymentMethod;
                return isResolved;
            }
        }
    }
}
