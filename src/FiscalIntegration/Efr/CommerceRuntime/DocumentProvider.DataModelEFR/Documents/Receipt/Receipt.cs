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
            /// The country region ISO code.
            /// </summary>
            [XmlIgnore]
            public CountryRegionISOCode CountryRegionISOCode { get; set; }

            /// <summary>
            /// The receipt DateTime.
            /// </summary>
            [XmlIgnore]
            public DateTime ReceiptDateTime { get; set; }

            /// <summary>
            /// The receipt DateTime formatted for serialization.
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
            /// Timestamp of the transaction start.
            /// </summary>
            [XmlIgnore]
            public DateTime TransactionStartDateTime { get; set; }

            /// <summary>
            /// The receipt TransactionStartDateTime formatted for serialization.
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
            /// The transaction location.
            /// </summary>
            [XmlAttribute(AttributeName = TransactionLocationAttributeName)]
            [DefaultValue("")]
            public string TransactionLocation { get; set; }

            /// <summary>
            /// The transaction terminal.
            /// </summary>
            [XmlAttribute(AttributeName = TransactionTerminalAttributeName)]
            [DefaultValue("")]
            public string TransactionTerminal { get; set; }

            /// <summary>
            /// The transaction number.
            /// </summary>
            [XmlAttribute(AttributeName = TransactionNumberAttributeName)]
            [DefaultValue("")]
            public string TransactionNumber { get; set; }

            /// <summary>
            /// The fiscal transaction id.
            /// </summary>
            [XmlAttribute(AttributeName = TransactionIdAttributeName)]
            [DefaultValue("")]
            public string TransactionId { get; set; }

            /// <summary>
            /// The total amount.
            /// </summary>
            [XmlIgnore]
            public decimal TotalAmount { get; set; }

            /// <summary>
            /// The total amount formatted for serialization.
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
            /// The operator id.
            /// </summary>
            [XmlAttribute(AttributeName = OperatorIdAttributeName)]
            [DefaultValue("")]
            public string OperatorId { get; set; }

            /// <summary>
            /// The operator name.
            /// </summary>
            [XmlAttribute(AttributeName = OperatorNameAttributeName)]
            [DefaultValue("")]
            public string OperatorName { get; set; }

            /// <summary>
            /// The non-fiscal transaction type.
            /// </summary>
            [XmlAttribute(AttributeName = NonFiscalTransactionTypeAttributeName)]
            [DefaultValue("")]
            public string NonFiscalTransactionType { get; set; }

            /// <summary>
            /// The non-fiscal signed transaction type.
            /// </summary>
            [XmlAttribute(AttributeName = NonFiscalSignedTransactionTypeAttributeName)]
            [DefaultValue("")]
            public string NonFiscalSignedTransactionType { get; set; }

            /// <summary>
            /// The training transaction flag.
            /// </summary>
            [XmlAttribute(AttributeName = IsTrainingTransactionAttributeName)]
            [DefaultValue(0)]
            public int IsTrainingTransaction { get; set; }

            /// <summary>
            /// The header.
            /// </summary>
            [XmlElement(ElementName = HeaderElementName)]
            public ReceiptHeader Header { get; set; }

            /// <summary>
            /// The array of position lines.
            /// </summary>
            [XmlElement(ElementName = PositionLinesElementName)]
            public ReceiptPositionLines PositionLines { get; set; }

            /// <summary>
            /// The array of payments.
            /// </summary>
            [XmlArray(ElementName = PaymentsElementName)]
            [XmlArrayItem(ElementName = PaymentElementName)]
            public List<ReceiptPayment> Payments { get; set; }

            /// <summary>
            /// Defines the payments serialization condition.
            /// </summary>
            [XmlIgnore]
            public bool PaymentsSpecified
            {
                get { return this.Payments?.Count != 0; }
            }

            /// <summary>
            /// The array of taxes.
            /// </summary>
            [XmlArray(ElementName = TaxesElementName)]
            [XmlArrayItem(ElementName = TaxElementName)]
            public List<ReceiptTax> Taxes { get; set; }

            /// <summary>
            /// Defines the taxes serialization condition.
            /// </summary>
            [XmlIgnore]
            public bool TaxesSpecified
            {
                get { return this.Taxes?.Count != 0; }
            }

            /// <summary>
            /// The footer.
            /// </summary>
            [XmlElement(ElementName = FooterElementName)]
            public ReceiptFooter Footer { get; set; }

            /// <summary>
            /// The corresponding Business premises ID.
            /// </summary>
            [XmlAttribute(AttributeName = RegistrationIdAttributeName)]
            [DefaultValue("")]
            public string RegistrationId { get; set; }

            /// <summary>
            /// The customer data.
            /// </summary>
            [XmlElement(ElementName = CustomerAttributeName)]
            public ReceiptCustomer Customer { get; set; }

            /// <summary>
            /// Indicates that taxes are not included in price.
            /// </summary>
            [XmlIgnore]
            public bool NetTaxFlag { get; set; }

            /// <summary>
            /// The receipt NetTaxFlag formatted for serialization.
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
