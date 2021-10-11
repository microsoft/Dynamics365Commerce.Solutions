/**
 * SAMPLE CODE NOTICE
 * 
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

namespace Contoso
{
    namespace CommerceRuntime.DocumentProvider.CleanCashSample
    {
        using System.IO;
        using System.Runtime.Serialization.Json;
        using System.Threading.Tasks;
        using Microsoft.Dynamics.Commerce.Runtime;

        /// <summary>
        /// Encapsulates the serialization logic.
        /// </summary>
        public static class SerializationHelper
        {
            /// <summary>
            /// Serializes <c>CleanCashFiscalTransactionData</c> to JSON string.
            /// </summary>
            /// <param name="cleanCashFiscalTransactionData">Sequential signature data to serialize.</param>
            /// <returns><c>CleanCashFiscalTransactionData</c> serialized to JSON string.</returns>
            public static async Task<string> SerializeFiscalTransactionDataToJsonAsync(CleanCashFiscalTransactionData cleanCashFiscalTransactionData)
            {
                ThrowIf.Null(cleanCashFiscalTransactionData, nameof(cleanCashFiscalTransactionData));

                using (var memoryStream = new MemoryStream())
                {
                    DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(cleanCashFiscalTransactionData.GetType());
                    jsonSerializer.WriteObject(memoryStream, cleanCashFiscalTransactionData);
                    memoryStream.Position = 0;
                    StreamReader reader = new StreamReader(memoryStream);
                    return await reader.ReadToEndAsync().ConfigureAwait(false);
                }
            }
        }
    }
}
