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
    namespace CommerceRuntime.DocumentProvider.SequentialSignatureNorwaySample
    {
        using System.Runtime.Serialization;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;

        /// <summary>
        /// Fiscal register result.
        /// </summary>
        [DataContract]
        public class FiscalRegisterResult : CommerceEntity
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="FiscalRegisterResult"/> class.
            /// </summary>
            public FiscalRegisterResult()
                : base(nameof(FiscalRegisterResult))
            {
            }

            /// <summary>
            /// Gets or sets the data to sign.
            /// </summary>
            [DataMember]
            public string DataToSign { get; set; }

            /// <summary>
            /// Gets or sets the key thumbprint used for signing.
            /// </summary>
            [DataMember]
            public string KeyThumbprint { get; set; }

            /// <summary>
            /// Gets or sets the sequential number.
            /// </summary>
            [DataMember]
            public long SequentialNumber { get; set; }

            /// <summary>
            /// Gets or sets the signature.
            /// </summary>
            [DataMember]
            public string Signature { get; set; }
        }
    }
}
