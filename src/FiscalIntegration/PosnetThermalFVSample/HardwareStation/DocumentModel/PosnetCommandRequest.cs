namespace Contoso
{
    namespace Commerce.HardwareStation.PosnetThermalFVFiscalPrinterSample.DocumentModel
    {
        using System;
        using System.Collections.Generic;
        using System.Runtime.Serialization;
        using Contoso.Commerce.HardwareStation.PosnetThermalFVFiscalPrinterSample.DocumentModel.Protocol;

        /// <summary>
        /// Implements command request.
        /// </summary>
        [DataContract]
        public sealed class PosnetCommandRequest : IPosnetCommandRequest
        {
            /// <summary>
            /// Command type.
            /// </summary>
            [DataMember]
            public CommandType Type { get; private set; }

            /// <summary>
            /// Command name.
            /// </summary>
            [DataMember]
            public string CommandName { get; private set; }

            /// <summary>
            /// The collection of command parameters.
            /// </summary>
            [DataMember]
            public IEnumerable<CommandParameter> CommandParameters { get; private set; }

            /// <summary>
            /// The collection of response parameters.
            /// </summary>
            public IDictionary<string, DataType> ResponseParameters { get; private set; }

            /// <summary>
            /// Initializes a new instance of <see cref="PosnetCommandRequest"/>.
            /// </summary>
            /// <param name="type">Command type.</param>
            /// <param name="name">Command name.</param>
            /// <param name="commandParameters">The collection of command parameters.</param>
            /// <param name="responseParameters">The collection of response parameters.</param>
            public PosnetCommandRequest(CommandType type, string name, IEnumerable<CommandParameter> commandParameters, IDictionary<string, DataType> responseParameters)
            {
                this.Type = type;
                this.CommandName = name;
                this.CommandParameters = commandParameters;
                this.ResponseParameters = responseParameters;
            }
        }
    }
}
