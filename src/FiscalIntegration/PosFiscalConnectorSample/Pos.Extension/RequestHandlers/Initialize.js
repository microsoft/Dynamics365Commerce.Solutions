System.register(["PosApi/Extend/RequestHandlers/FiscalIntegrationRequestHandlers", "PosApi/Entities", "../Connectors/FiscalConnectorFactory"], function (exports_1, context_1) {
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
    var FiscalIntegrationRequestHandlers_1, Entities_1, FiscalConnectorFactory_1, FiscalRegisterInitializeClientRequestHandlerExt;
    var __moduleName = context_1 && context_1.id;
    return {
        setters: [
            function (FiscalIntegrationRequestHandlers_1_1) {
                FiscalIntegrationRequestHandlers_1 = FiscalIntegrationRequestHandlers_1_1;
            },
            function (Entities_1_1) {
                Entities_1 = Entities_1_1;
            },
            function (FiscalConnectorFactory_1_1) {
                FiscalConnectorFactory_1 = FiscalConnectorFactory_1_1;
            }
        ],
        execute: function () {
            FiscalRegisterInitializeClientRequestHandlerExt = (function (_super) {
                __extends(FiscalRegisterInitializeClientRequestHandlerExt, _super);
                function FiscalRegisterInitializeClientRequestHandlerExt() {
                    return _super !== null && _super.apply(this, arguments) || this;
                }
                FiscalRegisterInitializeClientRequestHandlerExt.prototype.executeAsync = function (request) {
                    if (request.technicalProfile.ConnectorLocationValue !== Entities_1.ProxyEntities.FiscalIntegrationConnectorLocation.Pos) {
                        return this.defaultExecuteAsync(request);
                    }
                    var fiscalConnector = FiscalConnectorFactory_1.FiscalConnectorFactory.getFiscalConnector(request.technicalProfile.ConnectorName, this.context.runtime, this.context.logger);
                    if (fiscalConnector === null) {
                        return this.defaultExecuteAsync(request);
                    }
                    return fiscalConnector.initialize(request);
                };
                return FiscalRegisterInitializeClientRequestHandlerExt;
            }(FiscalIntegrationRequestHandlers_1.FiscalRegisterInitializeClientRequestHandler));
            exports_1("default", FiscalRegisterInitializeClientRequestHandlerExt);
        }
    };
});
//# sourceMappingURL=D:/a/_work/r1/a/r/src/FiscalIntegration/PosFiscalConnectorSample/Pos.Extension/RequestHandlers/Initialize.js.map