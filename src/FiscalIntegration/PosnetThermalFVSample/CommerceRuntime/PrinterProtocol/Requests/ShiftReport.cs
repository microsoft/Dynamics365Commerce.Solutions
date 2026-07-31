namespace Contoso
{
    namespace Commerce.Runtime.DocumentProvider.PosnetSample.PosnetProtocol.Requests
    {
        using Contoso.Commerce.Runtime.DocumentProvider.PosnetSample.DocumentModel.Protocol;
        using RequiredDataAnnotationsAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

        /// <summary>
        /// Represents shift report [shiftrep] request according to POSNET specification.
        /// </summary>
        [Command("shiftrep", CommandType.Request)]
        public sealed class ShiftReport
        {
            [CommandParameter("sh", DataType.Alphanum)]
            [RequiredDataAnnotations]
            public string ShiftName { get; set; }

            [CommandParameter("zr", DataType.Boolean)]
            public bool? ClearReport { get; set; }
        }
    }
}
