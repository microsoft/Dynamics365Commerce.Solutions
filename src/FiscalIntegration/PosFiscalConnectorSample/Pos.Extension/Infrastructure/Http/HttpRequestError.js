System.register([], function (exports_1, context_1) {
    "use strict";
    var __extends = (this && this.__extends) || (function () {
        var extendStatics = function (d, b) {
            extendStatics = Object.setPrototypeOf ||
                ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
                function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
            return extendStatics(d, b);
        };
        return function (d, b) {
            extendStatics(d, b);
            function __() { this.constructor = d; }
            d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
        };
    })();
    var HttpRequestError;
    var __moduleName = context_1 && context_1.id;
    return {
        setters: [],
        execute: function () {
            HttpRequestError = (function (_super) {
                __extends(HttpRequestError, _super);
                function HttpRequestError(message, response) {
                    var _this = _super.call(this, message) || this;
                    _this.__proto__ = HttpRequestError.prototype;
                    _this.response = response;
                    return _this;
                }
                return HttpRequestError;
            }(Error));
            exports_1("HttpRequestError", HttpRequestError);
        }
    };
});
//# sourceMappingURL=D:/a/_work/r1/a/r/src/FiscalIntegration/PosFiscalConnectorSample/Pos.Extension/Infrastructure/Http/HttpRequestError.js.map