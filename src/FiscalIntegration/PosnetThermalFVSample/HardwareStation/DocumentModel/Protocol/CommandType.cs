namespace Contoso
{
    namespace Commerce.HardwareStation.PosnetThermalFVFiscalPrinterSample.DocumentModel.Protocol
    {
        /// <summary>
        /// Types of commands supported by device.
        /// </summary>
        /// <remarks>Corresponds to the similar enum on the CRT side.</remarks>
        public enum CommandType
        {
            /// <summary>
            /// Empty value.
            /// </summary>
            None = 0,

            /// <summary>
            /// POSNET protocol request
            /// </summary>
            Request,

            /// <summary>
            /// Device driver function
            /// </summary>
            Function,

            /// <summary>
            /// Macro command
            /// </summary>
            Macro
        }
    }
}