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
    namespace CommerceRuntime.DocumentProvider.AtolSample.DataModel.Configuration
    {
        using System.Collections.Generic;
        using System.Runtime.Serialization;
        using Contoso.CommerceRuntime.DocumentProvider.AtolSample.DataModel.AtolTask;

        /// <summary>
        /// Represents document provider settings.
        /// </summary>
        [DataContract]
        public class DocumentProviderSettings
        {
            /// <summary>
            /// Gets or sets payment type mapping.
            /// </summary>
            [DataMember]
            public IDictionary<string, PaymentMethodType> PaymentTypeMapping { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the shift close shift report should be generated electronically only.
            /// </summary>
            [DataMember]
            public bool GenerateCloseShiftReportElectronically { get; set; }
        }
    }
}
