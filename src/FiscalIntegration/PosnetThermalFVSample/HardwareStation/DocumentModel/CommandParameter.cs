namespace Contoso
{
    namespace Commerce.HardwareStation.PosnetThermalFVFiscalPrinterSample.DocumentModel
    {
        using System.Runtime.Serialization;

        /// <summary>
        /// Implements the parameter of command executed on the printer.
        /// </summary>
        [DataContract]
        public sealed class CommandParameter
        {
            /// <summary>
            /// Name of command.
            /// </summary>
            [DataMember]
            public string Name { get; }

            /// <summary>
            /// Value of command.
            /// </summary>
            [DataMember]
            public CommandParameterValue Value { get; }

            /// <summary>
            /// Initializes a new instance of <see cref="CommandParameter"/>.
            /// </summary>
            /// <param name="name">The parameter name.</param>
            /// <param name="value">The parameter value.</param>
            public CommandParameter(string name, CommandParameterValue value)
            {
                Name = name;
                Value = value;
            }
        }
    }
}
