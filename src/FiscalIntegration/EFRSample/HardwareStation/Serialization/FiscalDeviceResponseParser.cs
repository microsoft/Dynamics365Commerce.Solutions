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
        using System.Xml.Linq;
        using Contoso.CommerceRuntime.DocumentProvider.DataModelEFR.Constants;
        using Contoso.CommerceRuntime.DocumentProvider.DataModelEFR.Documents;
        using Microsoft.Dynamics.Commerce.HardwareStation.PeripheralRequests;
        using Microsoft.Dynamics.Commerce.HardwareStation.Peripherals.Entities;

        /// <summary>
        /// The fiscal device response parser.
        /// </summary>
        public static class FiscalDeviceResponseParser
        {
            private const string ResultElementName = "Result";

            /// <summary>
            /// Parses the submit document fiscal device response from the service response.
            /// </summary>
            /// <param name="serviceResponse">The response from the service.</param>
            /// <param name="includeUserNotificationMessage">True to unclude user notification message.</param>
            /// <returns>The submit document fiscal device response.</returns>
            public static SubmitDocumentFiscalDeviceResponse ParseSubmitDocumentFiscalDeviceResponse(string serviceResponse, bool includeUserNotificationMessage)
            {
                FiscalPeripheralCommunicationResultType resultType = FiscalPeripheralCommunicationResultType.None;
                FiscalPeripheralFailureDetails failureDetails = new FiscalPeripheralFailureDetails();
                RegistrationResult registrationResult = null;
                string userNotificationMessage = string.Empty;

                if (!TryGetRegistrationResult(serviceResponse, out registrationResult))
                {
                    resultType = FiscalPeripheralCommunicationResultType.Failed;
                    failureDetails.IsRetryAllowed = true;
                    failureDetails.FailureType = FiscalPeripheralFailureType.BadResponse;
                }

                if (registrationResult != null)
                {
                    switch (registrationResult.ResultCode)
                    {
                        case RegistrationResultCodeConstants.TransactionProcessedSuccessfully:
                            resultType = FiscalPeripheralCommunicationResultType.Succeeded;
                            failureDetails.IsRetryAllowed = false;
                            failureDetails.FailureType = FiscalPeripheralFailureType.None;
                            break;

                        case RegistrationResultCodeConstants.CouldNotProcessTransaction:
                            resultType = FiscalPeripheralCommunicationResultType.Failed;
                            failureDetails.IsRetryAllowed = true;
                            failureDetails.FailureType = FiscalPeripheralFailureType.Other;
                            failureDetails.ErrorCode = registrationResult.ErrorCode;
                            break;

                        case RegistrationResultCodeConstants.InvalidRequestData:
                            resultType = FiscalPeripheralCommunicationResultType.Failed;
                            failureDetails.IsRetryAllowed = false;
                            failureDetails.FailureType = FiscalPeripheralFailureType.Other;
                            failureDetails.ErrorCode = registrationResult.ErrorCode;
                            break;

                        default:
                            throw new NotSupportedException(string.Format("Result code '{0}' is not supported.", registrationResult));
                    }

                    if (includeUserNotificationMessage)
                    {
                        userNotificationMessage = registrationResult.UserMessage;
                    }
                }

                var response = new SubmitDocumentFiscalDeviceResponse(response: serviceResponse, communicationResultType: resultType, failureDetails: failureDetails, fiscalPeripheralInfo: string.Empty, userNotificationMessage: userNotificationMessage);

                return response;
            }

            /// <summary>
            /// Gets the registration result from the service response.
            /// </summary>
            /// <param name="serviceResponse">The service response.</param>
            /// <param name="result">The registration result.</param>
            /// <returns>True if the operation was successful, otherwise false.</returns>
            private static bool TryGetRegistrationResult(string serviceResponse, out RegistrationResult result)
            {
                var resultElement = XDocument.Parse(serviceResponse)
                    .Root
                    .Element(ResultElementName);

                if (resultElement == null)
                {
                    result = null;
                    return false;
                }

                return XmlSerializationHelper.TryDeserialize(resultElement, ResultElementName, out result);
            }
        }
    }
}
