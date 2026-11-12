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
        /// The request to populate EFR customer data.
        /// </summary>
        public sealed class PopulateEfrCustomerDataRequest : Request
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="PopulateEfrCustomerDataRequest"/> class.
            /// </summary>
            /// <param name="receipt">The receipt.</param>
            /// <param name="salesOrder">The sales order.</param>
            /// <param name="fiscalIntegrationFunctionalityProfile">The fiscal integration functionality profile.</param>
            public PopulateEfrCustomerDataRequest(DataModelEFR.Documents.Receipt receipt, SalesOrder salesOrder, FiscalIntegrationFunctionalityProfile fiscalIntegrationFunctionalityProfile)
            {
                this.Receipt = receipt;
                this.SalesOrder = salesOrder;
                this.FiscalIntegrationFunctionalityProfile = fiscalIntegrationFunctionalityProfile;
            }

            /// <summary>
            /// Gets the receipt.
            /// </summary>
            public DataModelEFR.Documents.Receipt Receipt { get; private set; }

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
