namespace Contoso
{
    namespace Commerce.HardwareStation.PosnetThermalFVFiscalPrinterSample
    {
        using System;
        using System.Collections.Generic;
        using System.ComponentModel;
        using System.Linq;
        using System.Reflection;
        using System.Xml.Linq;

        /// <summary>
        /// Printer configuration controller.
        /// </summary>
        internal class ConfigurationController
        {
            private const string BooleanTypeName = "Boolean";
            private const string StringTypeName = "String";
            private const string DecimalTypeName = "Decimal";
            private const string IntegerTypeName = "Integer";

            private const string ValueTypeNodeName = "ValueType";
            private const string BooleanValueNodeName = "BooleanValue";
            private const string StringValueNodeName = "StringValue";
            private const string DecimalValueNodeName = "DecimalValue";
            private const string IntegerValueNodeName = "IntegerValue";

            /// <summary>
            /// Parses configuration.
            /// </summary>
            /// <param name="configurationString">The configuration string to be parsed.</param>
            /// <returns>The <see cref="ConfigurationModel"/> containing parsed values.</returns>
            internal ConfigurationModel ParseConfiguration(string configurationString)
            {
                ConfigurationModel printerConfiguration = new ConfigurationModel();

                XDocument configuration = XDocument.Parse(configurationString);

                string connectionStringName = nameof(printerConfiguration.ConnectionString);
                string printerTimeoutMillisecondsName = nameof(printerConfiguration.PrinterTimeoutMilliseconds);
                string synchronizeDateTimeName = nameof(printerConfiguration.SynchronizeDateTime);

                printerConfiguration.ConnectionString = this.GetPropertyValue(configuration, this.GetNamespaceValue(connectionStringName), this.GetNameValue(connectionStringName));

                string printerTimeoutMillisecondsString = this.GetPropertyValue(configuration, this.GetNamespaceValue(printerTimeoutMillisecondsName), this.GetNameValue(printerTimeoutMillisecondsName));
                printerConfiguration.PrinterTimeoutMilliseconds = Convert.ToInt32(printerTimeoutMillisecondsString);

                string synchronizeDateTimeString = this.GetPropertyValue(configuration, this.GetNamespaceValue(synchronizeDateTimeName), this.GetNameValue(synchronizeDateTimeName));
                printerConfiguration.SynchronizeDateTime = Convert.ToBoolean(synchronizeDateTimeString);

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
                var descendants = configuration.Descendants(ConfigurationElementConstants.RootElement).Descendants(ConfigurationElementConstants.PropertyElement);
                XElement element = GetElementByNamespaceAndName(descendants, namespaceValue, nameValue);
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
                string valueType = element.Element(ValueTypeNodeName).Value;
                switch (valueType)
                {
                    case BooleanTypeName:
                        value = element.Element(BooleanValueNodeName).Value;
                        break;
                    case StringTypeName:
                        value = element.Element(StringValueNodeName).Value;
                        break;
                    case DecimalTypeName:
                        value = element.Element(DecimalValueNodeName).Value;
                        break;
                    case IntegerTypeName:
                        value = element.Element(IntegerValueNodeName).Value;
                        break;
                    default:
                        value = null;
                        break;
                }

                return value;
            }

            /// <summary>
            /// Gets the <see cref="XElement"/> from the collection by its namespace and name.
            /// </summary>
            /// <param name="elements">The collection of <see cref="XElement"/>.</param>
            /// <param name="namespaceValue">The namespace name.</param>
            /// <param name="nameValue">The name of the element.</param>
            /// <returns>The <see cref="XElement"/> with the specified namespace and name.</returns>
            private static XElement GetElementByNamespaceAndName(IEnumerable<XElement> elements, string namespaceValue, string nameValue)
            {
                if (!elements.Any())
                {
                    throw new ArgumentException($"The {nameof(elements)} collection is empty!");
                }

                return elements.Single(a => a.Element(ConfigurationElementConstants.NamespaceElement).Value == namespaceValue &&
                    a.Element(ConfigurationElementConstants.NameElement).Value == nameValue);
            }
        }
    }
}
