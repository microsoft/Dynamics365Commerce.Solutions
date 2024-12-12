namespace Contoso
{
    namespace Commerce.Runtime.DocumentProvider.PosnetSample.PosnetProtocol.Requests
    {
        using Contoso.Commerce.Runtime.DocumentProvider.PosnetSample.DocumentModel.Protocol;
        using RequiredDataAnnotationsAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

        /// <summary>
        /// Represents return of goods request [stocash] according to POSNET specification.
        /// </summary>
        [Command("stocash", CommandType.Request)]
        public sealed class ReturnOfGoods
        {
            [CommandParameter("kw", DataType.Amount)]
            [RequiredDataAnnotations]
            public decimal? AmountOfPurchase { get; set; }
        }
    }
}
