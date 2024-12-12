namespace Contoso
{
    namespace Commerce.HardwareStation.PosnetThermalFVFiscalPrinterSample.DocumentModel
    {
        using System.Runtime.Serialization;
        using Contoso.Commerce.HardwareStation.PosnetThermalFVFiscalPrinterSample.DocumentModel.Protocol;

        /// <summary>
        /// Implements command parameter value sent to the printer.
        /// </summary>
        [DataContract]
        public sealed class CommandParameterValue
        {
            /// <summary>
            /// The value.
            /// </summary>
            [DataMember]
            public object Value { get; }

            /// <summary>
            /// Type of value supported by printer.
            /// </summary>
            [DataMember]
            public DataType ValueType { get; }

            /// <summary>
            /// Initializes a new instance of <see cref="CommandParameterValue"/>.
            /// </summary>
            /// <param name="value">The parameter value.</param>
            /// <param name="valueType">The parameter value type.</param>
            public CommandParameterValue(object value, DataType valueType)
            {
                ValueType = valueType;
                Value = value;
            }
        }
    }
}