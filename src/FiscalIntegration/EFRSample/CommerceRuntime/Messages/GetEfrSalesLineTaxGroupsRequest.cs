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
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;

        /// <summary>
        /// The request to get EFR sales line tax groups.
        /// </summary>
        public sealed class GetEfrSalesLineTaxGroupsRequest : Request
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="GetEfrSalesLineTaxGroupsRequest"/> class.
            /// </summary>
            /// <param name="salesLine">The sales line.</param>
            /// <param name="fiscalIntegrationFunctionalityProfile">The fiscal integration functionality profile.</param>
            public GetEfrSalesLineTaxGroupsRequest(SalesLine salesLine, FiscalIntegrationFunctionalityProfile fiscalIntegrationFunctionalityProfile)
            {
                this.SalesLine = salesLine;
                this.FiscalIntegrationFunctionalityProfile = fiscalIntegrationFunctionalityProfile;
            }

            /// <summary>
            /// Gets the sales line.
            /// </summary>
            public SalesLine SalesLine { get; private set; }

            /// <summary>
            /// Gets the fiscal integration functionality profile.
            /// </summary>
            public FiscalIntegrationFunctionalityProfile FiscalIntegrationFunctionalityProfile { get; private set; }
        }
    }
}
