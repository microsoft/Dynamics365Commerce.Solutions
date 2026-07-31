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
        using Contoso.CommerceRuntime.DocumentProvider.AtolSample.DataModel.Configuration;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;

        /// <summary>
        /// The service response to deserialize document provider settings.
        /// </summary>
        public class DeserializeDocumentProviderSettingsDocumentProviderAtolResponse : Response
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DeserializeDocumentProviderSettingsDocumentProviderAtolResponse"/> class.
            /// </summary>
            /// <param name="settings">The document provider settings.</param>
            public DeserializeDocumentProviderSettingsDocumentProviderAtolResponse(DocumentProviderSettings settings)
            {
                this.DocumentProviderSettings = settings;
            }

            /// <summary>
            /// Gets document provider settings.
            /// </summary>
            public DocumentProviderSettings DocumentProviderSettings { get; private set; }
        }
    }
}
