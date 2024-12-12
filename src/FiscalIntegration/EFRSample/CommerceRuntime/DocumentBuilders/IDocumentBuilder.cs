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
    namespace CommerceRuntime.DocumentProvider.EFRSample.DocumentBuilders
    {
        using System.Threading.Tasks;
        using Contoso.CommerceRuntime.DocumentProvider.DataModelEFR.Documents;

        /// <summary>
        /// The document builder interface.
        /// </summary>
        internal interface IDocumentBuilder
        {
            /// <summary>
            /// Builds fiscal integration document.
            /// </summary>
            /// <returns> The fiscal integration receipt document </returns>
            Task<IFiscalIntegrationDocument> BuildAsync();
        }
    }
}
