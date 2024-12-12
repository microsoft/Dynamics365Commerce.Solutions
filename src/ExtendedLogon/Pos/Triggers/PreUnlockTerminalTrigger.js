System.register(["PosApi/Extend/Triggers/ApplicationTriggers", "PosApi/Extend/Triggers/Triggers", "PosApi/TypeExtensions", "../Controls/PinInputDialog"], function (exports_1, context_1) {
    "use strict";
    var __extends = (this && this.__extends) || (function () {
        var extendStatics = function (d, b) {
            extendStatics = Object.setPrototypeOf ||
                ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
                function (d, b) { for (var p in b) if (Object.prototype.hasOwnProperty.call(b, p)) d[p] = b[p]; };
            return extendStatics(d, b);
        };
        return function (d, b) {
            if (typeof b !== "function" && b !== null)
                throw new TypeError("Class extends value " + String(b) + " is not a constructor or null");
            extendStatics(d, b);
            function __() { this.constructor = d; }
            d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
        };
    })();
    var Triggers, Triggers_1, TypeExtensions_1, PinInputDialog_1, PreUnlockTerminalTrigger;
    var __moduleName = context_1 && context_1.id;
    return {
        setters: [
            function (Triggers_2) {
                Triggers = Triggers_2;
            },
            function (Triggers_1_1) {
                Triggers_1 = Triggers_1_1;
            },
            function (TypeExtensions_1_1) {
                TypeExtensions_1 = TypeExtensions_1_1;
            },
            function (PinInputDialog_1_1) {
                PinInputDialog_1 = PinInputDialog_1_1;
            }
        ],
        execute: function () {
            PreUnlockTerminalTrigger = (function (_super) {
                __extends(PreUnlockTerminalTrigger, _super);
                function PreUnlockTerminalTrigger() {
                    return _super !== null && _super.apply(this, arguments) || this;
                }
                PreUnlockTerminalTrigger.prototype.execute = function (options) {
                    var newOptions = TypeExtensions_1.ObjectExtensions.clone(options);
                    if (TypeExtensions_1.StringExtensions.endsWith(newOptions.grantType, "msr") || TypeExtensions_1.StringExtensions.endsWith(newOptions.grantType, "barcode")) {
                        return PinInputDialog_1.default.getPinNumber(this.context).then(function (result) {
                            if (!result.canceled) {
                                newOptions.extraParameters = {
                                    pin: result.pinNumber
                                };
                                return Promise.resolve(new Triggers_1.CancelableTriggerResult(false, newOptions));
                            }
                            else {
                                return Promise.resolve(new Triggers_1.CancelableTriggerResult(true, newOptions));
                            }
                        });
                    }
                    else {
                        return Promise.resolve(new Triggers_1.CancelableTriggerResult(false, newOptions));
                    }
                };
                return PreUnlockTerminalTrigger;
            }(Triggers.PreUnlockTerminalTrigger));
            exports_1("default", PreUnlockTerminalTrigger);
        }
    };
});
//# sourceMappingURL=D:/a/_work/r1/a/r/src/ExtendedLogon/Pos/Triggers/PreUnlockTerminalTrigger.js.map