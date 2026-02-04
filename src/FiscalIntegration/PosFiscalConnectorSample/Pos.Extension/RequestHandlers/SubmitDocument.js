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
    var FiscalIntegrationRequestHandlers_1, Entities_1, FiscalConnectorFactory_1, FiscalRegisterSubmitDocumentClientRequestHandlerExt;
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
            FiscalRegisterSubmitDocumentClientRequestHandlerExt = (function (_super) {
                __extends(FiscalRegisterSubmitDocumentClientRequestHandlerExt, _super);
                function FiscalRegisterSubmitDocumentClientRequestHandlerExt() {
                    return _super !== null && _super.apply(this, arguments) || this;
                }
                FiscalRegisterSubmitDocumentClientRequestHandlerExt.prototype.executeAsync = function (request) {
                    var technicalProfile = request.document.FiscalConnectorTechnicalProfile;
                    if (technicalProfile.ConnectorLocationValue !== Entities_1.ProxyEntities.FiscalIntegrationConnectorLocation.Pos) {
                        return this.defaultExecuteAsync(request);
                    }
                    var fiscalConnector = FiscalConnectorFactory_1.FiscalConnectorFactory.getFiscalConnector(request.document.FiscalConnectorTechnicalProfile.ConnectorName, this.context.runtime, this.context.logger);
                    if (fiscalConnector === null) {
                        return this.defaultExecuteAsync(request);
                    }
                    return fiscalConnector.submitDocument(request);
                };
                return FiscalRegisterSubmitDocumentClientRequestHandlerExt;
            }(FiscalIntegrationRequestHandlers_1.FiscalRegisterSubmitDocumentClientRequestHandler));
            exports_1("default", FiscalRegisterSubmitDocumentClientRequestHandlerExt);
        }
    };
});
//# sourceMappingURL=C:/__w/1/s/r/src/FiscalIntegration/PosFiscalConnectorSample/Pos.Extension/RequestHandlers/SubmitDocument.js.map