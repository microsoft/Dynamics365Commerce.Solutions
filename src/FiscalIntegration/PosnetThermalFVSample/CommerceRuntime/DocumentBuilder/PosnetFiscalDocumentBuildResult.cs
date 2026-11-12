namespace Contoso
{
    namespace Commerce.Runtime.DocumentProvider.PosnetSample.DocumentBuilder
    {
        using Contoso.Commerce.Runtime.DocumentProvider.PosnetSample.DocumentModel.Protocol;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;

        /// <summary>
        /// The result of document building.
        /// </summary>
        public class PosnetFiscalDocumentBuildResult
        {
            /// <summary>
            /// The fiscal document.
            /// </summary>
            public IPosnetDocumentRequest Document { get; set; }

            /// <summary>
            /// The adjusted fiscal document.
            /// </summary>
            public FiscalIntegrationDocumentAdjustment DocumentAdjustment { get; set; }

            /// <summary>
            /// The document generation result.
            /// </summary>
            public FiscalIntegrationDocumentGenerationResultType DocumentGenerationResult { get; set; }
        }
    }
}
