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
    namespace CommerceRuntime.DocumentProvider.CleanCashSample.DocumentBuilders
    {
        using System.Linq;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.FiscalIntegration.DocumentProvider.Messages;

        /// <summary>
        /// Validates the generated fiscal integration document.
        /// </summary>
        public static class DocumentValidator
        {
            /// <summary>
            /// Gets whether document generation is required for a sales order.
            /// </summary>
            /// <param name="request">The document provider request.</param>
            /// <param name="eventType">The fiscal integration event type.</param>
            /// <returns>True if document generation is required, otherwise false.</returns>
            /// <remarks>The sales order should be adjusted.</remarks>
            public static bool IsDocumentGenerationRequired(GetFiscalDocumentDocumentProviderRequest request, FiscalIntegrationEventType eventType)
            {
                return request.SalesOrder.ActiveSalesLines.Any() && (eventType != FiscalIntegrationEventType.AuditEvent
                    || request.FiscalDocumentRetrievalCriteria.DocumentContext.AuditEvent != null);
            }
        }
    }
}
