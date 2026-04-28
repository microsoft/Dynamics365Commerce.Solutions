namespace Contoso
{
    namespace Commerce.Runtime.DocumentProvider.PosnetSample.DocumentModel
    {
        using System.Collections.Generic;
        using System.Runtime.Serialization;
        using Contoso.Commerce.Runtime.DocumentProvider.PosnetSample.DocumentModel.Protocol;

        /// <summary>
        /// Declares the document to be sent to connector.
        /// Document is a list of commands executed sequently on the connector.
        /// </summary>
        [DataContract]
        sealed class PosnetDocumentRequest : IPosnetDocumentRequest
        {
            /// <summary>
            /// The collection of commands.
            /// </summary>
            [DataMember]
            public IEnumerable<IPosnetCommandRequest> Commands { get; private set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="PosnetDocumentRequest"/>.
            /// </summary>
            /// <param name="commands">The collection of commands to be executed.</param>
            public PosnetDocumentRequest(IEnumerable<IPosnetCommandRequest> commands)
            {
                this.Commands = commands;
            }
        }
    }
}
