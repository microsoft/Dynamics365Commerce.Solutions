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
    namespace EFRSample.CommerceRuntime
    {
        using System.Threading.Tasks;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
        using Microsoft.Dynamics.Retail.Resources.Strings;
        using Contoso.CommerceRuntime.DocumentProvider.DataModelEFR.Constants;
        using Contoso.CommerceRuntime.DocumentProvider.EFRSample.Messages;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;

        /// <summary>
        /// The request handler for the <see cref="LocalizeEfrResourceRequest"/>.
        /// </summary>
        public sealed class LocalizeEfrResourceRequestHandler : SingleAsyncRequestHandler<LocalizeEfrResourceRequest>
        {
            private readonly StringLocalizer stringLocalizer;

            /// <summary>
            /// Initializes a new instance of <see cref="LocalizeEfrResourceRequestHandler"/>.
            /// </summary>
            public LocalizeEfrResourceRequestHandler()
            {
                this.stringLocalizer = CreateStringLocalizer();
            }

            /// <summary>
            /// Processes the request.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <returns>The response.</returns>
            protected override Task<Response> Process(LocalizeEfrResourceRequest request)
            {
                string fullTextId = SalesTransactionLocalizationConstants.LocalizationResourcePrefix + request.TextId;
                string result = stringLocalizer.Translate(request.CultureName, fullTextId);
                return Task.FromResult<Response>(new SingleEntityDataServiceResponse<string>(result));
            }

            /// <summary>
            /// Creates a string localizer.
            /// </summary>
            /// <returns>The string localizer.</returns>
            private static StringLocalizer CreateStringLocalizer()
            {
                string resourceFileName = string.Format(SalesTransactionLocalizationConstants.LocalizationResourceFileNameTemplate, typeof(LocalizeEfrResourceRequestHandler).Namespace);
                return new StringLocalizer(resourceFileName, typeof(LocalizeEfrResourceRequestHandler).Assembly);
            }
        }
    }
}
