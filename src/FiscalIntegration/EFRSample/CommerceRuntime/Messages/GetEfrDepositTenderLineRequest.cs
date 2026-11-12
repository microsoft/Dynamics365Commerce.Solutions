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
    namespace CommerceRuntime.DocumentProvider.EFRSample.Messages
    {
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;

        /// <summary>
        /// The request to get the EFR deposit tender line.
        /// </summary>
        public sealed class GetEfrDepositTenderLineRequest : Request
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="GetEfrDepositTenderLineRequest"/> class.
            /// </summary>
            /// <param name="salesOrder">The sales order.</param>
            /// <param name="fiscalIntegrationFunctionalityProfile">The fiscal integration functionality profile.</param>
            public GetEfrDepositTenderLineRequest(SalesOrder salesOrder, FiscalIntegrationFunctionalityProfile fiscalIntegrationFunctionalityProfile)
            {
                this.SalesOrder = salesOrder;
                this.FiscalIntegrationFunctionalityProfile = fiscalIntegrationFunctionalityProfile;
            }

            /// <summary>
            /// Gets the sales order.
            /// </summary>
            public SalesOrder SalesOrder { get; private set; }

            /// <summary>
            /// Gets the fiscal integration functionality profile.
            /// </summary>
            public FiscalIntegrationFunctionalityProfile FiscalIntegrationFunctionalityProfile { get; private set; }
        }
    }
}
