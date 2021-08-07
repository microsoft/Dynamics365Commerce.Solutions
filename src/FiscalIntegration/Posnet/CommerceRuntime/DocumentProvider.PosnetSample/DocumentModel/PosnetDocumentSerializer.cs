namespace Contoso
{
    namespace Commerce.Runtime.DocumentProvider.PosnetSample.DocumentModel
    {
        using System.Text;
        using Contoso.Commerce.Runtime.DocumentProvider.PosnetSample.DocumentModel.Protocol;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Newtonsoft.Json;

        /// <summary>
        /// The extension class to serialize document request to JSON.
        /// </summary>
        internal static class PosnetDocumentSerializer
        {
            /// <summary>
            /// Serializes the implementation of <see cref="IPosnetDocumentRequest"/> to JSON.
            /// </summary>
            /// <param name="documentRequest">the request.</param>
            /// <returns>JSON string.</returns>
            public static string ToJson(this IPosnetDocumentRequest documentRequest)
            {
                ThrowIf.Null(documentRequest, nameof(documentRequest));

                string jsonString = JsonConvert.SerializeObject(documentRequest, Formatting.Indented);
                return Encoding.UTF8.GetString(Encoding.Default.GetBytes(jsonString));
            }
        }
    }
}
