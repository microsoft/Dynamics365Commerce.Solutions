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
        using System.Collections.Generic;
        using System.Threading.Tasks;
        using System.Xml.Linq;
        using Contoso.CommerceRuntime.DocumentProvider.EpsonFP90IIISample.Constants;
        using Contoso.CommerceRuntime.DocumentProvider.EpsonFP90IIISample.Helpers;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.FiscalIntegration.DocumentProvider.Messages;

        /// <summary>
        /// Helper class for getting payment information
        /// </summary>
        public static class PaymentInfoBuilder
        {
            /// <summary>
            /// Fills in the payment information. 
            /// </summary>
            /// <param name="parentElement">The parent element.</param>
            /// <param name="request">The request.</param>
            /// <param name="adjustedSalesOrder">The adjusted sales order.</param>
            /// <returns>The updated xml element.</returns>
            public static async Task<XElement> FillPaymentsAsync(XElement parentElement, GetFiscalDocumentDocumentProviderRequest request, SalesOrder adjustedSalesOrder)
            {
                bool isReturn = SalesOrderHelper.IsReturnByTransaction(adjustedSalesOrder) || SalesOrderHelper.IsReturnProduct(adjustedSalesOrder);

                if (isReturn)
                {
                    parentElement = await DocumentElementBuilder.BuildPrintRecTotalAsync(parentElement, request, adjustedSalesOrder, null, decimal.Zero).ConfigureAwait(false);
                }
                else
                {
                    // The customer order deposit amount applied to the sales transaction must be tread as a payment, and it should in first order.
                    // Build PrintRecTotal element for deposit amount.
                    if (SalesOrderHelper.IsPrepaymentAmountAppliedOnPickup(adjustedSalesOrder))
                    {
                        parentElement = await DocumentElementBuilder.BuildPrintRecTotalAsync(parentElement, request, adjustedSalesOrder, null, adjustedSalesOrder.PrepaymentAmountAppliedOnPickup, ConfigurationController.ParseDepositPaymentType(request.FiscalIntegrationFunctionalityProfile)).ConfigureAwait(false);
                    }

                    IList<TenderLine> paymentLines = SalesOrderHelper.GetPaymentLines(adjustedSalesOrder);

                    //If product has 100% discount
                    if (paymentLines.Count == 0 && adjustedSalesOrder.ActiveSalesLines.Count > 0)
                    {
                        parentElement = await DocumentElementBuilder.BuildPrintRecTotalAsync(parentElement, request, adjustedSalesOrder).ConfigureAwait(false);
                    }
                    else
                    {
                        foreach (var tenderLine in paymentLines)
                        {
                            parentElement = await DocumentElementBuilder.BuildPrintRecTotalAsync(parentElement, request, adjustedSalesOrder, tenderLine.TenderTypeId, tenderLine.Amount).ConfigureAwait(false);
                        }
                    }
                    if (adjustedSalesOrder.SalesPaymentDifference > 0)
                    {
                        parentElement = await DocumentElementBuilder.BuildPrintRecTotalAsync(parentElement, request, adjustedSalesOrder, DocumentAttributeConstants.DiscountPaymentCode, adjustedSalesOrder.SalesPaymentDifference).ConfigureAwait(false);
                    }
                }
                return parentElement;
            }
        }
    }
}