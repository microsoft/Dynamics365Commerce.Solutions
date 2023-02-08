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
    namespace CommerceRuntime.DocumentProvider.CleanCashSample.DocumentBuilders
    {
        using System;
        using System.Collections.Generic;
        using System.Globalization;
        using System.Threading.Tasks;
        using Helpers;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.FiscalIntegration.DocumentProvider.Messages;

        /// <summary>
        /// Generates fiscal receipt document.
        /// </summary>
        public class DocumentBuilder
        {
            private const decimal ZeroAmount = 0.0M;
            private const string DecimalNumberFormat = "0.00";
            private const string DateTimeFormat = "yyyyMMddHHmm";
            private const string VatStringFormat = "{0};{1}";

            // Swedish culture should be used since Clean cash fiscal register device is Swedish specific.
            private readonly CultureInfo swedishCulture = new CultureInfo("sv-SE");

            /// <summary>
            /// The request.
            /// </summary>
            private readonly GetFiscalDocumentDocumentProviderRequest request;

            /// <summary>
            /// The sales order.
            /// </summary>
            private readonly SalesOrder salesOrder;

            /// <summary>
            /// Initializes the fields of the <see cref="DocumentBuilder" /> class.
            /// </summary>
            /// <param name="request">Retail transaction to register.</param>
            public DocumentBuilder(GetFiscalDocumentDocumentProviderRequest request)
            {
                ThrowIf.Null(request, nameof(request));
                ThrowIf.Null(request.SalesOrder, nameof(request.SalesOrder));

                this.request = request;
                this.salesOrder = request.SalesOrder;
            }

            /// <summary>
            /// Builds fiscal receipt document.
            /// </summary>
            /// <returns>The generated fiscal document string.</returns>
            public async Task<string> BuildAsync()
            {
                CleanCashFiscalTransactionData fiscalTransaction = new CleanCashFiscalTransactionData()
                {
                    StoreTaxRegNumber = request.RequestContext.GetDeviceConfiguration().TaxIdNumber ?? string.Empty,
                    TerminalId = salesOrder.TerminalId,
                    IsCopy = request.FiscalDocumentRetrievalCriteria.FiscalRegistrationEventType != FiscalIntegrationEventType.Sale,
                    ReceiptId = salesOrder.ReceiptId,
                    TransactionDate = salesOrder.CreatedDateTime.UtcDateTime.ToString(DateTimeFormat),
                    TotalAmount = salesOrder.TotalAmount.ToString(DecimalNumberFormat, this.swedishCulture),
                    NegativeAmount = salesOrder.IsReturnTransaction() ? Math.Abs(salesOrder.TotalAmount).ToString(DecimalNumberFormat, this.swedishCulture) :
                    ZeroAmount.ToString(DecimalNumberFormat, this.swedishCulture),
                };

                InitVatRegisters(fiscalTransaction);
                var document = await SerializationHelper.SerializeFiscalTransactionDataToJsonAsync(fiscalTransaction).ConfigureAwait(false);
                return document;
            }

            /// <summary>
            /// Initializes values of VAT registers by fiscal transaction taxes.
            /// </summary>
            /// <param name="fiscalTransaction">The fiscal transaction data.</param>
            private void InitVatRegisters(CleanCashFiscalTransactionData fiscalTransaction)
            {
                var cleanCashTaxLines = SalesTransactionHelper.GetCleanCashMappedTaxesFromTransaction(salesOrder, request.FiscalIntegrationFunctionalityProfile);

                fiscalTransaction.VAT1 = this.GetVATStrValue(cleanCashTaxLines, 1);
                fiscalTransaction.VAT2 = this.GetVATStrValue(cleanCashTaxLines, 2);
                fiscalTransaction.VAT3 = this.GetVATStrValue(cleanCashTaxLines, 3);
                fiscalTransaction.VAT4 = this.GetVATStrValue(cleanCashTaxLines, 4);
            }

            /// <summary>
            /// Converts tax lines dictionary to a string.
            /// </summary>
            /// <param name="cleanCashTaxLines">The CleanCash tax lines dictionary.</param>
            /// <param name="taxLineId">The tax line id.</param>
            /// <returns>Converted string from tax lines dictionary</returns>
            private string GetVATStrValue(Dictionary<int, CleanCashTaxLine> cleanCashTaxLines, int taxLineId)
            {
                CleanCashTaxLine cleanCashTaxLine = cleanCashTaxLines.TryGetValue(taxLineId, out CleanCashTaxLine value) ? value : new CleanCashTaxLine();
                return string.Format(VatStringFormat, Math.Abs(cleanCashTaxLine.TaxPercentage).ToString(DecimalNumberFormat, this.swedishCulture), cleanCashTaxLine.TaxAmount.ToString(DecimalNumberFormat, this.swedishCulture));
            }
        }
    }
}