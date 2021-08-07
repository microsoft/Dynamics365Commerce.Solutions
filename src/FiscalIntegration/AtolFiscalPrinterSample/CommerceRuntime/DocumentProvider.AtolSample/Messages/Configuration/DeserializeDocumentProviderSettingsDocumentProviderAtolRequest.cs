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
    namespace CommerceRuntime.DocumentProvider.AtolSample.Messages
    {
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;

        /// <summary>
        /// The service request to deserialize document provider settings.
        /// </summary>
        public class DeserializeDocumentProviderSettingsDocumentProviderAtolRequest : Request
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DeserializeDocumentProviderSettingsDocumentProviderAtolRequest"/> class.
            /// </summary>
            /// <param name="properties">Fiscal integration functionality profile properties.</param>
            public DeserializeDocumentProviderSettingsDocumentProviderAtolRequest(FiscalIntegrationFunctionalityProfile properties)
            {
                this.ConfigurationProperties = properties;
            }

            /// <summary>
            /// Gets configuration properties.
            /// </summary>
            public FiscalIntegrationFunctionalityProfile ConfigurationProperties { get; private set; }
        }
    }
}
