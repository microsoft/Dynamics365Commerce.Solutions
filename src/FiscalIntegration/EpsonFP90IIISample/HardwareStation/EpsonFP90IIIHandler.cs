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
    namespace HardwareStation.Connector.EpsonFP90IIISample
    {
        using System;
        using System.Collections.Generic;
        using System.Net.Http;
        using System.Threading.Tasks;
        using Microsoft.Dynamics.Commerce.HardwareStation;
        using Microsoft.Dynamics.Commerce.HardwareStation.PeripheralRequests;
        using Microsoft.Dynamics.Commerce.HardwareStation.Peripherals.Entities;
        using Microsoft.Dynamics.Commerce.Runtime.Handlers;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;

        /// <summary>
        /// Fiscal peripheral device handler for EPSON FP-90III.
        /// </summary>
#pragma warning disable CS0618 // Type or member is obsolete. JUSTIFICATION: TODO: transform to asynchronous handler
        public class EpsonFP90IIIHandler : INamedRequestHandler
#pragma warning restore CS0618 // Type or member is obsolete

        {
            /// <summary>
            /// Gets name of the handler.
            /// </summary>
            public string HandlerName => "EpsonFP90IIISample";

            /// <summary>
            /// Gets supported types.
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
            /// <param name="request">Th request.</param>
            /// <returns>The response.</returns>
            public Response Execute(Request request)
            {
                ThrowIf.Null(request, nameof(request));

                switch (request)
                {
                    case SubmitDocumentFiscalDeviceRequest submitDocumentFiscalDeviceRequest:
#pragma warning disable AvoidBlockingCallsAnalyzer // Avoid blocking asynchronous execution. JUSTIFICATION: remove when EpsonFP90IIIHandler will be transformed to asynchronous
                        return this.SubmitDocument(submitDocumentFiscalDeviceRequest).GetAwaiter().GetResult();
                    case IsReadyFiscalDeviceRequest isReadyFiscalDeviceRequest:
                        return this.IsReady(isReadyFiscalDeviceRequest).GetAwaiter().GetResult();
                    case InitializeFiscalDeviceRequest initializeFiscalDeviceRequest:
                        return this.Initialize(initializeFiscalDeviceRequest).GetAwaiter().GetResult();
#pragma warning restore AvoidBlockingCallsAnalyzer // Avoid blocking asynchronous execution.
                    default:
                        throw new NotSupportedException($"Request '{request.GetType()}' is not supported.");
                }
            }

            /// <summary>
            /// Initializes printer.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <returns>The response.</returns>
            private async Task<Response> Initialize(InitializeFiscalDeviceRequest request)
            {
                ThrowIf.Null(request.Document, nameof(request.Document));
                ThrowIf.Null(request.PeripheralInfo, nameof(request.PeripheralInfo));
                ThrowIf.Null(request.PeripheralInfo.DeviceName, nameof(request.PeripheralInfo.DeviceName));
                ThrowIf.Null(request.PeripheralInfo.DeviceProperties, nameof(request.PeripheralInfo.DeviceProperties));

                InitializeFiscalDeviceResponse response;

                ConfigurationModel configuration = new ConfigurationController().ParseConfiguration(request.PeripheralInfo.DeviceProperties);

                if (configuration.SynchronizeDateTime)
                {
                    string document = BuiltInDocuments.GetSetCurrentDateTimeDocument();

                    try
                    {
                        PrinterCommunicationController communicationController = new PrinterCommunicationController();
                        string fiscalPeripheralInfo = await GetFiscalPrinterFullSerialNumber(communicationController, configuration).ConfigureAwait(false);
                        string responseFromPrinter = await communicationController.SubmitDocumentAsync(document, configuration).ConfigureAwait(false);
                        FiscalDeviceResponseBase responseBase = ResponseParser.ParseResponse<SubmitDocumentFiscalDeviceResponse>(responseFromPrinter);

                        // We use the communication result type always as succeeded, because time synchronization is not necessary. 
                        response = new InitializeFiscalDeviceResponse(responseBase.Response, FiscalPeripheralCommunicationResultType.Succeeded, responseBase.FailureDetails, fiscalPeripheralInfo);
                    }
                    catch (HttpRequestException ex)
                    {
                        FiscalPeripheralFailureDetails failureDetails = new FiscalPeripheralFailureDetails();
                        failureDetails.ErrorMessage = ex.InnerException.Message;
                        failureDetails.IsRetryAllowed = false;
                        failureDetails.FailureType = FiscalPeripheralFailureType.NotAvailable;
                        response = new InitializeFiscalDeviceResponse(string.Empty, FiscalPeripheralCommunicationResultType.Failed, failureDetails, string.Empty);
                    }
                }
                else
                {
                    response = new InitializeFiscalDeviceResponse(string.Empty, FiscalPeripheralCommunicationResultType.Succeeded, new FiscalPeripheralFailureDetails(), string.Empty);
                }

                return response;
            }

            /// <summary>
            /// Submits document to printer.
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
                ConfigurationModel configuration = new ConfigurationController().ParseConfiguration(request.PeripheralInfo.DeviceProperties);

                try
                {
                    PrinterCommunicationController communicationController = new PrinterCommunicationController();
                    string fiscalPeripheralInfo = await GetFiscalPrinterFullSerialNumber(communicationController, configuration).ConfigureAwait(false);
                    string responseFromPrinter = await communicationController.SubmitDocumentAsync(request.Document, configuration).ConfigureAwait(false);
                    FiscalDeviceResponseBase responseBase = ResponseParser.ParseResponse<SubmitDocumentFiscalDeviceResponse>(responseFromPrinter);
                    response = new SubmitDocumentFiscalDeviceResponse(responseBase.Response, responseBase.CommunicationResultType, responseBase.FailureDetails, fiscalPeripheralInfo);
                }
                catch (HttpRequestException ex)
                {
                    FiscalPeripheralFailureDetails failureDetails = new FiscalPeripheralFailureDetails();
                    failureDetails.ErrorMessage = ex.InnerException.Message;
                    failureDetails.IsRetryAllowed = true;
                    failureDetails.FailureType = FiscalPeripheralFailureType.NotAvailable;
                    response = new SubmitDocumentFiscalDeviceResponse(string.Empty, FiscalPeripheralCommunicationResultType.Failed, failureDetails, string.Empty);
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
                ThrowIf.Null(request.PeripheralInfo, nameof(request.PeripheralInfo));
                ThrowIf.NullOrWhiteSpace(request.PeripheralInfo.DeviceName, nameof(request.PeripheralInfo.DeviceName));
                ThrowIf.NullOrWhiteSpace(request.PeripheralInfo.DeviceProperties, nameof(request.PeripheralInfo.DeviceProperties));

                IsReadyFiscalDeviceResponse response = new IsReadyFiscalDeviceResponse(false);
                ConfigurationModel configuration = new ConfigurationController().ParseConfiguration(request.PeripheralInfo.DeviceProperties);

                string document = BuiltInDocuments.GetCheckStatusDocument();

                try
                {
                    PrinterCommunicationController communicationController = new PrinterCommunicationController();
                    string responseFromPrinter = await communicationController.SubmitDocumentAsync(document, configuration).ConfigureAwait(false);
                    FiscalDeviceResponseBase deviceResponseBase = ResponseParser.ParseResponse<FiscalDeviceResponseBase>(responseFromPrinter);
                    response = new IsReadyFiscalDeviceResponse(deviceResponseBase.CommunicationResultType == FiscalPeripheralCommunicationResultType.Succeeded);
                }
                catch (HttpRequestException)
                {
                }

                return response;
            }

            /// <summary>
            /// Gets full serial number of the fiscal printer  in the format that could be used in the refund document.
            /// </summary>
            /// <param name="document">The document to submit.</param>
            /// <param name="configuration">The printers configuration.</param>
            /// <returns>The fiscal printer serial number.</returns>
            private async Task<string> GetFiscalPrinterFullSerialNumber(PrinterCommunicationController communicationController, ConfigurationModel configuration)
            {
                string printerFullSerialNumber = string.Empty;

                // We should optionally try to read printer serial number.
                // If the exception was reaised during reading of the serial number, we should suppress it
                // to avoid interruption of the document submission to the printer.
                try
                {
                    string responseFromPrinter = await communicationController.SubmitDocumentAsync(BuiltInDocuments.GetRTModeStatus(), configuration).ConfigureAwait(false);
                    string printerRTModeStatus = ResponseParser.ParseDirectIOCommandResponseData(responseFromPrinter);
                    string rtType = printerRTModeStatus.Substring(2, 1);

                    responseFromPrinter = await communicationController.SubmitDocumentAsync(BuiltInDocuments.GetFiscalSerialNumber(), configuration).ConfigureAwait(false);
                    string fiscalSerialNumber = ResponseParser.ParseDirectIOCommandResponseData(responseFromPrinter);
                    string model = fiscalSerialNumber.Substring(8, 2);
                    string manufacture = fiscalSerialNumber.Substring(10, 2);
                    string serialNumber = fiscalSerialNumber.Substring(2, 6);

                    printerFullSerialNumber = manufacture + rtType + model + serialNumber;
                }
                catch { }

                return printerFullSerialNumber;
            }
        }
    }
}