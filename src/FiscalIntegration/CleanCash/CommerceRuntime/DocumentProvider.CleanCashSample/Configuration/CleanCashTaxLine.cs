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
    namespace CommerceRuntime.DocumentProvider.CleanCashSample
    {
        /// <summary>
        /// Clean cash fiscal register request tax line.
        /// </summary>
        /// <remarks>Contains fiscal transaction tax data converted into clean cash device specific format.</remarks>
        internal class CleanCashTaxLine
        {
            private const decimal ZeroAmount = 0.0M;

            /// <summary>
            /// Initializes a new instance of the <see cref="CleanCashTaxLine" /> class with default properties values.
            /// </summary>
            public CleanCashTaxLine()
            {
                this.TaxAmount = ZeroAmount;
                this.TaxPercentage = ZeroAmount;
            }

            /// <summary>
            /// Gets or sets the tax amount.
            /// </summary>
            public decimal TaxAmount { get; set; }

            /// <summary>
            /// Gets or sets the tax percentage.
            /// </summary>
            public decimal TaxPercentage { get; set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="CleanCashTaxLine" /> class.
            /// </summary>
            /// <param name="taxAmount">Tax amount.</param>
            /// <param name="taxPercentage">Tax percentage.</param>
            public CleanCashTaxLine(decimal taxAmount, decimal taxPercentage)
            {
                this.TaxAmount = taxAmount;
                this.TaxPercentage = taxPercentage;
            }
        }
    }
}
