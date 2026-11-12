/**
 * SAMPLE CODE NOTICE
 * 
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

namespace Contoso.Commerce.Runtime.ExtendedLogonSample
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Handlers;


    public abstract class UniqueSecretExtendedAuthenticationService : INamedRequestHandlerAsync
    {
        // If your credential requires 256 bits, the length in Base64 will be 44.
        private const int RequiredCredentialLength = 44;
        private const int CredentialIdLength = 10;
        private const string ExtraParameterPinKey = "pin";

        /// <summary>
        /// Gets the unique name for this request handler.
        /// </summary>
        public abstract string HandlerName { get; }


        public IEnumerable<Type> SupportedRequestTypes
        {
            get
            {
                return new[]
                {
                    typeof(GetUserAuthenticationCredentialIdServiceRequest),
                    typeof(OverrideUserCredentialServiceRequest),
                    
                    /* CAUTION: If you want to use the out-of-box implementation of requests GetUserEnrollmentDetailsServiceRequest
                     and ConfirmUserAuthenticationServiceRequest, comment out them as the following.
                     Furthermore, in the Execute() method below, have a case switch to delegate the execution to the Commerce Runtime.
                     If you want to have your own implementation of requests GetUserEnrollmentDetailsServiceRequest
                     and ConfirmUserAuthenticationServiceRequest, remove the comments below and have your own implementation in the Execute() method.
                    */

                    // typeof(GetUserEnrollmentDetailsServiceRequest),
                    // typeof(ConfirmUserAuthenticationServiceRequest),
                };
            }
        }

        /// <summary>
        /// Executes the request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The response.</returns>
        public async Task<Response> Execute(Request request)
        {
            ThrowIf.Null(request, nameof(request));

            switch (request)
            {    
                case GetUserAuthenticationCredentialIdServiceRequest getUserAuthenticationCredentialIdServiceRequest:
                    return await this.GetUserAuthenticationCredentialId(getUserAuthenticationCredentialIdServiceRequest);
                case OverrideUserCredentialServiceRequest overrideUserCredentialServiceRequest:
                    return await this.OverrideUserCredential(overrideUserCredentialServiceRequest);
                // following requests are delegated to call out-of-box implementation
                case ConfirmUserAuthenticationServiceRequest confirmUserAuthenticationServiceRequest:
                case GetUserEnrollmentDetailsServiceRequest getUserEnrollmentDetailsServiceRequest:
                    return await request.RequestContext.ExecuteAsync<Response>(request);
                default:
                    return NotHandledResponse.Instance;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the service is enabled.
        /// </summary>
        /// <param name="deviceConfiguration">The device configuration.</param>
        /// <returns>A value indicating whether the service is enabled.</returns>
        protected abstract bool IsServiceEnabled(DeviceConfiguration deviceConfiguration);

        /// <summary>
        /// Gets a value indicating whether the service is requires the user password as a second factor authentication.
        /// </summary>
        /// <param name="deviceConfiguration">The device configuration.</param>
        /// <returns>A value indicating whether the service is requires the user password as a second factor authentication.</returns>
        protected abstract bool IsPasswordRequired(DeviceConfiguration deviceConfiguration);

        /// <summary>
        /// Confirms whether the user input credential is correct.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The response.</returns>
        private async Task<Response> OverrideUserCredential(OverrideUserCredentialServiceRequest request)
        {
            string pin = string.Empty;
            IDictionary<string, object> extraParameters = request.ExtraParameters;

            // The key name 'pin' must align with the key name defined in extraParameters from those trigger options
            if (extraParameters != null && extraParameters.ContainsKey(ExtraParameterPinKey))
            {
                pin = extraParameters[ExtraParameterPinKey].ToString();
            }

            // Here we simply use string concat to combine old credential and pin number, and the new overridden credential will be used as response.
            // You can use other function to update the credential per requirement.
            // Note that pin number will not be persisted in database, instead, the hashed value of the overridden credential will.
            string credential = request.Credential + pin;
            return await Task.FromResult(new OverrideUserCredentialServiceResponse(credential) as Response);
        }

        /// <summary>
        /// Gets the user credential identifier.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The response.</returns>
        private Task<Response> GetUserAuthenticationCredentialId(GetUserAuthenticationCredentialIdServiceRequest request)
        {
            string credential = request.Credential;

            DeviceConfiguration deviceConfiguration = request.RequestContext.GetDeviceConfiguration();

            // check whether extended logon is enabled for the current store.
            if (!this.IsServiceEnabled(deviceConfiguration))
            {
                throw new UserAuthenticationException(SecurityErrors.Microsoft_Dynamics_Commerce_Runtime_AuthenticationMethodDisabled, "Authentication service is disabled.");
            }

            if (string.IsNullOrWhiteSpace(request.Credential))
            {
                throw new DataValidationException(DataValidationErrors.Microsoft_Dynamics_Commerce_Runtime_MissingParameter, "credential");
            }

            // minimum length required for user credential, change this value of RequiredCredentialLength to meet your business requirement.
            if (credential.Length < RequiredCredentialLength)
            {
                throw new InsufficientCredentialLengthException(DataValidationErrors.Microsoft_Dynamics_Commerce_Runtime_InvalidFormat, credential.Length, RequiredCredentialLength);
            }

            // Credential id can be generated by credential of the request plus the extraParameter dictionary.
            // Here we just simply use substring of the credential as the credential id, change this value of CredentialIdLength to meet your business requirement.
            // Credential id MUST be unique accross employees.
            string credentialId = credential.Substring(0, CredentialIdLength);

            return Task.FromResult(new GetUserAuthenticationCredentialIdServiceResponse(credentialId) as Response);
        }

    }
}
