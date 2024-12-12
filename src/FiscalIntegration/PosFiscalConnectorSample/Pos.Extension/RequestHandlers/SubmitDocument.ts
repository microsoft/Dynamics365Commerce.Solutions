/**
 * SAMPLE CODE NOTICE
 *
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

import { FiscalRegisterSubmitDocumentClientRequestHandler } from "PosApi/Extend/RequestHandlers/FiscalIntegrationRequestHandlers";
import { FiscalRegisterSubmitDocumentClientRequest, FiscalRegisterSubmitDocumentClientResponse } from "PosApi/Consume/FiscalIntegration";
import { ClientEntities, ProxyEntities } from "PosApi/Entities";
import { IFiscalConnector } from "../Connectors/IFiscalConnector";
import { FiscalConnectorFactory } from "../Connectors/FiscalConnectorFactory";

/**
 * Override request handler class for submitting a document to a fiscal register.
 * Note. the filename has been shortened in order to fit the maximum path length during build.
 */
export default class FiscalRegisterSubmitDocumentClientRequestHandlerExt extends FiscalRegisterSubmitDocumentClientRequestHandler {
    /**
     * Executes the request handler asynchronously.
     * @param {FiscalRegisterSubmitDocumentClientRequest<FiscalRegisterSubmitDocumentClientResponse>} request The request.
     * @return {Promise<ICancelableDataResult<FiscalRegisterSubmitDocumentClientResponse>>} The cancelable promise containing the response.
     */
    public executeAsync(request: FiscalRegisterSubmitDocumentClientRequest<FiscalRegisterSubmitDocumentClientResponse>):
        Promise<ClientEntities.ICancelableDataResult<FiscalRegisterSubmitDocumentClientResponse>> {

        let technicalProfile: ProxyEntities.FiscalIntegrationTechnicalProfile = request.document.FiscalConnectorTechnicalProfile;
        if (technicalProfile.ConnectorLocationValue !== ProxyEntities.FiscalIntegrationConnectorLocation.Pos) {
            return this.defaultExecuteAsync(request);
        }

        let fiscalConnector: IFiscalConnector = FiscalConnectorFactory.getFiscalConnector(
            request.document.FiscalConnectorTechnicalProfile.ConnectorName,
            this.context.runtime,
            this.context.logger);

        if (fiscalConnector === null) {
            return this.defaultExecuteAsync(request);
        }

        return fiscalConnector.submitDocument(request);
    }
}