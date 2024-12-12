namespace Contoso.Commerce.Runtime.ExtendedLogonSample
{
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;

    /// <summary>
    /// Extended authentication service for magnetic cards.
    /// </summary>
    public class MagneticCardExtendedAuthenticationService : UniqueSecretExtendedAuthenticationService
    {
        /// <summary>
        /// Gets the unique name for this request handler.
        /// </summary>
        public override string HandlerName
        {
            get
            {
                // DO NOT change the value as it's hardcoded in POS.
                return "auth://example.auth.contoso.com/msr";
            }
        }

        /// <summary>
        /// Gets a value indicating whether the service is enabled.
        /// </summary>
        /// <param name="deviceConfiguration">The device configuration.</param>
        /// <returns>A value indicating whether the service is enabled.</returns>
        protected override bool IsServiceEnabled(DeviceConfiguration deviceConfiguration)
        {
            ThrowIf.Null(deviceConfiguration, nameof(deviceConfiguration));
            return deviceConfiguration.StaffCardLogOn;
        }

        /// <summary>
        /// Gets a value indicating whether the service is requires the user password as a second factor authentication.
        /// </summary>
        /// <param name="deviceConfiguration">The device configuration.</param>
        /// <returns>A value indicating whether the service is requires the user password as a second factor authentication.</returns>
        protected override bool IsPasswordRequired(DeviceConfiguration deviceConfiguration)
        {
            ThrowIf.Null(deviceConfiguration, nameof(deviceConfiguration));
            return deviceConfiguration.StaffCardLogOnRequiresPassword;
        }
    }
}
