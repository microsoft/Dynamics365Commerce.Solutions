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

        /// <summary>
        /// Represents a tax information.
        /// </summary>
        [DataContract]
        public class Tax
        {
            /// <summary>
            /// Gets or sets tax type.
            /// </summary>
            [IgnoreDataMember]
            public TaxType TaxType { get; set; }

            /// <summary>
            /// Gets or sets tax type text.
            /// </summary>
            [DataMember(Name = "type")]
            public string TaxTypeStringValue
            {
                get
                {
                    return EnumMemberSerializer.Serialize(this.TaxType);
                }

                set
                {
                    this.TaxType = EnumMemberSerializer.Deserialize<TaxType>(value);
                }
            }

            /// <summary>
            /// Gets or sets tax amount.
            /// </summary>
            [DataMember(Name = "sum", EmitDefaultValue = false)]
            public decimal Sum { get; set; }
        }
    }
}
