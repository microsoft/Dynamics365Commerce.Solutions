/**
 * SAMPLE CODE NOTICE
 *
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

import { ClientEntities } from "PosApi/Entities";
import { StringExtensions } from "PosApi/TypeExtensions";
import { IEfrFiscalRegisterResponse } from "./IEfrFiscalRegisterResponse";
import { RegistrationResultCodes } from "./RegistrationResultCodes";

/**
 * The EFR response parser.
 */
export class ResponseParser {
    /**
     * Parses the response after submitting a document.
     * @param {string} xmlString The respose XML string.
     * @param {boolean} includeUserNotificationMessage True to include the user notification message from EFR; false otherwise.
     * @returns {IEfrFiscalRegisterResponse} The parsed response.
     */
    public static parseSubmitDocumentResponse(xmlString: string, includeUserNotificationMessage: boolean):
        IEfrFiscalRegisterResponse {

        let result: IEfrFiscalRegisterResponse = {
            communicationResultType: ClientEntities.FiscalIntegration.FiscalRegisterCommunicationResultType.None,
            response: xmlString,
            userNotificationMessage: null,
            failureDetails: null,
            fiscalRegisterInfo: StringExtensions.EMPTY,
            warning: null
        };

        try {
            let parser: DOMParser = new DOMParser();
            let xml: Document = parser.parseFromString(xmlString, "text/xml");
            let resultElement: Element = xml.querySelector("Result");
            let resultCode: string = resultElement.getAttribute("RC");
            let errorCodeNode: Element = resultElement.querySelector("ErrorCode");
            let errorCode: string = null;
            if (errorCodeNode) {
                errorCode = errorCodeNode.textContent;
            }
            switch (resultCode) {
                case RegistrationResultCodes.TransactionProcessedSuccessfully:
                    result.communicationResultType = ClientEntities.FiscalIntegration.FiscalRegisterCommunicationResultType.Succeeded;
                    break;
                case RegistrationResultCodes.CouldNotProcessTransaction:
                    result.communicationResultType = ClientEntities.FiscalIntegration.FiscalRegisterCommunicationResultType.Failed;
                    result.failureDetails = {
                        isRetryAllowed: true,
                        failureType: ClientEntities.FiscalIntegration.FiscalRegisterFailureType.Other,
                        errorCode: errorCode
                    };
                    break;
                case RegistrationResultCodes.InvalidRequestData:
                    result.communicationResultType = ClientEntities.FiscalIntegration.FiscalRegisterCommunicationResultType.Failed;
                    result.failureDetails = {
                        isRetryAllowed: false,
                        failureType: ClientEntities.FiscalIntegration.FiscalRegisterFailureType.Other,
                        errorCode: errorCode
                    };
                    break;
            }
            if (includeUserNotificationMessage) {
                let userMessageNode: Element = resultElement.querySelector("UserMessage");
                if (userMessageNode) {
                    result.userNotificationMessage = userMessageNode.textContent;
                }
            }

            let warningNode: Element = resultElement.querySelector("Warning");
            if (warningNode) {
                result.warning = warningNode.textContent;
            }
        } catch (error) {
            result.communicationResultType = ClientEntities.FiscalIntegration.FiscalRegisterCommunicationResultType.Failed;
            result.failureDetails = {
                isRetryAllowed: true,
                failureType: ClientEntities.FiscalIntegration.FiscalRegisterFailureType.BadResponse,
                errorMessage: error.message
            };
        }

        return result;
    }
}