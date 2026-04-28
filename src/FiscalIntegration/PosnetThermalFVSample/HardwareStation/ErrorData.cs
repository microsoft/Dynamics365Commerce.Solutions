namespace Contoso
{
    namespace Commerce.HardwareStation.PosnetThermalFVFiscalPrinterSample
    {
        using Microsoft.Dynamics.Commerce.HardwareStation;
        using Microsoft.Dynamics.Commerce.HardwareStation.Peripherals.Entities;

        /// <summary>
        /// The error data to be sent as response.
        /// </summary>
        internal class ErrorData
        {
            /// <summary>
            /// The failure type.
            /// </summary>
            public FiscalPeripheralFailureType FailureType { get; }

            /// <summary>
            /// The value indicating whether retry is allowed.
            /// </summary>
            public bool IsRetryAllowed { get; }

            /// <summary>
            /// The error message.
            /// </summary>
            public string ErrorMessage { get; }

            /// <summary>
            /// Initializes a new instance of <see cref="ErrorData"/>.
            /// </summary>
            /// <param name="failureType">The failure type.</param>
            /// <param name="isRetryAllowed">The value indicating whether retry is allowed.</param>
            /// <param name="errorMessage"></param>
            public ErrorData(FiscalPeripheralFailureType failureType, bool isRetryAllowed, string errorMessage)
            {
                ThrowIf.Null(errorMessage, nameof(errorMessage));

                this.FailureType = failureType;
                this.IsRetryAllowed = isRetryAllowed;
                this.ErrorMessage = errorMessage;
            }
        }
    }
}
