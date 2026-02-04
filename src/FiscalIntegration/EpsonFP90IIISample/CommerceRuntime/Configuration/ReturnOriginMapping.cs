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
        /// Represents a class for mapping serial number of the printer to return origin and serial number for non-fiscal transaction.
        /// </summary>
        [DataContract]
        public class ReturnOriginMapping
        {
            /// <summary>
            /// List with mapping return origin to printer return origin.
            /// </summary>
            [DataMember]
            public List<ReturnOriginMappingItem> ReturnOrigins { get; set; }

            /// <summary>
            /// The serial number of the printer that issued the original document which is used in the absence of fiscal data.
            /// </summary>
            [DataMember]
            public string PrinterReturnOriginWithoutFiscalData { get; set; }
        }
    }
}
