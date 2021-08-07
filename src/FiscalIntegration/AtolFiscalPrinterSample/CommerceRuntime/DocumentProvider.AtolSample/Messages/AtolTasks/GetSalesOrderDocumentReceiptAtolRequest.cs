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
    namespace CommerceRuntime.DocumentProvider.AtolSample.Messages
    {
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;

        /// <summary>
        /// The request to generate the fiscal document for registration in the fiscal service.
        /// </summary>
        public class GetSalesOrderDocumentReceiptAtolRequest : Request
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="GetSalesOrderDocumentReceiptAtolRequest"/> class.
            /// </summary>
            /// <param name="fiscalIntegrationFunctionalityProfile">The fiscal integration functionality profile.</param>
            /// <param name="salesOrder">The sales order.</param>
            public GetSalesOrderDocumentReceiptAtolRequest(FiscalIntegrationFunctionalityProfile fiscalIntegrationFunctionalityProfile, SalesOrder salesOrder)
            {
                ThrowIf.Null(fiscalIntegrationFunctionalityProfile, nameof(fiscalIntegrationFunctionalityProfile));
                ThrowIf.Null(salesOrder, nameof(salesOrder));

                this.FiscalIntegrationFunctionalityProfile = fiscalIntegrationFunctionalityProfile;
                this.SalesOrder = salesOrder;
            }

            /// <summary>
            /// Gets fiscal integration functionality profile.
            /// </summary>
            public FiscalIntegrationFunctionalityProfile FiscalIntegrationFunctionalityProfile { get; }

            /// <summary>
            /// Gets the sales order.
            /// </summary>
            public SalesOrder SalesOrder { get; }
        }
    }
}
