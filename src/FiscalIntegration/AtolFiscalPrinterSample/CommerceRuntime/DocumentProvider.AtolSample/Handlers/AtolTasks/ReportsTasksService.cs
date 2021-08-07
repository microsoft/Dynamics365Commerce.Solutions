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
        using Contoso.CommerceRuntime.DocumentProvider.AtolSample.DataModel.AtolTask;
        using Contoso.CommerceRuntime.DocumentProvider.AtolSample.Messages;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.Framework.Serialization;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;

        /// <summary>
        /// Represents an entry point for requests related to printer report tasks.
        /// </summary>
        public class ReportsTasksService : IRequestHandlerAsync
        {
            /// <summary>
            /// Gets supported requests.
            /// </summary>
            public IEnumerable<Type> SupportedRequestTypes => new[]
            {
                typeof(GetReportXTaskDocumentProviderAtolRequest),
            };

            /// <summary>
            /// Represents an enty point for the request handler.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <returns>The response.</returns>
            public async Task<Response> Execute(Request request)
            {
                ThrowIf.Null(request, nameof(request));

                Response response;

                if (request is GetReportXTaskDocumentProviderAtolRequest getReportXTaskDocumentProviderAtolRequest)
                {
                    response = await this.GetReportXTask(getReportXTaskDocumentProviderAtolRequest).ConfigureAwait(false);
                }
                else
                {
                    throw new NotSupportedException(string.Format("Request '{0}' is not supported.", request.GetType()));
                }

                return response;
            }

            /// <summary>
            /// Gets task to generate report x.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <returns>Th response.</returns>
            private async Task<GetReportXTaskDocumentProviderAtolResponse> GetReportXTask(GetReportXTaskDocumentProviderAtolRequest request)
            {
                SerializeToAtolCommandRequest serializeToAtolCommandRequest = new SerializeToAtolCommandRequest(new AtolPrintXReportTask());
                var serializeToAtolCommandResponse = await request.RequestContext.ExecuteAsync<SerializeToAtolCommandResponse>(serializeToAtolCommandRequest).ConfigureAwait(false);
                string taskJson = serializeToAtolCommandResponse.SerializedCommand;
                var fiscalIntegrationDocument = new FiscalIntegrationDocument(taskJson, FiscalIntegrationDocumentGenerationResultType.Succeeded);
                return new GetReportXTaskDocumentProviderAtolResponse(fiscalIntegrationDocument);
            }
        }
    }
}
