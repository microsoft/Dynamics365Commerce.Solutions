/**
 * SAMPLE CODE NOTICE
 *
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

import { ObjectExtensions } from "PosApi/TypeExtensions";

/**
 * Represents the configuration from the fiscal integration technical profile.
 */
export class Configuration {
    /**
     * The endpoint address.
     */
    public readonly endpointAddress: URL;

    /**
     * The HTTP request timeout.
     */
    public readonly timeout: number;

    /**
     * The flag indicating whether or not it is required to show the message from EFR to the user.
     */
    public readonly showUserNotificationMessage: boolean;

    /**
     * Creates a new instance of {@link Configuration} class.
     * @param {string} endpointAddress The endpoint address.
     * @param {number} timeout The HTTP request timeout.
     * @param {boolean} showUserNotificationMessage The flag indicating whether or not it is required to show the message from EFR to the user.
     */
    public constructor(endpointAddress: string, timeout: number, showUserNotificationMessage: boolean) {
        this.endpointAddress = Configuration._convertToUrl(endpointAddress);
        this.timeout = timeout;
        this.showUserNotificationMessage = showUserNotificationMessage;
    }

    /**
     * Reads the configuration from XML.
     * @param {string} xmlString The configuration XML string.
     * @returns {Configuration} The configuration object.
     */
    public static read(xmlString: string): Configuration {
        let parser: DOMParser = new DOMParser();
        let xml: Document = parser.parseFromString(xmlString, "text/xml");
        let properties: NodeListOf<Element> = xml.querySelectorAll("ConfigurationProperty");
        return new Configuration(
            this._getStringValue("ConnectorConnectionInfo", "EndPointAddress", properties),
            this._getIntegerValue("ConnectorSettingsInfo", "Timeout", properties),
            this._getBooleanValue("ConnectorSettingsInfo", "ShowUserNotificationMessage", properties)
        );
    }

    private static _getStringValue(namespace: string, name: string, properties: NodeListOf<Element>): string {
        return this._getPropertyValue(namespace, name, properties, "StringValue");
    }

    private static _getIntegerValue(namespace: string, name: string, properties: NodeListOf<Element>): number {
        let text: string = this._getPropertyValue(namespace, name, properties, "IntegerValue");
        return parseInt(text, 10);
    }

    private static _getBooleanValue(namespace: string, name: string, properties: NodeListOf<Element>): boolean {
        let text: string = this._getPropertyValue(namespace, name, properties, "BooleanValue");
        return text.toLowerCase() === "true";
    }

    private static _getPropertyValue(namespace: string, name: string, properties: NodeListOf<Element>, valueType: PropertyValueType) {
        let propertyElement: Element = this._findProperty(namespace, name, properties);
        if (ObjectExtensions.isNullOrUndefined(propertyElement)) {
            throw new Error(`Property ${namespace}.${name} not found in the configuration.`);
        }

        let propertyValueElement: Element = propertyElement.querySelector(valueType);
        if (ObjectExtensions.isNullOrUndefined(propertyValueElement)) {
            throw new Error(`Property ${namespace}.${name} does not have ${valueType}.`);
        }

        return propertyValueElement.textContent;
    }

    private static _findProperty(namespace: string, name: string, properties: NodeListOf<Element>): Element {
        let result: Element = null;
        for (let i: number = 0; i < properties.length; i++) {
            let property: Element = properties[i];
            if (property.querySelector("Namespace").textContent === namespace &&
                property.querySelector("Name").textContent === name) {
                result = property;
                break;
            }
        }
        return result;
    }

    private static _convertToUrl(urlString: string): URL {
        let url: URL = new URL(urlString);
        if (url.protocol === "http:") {
            throw new Error("HTTP is not supported. Please configure EFR to use HTTPS. Refer to 'EFR Reference' for details.");
        }
        return url;
    }
}

type PropertyValueType = "StringValue" | "IntegerValue" | "BooleanValue";