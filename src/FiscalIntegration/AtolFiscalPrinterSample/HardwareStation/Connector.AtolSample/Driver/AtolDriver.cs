/**
 * SAMPLE CODE NOTICE
 * 
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

namespace Contoso
{
    namespace HardwareStation.Connector.AtolSample.Driver
    {
        using System;
        using Microsoft.Dynamics.Commerce.HardwareStation.Peripherals.Entities;

        /// <summary>
        /// The Atol driver.
        /// </summary>
        public class AtolDriver : IDisposable
        {
            private static readonly INativeMethodsInvoker nativeMethodsInvoker = NativeMethodsInitializer.CreateNativeMethodCaller();
            private bool disposedValue = false;

            /// <summary>
            /// Initializes a new instance of the <see cref="AtolDriver"/>.
            /// </summary>
            /// <param name="comPortNumber">The printer COM port.</param>
            /// <param name="baudRate">The printer baud rate.</param>
            public AtolDriver(string comPortNumber, string baudRate)
            {
                this.InitializeDriver(comPortNumber, baudRate);
            }

            /// <summary>
            /// Finalizes the <see cref="AtolDriver"/> and releases all allocated unamanaged resources.
            /// </summary>
            ~AtolDriver()
            {
                DisposeUnmanaged();
            }

            /// <summary>
            /// Disposes the <see cref="AtolDriver"/> and releases all allocated unamanaged resources.
            /// </summary>
            public void Dispose()
            {
                this.DisposeUnmanaged();
            }

            /// <summary>
            /// Processes document on the fiscal printer.
            /// </summary>
            /// <param name="document">Document to process.</param>
            /// <param name="printerResponse">Responce from the fiscal printer.</param>
            /// <param name="failureDetails">Detail about the failure</param>
            /// <returns>True - if document was processed, false - if processing failed.</returns>
            internal bool TryProcessDocument(string document, out string printerResponse, out FiscalPeripheralFailureDetails failureDetails, out string userNotificationMessage)
            {
                bool retVal = false;
                failureDetails = new FiscalPeripheralFailureDetails() { FailureType = FiscalPeripheralFailureType.None };
                userNotificationMessage = string.Empty;
                printerResponse = string.Empty;
                if (!nativeMethodsInvoker.IsOpened())
                {
                    nativeMethodsInvoker.Open();
                }

                if (nativeMethodsInvoker.IsOpened())
                {
                    nativeMethodsInvoker.SetParam(DriverConstants.PARAM_JSON_DATA, document);
                    nativeMethodsInvoker.ProcessJson();

                    printerResponse = nativeMethodsInvoker.GetParamString(DriverConstants.PARAM_JSON_DATA);
                    int errorCode = nativeMethodsInvoker.ErrorCode();
                    if (errorCode != 0)
                    {
                        failureDetails.ErrorCode = errorCode.ToString();
                        failureDetails.ErrorMessage = nativeMethodsInvoker.ErrorDescription();
                        failureDetails.FailureType = ErrorCodeToFailureType(errorCode);
                        failureDetails.IsRetryAllowed = true;
                        userNotificationMessage = nativeMethodsInvoker.ErrorDescription();
                    }
                    else
                    {
                        retVal = true;
                    }
                }
                else
                {
                    failureDetails = new FiscalPeripheralFailureDetails()
                    {
                        ErrorCode = nativeMethodsInvoker.ErrorCode().ToString(),
                        ErrorMessage = nativeMethodsInvoker.ErrorDescription(),
                        FailureType = ErrorCodeToFailureType(nativeMethodsInvoker.ErrorCode()),
                        IsRetryAllowed = true
                    };
                }

                return retVal;
            }

            /// <summary>
            /// Initializes printer driver.
            /// </summary>
            /// <param name="comPortNumber">The printer COM port.</param>
            /// <param name="baudRate">The printer baud rate.</param>
            private void InitializeDriver(string comPort, string baudRate)
            {
                nativeMethodsInvoker.SetSingleSetting(DriverConstants.SETTING_MODEL, DriverConstants.MODEL_ATOL_AUTO.ToString());
                nativeMethodsInvoker.SetSingleSetting(DriverConstants.SETTING_PORT, DriverConstants.PORT_COM.ToString());
                nativeMethodsInvoker.SetSingleSetting(DriverConstants.SETTING_COM_FILE, comPort);
                nativeMethodsInvoker.SetSingleSetting(DriverConstants.SETTING_BAUDRATE, baudRate);
                nativeMethodsInvoker.ApplySingleSettings();
                nativeMethodsInvoker.InitDevice();
                nativeMethodsInvoker.Open();
            }

            /// <summary>
            /// Converts printer error code to failure type.
            /// </summary>
            /// <param name="errorCode">Error code returned by the printer.</param>
            /// <returns>Returns failure type.</returns>
            private FiscalPeripheralFailureType ErrorCodeToFailureType(int errorCode)
            {
                switch(errorCode)
                {
                    case 0:
                        return FiscalPeripheralFailureType.None;
                    case 1:
                    case 2:
                    case 4:
                        return FiscalPeripheralFailureType.NotAvailable;
                    case 3:
                        return FiscalPeripheralFailureType.Busy;
                    case 44:
                    case 45:
                        return FiscalPeripheralFailureType.PaperOut;
                    default:
                        return FiscalPeripheralFailureType.SubmissionFailed;
                }
}

            /// <summary>
            /// Releases all allocated unamanaged resources from the <see cref="AtolDriver"/>.
            /// </summary>
            private void DisposeUnmanaged()
            {
                if (!disposedValue) // A pattern to avoid unnecessary locking, if possible.
                {
                    if (!disposedValue)
                    {
                        nativeMethodsInvoker.Close();
                        nativeMethodsInvoker.Destroy();

                        disposedValue = true;
                    }
                }
            }
        }
    }
}