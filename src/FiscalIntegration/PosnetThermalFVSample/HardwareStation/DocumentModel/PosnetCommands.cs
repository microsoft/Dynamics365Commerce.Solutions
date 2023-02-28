namespace Contoso
{
    namespace Commerce.HardwareStation.PosnetThermalFVFiscalPrinterSample.DocumentModel
    {
        using System;
        using System.Collections.Generic;
        using Contoso.Commerce.HardwareStation.PosnetThermalFVFiscalPrinterSample.DocumentModel.Protocol;

        /// <summary>
        /// Represents the commands executed on the printer.
        /// </summary>
        internal static class PosnetCommands
        {
            private const string DateTimeRequestParameterName = "da";

            private const string DateTimeFormat = @"yyyy-MM-dd;HH:mm";

            internal const string InitializeTransactionCommandName = "trinit";
            internal const string TransactionPaymentCommandName = "trpayment";
            internal const string SetDateTimeCommandName = "rtcset";
            internal const string CancelPaymentCommandName = "trpaymentcanc";
            internal const string CancelTransactionCommandName = "trcancel";
            internal const string EndOfTransactionCommandName = "trend";
            internal const string ShiftReportCommandName = "shiftrep";

            /// <summary>
            /// Checks if the command is the shift report command.
            /// </summary>
            /// <param name="commandName">The command name.</param>
            /// <returns>True if the command is the cancel transaction command, otherwise false.</returns>
            internal static bool IsCancelTransactionCommand(string commandName)
            {
                return commandName.Equals(CancelTransactionCommandName, StringComparison.OrdinalIgnoreCase);
            }

            /// <summary>
            /// Creates an instance of <see cref="IPosnetDocumentRequest"/> with a request to set printer datetime to the local machine datetime.
            /// </summary>
            /// <returns>A new instace if <see cref="IPosnetDocumentRequest"/></returns>
            internal static PosnetDocumentRequest CreateSynchronizeDateTimeRequest()
            {
                var commandParameterValue = new CommandParameterValue(DateTime.Now.ToString(DateTimeFormat), DataType.Alphanum);
                var commandParameter = new CommandParameter(DateTimeRequestParameterName, commandParameterValue);
                var commandRequest = new PosnetCommandRequest(CommandType.Request, SetDateTimeCommandName, new CommandParameter[] { commandParameter }, new Dictionary<string, DataType>());

                return new PosnetDocumentRequest(new PosnetCommandRequest[] { commandRequest });
            }

            /// <summary>
            /// Creates an instance of <see cref="IPosnetDocumentRequest"/> with a request to cancel payment on the printer.
            /// </summary>
            /// <returns>A new instace if <see cref="IPosnetDocumentRequest"/></returns>
            internal static IPosnetCommandRequest CreateCancelPaymentCommandRequest()
            {
                var commandRequest = new PosnetCommandRequest(CommandType.Request, CancelPaymentCommandName, new CommandParameter[0], new Dictionary<string, DataType>());
                return commandRequest;
            }

            /// <summary>
            /// Creates an instance of <see cref="IPosnetDocumentRequest"/> with a request to cancel transaction on the printer.
            /// </summary>
            /// <returns>A new instace if <see cref="IPosnetDocumentRequest"/></returns>
            internal static IPosnetCommandRequest CreateCancelTransactionCommandRequest()
            {
                var commandRequest = new PosnetCommandRequest(CommandType.Request, CancelTransactionCommandName, new CommandParameter[0], new Dictionary<string, DataType>());
                return commandRequest;
            }
        }
    }
}