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
    namespace CommerceRuntime.DocumentProvider.EpsonFP90IIISample
    {
        using System;
        using System.Collections.Generic;
        using System.Threading.Tasks;
        using Contoso.CommerceRuntime.DocumentProvider.EpsonFP90IIISample.DocumentBuilders;
        using Contoso.CommerceRuntime.DocumentProvider.EpsonFP90IIISample.Helpers;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.FiscalIntegration.DocumentProvider.Messages;
        using Microsoft.Dynamics.Commerce.Runtime.Handlers;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;
        using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;

        /// <summary>
        /// Sample for EPSON FP90III
        /// </summary>
        public class DocumentProviderEpsonFP90III : INamedRequestHandlerAsync
        {
            /// <summary>
            /// Gets the unique name for this request handler.
            /// </summary>
            public string HandlerName
            {
                get
                {
                    return "EpsonFP90IIISample";
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
                        typeof(GetSupportedRegistrableEventsDocumentProviderRequest),
                    };
                }
            }

            /// <summary>
            /// Executes the specified request using the specified request context and handler.
            /// </summary>
            /// <param name="request">The request to execute.</param>
            /// <returns>The response of the request from the request handler.</returns>
            public async Task<Response> Execute(Request request)
            {
                ThrowIf.Null(request, nameof(request));

                DocumentLocalizerHelper.CultureName = request.RequestContext.LanguageId;

                Type requestType = request.GetType();
                Response response;
               

                if (requestType == typeof(GetFiscalDocumentDocumentProviderRequest))
                {
                    response = await this.GetFiscalDocumentAsync((GetFiscalDocumentDocumentProviderRequest)request).ConfigureAwait(false);
                }
                else if (requestType == typeof(GetSupportedRegistrableEventsDocumentProviderRequest))
                {
                    response = this.GetSupportedRegisterableEvents((GetSupportedRegistrableEventsDocumentProviderRequest)request);
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
                FiscalIntegrationDocumentGenerationResultType generationResultType = FiscalIntegrationDocumentGenerationResultType.None;
                GetAdjustedSalesOrderFiscalIntegrationServiceResponse adjustedSalesOrderResponse = null;
                string document = string.Empty;

                switch (eventType)
                {
                    case FiscalIntegrationEventType.Sale:
                        adjustedSalesOrderResponse = await SalesOrderHelper.GetAdjustedSalesOrderAsync(request).ConfigureAwait(false);
                        request.SalesOrder = adjustedSalesOrderResponse.SalesOrder;
                        if (DocumentValidator.IsDocumentGenerationRequired(request.SalesOrder, eventType))
                        {
                            document = await DocumentBuilder.BuildAsync(request, adjustedSalesOrderResponse.SalesOrder).ConfigureAwait(false);
                            generationResultType = DocumentValidator.ValidateDocumentStructure(document, eventType);
                        }
                        else
                        {
                            generationResultType = FiscalIntegrationDocumentGenerationResultType.NotRequired;
                        }
                        break;

                    case FiscalIntegrationEventType.FiscalXReport:
                    case FiscalIntegrationEventType.FiscalZReport:
                        document = XZReportDocumentBuilder.Build(eventType);
                        generationResultType = DocumentValidator.ValidateDocumentStructure(document, eventType);
                        break;

                    default:
                        throw new NotSupportedException(string.Format("Fiscal integration event type '{0}' is not supported.", eventType));
                }

                // We are saving adjustment in the response because it will be saved in the DB after registration on a fiscal printer.
                // The goal of saving an adjustment is to capture the difference between original and adjusted sales.
                var fiscalIntegrationDocument = new FiscalIntegrationDocument(document, generationResultType);
                fiscalIntegrationDocument.DocumentAdjustment = adjustedSalesOrderResponse?.DocumentAdjustment;

                return new GetFiscalDocumentDocumentProviderResponse(fiscalIntegrationDocument);
            }

            /// <summary>
            /// Gets the supported registerable events document provider response.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <returns>The supported registerable events document provider response.</returns>
            private GetSupportedRegistrableEventsDocumentProviderResponse GetSupportedRegisterableEvents(GetSupportedRegistrableEventsDocumentProviderRequest request)
            {
                ICollection<int> supportedFiscalEventTypes = new List<int>();

                foreach (int eventType in this.SupportedRegistrableEventsId)
                {
                    supportedFiscalEventTypes.Add(eventType);
                }

                GetSupportedRegistrableEventsDocumentProviderResponse response = new GetSupportedRegistrableEventsDocumentProviderResponse(supportedFiscalEventTypes, new List<int>());
                return response;
            }
        }
    }
}
