System.register([], function (exports_1, context_1) {
    "use strict";
    var HttpStatusCodes;
    var __moduleName = context_1 && context_1.id;
    return {
        setters: [],
        execute: function () {
            HttpStatusCodes = (function () {
                function HttpStatusCodes() {
                }
                HttpStatusCodes.isSuccessful = function (statusCode) {
                    return statusCode >= HttpStatusCodes.OK && statusCode <= HttpStatusCodes.LAST_OK_STATUS_CODE;
                };
                HttpStatusCodes.OK = 200;
                HttpStatusCodes.TIMEOUT = 408;
                HttpStatusCodes.NO_CONTENT = 204;
                HttpStatusCodes.LAST_OK_STATUS_CODE = 299;
                return HttpStatusCodes;
            }());
            exports_1("HttpStatusCodes", HttpStatusCodes);
        }
    };
});
//# sourceMappingURL=D:/a/_work/r1/a/r/src/FiscalIntegration/PosFiscalConnectorSample/Pos.Extension/Infrastructure/Http/HttpStatusCodes.js.map