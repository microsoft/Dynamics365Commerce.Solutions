namespace Contoso
{
    namespace Commerce.HardwareStation.PosnetThermalFVFiscalPrinterSample.DocumentModel.Protocol
    {
        using System;
        using System.Collections.Generic;

        /// <summary>
        /// Interface declares command request.
        /// </summary>
        public interface IPosnetCommandRequest
        {
            /// <summary>
            /// Command type.
            /// </summary>
            CommandType Type { get; }

            /// <summary>
            /// Command name.
            /// </summary>
            string CommandName { get; }

            /// <summary>
            /// The collection of command parameters.
            /// </summary>
            IEnumerable<CommandParameter> CommandParameters { get; }

            /// <summary>
            /// The collection of response parameters.
            /// </summary>
            IDictionary<string, DataType> ResponseParameters { get; }
        }
    }
}
