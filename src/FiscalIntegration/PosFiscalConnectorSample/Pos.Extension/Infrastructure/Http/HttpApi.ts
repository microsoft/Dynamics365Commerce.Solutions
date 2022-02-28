/**
 * SAMPLE CODE NOTICE
 *
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

 import { ClientEntities } from "PosApi/Entities";
 import { IExtensionLogger } from "PosApi/Framework/Logging";
 import { IRuntime } from "PosApi/Framework/Runtime";
 import { IHttpRequest } from "../../Infrastructure/Http/IHttpRequest";
 import { IHttpResponse } from "../../Infrastructure/Http/IHttpResponse";
 import { ExecuteHttpRequestRequest } from "../../Messages/ExecuteHttpRequestRequest";
 import { ExecuteHttpRequestResponse } from "../../Messages/ExecuteHttpRequestResponse";

 /**
  * The base HTTP API.
  */
 export class HttpApi {
     protected runtime: IRuntime;
     protected logger: IExtensionLogger;

     /**
      * Creates a new instance of {@link HttpApi} class.
      * @param {IRuntime} runtime The runtime.
      * @param {IExtensionLogger} logger The logger.
      */
     public constructor(runtime: IRuntime, logger: IExtensionLogger) {
         this.runtime = runtime;
         this.logger = logger;
     }

     /**
      * Executes an HTTP request using the {@link ExecuteHttpRequestRequest}.
      * @param {IHttpRequest} httpRequest The HTTP request.
      * @returns {IHttpResponse} The HTTP response.
      */
     protected _executeHttpRequest(httpRequest: IHttpRequest): Promise<IHttpResponse> {
         return this.runtime.executeAsync(new ExecuteHttpRequestRequest(this.logger.getNewCorrelationId(), httpRequest))
             .then((result: ClientEntities.ICancelableDataResult<ExecuteHttpRequestResponse>) => {
                 if (result.canceled) {
                     throw new Error("The request was canceled.");
                 }
                 return result.data.httpResponse;
             });
     }
 }