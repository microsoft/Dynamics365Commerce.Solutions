namespace Contoso
{
    namespace Commerce.Runtime.DocumentProvider.PosnetSample
    {
        using System;
        using System.Collections.Generic;
        using System.Linq;
        using System.Threading.Tasks;
        using Contoso.Commerce.Runtime.DocumentProvider.PosnetSample.DocumentBuilder;
        using Contoso.Commerce.Runtime.DocumentProvider.PosnetSample.DocumentModel;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.FiscalIntegration.DocumentProvider.Messages;
        using Microsoft.Dynamics.Commerce.Runtime.Handlers;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;

        /// <summary>
        /// Sample document provider for fiscal printers supported POSNET protocol.
        /// </summary>
        public class DocumentProviderPosnetProtocol : INamedRequestHandlerAsync
        {
            /// <summary>
            /// Gets the unique name for this request handler.
            /// </summary>
            public string HandlerName
            {
                get
                {
                    return "POSNETFiscalPrinterSample";
                }
            }

            /// <summary>
            /// Gets the supported fiscal integration event type.
            /// </summary>
            public IEnumerable<int> SupportedRegistrableEventsId
            {
                get
                {
                    return new[]
                    {
                        (int)FiscalIntegrationEventType.Sale,
                        (int)FiscalIntegrationEventType.FiscalXReport,
                        (int)FiscalIntegrationEventType.FiscalZReport
                    };
                }
            }

            /// <summary>
            /// Gets the collection of supported request types by this handler.
            /// </summary>
            public IEnumerable<Type> SupportedRequestTypes
            {
                get
                {
                    return new[]
                    {
                        typeof(GetFiscalDocumentDocumentProviderRequest),
                        typeof(GetSupportedRegistrableEventsDocumentProviderRequest)
                    };
                }
            }

            /// <summary>
            /// Executes the specified request using the request.
            /// </summary>
            /// <param name="request">The request to execute.</param>
            /// <returns>The response of the request from the handler.</returns>
            public async Task<Response> Execute(Request request)
            {
                ThrowIf.Null(request, nameof(request));

                Type requestType = request.GetType();

                if (requestType == typeof(GetFiscalDocumentDocumentProviderRequest))
                {
                    return await this.GetFiscalDocumentAsync((GetFiscalDocumentDocumentProviderRequest)request).ConfigureAwait(false);
                }
                else if (requestType == typeof(GetSupportedRegistrableEventsDocumentProviderRequest))
                {
                    return await Task.FromResult(this.GetSupportedRegisterableEvents((GetSupportedRegistrableEventsDocumentProviderRequest)request));
                }
                else
                {
                    throw new NotSupportedException(string.Format("Request '{0}' is not supported.", request.GetType()));
                }
            }

            /// <summary>
            /// Gets the documet builder according to fiscal event type.
            /// </summary>
            /// <param name="eventType">Fiscal inegration event type.</param>
            /// <returns>Concrete builder.</returns>
            private IFiscalDocumentRequestBuilder GetPosnetDocumentRequestBuilder(GetFiscalDocumentDocumentProviderRequest request)
            {
                FiscalIntegrationEventType eventType = request.FiscalDocumentRetrievalCriteria.FiscalRegistrationEventType;

                IFiscalDocumentRequestBuilder builder;
                switch (eventType)
                {
                    case FiscalIntegrationEventType.Sale:
                        builder = new SalesReceiptBuilder(request);
                        break;

                    case FiscalIntegrationEventType.FiscalXReport:
                    case FiscalIntegrationEventType.FiscalZReport:
                        builder = new XZReportBuilder(request);
                        break;

                    default:
                        throw new NotSupportedException(string.Format("Fiscal integration event type '{0}' is not supported.", eventType));
                }

                return builder;
            }

            /// <summary>
            /// Gets the fiscal document document provider response.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <returns>The fiscal document document provider response.</returns>
            private async Task<Response> GetFiscalDocumentAsync(GetFiscalDocumentDocumentProviderRequest request)
            {
                ThrowIf.Null(request, nameof(request));

                string jsonDocument = string.Empty;
                var builtResult = await this.GetPosnetDocumentRequestBuilder(request).BuildAsync().ConfigureAwait(false);

                if (builtResult.DocumentGenerationResult == FiscalIntegrationDocumentGenerationResultType.Succeeded)
                {
                    jsonDocument = builtResult.Document.ToJson();
                }

                FiscalIntegrationDocument fiscalIntegrationDocument = new FiscalIntegrationDocument(jsonDocument, builtResult.DocumentGenerationResult)
                {
                    DocumentAdjustment = builtResult.DocumentAdjustment
                };

                GetFiscalDocumentDocumentProviderResponse response = new GetFiscalDocumentDocumentProviderResponse(fiscalIntegrationDocument);
                return response;
            }

            /// <summary>
            /// Gets the supported registerable events document provider response.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <returns>The supported registerable events document provider response.</returns>
            private Response GetSupportedRegisterableEvents(GetSupportedRegistrableEventsDocumentProviderRequest request)
            {
                ICollection<int> supportedFiscalEventTypes = this.SupportedRegistrableEventsId.ToList();
                GetSupportedRegistrableEventsDocumentProviderResponse response = new GetSupportedRegistrableEventsDocumentProviderResponse(supportedFiscalEventTypes, new List<int>());
                return response;
            }
        }
    }
}
