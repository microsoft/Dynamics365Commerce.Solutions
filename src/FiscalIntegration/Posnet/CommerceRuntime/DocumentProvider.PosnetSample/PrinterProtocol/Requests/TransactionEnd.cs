namespace Contoso
{
    namespace Commerce.Runtime.DocumentProvider.PosnetSample.PosnetProtocol.Requests
    {
        using Contoso.Commerce.Runtime.DocumentProvider.PosnetSample.DocumentModel.Protocol;

        /// <summary>
        /// Represents transaction end request [trend] according to POSNET specification.
        /// </summary>
        [Command("trend", CommandType.Request)]
        public sealed class TransactionEnd
        {
            /// <summary>
            /// Fiscal value of the transaction.
            /// </summary>
            [CommandParameter("to", DataType.Amount)]
            public decimal? FiscalValue { get; set; }

            /// <summary>
            /// Value of sent positive containers (gave out).
            /// </summary>
            [CommandParameter("op", DataType.Amount)]
            public decimal? SaleValue { get; set; }

            /// <summary>
            /// Value of sent negative containers (returned).
            /// </summary>
            [CommandParameter("om", DataType.Amount)]
            public decimal? ReturnValue { get; set; }

            /// <summary>
            /// Value of sent payment forms.
            /// </summary>
            [CommandParameter("fp", DataType.Amount)]
            public decimal? PaymentValue { get; set; }

            /// <summary>
            /// Value of sent changes.
            /// </summary>
            [CommandParameter("re", DataType.Amount)]
            public decimal? ChangeValue { get; set; }

            /// <summary>
            /// Automatic footer end flag.
            /// </summary>
            [CommandParameter("fe", DataType.Boolean)]
            public bool? AutomaticFooter { get; set; }
        }
    }
}
