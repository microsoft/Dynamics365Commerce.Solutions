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
        using System.Collections.ObjectModel;
        using System.Linq;
        using System.Threading.Tasks;
        using Contoso.CommerceRuntime.DocumentProvider.AtolSample.DataModel.AtolTask;
        using Contoso.CommerceRuntime.DocumentProvider.AtolSample.Messages;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
        using Microsoft.Dynamics.Commerce.Runtime.Framework.Serialization;
        using Microsoft.Dynamics.Commerce.Runtime.Localization.Data.Services.Messages.Russia;
        using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;

        /// <summary>
        /// Fiscal document builder.
        /// </summary>
        public static class DocumentBuilder
        {
            /// <summary>
            /// Builds fiscal receipt document.
            /// </summary>
            /// <param name="requestContext">The request context.</param>
            /// <param name="salesOrder">The sales order.</param>
            /// <param name="fiscalIntegrationFunctionalityProfile">The fiscal integration functionality profile.</param>
            /// <returns>The builded fiscal document string.</returns>
            public static async Task<string> Build(RequestContext requestContext, SalesOrder salesOrder, FiscalIntegrationFunctionalityProfile fiscalIntegrationFunctionalityProfile)
            {
                if (DocumentBuilder.CheckSaleAndReturnInOneSalesOrder(salesOrder))
                {
                    throw new DataValidationException(DataValidationErrors.Microsoft_Dynamics_Commerce_Runtime_MixingSalesAndReturnsProhibited, "There was an error generating a fiscal receipt. A fiscal receipt cannot include both sales and returns.");
                }

                var receipt = await BuildReceipt(requestContext, salesOrder, fiscalIntegrationFunctionalityProfile).ConfigureAwait(false);

                SerializeToAtolCommandRequest serializeToAtolCommandRequest = new SerializeToAtolCommandRequest(receipt);
                var serializeToAtolCommandResponse = await requestContext.ExecuteAsync<SerializeToAtolCommandResponse>(serializeToAtolCommandRequest).ConfigureAwait(false);

                return serializeToAtolCommandResponse.SerializedCommand;
            }

            /// <summary>
            /// Builds reciept.
            /// </summary>
            /// <param name="requestContext">The request context.</param>
            /// <param name="salesOrder">The sales order.</param>
            /// <param name="fiscalIntegrationFunctionalityProfile">The fiscal integration functionality profile.</param>
            /// <returns>Filled receipt sales orders.</returns>
            private static async Task<DataModel.AtolTask.Receipt> BuildReceipt(RequestContext requestContext, SalesOrder salesOrder, FiscalIntegrationFunctionalityProfile fiscalIntegrationFunctionalityProfile)
            {
                DataModel.AtolTask.Receipt receipt = new DataModel.AtolTask.Receipt();

                receipt.Type = IsReturnSalesOrder(salesOrder) ?
                    DataModel.AtolTask.ReceiptType.SellReturn :
                    DataModel.AtolTask.ReceiptType.Sell;

                receipt.Operator = await GetOperator(requestContext, salesOrder).ConfigureAwait(false);

                await FillCustomer(requestContext, salesOrder, receipt).ConfigureAwait(false);
                await FillPositions(requestContext, salesOrder, receipt).ConfigureAwait(false);
                await FillPayments(requestContext, receipt, salesOrder, fiscalIntegrationFunctionalityProfile).ConfigureAwait(false);
                FillFooter(salesOrder, receipt);

                return receipt;
            }

            /// <summary>
            /// Fills customer.
            /// </summary>
            /// <param name="requestContext">The request context.</param>
            /// <param name="salesOrder">The sales order.</param>
            /// <param name="receipt">The receipt for filling.</param>
            private static async Task FillCustomer(RequestContext requestContext, SalesOrder salesOrder, DataModel.AtolTask.Receipt receipt)
            {
                var getFiscalCustomerAdditionalInfoDataRequest = new GetFiscalCustomerAdditionalInfoDataRequest(salesOrder.Id);
                var getFiscalCustomerAdditionalInfoDataResponse = await requestContext.ExecuteAsync<SingleEntityDataServiceResponse<string>>(getFiscalCustomerAdditionalInfoDataRequest).ConfigureAwait(false);
                var emailOrPhone = JsonHelper.Deserialize<string>(getFiscalCustomerAdditionalInfoDataResponse.Entity);
                if (!string.IsNullOrEmpty(salesOrder.CustomerId) || !string.IsNullOrEmpty(emailOrPhone))
                {
                    receipt.Customer = new DataModel.AtolTask.Customer
                    {
                        Name = !string.IsNullOrEmpty(salesOrder.CustomerId) ? salesOrder.CustomerName : null,
                        EmailOrPhone = emailOrPhone,
                    };
                }
            }

            /// <summary>
            /// Checks if the sales order is a mixed sales order with a sale and return.
            /// </summary>
            /// <param name="salesOrder">The sales order.</param>
            /// <returns>True if the receipt contains a sale and a return, otherwise, false.</returns>
            private static bool CheckSaleAndReturnInOneSalesOrder(SalesOrder salesOrder)
            {
                return salesOrder.ActiveSalesLines.Any(x => x.Quantity > 0) &&
                    salesOrder.ActiveSalesLines.Any(x => x.Quantity < 0);
            }

            /// <summary>
            /// Fills in the footer of the receipt.
            /// </summary>
            /// <param name="salesOrder">The sales order.</param>
            /// <param name="receipt">The receipt for filling.</param>
            private static void FillFooter(SalesOrder salesOrder, DataModel.AtolTask.Receipt receipt)
            {
                AddBarcodeWithReceiptId(salesOrder, receipt);
            }

            /// <summary>
            /// Add the receipt identifier barcode to the receipt footer.
            /// </summary>
            /// <param name="salesOrder">The sales order.</param>
            /// <param name="receipt">The receipt for filling.</param>
            private static void AddBarcodeWithReceiptId(SalesOrder salesOrder, DataModel.AtolTask.Receipt receipt)
            {
                BarcodeItem barcode = new BarcodeItem
                {
                    Alignment = AlignmentType.Center,
                    BarcodeType = BarCodeType.Code128,
                    BarcodeValue = salesOrder.ReceiptId,
                    PrintText = true,
                    Height = 60,
                };

                receipt.PostItems.Add(barcode);
            }

            /// <summary>
            /// Gets operator.
            /// </summary>
            /// <param name="requestContext">The request context.</param>
            /// <param name="salesOrder">The sales order.</param>
            /// <returns>Operator name.</returns>
            private static async Task<Operator> GetOperator(RequestContext requestContext, SalesOrder salesOrder)
            {
                GetEmployeesServiceRequest getEmployeeRequest = new GetEmployeesServiceRequest(salesOrder.StaffId, QueryResultSettings.SingleRecord);
                GetEmployeesServiceResponse employeeResponse = await requestContext.ExecuteAsync<GetEmployeesServiceResponse>(getEmployeeRequest).ConfigureAwait(false);

                Employee employee = employeeResponse.Employees.SingleOrDefault();

                return new Operator
                {
                    Name = employee?.Name ?? string.Empty,
                };
            }

            /// <summary>
            /// Fills receipt positions.
            /// </summary>
            /// <param name="requestContext">The request context.</param>
            /// <param name="salesOrder">The sales order.</param>
            /// <param name="receipt">The receipt for filling.</param>
            private static async Task FillPositions(RequestContext requestContext, SalesOrder salesOrder, DataModel.AtolTask.Receipt receipt)
            {
                IEnumerable<SalesLine> salesLines = salesOrder.ActiveSalesLines;
                IEnumerable<string> itemIds = salesLines.Select(l => l.ItemId);
                ReadOnlyCollection<Item> products = await GetProductsByItemIdAsync(requestContext, itemIds).ConfigureAwait(false);

                foreach (var salesLine in salesLines)
                {
                    var itemPosition = await BuildPosition(requestContext, salesOrder, salesLine, products).ConfigureAwait(false);
                    receipt.Items.Add(itemPosition);
                }
            }

            /// <summary>
            /// Gets products by their item IDs.
            /// </summary>
            /// <param name="requestContext">The request context.</param>
            /// <param name="itemIds">Product's item identifiers.</param>
            /// <returns>The list of products.</returns>
            private static async Task<ReadOnlyCollection<Item>> GetProductsByItemIdAsync(RequestContext requestContext, IEnumerable<string> itemIds)
            {
                GetItemsDataRequest getItemsDataRequest = new GetItemsDataRequest(itemIds);
                GetItemsDataResponse getItemsResponse = await requestContext.ExecuteAsync<GetItemsDataResponse>(getItemsDataRequest).ConfigureAwait(false);

                ReadOnlyCollection<Item> items = getItemsResponse.Items;

                return items;
            }

            /// <summary>
            /// Builds receipt position.
            /// </summary>
            /// <param name="requestContext">The request context.</param>
            /// <param name="salesOrder">The sales order.</param>
            /// <param name="salesLine">The sales line.</param>
            /// <param name="products">The products of sales line.</param>
            /// <returns>Filled receipt position.</returns>
            private static async Task<PositionItem> BuildPosition(RequestContext requestContext, SalesOrder salesOrder, SalesLine salesLine, ReadOnlyCollection<Item> products)
            {
                string productName = GetProductNameByItemId(salesLine.ItemId, products);
                string description = description = salesLine.ItemId + " " + productName;
                GetProductsDataRequest productDataRequest = new GetProductsDataRequest(new long[] { salesLine.ProductId }, QueryResultSettings.SingleRecord);
                var response = await requestContext.ExecuteAsync<EntityDataServiceResponse<SimpleProduct>>(productDataRequest).ConfigureAwait(false);

                var itemType = response.PagedEntityCollection.Results[0].ItemType == ReleasedProductType.Service
                    ? PaymentObjectType.Service
                    : PaymentObjectType.Commodity;

                var taxItem = GetTax(salesLine);

                PositionItem position = new PositionItem
                {
                    Name = description,
                    Price = Math.Abs(salesLine.NetAmountWithAllInclusiveTaxPerUnit),
                    Quantity = Math.Abs(salesLine.Quantity),
                    Amount = Math.Abs(salesLine.TotalAmount),
                    InfoDiscount = Math.Abs(salesLine.DiscountAmount),
                    MeasurementUnit = salesLine.UnitOfMeasureSymbol,
                    PaymentObject = itemType,
                    Tax = taxItem,
                };

                return position;
            }

            /// <summary>
            /// Gets tax for selas line.
            /// </summary>
            /// <param name="salesLine">The sales line.</param>
            /// <returns>Tax for receipt position.</returns>
            private static Tax GetTax(SalesLine salesLine)
            {
                return new Tax
                {
                    TaxType = TaxTypeResolver.GetTaxType(salesLine),
                };
            }

            /// <summary>
            /// Gets the product name by its item identifier.
            /// </summary>
            /// <param name="itemId">The product item identifiers.</param>
            /// <param name="products">The list of products in the sales order.</param>
            /// <returns>The product name.</returns>
            private static string GetProductNameByItemId(string itemId, ReadOnlyCollection<Item> products)
            {
                string productName = string.Empty;
                List<string> namesList = (from item in products
                                          where item.ItemId == itemId
                                          select item.Name).ToList();

                if (namesList.Count > 0)
                {
                    productName = namesList.First();
                }

                return productName;
            }

            /// <summary>
            /// Fills payment receipt.
            /// </summary>
            /// <param name="requestContext">The request.</param>
            /// <param name="receipt">The receipt for filling.</param>
            /// <param name="salesOrder">The sales order.</param>
            /// <param name="fiscalIntegrationFunctionalityProfile">The fiscal integration functionality profile.</param>
            private static async Task FillPayments(RequestContext requestContext, DataModel.AtolTask.Receipt receipt, SalesOrder salesOrder, FiscalIntegrationFunctionalityProfile fiscalIntegrationFunctionalityProfile)
            {
                GetPaymentsForSalesOrderAtolRequest getPaymentsForSalesOrderAtolRequest = new GetPaymentsForSalesOrderAtolRequest(fiscalIntegrationFunctionalityProfile, salesOrder);
                var getPaymentsForSalesOrderAtolResponse = await requestContext.ExecuteAsync<GetPaymentsForSalesOrderAtolResponse>(getPaymentsForSalesOrderAtolRequest).ConfigureAwait(false);
                var payments = getPaymentsForSalesOrderAtolResponse.Payments
                    .GroupBy(x => x.PaymentMethod)
                    .Select(x => new Payment
                    {
                        PaymentMethod = x.Key,
                        Sum = x.Sum(y => y.Sum),
                    });
                receipt.Payments.AddRange(payments);
                receipt.Total = Math.Abs(salesOrder.AmountPaid);
            }

            /// <summary>
            /// Checks that a sales order is a return order.
            /// </summary>
            /// <param name="salesOrder">The sales order.</param>
            /// <returns>True if the sales order is a return order, otherwise, false.</returns>
            private static bool IsReturnSalesOrder(SalesOrder salesOrder)
            {
                bool isReturnTransaction = false;

                if (salesOrder.ExtensibleSalesTransactionType == ExtensibleSalesTransactionType.CustomerOrder)
                {
                    isReturnTransaction = salesOrder.CustomerOrderMode == CustomerOrderMode.Return;
                }
                else
                {
                    isReturnTransaction = salesOrder.IsReturnByReceipt;
                }

                isReturnTransaction = isReturnTransaction && salesOrder.ActiveSalesLines.All(salesLine => salesLine.ReturnTransactionId != string.Empty);

                isReturnTransaction = isReturnTransaction || salesOrder.ActiveSalesLines.All(salesLine => salesLine.Quantity < 0);

                return isReturnTransaction;
            }
        }
    }
}
