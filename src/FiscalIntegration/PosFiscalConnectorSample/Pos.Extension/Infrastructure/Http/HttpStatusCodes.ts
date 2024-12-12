/**
 * SAMPLE CODE NOTICE
 *
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

/**
 * Constants for HTTP status codes.
 */
export class HttpStatusCodes {
    public static readonly OK: number = 200;
    public static readonly TIMEOUT: number = 408;
    public static readonly NO_CONTENT: number = 204;
    private static readonly LAST_OK_STATUS_CODE: number = 299;

    /**
     * Checks if the status code is a successful code.
     * @param {number} statusCode The status code.
     * @returns {boolean} True if the status code is a successful code; false otherwise.
     */
    public static isSuccessful(statusCode: number): boolean {
        return statusCode >= HttpStatusCodes.OK && statusCode <= HttpStatusCodes.LAST_OK_STATUS_CODE;
    }
}