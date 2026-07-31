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
    namespace HardwareStation.Connector.CleanCashSample
    {
        using System;
        using System.Collections.Generic;
        using Microsoft.Dynamics.Commerce.HardwareStation.PeripheralRequests;
        using Microsoft.Dynamics.Commerce.HardwareStation.Peripherals.Entities;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.Handlers;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;
        using Newtonsoft.Json;

        /// <summary>
        /// Fiscal peripheral device handler for CleanCash.
        /// </summary>
#pragma warning disable CS0618 // Type or member is obsolete. JUSTIFICATION: TODO: transform to asynchronous handler
        public class CleanCashHandler : INamedRequestHandler
#pragma warning restore CS0618 // Type or member is obsolete
        {
            /// <summary>
            /// Gets name of the handler.
            /// </summary>
            public string HandlerName => "CleanCashSample";

            /// <summary>
            /// Gets supported types.
            /// </summary>
            public IEnumerable<Type> SupportedRequestTypes => new[]
            {
                typeof(InitializeFiscalDeviceRequest),
                typeof(SubmitDocumentFiscalDeviceRequest),
                typeof(IsReadyFiscalDeviceRequest)
            };

            /// <summary>
            /// Represents an entry point for the request handler.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <returns>The response.</returns>
            public Response Execute(Request request)
            {
                ThrowIf.Null(request, nameof(request));

                switch (request)
                {
                    case InitializeFiscalDeviceRequest initializeFiscalDeviceRequest:
                        return this.Initialize(initializeFiscalDeviceRequest);
                    case SubmitDocumentFiscalDeviceRequest submitDocumentFiscalDeviceRequest:
                        return this.SubmitDocument(submitDocumentFiscalDeviceRequest);
                    case IsReadyFiscalDeviceRequest isReadyFiscalDeviceRequest:
                        return this.IsReady(isReadyFiscalDeviceRequest);
                    default:
                        throw new NotSupportedException($"Request '{request.GetType()}' is not supported.");
                }
            }

            /// <summary>
            /// Submits the document.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <returns>The response.</returns>
            private Response SubmitDocument(SubmitDocumentFiscalDeviceRequest request)
            {
                ThrowIf.NullOrWhiteSpace(request.Document, nameof(request.Document));
                ThrowIf.Null(request.PeripheralInfo, nameof(request.PeripheralInfo));
                ThrowIf.Null(request.PeripheralInfo.DeviceName, nameof(request.PeripheralInfo.DeviceName));
                ThrowIf.Null(request.PeripheralInfo.DeviceProperties, nameof(request.PeripheralInfo.DeviceName));

                SubmitDocumentFiscalDeviceResponse response;

                try
                {
                    var fiscalTransactionData = JsonConvert.DeserializeObject<CleanCashFiscalTransactionData>(request.Document, new JsonSerializerSettings() { Formatting = Formatting.Indented });
                    var connectionString = ConfigurationController.GetConnectionStringValue(request.PeripheralInfo.DeviceProperties);
                    var timeout = ConfigurationController.GetTimeoutValue(request.PeripheralInfo.DeviceProperties);
                    CleanCashResponse registerResponse = null;
                    CleanCashLockContainer.Execute(() =>
                    {
                        using (var register = new CleanCashFiscalRegister(connectionString))
                        {
                            registerResponse = register.RegisterFiscalTransaction(fiscalTransactionData);
                        }
                    }, timeout);
                    
                    response = new SubmitDocumentFiscalDeviceResponse(registerResponse.ControlCode, FiscalPeripheralCommunicationResultType.Succeeded, new FiscalPeripheralFailureDetails(), registerResponse.DeviceId);
                }
                catch (TimeoutException)
                {
                    FiscalPeripheralFailureDetails failureDetails = new FiscalPeripheralFailureDetails
                    {
                        IsRetryAllowed = true,
                        FailureType = FiscalPeripheralFailureType.Timeout
                    };
                    response = new SubmitDocumentFiscalDeviceResponse(string.Empty, FiscalPeripheralCommunicationResultType.Failed, failureDetails, string.Empty);
                }
                catch (CleanCashDeviceException ex)
                {
                    FiscalPeripheralFailureDetails failureDetails = new FiscalPeripheralFailureDetails
                    {
                        ErrorCode = ex.ResultCode.ToString(),
                        IsRetryAllowed = true,
                        FailureType = ErrorCodesMapper.MapToFiscalPeripheralFailureType(ex.ResultCode)
                    };
                    response = new SubmitDocumentFiscalDeviceResponse(string.Empty, FiscalPeripheralCommunicationResultType.Failed, failureDetails, string.Empty);
                }
                return response;
            }

            /// <summary>
            /// Initializes service.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <returns>The response.</returns>
            private Response Initialize(InitializeFiscalDeviceRequest request)
            {
                ThrowIf.Null(request, nameof(request));
                var response = new InitializeFiscalDeviceResponse(string.Empty, FiscalPeripheralCommunicationResultType.None, new FiscalPeripheralFailureDetails(), string.Empty);
                return response;
            }

            /// <summary>
            /// Checks if printer is available.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <returns>The response.</returns>
            private Response IsReady(IsReadyFiscalDeviceRequest request)
            {
                ThrowIf.Null(request.PeripheralInfo, $"{nameof(request)}.{nameof(request.PeripheralInfo)}");
                ThrowIf.NullOrWhiteSpace(request.PeripheralInfo.DeviceName, $"{nameof(request)}.{nameof(request.PeripheralInfo)}.{nameof(request.PeripheralInfo.DeviceName)}");
                ThrowIf.NullOrWhiteSpace(request.PeripheralInfo.DeviceProperties, $"{nameof(request)}.{nameof(request.PeripheralInfo)}.{nameof(request.PeripheralInfo.DeviceProperties)}");

                var response = new IsReadyFiscalDeviceResponse(false);

                try
                {
                    var connectionString = ConfigurationController.GetConnectionStringValue(request.PeripheralInfo.DeviceProperties);
                    var timeout = ConfigurationController.GetTimeoutValue(request.PeripheralInfo.DeviceProperties);
                    bool isReady = false;
                    CleanCashLockContainer.Execute(() =>
                    {
                        using (var register = new CleanCashFiscalRegister(connectionString))
                        {
                            isReady = register.IsReady();
                        }
                    }, timeout);

                    response = new IsReadyFiscalDeviceResponse(isReady);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                return response;
            }
        }
    }
}