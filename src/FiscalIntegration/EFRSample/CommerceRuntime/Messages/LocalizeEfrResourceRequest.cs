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
        /// The request to localize a string resource according to a culture name.
        /// </summary>
        public sealed class LocalizeEfrResourceRequest : Request
        {
            /// <summary>
            /// Initializes a new instance of <see cref="LocalizeEfrResourceRequest"/>.
            /// </summary>
            /// <param name="cultureName">The name of the culture.</param>
            /// <param name="textId">The short text identifier (without the namespace).</param>
            public LocalizeEfrResourceRequest(string cultureName, string textId)
            {
                this.CultureName = cultureName;
                this.TextId = textId;
            }

            /// <summary>
            /// The culture name.
            /// </summary>
            public string CultureName { get; }

            /// <summary>
            /// The text identifier.
            /// </summary>
            public string TextId { get; }
        }
    }
}
