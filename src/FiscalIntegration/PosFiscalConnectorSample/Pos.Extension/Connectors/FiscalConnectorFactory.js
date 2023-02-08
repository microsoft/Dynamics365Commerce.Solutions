System.register(["./Efr/EfrFiscalConnector"], function (exports_1, context_1) {
    "use strict";
    var EfrFiscalConnector_1, FiscalConnectorFactory;
    var __moduleName = context_1 && context_1.id;
    return {
        setters: [
            function (EfrFiscalConnector_1_1) {
                EfrFiscalConnector_1 = EfrFiscalConnector_1_1;
            }
        ],
        execute: function () {
            FiscalConnectorFactory = (function () {
                function FiscalConnectorFactory() {
                }
                FiscalConnectorFactory.getFiscalConnector = function (connectorName, runtime, logger) {
                    switch (connectorName) {
                        case "EFRSample":
                            return new EfrFiscalConnector_1.EfrFiscalConnector(runtime, logger);
                        default:
                            return null;
                    }
                };
                return FiscalConnectorFactory;
            }());
            exports_1("FiscalConnectorFactory", FiscalConnectorFactory);
        }
    };
});
//# sourceMappingURL=D:/a/_work/r1/a/r/src/FiscalIntegration/PosFiscalConnectorSample/Pos.Extension/Connectors/FiscalConnectorFactory.js.map