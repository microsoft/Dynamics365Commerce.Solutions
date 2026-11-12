System.register(["PosApi/Consume/Dialogs"], function (exports_1, context_1) {
    "use strict";
    var Dialogs_1, PinInputDialog;
    var __moduleName = context_1 && context_1.id;
    return {
        setters: [
            function (Dialogs_1_1) {
                Dialogs_1 = Dialogs_1_1;
            }
        ],
        execute: function () {
            PinInputDialog = (function () {
                function PinInputDialog() {
                }
                PinInputDialog.getPinNumber = function (context) {
                    var textInputDialogOptions = {
                        title: "Pin number",
                        label: "Enter pin number for extended logon:",
                        textInputType: Commerce.Client.Entities.Dialogs.TextInputType.password,
                    };
                    var dialogRequest = new Dialogs_1.ShowTextInputDialogClientRequest(textInputDialogOptions);
                    var promise = context.runtime.executeAsync(dialogRequest)
                        .then(function (result) {
                        var _a, _b;
                        var requestPinNumberResult;
                        if (result.canceled) {
                            requestPinNumberResult = { canceled: true };
                        }
                        else {
                            requestPinNumberResult = {
                                canceled: false,
                                pinNumber: (_b = (_a = result.data) === null || _a === void 0 ? void 0 : _a.result) === null || _b === void 0 ? void 0 : _b.value
                            };
                        }
                        return requestPinNumberResult;
                    });
                    return promise;
                };
                return PinInputDialog;
            }());
            exports_1("default", PinInputDialog);
        }
    };
});
//# sourceMappingURL=C:/__w/1/s/r/src/ExtendedLogon/Pos/Controls/PinInputDialog.js.map