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
        using System.Threading.Tasks;
        using Contoso.CommerceRuntime.DocumentProvider.EpsonFP90IIISample.Helpers;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;

        /// <summary>
        /// Legacy resolver for creating department element.
        /// </summary>
        internal class DepartmentResolverRT : IDepartmentResolver
        {
            /// <summary>
            /// Gets vat rates mapping.
            /// </summary>
            public Dictionary<int, string> VatRatesMapping { get; }

            /// <summary>
            /// Constructor for DepartmentResolverRT class.
            /// </summary>
            /// <param name="functionalityProfile">Fiscal integration functionality profile.</param>
            public DepartmentResolverRT(FiscalIntegrationFunctionalityProfile functionalityProfile)
            {
                ThrowIf.Null(functionalityProfile, nameof(functionalityProfile));
                VatRatesMapping = ConfigurationController.ParseSupportedVATRates(functionalityProfile);
            }

            /// <summary>
            /// Gets the department number for fiscal printer.
            /// </summary>
            /// <param name="context">The current request context.</param>
            /// <param name="taxRatePercent">The tax rate.</param>
            /// <param name="receiptId">The receipt id.</param>
            /// <param name="productId">The product id.</param>
            /// /// <param name="isGiftCardLine">Indicates whether this instance is a gift card line.</param>
            /// <returns>Department number.</returns>
            public Task<string> GetDepartmentNumberAsync(RequestContext context, decimal taxRatePercent,
                string receiptId, long productId, bool isGiftCardLine)
            {
                decimal taxRatePercentSalesLine = Math.Round(taxRatePercent, 2);

                if (VatRatesMapping.TryGetValue(SalesOrderHelper.ConvertSalesTaxRateToInt(taxRatePercentSalesLine), out var department) == false)
                {
                    throw new ArgumentException(
                        $"The transaction {receiptId} couldn't be registered on a fiscal device or service due to the missing VAT data mapping (VAT rate percentage is {taxRatePercentSalesLine}). Please contact your system administrator.");
                }

                return Task.FromResult<string>(department);
            }
        }
    }
}
