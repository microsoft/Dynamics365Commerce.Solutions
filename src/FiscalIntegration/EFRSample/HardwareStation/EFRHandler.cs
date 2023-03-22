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
    namespace Commerce.HardwareStation.EFRSample
    {
        using System;
        using System.Collections.Generic;
        using System.Net.Http;
        using System.Threading;
        using System.Threading.Tasks;
        using Contoso.Commerce.HardwareStation.EFRSample.Constants;
        using Microsoft.Dynamics.Commerce.HardwareStation;
        using Microsoft.Dynamics.Commerce.HardwareStation.PeripheralRequests;
        using Microsoft.Dynamics.Commerce.HardwareStation.Peripherals.Entities;
        using Microsoft.Dynamics.Commerce.Runtime.Handlers;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;

        /// <summary>
        /// Handler for EFSTA (European Fiscal Standards Association) Fiscal Register.
        /// </summary>
        public class EFRHandler : INamedRequestHandlerAsync
        {
            public readonly static HttpClient httpClient = new HttpClient(new HttpClientHandler());

            /// <summary>
            /// Gets name of the handler.
            /// </summary>
            public string HandlerName => "EFRSample";

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
            /// Represents an enty point for the request handler.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <returns>The response.</returns>
            public Task<Response> Execute(Request request)
            {
                ThrowIf.Null(request, nameof(request));

                switch (request)
                {
                    case InitializeFiscalDeviceRequest initializeFiscalDeviceRequest:
                        return Task.FromResult(Initialize(initializeFiscalDeviceRequest));
                    case SubmitDocumentFiscalDeviceRequest submitDocumentFiscalDeviceRequest:
                        return SubmitDocument(submitDocumentFiscalDeviceRequest);
                    case IsReadyFiscalDeviceRequest isReadyFiscalDeviceRequest:
                        return IsReady(isReadyFiscalDeviceRequest);
                    default:
                        throw new NotSupportedException(string.Format("Request '{0}' is not supported.", request.GetType()));
                }
            }

            /// <summary>
            /// Initializes service.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <returns>The response.</returns>
            private Response Initialize(InitializeFiscalDeviceRequest request)
            {
                ThrowIf.Null(request, nameof(request));

                InitializeFiscalDeviceResponse response;

                response = new InitializeFiscalDeviceResponse(response: string.Empty, communicationResultType: FiscalPeripheralCommunicationResultType.None, failureDetails: new FiscalPeripheralFailureDetails(), fiscalPeripheralInfo: string.Empty);

                return response;
            }

            /// <summary>
            /// Submits the document.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <returns>The response.</returns>
            private async Task<Response> SubmitDocument(SubmitDocumentFiscalDeviceRequest request)
            {
                ThrowIf.NullOrWhiteSpace(request.Document, nameof(request.Document));
                ThrowIf.Null(request.PeripheralInfo, nameof(request.PeripheralInfo));
                ThrowIf.Null(request.PeripheralInfo.DeviceName, nameof(request.PeripheralInfo.DeviceName));
                ThrowIf.Null(request.PeripheralInfo.DeviceProperties, nameof(request.PeripheralInfo.DeviceName));

                SubmitDocumentFiscalDeviceResponse response;

                try
                {
                    using (var timeoutCancellationTokenSource = new CancellationTokenSource())
                    {
                        var timeoutValue = ConfigurationController.GetTimeoutValue(request.PeripheralInfo.DeviceProperties);
                        var includeUserNotificationMessage = ConfigurationController.GetShowUserNotificationMessage(request.PeripheralInfo.DeviceProperties);

                        var task = TransactionRegistrationController.RegisterAsync(
                            request.Document,
                            ConfigurationController.GetEndpointAddress(request.PeripheralInfo.DeviceProperties), 
                            timeoutCancellationTokenSource.Token
                        );

                        timeoutValue = timeoutValue > 0 ? timeoutValue : Timeout.Infinite;
                        Task firstCompletedTask = await Task.WhenAny(task, Task.Delay(timeoutValue)).ConfigureAwait(false);

                        if (firstCompletedTask == task)
                        {
                            string responseFromService = await task.ConfigureAwait(false);
                            response = FiscalDeviceResponseParser.ParseSubmitDocumentFiscalDeviceResponse(responseFromService, includeUserNotificationMessage);
                        }
                        else
                        {
                            timeoutCancellationTokenSource.Cancel();
                            FiscalPeripheralFailureDetails failureDetails = new FiscalPeripheralFailureDetails
                            {
                                ErrorMessage = "The operation has timed out.",
                                IsRetryAllowed = true,
                                FailureType = FiscalPeripheralFailureType.Timeout
                            };
                            response = new SubmitDocumentFiscalDeviceResponse(response: string.Empty,
                                                                              communicationResultType: FiscalPeripheralCommunicationResultType.Failed,
                                                                              failureDetails: failureDetails,
                                                                              fiscalPeripheralInfo: string.Empty);
                        }
                    }
                }
                catch (HttpRequestException ex)
                {
                    FiscalPeripheralFailureDetails failureDetails = new FiscalPeripheralFailureDetails
                    {
                        ErrorMessage = ex.InnerException.Message,
                        IsRetryAllowed = true,
                        FailureType = FiscalPeripheralFailureType.NotAvailable
                    };
                    response = new SubmitDocumentFiscalDeviceResponse(response: string.Empty,
                                                                      communicationResultType: FiscalPeripheralCommunicationResultType.Failed,
                                                                      failureDetails: failureDetails,
                                                                      fiscalPeripheralInfo: string.Empty);
                }
                return response;
            }

            /// <summary>
            /// Checks if printer is available.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <returns>The response.</returns>
            private async Task<Response> IsReady(IsReadyFiscalDeviceRequest request)
            {
                ThrowIf.Null(request.PeripheralInfo, $"{nameof(request)}.{nameof(request.PeripheralInfo)}");
                ThrowIf.NullOrWhiteSpace(request.PeripheralInfo.DeviceName, $"{nameof(request)}.{nameof(request.PeripheralInfo)}.{nameof(request.PeripheralInfo.DeviceName)}");
                ThrowIf.NullOrWhiteSpace(request.PeripheralInfo.DeviceProperties, $"{nameof(request)}.{nameof(request.PeripheralInfo)}.{nameof(request.PeripheralInfo.DeviceProperties)}");

                var response = new IsReadyFiscalDeviceResponse(false);
                
                try
                {
                    var timeoutValue = ConfigurationController.GetTimeoutValue(request.PeripheralInfo.DeviceProperties);
                    var endpointAddress = ConfigurationController.GetEndpointAddress(request.PeripheralInfo.DeviceProperties);
                    var task = httpClient.GetAsync(endpointAddress + "/" + RequestConstants.State);

                    timeoutValue = timeoutValue > 0 ? timeoutValue : Timeout.Infinite;
                    Task firstCompletedTask = await Task.WhenAny(task, Task.Delay(timeoutValue)).ConfigureAwait(false);

                    if (firstCompletedTask == task)
                    {
                        var httpResponse = await task.ConfigureAwait(false);

                        response = new IsReadyFiscalDeviceResponse(httpResponse.StatusCode == System.Net.HttpStatusCode.OK);
                    }
                }
                catch (Exception)
                {
                }

                return response;
            }
        }
    }
}