namespace Contoso
{
    namespace Commerce.HardwareStation.PosnetThermalFVFiscalPrinterSample.Driver
    {
        using System;
        
        /// <summary>
        /// The device status.
        /// </summary>
        internal enum DeviceStatusType : UInt32
        {
            /// <summary>
            /// Device is ready.
            /// </summary>
            Ready = 0,

            /// <summary>
            /// Device is in menu.
            /// </summary>
            InMenu = 1,

            /// <summary>
            /// Device is waiting for key.
            /// </summary>
            WaitingForKey = 2,

            /// <summary>
            /// Device is waiting for user input (error occured).
            /// </summary>
            ErrorOccured = 3,

            /// <summary>
            /// Unknown status.
            /// </summary>
            Other = 65537
        }
    }
}
