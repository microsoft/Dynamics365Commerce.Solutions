

namespace Contoso
{
    namespace CommerceRuntime.DocumentProvider.DataModelEFR.Documents
    {
        using System;
        using System.ComponentModel;
        using System.Xml.Serialization;
        using Contoso.CommerceRuntime.DocumentProvider.DataModelEFR.DocumentHelpers;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;

        /// <summary>
        /// The receipt document.
        /// </summary>
        [Serializable]
        public class NonFiscalReceipt
        {
            private const string ReceiptDateTimeAttributeName = "D";
            private const string TransactionLocationAttributeName = "TL";
            private const string TransactionTerminalAttributeName = "TT";
            private const string OperatorIdAttributeName = "Opr";
            private const string OperatorNameAttributeName = "OprN";
            private const string NonFiscalTransactionTypeAttributeName = "NF";
            private const string NonFiscalSignedTransactionTypeAttributeName = "NFS";
            private const string IsTrainingTransactionAttributeName = "test";

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
            [XmlAttribute(AttributeName = ReceiptDateTimeAttributeName)]
            [DefaultValue("")]
            public string ReceiptDateTimeStringValue {
                get {
                    return FormatHelper.FormatDateTime(ReceiptDateTime);
                }

                set {
                    ReceiptDateTime = DateTime.Parse(value);
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
        }
    }
}
