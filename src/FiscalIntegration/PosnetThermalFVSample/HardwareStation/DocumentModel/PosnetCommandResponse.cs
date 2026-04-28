namespace Contoso
{
    namespace Commerce.HardwareStation.PosnetThermalFVFiscalPrinterSample.DocumentModel
    {
        using System;
        using System.Collections.Generic;
        using System.Runtime.Serialization;
        using System.Linq;
        using Contoso.Commerce.HardwareStation.PosnetThermalFVFiscalPrinterSample.DocumentModel.Protocol;
        using Microsoft.Dynamics.Commerce.HardwareStation;

        /// <summary>
        /// Implements document response.
        /// </summary>
        [DataContract]
        sealed class PosnetCommandResponse : IPosnetCommandResponse
        {
            /// <summary>
            /// Name of request.
            /// </summary>
            [DataMember]
            public string RequestCommandName { get; }

            /// <summary>
            /// Indicates whether the request was executed successfully.
            /// </summary>
            [DataMember]
            public bool Success { get; }

            /// <summary>
            /// The code returned by the printer.
            /// </summary>
            [DataMember]
            public ulong Code { get; }

            /// <summary>
            /// Collection of result parameters.
            /// </summary>
            [DataMember]
            public IEnumerable<CommandParameter> Results { get; }

            /// <summary>
            /// Error message returned by the printer.
            /// </summary>
            [DataMember]
            public string ErrorMessage { get; }
            
            /// <summary>
            /// Initializes a new instance of <see cref="PosnetCommandRequest"/>.
            /// </summary>
            /// <param name="requestCommandName">Name of request.</param>
            /// <param name="code">The code returned by the printer.</param>
            /// <param name="results">The collection of the command execution results.</param>
            public PosnetCommandResponse(string requestCommandName, ulong code, IEnumerable<CommandParameter> results)
                : this(requestCommandName, true, code)
            {
                ThrowIf.Null(results, nameof(results));

                this.ErrorMessage = string.Empty;
                this.Results = results;
            }

            /// <summary>
            /// Initializes a new instance of <see cref="PosnetCommandRequest"/>.
            /// </summary>
            /// <param name="requestCommandName">Name of request.</param>
            /// <param name="code">The code returned by the printer.</param>
            /// <param name="errorMessage">The error message.</param>
            public PosnetCommandResponse(string requestCommandName, ulong code, string errorMessage)
                : this(requestCommandName, false, code)
            {
                ThrowIf.Null(errorMessage, nameof(errorMessage));

                this.ErrorMessage = errorMessage;
                this.Results = Enumerable.Empty<CommandParameter>();
            }

            /// <summary>
            /// Initializes a new instance of <see cref="PosnetCommandRequest"/>.
            /// </summary>
            /// <param name="requestCommandName">Name of request.</param>
            /// <param name="success">Indicates whether the request was executed successfully.</param>
            /// <param name="code">The code returned by the printer.</param>
            private PosnetCommandResponse(string requestCommandName, bool success, ulong code)
            {
                ThrowIf.NullOrWhiteSpace(requestCommandName, nameof(requestCommandName));

                this.RequestCommandName = requestCommandName;
                this.Success = success;
                this.Code = code;
            }
        }
    }
}