namespace Contoso
{
    namespace Commerce.HardwareStation.PosnetThermalFVFiscalPrinterSample
    {
        using System;
        using Contoso.Commerce.HardwareStation.PosnetThermalFVFiscalPrinterSample.Driver;

        /// <summary>
        /// The COM port driver initializer.
        /// </summary>
        internal sealed class SerialConnectionDriverInitializer : DriverInitializer
        {
            /// <summary>
            /// Gets the connection string.
            /// </summary>
            public string ConnectionString { get; }

            /// <summary>
            /// Initializes a new instance of <see cref="SerialConnectionDriverInitializer"/>.
            /// </summary>
            /// <param name="printerTimeoutMilliseconds">Printer timeout in milliseconds.</param>
            /// <param name="connectionString">The connection string.</param>
            public SerialConnectionDriverInitializer(UInt32 printerTimeoutMilliseconds, string connectionString)
                : base(printerTimeoutMilliseconds)
            {
                if (string.IsNullOrWhiteSpace(connectionString))
                    throw new ArgumentNullException(nameof(connectionString));

                this.ConnectionString = connectionString;
            }

            /// <summary>
            /// Creates a new instance of the <see cref="PosnetDriver"/>.
            /// </summary>
            /// <returns>A new instance of the <see cref="PosnetDriver"/>.</returns>
            public override PosnetDriver Create()
            {
                var deviceHandle = nativeMethodsInvoker.POS_CreateDeviceHandle((UInt32)DeviceConstants.POSNET_INTERFACE_RS232);
                this.SetDeviceParam(deviceHandle, (UInt32)DeviceConstants.POSNET_DEV_PARAM_COMSETTINGS, ConnectionString);
                var driver = new PosnetDriver(PrinterTimeoutMilliseconds, deviceHandle);
                return driver;
            }
        }
    }
}
