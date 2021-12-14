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
    namespace CommerceRuntime.DocumentProvider.EFRSample.DocumentBuilders
    {
        using System.Threading.Tasks;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Contoso.CommerceRuntime.DocumentProvider.DataModelEFR.Documents;
        using Contoso.CommerceRuntime.DocumentProvider.EFRSample.DocumentBuilders.Parameters;

        /// <summary>
        /// Incapsulates the document generation logic for Z-report printing event for Germany.
        /// </summary>
        public class ZReportBuilder : IDocumentBuilder
        {
            private const string TransactionTypeCode = "Z";

            /// <summary>
            /// The request.
            /// </summary>
            private readonly DocumentBuilderData documentBuilderData;

            /// <summary>
            /// Initializes a new instance of the <see cref="ZReportBuilder"/> class.
            /// </summary>
            /// <param name="documentBuilderData">The request.</param>
            public ZReportBuilder(DocumentBuilderData documentBuilderData)
            {
                ThrowIf.Null(documentBuilderData, nameof(documentBuilderData));

                this.documentBuilderData = documentBuilderData;
            }

            /// <summary>
            /// Builds fiscal integration document.
            /// </summary>
            /// <returns> The fiscal integration receipt document.</returns>
            public Task<IFiscalIntegrationDocument> BuildAsync()
            {
                IFiscalIntegrationDocument fiscalDocument = new NonFiscalEventRegistrationRequest
                {
                    Receipt = new NonFiscalReceipt
                    {
                        TransactionLocation = this.documentBuilderData.RequestContext.GetOrgUnit().OrgUnitNumber,
                        TransactionTerminal = this.documentBuilderData.FiscalDocumentRetrievalCriteria.ShiftTerminalId,
                        NonFiscalSignedTransactionType = TransactionTypeCode,
                    }
                };

                return Task.FromResult(fiscalDocument);
            }
        }
    }
}
