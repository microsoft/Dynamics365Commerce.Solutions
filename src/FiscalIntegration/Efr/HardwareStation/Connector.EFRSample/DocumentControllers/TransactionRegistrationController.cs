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
    namespace Commerce.HardwareStation.EFRSample
    {
        using System.Net.Http;
        using System.Text;
        using System.Threading;
        using System.Threading.Tasks;
        using Contoso.Commerce.HardwareStation.EFRSample.Constants;
        using ThrowIf = Microsoft.Dynamics.Commerce.HardwareStation.ThrowIf;

        /// <summary>
        /// Implements transaction registration methods.
        /// </summary>
        public static class TransactionRegistrationController
        {
            /// <summary>
            /// Http client.
            /// </summary>
            private static readonly HttpClient httpClient = new HttpClient();

            /// <summary>
            /// Registers sales transaction in the service.
            /// </summary>
            /// <param name="salesTransaction">The sales transaction to register.</param>
            /// <param name="endPointAddress">The service endpoint address.</param>
            /// <param name="token">The cancellation token.</param>
            /// <returns>The response from printer.</returns>
            public static Task<string> RegisterAsync(string salesTransaction, string endPointAddress, CancellationToken token)
            {
                return RunPostRequestAsync(
                        endPointAddress + "/" + RequestConstants.Register,
                        salesTransaction, token);
            }

            /// <summary>
            /// Runs POST request.
            /// </summary>
            /// <param name="requestUri">The request Uri.</param>
            /// <param name="requestBody">The request body.</param>
            /// <param name="token">The cancellation token.</param>
            /// <returns>The response from the service.</returns>
            private static async Task<string> RunPostRequestAsync(string requestUri, string requestBody, CancellationToken token)
            {
                ThrowIf.NullOrWhiteSpace(requestUri, nameof(requestUri));

                using (StringContent requestContent = new StringContent(requestBody, Encoding.UTF8, "application/xml"))
                {
                    using (HttpResponseMessage httpResponse = await httpClient.PostAsync(requestUri, requestContent, token).ConfigureAwait(false))
                    {
                        string response = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

                        return response;
                    }
                }
            }
        }
    }
}
