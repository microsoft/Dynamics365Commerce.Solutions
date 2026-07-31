/**
* SAMPLE CODE NOTICE
* 
* THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
* OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
* THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
* NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

namespace Contoso
{
    namespace Commerce.HardwareStation.EFRSample
    {
        using System;
        using System.Linq;
        using System.Xml.Linq;
        using Contoso.CommerceRuntime.DocumentProvider.DataModelEFR.Constants;

        /// <summary>
        /// Reads the settings from fiscal connector configuration.
        /// </summary>
        public static class ConfigurationController
        {
            private const string EndpointAddressIsNotSpecifiedErrorMessage = "EFR endpoint address is not specified.";

            /// <summary>
            /// Gets the service endpoint address.
            /// </summary>
            /// <param name="configurationString">The configuration string.</param>
            /// <returns>The service endpoint address.</returns>
            public static string GetEndpointAddress(string configurationString)
            {
                var xmlElement = GetStringValueElement(configurationString,
                        ConfigurationElementConstants.ConnectorConnectionInfo,
                        ConfigurationElementConstants.EndPointAddress);
                if (xmlElement == null)
                {
                    throw new EFRException(EFRException.FiscalRegisterEndpointAddressConfigurationErrorResourceId, EndpointAddressIsNotSpecifiedErrorMessage);
                }
                return xmlElement.Value;
            }

            /// <summary>
            /// Gets the service timeout setting.
            /// </summary>
            /// <param name="configurationString">The configuration string.</param>
            /// <returns>The service timeout value.</returns>
            public static int GetTimeoutValue(string configurationString)
            {
                int timeoutValue = 0;
                var xmlElement = GetIntegerValueElement(configurationString, ConfigurationElementConstants.ConnectorSettingsInfo,
                        ConfigurationElementConstants.Timeout);
                if (xmlElement != null)
                {
                    System.Int32.TryParse(xmlElement.Value, out timeoutValue);
                }
                return timeoutValue;
            }

            /// <summary>
            /// Gets the flag indicating whether or not to show the user message from the service.
            /// </summary>
            /// <param name="configurationString">The configuration string.</param>
            /// <returns>The flag indicating whether or not to show the user message from the service.</returns>
            public static bool GetShowUserNotificationMessage(string configurationString)
            {
                bool result = false;
                var xmlElement = GetBooleanValueElement(
                    configurationString,
                    ConfigurationElementConstants.ConnectorSettingsInfo,
                    ConfigurationElementConstants.ShowUserNotificationMessage
                );

                if (xmlElement != null)
                {
                    Boolean.TryParse(xmlElement.Value, out result);
                }
                return result;
            }

            /// <summary>
            /// Find the StringValue element for specific name and namespace of the fiscal document provider configuration.
            /// </summary>
            /// <param name="document">The fiscal document provider configuration in functionality profile.</param>
            /// <param name="namespaceValue">The value of namespace element.</param>
            /// <param name="nameValue">The value of name element.</param>
            /// <returns>The StoredStringValue element.</returns>
            private static XElement GetStringValueElement(string document, string namespaceValue, string nameValue)
            {
                var configurationElement = XElement.Parse(document)
                    .Descendants(ConfigurationElementConstants.PropertyElement)
                    .SingleOrDefault(a =>
                            a.Element(ConfigurationElementConstants.NamespaceElement).Value == namespaceValue
                         && a.Element(ConfigurationElementConstants.NameElement).Value == nameValue);
                return configurationElement?.Descendants(ConfigurationElementConstants.StringValueElement).FirstOrDefault();
            }

            /// <summary>
            /// Find the IntegerValue element for specific name and namespace of the fiscal document provider configuration.
            /// </summary>
            /// <param name="document">The fiscal document provider configuration in functionality profile.</param>
            /// <param name="namespaceValue">The value of namespace element.</param>
            /// <param name="nameValue">The value of name element.</param>
            /// <returns>The StoredIntegerValue element.</returns>
            private static XElement GetIntegerValueElement(string document, string namespaceValue, string nameValue)
            {
                var configurationElement = XElement.Parse(document)
                    .Descendants(ConfigurationElementConstants.PropertyElement)
                    .SingleOrDefault(a =>
                            a.Element(ConfigurationElementConstants.NamespaceElement).Value == namespaceValue
                         && a.Element(ConfigurationElementConstants.NameElement).Value == nameValue);
                return configurationElement?.Descendants(ConfigurationElementConstants.IntegerValueElement).FirstOrDefault();
            }

            /// <summary>
            /// Find the BooleanValue element for specific name and namespace of the fiscal document provider configuration.
            /// </summary>
            /// <param name="document">The fiscal document provider configuration in functionality profile.</param>
            /// <param name="namespaceValue">The value of namespace element.</param>
            /// <param name="nameValue">The value of name element.</param>
            /// <returns>The BooleanValue element.</returns>
            private static XElement GetBooleanValueElement(string document, string namespaceValue, string nameValue)
            {
                var configurationElement = XElement.Parse(document)
                    .Descendants(ConfigurationElementConstants.PropertyElement)
                    .SingleOrDefault(a =>
                            a.Element(ConfigurationElementConstants.NamespaceElement).Value == namespaceValue
                         && a.Element(ConfigurationElementConstants.NameElement).Value == nameValue);
                return configurationElement?.Descendants(ConfigurationElementConstants.BooleanValueElement).FirstOrDefault();
            }
        }
    }
}
