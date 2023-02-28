namespace Contoso
{
    namespace Commerce.Runtime.DocumentProvider.PosnetSample.PosnetProtocol.Requests
    {
        using Contoso.Commerce.Runtime.DocumentProvider.PosnetSample.DocumentModel.Protocol;
        using RequiredDataAnnotationsAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

        /// <summary>
        /// Represents transaction payment request [trpayment] according to POSNET specification.
        /// </summary>
        [Command("trpayment", CommandType.Request)]
        public sealed class TransactionPayment
        {
            /// <summary>
            /// Payment form type.
            /// </summary>
            [CommandParameter("ty", DataType.Num)]
            [RequiredDataAnnotations]
            public int? PaymentFormType { get; set; }

            /// <summary>
            /// Payment value.
            /// </summary>
            [CommandParameter("wa", DataType.Amount)]
            [RequiredDataAnnotations]
            public decimal? PaymentValue { get; set; }

            /// <summary>
            /// Payment form.
            /// </summary>
            [CommandParameter("na", DataType.Alphanum)]
            public string PaymentForm { get; set; }

            /// <summary>
            /// Change flag.
            /// </summary>
            [CommandParameter("re", DataType.Boolean)]
            public bool? IsChange { get; set; }
        }
    }
}
