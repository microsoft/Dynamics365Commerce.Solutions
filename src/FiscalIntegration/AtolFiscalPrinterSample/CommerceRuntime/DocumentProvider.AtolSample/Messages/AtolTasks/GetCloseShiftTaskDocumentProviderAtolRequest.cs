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
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;

        /// <summary>
        /// The request to get the close shift task document.
        /// </summary>
        public class GetCloseShiftTaskDocumentProviderAtolRequest : Request
        {
            /// <summary>
            /// Initializes  a new instance of the <see cref="GetCloseShiftTaskDocumentProviderAtolRequest"/> class.
            /// </summary>
            /// <param name="profile">The connector functionality profile.</param>
            public GetCloseShiftTaskDocumentProviderAtolRequest(FiscalIntegrationFunctionalityProfile profile)
            {
                ThrowIf.Null(profile, nameof(profile));
                this.FiscalIntegrationFunctionalityProfile = profile;
            }

            /// <summary>
            /// Gets fiscal integration functionality profile.
            /// </summary>
            public FiscalIntegrationFunctionalityProfile FiscalIntegrationFunctionalityProfile { get; }
        }
    }
}
