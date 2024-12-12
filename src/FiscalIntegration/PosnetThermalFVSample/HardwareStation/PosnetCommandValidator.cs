namespace Contoso
{
    namespace Commerce.HardwareStation.PosnetThermalFVFiscalPrinterSample
    {
        using Contoso.Commerce.HardwareStation.PosnetThermalFVFiscalPrinterSample.DocumentModel;
        using Contoso.Commerce.HardwareStation.PosnetThermalFVFiscalPrinterSample.DocumentModel.Protocol;
        using Contoso.Commerce.HardwareStation.PosnetThermalFVFiscalPrinterSample.Driver;

        /// <summary>
        /// Represents the posnet command and response validation class.
        /// </summary>
        internal class PosnetCommandValidator
        {
            private readonly PosnetDriver posnetDriver;

            /// <summary>
            /// Initializes a new instance of <see cref="PosnetCommandValidator"/>.
            /// </summary>
            /// <param name="posnetDriver">The posnet driver.</param>
            public PosnetCommandValidator(PosnetDriver posnetDriver)
            {
                this.posnetDriver = posnetDriver;
            }

            /// <summary>
            /// Checks if the command can be executed.
            /// </summary>
            /// <param name="posnetRequest">The posnet command request.</param>
            /// <returns>The POSNET command validation result.</returns>
            internal PosnetCommandValidationResult CanExecute(IPosnetCommandRequest posnetRequest)
            {
                bool success = true;

                switch (posnetRequest.CommandName)
                {
                    case PosnetCommands.EndOfTransactionCommandName:
                        success = !HasOutOfPaperError();
                        break;
                }

                return new PosnetCommandValidationResult(success);
            }

            /// <summary>
            /// Gets the result of the command execution.
            /// </summary>
            /// <param name="posnetRequest">The posnet command request.</param>
            /// <param name="posnetResponse">The posnet command response.</param>
            /// <returns>The POSNET command validation result.</returns>
            internal PosnetCommandValidationResult GetExecutionResult(IPosnetCommandRequest posnetRequest, IPosnetCommandResponse posnetResponse)
            {
                bool success = posnetResponse.Success;
                IPosnetCommandResponse response = posnetResponse;

                switch (posnetRequest.CommandName)
                {
                    case PosnetCommands.ShiftReportCommandName:
                        if (!success && HasOutOfPaperError())
                        {
                            // if the paper in the printer ran out when the command to print the report is executed,
                            // then ignore the error and add the answer that the command was completed successfully.
                            response = new PosnetCommandResponse(posnetRequest.CommandName, posnetResponse.Code, new CommandParameter[0]);
                        }
                        break;
                }

                return new PosnetCommandValidationResult(success, response);
            }

            /// <summary>
            /// Checks if the printer has an out of paper error.
            /// </summary>
            /// <returns>True if printer has out of paper error, otherwise false.</returns>
            private bool HasOutOfPaperError()
            {
                var response = posnetDriver.GetDeviceStatus();
                return response.DeviceStatus == DeviceStatusType.ErrorOccured
                    && (response.PrintingStatus == PrintingStatusType.NoPaperForOriginal
                    || response.PrintingStatus == PrintingStatusType.NoPaperForCopy);
            }
        }
    }
}