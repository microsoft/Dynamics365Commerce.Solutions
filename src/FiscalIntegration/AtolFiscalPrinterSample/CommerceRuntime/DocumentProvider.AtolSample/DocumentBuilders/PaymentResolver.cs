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
    namespace CommerceRuntime.DocumentProvider.AtolSample.DocumentBuilders
    {
        using System;
        using System.Collections.Generic;
        using System.Linq;
        using System.Threading.Tasks;
        using Contoso.CommerceRuntime.DocumentProvider.AtolSample.DataModel.AtolTask;
        using Contoso.CommerceRuntime.DocumentProvider.AtolSample.DataModel.Configuration;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;

        /// <summary>
        /// The payment resolver.
        /// </summary>
        public class PaymentResolver : IPaymentResolver
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="PaymentResolver"/> class.
            /// </summary>
            /// <param name="documentProviderSettings">The document provider settings.</param>
            public PaymentResolver(DocumentProviderSettings documentProviderSettings)
            {
                ThrowIf.Null(documentProviderSettings, nameof(documentProviderSettings));
                this.DocumentProviderSettings = documentProviderSettings;
            }

            /// <summary>
            /// Gets document provider settings.
            /// </summary>
            public DocumentProviderSettings DocumentProviderSettings { get; }

            /// <summary>
            /// Gets the payment method of the fiscal printer.
            /// </summary>
            /// <param name="context">The current request context.</param>
            /// <param name="tenderLine">The tender line of sales order.</param>
            /// <returns>The payment of the fiscal printer.</returns>
            public async Task<Payment> GetPaymentAsync(RequestContext context, TenderLine tenderLine)
            {
                ThrowIf.Null(tenderLine, nameof(tenderLine));

                Payment itemPayment;

                var getChannelTenderTypesDataRequest = new GetChannelTenderTypesDataRequest(context.GetPrincipal().ChannelId, QueryResultSettings.AllRecords);
                var tenderTypesResponse = await context.Runtime.ExecuteAsync<EntityDataServiceResponse<TenderType>>(getChannelTenderTypesDataRequest, context).ConfigureAwait(false);
                var tenderTypes = tenderTypesResponse.PagedEntityCollection.Results;
                TenderType tenderType = tenderTypes.SingleOrDefault(type => type.TenderTypeId == tenderLine.TenderTypeId);
                bool isPrinterPaymentMethodResolved = this.ResolvePrinterTenderType(this.DocumentProviderSettings.PaymentTypeMapping, tenderType, out PaymentMethodType printerPaymentMethodType);
                PaymentMethodType paymentType;

                if (isPrinterPaymentMethodResolved)
                {
                    paymentType = printerPaymentMethodType;
                }
                else
                {
                    paymentType = this.MapRetailOperationIdToPrinterPaymentMethod(tenderType?.OperationId);
                }

                itemPayment = new Payment
                {
                    PaymentMethod = paymentType,
                    Sum = Math.Abs(tenderLine.Amount),
                };

                return itemPayment;
            }

            /// <summary>
            /// Default mapping PaymentMethodType by store operation identifier.
            /// </summary>
            /// <param name="operationId">Store operation identifiers.</param>
            /// <returns>The type of payment method for the printer.</returns>
            private PaymentMethodType MapRetailOperationIdToPrinterPaymentMethod(int? operationId)
            {
                PaymentMethodType paymentMethodType;
                switch (operationId)
                {
                    case (int)RetailOperation.PayCash:
                        paymentMethodType = PaymentMethodType.Cash;
                        break;
                    case (int)RetailOperation.PayCard:
                        paymentMethodType = PaymentMethodType.Electronically;
                        break;
                    case (int)RetailOperation.PayCheck:
                        paymentMethodType = PaymentMethodType.Other;
                        break;
                    case (int)RetailOperation.PayCreditMemo:
                        paymentMethodType = PaymentMethodType.Electronically;
                        break;
                    case (int)RetailOperation.PayCurrency:
                        paymentMethodType = PaymentMethodType.Cash;
                        break;
                    case (int)RetailOperation.PayCustomerAccount:
                        paymentMethodType = PaymentMethodType.Credit;
                        break;
                    case (int)RetailOperation.PayGiftCertificate:
                        paymentMethodType = PaymentMethodType.Other;
                        break;
                    case (int)RetailOperation.PayLoyalty:
                        paymentMethodType = PaymentMethodType.Other;
                        break;
                    default:
                        throw new DataValidationException(PaymentErrors.Microsoft_Dynamics_Commerce_Runtime_InvalidPaymentRequest, $"There was an error generating a fiscal receipt. Unable to process a payment method for the {operationId} operation. Please contact your system administrator.");
                }

                return paymentMethodType;
            }

            /// <summary>
            /// Resolves the printer tender type according tender type mappings and operation.
            /// </summary>
            /// <param name="paymentTypeMappings">The payment methods mappings from configuration.</param>
            /// <param name="tenderType">The tender type.</param>
            /// <param name="paymentMethod">The resolved printer payment method.</param>
            /// <returns>True if resolving was successful otherwise False.</returns>
            private bool ResolvePrinterTenderType(IDictionary<string, PaymentMethodType> paymentTypeMappings, TenderType tenderType, out PaymentMethodType paymentMethod)
            {
                bool isResolved = false;
                paymentMethod = PaymentMethodType.Other;
                if (paymentTypeMappings != null && tenderType != null)
                {
                    isResolved = paymentTypeMappings.TryGetValue(tenderType.TenderTypeId, out paymentMethod);
                }

                return isResolved;
            }
        }
    }
}