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
        /// Represents item position.
        /// </summary>
        [DataContract]
        public class PositionItem : BaseItem
        {
            /// <summary>
            /// Gets type of item.
            /// </summary>
            [DataMember(Name = "type")]
            public override string Type { get { return "position"; } }

            /// <summary>
            /// Gets or sets the name of the product.
            /// </summary>
            [DataMember(Name = "name")]
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets price.
            /// </summary>
            [DataMember(Name = "price")]
            public decimal Price { get; set; }

            /// <summary>
            /// Gets or sets quantity.
            /// </summary>
            [DataMember(Name = "quantity")]
            public decimal Quantity { get; set; }

            /// <summary>
            /// Gets or sets total amount.
            /// </summary>
            [DataMember(Name = "amount")]
            public decimal Amount { get; set; }

            /// <summary>
            /// Gets or sets information about a discount.
            /// </summary>
            [DataMember(Name = "infoDiscountAmount", EmitDefaultValue = false)]
            public decimal InfoDiscount { get; set; }

            /// <summary>
            /// Gets or sets department.
            /// </summary>
            [DataMember(Name = "department", EmitDefaultValue = false)]
            public int Department { get; set; }

            /// <summary>
            /// Gets or sets units of measurement.
            /// </summary>
            [DataMember(Name = "measurementUnit", EmitDefaultValue = false)]
            public string MeasurementUnit { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether piece flag.
            /// </summary>
            [DataMember(Name = "piece")]
            public bool Piece { get; set; }

            /// <summary>
            /// Gets or sets payment method.
            /// </summary>
            [IgnoreDataMember]
            public PositionPaymentMethod PaymentMethod { get; set; }

            /// <summary>
            /// Gets or sets payment method text.
            /// </summary>
            [DataMember(Name = "paymentMethod", EmitDefaultValue = false)]
            public string PaymentMethodStringValue
            {
                get
                {
                    return EnumMemberSerializer.Serialize(this.PaymentMethod);
                }

                set
                {
                    this.PaymentMethod = EnumMemberSerializer.Deserialize<PositionPaymentMethod>(value);
                }
            }

            /// <summary>
            /// Gets or sets payment object type.
            /// </summary>
            [IgnoreDataMember]
            public PaymentObjectType PaymentObject { get; set; }

            /// <summary>
            /// Gets or sets payment object type text.
            /// </summary>
            [DataMember(Name = "paymentObject")]
            public string PaymentObjectStringValue
            {
                get
                {
                    return EnumMemberSerializer.Serialize(this.PaymentObject);
                }

                set
                {
                    this.PaymentObject = EnumMemberSerializer.Deserialize<PaymentObjectType>(value);
                }
            }

            /// <summary>
            /// Gets or sets nomenclature code.
            /// </summary>
            [DataMember(Name = "nomenclatureCode", EmitDefaultValue = false)]
            public NomenclatureCode NomenclatureCode { get; set; }

            /// <summary>
            /// Gets or sets marking code.
            /// </summary>
            [DataMember(Name = "markingCode", EmitDefaultValue = false)]
            public MarkingCode MarkingCode { get; set; }

            /// <summary>
            /// Gets or sets information about the agent.
            /// </summary>
            [DataMember(Name = "agentInfo", EmitDefaultValue = false)]
            public Agent Agent { get; set; }

            /// <summary>
            /// Gets or sets information about the supplier.
            /// </summary>
            [DataMember(Name = "supplierInfo", EmitDefaultValue = false)]
            public Supplier Supplier { get; set; }

            /// <summary>
            /// Gets or sets tax information.
            /// </summary>
            [DataMember(Name = "tax")]
            public Tax Tax { get; set; }

            /// <summary>
            /// Gets or sets additional details of the subject of payment.
            /// </summary>
            [DataMember(Name = "additionalAttribute", EmitDefaultValue = false)]
            public string AdditionalAttribute { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether to print additional details of the subject of payment.
            /// </summary>
            [DataMember(Name = "additionalAttributePrint", EmitDefaultValue = false)]
            public bool AdditionalAttributePrint { get; set; }

            /// <summary>
            /// Gets or sets value of the excise tax.
            /// </summary>
            [DataMember(Name = "exciseSum", EmitDefaultValue = false)]
            public decimal ExciseSum { get; set; }

            /// <summary>
            /// Gets or sets code of the country of origin.
            /// </summary>
            [DataMember(Name = "countryCode", EmitDefaultValue = false)]
            public string CountryCode { get; set; }

            /// <summary>
            /// Gets or sets number of customs declaration.
            /// </summary>
            [DataMember(Name = "customsDeclaration", EmitDefaultValue = false)]
            public string CustomsDeclaration { get; set; }

            /// <summary>
            /// Gets or sets custom parameter value 3.
            /// </summary>
            [DataMember(Name = "userParam3", EmitDefaultValue = false)]
            public int UserParam3 { get; set; }

            /// <summary>
            /// Gets or sets custom parameter value 4.
            /// </summary>
            [DataMember(Name = "userParam4", EmitDefaultValue = false)]
            public int UserParam4 { get; set; }

            /// <summary>
            /// Gets or sets custom parameter value 5.
            /// </summary>
            [DataMember(Name = "userParam5", EmitDefaultValue = false)]
            public int UserParam5 { get; set; }

            /// <summary>
            /// Gets or sets custom parameter value 6.
            /// </summary>
            [DataMember(Name = "userParam6", EmitDefaultValue = false)]
            public int UserParam6 { get; set; }
        }
    }
}
