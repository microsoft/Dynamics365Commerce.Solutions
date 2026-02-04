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
        using System.Runtime.Serialization;
        using Contoso.CommerceRuntime.DocumentProvider.AtolSample.DataModel.AtolTask;

        /// <summary>
        /// Represents a class for payment method mapping.
        /// </summary>
        [DataContract]
        public class PaymentMethod
        {
            /// <summary>
            /// Gets or sets store payment method.
            /// </summary>
            [DataMember]
            public string StorePaymentMethod { get; set; }

            /// <summary>
            /// Gets or sets printer PaymentType attribute mapping value.
            /// </summary>
            [DataMember]
            public PaymentMethodType AtolPaymentType { get; set; }
        }
    }
}