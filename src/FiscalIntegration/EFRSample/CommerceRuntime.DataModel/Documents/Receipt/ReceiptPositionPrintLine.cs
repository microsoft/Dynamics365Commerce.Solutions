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

        /// <summary>
        /// The receipt position print line.
        /// </summary>
        [Serializable]
        public class ReceiptPositionPrintLine
        {
            private const string PositionNumberAttributeName = "PN";
            private const string DescriptionAttributeName = "Dsc";
            private const string LineAmountAttributeName = "LAmt";

            /// <summary>
            /// Gets or sets the reference position number.
            /// </summary>
            [XmlAttribute(AttributeName = PositionNumberAttributeName)]
            [DefaultValue(0)]
            public int PositionNumber { get; set; }

            /// <summary>
            /// Gets or sets the description.
            /// </summary>
            [XmlAttribute(AttributeName = DescriptionAttributeName)]
            [DefaultValue("")]
            public string Description { get; set; }

            /// <summary>
            /// Gets or sets the print text.
            /// </summary>
            [XmlAttribute(AttributeName = LineAmountAttributeName)]
            [DefaultValue("")]
            public string LineAmountTxt { get; set; }
        }
    }
}
