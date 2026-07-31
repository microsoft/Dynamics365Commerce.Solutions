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
    namespace HardwareStation.Connector.AtolSample.Handlers
    {
        using System;
        using System.Collections.Generic;
        using System.Threading.Tasks;
        using Contoso.HardwareStation.Connector.AtolSample.AtolTasks;
        using Contoso.HardwareStation.Connector.AtolSample.Driver;
        using Contoso.HardwareStation.Connector.AtolSample.Messages;
        using Microsoft.Dynamics.Commerce.HardwareStation;
        using Microsoft.Dynamics.Commerce.HardwareStation.PeripheralRequests;
        using Microsoft.Dynamics.Commerce.HardwareStation.Peripherals.Entities;
        using Microsoft.Dynamics.Commerce.Runtime.Handlers;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;

        /// <summary>
        /// Fiscal peripheral service handler for Atol fiscal printer.
        /// </summary>
        public class ConnectorAtolService : INamedRequestHandlerAsync
        {
            private static AtolDriver atolDriver;
            private static readonly object syncObject = new object();

            /// <summary>
            /// Gets name of the handler.
            /// </summary>
            public string HandlerName => "AtolSample";

            /// <summary>
            /// Gets supported types.
            /// </summary>
            public IEnumerable<Type> SupportedRequestTypes => new[]
            {
                typeof(InitializeFiscalDeviceRequest),
                typeof(IsReadyFiscalDeviceRequest),
                typeof(SubmitDocumentFiscalDeviceRequest),
            };

            /// <summary>
            /// Represents an enty point for the request handler.
            /// </summary>
            /// <param name="request">Th request.</param>
            /// <returns>The response.</returns>
            public Task<Response> Execute(Request request)
            {
                ThrowIf.Null(request, nameof(request));

                switch (request)
                {
                    case InitializeFiscalDeviceRequest initializeFiscalDeviceRequest:
                        return Task.FromResult<Response>(Initialize(initializeFiscalDeviceRequest));
                    case IsReadyFiscalDeviceRequest isReadyFiscalDeviceRequest:
                        return IsReady(isReadyFiscalDeviceRequest);
                    case SubmitDocumentFiscalDeviceRequest submitDocumentFiscalDeviceRequest:
                        return SubmitDocument(submitDocumentFiscalDeviceRequest);
                    default:
                        throw new NotSupportedException($"Request '{request.GetType()}' is not supported.");
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

                response = new InitializeFiscalDeviceResponse(string.Empty, FiscalPeripheralCommunicationResultType.None, new FiscalPeripheralFailureDetails(), string.Empty);

                return response;
            }

            /// <summary>
            /// Checks if printer is available.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <returns>The response.</returns>
            private async Task<Response> IsReady(IsReadyFiscalDeviceRequest isReadyRequest)
            {
                var getConnectorSettingsResponse = await isReadyRequest.RequestContext.Runtime.ExecuteAsync<GetConnectorConfigurationConnectorAtolResponse>(new GetConnectorConfigurationConnectorAtolRequest(isReadyRequest.PeripheralInfo), isReadyRequest.RequestContext).ConfigureAwait(false);
                var connectorSettings = getConnectorSettingsResponse.ConnectorSettings;
                atolDriver = new AtolDriver(connectorSettings.ComPort, connectorSettings.BaudRate.ToString());

                lock (syncObject)
                {
                    try
                    {
                        string printerResponse = string.Empty;
                        FiscalPeripheralFailureDetails failureDetails = null;
                        string userNotificationMessage = string.Empty;
                            string command = AtolTasksList.GetDeviceStatusTask;
                            atolDriver.TryProcessDocument(command, out printerResponse, out failureDetails, out userNotificationMessage);
                        return new IsReadyFiscalDeviceResponse(failureDetails.FailureType == FiscalPeripheralFailureType.None);
                    }
                    catch (Exception)
                    {
                        return new IsReadyFiscalDeviceResponse(false);
                    }
                }
            }

            /// <summary>
            /// Submits document to printer.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <returns>The response.</returns>
            private async Task<Response> SubmitDocument(SubmitDocumentFiscalDeviceRequest submitDocumentRequest)
            {
                var getConnectorSettingsResponse = await submitDocumentRequest.RequestContext.Runtime.ExecuteAsync<GetConnectorConfigurationConnectorAtolResponse>(new GetConnectorConfigurationConnectorAtolRequest(submitDocumentRequest.PeripheralInfo), submitDocumentRequest.RequestContext).ConfigureAwait(false);
                var connectorSettings = getConnectorSettingsResponse.ConnectorSettings;
                atolDriver = new AtolDriver(connectorSettings.ComPort,connectorSettings.BaudRate.ToString());
                lock(syncObject)
                {
                    try
                    {
                        string printerResponse = string.Empty;
                        string userNotificationMessage = string.Empty;
                        FiscalPeripheralFailureDetails failureDetails = null;
                        PrintPreviousUnprintedDocument(connectorSettings);
                        string deviceInfo = GetPrinterDetails(connectorSettings);
                        if (atolDriver.TryProcessDocument(submitDocumentRequest.Document, out printerResponse, out failureDetails, out userNotificationMessage))
                        {
                            return new SubmitDocumentFiscalDeviceResponse(printerResponse, FiscalPeripheralCommunicationResultType.Succeeded, failureDetails, deviceInfo, userNotificationMessage);
                        }
                        else
                        {
                            return new SubmitDocumentFiscalDeviceResponse(printerResponse, FiscalPeripheralCommunicationResultType.Failed, failureDetails, string.Empty, userNotificationMessage);
                        }
                    }
                    catch (Exception ex)
                    {
                        return new SubmitDocumentFiscalDeviceResponse(string.Empty,
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
            }

            /// <summary>
            /// Gets inforrmation about the fiscal pritner.
            /// </summary>
            /// <param name="connectorSettings">Settings of the connector.</param>
            /// <returns>Information about the fiscal printer.</returns>
            private string GetPrinterDetails(DataModel.Configuration.ConnectorSettings connectorSettings)
            {
                string printerResponse = string.Empty;
                FiscalPeripheralFailureDetails failureDetails = null;
                string userNotificationMessage = string.Empty;
                string command = AtolTasksList.GetDeviceInfoTask;
                atolDriver.TryProcessDocument(command, out printerResponse, out failureDetails, out userNotificationMessage);
                return printerResponse;
            }

            /// <summary>
            /// Prints previous document that wasn't printed but was saved in the fiscal memory.
            /// </summary>
            /// <param name="connectorSettings">Settings of the connector.</param>
            private static void PrintPreviousUnprintedDocument(DataModel.Configuration.ConnectorSettings connectorSettings)
            {
                string printerResponse = string.Empty;
                FiscalPeripheralFailureDetails failureDetails = null;
                string userNotificationMessage = string.Empty;
                if (connectorSettings.PrintPreviousNotPrintedDocument)
                {
                    string command = AtolTasksList.PrintPreviousDocumentTask;
                    atolDriver.TryProcessDocument(command, out printerResponse, out failureDetails, out userNotificationMessage);
                }
            }
        }
    }
}
