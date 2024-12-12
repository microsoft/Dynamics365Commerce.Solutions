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
        using System.Linq;
        using System.Xml.Linq;
        using Microsoft.Dynamics.Commerce.HardwareStation.PeripheralRequests;
        using Microsoft.Dynamics.Commerce.HardwareStation.Peripherals.Entities;

        /// <summary>
        /// Class implements methods that parses responses from the printer.
        /// </summary>
        internal static class ResponseParser
        {
            /// <summary>
            /// Parses response from printer.
            /// </summary>
            /// <typeparam name="TResponse">The type of the fiscal device response.</typeparam>
            /// <param name="responseFromPrinter">The response from printer.</param>
            /// <returns>The fiscal device response.</returns>
            internal static TResponse ParseResponse<TResponse>(string responseFromPrinter) where TResponse : FiscalDeviceResponseBase
            {
                XDocument document = XDocument.Parse(responseFromPrinter);
                XAttribute resultAttribute = document.Descendants("response").Attributes("success").FirstOrDefault();
                bool result = (resultAttribute != null && resultAttribute.Value != null) ? Convert.ToBoolean(resultAttribute.Value) : false;
                XAttribute codeAttribute = document.Descendants("response").Attributes("code").FirstOrDefault();
                string code = (codeAttribute != null && codeAttribute.Value != null) ? codeAttribute.Value : string.Empty;
                XAttribute statusAttribute = document.Descendants("response").Attributes("status").FirstOrDefault();
                string status = statusAttribute != null ? statusAttribute.Value : string.Empty;

                FiscalPeripheralCommunicationResultType resultType = FiscalPeripheralCommunicationResultType.None;
                FiscalPeripheralFailureDetails failureDetails = new FiscalPeripheralFailureDetails();

                if (result)
                {
                    resultType = FiscalPeripheralCommunicationResultType.Succeeded;

                }
                else
                {
                    resultType = FiscalPeripheralCommunicationResultType.Failed;
                    failureDetails.ErrorCode = status;
                    failureDetails.ErrorMessage = code;

                    DecodeErrorCode(failureDetails, code, status);
                }

                string cleanResponse = document.Descendants("response").FirstOrDefault().ToString();
                TResponse response = (TResponse)Activator.CreateInstance(typeof(TResponse), new object[] { cleanResponse, resultType, failureDetails, string.Empty });

                return response;
            }

            /// <summary>
            /// Parses response from the printer for directIO command.
            /// </summary>
            /// <param name="responseFromPrinter">The response from printer.</param>
            /// <returns>Returned data from the printer response.</returns>
            internal static string ParseDirectIOCommandResponseData(string responseFromPrinter)
            {
                string responseData = string.Empty;
                FiscalDeviceResponseBase deviceResponseBase = ParseResponse<FiscalDeviceResponseBase>(responseFromPrinter);

                if (deviceResponseBase.CommunicationResultType == FiscalPeripheralCommunicationResultType.Succeeded)
                {
                    XDocument document = XDocument.Parse(responseFromPrinter);
                    responseData = document.Descendants("response").First()?.Descendants("addInfo").First()?.Element("responseData")?.Value ?? string.Empty;
                }

                return responseData;
            }

            /// <summary>
            /// Decodes eror codes returned from printer.
            /// </summary>
            /// <param name="failureDetails">Details about the failure.</param>
            /// <param name="errorCode">Code of the printer error.</param>
            /// <param name="status">The status of the response from the printer.</param>
            private static void DecodeErrorCode(FiscalPeripheralFailureDetails failureDetails, string errorCode, string status)
            {
                switch (errorCode)
                {
                    case "EPTR_REC_EMPTY":
                        failureDetails.IsRetryAllowed = true;
                        failureDetails.FailureType = FiscalPeripheralFailureType.PaperOut;
                        failureDetails.ErrorCode = status;
                        failureDetails.ErrorMessage = errorCode;
                        break;
                    case "TP_NO_ANSWER/OFF_LINE":
                    case "FP_NO_ANSWER":
                    case "LAN_ERROR":
                    case "FP_NO_ANSWER_NETWORK":
                        failureDetails.IsRetryAllowed = true;
                        failureDetails.FailureType = FiscalPeripheralFailureType.NotAvailable;
                        break;
                    case "LAN_TIME_OUT":
                        failureDetails.IsRetryAllowed = true;
                        failureDetails.FailureType = FiscalPeripheralFailureType.Busy;
                        break;
                    case "PRINTER ERROR":
                    case "PARSER_EROR":
                    case "NO_DATA":
                    case "NO_RAM":
                    case "INCOMPLETE FILE":
                    case "No valid XML command":
                        failureDetails.IsRetryAllowed = false;
                        failureDetails.FailureType = FiscalPeripheralFailureType.SubmissionFailed;
                        break;
                    default:
                        failureDetails.IsRetryAllowed = false;
                        failureDetails.FailureType = FiscalPeripheralFailureType.Other;
                        break;
                }
            }
        }
    }
}