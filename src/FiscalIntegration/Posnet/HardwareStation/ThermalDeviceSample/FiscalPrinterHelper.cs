namespace Contoso
{
    namespace Commerce.HardwareStation.PosnetThermalFVFiscalPrinterSample
    {
        using Contoso.Commerce.HardwareStation.PosnetThermalFVFiscalPrinterSample.Driver;
        using Microsoft.Dynamics.Commerce.HardwareStation.Peripherals.Entities;

        /// <summary>
        /// Contains various helper methods.
        /// </summary>
        internal static class FiscalPrinterHelper
        {
            private const string LeverUpErrorMessage = "lever up";
            private const string NoAccessToMechanismErrorMessage = "no access to mechanism";
            private const string CoverUpErrorMessage = "cover up";
            private const string NoPaperCopyErrorMessage = "no paper (copy)";
            private const string NoPaperOriginalErrorMessage = "no paper (original)";
            private const string WrongTemperatureOrPowerSupplyErrorMessage = "wrong temperature or power supply";
            private const string MomentaryPowerSupplyShortageErrorMessage = "momentary power supply shortage";
            private const string CutterErrorErrorMessage = "cutter error";
            private const string ChargerErrorErrorMessage = "charger error";
            private const string CoverUpWhenCuttingErrorMessage = "cover up when cutting";

            /// <summary>
            /// Retrieves the error data.
            /// </summary>
            /// <param name="printerErrorCode">The printer error code.</param>
            /// <param name="deviceStatusData">The device status data.</param>
            /// <returns>The <see cref="ErrorData"/> containing error details.</returns>
            internal static ErrorData GetErrorData(ulong printerErrorCode, FiscalDeviceStatus deviceStatusData)
            {
                switch (printerErrorCode)
                {
                    case (ulong)DeviceStatusCode.POSNET_STATUS_TIMEOUT:
                        return new ErrorData(FiscalPeripheralFailureType.Busy, true, string.Empty);

                    case (ulong)DeviceStatusCode.POSNET_STATUS_PENDING:
                        return GetErrorByDeviceStatus(deviceStatusData);

                    default:
                        return new ErrorData(FiscalPeripheralFailureType.SubmissionFailed, false, string.Empty);
                }
            }

            private static ErrorData GetErrorByDeviceStatus(FiscalDeviceStatus deviceStatusData)
            {
                if (deviceStatusData.DeviceStatus != DeviceStatusType.ErrorOccured)
                {
                    return new ErrorData(FiscalPeripheralFailureType.Other, false, string.Empty);
                }

                switch (deviceStatusData.PrintingStatus)
                {
                    case PrintingStatusType.LeverUp:
                        return new ErrorData(FiscalPeripheralFailureType.Other, true, LeverUpErrorMessage);

                    case PrintingStatusType.MechanismError:
                        return new ErrorData(FiscalPeripheralFailureType.NotAvailable, true, NoAccessToMechanismErrorMessage);

                    case PrintingStatusType.CoverUp:
                        return new ErrorData(FiscalPeripheralFailureType.Other, true, CoverUpErrorMessage);

                    case PrintingStatusType.NoPaperForCopy:
                        return new ErrorData(FiscalPeripheralFailureType.PaperOut, true, NoPaperCopyErrorMessage);

                    case PrintingStatusType.NoPaperForOriginal:
                        return new ErrorData(FiscalPeripheralFailureType.PaperOut, true, NoPaperOriginalErrorMessage);

                    case PrintingStatusType.WrongTemperatureOrPowerSupply:
                        return new ErrorData(FiscalPeripheralFailureType.Other, false, WrongTemperatureOrPowerSupplyErrorMessage);

                    case PrintingStatusType.MomentaryPowerSupplyShortage:
                        return new ErrorData(FiscalPeripheralFailureType.Other, true, MomentaryPowerSupplyShortageErrorMessage);

                    case PrintingStatusType.CutterError:
                        return new ErrorData(FiscalPeripheralFailureType.Other, false, CutterErrorErrorMessage);

                    case PrintingStatusType.ChargerError:
                        return new ErrorData(FiscalPeripheralFailureType.Other, true, ChargerErrorErrorMessage);

                    case PrintingStatusType.CoverUpWhenCutting:
                        return new ErrorData(FiscalPeripheralFailureType.Other, false, CoverUpWhenCuttingErrorMessage);

                    default:
                        return new ErrorData(FiscalPeripheralFailureType.Other, false, string.Empty);
                }
            }
        }
    }
}
