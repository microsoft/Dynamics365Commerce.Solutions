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
        using System;

        /// <summary>
        /// Provides localization services for fiscal document strings using a lazily initialized string localizer.
        /// </summary>
        public static class DocumentLocalizerHelper
        {
            private static readonly Lazy<DocumentStringLocalizer> localizer = new Lazy<DocumentStringLocalizer>();

            /// <summary>
            /// Gets or sets the culture name.
            /// </summary>
            public static string CultureName { get; set; } = "";

            /// <summary>
            /// Translates the specified text identifier according to the culture name.
            /// </summary>
            /// <param name="textId">The text identifier.</param>
            /// <returns>The localized string.</returns>
            public static string Translate(string textId)
                => localizer.Value.Translate(CultureName, textId);
        }
    }
}
