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
        using System.Collections.Generic;
        using System.ComponentModel;
        using System.Xml.Serialization;
        using Contoso.CommerceRuntime.DocumentProvider.DataModelEFR.DocumentHelpers;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;

        /// <summary>
        /// The receipt document.
        /// </summary>
        [Serializable]
        public class Receipt
        {
            private const string DateTimeAttributeName = "D";
            private const string TransactionStartDateTimeAttributeName = "D0";
            private const string TransactionLocationAttributeName = "TL";
            private const string TransactionTerminalAttributeName = "TT";
            private const string TransactionNumberAttributeName = "TN";
            private const string TransactionIdAttributeName = "TID";
            private const string TotalAmountAttributeName = "T";
            private const string OperatorIdAttributeName = "Opr";
            private const string OperatorNameAttributeName = "OprN";
            private const string NonFiscalTransactionTypeAttributeName = "NF";
            private const string NonFiscalSignedTransactionTypeAttributeName = "NFS";
            private const string IsTrainingTransactionAttributeName = "test";
            private const string HeaderElementName = "Head";
            private const string PositionLinesElementName = "PosA";
            private const string FooterElementName = "Foot";
            private const string PaymentsElementName = "PayA";
            private const string PaymentElementName = "Pay";
            private const string TaxesElementName = "TaxA";
            private const string TaxElementName = "Tax";
            private const string RegistrationIdAttributeName = "CZ_id_provoz";
            private const string CustomerAttributeName = "Ctm";
            private const string NetTaxFlagAttributeName = "TaxN";
            private const string TrueStringValue = "1";

            /// <summary>
            /// Gets or sets the country region ISO code.
            /// </summary>
            [XmlIgnore]
            public CountryRegionISOCode CountryRegionISOCode { get; set; }

            /// <summary>
            /// Gets or sets the receipt DateTime.
            /// </summary>
            [XmlIgnore]
            public DateTime ReceiptDateTime { get; set; }

            /// <summary>
            /// Gets or sets the receipt DateTime formatted for serialization.
            /// </summary>
            [XmlAttribute(AttributeName = DateTimeAttributeName)]
            [DefaultValue("")]
            public string ReceiptDateTimeStringValue
            {
                get
                {
                    return FormatHelper.FormatDateTime(ReceiptDateTime);
                }

                set
                {
                    ReceiptDateTime = DateTime.Parse(value);
                }
            }

            /// <summary>
            /// Gets or sets timestamp of the transaction start.
            /// </summary>
            [XmlIgnore]
            public DateTime TransactionStartDateTime { get; set; }

            /// <summary>
            /// Gets or sets the receipt TransactionStartDateTime formatted for serialization.
            /// </summary>
            [XmlAttribute(AttributeName = TransactionStartDateTimeAttributeName)]
            [DefaultValue("")]
            public string TransactionStartDateTimeStringValue
            {
                get
                {
                    return FormatHelper.FormatDateTime(TransactionStartDateTime);
                }

                set
                {
                    TransactionStartDateTime = DateTime.Parse(value);
                }
            }

            /// <summary>
            /// Gets or sets the transaction location.
            /// </summary>
            [XmlAttribute(AttributeName = TransactionLocationAttributeName)]
            [DefaultValue("")]
            public string TransactionLocation { get; set; }

            /// <summary>
            /// Gets or sets the transaction terminal.
            /// </summary>
            [XmlAttribute(AttributeName = TransactionTerminalAttributeName)]
            [DefaultValue("")]
            public string TransactionTerminal { get; set; }

            /// <summary>
            /// Gets or sets the transaction number.
            /// </summary>
            [XmlAttribute(AttributeName = TransactionNumberAttributeName)]
            [DefaultValue("")]
            public string TransactionNumber { get; set; }

            /// <summary>
            /// Gets or sets the fiscal transaction id.
            /// </summary>
            [XmlAttribute(AttributeName = TransactionIdAttributeName)]
            [DefaultValue("")]
            public string TransactionId { get; set; }

            /// <summary>
            /// Gets or sets the total amount.
            /// </summary>
            [XmlIgnore]
            public decimal TotalAmount { get; set; }

            /// <summary>
            /// Gets or sets the total amount formatted for serialization.
            /// </summary>
            [XmlAttribute(AttributeName = TotalAmountAttributeName)]
            public string TotalAmountStringValue
            {
                get
                {
                    return FormatHelper.FormatDecimal(TotalAmount);
                }

                set
                {
                    TotalAmount = decimal.Parse(value);
                }
            }

            /// <summary>
            /// Gets or sets the operator identifier.
            /// </summary>
            [XmlAttribute(AttributeName = OperatorIdAttributeName)]
            [DefaultValue("")]
            public string OperatorId { get; set; }

            /// <summary>
            /// Gets or sets the operator name.
            /// </summary>
            [XmlAttribute(AttributeName = OperatorNameAttributeName)]
            [DefaultValue("")]
            public string OperatorName { get; set; }

            /// <summary>
            /// Gets or sets the non-fiscal transaction type.
            /// </summary>
            [XmlAttribute(AttributeName = NonFiscalTransactionTypeAttributeName)]
            [DefaultValue("")]
            public string NonFiscalTransactionType { get; set; }

            /// <summary>
            /// Gets or sets the non-fiscal signed transaction type.
            /// </summary>
            [XmlAttribute(AttributeName = NonFiscalSignedTransactionTypeAttributeName)]
            [DefaultValue("")]
            public string NonFiscalSignedTransactionType { get; set; }

            /// <summary>
            /// Gets or sets the training transaction flag.
            /// </summary>
            [XmlAttribute(AttributeName = IsTrainingTransactionAttributeName)]
            [DefaultValue(0)]
            public int IsTrainingTransaction { get; set; }

            /// <summary>
            /// Gets or sets the header.
            /// </summary>
            [XmlElement(ElementName = HeaderElementName)]
            public ReceiptHeader Header { get; set; }

            /// <summary>
            /// Gets or sets the array of position lines.
            /// </summary>
            [XmlElement(ElementName = PositionLinesElementName)]
            public ReceiptPositionLines PositionLines { get; set; }

            /// <summary>
            /// Gets or sets the array of payments.
            /// </summary>
            [XmlArray(ElementName = PaymentsElementName)]
            [XmlArrayItem(ElementName = PaymentElementName)]
            public List<ReceiptPayment> Payments { get; set; }

            /// <summary>
            /// Gets a value indicating whether the payments serialization condition is true.
            /// </summary>
            [XmlIgnore]
            public bool PaymentsSpecified
            {
                get { return this.Payments?.Count != 0; }
            }

            /// <summary>
            /// Gets or sets the array of taxes.
            /// </summary>
            [XmlArray(ElementName = TaxesElementName)]
            [XmlArrayItem(ElementName = TaxElementName)]
            public List<ReceiptTax> Taxes { get; set; }

            /// <summary>
            /// Gets a value indicating whether the taxes serialization condition is true.
            /// </summary>
            [XmlIgnore]
            public bool TaxesSpecified
            {
                get { return this.Taxes?.Count != 0; }
            }

            /// <summary>
            /// Gets or sets the footer.
            /// </summary>
            [XmlElement(ElementName = FooterElementName)]
            public ReceiptFooter Footer { get; set; }

            /// <summary>
            /// Gets or sets the corresponding Business premises identifier.
            /// </summary>
            [XmlAttribute(AttributeName = RegistrationIdAttributeName)]
            [DefaultValue("")]
            public string RegistrationId { get; set; }

            /// <summary>
            /// Gets or sets the customer data.
            /// </summary>
            [XmlElement(ElementName = CustomerAttributeName)]
            public ReceiptCustomer Customer { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether taxes are not included in price.
            /// </summary>
            [XmlIgnore]
            public bool NetTaxFlag { get; set; }

            /// <summary>
            /// Gets or sets the receipt NetTaxFlag formatted for serialization.
            /// </summary>
            [XmlAttribute(AttributeName = NetTaxFlagAttributeName)]
            [DefaultValue("")]
            public string NetTaxFlagStringValue
            {
                get => NetTaxFlag ? TrueStringValue : string.Empty;
                set => NetTaxFlag = value == TrueStringValue;
            }
        }
    }
}
