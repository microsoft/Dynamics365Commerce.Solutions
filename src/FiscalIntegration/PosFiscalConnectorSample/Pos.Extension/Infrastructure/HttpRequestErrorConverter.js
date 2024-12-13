System.register(["PosApi/Entities", "PosApi/TypeExtensions", "./Http/HttpStatusCodes"], function (exports_1, context_1) {
    "use strict";
    var Entities_1, TypeExtensions_1, HttpStatusCodes_1, HttpRequestErrorConverter;
    var __moduleName = context_1 && context_1.id;
    return {
        setters: [
            function (Entities_1_1) {
                Entities_1 = Entities_1_1;
            },
            function (TypeExtensions_1_1) {
                TypeExtensions_1 = TypeExtensions_1_1;
            },
            function (HttpStatusCodes_1_1) {
                HttpStatusCodes_1 = HttpStatusCodes_1_1;
            }
        ],
        execute: function () {
            HttpRequestErrorConverter = (function () {
                function HttpRequestErrorConverter() {
                }
                HttpRequestErrorConverter.populateFailureDetails = function (failureDetails, requestError) {
                    var failureType;
                    var response = requestError.response;
                    var errorCode;
                    var errorMessage;
                    if (!TypeExtensions_1.ObjectExtensions.isNullOrUndefined(response)) {
                        errorCode = response.statusCode.toString();
                        errorMessage = response.statusText;
                        if (response.statusCode === HttpStatusCodes_1.HttpStatusCodes.TIMEOUT) {
                            failureType = Entities_1.ClientEntities.FiscalIntegration.FiscalRegisterFailureType.Timeout;
                        }
                    }
                    else {
                        failureType = Entities_1.ClientEntities.FiscalIntegration.FiscalRegisterFailureType.Other;
                        errorMessage = requestError.message;
                    }
                    failureDetails.failureType = failureType;
                    failureDetails.errorCode = errorCode;
                    failureDetails.errorMessage = errorMessage;
                };
                return HttpRequestErrorConverter;
            }());
            exports_1("HttpRequestErrorConverter", HttpRequestErrorConverter);
        }
    };
});
//# sourceMappingURL=D:/a/_work/r1/a/r/src/FiscalIntegration/PosFiscalConnectorSample/Pos.Extension/Infrastructure/HttpRequestErrorConverter.js.map