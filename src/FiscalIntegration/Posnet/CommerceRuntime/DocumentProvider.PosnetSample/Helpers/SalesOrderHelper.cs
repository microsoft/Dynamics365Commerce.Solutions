namespace Contoso
{
    namespace Commerce.Runtime.DocumentProvider.PosnetSample.Helpers
    {
        using System;
        using System.Collections.Generic;
        using System.Collections.ObjectModel;
        using System.Linq;
        using System.Text.RegularExpressions;
        using System.Threading.Tasks;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
        using Microsoft.Dynamics.Commerce.Runtime.FiscalIntegration.DocumentProvider.Messages;
        using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;

        /// <summary>
        /// The helper class for sales order is used to build fiscal receipt document.
        /// </summary>
        internal static class SalesOrderHelper
        {
            public static readonly int AmountFractionalDigits = 2;
            private const int LengthOfProductName = 24;
            private const int LengthOfDiscountName = 24;

            /// <summary>
            /// Check the sales order is a return order.
            /// </summary>
            /// <param name="salesOrder">The sales order.</param>
            /// <returns>A boolean value, true, if the sales order is a return order, else false.</returns>
            public static bool IsReturn(SalesOrder salesOrder)
            {
                ThrowIf.Null(salesOrder, nameof(salesOrder));

                bool isReturn = false;

                if (salesOrder.ExtensibleSalesTransactionType == ExtensibleSalesTransactionType.CustomerOrder)
                {
                    isReturn = salesOrder.CustomerOrderMode == CustomerOrderMode.Return;
                }
                else
                {
                    isReturn = salesOrder.IsReturnByReceipt || salesOrder.ActiveSalesLines.Any(salesLine => salesLine.Quantity < 0);
                }

                return isReturn;
            }

            /// <summary>
            /// Gets sum of discounts amount and their fiscal texts.
            /// </summary>
            /// <param name="discountLines">The discount lines.</param>
            /// <param name="discountTexts">The list with texts of discounts.</param>
            /// <returns>A tuple with amount of discount and fiscal texts. The Item1 of tuple is a sum of discounts, Item2 is a text of discount.</returns>
            public static Tuple<decimal, string> GetLineDiscountDetails(SalesLine salesLine, List<FiscalIntegrationSalesDiscountFiscalText> discountTexts)
            {
                decimal discountAmount = salesLine
                    .DiscountLines
                    .Sum(discount => discount.EffectiveAmount);

                return Tuple.Create(discountAmount, SalesOrderHelper.GetFiscalTextForDiscount(salesLine, discountTexts));
            }

            /// <summary>
            /// Gets total amount of change for sales order.
            /// </summary>
            /// <param name="salesOrder">The sales order.</param>
            /// <returns>Amount of change.</returns>
            public static decimal GetTotalChangeAmount(SalesOrder salesOrder)
            {
                decimal changeAmount = salesOrder.ActiveTenderLines
                    .Where(x => x.IsChangeLine == true)
                    .Sum(x => x.Amount);

                if (changeAmount != decimal.Zero)
                {
                    changeAmount = Math.Abs(Math.Round(changeAmount, AmountFractionalDigits)); ;
                }

                return changeAmount;
            }

            /// <summary>
            /// Gets total value of currency for sales order.
            /// </summary>
            /// <param name="salesOrder">The sales order.</param>
            /// <returns>Paid amount.</returns>
            public static decimal GetTotalPaidAmount(SalesOrder salesOrder)
            {
                decimal paidAmount = salesOrder.ActiveTenderLines
                    .Where(x => x.IsChangeLine == false)
                    .Sum(x => x.Amount);

                paidAmount += SalesOrderHelper.GetPrepaymentDepositAmount(salesOrder);

                return paidAmount;
            }

            /// <summary>
            /// Gets fiscal value of currency for sales order.
            /// </summary>
            /// <param name="salesOrder">The sales order.</param>
            /// <returns>Paid amount.</returns>
            public static decimal GetFiscalAmount(SalesOrder salesOrder)
            {
                decimal fiscalAmount = salesOrder.ActiveTenderLines.Sum(x => x.Amount);

                fiscalAmount += SalesOrderHelper.GetPrepaymentDepositAmount(salesOrder);

                return fiscalAmount;
            }

            /// <summary>
            /// Get products by their item IDs from sales lines.
            /// </summary>
            /// <param name="salesLines">The sales lines.</param>
            /// <param name="context">The request context.</param>
            /// <returns>The list of products.</returns>
            public static async Task<ReadOnlyCollection<Item>> GetProductsInSalesLinesAsync(IEnumerable<SalesLine> salesLines, RequestContext context)
            {
                ThrowIf.Null(context, nameof(context));
                ThrowIf.Null(context, nameof(salesLines));

                IEnumerable<string> itemIds = salesLines
                    .Select(l => l.ItemId)
                    .ToList();

                GetItemsDataRequest getItemsDataRequest = new GetItemsDataRequest(itemIds);
                GetItemsDataResponse getItemsResponse = await context.ExecuteAsync<GetItemsDataResponse>(getItemsDataRequest).ConfigureAwait(false);

                ReadOnlyCollection<Item> items = getItemsResponse.Items;

                return items;
            }            

            /// <summary>
            /// Gets the adjusted sales order and document.
            /// </summary>
            /// <param name="request">The sales order to adjust.</param>
            /// <returns>A tuple with adjusted sales order and document.</returns>
            public static async Task<Tuple<SalesOrder, FiscalIntegrationDocumentAdjustment>> AdjustSalesOrderAsync(GetFiscalDocumentDocumentProviderRequest request)
            {
                var adjustmentType =
                    FiscalIntegrationSalesOrderAdjustmentType.ExcludeGiftCards |
                    FiscalIntegrationSalesOrderAdjustmentType.ExcludeDeposit;

                var getAdjustedSalesOrderRequest = new GetAdjustedSalesOrderFiscalIntegrationServiceRequest(request.SalesOrder, adjustmentType);
                var response = await request.RequestContext.ExecuteAsync<GetAdjustedSalesOrderFiscalIntegrationServiceResponse>(getAdjustedSalesOrderRequest).ConfigureAwait(false);

                return Tuple.Create(response.SalesOrder, response.DocumentAdjustment);
            }

            /// <summary>
            /// Get the product name by its item ID.
            /// </summary>
            /// <param name="itemId">The product item ID.</param>
            /// <param name="products">The list of products in the sales order.</param>
            /// <returns>The product name.</returns>
            public static string GetProductNameByItemID(string itemId, ReadOnlyCollection<Item> products)
            {
                ThrowIf.Null(products, nameof(products));

                string productName = products
                    .Where(item => item.ItemId == itemId)
                    .Select(item =>
                    {
                        string productNameToPrint = GetProductNameWitoutNonPrintingSymbols(item.Name);
                        if (!string.IsNullOrWhiteSpace(productNameToPrint))
                        {
                            return productNameToPrint;
                        }

                        productNameToPrint = GetProductNameWitoutNonPrintingSymbols(item.NameAlias);
                        if (!string.IsNullOrWhiteSpace(productNameToPrint))
                        {
                            return productNameToPrint;
                        }

                        return item.ItemId;

                    }).FirstOrDefault();

                return productName.Substring(0, Math.Min(productName.Length, LengthOfProductName));
            }

            /// <summary>
            /// Gets the prepayment amount of customer order.
            /// </summary>
            /// <param name="salesOrder">The sales order.</param>
            /// <returns>The prepayment amount.</returns>
            public static decimal GetPrepaymentDepositAmount(SalesOrder salesOrder)
            {
                if (SalesOrderHelper.IsPrepaymentAmountAppliedOnPickup(salesOrder))
                    return Math.Abs(Math.Round(salesOrder.PrepaymentAmountAppliedOnPickup, SalesOrderHelper.AmountFractionalDigits));
                else
                    return decimal.Zero;
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
            /// Get fiscal texts of each discount in the sales order.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <returns>The list of discount fiscal texts.</returns>
            public static List<FiscalIntegrationSalesDiscountFiscalText> GetDiscountFiscalTextForSalesOrder(GetFiscalDocumentDocumentProviderRequest request)
            {
                ThrowIf.Null(request, nameof(request));
                ThrowIf.Null(request.SalesOrder, nameof(request.SalesOrder));

                GetFiscalTextsFromSalesOrderFiscalIntegrationServiceRequest fiscalTextRequest = new GetFiscalTextsFromSalesOrderFiscalIntegrationServiceRequest(request.SalesOrder, request.FiscalIntegrationFunctionalityProfileGroupId);
                GetFiscalTextsFromSalesOrderFiscalIntegrationServiceResponse fiscalTextResponse = request.RequestContext.Runtime.Execute<GetFiscalTextsFromSalesOrderFiscalIntegrationServiceResponse>(fiscalTextRequest, request.RequestContext);

                return fiscalTextResponse.SalesOrderDiscountsFiscalTexts;
            }

            /// <summary>
            /// Gets whether the customer order is a pick up or return order.
            /// </summary>
            /// <param name="salesOrder"></param>
            /// <returns>True if the sales order is customer order pick up or return, otherwise false.</returns>
            public static bool IsCustomerOrderPickupOrReturn(SalesOrder salesOrder)
            {
                ThrowIf.Null(salesOrder, nameof(salesOrder));

                return salesOrder.ExtensibleSalesTransactionType == ExtensibleSalesTransactionType.CustomerOrder
                    && (salesOrder.CustomerOrderMode == CustomerOrderMode.Pickup
                     || salesOrder.CustomerOrderMode == CustomerOrderMode.Return);
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
            /// Gets filtered sales lines for customer orders and sales.
            /// </summary>
            /// <param name="salesOrder">The sales order.</param>
            /// <param name="request">The request.</param>
            /// <returns>The collection of sales lines.</returns>
            public static IEnumerable<SalesLine> GetSalesLines(SalesOrder salesOrder, GetFiscalDocumentDocumentProviderRequest request)
            {
                return salesOrder.ActiveSalesLines
                    .Where(salesLine => salesOrder.ExtensibleSalesTransactionType == ExtensibleSalesTransactionType.Sales
                                        || salesOrder.ExtensibleSalesTransactionType == ExtensibleSalesTransactionType.SalesInvoice
                                        || SalesOrderHelper.IsCustomerOrderCreateOrEdit(salesOrder) && salesLine.DeliveryMode == request.RequestContext.GetChannelConfiguration().CarryoutDeliveryModeCode
                                        || SalesOrderHelper.IsCustomerOrderPickupOrReturn(salesOrder) && salesLine.Quantity != 0);
            }

            /// <summary>
            /// Gets fiscal texts for discounts per sales line.
            /// </summary>
            /// <param name="salesLine">The sales line.</param>
            /// <param name="discountTexts">Collection of fiscal texts of discounts.</param>
            /// <returns>The text for discounts.</returns>
            private static string GetFiscalTextForDiscount(SalesLine salesLine, List<FiscalIntegrationSalesDiscountFiscalText> discountTexts)
            {
                List<FiscalIntegrationSalesDiscountFiscalText> printDiscountFiscalTexts = new List<FiscalIntegrationSalesDiscountFiscalText>();

                // match periodic discount
                printDiscountFiscalTexts.AddRange(discountTexts.Where(line =>
                    line.SalesLineNumber == salesLine.LineNumber
                    && line.DiscountLineType == DiscountLineType.PeriodicDiscount));

                // match manual line discount
                printDiscountFiscalTexts.AddRange(discountTexts.Where(line =>
                    line.SalesLineNumber == salesLine.LineNumber
                    && line.DiscountLineType == DiscountLineType.ManualDiscount
                    && (line.ManualDiscountType == ManualDiscountType.LineDiscountAmount
                    || line.ManualDiscountType == ManualDiscountType.LineDiscountPercent)));

                // match manual total discount
                printDiscountFiscalTexts.AddRange(discountTexts.Where(line =>
                    line.SalesLineNumber == salesLine.LineNumber
                    && line.DiscountLineType == DiscountLineType.ManualDiscount
                    && (line.ManualDiscountType == ManualDiscountType.TotalDiscountAmount
                    || line.ManualDiscountType == ManualDiscountType.TotalDiscountPercent)));

                string fiscalText = string.Join(" ",
                    printDiscountFiscalTexts
                        .Where(text => text.DiscountFiscalTexts.Count > 0)
                        .Select(x => x.ToString()));

                if (!string.IsNullOrEmpty(fiscalText))
                {
                    fiscalText = fiscalText.Substring(0, Math.Min(fiscalText.Length, LengthOfDiscountName));
                }

                return fiscalText;
            }

            /// <summary>
            /// Gets product name without "control" and "symbol" character categories by replacing its with spaces.
            /// </summary>
            /// <param name="productName">The name of product.</param>
            /// <returns>The product name with spaces insted "control" and "symbol" character categories.</returns>
            private static string GetProductNameWitoutNonPrintingSymbols(string productName)
            {
                return productName == null ? string.Empty : Regex.Replace(productName, @"\p{C}|\p{S}", " ").Trim();
            }
        }
    }
}
