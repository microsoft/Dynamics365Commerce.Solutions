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
        using System.Threading.Tasks;
        using Microsoft.Dynamics.Commerce.Runtime;

        /// <summary>
        /// Interface declares method to get department number.
        /// </summary>
        public interface IDepartmentResolver
        {
            /// <summary>
            /// Gets the department number of the fiscal printer.
            /// </summary>
            /// <param name="context">The current request context.</param>
            /// <param name="taxRatePercent">The tax rate percent.</param>
            /// <param name="receiptId">The receipt id.</param>
            /// <param name="productId">The product id.</param>
            /// <param name="isGiftCardLine">Indicates whether this instance is a gift card line.</param>
            /// <returns>Department number.</returns>
            Task<string> GetDepartmentNumberAsync(RequestContext context, decimal taxRatePercent, string receiptId, long productId, bool isGiftCardLine);
        }
    }
}
