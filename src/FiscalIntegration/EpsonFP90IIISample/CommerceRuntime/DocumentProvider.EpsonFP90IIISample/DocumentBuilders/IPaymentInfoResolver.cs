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
        using System.Collections.Generic;
        using System.Threading.Tasks;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;

        /// <summary>
        /// Interface for resolver for creating PaymentInfo element for the store payment method.
        /// </summary>
        public interface IPaymentInfoResolver
        {
            /// <summary>
            /// Gets the payment information for fiscal printer by the store tenderTypeId.
            /// </summary>
            /// <param name="context">The current request context.</param>
            /// <param name="functionalityProfile">The functionality profile.</param>
            /// <param name="tenderTypeId">The tender type ID in HQ.</param>
            /// <returns>Information about payment.</returns>
            Task<PaymentInfo> GetPaymentInfoAsync(RequestContext context, FiscalIntegrationFunctionalityProfile functionalityProfile, string tenderTypeId);

            /// <summary>
            /// Gets the payment information for fiscal printer by deposit payment method.
            /// </summary>
            /// <param name="depositPaymentMethod">Deposit payment method description.</param>
            /// <returns>Information about payment.</returns>
            PaymentInfo GetDepositPaymentMethod(string depositPaymentMethod);
        }
    }
}
