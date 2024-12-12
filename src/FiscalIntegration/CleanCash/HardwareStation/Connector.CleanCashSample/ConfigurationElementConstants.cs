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
    namespace HardwareStation.Connector.CleanCashSample
    {
        /// <summary>
        /// The elements for fiscal connector configuration.
        /// </summary>
        public static class ConfigurationElementConstants
        {
            /// <summary>
            /// The inner text of Namespace element for ConnectorConnectionInfo.
            /// </summary>
            public const string ConnectorConnectionInfo = "ConnectorConnectionInfo";

            /// <summary>
            /// The name of the name element.
            /// </summary>
            public const string NameElement = "Name";

            /// <summary>
            /// The name of the namespace element.
            /// </summary>
            public const string NamespaceElement = "Namespace";

            /// <summary>
            /// The name of the property element.
            /// </summary>
            public const string PropertyElement = "ConfigurationProperty";

            /// <summary>
            /// The StringValue element.
            /// </summary>
            public const string StringValueElement = "StringValue";

            /// <summary>
            /// The IntegerValue element.
            /// </summary>
            public const string IntegerValueElement = "IntegerValue";

            /// <summary>
            /// The inner text of Namespace element for ConnectorSettingsInfo.
            /// </summary>
            public const string ConnectorSettingsInfo = "ConnectorSettingsInfo";

            /// <summary>
            /// The inner text of Name element for ConnectionString property.
            /// </summary>
            public const string ConnectionString = "ConnectionString";

            /// <summary>
            /// The inner text of Name element for Timeout property.
            /// </summary>
            public const string Timeout = "Timeout";
        }
    }
}