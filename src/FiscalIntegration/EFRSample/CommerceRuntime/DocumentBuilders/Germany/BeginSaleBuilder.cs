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
    namespace CommerceRuntime.DocumentProvider.EFRSample.DocumentBuilders.GermanyBuilders
    {
        using System.Linq;
        using System.Threading.Tasks;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;
        using Contoso.CommerceRuntime.DocumentProvider.DataModelEFR.Documents;
        using Contoso.CommerceRuntime.DocumentProvider.EFRSample.DocumentBuilders.Parameters;
        using Receipt = DataModelEFR.Documents.Receipt;

        /// <summary>
        /// Incapsulates the document generation logic for the begin sale event.
        /// </summary>
        internal class BeginSaleBuilder : IDocumentBuilder
        {
            /// <summary>
            /// The request.
            /// </summary>
            private readonly DocumentBuilderData documentBuilderData;

            /// <summary>
            /// The Cart.
            /// </summary>
            private Cart cart;

            /// <summary>
            /// Initializes a new instance of the <see cref="BeginSaleBuilder"/> class.
            /// </summary>
            /// <param name="documentBuilderData">The request.</param>
            private BeginSaleBuilder(DocumentBuilderData documentBuilderData)
            {
                ThrowIf.Null(documentBuilderData, nameof(documentBuilderData));

                this.documentBuilderData = documentBuilderData;
            }

            public static async Task<BeginSaleBuilder> Create(DocumentBuilderData documentBuilderData)
            {
                var instance = new BeginSaleBuilder(documentBuilderData);
                var cartSearchCriteria = new CartSearchCriteria(instance.documentBuilderData.FiscalDocumentRetrievalCriteria.TransactionId);
                var getCartServiceRequest = new GetCartServiceRequest(cartSearchCriteria, QueryResultSettings.SingleRecord);
                instance.cart = (await instance.documentBuilderData.RequestContext.ExecuteAsync<GetCartServiceResponse>(getCartServiceRequest).ConfigureAwait(false)).Carts.Single();

                return instance;
            }

            /// <summary>
            /// Builds fiscal integration document.
            /// </summary>
            /// <returns> The fiscal integration receipt document.</returns>
            public Task<IFiscalIntegrationDocument> BuildAsync()
            {
                var receipt = this.CreateReceipt(cart);
                return Task.FromResult<IFiscalIntegrationDocument>(new BeginSaleRegistrationRequest { Receipt = receipt });
            }

            /// <summary>
            /// Creates receipt.
            /// </summary>
            /// <param name="cart">The cart.</param>
            /// <returns>The receipt.</returns>
            private Receipt CreateReceipt(Cart cart)
            {
                var receipt = new Receipt
                {
                    TransactionLocation = this.documentBuilderData.RequestContext.GetOrgUnit().OrgUnitNumber,
                    TransactionTerminal = cart.TerminalId,
                    TransactionNumber = cart.Id,
                };
                return receipt;
            }
        }
    }
}
