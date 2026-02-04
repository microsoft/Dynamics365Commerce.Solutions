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
        /// Represents a receipt.
        /// </summary>
        [DataContract]
        [KnownType(typeof(PositionItem))]
        [KnownType(typeof(TextItem))]
        [KnownType(typeof(BarcodeItem))]
        public class Receipt
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Receipt"/> class.
            /// </summary>
            public Receipt()
            {
                this.Initialize();
            }

            /// <summary>
            /// Gets or sets type of receipt.
            /// </summary>
            [IgnoreDataMember]
            public ReceiptType Type { get; set; }

            /// <summary>
            /// Gets or sets type of receipt as a string.
            /// </summary>
            [DataMember(Name = "type")]
            public string TypeStringValue
            {
                get
                {
                    return EnumMemberSerializer.Serialize(this.Type);
                }

                set
                {
                    this.Type = EnumMemberSerializer.Deserialize<ReceiptType>(value);
                }
            }

            /// <summary>
            /// Gets or sets a value indicating whether the ignore error flag when printing non-fiscal elements from items.
            /// </summary>
            [DataMember(Name = "ignoreNonFiscalPrintErrors", EmitDefaultValue = false)]
            public bool IgnoreNonFiscalPrintErrors { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether electronic receipt flag.
            /// </summary>
            [DataMember(Name = "electronically", EmitDefaultValue = false)]
            public bool Electronically { get; set; }

            /// <summary>
            /// Gets or sets types of taxation.
            /// </summary>
            [DataMember(Name = "taxationType", EmitDefaultValue = false)]
            public string TaxationType { get; set; }

            /// <summary>
            /// Gets or sets place of payment.
            /// </summary>
            [DataMember(Name = "paymentsPlace", EmitDefaultValue = false)]
            public string PaymentsPlace { get; set; }

            /// <summary>
            /// Gets or sets machine number.
            /// </summary>
            [DataMember(Name = "machineNumber", EmitDefaultValue = false)]
            public string MachineNumber { get; set; }

            /// <summary>
            /// Gets or sets information about the operator.
            /// </summary>
            [DataMember(Name = "operator", EmitDefaultValue = false)]
            public Operator Operator { get; set; }

            /// <summary>
            /// Gets or sets information about the customer.
            /// </summary>
            [DataMember(Name = "clientInfo", EmitDefaultValue = false)]
            public Customer Customer { get; set; }

            /// <summary>
            /// Gets or sets information about the Ccompany.
            /// </summary>
            [DataMember(Name = "companyInfo", EmitDefaultValue = false)]
            public Company Company { get; set; }

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
            /// Gets or sets list items.
            /// </summary>
            [DataMember(Name = "items")]
            public ICollection<BaseItem> Items { get; set; }

            /// <summary>
            /// Gets or sets list of payments.
            /// </summary>
            [DataMember(Name = "payments")]
            public ICollection<Payment> Payments { get; set; }

            /// <summary>
            /// Gets or sets list of taxes.
            /// </summary>
            [DataMember(Name = "taxes", EmitDefaultValue = false)]
            public ICollection<Tax> Taxes { get; set; }

            /// <summary>
            /// Gets or sets total amount.
            /// </summary>
            [DataMember(Name = "total", EmitDefaultValue = false)]
            public decimal Total { get; set; }

            /// <summary>
            /// Gets or sets list of non-fiscal elements to print in the footer.
            /// </summary>
            [DataMember(Name = "preItems", EmitDefaultValue = false)]
            public ICollection<BaseItem> PreItems { get; set; }

            /// <summary>
            /// Gets or sets list of non-fiscal elements to print in the header.
            /// </summary>
            [DataMember(Name = "postItems", EmitDefaultValue = false)]
            public ICollection<BaseItem> PostItems { get; set; }

            /// <summary>
            /// Initializes default property values.
            /// </summary>
            /// <remarks>Being called from constructor and on deserialize.</remarks>
            private void Initialize()
            {
                this.Items = new List<BaseItem>();
                this.Payments = new List<Payment>();
                this.PostItems = new List<BaseItem>();
            }
        }
    }
}
