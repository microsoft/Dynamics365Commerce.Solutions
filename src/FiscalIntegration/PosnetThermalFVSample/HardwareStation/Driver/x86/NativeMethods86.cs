namespace Contoso
{
    namespace Commerce.HardwareStation.PosnetThermalFVFiscalPrinterSample
    {
        using System;
        using System.Runtime.InteropServices;
        using System.Text;

        /// <summary>
        /// The class encapsulating the native POSNET methods in 32-bit dll.
        /// </summary>
        internal static class NativeMethods86
        {
            private const string DLLName = "libposcmbth.dll";

            [DllImport(DLLName)]
            extern public static IntPtr POS_CreateDeviceHandle(UInt32 deviceType);

            [DllImport(DLLName)]
            extern public static IntPtr POS_SetDebugLevel(IntPtr hDevice, UInt32 debugLevel);

            [DllImport(DLLName)]
            extern public static IntPtr POS_SetDeviceParam(IntPtr hDevice, UInt32 paramCode, IntPtr paramValue);

            [DllImport(DLLName, CharSet = CharSet.Ansi)]
            extern public static IntPtr POS_CreateRequest(IntPtr hLocalDevice, string command);

            [DllImport(DLLName)]
            extern public static IntPtr POS_CreateRequestEx(IntPtr hLocalDevice, IntPtr command, IntPtr parameters);

            [DllImport(DLLName)]
            extern public static IntPtr POS_OpenDevice(IntPtr hDevice);

            [DllImport(DLLName, CharSet = CharSet.Ansi)]
            extern public static UInt32 POS_PushRequestParam(IntPtr hLocalDevice, string param_name, string param_value);

            [DllImport(DLLName)]
            extern public static UInt32 POS_PostRequest(IntPtr hRequest, byte mode);

            [DllImport(DLLName)]
            extern public static UInt32 POS_WaitForRequestCompleted(IntPtr hRequest, UInt32 timeout);

            [DllImport(DLLName)]
            extern public static UInt32 POS_GetRequestStatus(IntPtr hRequest);

            [DllImport(DLLName)]
            extern public static UInt32 POS_DestroyRequest(IntPtr hRequest);

            [DllImport(DLLName)]
            extern public static UInt32 POS_CloseDevice(IntPtr hLocalDevice);

            [DllImport(DLLName)]
            extern public static UInt32 POS_DestroyDeviceHandle(IntPtr hDevice);

            [DllImport(DLLName)]
            extern public static UInt32 POS_GetError(IntPtr hLocalDevice);

            [DllImport(DLLName)]
            extern public static UInt32 POS_GetPrnDeviceStatus(IntPtr hLocalDevice, char statusMode, ref UInt32 globalStatus, ref UInt32 printerStatus);

            [DllImport(DLLName, CharSet = CharSet.Ansi)]
            extern public static IntPtr POS_GetErrorString(UInt32 errorCode, string lang);

            [DllImport(DLLName, CharSet = CharSet.Ansi)]
            extern public static UInt32 POS_GetResponseValue(IntPtr request, string paramName, StringBuilder retVal, UInt32 retLength);
        }
    }
}
