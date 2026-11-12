/**
 * SAMPLE CODE NOTICE
 *
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

import { IHttpResponse } from "./IHttpResponse";

/**
 * The HTTP request error.
 */
export class HttpRequestError extends Error {
    public readonly response?: IHttpResponse;

    /**
     * Creates a new instance of {@link HttpRequestError}.
     * @param {string} message The error message.
     * @param {IHttpResponse} response The HTTP response.
     */
    constructor(message?: string, response?: IHttpResponse) {
        super(message);

        // https://github.com/Microsoft/TypeScript-wiki/blob/master/Breaking-Changes.md#extending-built-ins-like-error-array-and-map-may-no-longer-work
        // Set the prototype explicitly.
        (<any>this).__proto__ = HttpRequestError.prototype;

        this.response = response;
    }
}