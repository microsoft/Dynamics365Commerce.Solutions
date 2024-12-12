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
        using System.Collections.ObjectModel;
        using System.Linq;
        using System.Threading.Tasks;
        using System.Xml.Linq;
        using Contoso.CommerceRuntime.DocumentProvider.EpsonFP90IIISample.Constants;
        using Contoso.CommerceRuntime.DocumentProvider.EpsonFP90IIISample.Helpers;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.Localization.Entities.Italy;
        using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
        using Microsoft.Dynamics.Commerce.Runtime.FiscalIntegration.DocumentProvider.Messages;
        using Microsoft.Dynamics.Commerce.Runtime.Localization.Data.Services.Messages.Italy;

        /// <summary>
        /// Generates fiscal receipt document.
        /// </summary>
        public static class DocumentBuilder
        {
            /// <summary>
            /// Builds fiscal receipt document.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <param name="adjustedSalesOrder">The adjusted sales order.</param>
            /// <returns>The generated fiscal document string.</returns>
            public static async Task<string> BuildAsync(GetFiscalDocumentDocumentProviderRequest request, SalesOrder adjustedSalesOrder)
            {
                ThrowIf.Null(request, nameof(request));
                ThrowIf.Null(adjustedSalesOrder, nameof(adjustedSalesOrder));

                XDocument doc = new XDocument();
                XElement rootElement = DocumentElementBuilder.XElement(DocumentElementConstants.RootForSalesOrder);

                rootElement = await BuildStartElementAsync(request, adjustedSalesOrder, rootElement).ConfigureAwait(false);
                rootElement = await BuildBodyAsync(request, adjustedSalesOrder, rootElement).ConfigureAwait(false);
                rootElement = await BuildFooterAsync(request, adjustedSalesOrder, rootElement).ConfigureAwait(false);
                rootElement = BuildEndElement(request, rootElement);

                doc.Add(rootElement);

                return doc.ToString();
            }

            /// <summary>
            /// Builds start element for the fiscal receipt document.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <param name="adjustedSalesOrder">The adjusted sales order.</param>
            /// <param name="rootElement">The document root element.</param>
            /// <returns>The updated document root element.</returns>
            public static async Task<XElement> BuildStartElementAsync(GetFiscalDocumentDocumentProviderRequest request, SalesOrder adjustedSalesOrder, XElement rootElement)
            {
                ThrowIf.Null(request, nameof(request));
                ThrowIf.Null(adjustedSalesOrder, nameof(adjustedSalesOrder));

                // Company logo, it will upload manually via tool provided by Epson. Do not print if chosen "false" value of "Print fiscal data in reseipt header" property in configuration.
                if (ConfigurationController.ParsePrintFiscalDatatInReceiptHeader(request.FiscalIntegrationFunctionalityProfile))
                {
                    // Company name, name of the Operating unit linked with the current store.
                    string DeviceNumber = request.RequestContext.GetPrincipal()?.DeviceNumber ?? string.Empty;
                    foreach (string splitString in StringProcessor.SplitStringByWords(DeviceNumber, DocumentAttributeConstants.MaxHeaderAttributeLength).Take(DocumentAttributeConstants.ReceiptFieldStoreAddressLinesLimit))
                    {
                        rootElement = DocumentElementBuilder.BuildPrintRecMessageElement(rootElement, splitString, DocumentAttributeConstants.ReceiptHeaderMessageType, GetNextHeaderLineIndex(rootElement).ToString());
                    }

                    // Address
                    string OrgUnitFullAddress = request.RequestContext.GetOrgUnit()?.OrgUnitFullAddress ?? string.Empty;
                    foreach (string splitString in StringProcessor.SplitStringByWords(OrgUnitFullAddress.Replace("\n", DocumentAttributeConstants.Space), DocumentAttributeConstants.MaxHeaderAttributeLength).Take(DocumentAttributeConstants.ReceiptFieldCompanyLinesLimit))
                    {
                        rootElement = DocumentElementBuilder.BuildPrintRecMessageElement(rootElement, splitString, DocumentAttributeConstants.ReceiptHeaderMessageType, GetNextHeaderLineIndex(rootElement).ToString());
                    }

                    // VAT number, the tax identification number (TIN) specified on the Store.
                    rootElement = await FillStoreVATNumberAsync(request.RequestContext, rootElement).ConfigureAwait(false);

                    // Cashier name
                    string UserId = request.RequestContext.GetPrincipal()?.UserId ?? string.Empty;
                    rootElement = DocumentElementBuilder.BuildPrintRecMessageElement(rootElement, UserId, DocumentAttributeConstants.ReceiptHeaderMessageType, GetNextHeaderLineIndex(rootElement).ToString());
                }

                // Adds customer name and sales ID above receipt header if the sales order is customer order.
                if (adjustedSalesOrder.ExtensibleSalesTransactionType == ExtensibleSalesTransactionType.CustomerOrder)
                {
                    rootElement = await CustomerOrderDocumentBuilder.BuildHeaderAsync(request, rootElement, adjustedSalesOrder).ConfigureAwait(false);
                }

                rootElement = await BuildRefundStartElementAsync(request, adjustedSalesOrder, rootElement).ConfigureAwait(false);

                XElement beginFiscalReceipt = DocumentElementBuilder.XElement(DocumentElementConstants.BeginFiscalReceipt);
                beginFiscalReceipt.Add(new XAttribute(DocumentAttributeConstants.Operator, DocumentAttributeConstants.DefaultOperatorId));
                rootElement.Add(beginFiscalReceipt);

                return rootElement;
            }

            /// <summary>
            /// Builds end element for the fiscal receipt document.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <param name="rootElement">The document root element.</param>
            /// <returns>The updated document root element.</returns>
            public static XElement BuildEndElement(GetFiscalDocumentDocumentProviderRequest request, XElement rootElement)
            {
                ThrowIf.Null(request, nameof(request));
                ThrowIf.Null(rootElement, nameof(rootElement));

                XElement endFiscalReceipt = DocumentElementBuilder.XElement(DocumentElementConstants.EndFiscalReceipt);
                endFiscalReceipt.Add(DocumentElementBuilder.XAttribute(DocumentAttributeConstants.Operator, DocumentAttributeConstants.DefaultOperatorId));

                rootElement.Add(endFiscalReceipt);

                return rootElement;
            }

            /// <summary>
            /// Gets number of the next header row after existing, if receipt has no rows in header returns 1.
            /// </summary>
            /// <param name="parentelement"> The parent element.</param>
            /// <returns>Index of the next line in header</returns>
            public static int GetNextHeaderLineIndex(XElement parentElement)
            {
                int rowIndex;

                int.TryParse((from c in parentElement.Elements("printRecMessage")
                              where c.Attribute("messageType").Value == "1"
                              select c.Attribute("index").Value).Max(), out rowIndex);

                return rowIndex + 1;
            }

            /// <summary>
            /// Builds end element for the fiscal receipt document.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <param name="adjustedSalesOrder">The adjusted sales order.</param>
            /// <param name="rootElement">The document root element.</param>
            /// <returns>The updated document root element.</returns>
            public static async Task<XElement> BuildRefundStartElementAsync(GetFiscalDocumentDocumentProviderRequest request, SalesOrder adjustedSalesOrder, XElement rootElement)
            {
                ThrowIf.Null(adjustedSalesOrder, nameof(adjustedSalesOrder));
                ThrowIf.Null(request, nameof(request));
                ThrowIf.Null(request.RequestContext, nameof(request.RequestContext));
                ThrowIf.Null(rootElement, nameof(rootElement));

                bool isReturnByTransaction = SalesOrderHelper.IsReturnByTransaction(adjustedSalesOrder);
                bool isReturn = isReturnByTransaction || SalesOrderHelper.IsReturnProduct(adjustedSalesOrder);

                if (isReturn)
                {
                    // Refund messages format : REFUND zzzz nnnn ddmmyyyy sssssssssss.
                    var returnOriginMapping = ConfigurationController.ParseReturnOriginMappings(request.FiscalIntegrationFunctionalityProfile);
                    var refundMessage = string.Empty;

                    if (isReturnByTransaction && adjustedSalesOrder.CheckActiveSalesLinesReturnTransactionId())
                    {
                        refundMessage = await GetRefundMessageByReturnOriginalTransaction(request, adjustedSalesOrder, returnOriginMapping.PrinterReturnOriginWithoutFiscalData).ConfigureAwait(false);
                    }
                    else
                    {
                        refundMessage = await GetRefundMessageWithoutReturnOriginalTransaction(request, adjustedSalesOrder, returnOriginMapping).ConfigureAwait(false);
                    }

                    rootElement = DocumentElementBuilder.BuildPrintRecMessageElement(rootElement, refundMessage, DocumentAttributeConstants.DefaultMessageType);
                }

                return rootElement;
            }

            /// <summary>
            /// Builds the body section for the fiscal receipt document.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <param name="adjustedSalesOrder">The adjusted sales order.</param>
            /// <param name="rootElement">The document root element.</param>
            /// <returns>The updated document root element.</returns>
            private static async Task<XElement> BuildBodyAsync(GetFiscalDocumentDocumentProviderRequest request, SalesOrder adjustedSalesOrder, XElement rootElement)
            {
                ThrowIf.Null(request, nameof(request));
                ThrowIf.Null(adjustedSalesOrder, nameof(adjustedSalesOrder));

                IEnumerable<SalesLine> salesLines = adjustedSalesOrder.ActiveSalesLines;
                IEnumerable<string> itemIds = salesLines.Select(l => l.ItemId);
                ReadOnlyCollection<Item> products = await SalesOrderHelper.GetProductsByItemIDAsync(request.RequestContext, itemIds).ConfigureAwait(false);
                var discountFiscalTexts = new Lazy<Task<List<FiscalIntegrationSalesDiscountFiscalText>>>(async () => await SalesOrderHelper.GetDiscountFiscalTextForSalesOrderAsync(request, adjustedSalesOrder).ConfigureAwait(false));
                IDepartmentResolver departmentResolver = new DepartmentResolverFactory(request.FiscalIntegrationFunctionalityProfile).Build();
                var documentElementAdjustmentBuilderFactory = new DocumentElementAdjustmentBuilderFactory(request.FiscalIntegrationFunctionalityProfile);
                var documentElementAdjustmentBuilder = documentElementAdjustmentBuilderFactory.Build();

                // Lines
                foreach (var salesLine in salesLines)
                {
                    ChannelConfiguration channelConfiguration = request.RequestContext.GetChannelConfiguration();

                    // For customer order, should print standard fiscal receipt for pick up operation and for the lines with delivery mode "carry out" when create or edit customer order.
                    bool isCarryingOutLine = SalesOrderHelper.IsCustomerOrderCreateOrEdit(adjustedSalesOrder) && salesLine.DeliveryMode == channelConfiguration.CarryoutDeliveryModeCode;

                    // For customer order, should print standard fiscal receipt for customer order pick up lines and return lines that has quantity.
                    bool isPickingUpOrReturningLine = await SalesOrderHelper.IsCustomerOrderPickupOrReturn(request.RequestContext, adjustedSalesOrder, channelConfiguration).ConfigureAwait(false) && salesLine.Quantity != 0;

                    if (adjustedSalesOrder.ExtensibleSalesTransactionType == ExtensibleSalesTransactionType.Sales
                        || adjustedSalesOrder.ExtensibleSalesTransactionType == ExtensibleSalesTransactionType.SalesInvoice
                        || isCarryingOutLine
                        || isPickingUpOrReturningLine)
                    {
                        string department = await departmentResolver.GetDepartmentNumberAsync(request.RequestContext, Math.Round(salesLine.TaxRatePercent, 2), adjustedSalesOrder.ReceiptId, salesLine.ProductId, salesLine.IsGiftCardLine).ConfigureAwait(false);

                        // Item code, Item name, Quantity
                        rootElement = await DocumentElementBuilder.BuildPrintRecItemOrRecFundElementAsync(rootElement, adjustedSalesOrder, salesLine, products, request, department).ConfigureAwait(false);

                        // Amount, a fiscal printer calculates amount automatically.

                        // Additional description of discounts for the fiscal receipt
                        string fiscalText = string.Empty;

                        if (salesLine.DiscountAmount != decimal.Zero && salesLine.DiscountLines.Any())
                        {
                            List<FiscalIntegrationSalesDiscountFiscalText> printDiscountFiscalTexts = new List<FiscalIntegrationSalesDiscountFiscalText>();
                            var discountFiscalTextsValue = await discountFiscalTexts.Value.ConfigureAwait(false);
                            printDiscountFiscalTexts.AddRange(discountFiscalTextsValue.Where(line =>
                            line.SalesLineNumber == salesLine.LineNumber
                            && line.DiscountLineType == DiscountLineType.PeriodicDiscount));
                            printDiscountFiscalTexts.AddRange(discountFiscalTextsValue.Where(line =>
                            line.SalesLineNumber == salesLine.LineNumber
                            && line.DiscountLineType == DiscountLineType.ManualDiscount
                            && (line.ManualDiscountType == ManualDiscountType.LineDiscountAmount
                            || line.ManualDiscountType == ManualDiscountType.LineDiscountPercent)));
                            printDiscountFiscalTexts.AddRange(discountFiscalTextsValue.Where(line =>
                            line.SalesLineNumber == salesLine.LineNumber
                            && line.DiscountLineType == DiscountLineType.ManualDiscount
                            && (line.ManualDiscountType == ManualDiscountType.TotalDiscountAmount
                            || line.ManualDiscountType == ManualDiscountType.TotalDiscountPercent)));

                            fiscalText = string.Join(" ", printDiscountFiscalTexts.Where(text => text.DiscountFiscalTexts.Count > 0).Select(x => x.ToString()));
                            fiscalText = string.IsNullOrWhiteSpace(fiscalText) ? string.Empty : fiscalText;
                        }

                        //According to the legislation, we have to print all the legal text for discounts in receipt.
                        //We are splitting text by lines, because length of the text field is limited by printer.
                        List<string> splittedFiscalText = StringProcessor.SplitStringByWords(fiscalText, DocumentAttributeConstants.MaxAttributeLength);

                        foreach (string textPart in splittedFiscalText.Take(splittedFiscalText.Count - 1))
                        {
                            rootElement = DocumentElementBuilder.BuildPrintRecMessageElement(rootElement, textPart, DocumentAttributeConstants.DefaultMessageType);
                        }

                        rootElement = documentElementAdjustmentBuilder.BuildPrintRecItemOrSubtotalAdjustment(salesLine, rootElement, splittedFiscalText.LastOrDefault(), department);
                    }
                }

                // Payments
                rootElement = await PaymentInfoBuilder.FillPaymentsAsync(rootElement, request, adjustedSalesOrder).ConfigureAwait(false);

                return rootElement;
            }

            /// <summary>
            /// Gets the department value for element attribute.
            /// </summary>
            /// <param name="request">GetFiscalDocumentDocumentProvider request.</param>
            /// <param name="adjustedSalesOrder">Sales order.</param>
            /// <param name="salesLine">Sales line of sales order.</param>
            /// <returns>The department string value.</returns>
            private static string GetDepartmentValue(GetFiscalDocumentDocumentProviderRequest request, SalesOrder adjustedSalesOrder, SalesLine salesLine)
            {
                // VAT rate settings 1 : 21.00 ; 2 : 10.00 ; 3 : 4.00 ; 4 : 0.00
                // Finds an appropriate department value (left side of each combination, integer values 1, 2 …), if value not found, then apply the 1st value.
                decimal taxRatePercentSalesLine = Math.Round(salesLine.TaxRatePercent, 2);
                string department = DocumentAttributeConstants.DefaultDepartment;
                Dictionary<int, string> vatRates = ConfigurationController.ParseSupportedVATRates(request.FiscalIntegrationFunctionalityProfile);

                if (vatRates.TryGetValue(SalesOrderHelper.ConvertSalesTaxRateToInt(taxRatePercentSalesLine), out department) == false)
                {
                    throw new ArgumentException("The transaction " + adjustedSalesOrder.ReceiptId
                        + " couldn't be registered on a fiscal device or service due to the missing VAT data mapping (vat rate percentage is " + taxRatePercentSalesLine + "). Please contact your system administrator.");
                }

                return department;
            }

            /// <summary>
            /// Builds the footer section for the fiscal receipt document.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <param name="adjustedSalesOrder">The adjusted sales order.</param>
            /// <param name="rootElement">The document root element.</param>
            /// <returns>The updated document root element.</returns>
            private static async Task<XElement> BuildFooterAsync(GetFiscalDocumentDocumentProviderRequest request, SalesOrder adjustedSalesOrder, XElement rootElement)
            {
                ThrowIf.Null(request, nameof(request));
                ThrowIf.Null(adjustedSalesOrder, nameof(adjustedSalesOrder));

                // Fiscal customer lottery code number.
                rootElement = await DocumentBuilder.BuildFiscalCustomerLotteryCodeAsync(request.RequestContext, rootElement, adjustedSalesOrder).ConfigureAwait(false);

                string barcodeType = ConfigurationController.ParseReceiptNumberBarcodeType(request.FiscalIntegrationFunctionalityProfile);

                if (string.IsNullOrEmpty(barcodeType))
                {
                    // Receipt number
                    rootElement = DocumentElementBuilder.BuildPrintRecMessageElement(rootElement, adjustedSalesOrder.ReceiptId, DocumentAttributeConstants.DefaultMessageType);
                }
                else
                {
                    // Barcode
                    rootElement = DocumentElementBuilder.BuildPrintStandardBarCodeElement(rootElement, adjustedSalesOrder.ReceiptId, barcodeType);
                }

                return rootElement;
            }

            /// <summary>
            /// Gets refund message by original transaction.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <param name="adjustedSalesOrder">The adjusted sales order.</param>
            /// <param name="printerReturnOriginWithoutFiscalData">The printer return origin without fiscal data uses as serial number when fiscal data not exists.</param>
            /// <returns>The updated document root element.</returns>
            private static async Task<string> GetRefundMessageByReturnOriginalTransaction(GetFiscalDocumentDocumentProviderRequest request, SalesOrder adjustedSalesOrder, string printerReturnOriginWithoutFiscalData)
            {
                var refundMessage = string.Empty;

                // Gets original sales order.
                SalesOrder originalSalesOrder = await SalesOrderHelper.GetOriginalSalesOrderForReturnAsync(request.RequestContext, adjustedSalesOrder).ConfigureAwait(false);

                // Gets fiscal transaction.
                FiscalTransaction fiscalTransaction = null;
                if (originalSalesOrder != null)
                {
                    fiscalTransaction = originalSalesOrder.FiscalTransactions.SingleOrDefault(f => f.ConnectorGroup == request.FiscalIntegrationFunctionalityProfileGroupId && f.ConnectorName == request.FiscalIntegrationTechnicalProfile.ConnectorName);

                    var fiscalReceiptDate = originalSalesOrder.BusinessDate != null
                        ? originalSalesOrder.BusinessDate.Value.ToString(FiscalPrinterResponseConstants.DateFormatForFiscalDocument)
                        : originalSalesOrder.TransactionDateTime.ToString(FiscalPrinterResponseConstants.DateFormatForFiscalDocument);

                    if (fiscalTransaction != null && fiscalTransaction.RegistrationStatus == FiscalIntegrationRegistrationStatus.Completed && !string.IsNullOrWhiteSpace(fiscalTransaction.RegisterResponse))
                    {
                        fiscalReceiptDate = fiscalTransaction.TransDateTime.ToString(FiscalPrinterResponseConstants.DateFormatForFiscalDocument);

                        XElement registerResponseXElement = XElement.Parse(fiscalTransaction.RegisterResponse);

                        if (registerResponseXElement.Attributes(FiscalPrinterResponseConstants.Success).FirstOrDefault().Value.Equals("true"))
                        {
                            var zReportNumber = Convert.ToInt32(FiscalPrinterResponseParser.ParseRegisterResponseInfo(registerResponseXElement, FiscalPrinterResponseConstants.ZRepNumberElement));
                            var fiscalReceiptNumber = Convert.ToInt32(FiscalPrinterResponseParser.ParseRegisterResponseInfo(registerResponseXElement, FiscalPrinterResponseConstants.FiscalReceiptNumberElement));
                            fiscalReceiptDate = FiscalPrinterResponseParser.ParseRegisterResponseInfo(registerResponseXElement, FiscalPrinterResponseConstants.FiscalReceiptDateElement);
                            var serialNumber = string.IsNullOrWhiteSpace(fiscalTransaction.RegisterInfo) ? FiscalPrinterResponseConstants.DefaultFiscalSerialNumber : fiscalTransaction.RegisterInfo;

                            refundMessage = GetRefundMessage(fiscalReceiptDate, serialNumber, zReportNumber, fiscalReceiptNumber);
                        }
                        else
                        {
                            refundMessage = GetRefundMessage(fiscalReceiptDate);
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(printerReturnOriginWithoutFiscalData))
                        {
                            throw new ArgumentException($"PrinterReturnOriginWithoutFiscalData is not specified in the 'Return origin mapping' field of the connector functional profile {request.FiscalIntegrationFunctionalityProfile.ProfileId}.");
                        }
                        var serialNumber = printerReturnOriginWithoutFiscalData;

                        refundMessage = GetRefundMessage(fiscalReceiptDate, serialNumber);
                    }
                }
                else
                {
                    string fiscalReceiptDateFromCurrentTransaction = adjustedSalesOrder.BusinessDate != null
                        ? adjustedSalesOrder.BusinessDate.Value.ToString(FiscalPrinterResponseConstants.DateFormatForFiscalDocument)
                        : adjustedSalesOrder.TransactionDateTime.ToString(FiscalPrinterResponseConstants.DateFormatForFiscalDocument);

                    refundMessage = GetRefundMessage(fiscalReceiptDateFromCurrentTransaction);
                }

                return refundMessage;
            }
            
            /// <summary>
            /// Gets refund message by without original transaction.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <param name="adjustedSalesOrder">The adjusted sales order.</param>
            /// <param name="returnOriginMapping">The return origin mappings.</param>
            /// <returns>The updated document root element.</returns>
            private static async Task<string> GetRefundMessageWithoutReturnOriginalTransaction(GetFiscalDocumentDocumentProviderRequest request, SalesOrder adjustedSalesOrder, ReturnOriginMapping returnOriginMapping)
            {
                var returnOrigins = returnOriginMapping.ReturnOrigins.ToDictionary(item => item.ReturnOrigin, item => item.PrinterReturnOrigin);
                var returnOriginInfoCode = ConfigurationController.GetReturnOriginInfoCodeValue(request.FiscalIntegrationFunctionalityProfile);
                var originalSalesDateInfoCode = ConfigurationController.GetOriginalSalesDateInfoCodeValue(request.FiscalIntegrationFunctionalityProfile);

                var reasonCodesRequest = new GetTransactionReasonCodesDataRequest(request.RequestContext.GetChannel().RecordId, adjustedSalesOrder.TerminalId, adjustedSalesOrder.Id);
                var reasonCodesResponse = await request.RequestContext.ExecuteAsync<EntityDataServiceResponse<ReasonCodeLine>>(reasonCodesRequest).ConfigureAwait(false);

                var reasonCodeLineByReturnOrigin = reasonCodesResponse.PagedEntityCollection.FirstOrDefault(rec => rec.ReasonCodeId == returnOriginInfoCode);
                var reasonCodeLineByReturnDate = reasonCodesResponse.PagedEntityCollection.FirstOrDefault(rec => rec.ReasonCodeId == originalSalesDateInfoCode);

                if (reasonCodeLineByReturnOrigin == null)
                {
                    throw new DataValidationException($"Cannot print a fiscal receipt for the return transaction - ChannelId:{request.RequestContext.GetChannel().RecordId} TerminalId:{adjustedSalesOrder.TerminalId} TransactionId:{adjustedSalesOrder.Id}. A {returnOriginInfoCode} info code transaction that is linked to the store transaction is not found.");
                }

                if (reasonCodeLineByReturnDate == null)
                {
                    throw new DataValidationException($"Cannot print a fiscal receipt for the return transaction - ChannelId:{request.RequestContext.GetChannel().RecordId} TerminalId:{adjustedSalesOrder.TerminalId} TransactionId:{adjustedSalesOrder.Id}. A {originalSalesDateInfoCode} info code transaction that is linked to the store transaction is not found.");
                }

                if (ReasonCodeDateParser.TryParse(reasonCodeLineByReturnDate.Information, out var fiscalReceiptDate) == false)
                {
                    throw new FormatException($"Cannot print a fiscal receipt for the return transaction - ChannelId:{request.RequestContext.GetChannel().RecordId} TerminalId:{adjustedSalesOrder.TerminalId} TransactionId:{adjustedSalesOrder.Id}. A {originalSalesDateInfoCode} info code transaction that is linked to the store transaction does not contain a valid date.");
                }

                if (returnOrigins.TryGetValue(reasonCodeLineByReturnOrigin.SubReasonCodeId, out var serialNumber) == false)
                {
                    throw new DataValidationException(
                        $"Cannot print a fiscal receipt for the return transaction - ChannelId:{request.RequestContext.GetChannel().RecordId} TerminalId:{adjustedSalesOrder.TerminalId} TransactionId:{adjustedSalesOrder.Id}. "+
                        $"The value {reasonCodeLineByReturnOrigin.SubReasonCodeId} of the {returnOriginInfoCode} info code transaction that is linked to the store transaction does not correspond to a valid fiscal printer return origin in the 'Return origin mapping' field " +
                        $"of the connector functional profile {request.FiscalIntegrationFunctionalityProfile.ProfileId}.");
                }
                var refundMessage = GetRefundMessage(fiscalReceiptDate.ToString(FiscalPrinterResponseConstants.DateFormatForFiscalDocument), serialNumber);
                return refundMessage;
            }

            /// <summary>
            /// Gets refund message.
            /// </summary>
            /// <param name="receiptDate">The receipt date</param>
            /// <param name="serialNumber">The serial number.</param>
            /// <param name="zReportNumber">The report number</param>
            /// <param name="fiscalReceiptNumber">The fiscal receipt number</param>
            /// <returns>The refund message.</returns>
            private static string GetRefundMessage(string receiptDate,
                string serialNumber = FiscalPrinterResponseConstants.DefaultFiscalSerialNumber,
                int zReportNumber = FiscalPrinterResponseConstants.DefaultZRepNumber,
                int fiscalReceiptNumber = FiscalPrinterResponseConstants.DefaultFiscalReceiptNumber)
            {
                return string.Format("REFUND {0} {1} {2} {3}",
                    zReportNumber.ToString(FiscalPrinterResponseConstants.NumberFormat),
                    fiscalReceiptNumber.ToString(FiscalPrinterResponseConstants.NumberFormat), receiptDate,
                    serialNumber);
            }

            /// <summary>
            /// Fills in the store VAT number, if VAT number isn't empty, builds printRecMessageElement. 
            /// </summary>
            /// <param name="context">The request context.</param>
            /// <param name="parentElement">The parent element.</param>
            /// <returns>The updated xml element.</returns>
            private static async Task<XElement> FillStoreVATNumberAsync(RequestContext context, XElement parentElement)
            {
                GetDeviceConfigurationDataRequest getDeviceConfigurationDataRequest = new GetDeviceConfigurationDataRequest();
                SingleEntityDataServiceResponse<DeviceConfiguration> deviceConfiguration = await context.ExecuteAsync<SingleEntityDataServiceResponse<DeviceConfiguration>>(getDeviceConfigurationDataRequest).ConfigureAwait(false);
                string vatNumber = deviceConfiguration.Entity.TaxIdNumber;

                if (!string.IsNullOrWhiteSpace(vatNumber))
                {
                    parentElement = DocumentElementBuilder.BuildPrintRecMessageElement(parentElement, vatNumber + DocumentAttributeConstants.Space + DocumentAttributeConstants.PIVA, DocumentAttributeConstants.ReceiptHeaderMessageType, GetNextHeaderLineIndex(parentElement).ToString());
                }

                return parentElement;
            }

            /// <summary>
            /// Builds the fiscal customer lottery code element.
            /// </summary>
            /// <param name="context">The request context.</param>
            /// <param name="parentElement">The parent element.</param>
            /// <param name="salesOrder">The sales order.</param>
            /// <returns>The element containing the fiscal customer lottery code element.</returns>
            private static async Task<XElement> BuildFiscalCustomerLotteryCodeAsync(RequestContext context, XElement parentElement, SalesOrder salesOrder)
            {
                if (!salesOrder.SalesLines.All(x => x.Quantity >= 0))
                {
                    return parentElement;
                }

                if (context.Runtime.GetAsyncRequestHandlers<IRequestHandlerAsync>(typeof(GetFiscalCustomerDataDataRequest)).IsNullOrEmpty())
                {
                    return parentElement;
                }

                var request = new GetFiscalCustomerDataDataRequest(salesOrder.Id, salesOrder.TerminalId);

                var fiscalCustomerDataResponse = await context.ExecuteAsync<SingleEntityDataServiceResponse<FiscalCustomer>>(request).ConfigureAwait(false);
                var fiscalCustomer = fiscalCustomerDataResponse.Entity;

                if (fiscalCustomer == null)
                {
                    return parentElement;
                }

                return DocumentElementBuilder.BuildFiscalCustomerLotteryCodeElement(parentElement, fiscalCustomer.LotteryCode);
            }
        }
    }
}
