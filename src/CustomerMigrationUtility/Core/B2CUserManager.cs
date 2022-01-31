namespace Microsoft.Dynamics.Commerce.CustomerMigrationUtility.Core
{
    using Microsoft.IdentityModel.Clients.ActiveDirectory;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>The B2C user manager class.</summary>
    public class B2CUserManager
    {
        /// <summary>The group service resource id.</summary>
        private static string aadGraphResourceId = "https://graph.windows.net/";

        /// <summary>The API version.</summary>
        private static string aadGraphVersion = "api-version=1.6";

        /// <summary>The B2C shared custom domain.</summary>
        private const string B2CSharedDomain = "{0}.b2clogin.com";

        /// <summary>The authentication result.</summary>
        private static AuthenticationResult authenticationResult = null;

        /// <summary>The authentication context.</summary>
        private AuthenticationContext authenticationContext;

        /// <summary>The client credential.</summary>
        private ClientCredential clientCredential;

        /// <summary>The logger.</summary>
        private Logger logger;

        /// <summary>Initializes a new instance of the <see cref="B2CUserManager" /> class.</summary>  
        /// <param name="tenant">The B2C tenant id.</param>
        /// <param name="clientId">The B2C client id.</param>
        /// <param name="clientSecret">The B2C client secret.</param>
        public B2CUserManager(string tenant, string clientId, string clientSecret)
        {
            this.authenticationContext = new AuthenticationContext("https://login.microsoftonline.com/" + tenant);
            this.clientCredential = new ClientCredential(clientId, clientSecret);
            this.logger = new Logger();
        }

        /// <summary>
        /// Retrieves a list of all users.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<B2CUser> GetAllB2CAccounts(string tenant)
        {
            HttpResponseMessage httpResponse = null;
            JObject jsonObject = null;
            var httpClient = new HttpClient();

            try
            {
                var userId = string.Empty;

                if (authenticationResult == null || authenticationResult.ExpiresOn < DateTimeOffset.UtcNow.AddMinutes(5))
                {
                    this.logger.Trace("Acquiring the access token...");
                    authenticationResult = this.authenticationContext.AcquireTokenAsync(aadGraphResourceId, this.clientCredential).Result;
                    this.logger.Trace("Acquiring the access token is done.");
                }

                // Creating authentication header
                var authenticationHeaderValue = new AuthenticationHeaderValue("Bearer", authenticationResult.AccessToken);

                if (!tenant.EndsWith(".onmicrosoft.com"))
                {
                    tenant = tenant + ".onmicrosoft.com";
                }

                string url = $"{aadGraphResourceId}{tenant}/users?{aadGraphVersion}";
                httpResponse = httpClient.GetAsync(url, authenticationHeaderValue).Result;
                if (!httpResponse.IsSuccessStatusCode)
                {
                    var errorMessage = string.Empty;
                }

                var response = httpClient.GetContentAsync(httpResponse).Result;
                jsonObject = JObject.Parse(response);
            }
            catch (Exception ex)
            {
                var errorMessage = "Exception : " + ex.Message.ToString() + "\n";

                if (ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message.ToString()))
                {
                    errorMessage += "InnerException : " + ex.InnerException.Message.ToString() + "\n";
                }

                if (httpResponse != null)
                {
                    var response = httpClient.GetContentAsync(httpResponse).Result;
                    if (!string.IsNullOrEmpty(response))
                    {
                        errorMessage += "Service Failure Detail : " + response + "\n";
                    }
                }

                this.logger.Error(errorMessage);
            }

            foreach (var user in jsonObject["value"])
            {
                string customerAccountNumber = (string)user["signInNames"].SingleOrDefault(v => (string)v["type"] == "username")["value"];
                yield return new B2CUser((string)user["givenName"], (string)user["surname"], tenant, customerAccountNumber, null);
            }
        }


        /// <summary>Creating B2C account through Graph Api.</summary>
        /// <param name="user">The user to create.</param>
        /// <param name="updatePasswordInExistingAccount">Flag to indicate whether the password for the existing account should be updated.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        public async Task<Response> CreateB2CAccount(B2CUser user, bool updatePasswordInExistingAccount)
        {
            HttpResponseMessage httpResponse = null;
            var httpClient = new HttpClient();

            try
            {
                var userId = string.Empty;

                if (authenticationResult == null || authenticationResult.ExpiresOn < DateTimeOffset.UtcNow.AddMinutes(5))
                {
                    this.logger.Trace("Acquiring the access token...");
                    authenticationResult = await this.authenticationContext.AcquireTokenAsync(aadGraphResourceId, this.clientCredential);
                    this.logger.Trace("Acquiring the access token is done.");
                }

                // Creating authentication header
                var authenticationHeaderValue = new AuthenticationHeaderValue("Bearer", authenticationResult.AccessToken);

                if (!user.Tenant.EndsWith(".onmicrosoft.com"))
                {
                    user.Tenant = $"{user.Tenant}.onmicrosoft.com";
                }

                this.logger.Trace($"Checking the user account the email: {user.EMail} in B2C");

                string url = null;

                if (string.IsNullOrWhiteSpace(user.ExternalIssuerUserId))
                {
                    url = $"{aadGraphResourceId}{user.Tenant}/users?$filter=signInNames/any(x:x/value%20eq%20%27{WebUtility.UrlEncode(user.EMail)}%27)&{aadGraphVersion}";
                }
                else
                {
                    var hex = ToBase16String(Encoding.UTF8.GetBytes(user.ExternalIssuerUserId));
                    url = $"{aadGraphResourceId}{user.Tenant}/users?$filter=userIdentities/any(x:x/issuer eq '{user.ExternalIssuer}' and x/issuerUserId eq X'{hex}')&{aadGraphVersion}";
                }

                httpResponse = await httpClient.GetAsync(url, authenticationHeaderValue);
                var response = await httpClient.GetContentAsync(httpResponse);
                if (!httpResponse.IsSuccessStatusCode)
                {
                    var errorMessage = string.Empty;
                    if (!string.IsNullOrEmpty(response))
                    {
                        errorMessage += "Service Failure Detail : " + response + "\n";
                    }

                    this.logger.Error(errorMessage);
                    this.logger.Trace($"Checking the user account with email: {user.EMail} in B2C is failed.");
                }

                response = await httpClient.GetContentAsync(httpResponse);

                if (response.IndexOf(user.EMail, StringComparison.OrdinalIgnoreCase) < 0)
                {
                    url = $"{aadGraphResourceId}{user.Tenant}/users?{aadGraphVersion}";

                    this.logger.Trace($"Creating new account with ({user.EMail}) in B2C.");
                    httpResponse = await httpClient.PostAsync(user.CreateAccountRequestObject(), url, authenticationHeaderValue);

                    if (!httpResponse.IsSuccessStatusCode)
                    {
                        this.logger.Trace($"Creating new account with ({user.EMail}) in B2C is failed.");
                        return new Response(Status.Failed);
                    }

                    response = await httpClient.GetContentAsync(httpResponse);
                    var jsonObject = JObject.Parse(response);

                    return new Response(Status.Success, jsonObject["objectId"].ToString());
                }
                else
                {
                    var jsonObject = JObject.Parse(response);
                    var existinguserId = jsonObject["value"][0]["objectId"].ToString();

                    if (updatePasswordInExistingAccount)
                    {
                        var requestObject = new
                        {
                            passwordProfile = new
                            {
                                password = user.TemporaryPassword,
                                forceChangePasswordNextLogin = false
                            }
                        };

                        var objectId = jsonObject["value"][0]["objectId"].ToString();

                        url = $"{aadGraphResourceId}{user.Tenant}/users/{objectId}?{aadGraphVersion}";

                        this.logger.Trace($"Update the new password for the account: {user.EMail} in B2C.");
                        httpResponse = await httpClient.PatchAsync(requestObject, url, authenticationHeaderValue);

                        response = await httpClient.GetContentAsync(httpResponse);
                        if (!httpResponse.IsSuccessStatusCode)
                        {
                            this.logger.Trace($"Update the new password for the account: {user.EMail}) in B2C is failed.");
                            return new Response(Status.Failed, existinguserId);
                        }
                        else
                        {
                            return new Response(Status.Success, existinguserId);
                        }
                    }

                    return new Response(Status.AlreadyExist, existinguserId);
                }
            }
            catch (Exception ex)
            {
                var errorMessage = "Exception : " + ex.Message.ToString() + "\n";

                if (ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message.ToString()))
                {
                    errorMessage += "InnerException : " + ex.InnerException.Message.ToString() + "\n";
                }

                if (httpResponse != null)
                {
                    var response = await httpClient.GetContentAsync(httpResponse);
                    if (!string.IsNullOrEmpty(response))
                    {
                        errorMessage += "Service Failure Detail : " + response + "\n";
                    }
                }

                this.logger.Error(errorMessage);
            }

            return new Response(Status.Failed);
        }

        /// <summary>Disable B2C account through Graph Api.</summary>
        /// <param name="user">The user to disable</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        public async Task<Response> DisableB2CAccount(B2CUser user)
        {
            HttpResponseMessage httpResponse = null;
            var httpClient = new HttpClient();
            Status status = Status.NotFound;

            try
            {
                var userId = string.Empty;

                if (authenticationResult == null || authenticationResult.ExpiresOn < DateTimeOffset.UtcNow.AddMinutes(5))
                {
                    this.logger.Trace("Acquiring the access token...");
                    authenticationResult = await this.authenticationContext.AcquireTokenAsync(aadGraphResourceId, this.clientCredential);
                    this.logger.Trace("Acquiring the access token is done.");
                }

                // Creating authentication header
                var authenticationHeaderValue = new AuthenticationHeaderValue("Bearer", authenticationResult.AccessToken);

                if (!user.Tenant.EndsWith(".onmicrosoft.com"))
                {
                    user.Tenant = user.Tenant + ".onmicrosoft.com";
                }

                this.logger.Trace($"Checking the user account: {user.CustomerAccountNumber} in B2C");
                string url = $"{aadGraphResourceId}{user.Tenant}/users?$filter=signInNames/any(x:x/value%20eq%20%27{WebUtility.UrlEncode(user.CustomerAccountNumber)}%27)&{aadGraphVersion}";

                httpResponse = await httpClient.GetAsync(url, authenticationHeaderValue);

                if (!httpResponse.IsSuccessStatusCode)
                {
                    this.logger.Trace($"Checking the user account: {user.CustomerAccountNumber} in B2C is failed.");
                    return new Response(Status.Failed);
                }

                var response = await httpClient.GetContentAsync(httpResponse);

                if (response.IndexOf(user.CustomerAccountNumber, StringComparison.OrdinalIgnoreCase) < 0)
                {
                    return new Response(Status.NotFound);
                }
                else
                {
                    var jsonObject = JObject.Parse(response);
                    var objectId = jsonObject["value"][0]["objectId"].ToString();
                    url = $"{aadGraphResourceId}{user.Tenant}/users/{objectId}?{aadGraphVersion}";

                    var requestObject = new { accountEnabled = false };

                    this.logger.Trace($"Disabling the account: {user.CustomerAccountNumber} in B2C.");
                    httpResponse = httpClient.PatchAsync(requestObject, url, authenticationHeaderValue).Result;

                    response = await httpClient.GetContentAsync(httpResponse);
                    if (!httpResponse.IsSuccessStatusCode)
                    {
                        status = Status.Failed;
                        this.logger.Trace($"Disabling the account: {user.CustomerAccountNumber}) in B2C is failed.");
                        return new Response(Status.Failed, objectId);
                    }
                    else
                    {
                        return new Response(Status.Disabled, objectId);
                    }
                }
            }
            catch (Exception ex)
            {
                var errorMessage = "Exception : " + ex.Message.ToString() + "\n";

                if (ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message.ToString()))
                {
                    errorMessage += "InnerException : " + ex.InnerException.Message.ToString() + "\n";
                }

                if (httpResponse != null)
                {
                    var response = await httpClient.GetContentAsync(httpResponse);
                    if (!string.IsNullOrEmpty(response))
                    {
                        errorMessage += "Service Failure Detail : " + response + "\n";
                    }
                }

                this.logger.Error(errorMessage);
            }

            return new Response(Status.Failed);
        }

        /// <summary>Force the user the change the password at next login.</summary>
        /// <param name="UserId">The user object id.</param> 
        /// <param name="tenant">The B2C tenant</param>            
        /// <returns>The <see cref="ActionResult"/>.</returns>
        public async Task<Response> ForceChangePasswordNextLogin(
            string UserId,
            string tenant)
        {
            HttpResponseMessage httpResponse = null;
            var httpClient = new HttpClient();

            try
            {
                var userId = string.Empty;

                if (authenticationResult == null || authenticationResult.ExpiresOn < DateTimeOffset.UtcNow.AddMinutes(5))
                {
                    authenticationResult = await this.authenticationContext.AcquireTokenAsync(aadGraphResourceId, this.clientCredential);
                }

                // Creating authentication header
                var authenticationHeaderValue = new AuthenticationHeaderValue("Bearer", authenticationResult.AccessToken);

                if (!tenant.EndsWith(".onmicrosoft.com"))
                {
                    tenant = tenant + ".onmicrosoft.com";
                }
                var requestObject = new { passwordProfile = new { forceChangePasswordNextLogin = true } };

                var url = aadGraphResourceId + tenant + "/users/" + UserId + "?" + aadGraphVersion;

                httpResponse = await httpClient.PatchAsync(requestObject, url, authenticationHeaderValue);

                return new Response(!httpResponse.IsSuccessStatusCode ? Status.Failed : Status.Success, UserId);
            }
            catch (Exception ex)
            {
                var errorMessage = "Exception : " + ex.Message.ToString() + "\n";

                if (ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message.ToString()))
                {
                    errorMessage += "InnerException : " + ex.InnerException.Message.ToString() + "\n";
                }

                if (httpResponse != null)
                {
                    var response = await httpClient.GetContentAsync(httpResponse);
                    if (!string.IsNullOrEmpty(response))
                    {
                        errorMessage += "Service Failure Detail : " + response + "\n";
                    }
                }

                this.logger.Error(errorMessage);
            }

            return new Response(Status.Failed, UserId);
        }


        /// <summary>Retrieving the B2C token with test account for test automation.</summary>
        /// <param name="currentTenantConfig">The current tenant config</param>
        /// <param name="userAccount">The user account</param>
        /// <param name="password">The password</param>
        /// <returns>B2C token</returns>
        public async Task<string> AcquireB2CTokenByUsernamePasswordAsync(
            string userName,
            string password,
            string tenant,
            string nonInteractiveClientId,
            string nonInteractivePolicyId,
            string loginDomain,
            string scope
            )
        {
            if (string.IsNullOrWhiteSpace(tenant) ||
                string.IsNullOrWhiteSpace(nonInteractiveClientId) ||
                string.IsNullOrWhiteSpace(userName) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(loginDomain) ||
                string.IsNullOrWhiteSpace(nonInteractiveClientId) ||
                string.IsNullOrWhiteSpace(scope))
            {
                return null;
            }

            if (tenant.EndsWith(".onmicrosoft.com"))
            {
                tenant = tenant.Replace(".onmicrosoft.com", string.Empty);
            }

            // Generating the password in secured string
            var secureString = new SecureString();
            foreach (char chr in password.ToCharArray())
            {
                secureString.AppendChar(chr);
            }

            try
            {
                Microsoft.Identity.Client.IPublicClientApplication app = Microsoft.Identity.Client.PublicClientApplicationBuilder
                   .Create(nonInteractiveClientId)
                   .WithB2CAuthority(string.Format("https://{0}/tfp/{1}.onmicrosoft.com/{2}/oauth2/v2.0/authorize", loginDomain, tenant, nonInteractivePolicyId))
                   .Build();

                var authResult = await app.AcquireTokenByUsernamePassword(new string[] { scope }, userName, secureString).ExecuteAsync();

                return authResult?.AccessToken;
            }
            catch (Exception ex)
            {
                var errorMessage = "Exception : " + ex.Message.ToString() + "\n";

                if (ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message.ToString()))
                {
                    errorMessage += "InnerException : " + ex.InnerException.Message.ToString() + "\n";
                }

                this.logger.Error(errorMessage, false);
                return null;
            }
        }

        /// <summary>Generating the password.</summary>
        /// <param name="length">The password length.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        public string CreatePassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890_";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (length > 0)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
                length--;
            }

            return res.ToString();
        }

        private string ToBase16String(byte[] inArray)
        {
            StringBuilder result = new StringBuilder(inArray.Length * 2);

            for (int i = 0; i < inArray.Length; i++)
            {
                result.AppendFormat("{0:X}", inArray[i]);
            }

            return result.ToString();
        }
    }
}
