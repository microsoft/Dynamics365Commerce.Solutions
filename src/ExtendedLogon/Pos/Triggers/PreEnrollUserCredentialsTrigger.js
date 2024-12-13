System.register(["PosApi/Extend/Triggers/ApplicationTriggers", "PosApi/Extend/Triggers/Triggers", "../Controls/PinInputDialog"], function (exports_1, context_1) {
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
    var Triggers, Triggers_1, PinInputDialog_1, PreEnrollUserCredentialsTrigger;
    var __moduleName = context_1 && context_1.id;
    return {
        setters: [
            function (Triggers_2) {
                Triggers = Triggers_2;
            },
            function (Triggers_1_1) {
                Triggers_1 = Triggers_1_1;
            },
            function (PinInputDialog_1_1) {
                PinInputDialog_1 = PinInputDialog_1_1;
            }
        ],
        execute: function () {
            PreEnrollUserCredentialsTrigger = (function (_super) {
                __extends(PreEnrollUserCredentialsTrigger, _super);
                function PreEnrollUserCredentialsTrigger() {
                    return _super !== null && _super.apply(this, arguments) || this;
                }
                PreEnrollUserCredentialsTrigger.prototype.execute = function (options) {
                    return PinInputDialog_1.default.getPinNumber(this.context).then(function (result) {
                        if (!result.canceled) {
                            options.extraParameters = { pin: result.pinNumber };
                            return Promise.resolve(new Triggers_1.CancelableTriggerResult(false, options));
                        }
                        else {
                            return Promise.resolve(new Triggers_1.CancelableTriggerResult(true, options));
                        }
                    });
                };
                return PreEnrollUserCredentialsTrigger;
            }(Triggers.PreEnrollUserCredentialsTrigger));
            exports_1("default", PreEnrollUserCredentialsTrigger);
        }
    };
});
//# sourceMappingURL=D:/a/_work/r1/a/r/src/ExtendedLogon/Pos/Triggers/PreEnrollUserCredentialsTrigger.js.map