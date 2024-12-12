namespace Microsoft.Dynamics.Commerce.CustomerMigrationUtility.Core
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
 
    /// <summary>Simple HTTP client wrapper.</summary>
    public class HttpClient
    {
        /// <summary>Time out in seconds</summary>
        public const int TimeOutSecs = 100;

        private readonly static System.Net.Http.HttpClient client = new System.Net.Http.HttpClient() { Timeout = TimeSpan.FromSeconds(TimeOutSecs) };

        /// <summary>Gets the requested URL.</summary>
        /// <param name="path">The path</param>        
        /// <param name="authHeader">Authentication CommerceIdentityHeader</param>
        /// <param name="headers">The headers</param>
        /// <returns>HTTP response</returns>
        public async Task<HttpResponseMessage> GetAsync(string path,
                                                        AuthenticationHeaderValue authHeader,
                                                        List<KeyValuePair<string, string>> headers = null)
        {
            return await this.SendAsync(HttpMethod.Get, null, path, authHeader, headers).ConfigureAwait(false);
        }

        /// <summary>Gets the requested URL.</summary>
        /// <param name="path">The path</param>        
        /// <param name="authHeader">Authentication CommerceIdentityHeader</param>
        /// <param name="headers">The headers</param>
        /// <returns>HTTP response</returns>
        public async Task<HttpResponseMessage> DeleteAsync(string path,
                                                        AuthenticationHeaderValue authHeader,
                                                        List<KeyValuePair<string, string>> headers = null)
        {
            return await this.SendAsync(HttpMethod.Delete, null, path, authHeader, headers).ConfigureAwait(false);
        }

        /// <summary>Retrieves the content of the HTTP response message.</summary>
        /// <param name="response">HTTP response message.</param>
        /// <returns>Content of the response.</returns>
        public async Task<string> GetContentAsync(HttpResponseMessage response)
        {
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        /// <summary>Patch to the requested URL.</summary>
        /// <param name="data">Data to send</param>
        /// <param name="path">Relative path</param>        
        /// <param name="authHeader">The authentication header</param>
        /// <param name="headers">The headers</param>
        /// <returns>HTTP response</returns>
        public async Task<HttpResponseMessage> PatchAsync(object data,
                                                         string path,
                                                         AuthenticationHeaderValue authHeader,
                                                         List<KeyValuePair<string, string>> headers = null)
        {
            return await this.SendAsync(new HttpMethod("Patch"), data, path, authHeader, headers).ConfigureAwait(false);
        }


        /// <summary>Post to the requested URL.</summary>
        /// <param name="data">Data to send</param>
        /// <param name="path">Relative path</param>        
        /// <param name="authHeader">The authentication header</param>
        /// <param name="headers">The headers</param>
        /// <returns>HTTP response</returns>
        public async Task<HttpResponseMessage> PostAsync(object data,
                                                         string path,
                                                         AuthenticationHeaderValue authHeader,
                                                         List<KeyValuePair<string, string>> headers = null)
        {
            return await this.SendAsync(HttpMethod.Post, data, path, authHeader, headers).ConfigureAwait(false);
        }

        /// <summary>Send a HTTP request.</summary>
        /// <param name="method">HTTP method</param>
        /// <param name="data">Data to send</param>
        /// <param name="path">Relative path</param>        
        /// <param name="authHeader">The authentication header</param>
        /// <param name="headers">The headers</param>
        /// <returns>HTTP response</returns>
        private async Task<HttpResponseMessage> SendAsync(HttpMethod method,
                                                          object data,
                                                          string path,
                                                          AuthenticationHeaderValue authHeader,
                                                          List<KeyValuePair<string, string>> headers = null)
        {
            using (var request = new HttpRequestMessage(method, path))
            {
                if (headers != null)
                {
                    foreach (KeyValuePair<string, string> header in headers)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }

                if (method == HttpMethod.Post || method == new HttpMethod("PATCH"))
                {
                    var json = JsonConvert.SerializeObject(data, Formatting.None);
                    request.Content = new StringContent(json,
                                                        new UTF8Encoding(),
                                                        "application/json");
                }

                if (authHeader != null)
                {
                    request.Headers.Authorization = authHeader;
                }

                HttpResponseMessage response;
                try
                {
                    response =
                        await
                        client.SendAsync(request, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);
                }
                catch (TaskCanceledException e)
                {
                    throw new HttpRequestException(
                        "The request was canceled, probably timed out (timeout: " + TimeOutSecs +
                        " secs).",
                        e);
                }

                return response;
            }
        }
    }
}