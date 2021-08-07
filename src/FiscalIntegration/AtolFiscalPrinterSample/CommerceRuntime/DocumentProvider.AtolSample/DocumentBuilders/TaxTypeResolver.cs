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
        using Contoso.CommerceRuntime.DocumentProvider.AtolSample.DataModel.AtolTask;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;

        /// <summary>
        /// Resolver for getting tax type.
        /// </summary>
        public static class TaxTypeResolver
        {
            /// <summary>
            /// Gets the tax type of the fiscal printer.
            /// </summary>
            /// <param name="salesLine">Sales line of sales order.</param>
            /// <returns>VAT tax of the fiscal printer.</returns>
            public static TaxType GetTaxType(SalesLine salesLine)
            {
                decimal taxRatePercentSalesLine = Math.Round(salesLine.TaxRatePercent, 2);
                return string.IsNullOrEmpty(salesLine.ItemTaxGroupId) ?
                    TaxType.None :
                    MapTaxRateToTaxType(taxRatePercentSalesLine);
            }

            /// <summary>
            /// Maps a tax rate to the corresponding tax type.
            /// </summary>
            /// <param name="taxRatePercentSalesLine">The tax rate.</param>
            /// <returns>VAT tax of the fiscal printer.</returns>
            private static TaxType MapTaxRateToTaxType(decimal taxRatePercentSalesLine)
            {
                TaxType ret;

                switch (taxRatePercentSalesLine)
                {
                    case 0.00M:
                        ret = TaxType.Vat0;
                        break;

                    case 10.00M:
                        ret = TaxType.Vat10;
                        break;

                    case 20.00M:
                        ret = TaxType.Vat20;
                        break;

                    default:
                        throw new DataValidationException(DataValidationErrors.Microsoft_Dynamics_Commerce_Runtime_InvalidItemTaxGroup, $"There was an error generating a fiscal receipt. Unable to process the tax rate {taxRatePercentSalesLine}%. Please contact your system administrator.");
                }

                return ret;
            }
        }
    }
}
