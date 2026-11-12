namespace Contoso
{
    namespace Commerce.HardwareStation.PosnetThermalFVFiscalPrinterSample.Driver
    {
        using System;

        /// <summary>
        /// Represents fiscal device status data.
        /// </summary>
        internal struct FiscalDeviceStatus
        {
            /// <summary>
            /// The device status.
            /// </summary>
            public DeviceStatusType DeviceStatus { get; }

            /// <summary>
            /// The printing mechanism status.
            /// </summary>
            public PrintingStatusType PrintingStatus { get; }

            /// <summary>
            /// Initializes a new instance of <see cref="Driver.FiscalDeviceStatus"/>.
            /// </summary>
            /// <param name="deviceStatus">The device status value.</param>
            /// <param name="printingStatus">The printing status value.</param>
            public FiscalDeviceStatus(UInt32 deviceStatus, UInt32 printingStatus)
            {
                if (Enum.IsDefined(typeof(DeviceStatusType), deviceStatus))
                {
                    this.DeviceStatus = (DeviceStatusType)deviceStatus;
                }
                else
                {
                    this.DeviceStatus = DeviceStatusType.Other;
                }

                if (Enum.IsDefined(typeof(PrintingStatusType), printingStatus))
                {
                    this.PrintingStatus = (PrintingStatusType)printingStatus;
                }
                else
                {
                    this.PrintingStatus = PrintingStatusType.Other;
                }
            }

            /// <summary>
            /// Retrieves the device availability.
            /// </summary>
            /// <returns>
            /// True, if the device is ready;
            /// Otherwise, false.
            /// </returns>
            public bool IsDeviceReady() => this.DeviceStatus == DeviceStatusType.Ready && this.PrintingStatus == PrintingStatusType.NoError;
        }
    }
}
