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
    namespace CommerceRuntime.DocumentProvider.EpsonFP90IIISample.Helpers
    {
        using System;
        using System.Collections.Generic;
        using System.Collections.ObjectModel;
        using System.Globalization;
        using System.Linq;
        using System.Threading.Tasks;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
        using Microsoft.Dynamics.Commerce.Runtime.FiscalIntegration.DocumentProvider.Messages;
        using Microsoft.Dynamics.Commerce.Runtime.RealtimeServices.Messages;
        using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;

        /// <summary>
        /// The helper class for sales order is used to build fiscal receipt document.
        /// </summary>
        public static class SalesOrderHelper
        {
            /// <summary>
            /// Converts sales tax rate to type integer.
            /// </summary>
            /// <param name="taxRate">The sales tax rate.</param>
            /// <returns>The sales tax integer value.</returns>
            public static int ConvertSalesTaxRateToInt(object taxRate)
            {
                ThrowIf.Null(taxRate, nameof(taxRate));

                return Convert.ToInt32(Convert.ToDecimal(taxRate, CultureInfo.InvariantCulture) * 100);
            }

            /// <summary>
            /// Gets the adjusted version of a sales order.
            /// </summary>
            /// <param name="request">The sales order to adjust.</param>
            /// <returns>The adjusted sales order response.</returns>
            public static async Task<GetAdjustedSalesOrderFiscalIntegrationServiceResponse> GetAdjustedSalesOrderAsync(GetFiscalDocumentDocumentProviderRequest request)
            {
                var adjustmentType = FiscalIntegrationSalesOrderAdjustmentType.ExcludeDeposit;

                var getAdjustedSalesOrderRequest = new GetAdjustedSalesOrderFiscalIntegrationServiceRequest(request.SalesOrder, adjustmentType);
                return await request.RequestContext.ExecuteAsync<GetAdjustedSalesOrderFiscalIntegrationServiceResponse>(getAdjustedSalesOrderRequest).ConfigureAwait(false);
            }

            /// <summary>
            /// Gets whether the status of sales order is create or edit.
            /// </summary>
            /// <param name="salesOrder">The sales order.</param>
            /// <returns>True if the sales order is customer order create or edit, otherwise false.</returns>
            public static bool IsCustomerOrderCreateOrEdit(SalesOrder salesOrder)
            {
                ThrowIf.Null(salesOrder, nameof(salesOrder));

                return salesOrder.ExtensibleSalesTransactionType == ExtensibleSalesTransactionType.CustomerOrder
                    && salesOrder.CustomerOrderMode == CustomerOrderMode.CustomerOrderCreateOrEdit;
            }

            /// <summary>
            /// Gets whether the customer order is a pick up or return order.
            /// </summary>
            /// <param name="salesOrder">The sales order.</param>
            /// <param name="channelConfiguration">The channel configuration</param>
            /// <returns>True if the sales order is customer order pick up or return, otherwise false.</returns>
            [Obsolete("Please use IsCustomerOrderPickupOrReturn(RequestContext, SalesOrder, ChannelConfiguration) instead.")]
            public static bool IsCustomerOrderPickupOrReturn(SalesOrder salesOrder, ChannelConfiguration channelConfiguration)
            {
                ThrowIf.Null(salesOrder, nameof(salesOrder));

                return salesOrder.ExtensibleSalesTransactionType == ExtensibleSalesTransactionType.CustomerOrder
                    && (salesOrder.CustomerOrderMode == CustomerOrderMode.Pickup
                     || (salesOrder.CustomerOrderMode == CustomerOrderMode.Return
                        && (salesOrder.DeliveryMode == channelConfiguration.PickupDeliveryModeCode
                            || salesOrder.DeliveryMode == channelConfiguration.CarryoutDeliveryModeCode
                            || salesOrder.ActiveSalesLines.All(l => l.DeliveryMode == channelConfiguration.CarryoutDeliveryModeCode
                                || l.DeliveryMode == channelConfiguration.PickupDeliveryModeCode))));
            }

            /// <summary>
            /// Gets whether the customer order is a pick up or return order.
            /// </summary>
            /// <param name="context">The request context.</param>
            /// <param name="salesOrder">The sales order.</param>
            /// <param name="channelConfiguration">The channel configuration</param>
            /// <returns>True if the sales order is customer order pick up or return, otherwise false.</returns>
            public static async Task<bool> IsCustomerOrderPickupOrReturn(RequestContext context, SalesOrder salesOrder, ChannelConfiguration channelConfiguration)
            {
                ThrowIf.Null(salesOrder, nameof(salesOrder));

                return salesOrder.ExtensibleSalesTransactionType == ExtensibleSalesTransactionType.CustomerOrder
                    && (salesOrder.CustomerOrderMode == CustomerOrderMode.Pickup
                     || (salesOrder.CustomerOrderMode == CustomerOrderMode.Return
                        && await IsPickupOrCarryoutDeliveryMode(context, salesOrder, channelConfiguration).ConfigureAwait(false)));
            }

            /// <summary>
            /// Gets whether the deposit amount applied on pick up transaction.
            /// </summary>
            /// <param name="salesOrder">The sales order.</param>
            /// <returns>True if the deposit amount applied on pick up transaction, otherwise false.</returns>
            public static bool IsPrepaymentAmountAppliedOnPickup(SalesOrder salesOrder)
            {
                ThrowIf.Null(salesOrder, nameof(salesOrder));

                return salesOrder.ExtensibleSalesTransactionType == ExtensibleSalesTransactionType.CustomerOrder
                    && salesOrder.CustomerOrderMode == CustomerOrderMode.Pickup
                    && salesOrder.PrepaymentAmountAppliedOnPickup != 0;
            }

            /// <summary>
            /// Check the sales order is a return order and return by transaction.
            /// </summary>
            /// <param name="salesOrder">The sales order.</param>
            /// <returns>A boolean value, true, if the sales order is a return order and return by transaction, else false.</returns>
            public static bool IsReturnByTransaction(SalesOrder salesOrder)
            {
                ThrowIf.Null(salesOrder, nameof(salesOrder));

                bool isReturnByTransaction = false;

                if (salesOrder.ExtensibleSalesTransactionType == ExtensibleSalesTransactionType.CustomerOrder)
                {
                    isReturnByTransaction = salesOrder.CustomerOrderMode == CustomerOrderMode.Return;
                }
                else
                {
                    isReturnByTransaction = salesOrder.IsReturnByReceipt;
                }

                isReturnByTransaction = isReturnByTransaction && salesOrder.ActiveSalesLines.All(salesLine => salesLine.ReturnTransactionId != string.Empty);

                return isReturnByTransaction;
            }

            /// <summary>
            /// Check the sales order is a return product order.
            /// </summary>
            /// <param name="salesOrder">The sales order.</param>
            /// <returns>A boolean value, true, if the sales order is a return product order, else false.</returns>
            public static bool IsReturnProduct(SalesOrder salesOrder)
            {
                ThrowIf.Null(salesOrder, nameof(salesOrder));

                return !SalesOrderHelper.IsReturnByTransaction(salesOrder)
                    && salesOrder.ActiveSalesLines.All(salesline => salesline.Quantity < 0);
            }

            /// <summary>
            /// Get payment lines.
            /// </summary>
            /// <param name="salesOrder">The sales order.</param>
            /// <returns>The list of payment lines that are not voided and are not changes.</returns>
            public static IList<TenderLine> GetPaymentLines(SalesOrder salesOrder)
            {
                ThrowIf.Null(salesOrder, nameof(salesOrder));

                return salesOrder.ActiveTenderLines.Where(t => !t.IsChangeLine).ToList();
            }

            /// <summary>
            /// Get products by their item IDs.
            /// </summary>
            /// <param name="context">The request context.</param>
            /// <param name="itemIds">Products' item IDs.</param>
            /// <returns>The list of products.</returns>
            public static async Task<ReadOnlyCollection<Item>> GetProductsByItemIDAsync(RequestContext context, IEnumerable<string> itemIds)
            {
                ThrowIf.Null(context, nameof(context));

                GetItemsDataRequest getItemsDataRequest = new GetItemsDataRequest(itemIds);
                GetItemsDataResponse getItemsResponse = await context.ExecuteAsync<GetItemsDataResponse>(getItemsDataRequest).ConfigureAwait(false);

                ReadOnlyCollection<Item> items = getItemsResponse.Items;

                return items;
            }

            /// <summary>
            /// Get fiscal texts of each discount in the sales order.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <param name="adjustedSalesOrder">The adjusted sales order.</param>
            /// <returns>The list of discount fiscal texts.</returns>
            public static async Task<List<FiscalIntegrationSalesDiscountFiscalText>> GetDiscountFiscalTextForSalesOrderAsync(GetFiscalDocumentDocumentProviderRequest request, SalesOrder adjustedSalesOrder)
            {
                ThrowIf.Null(request, nameof(request));
                ThrowIf.Null(adjustedSalesOrder, nameof(adjustedSalesOrder));

                GetFiscalTextsFromSalesOrderFiscalIntegrationServiceRequest fiscalTextRequest = new GetFiscalTextsFromSalesOrderFiscalIntegrationServiceRequest(adjustedSalesOrder, request.FiscalIntegrationFunctionalityProfileGroupId);
                GetFiscalTextsFromSalesOrderFiscalIntegrationServiceResponse fiscalTextResponse = await request.RequestContext.Runtime.ExecuteAsync<GetFiscalTextsFromSalesOrderFiscalIntegrationServiceResponse>(fiscalTextRequest, request.RequestContext).ConfigureAwait(false);

                return fiscalTextResponse.SalesOrderDiscountsFiscalTexts;
            }

            /// <summary>
            /// Gets original sales order for sales order return.
            /// </summary>
            /// <param name="requestContext">The context of the request.</param>
            /// <param name="adjustedSalesOrderReturn">The adjusted sales order for return transaction.</param>
            /// <returns>The original sales order.</returns>
            public static async Task<SalesOrder> GetOriginalSalesOrderForReturnAsync(RequestContext requestContext, SalesOrder adjustedSalesOrderReturn)
            {
                ThrowIf.Null(requestContext, nameof(requestContext));
                ThrowIf.Null(adjustedSalesOrderReturn, nameof(adjustedSalesOrderReturn));

                string originalTransactionId = adjustedSalesOrderReturn.ActiveSalesLines
                    .FirstOrDefault(salesLine => salesLine.ReturnTransactionId != string.Empty)?.ReturnTransactionId;

                if (adjustedSalesOrderReturn.ExtensibleSalesTransactionType == ExtensibleSalesTransactionType.CustomerOrder)
                {
                    return string.IsNullOrWhiteSpace(originalTransactionId) ? null : await SalesOrderHelper.ResolveOriginalCustomerOrderAsync(requestContext, originalTransactionId).ConfigureAwait(false);
                }

                return string.IsNullOrWhiteSpace(originalTransactionId) ? null
                    : await SalesOrderHelper.GetSalesOrderByTransactionIdAndSearchLocationAsync(requestContext, originalTransactionId, SearchLocation.Local).ConfigureAwait(false) ?? await SalesOrderHelper.GetSalesOrderByTransactionIdAndSearchLocationAsync(requestContext, originalTransactionId, SearchLocation.Remote).ConfigureAwait(false);
            }

            /// <summary>
            /// Returns sales order by transaction id and search location.
            /// </summary>
            /// <param name="requestContext">The context of the request.</param>
            /// <param name="transactionId">The transaction id parameter.</param>
            /// <param name="searchLocationType">The search location type.</param>
            /// <returns>The SalesOrder.</returns>
            private static async Task<SalesOrder> GetSalesOrderByTransactionIdAndSearchLocationAsync(RequestContext requestContext, string transactionId, SearchLocation searchLocationType)
            {
                ThrowIf.Null(requestContext, nameof(requestContext));
                ThrowIf.NullOrWhiteSpace(transactionId, nameof(transactionId));

                try
                {
                    var request = new GetSalesOrderDetailsByTransactionIdServiceRequest(transactionId, searchLocationType);
                    var response = await requestContext.ExecuteAsync<GetSalesOrderDetailsServiceResponse>(request).ConfigureAwait(false);

                    return response.SalesOrder;
                }
                catch (CommerceException ex)
                {
                    if (searchLocationType == SearchLocation.Remote)
                    {
                        throw new CommunicationException(
                            CommunicationErrors.Microsoft_Dynamics_Commerce_Runtime_TransactionServiceException,
                            "The error occurred while searching the sales order to return.");
                    }

                    throw ex;
                }
            }

            /// <summary>
            /// Returns sales order by sales id and sarch location.
            /// </summary>
            /// <param name="requestContext">The context of the request.</param>
            /// <param name="salesId">The sales id parameter.</param>
            /// <param name="searchLocationType">The search location type.</param>
            /// <returns>The SalesOrder.</returns>
            private static async Task<SalesOrder> GetSalesOrderBySalesIdAndSearchLocationAsync(RequestContext requestContext, string salesId, SearchLocation searchLocationType)
            {
                ThrowIf.Null(requestContext, nameof(requestContext));
                ThrowIf.NullOrWhiteSpace(salesId, nameof(salesId));

                try
                {
                    var request = new GetSalesOrderDetailsBySalesIdServiceRequest(salesId, searchLocationType);
                    var response = await requestContext.ExecuteAsync<GetSalesOrderDetailsServiceResponse>(request).ConfigureAwait(false);

                    return response.SalesOrder;
                }
                catch (CommerceException ex)
                {
                    if (searchLocationType == SearchLocation.Remote)
                    {
                        throw new CommunicationException(
                            CommunicationErrors.Microsoft_Dynamics_Commerce_Runtime_TransactionServiceException,
                            "The error occurred while searching the customer order to return.");
                    }

                    throw ex;
                }
            }

            /// <summary>
            /// Returns sales order by invoice id.
            /// </summary>
            /// <param name="requestContext">The context of the request.</param>
            /// <param name="invoiceId">The invoice id parameter.</param>
            /// <returns>The SalesOrder.</returns>
            private static async Task<SalesOrder> GetSalesOrderByInvoiceIdAsync(RequestContext requestContext, string invoiceId)
            {
                ThrowIf.Null(requestContext, nameof(requestContext));
                ThrowIf.NullOrWhiteSpace(invoiceId, nameof(invoiceId));

                try
                {
                    var request = new GetInvoiceRealtimeRequest(null, invoiceId);
                    var response = await requestContext.ExecuteAsync<GetInvoiceRealtimeResponse>(request).ConfigureAwait(false);

                    return response.Order;
                }
                catch (CommerceException)
                {
                    throw new CommunicationException(
                        CommunicationErrors.Microsoft_Dynamics_Commerce_Runtime_TransactionServiceException,
                        "The error occurred while searching the customer order to return.");
                }
            }

            /// <summary>
            /// Returns invoces for the particular sale order.
            /// </summary>
            /// <param name="requestContext">The context of the request.</param>
            /// <param name="salesId">The sales id parameter.</param>
            /// <returns>The collection of sales invoices.</returns>
            private static async Task<PagedResult<SalesInvoice>> GetInvoicesBySalesIdAsync(RequestContext requestContext, string salesId)
            {
                ThrowIf.Null(requestContext, nameof(requestContext));
                ThrowIf.NullOrWhiteSpace(salesId, nameof(salesId));

                try
                {
                    var request = new GetInvoiceRealtimeRequest(salesId, null);
                    var response = await requestContext.ExecuteAsync<GetInvoiceRealtimeResponse>(request).ConfigureAwait(false);

                    return response.Invoices;
                }
                catch (CommerceException)
                {
                    throw new CommunicationException(
                        CommunicationErrors.Microsoft_Dynamics_Commerce_Runtime_TransactionServiceException,
                        "The error occurred while searching the customer order to return.");
                }
            }

            /// <summary>
            /// Returns the original customer order that has only one sales invoice.
            /// </summary>
            /// <param name="requestContext">The context of the request.</param>
            /// <param name="originalInvoiceId">The original sales invoice id parameter.</param>
            /// <returns>The sales order.</returns>
            private static async Task<SalesOrder> ResolveOriginalCustomerOrderAsync(RequestContext requestContext, string originalInvoiceId)
            {
                ThrowIf.Null(requestContext, nameof(requestContext));
                ThrowIf.NullOrWhiteSpace(originalInvoiceId, nameof(originalInvoiceId));

                var originalSalesIdResponse = await SalesOrderHelper.GetSalesOrderByInvoiceIdAsync(requestContext, originalInvoiceId).ConfigureAwait(false);
                var originalSalesId = originalSalesIdResponse?.SalesId;
                var salesOrderInvoices = string.IsNullOrWhiteSpace(originalSalesId) ? null : await SalesOrderHelper.GetInvoicesBySalesIdAsync(requestContext, originalSalesId).ConfigureAwait(false);
                var canResolveOriginalCustomerOrder = !salesOrderInvoices.IsNullOrEmpty() && salesOrderInvoices.Count() == 1;

                return !canResolveOriginalCustomerOrder ? null
                    : await SalesOrderHelper.GetSalesOrderBySalesIdAndSearchLocationAsync(requestContext, originalSalesId, SearchLocation.Local).ConfigureAwait(false) ?? await SalesOrderHelper.GetSalesOrderBySalesIdAndSearchLocationAsync(requestContext, originalSalesId, SearchLocation.Remote).ConfigureAwait(false);
            }

            /// <summary>
            /// Check the ActiveSalesLines has the same filled in ReturnTransactionId.
            /// </summary>
            /// <param name="salesOrder">The sales order.</param>
            /// <returns>A boolean value, true, if all active lines has the same return transaction Id, else false.</returns>
            internal static bool CheckActiveSalesLinesReturnTransactionId(this SalesOrder salesOrder)
            {
                return salesOrder.ActiveSalesLines.All(salesLine => salesLine.ReturnTransactionId != string.Empty)
                    && salesOrder.ActiveSalesLines.Select(salesLine => salesLine.ReturnTransactionId).Distinct().Count() == 1;
            }

            /// <summary>
            /// Checks whether given delivery mode code is pickup mode.
            /// </summary>
            /// <param name="context">The request context.</param>
            /// <param name="deliveryModeCode">The delivery mode code.</param>
            /// <returns>True if it is pickup mode, false otherwise.</returns>
            private static async Task<bool> IsPickupDeliveryMode(RequestContext context, string deliveryModeCode)
            {
                var request = new IsDeliveryModePickupServiceRequest(deliveryModeCode);
                var response = await context.ExecuteAsync<IsDeliveryModePickupServiceResponse>(request).ConfigureAwait(false);
                return response.IsPickupDeliveryMode;
            }

            private static async Task<bool> IsPickupOrCarryoutDeliveryMode(RequestContext context, SalesOrder salesOrder, ChannelConfiguration channelConfiguration)
            {
                if (salesOrder.DeliveryMode == channelConfiguration.CarryoutDeliveryModeCode ||
                    await IsPickupDeliveryMode(context, salesOrder.DeliveryMode).ConfigureAwait(false))
                {
                    return true;
                }

                foreach (var salesLine in salesOrder.ActiveSalesLines)
                {
                    if (!(salesLine.DeliveryMode == channelConfiguration.CarryoutDeliveryModeCode ||
                        await IsPickupDeliveryMode(context, salesLine.DeliveryMode).ConfigureAwait(false)))
                    {
                        return false;
                    }
                }

                return true;
            }
        }
    }
}
