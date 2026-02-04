/**
 * SAMPLE CODE NOTICE
 *
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

import { ExtensionRequestHandlerBase, ExtensionRequestType } from "PosApi/Create/RequestHandlers";
import { ClientEntities } from "PosApi/Entities";
import { ObjectExtensions, StringExtensions } from "PosApi/TypeExtensions";
import { ExecuteHttpRequestRequest } from "../Messages/ExecuteHttpRequestRequest";
import { ExecuteHttpRequestResponse } from "../Messages/ExecuteHttpRequestResponse";
import { HttpStatusCodes } from "../Infrastructure/Http/HttpStatusCodes";
import { IHttpHeaders} from "../Infrastructure/Http/IHttpHeaders";
import { IHttpRequest} from "../Infrastructure/Http/IHttpRequest";
import { IHttpResponse } from "../Infrastructure/Http/IHttpResponse";
import { HttpRequestError } from "../Infrastructure/Http/HttpRequestError";
import { HttpMethods } from "../Infrastructure/Http/HttpMethods";

/**
 * The request handler for {@link ExecuteHttpRequestRequest}.
 */
export default class ExecuteHttpRequestRequestHandler extends ExtensionRequestHandlerBase<ExecuteHttpRequestResponse> {
    /**
     * Gets the supported request type.
     * @return {ExtensionRequestType<TResponse>} The supported abstract or concrete request type.
     */
    public supportedRequestType(): ExtensionRequestType<ExecuteHttpRequestResponse> {
        return ExecuteHttpRequestRequest;
    }

    /**
     * Executes the request handler asynchronously.
     * @param {ExecuteHttpRequestRequest<TResponse>} request The request.
     * @return {Promise<ClientEntities.ICancelableDataResult<TResponse>>} The promise with a cancelable result containing the response.
     */
    public executeAsync(request: ExecuteHttpRequestRequest<ExecuteHttpRequestResponse>)
        : Promise<ClientEntities.ICancelableDataResult<ExecuteHttpRequestResponse>> {

        return ExecuteHttpRequestRequestHandler._executeHttpRequest(request.httpRequest).then((httpResponse: IHttpResponse) => {
            return {
                canceled: false,
                data: new ExecuteHttpRequestResponse(httpResponse)
            };
        });
    }

    /**
     * Executes an HTTP request.
     * @param {IHttpRequest} request The HTTP request.
     * @returns {IHttpResponse} The HTTP response.
     */
     private static _executeHttpRequest(request: IHttpRequest): Promise<IHttpResponse> {
        let promise: Promise<IHttpResponse> = new Promise((resolve, reject) => {
            let xhr: XMLHttpRequest = new XMLHttpRequest();
            xhr.ontimeout = function (): void {
                if (xhr != null) {
                    xhr.abort();
                    xhr = null;
                    let statusText: string = "Request Timeout";
                    let response: IHttpResponse = {
                        ok: false,
                        statusCode: HttpStatusCodes.TIMEOUT,
                        statusText: statusText,
                        headers: {},
                        body: StringExtensions.EMPTY
                    };
                    reject(new HttpRequestError(statusText, response));
                }
            };
            xhr.onloadend = function (): void {
                if (xhr === null) {
                    return;
                }
                let response: IHttpResponse = ExecuteHttpRequestRequestHandler._getResponse(xhr);
                xhr = null;
                if (response.ok) {
                    resolve(response);
                } else {
                    reject(new HttpRequestError(response.statusText, response));
                }
            };
            xhr.onerror = function (): void {
                let message: string = ExecuteHttpRequestRequestHandler._getErrorMessage();
                reject(new HttpRequestError(message));
            };

            try {
                xhr.open(request.method || HttpMethods.GET, request.requestUri, true);
            } catch (exception) {
                let message: string = ExecuteHttpRequestRequestHandler._getErrorMessage(exception);
                reject(new HttpRequestError(message));
                return;
            }

            // Transfer request headers to XHR object.
            if (request.headers) {
                ObjectExtensions.forEachKeyValuePair(request.headers,
                    (headerName: string, headerValue: string): void => {
                        xhr.setRequestHeader(headerName, headerValue);
                    });
            }

            xhr.timeout = request.timeout;

            try {
                xhr.send(request.body);
            } catch (exception) {
                let message: string = ExecuteHttpRequestRequestHandler._getErrorMessage(exception);
                reject(new HttpRequestError(message));
            }
        });
        return promise;
    }

    /**
     * Parse a header string into a key value pair object.
     * @param {string} headerString the string with the headers.
     * @returns {{ [headerName: string]: string }} the key value pair object containing the headers.
     */
    private static _parseXmlHttpResponseHeaders(headerString: string): IHttpHeaders {
        let responseHeaders: string[] = (headerString || "").split(/\r?\n/);
        let headers: IHttpHeaders = {};

        responseHeaders.forEach(function (value: string): void {
            if (value) {
                let pair: string[] = value.split(": ");
                headers[pair[0]] = pair[1];
            }
        });

        return headers;
    }

    private static _getErrorMessage(error?: Error): string {
        let errorMessage: string = "Failed to send the HTTP request.";

        if (!ObjectExtensions.isNullOrUndefined(error) && !ObjectExtensions.isNullOrUndefined(error.message)) {
            errorMessage = error.message;
        }

        return errorMessage;
    }

    /**
     * Converts the response of an XML HTTP request to the HTTP response.
     * @param {XMLHttpRequest} xhr The XML HTTP request.
     * @returns {IHttpResponse} The HTTP response.
     */
    private static _getResponse(xhr: XMLHttpRequest): IHttpResponse {
        // Workaround for XHR behavior on IE.
        let statusText: string = xhr.statusText;
        let statusCode: number = xhr.status;

        if (statusCode === 1223) {
            statusCode = HttpStatusCodes.NO_CONTENT;
            statusText = "No Content";
        }

        return {
            statusCode: statusCode,
            statusText: statusText,
            headers: this._parseXmlHttpResponseHeaders(xhr.getAllResponseHeaders()),
            body: xhr.responseText,
            ok: HttpStatusCodes.isSuccessful(statusCode)
        };
    }
}