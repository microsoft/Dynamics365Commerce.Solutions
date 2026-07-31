namespace Contoso
{
    namespace Commerce.Runtime.DocumentProvider.PosnetSample.PosnetProtocol
    {
        using System;
        using Contoso.Commerce.Runtime.DocumentProvider.PosnetSample.DocumentModel.Protocol;

        /// <summary>
        /// Specifies that a class represents a POSNET command.
        /// </summary>
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
        internal sealed class CommandAttribute : Attribute
        {
            /// <summary>
            /// Name of command.
            /// </summary>
            public string Name { get; }

            /// <summary>
            /// Type of command.
            /// </summary>
            public CommandType CommandType { get; }

            /// <summary>
            /// Initializes a new instance of <see cref="CommandAttribute"/>.
            /// </summary>
            /// <param name="name">Name of command.</param>
            /// <param name="commandType">Type of command.</param>
            internal CommandAttribute(string name, CommandType commandType)
            {
                Name = name;
                CommandType = commandType;
            }
        }
    }
}
