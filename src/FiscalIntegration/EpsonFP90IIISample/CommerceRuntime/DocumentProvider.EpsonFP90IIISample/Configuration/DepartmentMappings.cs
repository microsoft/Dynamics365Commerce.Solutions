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
        using System.Collections.Generic;
        using System.Runtime.Serialization;

        /// <summary>
        /// Represents a class for department number mappings.
        /// </summary>
        [DataContract]
        public class DepartmentMappings
        {
            /// <summary>
            /// Mapping department numbers to VAT rate, product type and VAT exempt nature.
            /// </summary>
            [DataMember]
            public List<FiscalPrinterDepartmentMappingItem> Departments { get; set; }
        }
    }
}