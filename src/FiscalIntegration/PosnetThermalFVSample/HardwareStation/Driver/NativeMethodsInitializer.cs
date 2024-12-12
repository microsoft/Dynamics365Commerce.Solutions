namespace Contoso
{
    namespace Commerce.HardwareStation.PosnetThermalFVFiscalPrinterSample
    {
        using System;

        /// <summary>
        /// The native methods caller initializer.
        /// </summary>
        internal static class NativeMethodsInitializer
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="INativeMethodsInvoker"/>.
            /// </summary>
            /// <returns>A new instance of the <see cref="INativeMethodsInvoker"/>.</returns>
            internal static INativeMethodsInvoker CreateNativeMethodCaller()
            {
                if (Environment.Is64BitProcess)
                {
                    return new NativeMethodsInvoker64();
                }
                else
                {
                    return new NativeMethodsInvoker86();
                }
            }
        }
    }
}