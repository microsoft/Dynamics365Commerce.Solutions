/**
 * SAMPLE CODE NOTICE
 *
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

import { IExtensionLogger } from "PosApi/Framework/Logging";
import { IRuntime } from "PosApi/Framework/Runtime";
import { HttpApi } from "../../Infrastructure/Http/HttpApi";
import { HttpMethods } from "../../Infrastructure/Http/HttpMethods";
import { IHttpRequest } from "../../Infrastructure/Http/IHttpRequest";
import { IHttpResponse } from "../../Infrastructure/Http/IHttpResponse";
import { Configuration } from "./Configuration";
import { IEfrFiscalRegisterResponse } from "./IEfrFiscalRegisterResponse";
import { ResponseParser } from "./ResponseParser";

/**
 * The EFR HTTP API.
 */
export class EfrHttpApi extends HttpApi {
    private configuration: Configuration;

    /**
     * Creates a new instance of {@link EfrHttpApi} class.
     * @param {IRuntime} runtime The runtime.
     * @param {Configuration} configuration The EFR configuration.
     */
    public constructor(runtime: IRuntime, logger: IExtensionLogger, configuration: Configuration) {
        super(runtime, logger);
        this.configuration = configuration;
    }

    /**
     * Checks the state of EFR.
     * @returns {boolean} True if EFR is alive; false otherwise.
     */
    public state(): Promise<boolean> {
        let httpRequest: IHttpRequest = {
            method: HttpMethods.GET,
            requestUri: new URL("state", this.configuration.endpointAddress.toString()).toString(),
            timeout: this.configuration.timeout
        };
        return this._executeHttpRequest(httpRequest).then((response: IHttpResponse) => {
            return response.ok;
        });
    }

    /**
     * Registers a fiscal document.
     * @param {string} documentXmlString The fiscal document XML string.
     * @returns {IEfrFiscalRegisterResponse} The EFR-specific fiscal register response.
     */
    public register(documentXmlString: string): Promise<IEfrFiscalRegisterResponse> {
        let httpRequest: IHttpRequest = {
            method: HttpMethods.POST,
            requestUri: new URL("register", this.configuration.endpointAddress.toString()).toString(),
            timeout: this.configuration.timeout,
            body: documentXmlString
        };
        return this._executeHttpRequest(httpRequest).then((response: IHttpResponse) => {
            return ResponseParser.parseSubmitDocumentResponse(response.body, this.configuration.showUserNotificationMessage);
        });
    }
}