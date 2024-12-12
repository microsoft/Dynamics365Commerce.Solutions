import {
    ITextInputDialogOptions, ShowTextInputDialogClientRequest,
    ShowTextInputDialogClientResponse
} from "PosApi/Consume/Dialogs";
import { ClientEntities } from "PosApi/Entities";
import { IExtensionContext } from "PosApi/Framework/ExtensionContext";

export interface IRequestPinNumberResult {
    canceled: boolean;
    pinNumber?: string;
}

/**
 * Example implementation of a text input dialog that accepts extra PIN number from POS user.
 */
export default class PinInputDialog {

    public static getPinNumber(context: IExtensionContext): Promise<IRequestPinNumberResult> {
        let textInputDialogOptions: ITextInputDialogOptions = {
            title: "Pin number",
            label: "Enter pin number for extended logon:",
            textInputType: Commerce.Client.Entities.Dialogs.TextInputType.password,
        };

        let dialogRequest: ShowTextInputDialogClientRequest<ShowTextInputDialogClientResponse> =
            new ShowTextInputDialogClientRequest<ShowTextInputDialogClientResponse>(textInputDialogOptions);

        let promise: Promise<IRequestPinNumberResult> = context.runtime.executeAsync(dialogRequest)
            .then((result: ClientEntities.ICancelableDataResult<ShowTextInputDialogClientResponse>) => {
                let requestPinNumberResult: IRequestPinNumberResult;

                if (result.canceled) {
                    requestPinNumberResult = { canceled: true };
                } else {
                    requestPinNumberResult = {
                        canceled: false,
                        pinNumber: result.data?.result?.value
                    };
                }

                return requestPinNumberResult;
            });
        return promise;
    }
}