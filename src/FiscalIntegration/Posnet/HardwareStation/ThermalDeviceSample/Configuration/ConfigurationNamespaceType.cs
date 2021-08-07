namespace Contoso
{
    namespace Commerce.HardwareStation.PosnetThermalFVFiscalPrinterSample
    {
        using System.ComponentModel;

        /// <summary>
        /// Enumerates configuration namespace values.
        /// </summary>
        internal enum ConfigurationNamespaceType
        {
            /// <summary>
            /// The connection namespace value.
            /// </summary>
            [Description("ConnectorConnectionInfo")]
            Connection = 0,

            /// <summary>
            /// The settings namespace value.
            /// </summary>
            [Description("ConnectorSettingsInfo")]
            Settings = 1
        }
    }
}
