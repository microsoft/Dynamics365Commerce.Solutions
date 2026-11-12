namespace Contoso
{
    namespace Commerce.Runtime.DocumentProvider.PosnetSample.PosnetProtocol.Requests
    {
        using Contoso.Commerce.Runtime.DocumentProvider.PosnetSample.DocumentModel.Protocol;
        using RequiredDataAnnotationsAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

        /// <summary>
        /// Represents transaction NIP set request [trnipset] according to POSNET specification (POT-AND-DEV-05 Version: 4404).
        /// </summary>
        [Command("trnipset", CommandType.Request)]
        public sealed class TransactionNipSet
        {
            /// <summary>
            /// NIP (VAT ID).
            /// </summary>
            [CommandParameter("ni", DataType.Alphanum)]
            [RequiredDataAnnotations]
            public string Nip { get; set; }

            /// <summary>
            /// Print as highlighted.
            /// </summary>
            [CommandParameter("dw", DataType.Boolean)]
            public bool? Highlight { get; set; }

            /// <summary>
            /// Description.
            /// </summary>
            [CommandParameter("ds", DataType.Alphanum)]
            public string Description { get; set; }
        }
    }
}
