System.register(["PosApi/TypeExtensions"], function (exports_1, context_1) {
    "use strict";
    var TypeExtensions_1, Configuration;
    var __moduleName = context_1 && context_1.id;
    return {
        setters: [
            function (TypeExtensions_1_1) {
                TypeExtensions_1 = TypeExtensions_1_1;
            }
        ],
        execute: function () {
            Configuration = (function () {
                function Configuration(endpointAddress, timeout, showUserNotificationMessage) {
                    this.endpointAddress = Configuration._convertToUrl(endpointAddress);
                    this.timeout = timeout;
                    this.showUserNotificationMessage = showUserNotificationMessage;
                }
                Configuration.read = function (xmlString) {
                    var parser = new DOMParser();
                    var xml = parser.parseFromString(xmlString, "text/xml");
                    var properties = xml.querySelectorAll("ConfigurationProperty");
                    return new Configuration(this._getStringValue("ConnectorConnectionInfo", "EndPointAddress", properties), this._getIntegerValue("ConnectorSettingsInfo", "Timeout", properties), this._getBooleanValue("ConnectorSettingsInfo", "ShowUserNotificationMessage", properties));
                };
                Configuration._getStringValue = function (namespace, name, properties) {
                    return this._getPropertyValue(namespace, name, properties, "StringValue");
                };
                Configuration._getIntegerValue = function (namespace, name, properties) {
                    var text = this._getPropertyValue(namespace, name, properties, "IntegerValue");
                    return parseInt(text, 10);
                };
                Configuration._getBooleanValue = function (namespace, name, properties) {
                    var text = this._getPropertyValue(namespace, name, properties, "BooleanValue");
                    return text.toLowerCase() === "true";
                };
                Configuration._getPropertyValue = function (namespace, name, properties, valueType) {
                    var propertyElement = this._findProperty(namespace, name, properties);
                    if (TypeExtensions_1.ObjectExtensions.isNullOrUndefined(propertyElement)) {
                        throw new Error("Property " + namespace + "." + name + " not found in the configuration.");
                    }
                    var propertyValueElement = propertyElement.querySelector(valueType);
                    if (TypeExtensions_1.ObjectExtensions.isNullOrUndefined(propertyValueElement)) {
                        throw new Error("Property " + namespace + "." + name + " does not have " + valueType + ".");
                    }
                    return propertyValueElement.textContent;
                };
                Configuration._findProperty = function (namespace, name, properties) {
                    var result = null;
                    for (var i = 0; i < properties.length; i++) {
                        var property = properties[i];
                        if (property.querySelector("Namespace").textContent === namespace &&
                            property.querySelector("Name").textContent === name) {
                            result = property;
                            break;
                        }
                    }
                    return result;
                };
                Configuration._convertToUrl = function (urlString) {
                    var url = new URL(urlString);
                    if (url.protocol === "http:") {
                        throw new Error("HTTP is not supported. Please configure EFR to use HTTPS. Refer to 'EFR Reference' for details.");
                    }
                    return url;
                };
                return Configuration;
            }());
            exports_1("Configuration", Configuration);
        }
    };
});
//# sourceMappingURL=C:/__w/1/s/r/src/FiscalIntegration/PosFiscalConnectorSample/Pos.Extension/Connectors/Efr/Configuration.js.map