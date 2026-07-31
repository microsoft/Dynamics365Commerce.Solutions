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
    namespace HardwareStation.Connector.EpsonFP90IIISample
    {
        using System;
        using System.ComponentModel;
        using System.Linq;
        using System.Reflection;
        using System.Xml.Linq;

        /// <summary>
        /// Printer configuration controller.
        /// </summary>
        internal class ConfigurationController
        {
            /// <summary>
            /// Parses configuration.
            /// </summary>
            /// <param name="configurationString"></param>
            /// <returns></returns>
            internal ConfigurationModel ParseConfiguration(string configurationString)
            {
                ConfigurationModel printerConfiguration = new ConfigurationModel();

                XDocument configuration = XDocument.Parse(configurationString);

                string endPointAddressName = nameof(printerConfiguration.EndPointAddress);
                printerConfiguration.EndPointAddress = this.GetPropertyValue(configuration, this.GetNamespaceValue(endPointAddressName), this.GetNameValue(endPointAddressName));
                string synchronizeDateTime = nameof(printerConfiguration.SynchronizeDateTime);
                printerConfiguration.SynchronizeDateTime = Convert.ToBoolean(this.GetPropertyValue(configuration, this.GetNamespaceValue(synchronizeDateTime), GetNameValue(synchronizeDateTime)));
                return printerConfiguration;
            }

            /// <summary>
            /// Gets the name value of the <c>ConfigurationAttribute</c>.
            /// </summary>
            /// <param name="memberName">The name of the class member.</param>
            /// <returns>The name value.</returns>
            private string GetNameValue(string memberName)
            {
                MemberInfo mi = typeof(ConfigurationModel).GetMember(memberName).Single();
                ConfigurationAttribute configurationAttribute = Attribute.GetCustomAttribute(mi, typeof(ConfigurationAttribute)) as ConfigurationAttribute;

                return configurationAttribute.NameValue;
            }

            /// <summary>
            /// Gest the namespace value of the <c>ConfigurationAttribute</c>.
            /// </summary>
            /// <param name="memberName">The name of the class member.</param>
            /// <returns>The namespace value.</returns>
            private string GetNamespaceValue(string memberName)
            {
                string namespaceValue;

                MemberInfo mi = typeof(ConfigurationModel).GetMember(memberName).Single();
                ConfigurationAttribute configurationAttribute = Attribute.GetCustomAttribute(mi, typeof(ConfigurationAttribute)) as ConfigurationAttribute;
                ConfigurationNamespaceType namespaceType = configurationAttribute.NamespaceType;
                FieldInfo fi = namespaceType.GetType().GetField(namespaceType.ToString());
                DescriptionAttribute[] attrs = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
                namespaceValue = attrs.Length > 0 ? attrs[0].Description : string.Empty;

                return namespaceValue;
            }

            /// <summary>
            /// Gets the value of the proprty.
            /// </summary>
            /// <param name="configuration">The configuration.</param>
            /// <param name="namespaceValue">The namespace element value.</param>
            /// <param name="nameValue">The name element value.</param>
            /// <returns></returns>
            private string GetPropertyValue(XDocument configuration, string namespaceValue, string nameValue)
            {
                XElement element = configuration.Descendants(ConfigurationElementConstants.RootElement).Descendants(ConfigurationElementConstants.PropertyElement)
                                .Single(a => a.Element(ConfigurationElementConstants.NamespaceElement).Value == namespaceValue && a.Element(ConfigurationElementConstants.NameElement).Value == nameValue);
                return this.ReadValue(element);
            }

            /// <summary>
            /// Reads value for the configuration property.
            /// </summary>
            /// <param name="element">The configuration property element.</param>
            /// <returns>The value of the property.</returns>
            private string ReadValue(XElement element)
            {
                string value;
                string valueType = element.Element("ValueType").Value;
                switch (valueType)
                {
                    case "Boolean":
                        value = element.Element("BooleanValue").Value;
                        break;
                    case "String":
                        value = element.Element("StringValue").Value;
                        break;
                    case "Decimal":
                        value = element.Element("DecimalValue").Value;
                        break;
                    case "Integer":
                        value = element.Element("IntegerValue").Value;
                        break;
                    default:
                        value = null;
                        break;
                }

                return value;
            }
        }
    }
}
