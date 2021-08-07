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
        using System.Collections.Generic;
        using System.Runtime.Serialization;

        /// <summary>
        /// Represents a payment.
        /// </summary>
        [DataContract]
        public class Payment
        {
            /// <summary>
            /// Gets or sets payment method.
            /// </summary>
            [IgnoreDataMember]
            public PaymentMethodType PaymentMethod { get; set; }

            /// <summary>
            /// Gets or sets payment method text.
            /// </summary>
            [DataMember(Name = "type")]
            public string PaymentMethodStringValue
            {
                get
                {
                    return EnumMemberSerializer.Serialize(this.PaymentMethod);
                }

                set
                {
                    this.PaymentMethod = EnumMemberSerializer.Deserialize<PaymentMethodType>(value);
                }
            }

            /// <summary>
            /// Gets or sets payment amount.
            /// </summary>
            [DataMember(Name = "sum")]
            public decimal Sum { get; set; }

            /// <summary>
            /// Gets or sets list of non-fiscal items to print.
            /// </summary>
            [DataMember(Name = "printItems", EmitDefaultValue = false)]
            public ICollection<TextItem> PrintItems { get; set; }
        }
    }
}
