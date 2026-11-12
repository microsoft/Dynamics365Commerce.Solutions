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
    namespace CommerceRuntime.DocumentProvider.EFRSample.Messages
    {
        using Microsoft.Dynamics.Commerce.Runtime.Messages;

        /// <summary>
        /// The request to localize an EFR resource.
        /// </summary>
        public sealed class LocalizeEfrResourceRequest : Request
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="LocalizeEfrResourceRequest"/> class.
            /// </summary>
            /// <param name="cultureName">The culture name.</param>
            /// <param name="textId">The text identifier.</param>
            public LocalizeEfrResourceRequest(string cultureName, string textId)
            {
                this.CultureName = cultureName;
                this.TextId = textId;
            }

            /// <summary>
            /// Gets the culture name.
            /// </summary>
            public string CultureName { get; }

            /// <summary>
            /// Gets the text identifier.
            /// </summary>
            public string TextId { get; }
        }
    }
}
