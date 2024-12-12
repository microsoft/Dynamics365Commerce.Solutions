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
    namespace CommerceRuntime.DocumentProvider.AtolSample.DataModel.AtolTask
    {
        using System.Runtime.Serialization;
        using Newtonsoft.Json;

        /// <summary>
        /// The fiscal parameters.
        /// </summary>
        [DataContract]
        public class FiscalParameters
        {
            private const string FiscalParametersJsonPropertyName = "fiscalDocumentSign";

            /// <summary>
            /// Gets or sets the fiscal document date and time in the following format: YYYY-MM-DDThh:mm:ssTZD.
            /// </summary>
            [DataMember]
            public string FiscalDocumentDateTime { get; set; }

            /// <summary>
            /// Gets or sets the fiscal document number.
            /// </summary>
            [DataMember]
            public int FiscalDocumentNumber { get; set; }

            /// <summary>
            /// Gets or sets the fiscal document signature. Fiscal document signature is a 10-digit sequence.
            /// </summary>
            [DataMember]
            [JsonProperty(FiscalParametersJsonPropertyName)]
            public string FiscalDocumentSignature { get; set; }

            /// <summary>
            /// Gets or sets the fiscal receipt number.
            /// </summary>
            [DataMember]
            public int FiscalReceiptNumber { get; set; }

            /// <summary>
            /// Gets or sets the FN number.
            /// </summary>
            [DataMember]
            public string FnNumber { get; set; }

            /// <summary>
            /// Gets or sets the registration number.
            /// </summary>
            [DataMember]
            public string RegistrationNumber { get; set; }
        }
    }
}
