namespace Contoso
{
    namespace Commerce.HardwareStation.PosnetThermalFVFiscalPrinterSample.DocumentModel
    {
        using System.Collections.Generic;
        using System.Runtime.Serialization;
        using Contoso.Commerce.HardwareStation.PosnetThermalFVFiscalPrinterSample.DocumentModel.Protocol;

        /// <summary>
        /// Declares the document to be sent to connector.
        /// Document is a list of commands executed sequently on the connector.
        /// </summary>
        [DataContract]
        internal sealed class PosnetDocumentRequest : IPosnetDocumentRequest
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
            public PosnetDocumentRequest(IEnumerable<PosnetCommandRequest> commands)
            {
                this.Commands = commands;
            }
        }
    }
}