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
    namespace CommerceRuntime.DocumentProvider.DataModelEFR.Documents
    {
        using System;
        using System.ComponentModel;
        using System.Xml.Serialization;
        using Contoso.CommerceRuntime.DocumentProvider.DataModelEFR.DocumentHelpers;

        /// <summary>
        /// The receipt payment.
        /// </summary>
        [Serializable]
        public class ReceiptPayment
        {
            private const string DescriptionAttributeName = "Dsc";
            private const string AmountAttributeName = "Amt";
            private const string UniqueIdentifierAttributeName = "UID";
            private const string PaymentTypeGroupAttributeName = "PayG";
            private const string ForeignCurrencyCodeAttributeName = "CC";
            private const string ForeignAmountAttributeName = "FAmt";
            private const string CZFieldAttributeName = "CZ_field";
            private const string VoucherIDAttributeName = "VouN";

            /// <summary>
            /// Gets or sets the description.
            /// </summary>
            [XmlAttribute(AttributeName = DescriptionAttributeName)]
            [DefaultValue("")]
            public string Description { get; set; }

            /// <summary>
            /// Gets or sets the amount.
            /// </summary>
            [XmlIgnore]
            public decimal Amount { get; set; }

            /// <summary>
            /// Gets or sets the amount formatted for serialization.
            /// </summary>
            [XmlAttribute(AttributeName = AmountAttributeName)]
            public string AmountStringValue
            {
                get
                {
                    return FormatHelper.FormatDecimal(Amount);
                }

                set
                {
                    Amount = decimal.Parse(value);
                }
            }

            /// <summary>
            /// Gets or sets the unique identifier.
            /// </summary>
            [XmlAttribute(AttributeName = UniqueIdentifierAttributeName)]
            [DefaultValue("")]
            public string UniqueIdentifier { get; set; }

            /// <summary>
            /// Gets or sets the payment type group.
            /// </summary>
            [XmlAttribute(AttributeName = PaymentTypeGroupAttributeName)]
            [DefaultValue("")]
            public string PaymentTypeGroup { get; set; }

            /// <summary>
            /// Gets or sets the CZ fields for transaction marking.
            /// </summary>
            [XmlAttribute(AttributeName = CZFieldAttributeName)]
            [DefaultValue(0)]
            public int CZField { get; set; }

            /// <summary>
            /// Gets or sets the amount of the foreign payment.
            /// </summary>
            [XmlIgnore]
            public decimal? ForeignAmount { get; set; }

            /// <summary>
            /// Gets or sets the amount formatted for serialization.
            /// </summary>
            [XmlAttribute(AttributeName = ForeignAmountAttributeName)]
            public string ForeignAmountValue
            {
                get
                {
                    return FormatHelper.FormatDecimal(ForeignAmount);
                }

                set
                {
                    ForeignAmount = decimal.Parse(value);
                }
            }

            /// <summary>
            /// Gets or sets the foreign currency code.
            /// </summary>
            [XmlAttribute(AttributeName = ForeignCurrencyCodeAttributeName)]
            public string ForeignCurrencyCode { get; set; }

            /// <summary>
            /// Gets or sets the identifier of the voucher.
            /// </summary>
            [XmlAttribute(AttributeName = VoucherIDAttributeName)]
            [DefaultValue("")]
            public string VoucherId { get; set; }
        }
    }
}
