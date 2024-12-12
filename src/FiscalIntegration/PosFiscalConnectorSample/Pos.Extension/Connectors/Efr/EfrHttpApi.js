System.register(["../../Infrastructure/Http/HttpApi", "../../Infrastructure/Http/HttpMethods", "./ResponseParser"], function (exports_1, context_1) {
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
    var HttpApi_1, HttpMethods_1, ResponseParser_1, EfrHttpApi;
    var __moduleName = context_1 && context_1.id;
    return {
        setters: [
            function (HttpApi_1_1) {
                HttpApi_1 = HttpApi_1_1;
            },
            function (HttpMethods_1_1) {
                HttpMethods_1 = HttpMethods_1_1;
            },
            function (ResponseParser_1_1) {
                ResponseParser_1 = ResponseParser_1_1;
            }
        ],
        execute: function () {
            EfrHttpApi = (function (_super) {
                __extends(EfrHttpApi, _super);
                function EfrHttpApi(runtime, logger, configuration) {
                    var _this = _super.call(this, runtime, logger) || this;
                    _this.configuration = configuration;
                    return _this;
                }
                EfrHttpApi.prototype.state = function () {
                    var httpRequest = {
                        method: HttpMethods_1.HttpMethods.GET,
                        requestUri: new URL("state", this.configuration.endpointAddress.toString()).toString(),
                        timeout: this.configuration.timeout
                    };
                    return this._executeHttpRequest(httpRequest).then(function (response) {
                        return response.ok;
                    });
                };
                EfrHttpApi.prototype.register = function (documentXmlString) {
                    var _this = this;
                    var httpRequest = {
                        method: HttpMethods_1.HttpMethods.POST,
                        requestUri: new URL("register", this.configuration.endpointAddress.toString()).toString(),
                        timeout: this.configuration.timeout,
                        body: documentXmlString
                    };
                    return this._executeHttpRequest(httpRequest).then(function (response) {
                        return ResponseParser_1.ResponseParser.parseSubmitDocumentResponse(response.body, _this.configuration.showUserNotificationMessage);
                    });
                };
                return EfrHttpApi;
            }(HttpApi_1.HttpApi));
            exports_1("EfrHttpApi", EfrHttpApi);
        }
    };
});
//# sourceMappingURL=C:/__w/1/s/r/src/FiscalIntegration/PosFiscalConnectorSample/Pos.Extension/Connectors/Efr/EfrHttpApi.js.map