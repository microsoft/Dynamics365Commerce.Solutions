namespace Contoso
{
    namespace Commerce.Runtime.DocumentProvider.PosnetSample.PosnetProtocol.Requests
    {
        using Contoso.Commerce.Runtime.DocumentProvider.PosnetSample.DocumentModel.Protocol;

        /// <summary>
        /// Represents transaction initialization request [trinit] according to POSNET specification.
        /// </summary>
        [Command("trinit", CommandType.Request)]
        public sealed class TransactionInit
        {
            /// <summary>
            /// Block mode.
            /// </summary>
            [CommandParameter("bm", DataType.Boolean)]
            public bool? BlockMode { get; set; }
        }
    }
}
