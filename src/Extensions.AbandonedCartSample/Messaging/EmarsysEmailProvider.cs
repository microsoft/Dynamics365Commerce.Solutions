/**
* SAMPLE CODE NOTICE
* 
* THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
* OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
* THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
* NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

namespace Contoso.RetailServer.Ecommerce.AbandonedCartSample.Messaging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Dynamics.Commerce.RetailProxy;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using Contoso.RetailServer.Ecommerce.AbandonedCartSample.Options;
    using Contoso.RetailServer.Ecommerce.AbandonedCartSample.Common;
    using Contoso.RetailServer.Ecommerce.AbandonedCartSample.Security;

    public class EmarsysEmailProvider : IEmailProvider
    {
        private readonly IKeyVaultClient KeyVaultClient;
        private readonly HttpClient client;
        private readonly EmarsysClientOptions emarsysClientOptions;
        private readonly ILogger<EmarsysEmailProvider> logger;
        private readonly MediaOptions mediaOptions;

        public EmarsysEmailProvider(IKeyVaultClient keyVaultClient,
            IOptions<EmarsysClientOptions> emarsysClientOptions,
            IOptions<MediaOptions> mediaOptions,
            IHttpClientFactory httpClientFactory,
            ILogger<EmarsysEmailProvider> emailProviderLogger)
        {
            keyVaultClient.VerifyIsNotNull(nameof(keyVaultClient));
            emarsysClientOptions.VerifyIsNotNull(nameof(emarsysClientOptions));
            ValidationHelpers.TryValidate(emarsysClientOptions.Value);
            mediaOptions.VerifyIsNotNull(nameof(mediaOptions));
            ValidationHelpers.TryValidate(mediaOptions.Value);
            httpClientFactory.VerifyIsNotNull(nameof(httpClientFactory));
            emailProviderLogger.VerifyIsNotNull(nameof(emailProviderLogger));


            this.KeyVaultClient = keyVaultClient;
            this.emarsysClientOptions = emarsysClientOptions.Value;
            this.mediaOptions = mediaOptions.Value;
            this.logger = emailProviderLogger;
            this.client = httpClientFactory.CreateClient();
        }

        public async Task<HttpStatusCode> SendEmails(
                                                     List<Cart> abandonedCarts,
                                                     PagedResult<Customer> customers,
                                                     PagedResult<Product> products,
                                                     string returnToCartUrl,
                                                     string currency)
        {
            var emailPayload = this.BuildEmailPayload(abandonedCarts, customers, products, returnToCartUrl, currency);

            var emarsysApiUserName = await this.KeyVaultClient.GetSecretValueFromKeyVaultAsync(this.emarsysClientOptions.ApiUserNameKeyVaultSecretName).ConfigureAwait(false);
            ValidationHelpers.VerifyIsNotNullOrWhiteSpace(emarsysApiUserName, nameof(emarsysApiUserName));

            var emarsysApiSecret = await this.KeyVaultClient.GetSecretValueFromKeyVaultAsync(this.emarsysClientOptions.ApiSecretKeyVaultSecretName).ConfigureAwait(false);
            ValidationHelpers.VerifyIsNotNullOrWhiteSpace(emarsysApiSecret, nameof(emarsysApiSecret));

            var nonce = GetRandomString(32);
            var timestamp = DateTime.UtcNow.ToString("o");
            var passwordDigest = System.Convert.ToBase64String(Encoding.UTF8.GetBytes(Sha1(nonce + timestamp + emarsysApiSecret)));
            var authHeader = String.Format("Username=\"{0}\", PasswordDigest=\"{1}\", Nonce=\"{2}\", Created=\"{3}\"  Content-type: application/json;charset=\"utf-8\"", emarsysApiUserName, passwordDigest, nonce, timestamp);
            var req = new HttpRequestMessage(HttpMethod.Post, string.Format(this.emarsysClientOptions.ApiUrl, this.emarsysClientOptions.ExternalEventId));
            req.Headers.Add("X-WSSE", "UsernameToken " + authHeader);
            req.Content = new StringContent(JsonConvert.SerializeObject(emailPayload), Encoding.UTF8, "application/json");
            this.client.DefaultRequestHeaders.Accept.Clear();
            HttpResponseMessage emailProviderResponse;
            try
            {
                emailProviderResponse = await this.client.SendAsync(req).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Errors occured while trying to send emails {ex.Message}");
                throw ex;
            }

            return emailProviderResponse.StatusCode;
        }

        private static string Sha1(string input)
        {
            var hashInBytes = new SHA1CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(input)); // CodeQL [SM02196] This is required by the Emarsys API. See: https://dev.emarsys.com/docs/emarsys-api/10827827b819d-3-configure-authentication
            return string.Join(string.Empty, Array.ConvertAll(hashInBytes, b => b.ToString("x2")));
        }

        private static string GetRandomString(int length)
        {
            var random = new Random();
            string[] chars = new string[] { "0", "2", "3", "4", "5", "6", "8", "9", "a", "b", "c", "d", "e", "f", "g", "h", "j", "k", "m", "n", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
            var sb = new StringBuilder();
            for (int i = 0; i < length; i++) sb.Append(chars[random.Next(chars.Length)]);
            return sb.ToString();
        }

        private EmailPayload BuildEmailPayload(List<Cart> abandonedCarts,
                                       PagedResult<Customer> customers,
                                       PagedResult<Product> products,
                                       string returnToCartUrl,
                                       string currency)
        {
            var contacts = new List<Contact>();
            foreach (var abandonedCart in abandonedCarts)
            {
                var cartCustomer = customers.Single(customer => customer.AccountNumber == abandonedCart.CustomerId);
                var contact = new Contact();
                contact.ExternalId = cartCustomer.Email;
                contact.Data = new Data();
                contact.Data.FirstName = cartCustomer.FirstName;
                contact.Data.LastName = cartCustomer.LastName;
                contact.Data.Language = cartCustomer.Language;
                contact.Data.Currency = currency;

                contact.Data.ReturnToCartUrl = returnToCartUrl;

                contact.Data.OrderDetails = new List<OrderDetail>();
                foreach (var cartLine in abandonedCart.CartLines)
                {
                    var productInfo = products.SingleOrDefault(product => product.RecordId == cartLine.ProductId);
                    if (productInfo == null)
                    {
                        continue;
                    }

                    var orderDetails = new OrderDetail();
                    orderDetails.ProductName = productInfo?.ProductName ?? "Not Available";
                    var productImage = productInfo?.Image?.Items?.FirstOrDefault();
                    if (productImage != null)
                    {
                        orderDetails.productImage = new ProductImage()
                        {
                            ViewPortImages = new List<ViewPortImage>(),
                            altText = String.IsNullOrEmpty(productImage.AltText) ? productInfo.ProductName : productImage.AltText
                        };

                        foreach (var viewPort in mediaOptions.ImageViewPorts)
                        {
                            orderDetails.productImage.ViewPortImages.Add(
                                new ViewPortImage()
                                {
                                    url = GetImageUrlString(productImage.Url, viewPort),
                                    media = viewPort.Viewport,
                                    UseForDefaultImageTag = viewPort.UseForDefaultImageTag
                                }
                               );
                        }
                    }

                    orderDetails.ProductId = cartLine.ProductId.ToString();
                    orderDetails.Quantity = cartLine.Quantity.HasValue ? this.Normalize(cartLine.Quantity.Value).ToString() : "Not specified";
                    orderDetails.UnitPrice = cartLine.Price.HasValue ? this.Normalize(cartLine.Price.Value).ToString() : "Not Specified";
                    contact.Data.OrderDetails.Add(orderDetails);
                }

                contacts.Add(contact);
            }

            return new EmailPayload() { Contacts = contacts, KeyId = this.emarsysClientOptions.EmarsysContactKeyId, ReturnToCartUrl = returnToCartUrl };
        }

        private string GetImageUrlString(string imageUrl, ImageViewPort viewPort)
        {
            const string urlSuffix = "http";
            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                imageUrl = string.Empty;
            }
            else if (!imageUrl.StartsWith(urlSuffix, StringComparison.InvariantCultureIgnoreCase))
            {
                var imageSuffix = $"{imageUrl}{"&w="}{ viewPort.ImageWidth}{"&h="}{viewPort.ImageHeight}{"&q=80&m=6&f=jpg"}";
                imageUrl = $"{mediaOptions.ImageServerUrl}/{imageSuffix}";
            }

            return imageUrl;
        }

        private decimal Normalize(decimal inputValue)
        {
            return inputValue / 1.000000000000000000000000000000000m;
        }
    }
}
