namespace Contoso
{
    namespace Commerce.HardwareStation.PosnetThermalFVFiscalPrinterSample.DocumentModel
    {
        using Contoso.Commerce.HardwareStation.PosnetThermalFVFiscalPrinterSample.DocumentModel.Protocol;

        /// <summary>
        /// Represents the result class of command and response validation for posnet.
        /// </summary>
        internal class PosnetCommandValidationResult
        {
            /// <summary>
            /// Gets an indicator of whether the validation was successful.
            /// </summary>
            public bool Success { get; }

            /// <summary>
            /// Gets the posnet response.
            /// </summary>
            public IPosnetCommandResponse Response { get; }

            /// <summary>
            /// Initializes a new instance of <see cref="PosnetCommandValidationResult"/>.
            /// </summary>
            /// <param name="success">Indicate if the verification was successful.</param>
            public PosnetCommandValidationResult(bool success)
            {
                Success = success;
            }

            /// <summary>
            /// Initializes a new instance of <see cref="PosnetCommandValidationResult"/>.
            /// </summary>
            /// <param name="success">Indicate if the verification was successful.</param>
            /// <param name="response">The posnet response.</param>
            public PosnetCommandValidationResult(bool success, IPosnetCommandResponse response)
                : this(success)
            {
                Response = response;
            }
        }
    }
}