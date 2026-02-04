namespace Contoso
{
    namespace Commerce.HardwareStation.PosnetThermalFVFiscalPrinterSample
    {
        using System;
        using System.Runtime.InteropServices;

        /// <summary>
        /// Base class for driver initializers.
        /// </summary>
        internal abstract class DriverInitializer
        {
            protected static readonly INativeMethodsInvoker nativeMethodsInvoker = NativeMethodsInitializer.CreateNativeMethodCaller();

            /// <summary>
            /// Gets the printer timeout in milliseconds.
            /// </summary>
            public UInt32 PrinterTimeoutMilliseconds { get; }

            /// <summary>
            /// Initializes a new instance of the <see cref="DriverInitializer"/>.
            /// </summary>
            /// <param name="printerTimeoutMilliseconds">Printer timeout in milliseconds.</param>
            public DriverInitializer(UInt32 printerTimeoutMilliseconds)
            {
                this.PrinterTimeoutMilliseconds = printerTimeoutMilliseconds;
            }

            /// <summary>
            /// Creates a new instance of the <see cref="PosnetDriver"/>.
            /// </summary>
            /// <returns>A new instance of the <see cref="PosnetDriver"/>.</returns>
            public abstract PosnetDriver Create();

            /// <summary>
            /// Sets the device parameter.
            /// </summary>
            /// <param name="deviceHandle">The device handle.</param>
            /// <param name="paramName">The parameter name.</param>
            /// <param name="paramValue">The parameter value.</param>
            protected void SetDeviceParam(IntPtr deviceHandle, UInt32 paramName, string paramValue)
            {
                var paramValuePtr = IntPtr.Zero;
                try
                {
                    paramValuePtr = Marshal.StringToHGlobalAnsi(paramValue);
                    nativeMethodsInvoker.POS_SetDeviceParam(deviceHandle, paramName, paramValuePtr);
                }
                finally
                {
                    if (paramValuePtr != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(paramValuePtr);
                        paramValuePtr = IntPtr.Zero;
                    }
                }
            }
        }
    }
}
