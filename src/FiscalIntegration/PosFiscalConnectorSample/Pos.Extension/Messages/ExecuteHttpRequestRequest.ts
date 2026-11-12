/**
 * SAMPLE CODE NOTICE
 *
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

import { ExtensionRequestBase } from "PosApi/Create/RequestHandlers";
import { IHttpRequest } from "../Infrastructure/Http/IHttpRequest";
import { ExecuteHttpRequestResponse } from "./ExecuteHttpRequestResponse";

/**
 * The request to execute an HTTP request.
 */
export class ExecuteHttpRequestRequest<TResponse extends ExecuteHttpRequestResponse> extends ExtensionRequestBase<TResponse> {
    /**
     * The HTTP request.
     */
    public httpRequest: IHttpRequest;

    /**
     * Creates a new instance of {@link ExecuteHttpRequestRequest}.
     * @param {string} correlationId The correlation identifier.
     * @param {IHttpRequest} httpRequest The HTTP request.
     */
    constructor(correlationId: string, httpRequest: IHttpRequest) {
        super(correlationId);
        this.httpRequest = httpRequest;
    }
}