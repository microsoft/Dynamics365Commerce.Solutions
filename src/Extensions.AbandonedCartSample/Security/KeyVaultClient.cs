/**
* SAMPLE CODE NOTICE
* 
* THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
* OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
* THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
* NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

namespace Contoso.RetailServer.Ecommerce.AbandonedCartSample.Security
{
    using System;
    using System.Threading.Tasks;

    using global::Azure.Identity;
    using global::Azure.Security.KeyVault.Secrets;
    using Microsoft.Extensions.Options;
    using Microsoft.Extensions.Logging;
    using Contoso.RetailServer.Ecommerce.AbandonedCartSample.Options;
    using Contoso.RetailServer.Ecommerce.AbandonedCartSample.Common;

    public class KeyVaultClient : IKeyVaultClient
    {
        private SecretClient secretClient;

        private readonly KeyVaultOptions options;
        private readonly ILogger<KeyVaultClient> logger;

        public KeyVaultClient(
            IOptions<KeyVaultOptions> keyVaultOptions,
             ILogger<KeyVaultClient> keyVaultClientLogger)
        {
            keyVaultOptions.VerifyIsNotNull(nameof(keyVaultOptions));
            ValidationHelpers.TryValidate(keyVaultOptions.Value);
            keyVaultClientLogger.VerifyIsNotNull(nameof(keyVaultClientLogger));

            this.options = keyVaultOptions.Value;
            this.logger = keyVaultClientLogger;
        }

        private SecretClient GetSecretClient()
        {
            return this.secretClient ?? new SecretClient(
                       new Uri(this.options.KeyVaultUrl),
                       new DefaultAzureCredential());
        }

        public async Task<string> GetSecretValueFromKeyVaultAsync(string secretName)
        {
            ValidationHelpers.VerifyIsNotNullOrWhiteSpace(secretName, nameof(secretName));
            this.secretClient = GetSecretClient();
            var secret = await this.secretClient.GetSecretAsync(secretName).ConfigureAwait(false);
            return secret?.Value?.Value;
        }

    }
}
