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
        /// The receipt tax.
        /// </summary>
        [Serializable]
        public class ReceiptTax
        {
            private const string TaxGroupAttributeName = "TaxG";
            private const string TaxPercentAttributeName = "Prc";
            private const string NetAmountAttributeName = "Net";
            private const string GrossAmountAttributeName = "Amt";
            private const string TaxAmountAttributeName = "TAmt";

            /// <summary>
            /// The tax group.
            /// </summary>
            [XmlAttribute(AttributeName = TaxGroupAttributeName)]
            [DefaultValue("")]
            public string TaxGroup { get; set; }

            /// <summary>
            /// The tax percent.
            /// </summary>
            [XmlIgnore]
            public decimal TaxPercent { get; set; }

            /// <summary>
            /// The tax percent formatted for serialization.
            /// </summary>
            [XmlAttribute(AttributeName = TaxPercentAttributeName)]
            public string TaxPercentStringValue
            {
                get
                {
                    return FormatHelper.FormatDecimal(TaxPercent);
                }

                set
                {
                    TaxPercent = decimal.Parse(value);
                }
            }

            /// <summary>
            /// The amount without tax.
            /// </summary>
            [XmlIgnore]
            public decimal NetAmount { get; set; }

            /// <summary>
            /// /// The amount without tax formatted for serialization.
            /// </summary>
            [XmlAttribute(AttributeName = NetAmountAttributeName)]
            public string NetAmountStringValue
            {
                get
                {
                    return FormatHelper.FormatDecimal(NetAmount);
                }

                set
                {
                    NetAmount = decimal.Parse(value);
                }
            }

            /// <summary>
            /// The tax amount.
            /// </summary>
            [XmlIgnore]
            public decimal TaxAmount { get; set; }

            /// <summary>
            /// The tax amount formatted for serialization.
            /// </summary>
            [XmlAttribute(AttributeName = TaxAmountAttributeName)]
            public string TaxAmountStringValue
            {
                get
                {
                    return FormatHelper.FormatDecimal(TaxAmount);
                }

                set
                {
                    TaxAmount = decimal.Parse(value);
                }
            }

            /// <summary>
            /// The amount with tax.
            /// </summary>
            [XmlIgnore]
            public decimal GrossAmount { get; set; }

            /// <summary>
            /// The amount with tax formatted for serialization.
            /// </summary>
            [XmlAttribute(AttributeName = GrossAmountAttributeName)]
            public string GrossAmountStringValue
            {
                get
                {
                    return FormatHelper.FormatDecimal(GrossAmount);
                }

                set
                {
                    GrossAmount = decimal.Parse(value);
                }
            }
        }
    }
}
