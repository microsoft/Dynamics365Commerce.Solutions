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
import { IExtensionLogger } from "PosApi/Framework/Logging";
import { StringExtensions } from "PosApi/TypeExtensions";
import { HttpRequestError } from "../../Infrastructure/Http/HttpRequestError";
import { HttpRequestErrorConverter } from "../../Infrastructure/HttpRequestErrorConverter";
import { IFiscalConnector } from "../IFiscalConnector";
import { IEfrFiscalRegisterResponse } from "./IEfrFiscalRegisterResponse";
import { Configuration } from "./Configuration";
import { EfrHttpApi } from "./EfrHttpApi";
import { IRuntime } from "PosApi/Framework/Runtime";

/**
 * The EFR fiscal connector.
 */
export class EfrFiscalConnector implements IFiscalConnector {
    private runtime: IRuntime;
    private logger: IExtensionLogger;

    /**
     * Creates a new instance of the {@link EfrFiscalConnector}.
     * @param {IRuntime} The runtime.
     * @param {IExtensionLogger} logger The logger.
     */
    constructor(runtime: IRuntime, logger: IExtensionLogger) {
        this.runtime = runtime;
        this.logger = logger;
    }

    /**
     * Initializes the fiscal device or service.
     * @param request The request to initialize.
     * @returns The response with the result.
     */
    initialize(request: FiscalRegisterInitializeClientRequest<FiscalRegisterInitializeClientResponse>):
        Promise<ClientEntities.ICancelableDataResult<FiscalRegisterInitializeClientResponse>> {

        this.logger.logVerbose("[FiscalConnectorSample] Skipping EFR initialization.", request.correlationId);
        return Promise.resolve({
            canceled: false,
            data: new FiscalRegisterInitializeClientResponse({
                communicationResultType: ClientEntities.FiscalIntegration.FiscalRegisterCommunicationResultType.None,
                response: StringExtensions.EMPTY,
                fiscalRegisterInfo: StringExtensions.EMPTY
            })
        });
    }

    /**
     * Checks whether or not the fiscal device or service is ready.
     * @param request The request with the technical profile.
     * @returns The response with the result.
     */
    isReady(request: FiscalRegisterIsReadyClientRequest<FiscalRegisterIsReadyClientResponse>):
        Promise<ClientEntities.ICancelableDataResult<FiscalRegisterIsReadyClientResponse>> {

        this.logger.logVerbose("[FiscalConnectorSample] Checking if EFR is ready.", request.correlationId);
        return Promise.resolve().then(() => {
            let configuration: Configuration = Configuration.read(request.technicalProfile.TechnicalProfile);
            let api: EfrHttpApi = new EfrHttpApi(this.runtime, this.logger, configuration);
            return api.state();
        }).then((isReady: boolean) => {
            this.logger.logVerbose("[FiscalConnectorSample] Successfully checked the state of EFR.", request.correlationId);
            return new FiscalRegisterIsReadyClientResponse(isReady);
        }).catch((reason: Error) => {
            this.logger.logError("[FiscalConnectorSample] There was an error checking the state of EFR.", request.correlationId, reason.message);
            return new FiscalRegisterIsReadyClientResponse(false);
        }).then((response: FiscalRegisterIsReadyClientResponse) => {
            return {
                canceled: false,
                data: response
            };
        });
    }

    /**
     * Submits the document to the fiscal device or service.
     * @param request The request to submit the document.
     * @returns The response with the result.
     */
    submitDocument(request: FiscalRegisterSubmitDocumentClientRequest<FiscalRegisterSubmitDocumentClientResponse>):
        Promise<ClientEntities.ICancelableDataResult<FiscalRegisterSubmitDocumentClientResponse>> {

        this.logger.logVerbose("[FiscalConnectorSample] Submitting a document to EFR.", request.correlationId);
        return Promise.resolve().then(() => {
            let configuration: Configuration = Configuration.read(request.document.FiscalConnectorTechnicalProfile.TechnicalProfile);
            let api: EfrHttpApi = new EfrHttpApi(this.runtime, this.logger, configuration);
            return api.register(request.document.Document);
        }).then((registerResponse: IEfrFiscalRegisterResponse) => {
            this.logger.logVerbose("[FiscalConnectorSample] Successfully submitted the document to EFR.", request.correlationId);
            if (!StringExtensions.isNullOrWhitespace(registerResponse.warning)) {
                this.logger.logWarning("[FiscalConnectorSample]", request.correlationId, registerResponse.warning);
            }
            return new FiscalRegisterSubmitDocumentClientResponse(registerResponse);
        }).catch((reason: Error | HttpRequestError) => {
            this.logger.logError("[FiscalConnectorSample] There was an error submitting the document to EFR.", request.correlationId, reason.message);
            let failureDetails: ClientEntities.FiscalIntegration.IFiscalRegisterFailureDetails = {
                isRetryAllowed: true,
                failureType: ClientEntities.FiscalIntegration.FiscalRegisterFailureType.Other,
                errorMessage: reason.message
            };
            if (reason instanceof HttpRequestError) {
                HttpRequestErrorConverter.populateFailureDetails(failureDetails, reason);
            }
            return new FiscalRegisterSubmitDocumentClientResponse({
                communicationResultType: ClientEntities.FiscalIntegration.FiscalRegisterCommunicationResultType.Failed,
                response: StringExtensions.EMPTY,
                fiscalRegisterInfo: StringExtensions.EMPTY,
                failureDetails: failureDetails
            });
        }).then((response: FiscalRegisterSubmitDocumentClientResponse) => {
            return {
                canceled: false,
                data: response
            };
        });
    }
}
