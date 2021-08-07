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
        using System.Linq;
        using System.Threading.Tasks;
        using Contoso.CommerceRuntime.DocumentProvider.AtolSample.DataModel.AtolTask;
        using Contoso.CommerceRuntime.DocumentProvider.AtolSample.DocumentBuilders;
        using Contoso.CommerceRuntime.DocumentProvider.AtolSample.Messages;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;

        /// <summary>
        /// Represents an entry point for requests related to a sales payment order.
        /// </summary>
        public class PaymentService : IRequestHandlerAsync
        {
            /// <summary>
            /// Gets supported types.
            /// </summary>
            public IEnumerable<Type> SupportedRequestTypes => new[]
            {
                typeof(GetPaymentsForSalesOrderAtolRequest),
            };

            /// <summary>
            /// Represents an enty point for the request handler.
            /// </summary>
            /// <param name="request">Th request.</param>
            /// <returns>The response.</returns>
            public async Task<Response> Execute(Request request)
            {
                ThrowIf.Null(request, nameof(request));

                Response response;

                if (request is GetPaymentsForSalesOrderAtolRequest getPaymentsForSalesOrderAtolRequest)
                {
                    response = await this.GetPaymentsAsync(getPaymentsForSalesOrderAtolRequest).ConfigureAwait(false);
                }
                else
                {
                    throw new NotSupportedException(string.Format("Request '{0}' is not supported.", request.GetType()));
                }

                return response;
            }

            /// <summary>
            /// Gets payments for sales order.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <returns>The list of payments for a sales order.</returns>
            private async Task<GetPaymentsForSalesOrderAtolResponse> GetPaymentsAsync(GetPaymentsForSalesOrderAtolRequest request)
            {
                DeserializeDocumentProviderSettingsDocumentProviderAtolRequest deserializeDocumentProviderSettingsDocumentProviderAtolRequest = new DeserializeDocumentProviderSettingsDocumentProviderAtolRequest(request.FiscalIntegrationFunctionalityProfile);
                var deserializeDocumentProviderSettingsDocumentProviderAtolResponse = await request.RequestContext.ExecuteAsync<DeserializeDocumentProviderSettingsDocumentProviderAtolResponse>(deserializeDocumentProviderSettingsDocumentProviderAtolRequest).ConfigureAwait(false);

                IPaymentResolver paymentResolver = new PaymentResolver(deserializeDocumentProviderSettingsDocumentProviderAtolResponse.DocumentProviderSettings);
                IEnumerable<TenderLine> activeTenderLines = request.SalesOrder.ActiveTenderLines.Where(t => !t.IsChangeLine);

                List<Payment> payments = new List<Payment>();
                foreach (var tenderLine in activeTenderLines)
                {
                    var payment = await paymentResolver.GetPaymentAsync(request.RequestContext, tenderLine).ConfigureAwait(false);
                    payments.Add(payment);
                }

                return new GetPaymentsForSalesOrderAtolResponse(payments);
            }
        }
    }
}