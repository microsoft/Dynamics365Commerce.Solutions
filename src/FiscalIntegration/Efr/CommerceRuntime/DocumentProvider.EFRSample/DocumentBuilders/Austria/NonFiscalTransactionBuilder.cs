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
    namespace CommerceRuntime.DocumentProvider.EFRSample.DocumentBuilders.Austria
    {
        using System.Threading.Tasks;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
        using Contoso.CommerceRuntime.DocumentProvider.DataModelEFR.Documents;
        using Contoso.CommerceRuntime.DocumentProvider.EFRSample.DocumentBuilders.Parameters;
        using Contoso.CommerceRuntime.DocumentProvider.EFRSample.Messages;

        /// <summary>
        /// Incapsulates the non-fiscal transaction document generation logic specific for Austia.
        /// </summary>
        internal class NonFiscalTransactionBuilder : IDocumentBuilder
        {
            /// <summary>
            /// The request.
            /// </summary>
            private readonly DocumentBuilderData documentBuilderData;

            /// <summary>
            /// Initializes a new instance of the <see cref="NonFiscalTransactionBuilder"/> class.
            /// </summary>
            /// <param name="documentBuilderData">The request.</param>
            public NonFiscalTransactionBuilder(DocumentBuilderData documentBuilderData)
            {
                ThrowIf.Null(documentBuilderData, nameof(documentBuilderData));

                this.documentBuilderData = documentBuilderData;
            }

            /// <summary>
            /// Builds the fiscal integration document.
            /// </summary>
            /// <returns>The non-fiscal transaction receipt document.</returns>
            public async Task<IFiscalIntegrationDocument> BuildAsync()
            {
                return new NonFiscalEventRegistrationRequest()
                {
                    Receipt = await CreateReceiptAsync().ConfigureAwait(false)
                };
            }

            /// <summary>
            /// Creates receipt.
            /// </summary>
            /// <returns>The receipt.</returns>
            private async Task<NonFiscalReceipt> CreateReceiptAsync()
            {
                NonFiscalReceipt receipt = new NonFiscalReceipt
                {
                    CountryRegionISOCode = this.documentBuilderData.RequestContext.GetChannelConfiguration().CountryRegionISOCode,
                    IsTrainingTransaction = 0,
                };

                if (this.documentBuilderData.NonFiscalDocumentRetrievalCriteria.FiscalRegistrationEventType == FiscalIntegrationEventType.AuditEvent &&
                    this.documentBuilderData.NonFiscalDocumentRetrievalCriteria.DocumentContext.AuditEvent != null)
                {
                    var auditEvent = this.documentBuilderData.NonFiscalDocumentRetrievalCriteria.DocumentContext.AuditEvent;
                    receipt.ReceiptDateTime = auditEvent.EventDateTime.DateTime;
                    receipt.TransactionLocation = auditEvent.Store;
                    receipt.TransactionTerminal = auditEvent.Terminal;
                    receipt.OperatorId = auditEvent.Staff;
                    receipt.OperatorName = string.IsNullOrEmpty(auditEvent.Staff) ? string.Empty : await GetOperatorName(auditEvent.Staff).ConfigureAwait(false);
                    receipt.NonFiscalTransactionType = await TranslateAsync(this.documentBuilderData.RequestContext, this.documentBuilderData.RequestContext.LanguageId, auditEvent.ExtensibleAuditEventType.Name).ConfigureAwait(false);
                }
                else if (this.documentBuilderData.NonFiscalDocumentRetrievalCriteria.FiscalRegistrationEventType != FiscalIntegrationEventType.AuditEvent)
                {
                    var staffId = this.documentBuilderData.RequestContext.GetPrincipal().UserId;
                    receipt.OperatorId = staffId;
                    receipt.ReceiptDateTime = this.documentBuilderData.RequestContext.GetNowInChannelTimeZone().DateTime;
                    receipt.OperatorName = string.IsNullOrEmpty(staffId) ? string.Empty : await GetOperatorName(staffId).ConfigureAwait(false);
                    receipt.TransactionLocation = this.documentBuilderData.RequestContext.GetOrgUnit().OrgUnitNumber;
                    receipt.TransactionTerminal = this.documentBuilderData.NonFiscalDocumentRetrievalCriteria.ShiftTerminalId;
                    receipt.NonFiscalTransactionType = await TranslateAsync(this.documentBuilderData.RequestContext, this.documentBuilderData.RequestContext.LanguageId, this.documentBuilderData.NonFiscalDocumentRetrievalCriteria.FiscalRegistrationEventType.ToString()).ConfigureAwait(false);
                }

                return receipt;
            }

            private static async Task<string> TranslateAsync(RequestContext requestContext, string cultureName, string textId)
            {
                return (await requestContext.ExecuteAsync<SingleEntityDataServiceResponse<string>>(new LocalizeEfrResourceRequest(cultureName, textId)).ConfigureAwait(false)).Entity;
            }

            private async Task<string> GetOperatorName(string staffId)
            {
                var request = new GetEfrOperatorNameRequest(staffId);
                return (await this.documentBuilderData.RequestContext.ExecuteAsync<SingleEntityDataServiceResponse<string>>(request).ConfigureAwait(false)).Entity;
            }
        }
    }
}
