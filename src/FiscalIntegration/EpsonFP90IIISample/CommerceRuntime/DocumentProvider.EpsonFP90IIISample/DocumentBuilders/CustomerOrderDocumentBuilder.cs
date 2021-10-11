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
    namespace CommerceRuntime.DocumentProvider.EpsonFP90IIISample.DocumentBuilders
    {
        using System.Xml.Linq;
        using System.Linq;
        using Contoso.CommerceRuntime.DocumentProvider.EpsonFP90IIISample.Constants;
        using Contoso.CommerceRuntime.DocumentProvider.EpsonFP90IIISample.Helpers;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.FiscalIntegration.DocumentProvider.Messages;
        using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;
        using System.Threading.Tasks;

        /// <summary>
        /// Generates customer order document.
        /// </summary>
        public static class CustomerOrderDocumentBuilder
        {
            /// <summary>
            /// Builds header section for the fiscal receipt document when sales order is a customer order.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <param name="rootElement">The document root element.</param>
            /// <param name="adjustedSalesOrder">The adjusted sales order.</param>
            /// <returns>The updated document root element.</returns>
            public static async Task<XElement> BuildHeaderAsync(GetFiscalDocumentDocumentProviderRequest request, XElement rootElement, SalesOrder adjustedSalesOrder)
            {
                ThrowIf.Null(request, nameof(request));
                ThrowIf.Null(rootElement, nameof(rootElement));
                ThrowIf.Null(adjustedSalesOrder, nameof(adjustedSalesOrder));

                // Customer name
                var customer = await GetCustomerAsync(request, adjustedSalesOrder).ConfigureAwait(false);

                if (customer != null)
                {
                    foreach (string splitString in StringProcessor.SplitStringByWords(customer.Name, DocumentAttributeConstants.MaxHeaderAttributeLength).Take(DocumentAttributeConstants.ReceiptFieldCustomerNameLinesLimit))
                    {
                        rootElement = DocumentElementBuilder.BuildPrintRecMessageElement(rootElement, splitString, DocumentAttributeConstants.ReceiptHeaderMessageType, DocumentBuilder.GetNextHeaderLineIndex(rootElement).ToString());
                    }
                }

                // Sales ID
                rootElement = DocumentElementBuilder.BuildPrintRecMessageElement(rootElement, adjustedSalesOrder.SalesId, DocumentAttributeConstants.ReceiptHeaderMessageType, DocumentBuilder.GetNextHeaderLineIndex(rootElement).ToString());

                return rootElement;
            }

            /// <summary>
            /// Gets customer by customer ID.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <param name="adjustedSalesOrder">The adjusted sales order.</param>
            /// <returns>The customer.</returns>
            private static async Task<Customer> GetCustomerAsync(GetFiscalDocumentDocumentProviderRequest request, SalesOrder adjustedSalesOrder)
            {
                if (string.IsNullOrWhiteSpace(adjustedSalesOrder.CustomerId))
                {
                    return null;
                }

                var customersServiceRequest = new GetCustomersServiceRequest(QueryResultSettings.SingleRecord, adjustedSalesOrder.CustomerId);
                var response = await request.RequestContext.ExecuteAsync<GetCustomersServiceResponse>(customersServiceRequest).ConfigureAwait(false);

                return response.Customers.FirstOrDefault();
            }
        }
    }
}
