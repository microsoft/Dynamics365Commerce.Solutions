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
        /// Response for request to generate fiscal document.
        /// </summary>
        public class GetSalesOrderDocumentReceiptAtolResponse : Response
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="GetSalesOrderDocumentReceiptAtolResponse"/> class.
            /// </summary>
            /// <param name="fiscalIntegrationDocument">The fiscal integration document.</param>
            public GetSalesOrderDocumentReceiptAtolResponse(FiscalIntegrationDocument fiscalIntegrationDocument)
            {
                ThrowIf.Null(fiscalIntegrationDocument, nameof(fiscalIntegrationDocument));
                this.FiscalIntegrationDocument = fiscalIntegrationDocument;
            }

            /// <summary>
            /// Gets fiscal integration document.
            /// </summary>
            public FiscalIntegrationDocument FiscalIntegrationDocument { get; }
        }
    }
}
