/**
 * SAMPLE CODE NOTICE
 *
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

import { IHttpHeaders } from "./IHttpHeaders";

/**
 * Interface for an HTTP request.
 */
export interface IHttpRequest {
    /**
     * The request uri.
     */
    requestUri: string;

    /**
     * The request body.
     */
    body?: string;

    /**
     * The request headers.
     */
    headers?: IHttpHeaders;

    /**
     * The request method.
     */
    method?: string;

    /**
     * The timeout (in milliseconds).
     */
    timeout: number;
}