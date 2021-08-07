namespace Contoso
{
    namespace Commerce.Runtime.DocumentProvider.PosnetSample.DocumentBuilder
    {
        using System;
        using System.Collections.Generic;
        using System.Threading.Tasks;
        using Contoso.Commerce.Runtime.DocumentProvider.PosnetSample.DocumentModel;
        using Contoso.Commerce.Runtime.DocumentProvider.PosnetSample.DocumentModel.Protocol;
        using Contoso.Commerce.Runtime.DocumentProvider.PosnetSample.PosnetProtocol;
        using Contoso.Commerce.Runtime.DocumentProvider.PosnetSample.PosnetProtocol.Requests;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.FiscalIntegration.DocumentProvider.Messages;

        /// <summary>
        /// Implements builder for X/Z report.
        /// </summary>
        internal class XZReportBuilder : IFiscalDocumentRequestBuilder
        {
            private GetFiscalDocumentDocumentProviderRequest Request;
            private const string DefaultShiftName = "1";

            public XZReportBuilder(GetFiscalDocumentDocumentProviderRequest request)
            {
                this.Request = request;
            }

            /// <summary>
            /// Builds fiscal document for X/Z report.
            /// </summary>
            /// <returns>The <see cref="PosnetFiscalDocumentBuildResult"/> instance.</returns>
            public async Task<PosnetFiscalDocumentBuildResult> BuildAsync()
            {
                IEnumerable<IPosnetCommandRequest> commands = null;
                FiscalIntegrationEventType eventType = Request.FiscalDocumentRetrievalCriteria.FiscalRegistrationEventType;

                if (eventType == FiscalIntegrationEventType.FiscalXReport)
                {
                    commands = this.CreateReport(false);
                }
                else if (eventType == FiscalIntegrationEventType.FiscalZReport)
                {
                    commands = this.CreateReport(true);
                }

                if (commands == null)
                {
                    throw new Exception($"Event type {eventType} is not supported.");
                }

                return await Task.FromResult(new PosnetFiscalDocumentBuildResult
                {
                    Document = new PosnetDocumentRequest(commands),
                    DocumentAdjustment = new FiscalIntegrationDocumentAdjustment(),
                    DocumentGenerationResult = FiscalIntegrationDocumentGenerationResultType.Succeeded
                });
            }

            /// <summary>
            /// Creates X or Z report depends on parameter.
            /// </summary>
            /// <param name="isZReport">Defines whether X or Z report should be created.</param>
            /// <returns>An array with X/Z report request.</returns>
            private IEnumerable<IPosnetCommandRequest> CreateReport(bool isZReport)
            {
                ShiftReport request = new ShiftReport
                {
                    ShiftName = DefaultShiftName,
                    ClearReport = isZReport
                };

                return new IPosnetCommandRequest[]
                {
                    PrinterRequestBuilder.BuildRequestCommand(request)
                };
            }
        }
    }
}
