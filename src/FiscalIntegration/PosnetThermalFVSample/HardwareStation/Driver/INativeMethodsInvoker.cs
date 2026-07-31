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
            IntPtr POS_CreateDeviceHandle(UInt32 deviceType);

            IntPtr POS_SetDebugLevel(IntPtr hDevice, UInt32 debugLevel);

            IntPtr POS_SetDeviceParam(IntPtr hDevice, UInt32 paramCode, IntPtr paramValue);

            IntPtr POS_CreateRequest(IntPtr hLocalDevice, string command);

            IntPtr POS_CreateRequestEx(IntPtr hLocalDevice, IntPtr command, IntPtr parameters);

            IntPtr POS_OpenDevice(IntPtr hDevice);

            UInt32 POS_PushRequestParam(IntPtr hLocalDevice, string param_name, string param_value);

            UInt32 POS_PostRequest(IntPtr hRequest, byte mode);

            UInt32 POS_WaitForRequestCompleted(IntPtr hRequest, UInt32 timeout);

            UInt32 POS_GetRequestStatus(IntPtr hRequest);

            UInt32 POS_DestroyRequest(IntPtr hRequest);

            UInt32 POS_CloseDevice(IntPtr hLocalDevice);

            UInt32 POS_DestroyDeviceHandle(IntPtr hDevice);

            UInt32 POS_GetError(IntPtr hLocalDevice);

            UInt32 POS_GetPrnDeviceStatus(IntPtr hLocalDevice, char statusMode, ref UInt32 globalStatus, ref UInt32 printerStatus);

            IntPtr POS_GetErrorString(UInt32 errorCode, string lang);

            UInt32 POS_GetResponseValue(IntPtr request, string paramName, StringBuilder retVal, UInt32 retLength);
        }
    }
}