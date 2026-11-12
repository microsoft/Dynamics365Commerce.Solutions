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
        using System.Threading.Tasks;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;

        /// <summary>
        /// The sales line extensions.
        /// </summary>
        internal static class SalesLineExtensions
        {
            /// <summary>
            /// Gets sales line item serial or batch number.
            /// </summary>
            /// <param name="salesLine">The sales line.</param>
            /// <returns>The serial number of batch number.</returns>
            public static string ItemIdentity(this SalesLine salesLine)
            {
                if (salesLine.IsGiftCardLine)
                {
                    return salesLine.GiftCardId;
                }

                return salesLine.SerialNumber != string.Empty ? salesLine.SerialNumber : salesLine.BatchId;
            }

            /// <summary>
            /// Gets origin sales order for the return operation.
            /// </summary>
            /// <param name="salesLine">The sales line.</param>
            /// <param name="requestContext">The request context.</param>
            /// <returns>The origin sales order.</returns>
            public static async Task<SalesOrder> GetOriginSalesOrderAsync(this SalesLine salesLine, RequestContext requestContext)
            {
                ThrowIf.Null(requestContext, nameof(requestContext));

                if (string.IsNullOrWhiteSpace(salesLine.ReturnTransactionId))
                {
                    return null;
                }

                GetSalesOrderDetailsByTransactionIdServiceRequest salesOrderRequest = new GetSalesOrderDetailsByTransactionIdServiceRequest(
                    salesLine.ReturnTransactionId,
                    SearchLocation.Local);

                GetSalesOrderDetailsServiceResponse salesOrderResponse = await requestContext.ExecuteAsync<GetSalesOrderDetailsServiceResponse>(salesOrderRequest).ConfigureAwait(false);

                return salesOrderResponse.SalesOrder;
            }
        }
    }
}
