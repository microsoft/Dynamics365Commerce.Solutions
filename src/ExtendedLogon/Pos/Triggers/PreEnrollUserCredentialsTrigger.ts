/**
 * SAMPLE CODE NOTICE
 * 
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

import * as Triggers from "PosApi/Extend/Triggers/ApplicationTriggers";
import { CancelableTriggerResult } from "PosApi/Extend/Triggers/Triggers";
import PinInputDialog, { IRequestPinNumberResult } from "../Controls/PinInputDialog";
import IPreEnrollUserCredentialsTriggerOptions = Triggers.IPreEnrollUserCredentialsTriggerOptions;

/**
 * Example implementation of an PreEnrollUserCredentialsTrigger trigger that accepts extra PIN number and modifies extraParameters.
 */
export default class PreEnrollUserCredentialsTrigger extends Triggers.PreEnrollUserCredentialsTrigger {
    /**
     * Executes the trigger functionality.
     * The extraParameters can be updated and this modifies the value used in the enroll operation.
     * @param {Triggers.IPreEnrollUserCredentialsTriggerOptions} options The options provided to the trigger.
     */
    public execute(options: Triggers.IPreEnrollUserCredentialsTriggerOptions):
        Promise<CancelableTriggerResult<IPreEnrollUserCredentialsTriggerOptions>> {
        return PinInputDialog.getPinNumber(this.context).then((result: IRequestPinNumberResult) => {
            if (!result.canceled) {
                options.extraParameters = { pin: result.pinNumber };
                return Promise.resolve(new CancelableTriggerResult(false, options));
            } else {
                return Promise.resolve(new CancelableTriggerResult(true, options));
            }
        });
    }
}
