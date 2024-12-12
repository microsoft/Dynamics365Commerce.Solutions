System.register(["PosApi/Create/RequestHandlers"], function (exports_1, context_1) {
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
    var RequestHandlers_1, ExecuteHttpRequestResponse;
    var __moduleName = context_1 && context_1.id;
    return {
        setters: [
            function (RequestHandlers_1_1) {
                RequestHandlers_1 = RequestHandlers_1_1;
            }
        ],
        execute: function () {
            ExecuteHttpRequestResponse = (function (_super) {
                __extends(ExecuteHttpRequestResponse, _super);
                function ExecuteHttpRequestResponse(httpResponse) {
                    var _this = _super.call(this) || this;
                    _this.httpResponse = httpResponse;
                    return _this;
                }
                return ExecuteHttpRequestResponse;
            }(RequestHandlers_1.Response));
            exports_1("ExecuteHttpRequestResponse", ExecuteHttpRequestResponse);
        }
    };
});
//# sourceMappingURL=C:/__w/1/s/r/src/FiscalIntegration/PosFiscalConnectorSample/Pos.Extension/Messages/ExecuteHttpRequestResponse.js.map