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
        /// The request to get the EFR tender type name.
        /// </summary>
        public sealed class GetEfrGetTenderTypeNameRequest : Request
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="GetEfrGetTenderTypeNameRequest"/> class.
            /// </summary>
            /// <param name="tenderTypeId">The tender type identifier.</param>
            public GetEfrGetTenderTypeNameRequest(string tenderTypeId)
            {
                this.TenderTypeId = tenderTypeId;
            }

            /// <summary>
            /// Gets the tender type identifier.
            /// </summary>
            public string TenderTypeId { get; private set; }
        }
    }
}
