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
    namespace CommerceRuntime.DocumentProvider.EpsonFP90IIISample
    {
        using System.Runtime.Serialization;

        /// <summary>
        /// Represents a class for mapping department numbers to VAT rate, product type and VAT exempt nature.
        /// </summary>
        [DataContract]
        public class FiscalPrinterDepartmentMappingItem
        {
            /// <summary>
            /// Product type attribute mapping value.
            /// </summary>
            [DataMember]
            public string ProductType { get; set; }

            /// <summary>
            /// VAT rate attribute mapping value.
            /// </summary>
            [DataMember]
            public string VATRate { get; set; }

            /// <summary>
            /// VAT exempt nature attribute mapping value.
            /// </summary>
            [DataMember]
            public string VATExemptNature { get; set; }

            /// <summary>
            /// Department number attribute mapping value.
            /// </summary>
            [DataMember]
            public string DepartmentNumber { get; set; }
        }
    }
}