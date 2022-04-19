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
    namespace Commerce.Runtime.DocumentProvider.PosnetSample.Documents
    {
        using System.Runtime.Serialization;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;

        /// <summary>
        /// The transaction registration response for Posnet.
        /// </summary>
        [DataContract]
        internal class PosnetTransactionRegistrationResponse
        {
            /// <summary>
            /// Gets or sets the registration result.
            /// </summary>
            [DataMember]
            public FiscalIntegrationRegistrationResult RegistrationResult { get; set; }
        }
    }
}
