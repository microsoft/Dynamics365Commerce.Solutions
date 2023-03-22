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
        /// Legacy resolver for creating PaymentInfo element for the store payment method.
        /// </summary>
        public class PaymentInfoResolverRT : IPaymentInfoResolver
        {
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
                if (tenderTypeId == DocumentAttributeConstants.DiscountPaymentCode)
                {
                    return null;
                }
                else
                {
                    Dictionary<string, int> tenderTypeMappings = ConfigurationController.ParseSupportedTenderTypeMappings(functionalityProfile);

                    var getChannelTenderTypesDataRequest = new GetChannelTenderTypesDataRequest(context.GetPrincipal().ChannelId, QueryResultSettings.AllRecords);

                    var tenderTypesResponse = await context.Runtime.ExecuteAsync<EntityDataServiceResponse<TenderType>>(getChannelTenderTypesDataRequest, context).ConfigureAwait(false);
                    var tenderTypes = tenderTypesResponse.PagedEntityCollection.Results;
                    TenderType tenderType = tenderTypes.SingleOrDefault(type => type.TenderTypeId == tenderTypeId);
                    int? printerTenderType = GetPrinterTenderType(tenderTypeMappings, tenderType);

                    if (printerTenderType != null)
                    {
                        string indexAttribute = GetIndexAttribute(printerTenderType.Value, tenderType);
                        return new PaymentInfo(tenderType?.Name ?? string.Empty, printerTenderType.ToString(), indexAttribute);
                    }
                }
                return null;
            }

            /// <summary>
            /// Gets the payment information for fiscal printer by deposit payment method.
            /// </summary>
            /// <param name="depositPaymentMethod">Deposit payment method description.</param>
            /// <returns>Information about payment.</returns>
            public PaymentInfo GetDepositPaymentMethod(string depositPaymentMethod)
            {
                int depositPaymentType = (int)Enum.Parse(typeof(PrinterPaymentType), depositPaymentMethod);
                return new PaymentInfo(string.Empty, depositPaymentType.ToString(), GetIndexAttribute(depositPaymentType));
            }

            /// <summary>
            /// Gets the value of attribute index according printer tender type from user configuration.
            /// </summary>
            /// <param name="printerTenderType">The printer tender type number.</param>
            /// <param name="tenderType">The tender type.</param>
            /// <returns>The value of attribute index.</returns>
            private string GetIndexAttribute(int printerTenderType, TenderType tenderType = null)
            {
                string index;
                switch (printerTenderType)
                {
                    case (int)PrinterPaymentType.Cash:
                        index = DocumentAttributeConstants.DefaultIndexForCash;
                        break;
                    case (int)PrinterPaymentType.CreditOrCreditCard:
                        index = tenderType?.OperationId == (int)RetailOperation.PayCreditMemo
                            ? DocumentAttributeConstants.DefaultIndexForCreditMemo
                            : DocumentAttributeConstants.DefaultIndexForCreditAndTicket;
                        break;
                    case (int)PrinterPaymentType.Ticket:
                        index = DocumentAttributeConstants.DefaultIndexForCreditAndTicket;
                        break;
                    default:
                        index = string.Empty;
                        break;
                }
                return index;
            }

            /// <summary>
            /// Gets the printer tender type according tender type mappings and operation.
            /// </summary>
            /// <param name="tenderTypeMappings">The tender type mappings from HQ configuration.</param>
            /// <param name="tenderType">The tender type.</param>
            /// <returns>Printer tender type.</returns>
            private int? GetPrinterTenderType(Dictionary<string, int> tenderTypeMappings, TenderType tenderType)
            {
                if (tenderTypeMappings == null || tenderType == null)
                {
                    return null;
                }
                bool isMapped = tenderTypeMappings.TryGetValue(tenderType.TenderTypeId, out int printerTenderType);
                if (!isMapped)
                {
                    if (tenderType.OperationId == (int)RetailOperation.PayCreditMemo)
                    {
                        printerTenderType = (int)PrinterPaymentType.CreditOrCreditCard;
                    }
                    else
                    {
                        return null;
                    }
                }
                return printerTenderType;
            }
        }
    }
}
