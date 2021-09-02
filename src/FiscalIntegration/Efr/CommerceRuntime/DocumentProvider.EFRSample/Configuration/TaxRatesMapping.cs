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
    namespace CommerceRuntime.DocumentProvider.EFRSample
    {
        using System.Collections.Generic;
        using System.Linq;

        /// <summary>
        /// Incapsulates tax rate to tax group mapping.
        /// </summary>
        internal class TaxRatesMapping
        {
            /// <summary>
            /// Mapping tax rate to tax group.
            /// </summary>
            private Dictionary<decimal, string> mapping;

            /// <summary>
            /// Initializes a new instance of the <see cref="TaxRatesMapping"/> class.
            /// </summary>
            /// <param name="mapping">The tax rates mapping.</param>
            public TaxRatesMapping(Dictionary<decimal, string> mapping)
            {
                this.mapping = mapping;
            }

            /// <summary>
            /// Gets tax group by tax rate.
            /// </summary>
            /// <param name="taxRate">The tax rate.</param>
            /// <returns>The tax group.</returns>
            public string this[decimal taxRate]
            {
                get
                {
                    string taxGroup = string.Empty;

                    if (mapping != null && mapping.TryGetValue(taxRate, out taxGroup))
                    {
                        return taxGroup;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }

            /// <summary>
            /// Gets the single tax rate related to the tax group.
            /// </summary>
            /// <param name="taxGroup">The tax group string.</param>
            /// <returns>The tax rate value.</returns>
            public decimal GetSignleRateByName(string taxGroup)
            {
                return this.mapping.Single(c => c.Value == taxGroup).Key;
            }

            /// <summary>
            /// Checks if the tax group exists in tax mappings.
            /// </summary>
            /// <param name="taxGroup"></param>
            /// <returns>True if mapping contains the tax group; False otherwise.</returns>
            public bool ContainsTax(string taxGroup)
            {
                return this.mapping.ContainsValue(taxGroup);
            }
        }
    }
}
