namespace Contoso
{
    namespace Commerce.Runtime.DocumentProvider.PosnetSample.PosnetProtocol.Requests
    {
        using Contoso.Commerce.Runtime.DocumentProvider.PosnetSample.DocumentModel.Protocol;

        /// <summary>
        /// Represents counter status request [scnt] according to POSNET specification.
        /// </summary>
        [Command("scnt", CommandType.Request)]
        public sealed class CounterStatus
        {
            /// <summary>
            /// Daily reports counter.
            /// </summary>
            [CommandParametersResult("rd", DataType.Num)]
            public long DailyReportCounter { get; set; }

            /// <summary>
            /// Last printed header number.
            /// </summary>
            [CommandParametersResult("hn", DataType.Num)]
            public long LastPrintedHeaderNumber { get; set; }

            /// <summary>
            /// Last printed receipt number.
            /// </summary>
            [CommandParametersResult("bn", DataType.Num)]
            public long LastPrintedReceiptNumber { get; set; }

            /// <summary>
            /// Last printed VAT invoice number.
            /// </summary>
            [CommandParametersResult("fn", DataType.Num)]
            public long LastPrintedVATInvoiceNumber { get; set; }

            /// <summary>
            /// Fiscal memory id.
            /// </summary>
            [CommandParametersResult("nu", DataType.Alphanum)]
            public string FiscalMemoryId { get; set; }
        }
    }
}
