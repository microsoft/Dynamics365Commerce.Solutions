/**
 * SAMPLE CODE NOTICE
 * 
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

namespace Contoso.CommerceRuntime.PricingEngine
{
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.Services.PricingEngine.DiscountData;

    /// <summary>
    /// The discount supports fraction quantity sales lines in mix and match discount.
    /// </summary>
    public class DecimalMixAndMatchDiscount : MixAndMatchDiscount
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DecimalMixAndMatchDiscount" /> class.
        /// </summary>
        /// <param name="validationPeriod">Validation period.</param>
        public DecimalMixAndMatchDiscount(ValidationPeriod validationPeriod) : base(validationPeriod)
        {
        }

        protected override void RemoveItemGroupIndexesWithFractionalQuantityFromLookups(decimal[] remainingQuantities)
        {
            // Override this method to not remove item groups with fractional quantity.
        }
    }
}
