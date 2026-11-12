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
        using System.Collections.Generic;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;
        using Contoso.CommerceRuntime.DocumentProvider.DataModelEFR.Documents;

        /// <summary>
        /// The request to get EFR customer account deposit receipt positions.
        /// </summary>
        public sealed class GetEfrCustomerAccountDepositPositionsRequest : Request
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="GetEfrCustomerAccountDepositPositionsRequest"/> class.
            /// </summary>
            /// <param name="receiptPositions">The receipt positions.</param>
            /// <param name="salesOrder">The sales order.</param>
            /// <param name="fiscalIntegrationFunctionalityProfile">The fiscal integration functionality profile.</param>
            public GetEfrCustomerAccountDepositPositionsRequest(List<ReceiptPosition> receiptPositions, SalesOrder salesOrder, FiscalIntegrationFunctionalityProfile fiscalIntegrationFunctionalityProfile)
            {
                this.ReceiptPositions = receiptPositions;
                this.SalesOrder = salesOrder;
                this.FiscalIntegrationFunctionalityProfile = fiscalIntegrationFunctionalityProfile;
            }

            /// <summary>
            /// Gets the receipt positions.
            /// </summary>
            public List<ReceiptPosition> ReceiptPositions { get; private set; }

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
