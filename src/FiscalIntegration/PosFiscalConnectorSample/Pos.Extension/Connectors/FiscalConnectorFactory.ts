/**
 * SAMPLE CODE NOTICE
 *
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

import { IRuntime } from "PosApi/Framework/Runtime";
import { IExtensionLogger } from "PosApi/Framework/Logging";
import { IFiscalConnector } from "./IFiscalConnector";
import { EfrFiscalConnector } from "./Efr/EfrFiscalConnector";

export class FiscalConnectorFactory {

    /**
     * Gets the fiscal connector by name.
     * @param {string} connectorName The fiscal connector name.
     * @param {IRuntime} runtime The runtime.
     * @param {IExtensionLogger} logger The logger.
     * @returns {IFiscalConnector} The fiscal connector or null if not found.
     */
    static getFiscalConnector(connectorName: string, runtime: IRuntime, logger: IExtensionLogger): IFiscalConnector {
        switch (connectorName) {
            case "EFRSample":
                return new EfrFiscalConnector(runtime, logger);
            default:
                return null;
        }
    }
}