using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Dynamic;
using System.Text;

namespace Microsoft.Dynamics.Commerce.CustomerMigrationUtility.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class B2CUser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="B2CUser"/> class.
        /// </summary>
        /// <param name="givenName">The given name of the user.</param>
        /// <param name="surname">The surname of the user.</param>
        /// <param name="tenant">The tenant of the user.</param>
        /// <param name="customerAccountNumber">The customer account number of the user.</param>
        /// <param name="email">The e-Mail address of the user.</param>
        /// <param name="externalIssuer">The external issuer of the user.</param>
        /// <param name="externalIssuerUserId">The external issues user id of the user.</param>
        public B2CUser(string givenName, string surname, string tenant, string customerAccountNumber, string email, string externalIssuer, string externalIssuerUserId)
        {
            if (string.IsNullOrWhiteSpace(givenName))
            {
                throw new ArgumentNullException("givenName");
            }

            if (string.IsNullOrWhiteSpace(surname))
            {
                throw new ArgumentNullException("surname");
            }

            this.GivenName = givenName;
            this.Surname = surname;
            this.Tenant = tenant;
            this.CustomerAccountNumber = customerAccountNumber;
            this.EMail = email;
            this.ExternalIssuer = externalIssuer;
            this.ExternalIssuerUserId = externalIssuerUserId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="B2CUser"/> class.
        /// </summary>
        /// <param name="givenName">The given name of the user.</param>
        /// <param name="surname">The surname of the user.</param>
        /// <param name="tenant">The tenant of the user.</param>
        /// <param name="customerAccountNumber">The customer account number of the user.</param>
        /// <param name="email">The e-Mail address of the user.</param>
        public B2CUser(string givenName, string surname, string tenant, string customerAccountNumber, string email) : this(givenName, surname, tenant, customerAccountNumber, email, null, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="B2CUser"/> class.
        /// </summary>
        /// <param name="givenName">The given name of the user.</param>
        /// <param name="surname">The surname of the user.</param>
        /// <param name="tenant">The tenant of the user.</param>
        public B2CUser(string givenName, string surname, string tenant) : this(givenName, surname, tenant, null, null) { }

        /// <summary>
        /// Gets or sets the first name of the user.
        /// </summary>
        public string Surname { get; set; }

        /// <summary>
        /// Gets or sets the last name of the user.
        /// </summary>
        public string GivenName { get; set; }

        /// <summary>
        /// Gets or sets the Customer Account Number of the user.
        /// </summary>
        [JsonIgnore]
        public string CustomerAccountNumber { get; set; }

        /// <summary>
        /// Gets or sets the e-Mail address of the user.
        /// </summary>
        [JsonIgnore]
        public string EMail { get; set; }

        /// <summary>
        /// Gets or sets the external issuer of the user.
        /// </summary>
        public string ExternalIssuer { get; set; }

        /// <summary>
        /// Gets or sets the external issue user identifier of the user.
        /// </summary>
        public string ExternalIssuerUserId { get; set; }

        /// <summary>
        /// Gets or set the tenant of the user.
        /// </summary>
        public string Tenant { get; set; }

        /// <summary>
        /// Gets or sets a temporary password for the user.
        /// </summary>
        public string TemporaryPassword { get; set; }

        /// <summary>
        /// Indicates whether the user has a mapped external identity.
        /// </summary>
        /// <returns>Flag indicating whether the user has an external identify.</returns>
        public bool HasExternalIdentity()
        {
            return !string.IsNullOrWhiteSpace(this.ExternalIssuer) && !string.IsNullOrWhiteSpace(this.ExternalIssuerUserId);
        }

        /// <summary>
        /// Creates a request object to generate a account in AAD B2C.
        /// </summary>
        /// <returns>The local account request object.</returns>
        public object CreateAccountRequestObject(string b2cTenant)
        {
            dynamic accountRequestObject = new ExpandoObject();

            accountRequestObject.accountEnabled = true;
            accountRequestObject.surName = this.Surname;
            accountRequestObject.givenName = this.GivenName;
            accountRequestObject.displayName = $"{this.GivenName} {this.Surname}";
            accountRequestObject.passwordPolicies = "DisablePasswordExpiration";
            accountRequestObject.passwordProfile = new
            {
                password = this.TemporaryPassword,
                forceChangePasswordNextSignIn = false
            };

            if (this.HasExternalIdentity())
            {
                accountRequestObject.otherMails = new[] { this.EMail };
                accountRequestObject.userPrincipalName = $"{Guid.NewGuid().ToString()}@{this.Tenant}";
                accountRequestObject.identities = new[] { new { signInType = "federated", issuer = this.ExternalIssuer, issuerAssignedId = this.ExternalIssuerUserId }, new { signInType = "AXCustomerAccountNumber", issuer = b2cTenant, issuerAssignedId = this.CustomerAccountNumber } };
            }
            else
            {
                accountRequestObject.identities = new[] { new { signInType = "emailAddress", issuer = b2cTenant, issuerAssignedId = this.EMail }, new { signInType = "AXCustomerAccountNumber", issuer = b2cTenant, issuerAssignedId = this.CustomerAccountNumber } };
            }

            return accountRequestObject;
        }
    }
}
