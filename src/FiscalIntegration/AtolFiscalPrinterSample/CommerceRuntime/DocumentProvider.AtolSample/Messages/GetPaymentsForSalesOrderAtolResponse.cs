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
        using System.Collections.Generic;
        using Contoso.CommerceRuntime.DocumentProvider.AtolSample.DataModel.AtolTask;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;

        /// <summary>
        /// Response to providing payments for a sales order.
        /// </summary>
        public class GetPaymentsForSalesOrderAtolResponse : Response
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="GetPaymentsForSalesOrderAtolResponse"/> class.
            /// </summary>
            /// <param name="payments">List of payments.</param>
            public GetPaymentsForSalesOrderAtolResponse(IReadOnlyCollection<Payment> payments)
            {
                ThrowIf.Null(payments, nameof(payments));
                this.Payments = payments;
            }

            /// <summary>
            /// Gets list of payments.
            /// </summary>
            public IReadOnlyCollection<Payment> Payments { get; }
        }
    }
}
