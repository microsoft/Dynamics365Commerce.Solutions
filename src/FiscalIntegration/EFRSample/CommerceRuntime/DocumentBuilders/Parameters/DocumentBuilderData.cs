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

        public sealed class DocumentBuilderData
        {
            /// <summary>
            /// The request context.
            /// </summary>
            public RequestContext RequestContext { get; set; }

            /// <summary>
            /// The sales order.
            /// </summary>
            public SalesOrder SalesOrder { get; set; }

            public CountryRegionISOCode CountryRegionISOCode { get; set; }

            public FiscalIntegrationDocumentRetrievalCriteria FiscalDocumentRetrievalCriteria { get; set; }

            public FiscalIntegrationDocumentRetrievalCriteria NonFiscalDocumentRetrievalCriteria { get; set; }

            public FiscalIntegrationFunctionalityProfile FiscalIntegrationFunctionalityProfile { get; set; }

            public DocumentBuilderData()
            {
            }

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