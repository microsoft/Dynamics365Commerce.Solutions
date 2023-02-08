System.register(["PosApi/Consume/FiscalIntegration", "PosApi/Entities", "PosApi/TypeExtensions", "../../Infrastructure/Http/HttpRequestError", "../../Infrastructure/HttpRequestErrorConverter", "./Configuration", "./EfrHttpApi"], function (exports_1, context_1) {
    "use strict";
    var FiscalIntegration_1, Entities_1, TypeExtensions_1, HttpRequestError_1, HttpRequestErrorConverter_1, Configuration_1, EfrHttpApi_1, EfrFiscalConnector;
    var __moduleName = context_1 && context_1.id;
    return {
        setters: [
            function (FiscalIntegration_1_1) {
                FiscalIntegration_1 = FiscalIntegration_1_1;
            },
            function (Entities_1_1) {
                Entities_1 = Entities_1_1;
            },
            function (TypeExtensions_1_1) {
                TypeExtensions_1 = TypeExtensions_1_1;
            },
            function (HttpRequestError_1_1) {
                HttpRequestError_1 = HttpRequestError_1_1;
            },
            function (HttpRequestErrorConverter_1_1) {
                HttpRequestErrorConverter_1 = HttpRequestErrorConverter_1_1;
            },
            function (Configuration_1_1) {
                Configuration_1 = Configuration_1_1;
            },
            function (EfrHttpApi_1_1) {
                EfrHttpApi_1 = EfrHttpApi_1_1;
            }
        ],
        execute: function () {
            EfrFiscalConnector = (function () {
                function EfrFiscalConnector(runtime, logger) {
                    this.runtime = runtime;
                    this.logger = logger;
                }
                EfrFiscalConnector.prototype.initialize = function (request) {
                    this.logger.logVerbose("[FiscalConnectorSample] Skipping EFR initialization.", request.correlationId);
                    return Promise.resolve({
                        canceled: false,
                        data: new FiscalIntegration_1.FiscalRegisterInitializeClientResponse({
                            communicationResultType: Entities_1.ClientEntities.FiscalIntegration.FiscalRegisterCommunicationResultType.None,
                            response: TypeExtensions_1.StringExtensions.EMPTY,
                            fiscalRegisterInfo: TypeExtensions_1.StringExtensions.EMPTY
                        })
                    });
                };
                EfrFiscalConnector.prototype.isReady = function (request) {
                    var _this = this;
                    this.logger.logVerbose("[FiscalConnectorSample] Checking if EFR is ready.", request.correlationId);
                    return Promise.resolve().then(function () {
                        var configuration = Configuration_1.Configuration.read(request.technicalProfile.TechnicalProfile);
                        var api = new EfrHttpApi_1.EfrHttpApi(_this.runtime, _this.logger, configuration);
                        return api.state();
                    }).then(function (isReady) {
                        _this.logger.logVerbose("[FiscalConnectorSample] Successfully checked the state of EFR.", request.correlationId);
                        return new FiscalIntegration_1.FiscalRegisterIsReadyClientResponse(isReady);
                    }).catch(function (reason) {
                        _this.logger.logError("[FiscalConnectorSample] There was an error checking the state of EFR.", request.correlationId, reason.message);
                        return new FiscalIntegration_1.FiscalRegisterIsReadyClientResponse(false);
                    }).then(function (response) {
                        return {
                            canceled: false,
                            data: response
                        };
                    });
                };
                EfrFiscalConnector.prototype.submitDocument = function (request) {
                    var _this = this;
                    this.logger.logVerbose("[FiscalConnectorSample] Submitting a document to EFR.", request.correlationId);
                    return Promise.resolve().then(function () {
                        var configuration = Configuration_1.Configuration.read(request.document.FiscalConnectorTechnicalProfile.TechnicalProfile);
                        var api = new EfrHttpApi_1.EfrHttpApi(_this.runtime, _this.logger, configuration);
                        return api.register(request.document.Document);
                    }).then(function (registerResponse) {
                        _this.logger.logVerbose("[FiscalConnectorSample] Successfully submitted the document to EFR.", request.correlationId);
                        if (!TypeExtensions_1.StringExtensions.isNullOrWhitespace(registerResponse.warning)) {
                            _this.logger.logWarning("[FiscalConnectorSample]", request.correlationId, registerResponse.warning);
                        }
                        return new FiscalIntegration_1.FiscalRegisterSubmitDocumentClientResponse(registerResponse);
                    }).catch(function (reason) {
                        _this.logger.logError("[FiscalConnectorSample] There was an error submitting the document to EFR.", request.correlationId, reason.message);
                        var failureDetails = {
                            isRetryAllowed: true,
                            failureType: Entities_1.ClientEntities.FiscalIntegration.FiscalRegisterFailureType.Other,
                            errorMessage: reason.message
                        };
                        if (reason instanceof HttpRequestError_1.HttpRequestError) {
                            HttpRequestErrorConverter_1.HttpRequestErrorConverter.populateFailureDetails(failureDetails, reason);
                        }
                        return new FiscalIntegration_1.FiscalRegisterSubmitDocumentClientResponse({
                            communicationResultType: Entities_1.ClientEntities.FiscalIntegration.FiscalRegisterCommunicationResultType.Failed,
                            response: TypeExtensions_1.StringExtensions.EMPTY,
                            fiscalRegisterInfo: TypeExtensions_1.StringExtensions.EMPTY,
                            failureDetails: failureDetails
                        });
                    }).then(function (response) {
                        return {
                            canceled: false,
                            data: response
                        };
                    });
                };
                return EfrFiscalConnector;
            }());
            exports_1("EfrFiscalConnector", EfrFiscalConnector);
        }
    };
});
//# sourceMappingURL=D:/a/_work/r1/a/r/src/FiscalIntegration/PosFiscalConnectorSample/Pos.Extension/Connectors/Efr/EfrFiscalConnector.js.map