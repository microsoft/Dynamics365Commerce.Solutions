/**
 * SAMPLE CODE NOTICE
 *
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

import { ClientEntities } from "PosApi/Entities";
import { ObjectExtensions } from "PosApi/TypeExtensions";
import { HttpRequestError } from "./Http/HttpRequestError";
import { HttpStatusCodes } from "./Http/HttpStatusCodes";
import { IHttpResponse } from "./Http/IHttpResponse";

/**
 * Converts an HTTP request error object.
 */
export class HttpRequestErrorConverter {
    /**
     * Populates the failure details object with the values based on the HTTP request error.
     * @param {ClientEntities.IFiscalRegisterFailureDetails} failureDetails The failure details object.
     * @param {HttpRequestError} requestError The HTTP request error.
     */
    public static populateFailureDetails(failureDetails: ClientEntities.FiscalIntegration.IFiscalRegisterFailureDetails, requestError: HttpRequestError): void {
        let failureType: ClientEntities.FiscalIntegration.FiscalRegisterFailureType;
        let response: IHttpResponse = requestError.response;
        let errorCode: string;
        let errorMessage: string;
        if (!ObjectExtensions.isNullOrUndefined(response)) {
            errorCode = response.statusCode.toString();
            errorMessage = response.statusText;
            if (response.statusCode === HttpStatusCodes.TIMEOUT) {
                failureType = ClientEntities.FiscalIntegration.FiscalRegisterFailureType.Timeout;
            }
        } else {
            failureType = ClientEntities.FiscalIntegration.FiscalRegisterFailureType.Other;
            errorMessage = requestError.message;
        }
        failureDetails.failureType = failureType;
        failureDetails.errorCode = errorCode;
        failureDetails.errorMessage = errorMessage;
    }
}