namespace Contoso
{
    namespace Commerce.Runtime.DocumentProvider.PosnetSample.PosnetProtocol
    {
        using System;
        using Contoso.Commerce.Runtime.DocumentProvider.PosnetSample.DocumentModel.Protocol;

        /// <summary>
        /// Specifies that a property is a POSNET command result parameter.
        /// </summary>
        [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
        internal sealed class CommandParametersResultAttribute : Attribute
        {
            /// <summary>
            /// Name of parameter.
            /// </summary>
            public string Name { get; }

            /// <summary>
            /// Type of value.
            /// </summary>
            public DataType ValueType { get; }

            /// <summary>
            /// Initializes a new instance of <see cref="CommandParametersResultAttribute"/>.
            /// </summary>
            /// <param name="name">Name of parameter.</param>
            /// <param name="valueType">Type od value.</param>
            public CommandParametersResultAttribute(string name, DataType valueType)
            {
                this.Name = name;
                this.ValueType = valueType;
            }
        }
    }
}
