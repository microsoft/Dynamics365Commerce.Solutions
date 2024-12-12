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

        public static class DocumentLocalizerHelper
        {
            private static readonly Lazy<DocumentStringLocalizer> localizer = new Lazy<DocumentStringLocalizer>();

            public static string CultureName { get; set; } = "";

            /// <summary>
            /// Translates the specified text identifier according to culture name.
            /// </summary>
            /// <param name="cultureName">The name of the culture.</param>
            /// <param name="textId">The text identifier.</param>
            /// <returns>The localized string.</returns>
            public static string Translate(string textId)
            {
                return localizer.Value.Translate(CultureName, textId);
            }
        }
    }
}
