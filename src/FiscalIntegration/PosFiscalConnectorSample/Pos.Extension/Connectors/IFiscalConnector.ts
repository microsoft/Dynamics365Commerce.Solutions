/**
 * SAMPLE CODE NOTICE
 *
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

import {
    FiscalRegisterInitializeClientRequest,
    FiscalRegisterInitializeClientResponse,
    FiscalRegisterIsReadyClientRequest,
    FiscalRegisterIsReadyClientResponse,
    FiscalRegisterSubmitDocumentClientRequest,
    FiscalRegisterSubmitDocumentClientResponse
} from "PosApi/Consume/FiscalIntegration";
import { ClientEntities } from "PosApi/Entities";

/**
 * The connector interface for a fiscal device or service.
 */
export interface IFiscalConnector {
    /**
     * Initializes the fiscal device or service.
     * @param request The request to initialize.
     * @returns The response with the result.
     */
    initialize(request: FiscalRegisterInitializeClientRequest<FiscalRegisterInitializeClientResponse>):
        Promise<ClientEntities.ICancelableDataResult<FiscalRegisterInitializeClientResponse>>;

    /**
     * Checks whether or not the fiscal device or service is ready.
     * @param request The request with the technical profile.
     * @returns The response with the result.
     */
    isReady(request: FiscalRegisterIsReadyClientRequest<FiscalRegisterIsReadyClientResponse>):
        Promise<ClientEntities.ICancelableDataResult<FiscalRegisterIsReadyClientResponse>>;

    /**
     * Submits the document to the fiscal device or service.
     * @param request The request to submit the document.
     * @returns The response with the result.
     */
    submitDocument(request: FiscalRegisterSubmitDocumentClientRequest<FiscalRegisterSubmitDocumentClientResponse>):
        Promise<ClientEntities.ICancelableDataResult<FiscalRegisterSubmitDocumentClientResponse>>;
}