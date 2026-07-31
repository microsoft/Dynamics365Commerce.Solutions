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
    using Contoso.RetailServer.Ecommerce.AbandonedCartSample.Common;
    using Contoso.RetailServer.Ecommerce.AbandonedCartSample.Options;
    using Microsoft.Extensions.Options;
    using Microsoft.Identity.Client;

    public class TokenProvider : ITokenProvider
    {
        private const string AuthorityBaseUrl = @"https://login.windows.net/";

        private readonly IKeyVaultClient KeyVaultClient;

        public TokenProvider(IKeyVaultClient keyVaultClient,
            IOptions<RetailServerClientOptions> retailServerClientOptions)
        {
            keyVaultClient.VerifyIsNotNull(nameof(keyVaultClient));
            retailServerClientOptions.VerifyIsNotNull(nameof(retailServerClientOptions));
            ValidationHelpers.TryValidate(retailServerClientOptions.Value);

            this.KeyVaultClient = keyVaultClient;
        }

        private async Task<string> GetToken(string clientId, string secret, string tenantId, string audience)
        {
            var authority = new Uri($"{AuthorityBaseUrl}{tenantId}");

            var confidentialClientApplication = ConfidentialClientApplicationBuilder
                .Create(clientId)
                .WithClientSecret(secret)
                .WithAuthority(authority)
                .Build();

            var token = await confidentialClientApplication
                .AcquireTokenForClient(new[] { $"{audience}/.default" })
                .ExecuteAsync()
                .ConfigureAwait(false);

            return token?.AccessToken;
        }

        public async Task<string> GetTokenAsync(string AppIdKeyVaultSecretName, string AppSecretKeyVaultSecretName, string tenantId, string audienceId)
        {
            return await this.GetToken(
                    await this.KeyVaultClient.GetSecretValueFromKeyVaultAsync(AppIdKeyVaultSecretName).ConfigureAwait(false),
                    await this.KeyVaultClient.GetSecretValueFromKeyVaultAsync(AppSecretKeyVaultSecretName).ConfigureAwait(false),
                    tenantId,
                    audienceId).ConfigureAwait(false);
        }
    }
}
