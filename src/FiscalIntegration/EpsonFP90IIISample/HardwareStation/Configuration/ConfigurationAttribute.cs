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

        /// <summary>
        /// Represent the attribute for printer configuration.
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