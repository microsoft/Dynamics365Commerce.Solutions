/**
 * SAMPLE CODE NOTICE
 *
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

import { Response } from "PosApi/Create/RequestHandlers";
import { IHttpResponse } from "../Infrastructure/Http/IHttpResponse";

/**
 * The response with the result of the HTTP request.
 */
export class ExecuteHttpRequestResponse extends Response {
    /**
     * The HTTP response.
     */
    public httpResponse: IHttpResponse;

    /**
     * Creates a new instance of {@link @ExecuteHttpRequestResponse}.
     * @param {IHttpResponse} httpResponse The HTTP response.
     */
    constructor(httpResponse: IHttpResponse) {
        super();
        this.httpResponse = httpResponse;
    }
}