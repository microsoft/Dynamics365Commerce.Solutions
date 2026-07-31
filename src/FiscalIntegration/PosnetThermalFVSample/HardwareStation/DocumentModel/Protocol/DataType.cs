namespace Contoso
{
    namespace Commerce.HardwareStation.PosnetThermalFVFiscalPrinterSample.DocumentModel.Protocol
    {
        /// <summary>
        /// Types of data used in Posnet protocol.
        /// </summary>
        public enum DataType
        {
            /// <summary>
            /// Empty value.
            /// </summary>
            None = 0,

            /// <summary>
            /// Decimal numerical
            /// </summary>
            Num,

            /// <summary>
            /// Amount value
            /// The last two digits constitutes decimal value
            /// </summary>
            Amount,

            /// <summary>
            /// ASCII string
            /// </summary>
            Alphanum,

            /// <summary>
            /// Date in format yyyy-mm-dd
            /// </summary>
            Date,

            /// <summary>
            /// Date and time in format yyyy-mm-dd,hh:mm
            /// </summary>
            DateTime,

            /// <summary>
            /// Boolean value. May have the value 0,1 or T,F or Y,N
            /// </summary>
            Boolean
        }
    }
}
