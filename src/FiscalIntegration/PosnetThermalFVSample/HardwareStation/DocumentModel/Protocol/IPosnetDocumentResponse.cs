namespace Contoso
{
    namespace Commerce.HardwareStation.PosnetThermalFVFiscalPrinterSample.DocumentModel.Protocol
    {
        using System.Collections.Generic;

        /// <summary>
        /// The response for document request.
        /// </summary>
        public interface IPosnetDocumentResponse
        {
            /// <summary>
            /// Indicates whether the request was executed successfully.
            /// </summary>
            bool Success { get; }

            /// <summary>
            /// The collection of responses.
            /// </summary>
            IEnumerable<IPosnetCommandResponse> Responses { get; }
        }
    }
}
