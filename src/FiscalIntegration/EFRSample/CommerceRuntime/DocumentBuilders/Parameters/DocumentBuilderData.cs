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
    namespace CommerceRuntime.DocumentProvider.EFRSample.DocumentBuilders.Parameters
    {
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.FiscalIntegration.DocumentProvider.Messages;

        /// <summary>
        /// Contains data for document builders.
        /// </summary>
        public sealed class DocumentBuilderData
        {
            /// <summary>
            /// Gets or sets the request context.
            /// </summary>
            public RequestContext RequestContext { get; set; }

            /// <summary>
            /// Gets or sets the sales order.
            /// </summary>
            public SalesOrder SalesOrder { get; set; }

            /// <summary>
            /// Gets or sets the country region ISO code.
            /// </summary>
            public CountryRegionISOCode CountryRegionISOCode { get; set; }

            /// <summary>
            /// Gets or sets the fiscal document retrieval criteria.
            /// </summary>
            public FiscalIntegrationDocumentRetrievalCriteria FiscalDocumentRetrievalCriteria { get; set; }

            /// <summary>
            /// Gets or sets the non-fiscal document retrieval criteria.
            /// </summary>
            public FiscalIntegrationDocumentRetrievalCriteria NonFiscalDocumentRetrievalCriteria { get; set; }

            /// <summary>
            /// Gets or sets the fiscal integration functionality profile.
            /// </summary>
            public FiscalIntegrationFunctionalityProfile FiscalIntegrationFunctionalityProfile { get; set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="DocumentBuilderData"/> class.
            /// </summary>
            public DocumentBuilderData()
            {
            }

            /// <summary>
            /// Creates document builder data from a fiscal document request.
            /// </summary>
            /// <param name="request">The fiscal document request.</param>
            /// <returns>The document builder data.</returns>
            public static DocumentBuilderData FromRequest(GetFiscalDocumentDocumentProviderRequest request)
            {
                return new DocumentBuilderData()
                {
                    RequestContext = request.RequestContext,
                    SalesOrder = request.SalesOrder,
                    FiscalDocumentRetrievalCriteria = request.FiscalDocumentRetrievalCriteria,
                    FiscalIntegrationFunctionalityProfile = request.FiscalIntegrationFunctionalityProfile,
                };
            }

            /// <summary>
            /// Creates document builder data from a non-fiscal document request.
            /// </summary>
            /// <param name="request">The non-fiscal document request.</param>
            /// <returns>The document builder data.</returns>
            public static DocumentBuilderData FromRequest(GetNonFiscalDocumentDocumentProviderRequest request)
            {
                return new DocumentBuilderData()
                {
                    RequestContext = request.RequestContext,
                    SalesOrder = request.SalesOrder,
                    NonFiscalDocumentRetrievalCriteria = request.NonFiscalDocumentRetrievalCriteria,
                    FiscalIntegrationFunctionalityProfile = request.FiscalIntegrationFunctionalityProfile,
                };
            }
        }
    }
}