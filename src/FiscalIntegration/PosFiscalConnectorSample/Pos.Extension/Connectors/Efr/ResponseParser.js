System.register(["PosApi/Entities", "PosApi/TypeExtensions", "./RegistrationResultCodes"], function (exports_1, context_1) {
    "use strict";
    var Entities_1, TypeExtensions_1, RegistrationResultCodes_1, ResponseParser;
    var __moduleName = context_1 && context_1.id;
    return {
        setters: [
            function (Entities_1_1) {
                Entities_1 = Entities_1_1;
            },
            function (TypeExtensions_1_1) {
                TypeExtensions_1 = TypeExtensions_1_1;
            },
            function (RegistrationResultCodes_1_1) {
                RegistrationResultCodes_1 = RegistrationResultCodes_1_1;
            }
        ],
        execute: function () {
            ResponseParser = (function () {
                function ResponseParser() {
                }
                ResponseParser.parseSubmitDocumentResponse = function (xmlString, includeUserNotificationMessage) {
                    var result = {
                        communicationResultType: Entities_1.ClientEntities.FiscalIntegration.FiscalRegisterCommunicationResultType.None,
                        response: xmlString,
                        userNotificationMessage: null,
                        failureDetails: null,
                        fiscalRegisterInfo: TypeExtensions_1.StringExtensions.EMPTY,
                        warning: null
                    };
                    try {
                        var parser = new DOMParser();
                        var xml = parser.parseFromString(xmlString, "text/xml");
                        var resultElement = xml.querySelector("Result");
                        var resultCode = resultElement.getAttribute("RC");
                        var errorCodeNode = resultElement.querySelector("ErrorCode");
                        var errorCode = null;
                        if (errorCodeNode) {
                            errorCode = errorCodeNode.textContent;
                        }
                        switch (resultCode) {
                            case RegistrationResultCodes_1.RegistrationResultCodes.TransactionProcessedSuccessfully:
                                result.communicationResultType = Entities_1.ClientEntities.FiscalIntegration.FiscalRegisterCommunicationResultType.Succeeded;
                                break;
                            case RegistrationResultCodes_1.RegistrationResultCodes.CouldNotProcessTransaction:
                                result.communicationResultType = Entities_1.ClientEntities.FiscalIntegration.FiscalRegisterCommunicationResultType.Failed;
                                result.failureDetails = {
                                    isRetryAllowed: true,
                                    failureType: Entities_1.ClientEntities.FiscalIntegration.FiscalRegisterFailureType.Other,
                                    errorCode: errorCode
                                };
                                break;
                            case RegistrationResultCodes_1.RegistrationResultCodes.InvalidRequestData:
                                result.communicationResultType = Entities_1.ClientEntities.FiscalIntegration.FiscalRegisterCommunicationResultType.Failed;
                                result.failureDetails = {
                                    isRetryAllowed: false,
                                    failureType: Entities_1.ClientEntities.FiscalIntegration.FiscalRegisterFailureType.Other,
                                    errorCode: errorCode
                                };
                                break;
                        }
                        if (includeUserNotificationMessage) {
                            var userMessageNode = resultElement.querySelector("UserMessage");
                            if (userMessageNode) {
                                result.userNotificationMessage = userMessageNode.textContent;
                            }
                        }
                        var warningNode = resultElement.querySelector("Warning");
                        if (warningNode) {
                            result.warning = warningNode.textContent;
                        }
                    }
                    catch (error) {
                        result.communicationResultType = Entities_1.ClientEntities.FiscalIntegration.FiscalRegisterCommunicationResultType.Failed;
                        result.failureDetails = {
                            isRetryAllowed: true,
                            failureType: Entities_1.ClientEntities.FiscalIntegration.FiscalRegisterFailureType.BadResponse,
                            errorMessage: error.message
                        };
                    }
                    return result;
                };
                return ResponseParser;
            }());
            exports_1("ResponseParser", ResponseParser);
        }
    };
});
//# sourceMappingURL=D:/a/_work/r1/a/r/src/FiscalIntegration/PosFiscalConnectorSample/Pos.Extension/Connectors/Efr/ResponseParser.js.map