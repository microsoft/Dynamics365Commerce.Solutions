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
    namespace CommerceRuntime.DocumentProvider.EpsonFP90IIISample.Helpers
    {
        using Contoso.CommerceRuntime.DocumentProvider.EpsonFP90IIISample.DocumentBuilders;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;

        /// <summary>
        /// Builds DocumentElementAdjustmentBuilder to create printRecItemAdjustment attribute for the fiscal receipt document considering the amount of discounts applied.
        /// </summary>
        public class DocumentElementAdjustmentBuilderFactory
        {
            /// <summary>
            /// The version of configuration used.
            /// </summary>
            private ConfigurationVersion ConfigurationVersion;

            /// <summary>
            /// The boolean value of EnableFreeOfChargeItems property.
            /// </summary>
            private bool EnableFreeOfChargeItems;

            /// <summary>
            /// Initializes a new instance of the <see cref="DocumentElementAdjustmentBuilderFactory "/> class.
            /// </summary>
            /// <param name="configurationVersion">Configuration version section value of document provider manifest.</param>
            /// <param name="isFreeOfChargItemsEnabled">EnableFreeOfChargeItems section value of document provider manifest.</param>
            public DocumentElementAdjustmentBuilderFactory(ConfigurationVersion configurationVersion, bool isFreeOfChargeItemsEnabled)
            {
                ConfigurationVersion = configurationVersion;
                EnableFreeOfChargeItems = isFreeOfChargeItemsEnabled;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="DocumentElementAdjustmentBuilderFactory "/> class.
            /// </summary>
            /// <param name="fiscalIntegrationFunctionalityProfile">The fiscal integration functionality profile.</param>
            public DocumentElementAdjustmentBuilderFactory(FiscalIntegrationFunctionalityProfile fiscalIntegrationFunctionalityProfile)
            {
                ConfigurationVersion = ConfigurationController.GetConfigurationVersion(fiscalIntegrationFunctionalityProfile);
                EnableFreeOfChargeItems = ConfigurationController.ParseEnableFreeOfChargeItemsProperty(fiscalIntegrationFunctionalityProfile);
            }

            /// <summary>
            /// Builds the concrete DocumentElementAdjustmentBuilder based on the configuration version.
            /// </summary>
            /// <returns>DocumentElementAdjustmentBuilder.</returns>
            public IDocumentElementAdjustmentBuilder Build()
            {
                switch (ConfigurationVersion)
                {
                    case ConfigurationVersion.RT2 when EnableFreeOfChargeItems:
                        return new DocumentElementAdjustmentBuilderRT2();
                    default:
                        return new DocumentElementAdjustmentBuilderRT();
                }
            }
        }
    }
}
