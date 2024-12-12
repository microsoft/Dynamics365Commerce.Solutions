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
    namespace CommerceRuntime.DocumentProvider.AtolSample.Handlers
    {
        using System;
        using System.Collections.Generic;
        using System.Threading.Tasks;
        using Contoso.CommerceRuntime.DocumentProvider.AtolSample.DocumentBuilders;
        using Contoso.CommerceRuntime.DocumentProvider.AtolSample.Messages;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;

        /// <summary>
        /// Represents an entry point for requests related to printer sales tasks.
        /// </summary>
        public class SalesTasksService : IRequestHandlerAsync
        {
            /// <summary>
            /// Gets supported requests.
            /// </summary>
            public IEnumerable<Type> SupportedRequestTypes => new[]
            {
                typeof(GetSalesOrderDocumentReceiptAtolRequest),
            };

            /// <summary>
            /// Represents an enty point for the request handler.
            /// </summary>
            /// <param name="request">Th request.</param>
            /// <returns>The response.</returns>
            public Task<Response> Execute(Request request)
            {
                ThrowIf.Null(request, nameof(request));

                switch (request)
                {
                    case GetSalesOrderDocumentReceiptAtolRequest getSalesOrderDocumentReceiptAtolRequest:
                        return GetFiscalDocumentAsync(getSalesOrderDocumentReceiptAtolRequest);
                    default:
                        throw new NotSupportedException($"Request '{request.GetType()}' is not supported.");
                }
            }

            /// <summary>
            /// Gets the seles order document receipt provider response.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <returns>The seles order document receipt provider response.</returns>
            private async Task<Response> GetFiscalDocumentAsync(GetSalesOrderDocumentReceiptAtolRequest request)
            {
                var document = await DocumentBuilder.Build(request.RequestContext, request.SalesOrder, request.FiscalIntegrationFunctionalityProfile).ConfigureAwait(false);
                var fiscalIntegrationDocument = new FiscalIntegrationDocument(document, FiscalIntegrationDocumentGenerationResultType.Succeeded);
                return new GetSalesOrderDocumentReceiptAtolResponse(fiscalIntegrationDocument);
            }
        }
    }
}
