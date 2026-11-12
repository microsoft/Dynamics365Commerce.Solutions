System.register(["../../Messages/ExecuteHttpRequestRequest"], function (exports_1, context_1) {
    "use strict";
    var ExecuteHttpRequestRequest_1, HttpApi;
    var __moduleName = context_1 && context_1.id;
    return {
        setters: [
            function (ExecuteHttpRequestRequest_1_1) {
                ExecuteHttpRequestRequest_1 = ExecuteHttpRequestRequest_1_1;
            }
        ],
        execute: function () {
            HttpApi = (function () {
                function HttpApi(runtime, logger) {
                    this.runtime = runtime;
                    this.logger = logger;
                }
                HttpApi.prototype._executeHttpRequest = function (httpRequest) {
                    return this.runtime.executeAsync(new ExecuteHttpRequestRequest_1.ExecuteHttpRequestRequest(this.logger.getNewCorrelationId(), httpRequest))
                        .then(function (result) {
                        if (result.canceled) {
                            throw new Error("The request was canceled.");
                        }
                        return result.data.httpResponse;
                    });
                };
                return HttpApi;
            }());
            exports_1("HttpApi", HttpApi);
        }
    };
});
//# sourceMappingURL=C:/__w/1/s/r/src/FiscalIntegration/PosFiscalConnectorSample/Pos.Extension/Infrastructure/Http/HttpApi.js.map