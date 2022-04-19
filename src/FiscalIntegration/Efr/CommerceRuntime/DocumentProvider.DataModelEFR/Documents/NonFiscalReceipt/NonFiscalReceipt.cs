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
            [XmlAttribute(AttributeName = ReceiptDateTimeAttributeName)]
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
        }
    }
}
