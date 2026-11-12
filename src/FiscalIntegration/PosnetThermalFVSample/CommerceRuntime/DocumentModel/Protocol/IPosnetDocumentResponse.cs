namespace Contoso
{
    namespace Commerce.Runtime.DocumentProvider.PosnetSample.DocumentModel.Protocol
    {
        using System.Collections.Generic;

        /// <summary>
        /// The response for document request.
        /// </summary>
        public interface IPosnetDocumentResponse
        {
            /// <summary>
            /// Shows whether the sent document executed with errors.
            /// </summary>
            bool Success { get; }

            /// <summary>
            /// The collection of responses.
            /// </summary>
            IEnumerable<IPosnetCommandResponse> Responses { get; }
        }
    }
}
