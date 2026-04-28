namespace Contoso
{
    namespace Commerce.HardwareStation.PosnetThermalFVFiscalPrinterSample
    {
        using System;

        /// <summary>
        /// Represents the attribute for printer configuration.
        /// </summary>
        internal sealed class ConfigurationAttribute : Attribute
        {
            /// <summary>
            /// Configuration attribute.
            /// </summary>
            /// <param name="namespaceType">The type of namespace.</param>
            /// <param name="nameValue">The configuration attribute name value.</param>
            internal ConfigurationAttribute(ConfigurationNamespaceType namespaceType, string nameValue)
            {
                this.NamespaceType = namespaceType;
                this.NameValue = nameValue;
            }

            /// <summary>
            /// Gets or sets the name value.
            /// </summary>
            internal string NameValue { get; set; }

            /// <summary>
            /// Gets or sets type of the namespace.
            /// </summary>
            internal ConfigurationNamespaceType NamespaceType { get; set; }
        }
    }
}