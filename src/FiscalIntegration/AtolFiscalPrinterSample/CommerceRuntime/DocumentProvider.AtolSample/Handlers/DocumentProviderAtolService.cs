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
        using Contoso.CommerceRuntime.DocumentProvider.AtolSample.Messages;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.FiscalIntegration.DocumentProvider.Messages;
        using Microsoft.Dynamics.Commerce.Runtime.Handlers;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;

        /// <summary>
        /// Document provider service for Atol fiscal printer.
        /// </summary>
        public class DocumentProviderAtolService : INamedRequestHandlerAsync
        {
            /// <summary>
            /// Gets handler name.
            /// </summary>
            public string HandlerName => "AtolSample";

            /// <summary>
            /// Gets the list of the support fiscal events.
            /// </summary>
            public IEnumerable<int> SupportedRegistrableEventsId
            {
                get
                {
                    return new[]
                    {
                        (int)FiscalIntegrationEventType.Sale,
                        (int)FiscalIntegrationEventType.FiscalXReport,
                        (int)FiscalIntegrationEventType.FiscalZReport,
                    };
                }
            }

            /// <summary>
            /// Gets supported requests.
            /// </summary>
            IEnumerable<Type> IRequestHandlerAsync.SupportedRequestTypes => new[]
            {
                typeof(GetFiscalDocumentDocumentProviderRequest),
                typeof(GetSupportedRegistrableEventsDocumentProviderRequest),
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

                if (request is GetFiscalDocumentDocumentProviderRequest getFiscalDocumentDocumentProviderRequest)
                {
                    response = await this.GetFiscalDocumentAsync(getFiscalDocumentDocumentProviderRequest).ConfigureAwait(false);
                }
                else if (request is GetSupportedRegistrableEventsDocumentProviderRequest getSupportedRegistrableEventsDocumentProviderRequest)
                {
                    response = this.GetSupportedRegisterableEvents(getSupportedRegistrableEventsDocumentProviderRequest);
                }
                else
                {
                    throw new NotSupportedException(string.Format("Request '{0}' is not supported.", request.GetType()));
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
                switch (eventType)
                {
                    case FiscalIntegrationEventType.Sale:
                        var getSalesOrderDocumentReceiptAtolResponse = await request.RequestContext.ExecuteAsync<GetSalesOrderDocumentReceiptAtolResponse>(new GetSalesOrderDocumentReceiptAtolRequest(request.FiscalIntegrationFunctionalityProfile, request.SalesOrder)).ConfigureAwait(false);
                        return new GetFiscalDocumentDocumentProviderResponse(getSalesOrderDocumentReceiptAtolResponse.FiscalIntegrationDocument);
                    case FiscalIntegrationEventType.FiscalXReport:
                        var configuredTaskAwaitable = (await request.RequestContext.ExecuteAsync<GetReportXTaskDocumentProviderAtolResponse>(new GetReportXTaskDocumentProviderAtolRequest()).ConfigureAwait(false)).FiscalIntegrationDocument;
                        return new GetFiscalDocumentDocumentProviderResponse(configuredTaskAwaitable);
                    case FiscalIntegrationEventType.FiscalZReport:
                        GetCloseShiftTaskDocumentProviderAtolResponse getCloseShiftTaskDocumentProviderAtolResponse = await request.RequestContext.ExecuteAsync<GetCloseShiftTaskDocumentProviderAtolResponse>(new GetCloseShiftTaskDocumentProviderAtolRequest(request.FiscalIntegrationFunctionalityProfile)).ConfigureAwait(false);
                        return new GetFiscalDocumentDocumentProviderResponse(getCloseShiftTaskDocumentProviderAtolResponse.FiscalIntegrationDocument);
                }

                return null;
            }

            /// <summary>
            /// Gets the supported registerable events.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <returns>The supported registerable events.</returns>
            private GetSupportedRegistrableEventsDocumentProviderResponse GetSupportedRegisterableEvents(GetSupportedRegistrableEventsDocumentProviderRequest request)
            {
                ICollection<int> supportedFiscalEventTypes = new List<int>(this.SupportedRegistrableEventsId);
                GetSupportedRegistrableEventsDocumentProviderResponse response = new GetSupportedRegistrableEventsDocumentProviderResponse(
                        supportedFiscalEventTypes,
                        new List<int>());

                return response;
            }
        }
    }
}
