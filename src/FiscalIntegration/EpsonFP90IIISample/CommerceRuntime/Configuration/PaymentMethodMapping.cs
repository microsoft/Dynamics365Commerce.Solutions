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
        /// Represents a class for payment methods mapping.
        /// </summary>
        [DataContract]
        public class PaymentMethodMapping
        {
            /// <summary>
            /// List with mapping Store Payment methods to Printer types and indexes.
            /// </summary>
            [DataMember]
            public List<PaymentMethod> PaymentMethods { get; set; }

            /// <summary>
            /// Mapping Deposit payment method to printer type and index.
            /// </summary>
            [DataMember]
            public PaymentMethod DepositPaymentMethod { get; set; }
        }
    }
}
