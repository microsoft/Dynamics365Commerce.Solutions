namespace Contoso
{
    namespace Commerce.Runtime.DocumentProvider.PosnetSample.DocumentModel.Protocol
    {
        /// <summary>
        /// Types of commands supported by connector.
        /// </summary>
        public enum CommandType
        {
            /// <summary>
            /// The default value of type of command.
            /// </summary>
            None = 0,

            /// <summary>
            /// POSNET protocol request.
            /// </summary>
            Request,

            /// <summary>
            /// Device driver function.
            /// </summary>
            Function,

            /// <summary>
            /// Macro command.
            /// </summary>
            Macro
        }
    }
}
