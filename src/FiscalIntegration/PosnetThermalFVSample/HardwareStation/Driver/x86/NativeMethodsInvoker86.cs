namespace Contoso
{
    namespace Commerce.HardwareStation.PosnetThermalFVFiscalPrinterSample
    {
        using System;
        using System.Text;

        /// <summary>
        /// Invokes fiscal device methods in 32-bit dll.
        /// </summary>
        internal sealed class NativeMethodsInvoker86 : INativeMethodsInvoker
        {
            public UInt32 POS_CloseDevice(IntPtr hLocalDevice) => NativeMethods86.POS_CloseDevice(hLocalDevice);

            public IntPtr POS_CreateDeviceHandle(UInt32 deviceType) => NativeMethods86.POS_CreateDeviceHandle(deviceType);

            public IntPtr POS_CreateRequest(IntPtr hLocalDevice, string command) => NativeMethods86.POS_CreateRequest(hLocalDevice, command);

            public IntPtr POS_CreateRequestEx(IntPtr hLocalDevice, IntPtr command, IntPtr parameters) => NativeMethods86.POS_CreateRequestEx(hLocalDevice, command, parameters);

            public UInt32 POS_DestroyDeviceHandle(IntPtr hDevice) => NativeMethods86.POS_DestroyDeviceHandle(hDevice);

            public UInt32 POS_DestroyRequest(IntPtr hRequest) => NativeMethods86.POS_DestroyRequest(hRequest);

            public UInt32 POS_GetError(IntPtr hLocalDevice) => NativeMethods86.POS_GetError(hLocalDevice);

            public IntPtr POS_GetErrorString(UInt32 errorCode, string lang) => NativeMethods86.POS_GetErrorString(errorCode, lang);

            public UInt32 POS_GetPrnDeviceStatus(IntPtr hLocalDevice, char statusMode, ref UInt32 globalStatus, ref UInt32 printerStatus) =>
                NativeMethods86.POS_GetPrnDeviceStatus(hLocalDevice, statusMode, ref globalStatus, ref printerStatus);

            public UInt32 POS_GetRequestStatus(IntPtr hRequest) =>
                NativeMethods86.POS_GetRequestStatus(hRequest);

            public UInt32 POS_GetResponseValue(IntPtr request, string paramName, StringBuilder retVal, UInt32 retLength) =>
                NativeMethods86.POS_GetResponseValue(request, paramName, retVal, retLength);

            public IntPtr POS_OpenDevice(IntPtr hDevice) => NativeMethods86.POS_OpenDevice(hDevice);

            public UInt32 POS_PostRequest(IntPtr hRequest, byte mode) => NativeMethods86.POS_PostRequest(hRequest, mode);

            public UInt32 POS_PushRequestParam(IntPtr hLocalDevice, string param_name, string param_value) =>
                NativeMethods86.POS_PushRequestParam(hLocalDevice, param_name, param_value);

            public IntPtr POS_SetDebugLevel(IntPtr hDevice, UInt32 debugLevel) => NativeMethods86.POS_SetDebugLevel(hDevice, debugLevel);

            public IntPtr POS_SetDeviceParam(IntPtr hDevice, UInt32 paramCode, IntPtr paramValue) =>
                NativeMethods86.POS_SetDeviceParam(hDevice, paramCode, paramValue);

            public UInt32 POS_WaitForRequestCompleted(IntPtr hRequest, UInt32 timeout) => NativeMethods86.POS_WaitForRequestCompleted(hRequest, timeout);
        }
    }
}