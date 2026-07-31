namespace Contoso
{
    namespace Commerce.HardwareStation.PosnetThermalFVFiscalPrinterSample
    {
        using System;
        using System.Collections.Generic;
        using System.Globalization;
        using System.Linq;
        using System.Reflection;
        using System.Runtime.InteropServices;
        using System.Text;
        using Contoso.Commerce.HardwareStation.PosnetThermalFVFiscalPrinterSample.DocumentModel;
        using Contoso.Commerce.HardwareStation.PosnetThermalFVFiscalPrinterSample.DocumentModel.Protocol;
        using Contoso.Commerce.HardwareStation.PosnetThermalFVFiscalPrinterSample.Driver;

        /// <summary>
        /// The Posnet driver.
        /// </summary>
        internal sealed class PosnetDriver : IDriverRequestHandler<IPosnetCommandRequest, IPosnetCommandResponse>, IDisposable
        {
            private static readonly INativeMethodsInvoker nativeMethodsInvoker = NativeMethodsInitializer.CreateNativeMethodCaller();
            private static readonly Dictionary<string, MethodInfo> nativeFunctions =
                typeof(INativeMethodsInvoker)
                .GetMethods()
                .ToDictionary(mi => mi.Name, mi => mi, StringComparer.OrdinalIgnoreCase);

            private const string DeviceMethodParameterName = "hDevice";
            private const string LocalDeviceMethodParameterName = "hLocalDevice";

            private const char ForcePollingDeviceStatusMode = '1';

            private const string Lang = "PL";

            private readonly UInt32 timeoutMilliseconds;

            private IntPtr deviceHandle;
            private IntPtr localDevice;

            /// <summary>
            /// Initializes a new instance of the <see cref="PosnetDriver"/>.
            /// </summary>
            /// <param name="timeoutMilliseconds">The printer timeout.</param>
            /// <param name="deviceHandle">The device handle.</param>
            internal PosnetDriver(UInt32 timeoutMilliseconds, IntPtr deviceHandle)
            {
                this.timeoutMilliseconds = timeoutMilliseconds;
                this.deviceHandle = deviceHandle;
            }

            /// <summary>
            /// Opens the local device.
            /// </summary>
            public void OpenLocalDevice()
            {
                if (localDevice == IntPtr.Zero)
                {
                    localDevice = nativeMethodsInvoker.POS_OpenDevice(deviceHandle);
                }
            }

            /// <summary>
            /// Closes the local device.
            /// </summary>
            public void CloseLocalDevice()
            {
                if (localDevice != IntPtr.Zero)
                {
                    nativeMethodsInvoker.POS_CloseDevice(localDevice);
                    localDevice = IntPtr.Zero;
                }
            }

            /// <summary>
            /// Closes the device handle.
            /// </summary>
            private void CloseDeviceHandle()
            {
                if (deviceHandle != IntPtr.Zero)
                {
                    nativeMethodsInvoker.POS_DestroyDeviceHandle(deviceHandle);
                    deviceHandle = IntPtr.Zero;
                }
            }

            /// <summary>
            /// Executes the command and returns the result.
            /// </summary>
            /// <param name="posnetCommand">The command to be executed.</param>
            /// <returns>The result of the execution.</returns>
            public IPosnetCommandResponse ExecuteCommand(IPosnetCommandRequest posnetCommand)
            {
                if (disposedValue)
                {
                    throw new ObjectDisposedException("The device was disposed!");
                }

                OpenLocalDevice();

                if (localDevice == IntPtr.Zero)
                {
                    throw new InvalidOperationException("The local device was not created correctly!");
                }

                switch (posnetCommand.Type)
                {
                    case CommandType.Request:
                        return ExecuteRequest(posnetCommand);
                    case CommandType.Function:
                        return ExecuteFunction(posnetCommand);
                    default:
                        throw new NotSupportedException($"The request type {posnetCommand.Type} is not supported!");
                }
            }

            /// <summary>
            /// Gets the device status.
            /// </summary>
            /// <returns>
            /// True, if device is in ready state;
            /// Otherwise, false.
            /// </returns>
            public FiscalDeviceStatus GetDeviceStatus()
            {
                if (disposedValue)
                {
                    throw new ObjectDisposedException("The device was disposed!");
                }

                OpenLocalDevice();

                if (localDevice == IntPtr.Zero)
                {
                    throw new InvalidOperationException("The local device was not created correctly!");
                }

                UInt32 globalStatus = 0;
                UInt32 printingStatus = 0;
                nativeMethodsInvoker.POS_GetPrnDeviceStatus(localDevice, ForcePollingDeviceStatusMode, ref globalStatus, ref printingStatus);

                return new FiscalDeviceStatus(globalStatus, printingStatus);
            }

            /// <summary>
            /// Executes the request and returns the result.
            /// </summary>
            /// <param name="request">The request to be executed.</param>
            /// <returns>The result of the execution.</returns>
            private IPosnetCommandResponse ExecuteRequest(IPosnetCommandRequest request)
            {
                IntPtr requestHandler = IntPtr.Zero;

                try
                {
                    requestHandler = nativeMethodsInvoker.POS_CreateRequest(localDevice, request.CommandName);

                    if (requestHandler == IntPtr.Zero)
                    {
                        ulong returnCode = nativeMethodsInvoker.POS_GetError(localDevice);
                        return new PosnetCommandResponse(request.CommandName, returnCode, string.Empty);
                    }

                    foreach (var commandParameter in request.CommandParameters)
                    {
                        if (commandParameter.Value.ValueType != DataType.Alphanum)
                        {
                            throw new ArgumentException($"The {nameof(commandParameter)} has type {commandParameter.Value.ValueType}, when Alphanum type was expected");
                        }

                        nativeMethodsInvoker.POS_PushRequestParam(requestHandler, commandParameter.Name, (string)commandParameter.Value.Value);
                    }

                    nativeMethodsInvoker.POS_PostRequest(requestHandler, DeviceConstants.POSNET_REQMODE_SPOOL);

                    nativeMethodsInvoker.POS_WaitForRequestCompleted(requestHandler, timeoutMilliseconds);

                    ulong result = nativeMethodsInvoker.POS_GetRequestStatus(requestHandler);
                    if (result == DeviceConstants.POSNET_STATUS_OK)
                    {
                        var responseParameters = request.ResponseParameters.Select(rp => GetResponseParameter(requestHandler, rp.Key, rp.Value)).ToList();
                        return new PosnetCommandResponse(request.CommandName, result, responseParameters);
                    }
                    else
                    {
                        IntPtr errorMessageUnmanaged = nativeMethodsInvoker.POS_GetErrorString((uint)result, Lang);
                        string errorMessage = Marshal.PtrToStringAnsi(errorMessageUnmanaged) ?? string.Empty;
                        return new PosnetCommandResponse(request.CommandName, result, errorMessage);
                    }
                }
                finally
                {
                    if (requestHandler != IntPtr.Zero)
                    {
                        nativeMethodsInvoker.POS_DestroyRequest(requestHandler);
                    }
                }
            }

            /// <summary>
            /// Executes the function and returns the result.
            /// </summary>
            /// <param name="posnetCommand">The request to be executed.</param>
            /// <returns>The result of the execution.</returns>
            private IPosnetCommandResponse ExecuteFunction(IPosnetCommandRequest posnetCommand)
            {
                var method = nativeFunctions[posnetCommand.CommandName];
                var methodParameters = method.GetParameters();
                var parameterValues = new object[methodParameters.Length];

                if (methodParameters.Length > 0)
                {
                    SetHandleParameters(methodParameters, parameterValues);
                }

                for (int i = 0; i < methodParameters.Length; ++i)
                {
                    if (parameterValues[i] == null)
                    {
                        var posnetCommandParameter = posnetCommand.CommandParameters.First(cp => cp.Name.Equals(methodParameters[i].Name));
                        var parameterType = methodParameters[i].ParameterType;
                        parameterValues[i] = Convert.ChangeType(posnetCommandParameter.Value.Value, parameterType);
                    }
                }

                var result = method.Invoke(nativeMethodsInvoker, parameterValues) ?? new object();
                var resultParameterValue = new CommandParameterValue(result.ToString(), DataType.Alphanum);
                var resultParameter = new CommandParameter(string.Empty, resultParameterValue);

                return new PosnetCommandResponse(posnetCommand.CommandName, 0, new CommandParameter[] { resultParameter });
            }

            /// <summary>
            /// Sets deviceHandle and localDevice parameters in the function parameter array.
            /// </summary>
            /// <param name="methodParameters">The array of <see cref="ParameterInfo"/> describing method parameters.</param>
            /// <param name="invokeParameterValues">The array of the corresponding parameter values.</param>
            private void SetHandleParameters(ParameterInfo[] methodParameters, object[] invokeParameterValues)
            {
                int index = Array.FindIndex(methodParameters, mp => mp.Name.Equals(DeviceMethodParameterName, StringComparison.OrdinalIgnoreCase));
                if (index != -1)
                {
                    invokeParameterValues[index] = deviceHandle;
                }

                index = Array.FindIndex(methodParameters, mp => mp.Name.Equals(LocalDeviceMethodParameterName, StringComparison.OrdinalIgnoreCase));
                if (index != -1)
                {
                    invokeParameterValues[index] = localDevice;
                }
            }

            /// <summary>
            /// Gets the response parameter.
            /// </summary>
            /// <param name="request">The request handle.</param>
            /// <param name="paramName">The parameter name.</param>
            /// <param name="dataType">The parameter type.</param>
            /// <returns>The response parameter.</returns>
            private CommandParameter GetResponseParameter(IntPtr request, string paramName, DataType dataType)
            {
                const int maxBufferSize = 1048576;

                UInt32 bufferSize = 512;
                StringBuilder buffer = new StringBuilder((int)bufferSize * 2);
                UInt32 resultCode = 0;
                do
                {
                    bufferSize *= 2; // Trying to allocate enough memory for the error message, starting with 1 KB and ending with 1 MB.
                    buffer.Clear();
                    buffer.EnsureCapacity((int)bufferSize);
                    resultCode = nativeMethodsInvoker.POS_GetResponseValue(request, paramName, buffer, bufferSize);
                }
                while ((resultCode == DeviceConstants.POSNET_STATUS_BUFFERTOOSHORT) && (bufferSize < maxBufferSize));

                string resultString = (resultCode != DeviceConstants.POSNET_STATUS_BUFFERTOOSHORT) ? buffer.ToString().TrimEnd() : string.Empty;
                CommandParameterValue result = null;
                switch (dataType)
                {
                    case DataType.Num:
                        result =  PosnetParser.ParseNum(resultString);
                        break;
                    case DataType.Amount:
                        result = PosnetParser.ParseAmount(resultString);
                        break;
                    case DataType.Alphanum:
                        result = PosnetParser.ParseAlphanum(resultString);
                        break;
                    case DataType.Date:
                        result = PosnetParser.ParseDate(resultString);
                        break;
                    case DataType.DateTime:
                        result = PosnetParser.ParseDateTime(resultString);
                        break;
                    case DataType.Boolean:
                        result = PosnetParser.ParseBoolean(resultString);
                        break;
                    default:
                        throw new NotSupportedException("This data type is not supported!");
                }

                return new CommandParameter(paramName, result);
            }

            private bool disposedValue = false;

            /// <summary>
            /// Releases all allocated unamanaged resources from the <see cref="PosnetDriver"/>.
            /// </summary>
            private void DisposeUnmanaged()
            {
                if (!disposedValue) // A pattern to avoid unnecessary locking, if possible.
                {
                    if (!disposedValue)
                    {
                        this.CloseLocalDevice();
                        this.CloseDeviceHandle();

                        disposedValue = true;
                    }
                }
            }

            /// <summary>
            /// Finalizes the <see cref="PosnetDriver"/> and releases all allocated unamanaged resources.
            /// </summary>
            ~PosnetDriver()
            {
                DisposeUnmanaged();
            }

            /// <summary>
            /// Disposes the <see cref="PosnetDriver"/> and releases all allocated unamanaged resources.
            /// </summary>
            public void Dispose()
            {
                this.DisposeUnmanaged();
                GC.SuppressFinalize(this);
            }
        }
    }
}
