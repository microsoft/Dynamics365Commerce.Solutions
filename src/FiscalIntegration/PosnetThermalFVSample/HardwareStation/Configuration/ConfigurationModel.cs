namespace Contoso
{
    namespace Commerce.HardwareStation.PosnetThermalFVFiscalPrinterSample
    {
        /// <summary>
        /// Represent printer configuration model.
        /// </summary>
        internal class ConfigurationModel
        {
            /// <summary>
            /// Gets or sets printers endpoint address.
            /// </summary>
            [ConfigurationAttribute(ConfigurationNamespaceType.Connection, "ConnectionString")]
            public string ConnectionString { get; set; }

            /// <summary>
            /// Gets or sets POSNET timeout.
            /// </summary>
            [ConfigurationAttribute(ConfigurationNamespaceType.Settings, "PrinterTimeoutMilliseconds")]
            public int PrinterTimeoutMilliseconds { get; set; }

            /// <summary>
            /// Gets or sets the flag indicating that date and time synchronization are required.
            /// </summary>
            [ConfigurationAttribute(ConfigurationNamespaceType.Settings, "DateTimeSynchronizationRequired")]
            public bool SynchronizeDateTime { get; set; }
        }
    }
}
