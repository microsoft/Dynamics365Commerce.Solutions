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
        /// The request to get the EFR non-fiscal transaction type.
        /// </summary>
        public sealed class GetEfrNonFiscalTransactionTypeRequest : Request
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="GetEfrNonFiscalTransactionTypeRequest"/> class.
            /// </summary>
            /// <param name="fiscalRegistrationEventType">The fiscal registration event type.</param>
            /// <param name="salesOrder">The sales order.</param>
            public GetEfrNonFiscalTransactionTypeRequest(FiscalIntegrationEventType fiscalRegistrationEventType, SalesOrder salesOrder)
            {
                this.FiscalRegistrationEventType = fiscalRegistrationEventType;
                this.SalesOrder = salesOrder;
            }

            /// <summary>
            /// Gets the fiscal registration event type.
            /// </summary>
            public FiscalIntegrationEventType FiscalRegistrationEventType { get; private set; }

            /// <summary>
            /// Gets the sales order.
            /// </summary>
            public SalesOrder SalesOrder { get; private set; }
        }
    }
}
