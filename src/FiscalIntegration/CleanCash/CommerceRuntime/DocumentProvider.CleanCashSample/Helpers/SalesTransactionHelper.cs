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
    namespace CommerceRuntime.DocumentProvider.CleanCashSample.Helpers
    {
        using System.Collections.Generic;
        using System.Linq;
        using CleanCashSample;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;

        /// <summary>
        /// Contains helper methods for sales transaction document generation.
        /// </summary>
        public static class SalesTransactionHelper
        {
            private const string TaxCodesWithUnequalTaxRatesMappedToSingleVATGroup = "Tax codes with unequal tax rates are mapped to the control unit VAT group {0}.";

            /// <summary>
            /// Tax codes mapping.
            /// </summary>
            private static IDictionary<string, TaxCodesMapping> TaxCodesMappings;

            /// <summary>
            /// Checks if the transaction is return.
            /// </summary>
            /// <param name="salesOrder">The sales order.</param>
            /// <returns>True if any transaction is return, otherwise false.</returns>
            public static bool IsReturnTransaction (this SalesOrder salesOrder)
            {
                return salesOrder.SalesLines.Any(salesLine => salesLine.Quantity < 0 && !salesLine.IsVoided);
            }

            /// <summary>
            /// Get the taxes from transaction mapped to the CleanCash VAT ids.
            /// </summary>
            /// <param name="salesOrder">The sales order.</param>
            /// <param name="functionalityProfile">The functionality profile.</param>
            /// <returns>Taxes from transaction mapped to the CleanCash VAT ids.</returns>
            internal static Dictionary<int, CleanCashTaxLine> GetCleanCashMappedTaxesFromTransaction(SalesOrder salesOrder, FiscalIntegrationFunctionalityProfile functionalityProfile)
            {
                var cleanCashTaxLines = new Dictionary<int, CleanCashTaxLine>();
                var taxMappings = GetTaxCodesMapping(functionalityProfile);

                foreach (var tax in salesOrder.TaxLines)
                {
                    int cleanCashVATRegisterId = taxMappings[tax.TaxCode];

                    if (cleanCashTaxLines.ContainsKey(cleanCashVATRegisterId))
                    {
                        var cleanCashTaxLine = cleanCashTaxLines[cleanCashVATRegisterId];

                        if (cleanCashTaxLine.TaxPercentage != tax.Percentage)
                        {
                            throw new DataValidationException(DataValidationErrors.Microsoft_Dynamics_Commerce_Runtime_RequiredValueNotFound, TaxCodesWithUnequalTaxRatesMappedToSingleVATGroup);
                        }

                        cleanCashTaxLine.TaxAmount += tax.Amount;
                    }
                    else
                    {
                        var cleanCashTaxLine = new CleanCashTaxLine(tax.Amount, tax.Percentage);
                        cleanCashTaxLines.Add(cleanCashVATRegisterId, cleanCashTaxLine);
                    }
                }
                return cleanCashTaxLines;
            }

            /// <summary>
            /// Gets tax codes mapping.
            /// </summary>
            /// <param name="functionalityProfile">The functionality profile.</param>
            /// <returns>The tax codes mapping.</returns>
            private static TaxCodesMapping GetTaxCodesMapping(FiscalIntegrationFunctionalityProfile functionalityProfile)
            {
                ThrowIf.Null(functionalityProfile, nameof(functionalityProfile));
                ThrowIf.NullOrWhiteSpace(functionalityProfile.ProfileId, nameof(functionalityProfile.ProfileId));

                if (TaxCodesMappings == null)
                {
                    TaxCodesMappings = new Dictionary<string, TaxCodesMapping>();
                }

                if (!TaxCodesMappings.ContainsKey(functionalityProfile.ProfileId))
                {
                    TaxCodesMappings.Add(
                        functionalityProfile.ProfileId,
                        ConfigurationController.GetSupportedTaxCodes(functionalityProfile));
                }

                return TaxCodesMappings[functionalityProfile.ProfileId];
            }
        }
    }
}