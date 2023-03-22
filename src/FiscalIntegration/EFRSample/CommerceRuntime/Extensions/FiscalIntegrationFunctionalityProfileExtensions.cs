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
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;

        internal static class FiscalIntegrationFunctionalityProfileExtensions
        {
            /// <summary>
            /// Tax rates mapping.
            /// </summary>
            private static IDictionary<string, TaxRatesMapping> taxRatesMappings;

            /// <summary>
            /// Gets tax rates mapping.
            /// </summary>
            /// <param name="functionalityProfile">The functionality profile.</param>
            /// <returns>The tax rates mapping.</returns>
            public static TaxRatesMapping GetTaxRatesMapping(this FiscalIntegrationFunctionalityProfile functionalityProfile)
            {

                if (taxRatesMappings == null)
                {
                    taxRatesMappings = new Dictionary<string, TaxRatesMapping>();
                }

                if (!taxRatesMappings.ContainsKey(functionalityProfile.ProfileId))
                {
                    taxRatesMappings.Add(
                        functionalityProfile.ProfileId,
                        ConfigurationController.GetSupportedTaxRates(functionalityProfile));
                }

                return taxRatesMappings[functionalityProfile.ProfileId];
            }

            /// <summary>
            /// Gets payment type group.
            /// </summary>
            /// <param name="functionalityProfile">The functionality profile.</param>
            /// <param name="tenderTypeId">The tender type identifier.</param>
            /// <returns>The payment group.</returns>
            public static string GetPaymentTypeGroup(this FiscalIntegrationFunctionalityProfile functionalityProfile, string tenderTypeId)
            {
                return ConfigurationController.GetTenderTypeMapping(functionalityProfile)[tenderTypeId];
            }

            /// <summary>
            /// Gets tax group by tax percentage.
            /// </summary>
            /// <param name="functionalityProfile">The functionality profile.</param>
            /// <param name="percentage">The tax rate.</param>
            /// <returns>The tax groups.</returns>
            public static string GetTaxGroupByRate(this FiscalIntegrationFunctionalityProfile functionalityProfile, decimal percentage)
            {
                return functionalityProfile.GetTaxRatesMapping()[percentage];
            }
        }
    }
}