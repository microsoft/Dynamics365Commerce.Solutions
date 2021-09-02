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
        /// Factory for creating instances of IDepartmentResolver.
        /// </summary>
        public class DepartmentResolverFactory
        {
            /// <summary>
            /// The functionality profile.
            /// </summary>
            private FiscalIntegrationFunctionalityProfile FiscalIntegrationFunctionalityProfile { get; }

            /// <summary>
            /// The version of configuration used.
            /// </summary>
            private ConfigurationVersion ConfigurationVersion { get; }

            /// <summary>
            /// Constructor for DepartmentResolverFactory class.
            /// </summary>
            /// <param name="fiscalIntegrationFunctionalityProfile">The functionality profile.</param>
            public DepartmentResolverFactory(FiscalIntegrationFunctionalityProfile fiscalIntegrationFunctionalityProfile)
            {
                FiscalIntegrationFunctionalityProfile = fiscalIntegrationFunctionalityProfile;
                ConfigurationVersion = ConfigurationController.GetConfigurationVersion(FiscalIntegrationFunctionalityProfile);
            }

            /// <summary>
            /// Creates instance of IDepartmentResolver based on configuration version.
            /// </summary>
            /// <returns>The instance of IDepartmentResolver.</returns>
            public IDepartmentResolver Build()
            {
                return ConfigurationVersion == ConfigurationVersion.RT2
                    ? new DepartmentResolverRT2(FiscalIntegrationFunctionalityProfile)
                    : (IDepartmentResolver)new DepartmentResolverRT(FiscalIntegrationFunctionalityProfile);
            }
        }
    }
}