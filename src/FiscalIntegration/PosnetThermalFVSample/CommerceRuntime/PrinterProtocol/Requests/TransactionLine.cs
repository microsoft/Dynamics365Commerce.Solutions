namespace Contoso
{
    namespace Commerce.Runtime.DocumentProvider.PosnetSample.PosnetProtocol.Requests
    {
        using Contoso.Commerce.Runtime.DocumentProvider.PosnetSample.DocumentModel.Protocol;
        using RequiredDataAnnotationsAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

        /// <summary>
        /// Represents transaction line request [trline] according to POSNET specification.
        /// </summary>
        [Command("trline", CommandType.Request)]
        public sealed class TransactionLine
        {
            /// <summary>
            /// Name of goods.
            /// </summary>
            [CommandParameter("na", DataType.Alphanum)]
            [RequiredDataAnnotations]
            public string NameOfGoods { get; set; }

            /// <summary>
            /// VAT rate.
            /// </summary>
            [CommandParameter("vt", DataType.Num)]
            [RequiredDataAnnotations]
            public int? VATRateId { get; set; }

            /// <summary>
            /// Price.
            /// </summary>
            [CommandParameter("pr", DataType.Amount)]
            [RequiredDataAnnotations]
            public decimal? Price { get; set; }

            /// <summary>
            /// Item cancellation flag.
            /// </summary>
            [CommandParameter("st", DataType.Boolean)]
            public bool? ItemCancellation { get; set; }

            /// <summary>
            /// Total amount.
            /// </summary>
            [CommandParameter("wa", DataType.Amount)]
            public decimal? TotalAmount { get; set; }

            /// <summary>
            /// Number of goods.
            /// </summary>
            [CommandParameter("il", DataType.Num)]
            public decimal? NumberOfGoods { get; set; }

            /// <summary>
            /// Goods description.
            /// </summary>
            [CommandParameter("op", DataType.Alphanum)]
            public string GoodsDescription { get; set; }

            /// <summary>
            /// Measure unit.
            /// </summary>
            [CommandParameter("jm", DataType.Alphanum)]
            public string MeasureUnit { get; set; }

            /// <summary>
            /// Discount\Surcharge.
            /// </summary>
            [CommandParameter("rd", DataType.Boolean)]
            public bool? DiscountSurcharge { get; set; }

            /// <summary>
            /// Duscount\Surcharge name.
            /// </summary>
            [CommandParameter("rn", DataType.Alphanum)]
            public string DiscountSurchargeName { get; set; }

            /// <summary>
            /// Duscount\Surcharge percent.
            /// </summary>
            [CommandParameter("rp", DataType.Num)]
            public decimal? DuscountSurchargePercent { get; set; }

            /// <summary>
            /// Duscount\Surcharge amount.
            /// </summary>
            [CommandParameter("rw", DataType.Amount)]
            public decimal? DuscountSurchargeAmount { get; set; }
        }
    }
}
