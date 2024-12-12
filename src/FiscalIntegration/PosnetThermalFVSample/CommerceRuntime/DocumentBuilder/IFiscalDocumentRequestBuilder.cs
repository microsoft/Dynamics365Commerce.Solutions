namespace Contoso
{
    namespace Commerce.Runtime.DocumentProvider.PosnetSample.DocumentBuilder
    {
        using System.Threading.Tasks;

        /// <summary>
        /// Document builder interface.
        /// </summary>
        public interface IFiscalDocumentRequestBuilder
        {
            /// <summary>
            /// Builds a fiscal document.
            /// </summary>
            /// <returns>The <see cref="PosnetFiscalDocumentBuildResult"/> instance.</returns>
            Task<PosnetFiscalDocumentBuildResult> BuildAsync();
        }
    }
}
