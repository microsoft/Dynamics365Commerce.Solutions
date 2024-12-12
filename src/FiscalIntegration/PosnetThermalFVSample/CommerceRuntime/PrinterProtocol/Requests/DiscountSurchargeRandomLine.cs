namespace Contoso
{
    namespace Commerce.Runtime.DocumentProvider.PosnetSample.PosnetProtocol.Requests
    {
        using Contoso.Commerce.Runtime.DocumentProvider.PosnetSample.DocumentModel.Protocol;
        using RequiredDataAnnotationsAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

        /// <summary>
        /// Represents discount\surcharge from a random line request [trdiscntline] according to POSNET specification.
        /// </summary>
        [Command("trdiscntline", CommandType.Request)]
        public sealed class DiscountSurchargeRandomLine
        {
            /// <summary>
            /// VAT rate.
            /// </summary>
            [CommandParameter("vt", DataType.Num)]
            [RequiredDataAnnotations]
            public int? VATRateId { get; set; }

            /// <summary>
            /// Name of goods.
            /// </summary>
            [CommandParameter("ds", DataType.Alphanum)]
            [RequiredDataAnnotations]
            public string NameOfGoods { get; set; }

            /// <summary>
            /// Sales amount with granted discount/surcharge.
            /// </summary>
            [CommandParameter("rt", DataType.Amount)]
            [RequiredDataAnnotations]
            public decimal? SalesAmount { get; set; }

            /// <summary>
            /// Discount/surcharge cancelling (true)/without cancelling (false).
            /// </summary>
            [CommandParameter("st", DataType.Boolean)]
            public bool? DiscountSurchargeCancelling { get; set; }

            /// <summary>
            /// Discount (true) surcharge (false).
            /// </summary>
            [CommandParameter("rd", DataType.Boolean)]
            public bool? DiscountSurcharge { get; set; }

            /// <summary>
            /// Discount/surcharge percent.
            /// </summary>
            [CommandParameter("rp", DataType.Num)]
            public bool? DiscountSurchargePercent { get; set; }

            /// <summary>
            /// Discount/surcharge amount.
            /// </summary>
            [CommandParameter("rw", DataType.Amount)]
            public bool? DiscountSurchargeValue { get; set; }

            /// <summary>
            /// Discount/surcharge name.
            /// </summary>
            [CommandParameter("na", DataType.Alphanum)]
            public string Name { get; set; }
        }
    }
}
