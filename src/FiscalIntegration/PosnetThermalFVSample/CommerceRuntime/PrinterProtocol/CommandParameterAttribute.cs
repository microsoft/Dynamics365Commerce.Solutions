namespace Contoso
{
    namespace Commerce.Runtime.DocumentProvider.PosnetSample.PosnetProtocol
    {
        using System;
        using Contoso.Commerce.Runtime.DocumentProvider.PosnetSample.DocumentModel.Protocol;

        /// <summary>
        /// Specifies that a property is a POSNET command parameter.
        /// </summary>
        [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
        internal sealed class CommandParameterAttribute : Attribute
        {
            /// <summary>
            /// Initializes a new instance of <see cref="CommandParameterAttribute"/>.
            /// </summary>
            /// <param name="name">Name of the parameter.</param>
            /// <param name="valueType">Value type.</param>
            public CommandParameterAttribute(string name, DataType valueType)
            {
                Name = name;
                ValueType = valueType;
            }

            /// <summary>
            /// Name of parameter.
            /// </summary>
            public string Name { get; }

            /// <summary>
            /// Type of value.
            /// </summary>
            public DataType ValueType { get; }
        }
    }
}
