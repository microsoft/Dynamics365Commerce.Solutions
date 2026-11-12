namespace Contoso
{
    namespace Commerce.HardwareStation.PosnetThermalFVFiscalPrinterSample
    {
        using System;
        using System.Text;

        /// <summary>
        /// Provides the common interface to invoke the POSNET fiscal device methods regardless of platform bitness.
        /// </summary>
        internal interface INativeMethodsInvoker
        {
            /// <summary>
            /// Creates a device handle.
            /// </summary>
            /// <param name="deviceType">The device type.</param>
            /// <returns>The device handle.</returns>
            IntPtr POS_CreateDeviceHandle(UInt32 deviceType);

            /// <summary>
            /// Sets the debug level.
            /// </summary>
            /// <param name="hDevice">The device handle.</param>
            /// <param name="debugLevel">The debug level.</param>
            /// <returns>The result pointer.</returns>
            IntPtr POS_SetDebugLevel(IntPtr hDevice, UInt32 debugLevel);

            /// <summary>
            /// Sets a device parameter.
            /// </summary>
            /// <param name="hDevice">The device handle.</param>
            /// <param name="paramCode">The parameter code.</param>
            /// <param name="paramValue">The parameter value.</param>
            /// <returns>The result pointer.</returns>
            IntPtr POS_SetDeviceParam(IntPtr hDevice, UInt32 paramCode, IntPtr paramValue);

            /// <summary>
            /// Creates a request.
            /// </summary>
            /// <param name="hLocalDevice">The local device handle.</param>
            /// <param name="command">The command.</param>
            /// <returns>The request handle.</returns>
            IntPtr POS_CreateRequest(IntPtr hLocalDevice, string command);

            /// <summary>
            /// Creates a request with parameters.
            /// </summary>
            /// <param name="hLocalDevice">The local device handle.</param>
            /// <param name="command">The command.</param>
            /// <param name="parameters">The parameters.</param>
            /// <returns>The request handle.</returns>
            IntPtr POS_CreateRequestEx(IntPtr hLocalDevice, IntPtr command, IntPtr parameters);

            /// <summary>
            /// Opens the device.
            /// </summary>
            /// <param name="hDevice">The device handle.</param>
            /// <returns>The local device handle.</returns>
            IntPtr POS_OpenDevice(IntPtr hDevice);

            /// <summary>
            /// Pushes a request parameter.
            /// </summary>
            /// <param name="hLocalDevice">The local device handle.</param>
            /// <param name="param_name">The parameter name.</param>
            /// <param name="param_value">The parameter value.</param>
            /// <returns>The result code.</returns>
            UInt32 POS_PushRequestParam(IntPtr hLocalDevice, string param_name, string param_value);

            /// <summary>
            /// Posts the request.
            /// </summary>
            /// <param name="hRequest">The request handle.</param>
            /// <param name="mode">The mode.</param>
            /// <returns>The result code.</returns>
            UInt32 POS_PostRequest(IntPtr hRequest, byte mode);

            /// <summary>
            /// Waits for request completion.
            /// </summary>
            /// <param name="hRequest">The request handle.</param>
            /// <param name="timeout">The timeout.</param>
            /// <returns>The result code.</returns>
            UInt32 POS_WaitForRequestCompleted(IntPtr hRequest, UInt32 timeout);

            /// <summary>
            /// Gets the request status.
            /// </summary>
            /// <param name="hRequest">The request handle.</param>
            /// <returns>The request status.</returns>
            UInt32 POS_GetRequestStatus(IntPtr hRequest);

            /// <summary>
            /// Destroys the request.
            /// </summary>
            /// <param name="hRequest">The request handle.</param>
            /// <returns>The result code.</returns>
            UInt32 POS_DestroyRequest(IntPtr hRequest);

            /// <summary>
            /// Closes the device.
            /// </summary>
            /// <param name="hLocalDevice">The local device handle.</param>
            /// <returns>The result code.</returns>
            UInt32 POS_CloseDevice(IntPtr hLocalDevice);

            /// <summary>
            /// Destroys the device handle.
            /// </summary>
            /// <param name="hDevice">The device handle.</param>
            /// <returns>The result code.</returns>
            UInt32 POS_DestroyDeviceHandle(IntPtr hDevice);

            /// <summary>
            /// Gets the error code.
            /// </summary>
            /// <param name="hLocalDevice">The local device handle.</param>
            /// <returns>The error code.</returns>
            UInt32 POS_GetError(IntPtr hLocalDevice);

            /// <summary>
            /// Gets the printer device status.
            /// </summary>
            /// <param name="hLocalDevice">The local device handle.</param>
            /// <param name="statusMode">The status mode.</param>
            /// <param name="globalStatus">The global status.</param>
            /// <param name="printerStatus">The printer status.</param>
            /// <returns>The result code.</returns>
            UInt32 POS_GetPrnDeviceStatus(IntPtr hLocalDevice, char statusMode, ref UInt32 globalStatus, ref UInt32 printerStatus);

            /// <summary>
            /// Gets the error string.
            /// </summary>
            /// <param name="errorCode">The error code.</param>
            /// <param name="lang">The language.</param>
            /// <returns>The error string pointer.</returns>
            IntPtr POS_GetErrorString(UInt32 errorCode, string lang);

            /// <summary>
            /// Gets the response value.
            /// </summary>
            /// <param name="request">The request handle.</param>
            /// <param name="paramName">The parameter name.</param>
            /// <param name="retVal">The returned value.</param>
            /// <param name="retLength">The returned value length.</param>
            /// <returns>The result code.</returns>
            UInt32 POS_GetResponseValue(IntPtr request, string paramName, StringBuilder retVal, UInt32 retLength);
        }
    }
}
