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
    namespace CommerceRuntime.DocumentProvider.EFRSample.DocumentBuilders.GermanyBuilders
    {
        using System.Collections.Generic;
        using System.Globalization;
        using System.Linq;
        using System.Threading.Tasks;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
        using Contoso.CommerceRuntime.DocumentProvider.DataModelEFR.Documents;
        using Contoso.CommerceRuntime.DocumentProvider.EFRSample.DocumentBuilders.Parameters;
        using Contoso.CommerceRuntime.DocumentProvider.EFRSample.Extensions;
        using Contoso.CommerceRuntime.DocumentProvider.EFRSample.Messages;
        using Receipt = DataModelEFR.Documents.Receipt;

        /// <summary>
        /// Encapsulates the document generation logic for the shift cash over/short (difference) event for Germany.
        /// EFR requires a signed cash-variance document (NFS="DIFF") whenever a shift closes with a difference
        /// between the declared and the expected tender amounts. The document breaks the variance down per tender,
        /// reporting one position line and one payment for every tender that is out of balance.
        /// </summary>
        public class ShiftDifferenceBuilder : IDocumentBuilder
        {
            /// <summary>
            /// The non-fiscal signed transaction type for a cash difference document.
            /// </summary>
            private const string TransactionTypeCode = "DIFF";

            /// <summary>
            /// The request.
            /// </summary>
            private readonly DocumentBuilderData documentBuilderData;

            /// <summary>
            /// The shift the cash difference is reported for.
            /// </summary>
            private Shift shift;

            /// <summary>
            /// Initializes a new instance of the <see cref="ShiftDifferenceBuilder"/> class.
            /// </summary>
            /// <param name="documentBuilderData">The request.</param>
            private ShiftDifferenceBuilder(DocumentBuilderData documentBuilderData)
            {
                ThrowIf.Null(documentBuilderData, nameof(documentBuilderData));

                this.documentBuilderData = documentBuilderData;
            }

            /// <summary>
            /// Creates a new instance of the <see cref="ShiftDifferenceBuilder"/> class and loads the shift data.
            /// </summary>
            /// <param name="documentBuilderData">The request.</param>
            /// <returns>The document builder.</returns>
            public static async Task<ShiftDifferenceBuilder> Create(DocumentBuilderData documentBuilderData)
            {
                var instance = new ShiftDifferenceBuilder(documentBuilderData);
                var criteria = documentBuilderData.FiscalDocumentRetrievalCriteria;

                ValidateShiftIdentity(criteria);

                instance.shift = await instance.GetShiftAsync(criteria.ShiftTerminalId, criteria.ShiftId.Value).ConfigureAwait(false);

                return instance;
            }

            /// <summary>
            /// Validates that the retrieval criteria identify the shift the cash difference has to be reported for.
            /// A cash variance document that is silently skipped is a compliance gap, so an event without a shift
            /// identity fails loudly instead of being reported as not required.
            /// </summary>
            /// <param name="criteria">The fiscal document retrieval criteria.</param>
            private static void ValidateShiftIdentity(FiscalIntegrationDocumentRetrievalCriteria criteria)
            {
                if (!criteria.ShiftId.HasValue || string.IsNullOrWhiteSpace(criteria.ShiftTerminalId))
                {
                    throw new DataValidationException(
                        DataValidationErrors.Microsoft_Dynamics_Commerce_Runtime_RequiredValueNotFound,
                        "The cash difference event does not identify the shift, the cash difference document cannot be generated.");
                }
            }

            /// <summary>
            /// Builds fiscal integration document.
            /// </summary>
            /// <returns> The fiscal integration receipt document, or null when no cash difference has to be reported.</returns>
            public async Task<IFiscalIntegrationDocument> BuildAsync()
            {
                var varianceTenderLines = this.GetVarianceTenderLines();

                if (!varianceTenderLines.Any())
                {
                    // The shift is balanced (no cash over/short), so no DIFF document is required.
                    return null;
                }

                var receipt = await this.CreateReceiptAsync(varianceTenderLines).ConfigureAwait(false);
                return new SalesTransactionRegistrationRequest
                {
                    Receipt = receipt
                };
            }

            /// <summary>
            /// Gets the shift the cash difference is reported for.
            /// </summary>
            /// <param name="shiftTerminalId">The shift terminal identifier.</param>
            /// <param name="shiftId">The shift identifier.</param>
            /// <returns>The shift.</returns>
            private async Task<Shift> GetShiftAsync(string shiftTerminalId, long shiftId)
            {
                var getShiftDataRequest = new GetShiftDataRequest(shiftTerminalId, shiftId);
                var shift = (await this.documentBuilderData.RequestContext.ExecuteAsync<SingleEntityDataServiceResponse<Shift>>(getShiftDataRequest).ConfigureAwait(false)).Entity;

                if (shift == null)
                {
                    throw new DataValidationException(
                        DataValidationErrors.Microsoft_Dynamics_Commerce_Runtime_ShiftNotFound,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "The shift '{0}' of the terminal '{1}' is not found, the cash difference document cannot be generated.",
                            shiftId,
                            shiftTerminalId));
                }

                return shift;
            }

            /// <summary>
            /// Gets the shift tender lines that have a non-zero cash over/short amount.
            /// </summary>
            /// <returns>The tender lines with a cash difference.</returns>
            private IList<ShiftTenderLine> GetVarianceTenderLines()
            {
                if (this.shift.TenderLines == null)
                {
                    return new List<ShiftTenderLine>();
                }

                return this.shift.TenderLines
                    .Where(tenderLine => tenderLine.OverShortAmountOfStoreCurrency != decimal.Zero)
                    .ToList();
            }

            /// <summary>
            /// Creates a receipt.
            /// </summary>
            /// <param name="varianceTenderLines">The tender lines with a cash difference.</param>
            /// <returns>The receipt.</returns>
            private async Task<Receipt> CreateReceiptAsync(IList<ShiftTenderLine> varianceTenderLines)
            {
                var resolvedTenderLines = await Task.WhenAll(
                    varianceTenderLines.Select(async tenderLine => new ResolvedTenderLine
                    {
                        TenderLine = tenderLine,
                        TenderTypeName = await this.GetEfrTenderTypeName(tenderLine.TenderTypeId).ConfigureAwait(false),
                    })).ConfigureAwait(false);

                var orgUnit = this.documentBuilderData.RequestContext.GetOrgUnit();

                return new Receipt
                {
                    TransactionLocation = orgUnit.OrgUnitNumber,
                    TransactionTerminal = this.documentBuilderData.FiscalDocumentRetrievalCriteria.ShiftTerminalId,
                    TransactionNumber = this.documentBuilderData.FiscalDocumentRetrievalCriteria.TransactionId,
                    TotalAmount = varianceTenderLines.Sum(tenderLine => tenderLine.OverShortAmountOfStoreCurrency),
                    NonFiscalSignedTransactionType = TransactionTypeCode,
                    PositionLines = CreateReceiptPositionLines(resolvedTenderLines),
                    Payments = this.CreateReceiptPayments(resolvedTenderLines, orgUnit.Currency),
                };
            }

            /// <summary>
            /// Creates position lines, one per tender with a cash difference.
            /// </summary>
            /// <param name="resolvedTenderLines">The resolved tender lines.</param>
            /// <returns>The receipt position lines.</returns>
            private static ReceiptPositionLines CreateReceiptPositionLines(IEnumerable<ResolvedTenderLine> resolvedTenderLines)
            {
                var positions = resolvedTenderLines
                    .Select((resolvedTenderLine, index) => new ReceiptPosition
                    {
                        PositionNumber = index + 1,
                        Description = resolvedTenderLine.TenderTypeName,
                        Amount = resolvedTenderLine.TenderLine.OverShortAmountOfStoreCurrency,
                    })
                    .ToList();

                return new ReceiptPositionLines
                {
                    ReceiptPositions = positions,
                };
            }

            /// <summary>
            /// Creates payments, one per tender with a cash difference.
            /// </summary>
            /// <param name="resolvedTenderLines">The resolved tender lines.</param>
            /// <param name="storeCurrencyCode">The store (channel) currency code used to detect foreign-currency tenders.</param>
            /// <returns>The payments.</returns>
            private List<ReceiptPayment> CreateReceiptPayments(IEnumerable<ResolvedTenderLine> resolvedTenderLines, string storeCurrencyCode)
            {
                return resolvedTenderLines
                    .Select(resolvedTenderLine =>
                    {
                        var tenderLine = resolvedTenderLine.TenderLine;
                        var payment = new ReceiptPayment
                        {
                            Description = resolvedTenderLine.TenderTypeName,
                            PaymentTypeGroup = this.documentBuilderData.FiscalIntegrationFunctionalityProfile.GetPaymentTypeGroup(tenderLine.TenderTypeId),
                            Amount = tenderLine.OverShortAmountOfStoreCurrency,
                        };

                        if (!string.IsNullOrWhiteSpace(tenderLine.TenderCurrency) && tenderLine.TenderCurrency != storeCurrencyCode)
                        {
                            payment.ForeignAmount = tenderLine.OverShortAmountOfTenderCurrency;
                            payment.ForeignCurrencyCode = tenderLine.TenderCurrency;
                        }

                        return payment;
                    })
                    .ToList();
            }

            /// <summary>
            /// Gets the tender type name as configured for EFR.
            /// </summary>
            /// <param name="tenderTypeId">The tender type identifier.</param>
            /// <returns>The tender type name.</returns>
            private async Task<string> GetEfrTenderTypeName(string tenderTypeId)
            {
                var request = new GetEfrGetTenderTypeNameRequest(tenderTypeId);
                return (await this.documentBuilderData.RequestContext.ExecuteAsync<SingleEntityDataServiceResponse<string>>(request).ConfigureAwait(false)).Entity;
            }

            /// <summary>
            /// Associates a shift tender line with its resolved EFR tender type name.
            /// </summary>
            private sealed class ResolvedTenderLine
            {
                /// <summary>
                /// Gets or sets the shift tender line.
                /// </summary>
                public ShiftTenderLine TenderLine { get; set; }

                /// <summary>
                /// Gets or sets the resolved EFR tender type name.
                /// </summary>
                public string TenderTypeName { get; set; }
            }
        }
    }
}
