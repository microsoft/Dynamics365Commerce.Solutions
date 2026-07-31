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
    namespace CommerceRuntime.DocumentProvider.EpsonFP90IIISample.DocumentBuilders
    {
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;

        /// <summary>
        /// Factory for creating instances of IPaymentInfoResolver.
        /// </summary>
        public class PaymentInfoResolverFactory
        {
            /// <summary>
            /// GetFiscalDocumentDocumentProvider request.
            /// </summary>
            private FiscalIntegrationFunctionalityProfile FiscalIntegrationFunctionalityProfile { get; }

            /// <summary>
            /// The version of configuration used.
            /// </summary>
            private ConfigurationVersion ConfigurationVersion { get; }

            /// <summary>
            /// Constructor for PaymentInfoResolverFactory class.
            /// </summary>
            /// <param name="fiscalIntegrationFunctionalityProfile">FiscalIntegrationFunctionalityProfile profile.</param>
            public PaymentInfoResolverFactory(FiscalIntegrationFunctionalityProfile fiscalIntegrationFunctionalityProfile)
            {
                FiscalIntegrationFunctionalityProfile = fiscalIntegrationFunctionalityProfile;
                ConfigurationVersion = ConfigurationController.GetConfigurationVersion(FiscalIntegrationFunctionalityProfile);
            }

            /// <summary>
            /// Creates instance of IPaymentInfoResolver based on configuration version.
            /// </summary>
            /// <returns>The instance of IPaymentInfoResolver.</returns>
            public IPaymentInfoResolver Build()
            {
                return ConfigurationVersion == ConfigurationVersion.RT2
                    ? new PaymentInfoResolverRT2(FiscalIntegrationFunctionalityProfile)
                    : (IPaymentInfoResolver)new PaymentInfoResolverRT();
            }
        }
    }
}
