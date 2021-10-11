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
    namespace CommerceRuntime.DocumentProvider.CleanCashSample
    {
        using DocumentBuilders;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.FiscalIntegration.DocumentProvider.Messages;
        using Microsoft.Dynamics.Commerce.Runtime.Handlers;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;
        using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;
        using System;
        using System.Collections.Generic;
        using System.Linq;
        using System.Threading.Tasks;

        /// <summary>
        /// Sample for Clean Cash
        /// </summary>
        public class DocumentProviderCleanCashAsync : INamedRequestHandlerAsync
        {
            /// <summary>
            /// Gets the unique name for this request handler.
            /// </summary>
            public string HandlerName => "CleanCashSample";

            /// <summary>
            /// Gets the supported fiscal integration event type.
            /// </summary>
            public IEnumerable<int> SupportedRegistrableEventsId => new[]
            {
                (int)FiscalIntegrationEventType.Sale,
                (int)FiscalIntegrationEventType.AuditEvent,
            };

            /// <summary>
            /// Gets the collection of supported request types by this handler.
            /// </summary>
            public IEnumerable<Type> SupportedRequestTypes => new[]
            {
                typeof(GetFiscalDocumentDocumentProviderRequest),
                typeof(GetSupportedRegistrableEventsDocumentProviderRequest),
            };

            /// <summary>
            /// Executes the specified request using the specified request context and handler.
            /// </summary>
            /// <param name="request">The request to execute.</param>
            /// <returns>The response of the request from the request handler.</returns>
            public async Task<Response> Execute(Request request)
            {
                ThrowIf.Null(request, nameof(request));

                Type requestType = request.GetType();
                Response response;

                if (requestType == typeof(GetFiscalDocumentDocumentProviderRequest))
                {
                    response = await this.GetFiscalDocumentAsync((GetFiscalDocumentDocumentProviderRequest)request).ConfigureAwait(false);
                }
                else if (requestType == typeof(GetSupportedRegistrableEventsDocumentProviderRequest))
                {
                    response = this.GetSupportedRegistrableEvents();
                }
                else
                {
                    throw new NotSupportedException($"Request '{request.GetType()}' is not supported.");
                }

                return response;
            }

            /// <summary>
            /// Gets the fiscal document document provider response.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <returns>The fiscal document document provider response.</returns>
            private async Task<GetFiscalDocumentDocumentProviderResponse> GetFiscalDocumentAsync(GetFiscalDocumentDocumentProviderRequest request)
            {
                FiscalIntegrationEventType eventType = request.FiscalDocumentRetrievalCriteria.FiscalRegistrationEventType;
                FiscalIntegrationDocumentGenerationResultType generationResultType = FiscalIntegrationDocumentGenerationResultType.None;

                string document = string.Empty;
                switch (eventType)
                {
                    case FiscalIntegrationEventType.Sale:
                    case FiscalIntegrationEventType.AuditEvent:
                        await FillSalesOrderAsync(request, eventType).ConfigureAwait(false);

                        if (DocumentValidator.IsDocumentGenerationRequired(request, eventType))
                        {
                            document = await new DocumentBuilder(request).BuildAsync().ConfigureAwait(false);
                            generationResultType = FiscalIntegrationDocumentGenerationResultType.Succeeded;
                        }
                        else
                        {
                            generationResultType = FiscalIntegrationDocumentGenerationResultType.NotRequired;
                        }
                        break;

                    default:
                        throw new NotSupportedException($"Fiscal integration event type '{eventType}' is not supported.");
                }
                var fiscalIntegrationDocument = new FiscalIntegrationDocument(document, generationResultType);
                return new GetFiscalDocumentDocumentProviderResponse(fiscalIntegrationDocument);
            }

            /// <summary>
            /// Gets the supported registrable events document provider response.
            /// </summary>
            /// <returns>The supported registrable events document provider response.</returns>
            private GetSupportedRegistrableEventsDocumentProviderResponse GetSupportedRegistrableEvents()
            {
                return new GetSupportedRegistrableEventsDocumentProviderResponse(this.SupportedRegistrableEventsId.ToList(), new List<int>());
            }

            /// <summary>
            /// Retrieve information about SalesOrder for AuditEvents.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <param name="eventType">The fiscal integration event type.</param>
            private static async Task FillSalesOrderAsync(GetFiscalDocumentDocumentProviderRequest request, FiscalIntegrationEventType eventType)
            {
                if (eventType == FiscalIntegrationEventType.AuditEvent)
                {
                    string refTransactionId = request.FiscalDocumentRetrievalCriteria.DocumentContext.AuditEvent?.RefTransactionId;
                    if (!string.IsNullOrEmpty(refTransactionId))
                    {
                        request.SalesOrder = await GetSalesOrderByTransactionIdAsync(request.RequestContext, refTransactionId, request.FiscalDocumentRetrievalCriteria.IsRemoteTransaction).ConfigureAwait(false);
                    }
                }
            }

            /// <summary>
            /// Returns Sales Order by transaction id and terminal id. Used to get local orders.
            /// </summary>
            /// <param name="requestContext">The context of the request.</param>
            /// <param name="transactionId">The transaction id parameter.</param>
            /// <param name="isRemoteTransaction">Client sends if this is local or remote transaction.</param>
            /// <returns>The SalesOrder.</returns>
            private static async Task<SalesOrder> GetSalesOrderByTransactionIdAsync(RequestContext requestContext, string transactionId, bool isRemoteTransaction)
            {
                // Based on order type we decide the Search location to be efficient
                SearchLocation searchLocationType = isRemoteTransaction ? SearchLocation.All : SearchLocation.Local;

                // Get the order. If order sales id is provided then it should be remote search mode.
                var request = new GetSalesOrderDetailsByTransactionIdServiceRequest(transactionId, searchLocationType);
                var response = await requestContext.ExecuteAsync<GetSalesOrderDetailsServiceResponse>(request).ConfigureAwait(false);

                return response.SalesOrder;
            }
        }
    }
}
