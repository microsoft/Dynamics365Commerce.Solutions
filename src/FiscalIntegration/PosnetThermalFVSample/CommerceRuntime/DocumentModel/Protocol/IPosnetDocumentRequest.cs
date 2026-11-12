namespace Contoso
{
    namespace Commerce.Runtime.DocumentProvider.PosnetSample.DocumentModel.Protocol
    {
        using System.Collections.Generic;

        /// <summary>
        /// Declares the document to be sent to connector.
        /// Document is a list of commands executed sequentially on the connector.
        /// </summary>
        public interface IPosnetDocumentRequest
        {
            /// <summary>
            /// The collection of commands.
            /// </summary>
            IEnumerable<IPosnetCommandRequest> Commands { get; }
        }
    }
}
