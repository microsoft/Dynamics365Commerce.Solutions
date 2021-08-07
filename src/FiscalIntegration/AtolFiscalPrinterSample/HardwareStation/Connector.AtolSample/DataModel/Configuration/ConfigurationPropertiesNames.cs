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
    namespace HardwareStation.Connector.AtolSample.DataModel.Configuration
    {
        /// <summary>
        /// Class represents connector xml configuration element names.
        /// </summary>
        public static class ConfigurationPropertiesNames
        {
            /// <summary>
            /// The inner text of Namespace element for ConnectorGeneralInfo.
            /// </summary>
            public const string NamespaceConnectorGeneralInfo = "ConnectorGeneralInfo";

            /// <summary>
            /// The inner text of Namespace element for ConnectorConnectionInfo.
            /// </summary>
            public const string NamespaceConnectorConnectionInfo = "ConnectorConnectionInfo";

            /// <summary>
            /// The inner text of Namespace element for ConnectorSettingsInfo.
            /// </summary>
            public const string NamespaceConnectorSettingsInfo = "ConnectorSettingsInfo";

            /// <summary>
            /// Print previous not printed document property element.
            /// </summary>
            public const string PropertyPrintPreviousNotPrintedDocument = "PrintPreviousNotPrintedDocument";

            /// <summary>
            /// The COM port property element.
            /// </summary>
            public const string PropertyComPort = "ComPort";

            /// <summary>
            /// The baud rate property element.
            /// </summary>
            public const string PropertyBaudRate = "BaudRate";

            /// <summary>
            /// The printer timeout milliseconds property element.
            /// </summary>
            public const string PropertyPrinterTimeoutMilliseconds = "PrinterTimeoutMilliseconds";
        }
    }
}
