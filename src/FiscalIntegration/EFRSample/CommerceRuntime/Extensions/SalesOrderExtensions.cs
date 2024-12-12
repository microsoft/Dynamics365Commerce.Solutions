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
    namespace CommerceRuntime.DocumentProvider.EFRSample.Extensions
    {
        using System.Collections.Generic;
        using System.Linq;
        using System.Threading.Tasks;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;
        using Contoso.CommerceRuntime.DocumentProvider.DataModelEFR.Constants;

        /// <summary>
        /// The sales order extensions.
        /// </summary>
        internal static class SalesOrderExtensions
        {
            /// <summary>
            /// Calculates total payment amount with prepayment.
            /// </summary>
            /// <param name="salesOrder">The sales order.</param>
            /// <returns>The total payment amount.</returns>
            public static decimal CalcTotalPaymentAmountWithPrepayment(this SalesOrder salesOrder)
            {
                return salesOrder.ActiveTenderLines.Sum(tenderLine => tenderLine.Amount) + salesOrder.PrepaymentAmountAppliedOnPickup;
            }

            /// <summary>
            /// Calculates total payment amount.
            /// </summary>
            /// <param name="salesOrder">The sales order.</param>
            /// <returns>The total payment amount.</returns>
            public static decimal CalcTotalPaymentAmount(this SalesOrder salesOrder)
            {
                return salesOrder.ActiveTenderLines.Sum(tenderLine => tenderLine.Amount);
            }

            /// <summary>
            /// Checks if sales order has gift card.
            /// </summary>
            /// <param name="salesOrder">The sales order.</param>
            /// <returns>True if sales order has gift card; otherwise, false.</returns>
            public static bool HasGiftCard(this SalesOrder salesOrder)
            {
                return salesOrder.ActiveSalesLines.Any(l => l.IsGiftCardLine);
            }

            /// <summary>
            /// Checks if sales order has deposit lines only.
            /// </summary>
            /// <param name="salesOrder">The sales order.</param>
            /// <param name="requestContext">The request context.</param>
            /// <returns>True if sales order has deposit lines only; otherwise, false.</returns>
            public static bool HasDepositLinesOnly(this SalesOrder salesOrder, RequestContext requestContext)
            {
                ThrowIf.Null(requestContext, nameof(requestContext));

                string carryoutDeliveryModeCode = requestContext.GetChannelConfiguration().CarryoutDeliveryModeCode;
                return !salesOrder.ActiveSalesLines.Any(l => l.DeliveryMode == carryoutDeliveryModeCode);
            }

            /// <summary>
            /// Checks if sales order is in deposit processing stage.
            /// </summary>
            /// <param name="salesOrder">The sales order.</param>
            /// <returns>True if sales order is in deposit processing stage; otherwise, false.</returns>
            public static bool IsDepositProcessing(this SalesOrder salesOrder)
            {
                if ((salesOrder.ExtensibleSalesTransactionType == ExtensibleSalesTransactionType.CustomerOrder
                     || salesOrder.ExtensibleSalesTransactionType == ExtensibleSalesTransactionType.AsyncCustomerOrder)
                     && (salesOrder.CustomerOrderMode == CustomerOrderMode.CustomerOrderCreateOrEdit 
                     || salesOrder.CustomerOrderMode == CustomerOrderMode.Cancellation))
                {
                    return true;
                }

                return false;
            }

            /// <summary>
            /// Gets sales lines with non-zero quantity.
            /// </summary>
            /// <param name="salesOrder">The sales order.</param>
            /// <returns>The sales lines.</returns>
            public static IEnumerable<SalesLine> SalesLinesWithNonZeroQuantity(this SalesOrder salesOrder)
            {
                return salesOrder.ActiveSalesLines.Where(l => l.Quantity != 0);
            }

            /// <summary>
            /// Gets active tender lines grouped by tender type id.
            /// </summary>
            /// <param name="salesOrder">The sales order.</param>
            /// <returns>The tender lines.</returns>
            public static IEnumerable<IGrouping<string, TenderLine>> TenderLinesGrpByTenderTypeId(this SalesOrder salesOrder)
            {
                return salesOrder.ActiveTenderLines.GroupBy(tenderLine => tenderLine.TenderTypeId);
            }

            /// <summary>
            /// Gets tax lines grouped by percentage.
            /// </summary>
            /// <param name="salesOrder">The sales order.</param>
            /// <returns>The tax lines.</returns>
            public static IEnumerable<IGrouping<decimal, TaxLine>> TaxLinesGrpByPercentage(this SalesOrder salesOrder)
            {
                return salesOrder.SalesLinesWithNonZeroQuantity().SelectMany(salesLine => salesLine.TaxLines).GroupBy(taxLine => taxLine.Percentage);
            }

            /// <summary>
            /// Gets non-fiscal transaction type.
            /// </summary>
            /// <param name="salesOrder">The sales order.</param>
            /// <param name="requestContext">The request context.</param>
            /// <returns>The non-fiscal transaction type.</returns>
            public static string NonFiscalTransactionType(this SalesOrder salesOrder, RequestContext requestContext)
            {
                if (salesOrder.IsDepositProcessing() && salesOrder.HasDepositLinesOnly(requestContext))
                {
                    return SalesTransactionLocalizationConstants.CustomerDeposit;
                }

                if (salesOrder.HasGiftCard())
                {
                    return SalesTransactionLocalizationConstants.GiftCard;
                }

                return string.Empty;
            }

            /// <summary>
            /// Gets adjusted sales order.
            /// </summary>
            /// <param name="salesOrder">The sales order.</param>
            /// <param name="requestContext">The request context.</param>
            /// <param name="adjustmentType">The adjustment type.</param>
            /// <returns>The adjusted sales order fiscal integration service response.</returns>
            public static async Task<GetAdjustedSalesOrderFiscalIntegrationServiceResponse> GetAdjustedSalesOrderAsync(this SalesOrder salesOrder,
                RequestContext requestContext,
                FiscalIntegrationSalesOrderAdjustmentType adjustmentType)
            {
                ThrowIf.Null(requestContext, nameof(requestContext));

                var getAdjustedSalesOrderRequest = new GetAdjustedSalesOrderFiscalIntegrationServiceRequest(salesOrder, adjustmentType);
                return await requestContext.ExecuteAsync<GetAdjustedSalesOrderFiscalIntegrationServiceResponse>(getAdjustedSalesOrderRequest).ConfigureAwait(false);
            }

            /// <summary>
            /// Gets the non-giftcard lines with non-zero quantity from a sales order.
            /// </summary>
            /// <param name="salesOrder">The sales order.</param>
            /// <returns>The non-giftcard lines with non-zero quantity of a sales order.</returns>
            public static IEnumerable<SalesLine> GetNonGiftCardLines(this SalesOrder salesOrder)
            {
                return salesOrder.SalesLinesWithNonZeroQuantity().Where(salesLine => !salesLine.IsGiftCardLine);
            }

            /// <summary>
            /// Gets the carryout lines from a sales order.
            /// </summary>
            /// <param name="salesOrder">The sales order.</param>
            /// <param name="requestContext">The request context.</param>
            /// <returns>The carryout lines.</returns>
            public static IEnumerable<SalesLine> GetCarryOutLines(this SalesOrder salesOrder, RequestContext requestContext)
            {
                string carryoutDeliveryModeCode = requestContext.GetChannelConfiguration().CarryoutDeliveryModeCode;
                return salesOrder.GetNonGiftCardLines().Where(l => l.DeliveryMode == carryoutDeliveryModeCode);
            }
        }
    }
}
