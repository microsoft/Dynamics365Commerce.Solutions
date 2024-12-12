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
        /// The response constains the report X task document.
        /// </summary>
        public class GetReportXTaskDocumentProviderAtolResponse : Response
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="GetReportXTaskDocumentProviderAtolResponse"/> class.
            /// </summary>
            /// <param name="document">Generated document.</param>
            public GetReportXTaskDocumentProviderAtolResponse(FiscalIntegrationDocument document)
            {
                ThrowIf.Null(document, nameof(document));
                this.FiscalIntegrationDocument = document;
            }

            /// <summary>
            /// Gets fiscal integration document.
            /// </summary>
            public FiscalIntegrationDocument FiscalIntegrationDocument { get; private set; }
        }
    }
}
