namespace Contoso
{
    namespace Commerce.HardwareStation.PosnetThermalFVFiscalPrinterSample
    {
        using System;
        using System.Collections.Generic;
        using System.Linq;
        using System.Text;
        using Contoso.Commerce.HardwareStation.PosnetThermalFVFiscalPrinterSample.DocumentModel;
        using Contoso.Commerce.HardwareStation.PosnetThermalFVFiscalPrinterSample.DocumentModel.Protocol;
        using Contoso.Commerce.HardwareStation.PosnetThermalFVFiscalPrinterSample.Driver;
        using Microsoft.Dynamics.Commerce.HardwareStation;
        using Microsoft.Dynamics.Commerce.HardwareStation.PeripheralRequests;
        using Microsoft.Dynamics.Commerce.HardwareStation.Peripherals.Entities;
        using Microsoft.Dynamics.Commerce.Runtime.Handlers;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;
        using Newtonsoft.Json;

        /// <summary>
        /// The fiscal printer manager.
        /// </summary>
        public class FiscalPrinterHandler : INamedRequestHandler
        {
            private const string InitializerTransactionCommandName = "trinit";
            private const string TransactionPaymentCommandName = "trpayment";

            private static PosnetDriver posnetDriver;
            private static readonly object syncObject = new object();

            /// <summary>
            /// Gets name of the handler.
            /// </summary>
            public string HandlerName => "PosnetThermalFVEJ";

            /// <summary>
            /// Gets the supported request types.
            /// </summary>
            public IEnumerable<Type> SupportedRequestTypes => new[]
            {
                typeof(IsReadyFiscalDeviceRequest),
                typeof(SubmitDocumentFiscalDeviceRequest),
                typeof(InitializeFiscalDeviceRequest)
            };

            /// <summary>
            /// Represents an enty point for the request handler.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <returns>The response.</returns>
            public Response Execute(Request request)
            {
                ThrowIf.Null(request, nameof(request));

                Type requestType = request.GetType();

                if (requestType == typeof(SubmitDocumentFiscalDeviceRequest))
                {
                    SubmitDocumentFiscalDeviceRequest submitDocumentRequest = (SubmitDocumentFiscalDeviceRequest)request;
                    return this.SubmitDocument(submitDocumentRequest);
                }
                else if (requestType == typeof(IsReadyFiscalDeviceRequest))
                {
                    IsReadyFiscalDeviceRequest isReadyRequest = (IsReadyFiscalDeviceRequest)request;
                    return this.IsReady(isReadyRequest);
                }
                else if (requestType == typeof(InitializeFiscalDeviceRequest))
                {
                    InitializeFiscalDeviceRequest initializeRequest = (InitializeFiscalDeviceRequest)request;
                    return this.Initialize(initializeRequest);
                }
                else
                {
                    throw new NotSupportedException($"Request '{requestType}' is not supported.");
                }
            }

            /// <summary>
            /// Initializes printer.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <returns>The response.</returns>
            private Response Initialize(InitializeFiscalDeviceRequest request)
            {
                ThrowIf.Null(request.Document, nameof(request.Document));
                ThrowIf.Null(request.PeripheralInfo, nameof(request.PeripheralInfo));
                ThrowIf.Null(request.PeripheralInfo.DeviceName, nameof(request.PeripheralInfo.DeviceName));
                ThrowIf.Null(request.PeripheralInfo.DeviceProperties, nameof(request.PeripheralInfo.DeviceProperties));

                InitializeFiscalDeviceResponse response = null;
                try
                {
                    var configuration = new ConfigurationController().ParseConfiguration(request.PeripheralInfo.DeviceProperties);
                    if (configuration.PrinterTimeoutMilliseconds <= 0)
                    {
                        throw new ArgumentException($"The {nameof(configuration.PrinterTimeoutMilliseconds)} is not within the correct value range!");
                    }

                    string connectionString = configuration.ConnectionString;
                    ThrowIf.NullOrWhiteSpace(connectionString, nameof(connectionString));
                    UInt32 printerTimeoutMilliseconds = (UInt32)configuration.PrinterTimeoutMilliseconds;

                    IPosnetCommandResponse synchronizationResponse = null;
                    lock (syncObject)
                    {
                        posnetDriver?.Dispose();
                        posnetDriver = new SerialConnectionDriverInitializer(printerTimeoutMilliseconds, connectionString).Create();

                        if (configuration.SynchronizeDateTime)
                        {
                            synchronizationResponse = this.SynchronizeDateTime();
                        }
                    }

                    if (synchronizationResponse == null || synchronizationResponse.Success)
                    {
                        response = new InitializeFiscalDeviceResponse("success", FiscalPeripheralCommunicationResultType.Succeeded, new FiscalPeripheralFailureDetails(), string.Empty);
                    }
                    else
                    {
                        FiscalDeviceStatus deviceStatusData = this.GetDeviceStatus();
                        var errorData = FiscalPrinterHelper.GetErrorData(synchronizationResponse.Code, deviceStatusData);
                        response = new InitializeFiscalDeviceResponse("failure",
                            FiscalPeripheralCommunicationResultType.Failed,
                            new FiscalPeripheralFailureDetails()
                            {
                                ErrorCode = synchronizationResponse.Code.ToString(),
                                ErrorMessage = errorData.ErrorMessage,
                                IsRetryAllowed = errorData.IsRetryAllowed,
                                FailureType = errorData.FailureType
                            },
                            string.Empty);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);

                    response = new InitializeFiscalDeviceResponse("failure",
                        FiscalPeripheralCommunicationResultType.Failed,
                        new FiscalPeripheralFailureDetails()
                        {
                            ErrorCode = string.Empty,
                            ErrorMessage = ex.Message,
                            IsRetryAllowed = false,
                            FailureType = FiscalPeripheralFailureType.Other
                        },
                        string.Empty);
                }
                return response;
            }

            /// <summary>
            /// Submits document to printer.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <returns>The response.</returns>
            private Response SubmitDocument(SubmitDocumentFiscalDeviceRequest request)
            {
                IEnumerable<IPosnetCommandResponse> responses = null;
                try
                {
                    ThrowIf.NullOrWhiteSpace(request.Document, nameof(request.Document));
                    ThrowIf.Null(request.PeripheralInfo, nameof(request.PeripheralInfo));
                    ThrowIf.Null(request.PeripheralInfo.DeviceName, nameof(request.PeripheralInfo.DeviceName));
                    ThrowIf.Null(request.PeripheralInfo.DeviceProperties, nameof(request.PeripheralInfo.DeviceName));

                    var posnetRequest = DeserializeFromJson<PosnetDocumentRequest>(request.Document);
                    responses = ProcessDocument(posnetRequest);
                    bool success = responses.All(r => r.Success);

                    if (success)
                    {
                        var fiscalDocumentResponse = new PosnetDocumentResponse(true, responses);
                        return new SubmitDocumentFiscalDeviceResponse(
                            SerializeToJson(fiscalDocumentResponse),
                            FiscalPeripheralCommunicationResultType.Succeeded,
                            new FiscalPeripheralFailureDetails(),
                            string.Empty);
                    }
                    else
                    {
                        PosnetDocumentResponse fiscalDocumentResponse = new PosnetDocumentResponse(false, responses);
                        IPosnetCommandResponse firstError = responses.First(r => !r.Success);
                        FiscalDeviceStatus deviceStatusData = this.GetDeviceStatus();

                        ErrorData errorData = FiscalPrinterHelper.GetErrorData(firstError.Code, deviceStatusData);
                        return new SubmitDocumentFiscalDeviceResponse(
                            SerializeToJson(fiscalDocumentResponse),
                            FiscalPeripheralCommunicationResultType.Failed,
                            new FiscalPeripheralFailureDetails()
                            {
                                ErrorCode = firstError.Code.ToString(),
                                ErrorMessage = errorData.ErrorMessage,
                                IsRetryAllowed = errorData.IsRetryAllowed,
                                FailureType = errorData.FailureType
                            },
                            string.Empty);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);

                    string responseString = (responses != null) ?
                        SerializeToJson(new PosnetDocumentResponse(false, responses))
                        : "error";

                    return new SubmitDocumentFiscalDeviceResponse(responseString,
                            FiscalPeripheralCommunicationResultType.Failed,
                            new FiscalPeripheralFailureDetails()
                            {
                                ErrorCode = string.Empty,
                                ErrorMessage = ex.Message,
                                IsRetryAllowed = false,
                                FailureType = FiscalPeripheralFailureType.Other
                            },
                            string.Empty);
                }
            }

            /// <summary>
            /// Processes the document.
            /// </summary>
            /// <param name="posnetRequest">Posnet document request.</param>
            /// <returns>Collection of responses to the individual commands.</returns>
            private static IEnumerable<IPosnetCommandResponse> ProcessDocument(PosnetDocumentRequest posnetRequest)
            {
                ThrowIf.Null(posnetRequest?.Commands, nameof(posnetRequest));

                var responsesList = new List<IPosnetCommandResponse>(posnetRequest.Commands.Count());
                lock (syncObject)
                {
                    try
                    {
                        foreach (var posnetCommand in posnetRequest.Commands)
                        {
                            var posnetResponse = posnetDriver.ExecuteCommand(posnetCommand);
                            responsesList.Add(posnetResponse);

                            if (!posnetResponse.Success)
                            {
                                CancelDocument(responsesList);
                                break;
                            }
                        }

                        return responsesList;
                    }
                    catch (Exception)
                    {
                        CancelDocument(responsesList);
                        throw;
                    }
                }
            }

            /// <summary>
            /// Checks if printer is available.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <returns>The response.</returns>
            private Response IsReady(IsReadyFiscalDeviceRequest request)
            {
                ThrowIf.Null(request.PeripheralInfo, nameof(request.PeripheralInfo));
                ThrowIf.NullOrWhiteSpace(request.PeripheralInfo.DeviceName, nameof(request.PeripheralInfo.DeviceName));
                ThrowIf.NullOrWhiteSpace(request.PeripheralInfo.DeviceProperties, nameof(request.PeripheralInfo.DeviceProperties));

                IsReadyFiscalDeviceResponse response = null;
                try
                {
                    bool result = this.GetDeviceStatus().IsDeviceReady();
                    response = new IsReadyFiscalDeviceResponse(result);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    response = new IsReadyFiscalDeviceResponse(false);
                }

                return response;
            }

            /// <summary>
            /// Synchronizes date and time on printer with the local date and time.
            /// </summary>
            /// <returns><see cref="IPosnetCommandResponse"/> containing the synchronization result.</returns>
            private IPosnetCommandResponse SynchronizeDateTime()
            {
                var request = FiscalPrinterHelper.CreateSynchronizeDateTimeRequest();
                return ProcessDocument(request).Single();
            }

            /// <summary>
            /// Retrieves the fiscal device status.
            /// </summary>
            /// <returns><see cref="FiscalDeviceStatus"/> containing the device status data.</returns>
            private FiscalDeviceStatus GetDeviceStatus()
            {
                lock (syncObject)
                {
                    return posnetDriver.GetDeviceStatus();
                }
            }

            /// <summary>
            /// Cancels the failed document.
            /// </summary>
            /// <param name="responsesList">The response list of the document.</param>
            private static void CancelDocument(List<IPosnetCommandResponse> responsesList)
            {
                try
                {
                    for (int i = responsesList.Count - 1; i >= 0; --i)
                    {
                        CancelRequest(responsesList[i]);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            /// <summary>
            /// Cancels the failed posnet command if necessary.
            /// </summary>
            /// <param name="posnetCommandResponse">Response to the command to be cancelled.</param>
            private static void CancelRequest(IPosnetCommandResponse posnetCommandResponse)
            {
                if (!posnetCommandResponse.Success)
                {
                    return;
                }

                IPosnetCommandRequest posnetCommand = null;
                switch (posnetCommandResponse.RequestCommandName)
                {
                    case TransactionPaymentCommandName:
                        posnetCommand = FiscalPrinterHelper.CreateCancelPaymentCommandRequest();
                        break;
                    case InitializerTransactionCommandName:
                        posnetCommand = FiscalPrinterHelper.CreateCancelTransactionCommandRequest();
                        break;
                    default:
                        return;
                }

                IPosnetCommandResponse response = null;
                response = posnetDriver.ExecuteCommand(posnetCommand);
            }

            /// <summary>
            /// Serializes the object to JSON.
            /// </summary>
            /// <typeparam name="T">Type of the object to be serialized.</typeparam>
            /// <param name="value">Value of the object to be serialized.</param>
            /// <returns>Object as JSON string.</returns>
            private static string SerializeToJson<T>(T value)
            {
                var jsonString = JsonConvert.SerializeObject(value, Formatting.Indented);
                return Encoding.UTF8.GetString(Encoding.Default.GetBytes(jsonString));
            }

            /// <summary>
            /// Deserialized the object from JSON format.
            /// </summary>
            /// <typeparam name="T">Type of the object to be deserialized.</typeparam>
            /// <param name="value">JSON string containing object data.</param>
            /// <returns>The deserialized object.</returns>
            private static T DeserializeFromJson<T>(string value)
            {
                ThrowIf.NullOrWhiteSpace(value, nameof(value));

                return JsonConvert.DeserializeObject<T>(value, new JsonSerializerSettings() { Formatting = Formatting.Indented });
            }
        }
    }
}

